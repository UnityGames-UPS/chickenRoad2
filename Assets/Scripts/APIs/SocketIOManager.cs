using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Best.SocketIO;
using Best.SocketIO.Events;
using DG.Tweening;

public class SocketIOManager : MonoBehaviour
{
  [SerializeField] private SlotBehaviour slotManager;
  [SerializeField] private UIManager uiManager;
  [SerializeField] internal JSFunctCalls JSManager;
  [SerializeField] private string testToken;
  [SerializeField] private GameObject RaycastBlocker;
  internal List<string> bonusdata = null;
  internal GameData InitialData = null;

  internal Root ResultData = null;
  internal Player PlayerData = null;
  internal bool isResultdone = false;
  internal bool SetInit = false;

  private SocketManager manager;
  protected string SocketURI = null;
  // protected string TestSocketURI = "https://game-crm-rtp-backend.onrender.com/";
  protected string TestSocketURI = "https://devrealtime.dingdinghouse.com/";
  protected string nameSpace = "playground";
  private Socket gameSocket;
  protected string gameID = "SL-VIK";
  //protected string gameID = "";
  private const int maxReconnectionAttempts = 6;
  private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(10);
  string myAuth = null;

  private bool isConnected = false; //Back2 Start
  private bool hasEverConnected = false;
  private const int MaxReconnectAttempts = 5;
  private const float ReconnectDelaySeconds = 2f;

  private float lastPongTime = 0f;
  private float pingInterval = 2f;
  private float pongTimeout = 3f;
  private bool waitingForPong = false;
  private int missedPongs = 0;
  private const int MaxMissedPongs = 5;
  private Coroutine PingRoutine; //Back2 end
  private void Awake()
  {
    //Debug.unityLogger.logEnabled = false;
    SetInit = false;
  }

  private void Start()
  {
    OpenSocket();
  }

  void CloseGame()
  {
    Debug.Log("Unity: Closing Game");
    StartCoroutine(CloseSocket());
  }

  void ReceiveAuthToken(string jsonData)
  {
    Debug.Log("Received data: " + jsonData);
    var data = JsonUtility.FromJson<AuthTokenData>(jsonData);
    SocketURI = data.socketURL;
    myAuth = data.cookie;
    nameSpace = data.nameSpace;
  }

  private void OpenSocket()
  {
    //Create and setup SocketOptions
    SocketOptions options = new SocketOptions(); //Back2 Start
    options.AutoConnect = false;
    options.Reconnection = false;
    options.Timeout = TimeSpan.FromSeconds(3); //Back2 end
    options.ConnectWith = Best.SocketIO.Transports.TransportTypes.WebSocket;

#if UNITY_WEBGL && !UNITY_EDITOR
        JSManager.SendCustomMessage("authToken");
        StartCoroutine(WaitForAuthToken(options));
#else
    object authFunction(SocketManager manager, Socket socket)
    {
      return new
      {
        token = testToken
      };
    }
    options.Auth = authFunction;
    SetupSocketManager(options);
#endif
  }

  private IEnumerator WaitForAuthToken(SocketOptions options)
  {
    // Wait until myAuth is not null
    while (myAuth == null)
    {
      Debug.Log("My Auth is null");
      yield return null;
    }
    while (SocketURI == null)
    {
      Debug.Log("My Socket is null");
      yield return null;
    }
    Debug.Log("My Auth is not null");
    // Once myAuth is set, configure the authFunction
    object authFunction(SocketManager manager, Socket socket)
    {
      return new
      {
        token = myAuth
      };
    }
    options.Auth = authFunction;
    Debug.Log("Auth function configured with token: " + myAuth);

    // Proceed with connecting to the server
    SetupSocketManager(options);

    yield return null;
  }

  private void SetupSocketManager(SocketOptions options)
  {
    Debug.Log("Setup socket manager");
    // Create and setup SocketManager
#if UNITY_EDITOR
    this.manager = new SocketManager(new Uri(TestSocketURI), options);
#else
    this.manager = new SocketManager(new Uri(SocketURI), options);
#endif

    if (string.IsNullOrEmpty(nameSpace))
    {
      gameSocket = this.manager.Socket;
    }
    else
    {
      Debug.Log("nameSpace: " + nameSpace);
      gameSocket = this.manager.GetSocket("/" + nameSpace);
    }
    // Set subscriptions
    gameSocket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
    gameSocket.On(SocketIOEventTypes.Disconnect, OnDisconnected); //Back2 Start
    gameSocket.On<Error>(SocketIOEventTypes.Error, OnError);
    gameSocket.On<string>("game:init", OnListenEvent);
    gameSocket.On<string>("result", OnResult);
    gameSocket.On<string>("Play", OnPlay);
    gameSocket.On<string>("Go", OnGO);
    gameSocket.On<bool>("socketState", OnSocketState);
    gameSocket.On<string>("internalError", OnSocketError);
    gameSocket.On<string>("alert", OnSocketAlert);
    gameSocket.On<string>("pong", OnPongReceived); //Back2 Start
    gameSocket.On<string>("AnotherDevice", OnSocketOtherDevice);

    manager.Open(); //Back2 Start
  }

  // Connected event handler implementation
  void OnConnected(ConnectResponse resp) //Back2 Start
  {
    Debug.Log("✅ Connected to server.");

    if (hasEverConnected)
    {
      uiManager.CheckAndClosePopups();
    }

    isConnected = true;
    hasEverConnected = true;
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    SendPing();
  } //Back2 end

  private void OnDisconnected() //Back2 Start
  {
    //  Debug.LogWarning("⚠️ Disconnected from server.");
    isConnected = false;
    ResetPingRoutine();
    uiManager.DisconnectionPopup();
  } //Back2 end

  private void OnPongReceived(string data) //Back2 Start
  {
    //  Debug.Log("✅ Received pong from server.");
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    // Debug.Log($"⏱️ Updated last pong time: {lastPongTime}");
    // Debug.Log($"📦 Pong payload: {data}");
  } //Back2 end

  private void OnError(Error err)
  {
    Debug.LogError("Socket Error Message: " + err);
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("error");
#endif
  }

  void OnResult(string data)
  {
    ParseResponse(data);
  }
  void OnPlay(string data)
  {
    Play(data);
  }
  void OnGO(string data)
  {
    Go();
  }
  void OnCashOut(string data)
  {
    ParseResponse(data);
  }
  private void OnListenEvent(string data)
  {
    ParseResponse(data);
  }

  private void OnSocketState(bool state)
  {
    if (state)
    {
      Debug.Log("my state is " + state);
    }
  }
  private void OnSocketError(string data)
  {
    Debug.Log("Received error with data: " + data);
  }

  private void OnSocketAlert(string data)
  {
    Debug.Log("Received alert with data: " + data);
  }

  private void OnSocketOtherDevice(string data)
  {
    Debug.Log("Received Device Error with data: " + data);
    uiManager.ADfunction();
  }

  private void SendPing() //Back2 Start
  {
    ResetPingRoutine();
    PingRoutine = StartCoroutine(PingCheck());
  }

  void ResetPingRoutine()
  {
    if (PingRoutine != null)
    {
      StopCoroutine(PingRoutine);
    }
    PingRoutine = null;
  }

  private IEnumerator PingCheck()
  {
    while (true)
    {
      //  Debug.Log($"🟡 PingCheck | waitingForPong: {waitingForPong}, missedPongs: {missedPongs}, timeSinceLastPong: {Time.time - lastPongTime}");

      if (missedPongs == 0)
      {
        uiManager.CheckAndClosePopups();
      }

      // If waiting for pong, and timeout passed
      if (waitingForPong)
      {
        if (missedPongs == 2)
        {
          uiManager.ReconnectionPopup();
        }
        missedPongs++;
        //  Debug.LogWarning($"⚠️ Pong missed #{missedPongs}/{MaxMissedPongs}");

        if (missedPongs >= MaxMissedPongs)
        {
          //  Debug.LogError("❌ Unable to connect to server — 5 consecutive pongs missed.");
          isConnected = false;
          uiManager.DisconnectionPopup();
          yield break;
        }
      }

      // Send next ping
      waitingForPong = true;
      lastPongTime = Time.time;
      //      Debug.Log("📤 Sending ping...");
      SendDataWithNamespace("ping");
      yield return new WaitForSeconds(pingInterval);
    }
  } //Back2 end
  internal void SendDataWithNamespace(string eventName, string json = null)
  {
    // Send the message
    if (gameSocket != null && gameSocket.IsOpen)
    {
      if (json != null)
      {
        gameSocket.Emit(eventName, json);
        Debug.Log("JSON data sent: " + json);
      }
      else
      {
        gameSocket.Emit(eventName);
      }
    }
    else
    {
      Debug.LogWarning("Socket is not connected.");
    }
  }

  internal IEnumerator CloseSocket() //Back2 Start
  {
    RaycastBlocker.SetActive(true);
    ResetPingRoutine();

    Debug.Log("Closing Socket");

    manager?.Close();
    manager = null;

    Debug.Log("Waiting for socket to close");

    yield return new WaitForSeconds(0.5f);

    Debug.Log("Socket Closed");

#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnExit"); //Telling the react platform user wants to quit and go back to homepage
#endif
  } //Back2 end

  private void ParseResponse(string jsonObject)
  {
    Debug.Log(jsonObject);
    Root myData = JsonConvert.DeserializeObject<Root>(jsonObject);

    string id = myData.id;

    switch (id)
    {
      case "initData":
        {
          InitialData = myData.gameData;
          PlayerData = myData.player;


          if (!SetInit)
          {

            RefreshUI();
            SetInit = true;
          }
          else
          {
            RefreshUI();
          }
          break;
        }
      case "Play":
        {
          ResultData = myData;
          PlayerData = myData.player;
          isResultdone = true;
          Play(myData.payload.difficulty);
          break;
        }
      case "Go":
        {
          ResultData = myData;
          PlayerData = myData.player;
          isResultdone = true;
          Go();
          break;
        }
      case "Cashout":
        {
          ResultData = myData;
          PlayerData = myData.player;
          isResultdone = true;
          CashOut();
          break;
        }
    }
  }
  private void Play(string jsonObject)
  {



    AccumulateResult(0, jsonObject, "GO");

  }
  private void CashOut()
  {
    StartCoroutine(slotManager.ManageCashout());
    uiManager.ShowWinPopup(true, ResultData.payload.winAmount.ToString());
  }
  private void Go()
  {
    uiManager.SetPlayerBalance(PlayerData);
    if (ResultData.payload.completedAllSteps != null)
    {
      if (ResultData.payload.completedAllSteps)
      {
        slotManager.Win();
        return;
      }
    }
    if (!ResultData.payload.isCrash)
    {
      slotManager.JumpChicken();
      uiManager.setBEtBtnsIntractable(true);
      uiManager.ShowCashcollect(ResultData.payload.currentWinAmount.ToString());
    }
    else
    {
      slotManager.Die();
      uiManager.ShowCashcollect("0");
    }

  }


  private void RefreshUI()
  {
    uiManager.SetInitData(InitialData);
    uiManager.SetPlayerBalance(PlayerData);
    slotManager.SpawnRoad();



#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnEnter");
#endif
    RaycastBlocker.SetActive(false);
  }

  private void PopulateSlotSocket(List<string> LineIds)
  {
    // // slotManager.shuffleInitialMatrix();
    // slotManager.InitializeMatrix();
    // for (int i = 0; i < LineIds.Count; i++)
    // {
    //   slotManager.FetchLines(LineIds[i], i);
    // }
    // slotManager.SetInitialUI();
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnEnter");
#endif
    RaycastBlocker.SetActive(false);
  }

  internal void AccumulateResult(int currBet, string difficulty, string type)
  {
    if (PlayerData.balance < InitialData.bets[currBet])
    {
      uiManager.LowBalPopup();
      return;
    }
    isResultdone = false;
    MessageData message = new();
    message.type = type;
    message.payload.betIndex = currBet;
    message.payload.difficulty = difficulty;

    // Serialize message data to JSON
    string json = JsonUtility.ToJson(message);
    SendDataWithNamespace("request", json);
  }

  private List<string> ConvertListListIntToListString(List<List<int>> listOfLists)
  {
    List<string> resultList = new List<string>();

    foreach (List<int> innerList in listOfLists)
    {
      // Convert each integer in the inner list to string
      List<string> stringList = new List<string>();
      foreach (int number in innerList)
      {
        stringList.Add(number.ToString());
      }

      // Join the string representation of integers with ","
      string joinedString = string.Join(",", stringList.ToArray()).Trim();
      resultList.Add(joinedString);
    }

    return resultList;
  }
}

[Serializable]
public class MessageData
{
  public string type;
  public Data payload = new();
}

[Serializable]
public class Data
{
  public int betIndex;
  public string Event;
  public string difficulty;
  public List<int> index;
  public int option;
}

[Serializable]
public class GameData
{
  public List<double> bets;
  public Paytable paytable;
  public int historyLimit;
  public ProvablyFair provablyFair;
}
[Serializable]
public class Paytable
{
  public List<double> easy;
  public List<double> medium;
  public List<double> hard;
  public List<double> hardcore;
}
[Serializable]
public class Player
{
  public double balance;
}
[Serializable]
public class ProvablyFair
{
  public string nextServerSeedHash;
  public string clientSeed;
}
[Serializable]
public class Root
{
  public string id;
  public GameData gameData;
  public Player player;


  public bool success;
  public Payload payload;

}



[Serializable]
public class AuthTokenData
{
  public string cookie;
  public string socketURL;
  public string nameSpace;
}
[Serializable]
public class Payload
{
  public string difficulty;
  public int maxSteps;
  public int currentStep;
  public double currentWinAmount;
  public double winAmount;
  public ProvablyFair provablyFair;
  public bool completedAllSteps;
  public bool isCrash;
}