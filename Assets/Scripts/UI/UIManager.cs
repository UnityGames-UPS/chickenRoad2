using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;


public class UIManager : MonoBehaviour
{

  [Header("Menu UI")]
  [SerializeField]
  private Button Menu_Button;
  [SerializeField]
  private GameObject Menu_Object;
  [SerializeField]
  private TMP_Text PlayerbalanceTxt;
  [SerializeField] private Button HowToPlayBtn;
  [SerializeField] private TMP_Text currentBetText;
  [SerializeField] private GameObject HowToPlayPopup;
  [SerializeField] internal Button CloseHTP;
  [SerializeField] private GameObject WinPopup;
  [SerializeField] private TMP_Text WinPopupText;
  [Header("main btn Pannel")]
  [SerializeField] internal Button PlayBtn;
  [SerializeField] private Button GoBtn;
  [SerializeField] private Button CollectBtn;
  [SerializeField] private Button EasyBtn;
  [SerializeField] private Button midiumBtn;
  [SerializeField] private Button HardBtn;
  [SerializeField] private Button HardcoreBtn;
  [SerializeField] private Button MinBetBtn;
  [SerializeField] private Button MaxBetBtn;
  [SerializeField] private Button BetOpt1Btn;
  [SerializeField] private Button BetOpt2Btn;
  [SerializeField] private Button BetOpt3Btn;
  [SerializeField] private Button BetOpt4Btn;

  [Header("provablyFair")]
  [SerializeField] private GameObject ProvablyfairPanel;
  [SerializeField] private GameObject GameRulesPanel;
  [SerializeField] private GameObject HistoryPanel;
  [SerializeField] private Button PFExitbtn;
  [SerializeField] private TMP_Text ClientSeed;
  [SerializeField] private TMP_Text NextSeed;
  [Header("AvatarPanel")]
  [SerializeField] private GameObject avatarPanel;
  [SerializeField] private Image currentAvatar;
  [SerializeField] private List<GameObject> photoList;
  [SerializeField] private List<Sprite> spriteAvatar;
  [SerializeField] private Button AVExitbtn;
  [SerializeField] private Button AVSaveButton;


  [Header("Settings UI")]
  [SerializeField]
  private Button Settings_Button;
  [SerializeField]
  private GameObject Settings_Object;
  [SerializeField]
  private RectTransform Settings_RT;
  [SerializeField]
  private Button ChangeAvatarbtn;



  [SerializeField]
  private Button Exit_Button;
  [SerializeField]
  private GameObject Exit_Object;
  [SerializeField]
  private RectTransform Exit_RT;

  [SerializeField]
  private Button ProbablyFairbtn;
  [SerializeField] private Button GameRulebtn;
  [SerializeField] private Button historyBtn;
  [SerializeField]
  private GameObject Paytable_Object;
  [SerializeField]
  private RectTransform Paytable_RT;

  [Header("Popus UI")]
  [SerializeField]
  private GameObject MainPopup_Object;
  [SerializeField] private List<GameObject> allPopups;
  [SerializeField]
  private Button ClosePopupBtn;
  [Header("Game Rule")]
  [SerializeField]
  private TMP_Text MinBetText;
  [SerializeField]
  private TMP_Text MaxBetText;
  [Header("About Popup")]
  [SerializeField]
  private GameObject AboutPopup_Object;
  [SerializeField]
  private Button AboutExit_Button;
  [SerializeField]
  private Image AboutLogo_Image;
  [SerializeField]
  private Button Support_Button;

  [Header("Paytable Popup")]
  [SerializeField]
  private GameObject PaytablePopup_Object;
  [SerializeField]
  private Button PaytableExit_Button;
  [SerializeField] private TMP_Text[] SymbolsText;
  [SerializeField]
  private TMP_Text FreeSpin_Text;
  [SerializeField]
  private TMP_Text Scatter_Text;
  [SerializeField]
  private TMP_Text Jackpot_Text;
  [SerializeField]
  private TMP_Text Bonus_Text;
  [SerializeField]
  private TMP_Text Wild_Text;

  [Header("Settings Popup")]
  [SerializeField]
  private GameObject SettingsPopup_Object;
  [SerializeField]
  private Button SettingsExit_Button;
  [SerializeField]
  private Button Sound_Button;
  [SerializeField]
  private Button Music_Button;

  [SerializeField]
  private GameObject MusicOn_Object;
  [SerializeField]
  private GameObject MusicOff_Object;
  [SerializeField]
  private GameObject SoundOn_Object;
  [SerializeField]
  private GameObject SoundOff_Object;


  [Header("Disconnection Popup")]
  [SerializeField]
  private Button CloseDisconnect_Button;
  [SerializeField]
  private GameObject DisconnectPopup_Object;

  [Header("AnotherDevice Popup")]
  [SerializeField]
  private Button CloseAD_Button;
  [SerializeField]
  private GameObject ADPopup_Object;

  [Header("Reconnection Popup")]
  [SerializeField]
  private TMP_Text reconnect_Text;
  [SerializeField]
  private GameObject ReconnectPopup_Object;

  [Header("LowBalance Popup")]
  [SerializeField]
  private Button LBExit_Button;
  [SerializeField]
  private GameObject LBPopup_Object;

  [Header("Quit Popup")]
  [SerializeField]
  private GameObject QuitPopup_Object;
  [SerializeField]
  private Button YesQuit_Button;
  [SerializeField]
  private Button NoQuit_Button;
  [SerializeField]
  private Button CrossQuit_Button;

  [SerializeField]
  private AudioController audioController;
  [SerializeField]
  private Button m_AwakeGameButton;
  [Header("History Popup")]
  [SerializeField] private GameObject HistoryPopup;
  [SerializeField]
  private Button CloseHistoryButton;

  [SerializeField] private GameObject HistoryPrefab;
  [SerializeField] private Transform HistorySpawnPanel;
  private Button LoadMoreButton;


  [SerializeField]
  private Button GameExit_Button;

  [SerializeField]
  private SlotBehaviour slotManager;

  [SerializeField]
  private SocketIOManager socketManager;
  private bool isMusic = true;
  private bool isSound = true;
  private Tween WinPopupTextTween;
  private Tween ClosePopupTween;
  internal bool isExit = false;
  internal int FreeSpins;
  internal int selectedAvatar;
  private void Start()
  {

    if (Menu_Button) Menu_Button.onClick.RemoveAllListeners();
    if (Menu_Button) Menu_Button.onClick.AddListener(OpenMenu);

    if (Exit_Button) Exit_Button.onClick.RemoveAllListeners();
    if (Exit_Button) Exit_Button.onClick.AddListener(CloseMenu);

    //if (About_Button) About_Button.onClick.RemoveAllListeners();
    //if (About_Button) About_Button.onClick.AddListener(delegate { OpenPopup(AboutPopup_Object); });

    if (AboutExit_Button) AboutExit_Button.onClick.RemoveAllListeners();
    if (AboutExit_Button) AboutExit_Button.onClick.AddListener(delegate { ClosePopup(AboutPopup_Object); });

    if (ProbablyFairbtn) ProbablyFairbtn.onClick.RemoveAllListeners();
    if (ProbablyFairbtn) ProbablyFairbtn.onClick.AddListener(delegate { OpenPopup(ProvablyfairPanel); });

    if (GameRulebtn) GameRulebtn.onClick.RemoveAllListeners();
    if (GameRulebtn) GameRulebtn.onClick.AddListener(delegate { OpenPopup(GameRulesPanel); });

    if (historyBtn) historyBtn.onClick.RemoveAllListeners();
    if (historyBtn) historyBtn.onClick.AddListener(delegate { OpenPopup(HistoryPopup); socketManager.AccumulateHistory(); });
    if (LoadMoreButton) historyBtn.onClick.RemoveAllListeners();
    if (LoadMoreButton) LoadMoreButton.onClick.AddListener(delegate { socketManager.AccumulateHistory(); });

    if (ChangeAvatarbtn) ChangeAvatarbtn.onClick.RemoveAllListeners();
    if (ChangeAvatarbtn) ChangeAvatarbtn.onClick.AddListener(delegate { OpenPopup(avatarPanel); setAvatarList(); });

    if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
    if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); });

    if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
    if (Settings_Button) Settings_Button.onClick.AddListener(delegate { OpenPopup(SettingsPopup_Object); });

    if (SettingsExit_Button) SettingsExit_Button.onClick.RemoveAllListeners();
    if (SettingsExit_Button) SettingsExit_Button.onClick.AddListener(delegate { ClosePopup(SettingsPopup_Object); });

    if (MusicOn_Object) MusicOn_Object.SetActive(true);
    if (MusicOff_Object) MusicOff_Object.SetActive(false);

    if (SoundOn_Object) SoundOn_Object.SetActive(true);
    if (SoundOff_Object) SoundOff_Object.SetActive(false);

    if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
    if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate
    {
      OpenPopup(QuitPopup_Object);
    });

    if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
    if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate
    {
      if (!isExit)
      {
        ClosePopup(QuitPopup_Object);
      }
    });

    if (CrossQuit_Button) CrossQuit_Button.onClick.RemoveAllListeners();
    if (CrossQuit_Button) CrossQuit_Button.onClick.AddListener(delegate
    {
      if (!isExit)
      {
        ClosePopup(QuitPopup_Object);
      }
    });

    if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
    if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

    if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
    if (YesQuit_Button) YesQuit_Button.onClick.AddListener(delegate
    {
      CallOnExitFunction();
      Debug.Log("quit event: pressed YES Button ");

    });

    if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
    if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(CallOnExitFunction); //BackendChanges

    if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
    if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);


    if (audioController) audioController.ToggleMute(false);

    isMusic = true;
    isSound = true;

    if (Sound_Button) Sound_Button.onClick.RemoveAllListeners();
    if (Sound_Button) Sound_Button.onClick.AddListener(ToggleSound);

    if (Music_Button) Music_Button.onClick.RemoveAllListeners();
    if (Music_Button) Music_Button.onClick.AddListener(ToggleMusic);

    if (PlayBtn) PlayBtn.onClick.RemoveAllListeners();
    if (PlayBtn) PlayBtn.onClick.AddListener(() => { slotManager.OnClickPlay(); PlayBtn.gameObject.SetActive(false); setBtnsIntractable(false); });

    if (CollectBtn) CollectBtn.onClick.RemoveAllListeners();
    if (CollectBtn) CollectBtn.onClick.AddListener(() => { slotManager.OnClickCashOut(); setBtnsIntractable(false); });

    if (GoBtn) GoBtn.onClick.RemoveAllListeners();
    if (GoBtn) GoBtn.onClick.AddListener(() => { slotManager.OnClickGO(); setBtnsIntractable(false); });

    if (EasyBtn) EasyBtn.onClick.RemoveAllListeners();
    if (EasyBtn) EasyBtn.onClick.AddListener(() => { slotManager.difficulty = "easy"; slotManager.ActivatePaytable(socketManager.InitialData.paytable.easy); });

    if (midiumBtn) midiumBtn.onClick.RemoveAllListeners();
    if (midiumBtn) midiumBtn.onClick.AddListener(() => { slotManager.difficulty = "medium"; slotManager.ActivatePaytable(socketManager.InitialData.paytable.medium); });

    if (HardBtn) HardBtn.onClick.RemoveAllListeners();
    if (HardBtn) HardBtn.onClick.AddListener(() => { slotManager.difficulty = "hard"; slotManager.ActivatePaytable(socketManager.InitialData.paytable.hard); });

    if (HardcoreBtn) HardcoreBtn.onClick.RemoveAllListeners();
    if (HardcoreBtn) HardcoreBtn.onClick.AddListener(() => { slotManager.difficulty = "hardcore"; slotManager.ActivatePaytable(socketManager.InitialData.paytable.hardcore); });

    if (MinBetBtn) MinBetBtn.onClick.RemoveAllListeners();
    if (MinBetBtn) MinBetBtn.onClick.AddListener(() => { ToggleBet(false); });

    if (MaxBetBtn) MaxBetBtn.onClick.RemoveAllListeners();
    if (MaxBetBtn) MaxBetBtn.onClick.AddListener(() => { ToggleBet(true); });

    if (HowToPlayBtn) HowToPlayBtn.onClick.RemoveAllListeners();
    if (HowToPlayBtn) HowToPlayBtn.onClick.AddListener(() => { OpenPopup(HowToPlayPopup); });

    if (ClosePopupBtn) ClosePopupBtn.onClick.RemoveAllListeners();
    if (ClosePopupBtn) ClosePopupBtn.onClick.AddListener(() => { ClosePopup(); });

    if (CloseHTP) CloseHTP.onClick.RemoveAllListeners();
    if (CloseHTP) CloseHTP.onClick.AddListener(() => { ClosePopup(); });

    if (AVExitbtn) AVExitbtn.onClick.RemoveAllListeners();
    if (AVExitbtn) AVExitbtn.onClick.AddListener(() => { ClosePopup(); });

    if (AVSaveButton) AVSaveButton.onClick.RemoveAllListeners();
    if (AVSaveButton) AVSaveButton.onClick.AddListener(() => { ClosePopup(); PlayerPrefs.SetInt("Avatar", selectedAvatar); });

    if (BetOpt1Btn) BetOpt1Btn.onClick.RemoveAllListeners();
    if (BetOpt1Btn) BetOpt1Btn.onClick.AddListener(() => { currentBetText.text = BetOpt1Btn.GetComponentInChildren<TMP_Text>().text; slotManager.currentBet = 0; });
    if (BetOpt2Btn) BetOpt2Btn.onClick.RemoveAllListeners();
    if (BetOpt2Btn) BetOpt2Btn.onClick.AddListener(() => { currentBetText.text = BetOpt2Btn.GetComponentInChildren<TMP_Text>().text; slotManager.currentBet = 1; });
    if (BetOpt3Btn) BetOpt3Btn.onClick.RemoveAllListeners();
    if (BetOpt3Btn) BetOpt3Btn.onClick.AddListener(() => { currentBetText.text = BetOpt3Btn.GetComponentInChildren<TMP_Text>().text; slotManager.currentBet = 2; });
    if (BetOpt4Btn) BetOpt4Btn.onClick.RemoveAllListeners();
    if (BetOpt4Btn) BetOpt4Btn.onClick.AddListener(() => { currentBetText.text = BetOpt4Btn.GetComponentInChildren<TMP_Text>().text; slotManager.currentBet = 3; });

    SetupButtonListeners();
    setAvatarList();
  }
  private void SetupButtonListeners()
  {
    for (int i = 0; i < photoList.Count; i++)
    {
      int index = i;

      Button btn = photoList[i].GetComponent<Button>();
      if (btn == null) continue;

      btn.onClick.RemoveAllListeners();
      btn.onClick.AddListener(() => OnAvatarClicked(index));
    }
  }

  private void OnAvatarClicked(int index)
  {
    selectedAvatar = index;
    //  PlayerPrefs.SetInt("Avatar", selectedAvatar);

    UpdateAvatarHighlight();
  }

  private void UpdateAvatarHighlight()
  {
    for (int i = 0; i < photoList.Count; i++)
    {
      if (photoList[i].transform.childCount == 0)
        continue;

      GameObject highlight = photoList[i].transform.GetChild(0).gameObject;
      highlight.SetActive(i == selectedAvatar);
    }
  }
  internal void setAvatarList()
  {
    int savedAvatar = PlayerPrefs.GetInt("Avatar", 0);
    for (int i = 0; i < photoList.Count; i++)
      currentAvatar.sprite = spriteAvatar[savedAvatar];
    for (int i = 0; i < photoList.Count; i++)
    {
      photoList[i].GetComponent<Image>().sprite = spriteAvatar[i];
      Transform highlight = photoList[i].transform.GetChild(0);
      if (i == savedAvatar)
      {
        highlight.gameObject.SetActive(true);
      }
      else
      {
        highlight.gameObject.SetActive(false);
      }
    }

  }
  internal void LowBalPopup()
  {
    OpenPopup(LBPopup_Object);
  }
  internal void SetPlayerBalance(Player player)
  {
    PlayerbalanceTxt.text = player.balance.ToString();
  }

  internal void DisconnectionPopup()
  {
    if (!isExit)
    {
      OpenPopup(DisconnectPopup_Object);
    }
  }

  internal void ReconnectionPopup()
  {
    OpenPopup(ReconnectPopup_Object);
  }

  internal void CheckAndClosePopups()
  {
    if (ReconnectPopup_Object.activeInHierarchy)
    {
      ClosePopup(ReconnectPopup_Object);
    }
    if (DisconnectPopup_Object.activeInHierarchy)
    {
      ClosePopup(DisconnectPopup_Object);
    }
  }





  internal void ShowCashcollect(string collectAmount)
  {
    CollectBtn.GetComponentInChildren<TMP_Text>().text = "Collect : \n" + collectAmount;
  }

  internal void ShowWinPopup(bool show, string collectAmount = "0")
  {
    audioController.PlayWLAudio("cashout");
    WinPopup.SetActive(show);
    WinPopupText.text = collectAmount;
  }

  internal void ADfunction()
  {
    OpenPopup(ADPopup_Object);
  }



  private void CallOnExitFunction()
  {
    if (!isExit)
    {
      isExit = true;
      audioController.PlayButtonAudio();
      slotManager.CallCloseSocket();
    }
  }

  private void OpenMenu()
  {
    audioController.PlayButtonAudio();
    if (Menu_Object) Menu_Object.SetActive(false);
    if (Exit_Object) Exit_Object.SetActive(true);
    //if (About_Object) About_Object.SetActive(true);
    if (Paytable_Object) Paytable_Object.SetActive(true);
    if (Settings_Object) Settings_Object.SetActive(true);

    //DOTween.To(() => About_RT.anchoredPosition, (val) => About_RT.anchoredPosition = val, new Vector2(About_RT.anchoredPosition.x, About_RT.anchoredPosition.y + 150), 0.1f).OnUpdate(() =>
    //{
    //    LayoutRebuilder.ForceRebuildLayoutImmediate(About_RT);
    //});

    DOTween.To(() => Paytable_RT.anchoredPosition, (val) => Paytable_RT.anchoredPosition = val, new Vector2(Paytable_RT.anchoredPosition.x, Paytable_RT.anchoredPosition.y + 125), 0.1f).OnUpdate(() =>
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate(Paytable_RT);
    });

    DOTween.To(() => Settings_RT.anchoredPosition, (val) => Settings_RT.anchoredPosition = val, new Vector2(Settings_RT.anchoredPosition.x, Settings_RT.anchoredPosition.y + 250), 0.1f).OnUpdate(() =>
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate(Settings_RT);
    });
  }

  private void CloseMenu()
  {

    if (audioController) audioController.PlayButtonAudio();
    //DOTween.To(() => About_RT.anchoredPosition, (val) => About_RT.anchoredPosition = val, new Vector2(About_RT.anchoredPosition.x, About_RT.anchoredPosition.y - 150), 0.1f).OnUpdate(() =>
    //{
    //    LayoutRebuilder.ForceRebuildLayoutImmediate(About_RT);
    //});

    DOTween.To(() => Paytable_RT.anchoredPosition, (val) => Paytable_RT.anchoredPosition = val, new Vector2(Paytable_RT.anchoredPosition.x, Paytable_RT.anchoredPosition.y - 125), 0.1f).OnUpdate(() =>
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate(Paytable_RT);
    });

    DOTween.To(() => Settings_RT.anchoredPosition, (val) => Settings_RT.anchoredPosition = val, new Vector2(Settings_RT.anchoredPosition.x, Settings_RT.anchoredPosition.y - 250), 0.1f).OnUpdate(() =>
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate(Settings_RT);
    });

    DOVirtual.DelayedCall(0.1f, () =>
     {
       if (Menu_Object) Menu_Object.SetActive(true);
       if (Exit_Object) Exit_Object.SetActive(false);
       //if (About_Object) About_Object.SetActive(false);
       if (Paytable_Object) Paytable_Object.SetActive(false);
       if (Settings_Object) Settings_Object.SetActive(false);
     });
  }

  private void OpenPopup(GameObject Popup)
  {
    foreach (var popup in allPopups)
    {

      popup.SetActive(false);
    }
    if (audioController) audioController.PlayButtonAudio();
    if (Popup) Popup.SetActive(true);
    if (MainPopup_Object) MainPopup_Object.SetActive(true);
  }

  internal void ClosePopup(GameObject Popup)
  {
    if (audioController) audioController.PlayButtonAudio();
    if (Popup) Popup.SetActive(false);
    if (!DisconnectPopup_Object.activeSelf)
    {
      if (MainPopup_Object) MainPopup_Object.SetActive(false);
    }
  }
  public void ClosePopup()
  {
    if (audioController) audioController.PlayButtonAudio();
    foreach (var popup in allPopups)
    {

      popup.SetActive(false);
    }
    if (!DisconnectPopup_Object.activeSelf)
    {
      if (MainPopup_Object) MainPopup_Object.SetActive(false);
    }
  }
  public void ToggleMusic()
  {
    isMusic = !isMusic;
    if (isMusic)
    {
      if (MusicOn_Object) MusicOn_Object.SetActive(true);
      if (MusicOff_Object) MusicOff_Object.SetActive(false);
      audioController.ToggleMute(false, "bg");
    }
    else
    {
      if (MusicOn_Object) MusicOn_Object.SetActive(false);
      if (MusicOff_Object) MusicOff_Object.SetActive(true);
      audioController.ToggleMute(true, "bg");
    }
  }


  public void ToggleSound()
  {
    isSound = !isSound;
    if (isSound)
    {
      if (SoundOn_Object) SoundOn_Object.SetActive(true);
      if (SoundOff_Object) SoundOff_Object.SetActive(false);
      if (audioController) audioController.ToggleMute(false, "button");
      if (audioController) audioController.ToggleMute(false, "wl");
    }
    else
    {
      if (SoundOn_Object) SoundOn_Object.SetActive(false);
      if (SoundOff_Object) SoundOff_Object.SetActive(true);
      if (audioController) audioController.ToggleMute(true, "button");
      if (audioController) audioController.ToggleMute(true, "wl");
    }
  }

  private void ToggleBet(bool minus)
  {
    if (minus)
    {
      if (slotManager.currentBet > 0)
      {
        slotManager.currentBet--;
      }
    }
    else
    {
      if (slotManager.currentBet < socketManager.InitialData.bets.Count - 1)
      {
        slotManager.currentBet++;
      }
    }
    currentBetText.text = socketManager.InitialData.bets[slotManager.currentBet].ToString();
  }
  internal void SetInitData(GameData initdata)
  {
    ClientSeed.text = initdata.provablyFair.clientSeed.ToString();
    NextSeed.text = initdata.provablyFair.nextServerSeedHash.ToString();
    MinBetText.text = initdata.bets[0].ToString();
    MaxBetText.text = initdata.bets[initdata.bets.Count - 1].ToString();
    currentBetText.text = initdata.bets[slotManager.currentBet].ToString();
    BetOpt1Btn.GetComponentInChildren<TMP_Text>().text = initdata.bets[0].ToString();
    BetOpt2Btn.GetComponentInChildren<TMP_Text>().text = initdata.bets[1].ToString();
    BetOpt3Btn.GetComponentInChildren<TMP_Text>().text = initdata.bets[2].ToString();
    BetOpt4Btn.GetComponentInChildren<TMP_Text>().text = initdata.bets[3].ToString();
  }
  internal void setBtnsIntractable(bool isIntractable)
  {
    PlayBtn.interactable = isIntractable;
    GoBtn.interactable = isIntractable;
    CollectBtn.interactable = isIntractable;
    MaxBetBtn.interactable = isIntractable;
    MinBetBtn.interactable = isIntractable;
    BetOpt1Btn.interactable = isIntractable;
    BetOpt2Btn.interactable = isIntractable;
    BetOpt3Btn.interactable = isIntractable;
    BetOpt4Btn.interactable = isIntractable;
    EasyBtn.interactable = isIntractable;
    midiumBtn.interactable = isIntractable;
    HardBtn.interactable = isIntractable;
    HardcoreBtn.interactable = isIntractable;

  }
  internal void setBEtBtnsIntractable(bool isIntractable)
  {
    CollectBtn.interactable = isIntractable;
    GoBtn.interactable = isIntractable;

  }
  internal void SetHistory(List<History> historyList)
  {
    // Clear old history
    foreach (Transform child in HistorySpawnPanel)
    {
      Destroy(child.gameObject);
    }

    if (historyList == null || historyList.Count == 0)
      return;

    foreach (var history in historyList)
    {
      GameObject obj = Instantiate(HistoryPrefab, HistorySpawnPanel);
      HistoryPrefab prefab = obj.GetComponent<HistoryPrefab>();

      string date = history.timestamp.ToString("dd MMM yyyy HH:mm");
      string bet = history.bet_amount;
      string win = history.total_win;

      // If multiplier exists inside details (example parsing)
      string multiplier = "-";


      if (!string.IsNullOrEmpty(history.details))
      {
        try
        {
          HistoryDetails details =
              JsonUtility.FromJson<HistoryDetails>(history.details);

          if (details?.provablyFair != null)
          {
            multiplier = "x" + details.provablyFair.multiplier.ToString("0.00");
          }
        }
        catch (Exception e)
        {
          Debug.LogWarning("Failed to parse history details: " + e.Message);
        }
      }


      prefab.SetData(date, bet, multiplier, win);
    }
  }

}
