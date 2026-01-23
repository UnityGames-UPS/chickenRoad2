using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;


public class SlotBehaviour : MonoBehaviour
{
  [Header("Reference")]
  [SerializeField] private SocketIOManager SocketManager;
  [SerializeField] private UIManager uiManager;
  [SerializeField] private AudioController audioController;

  [Header("Road Reference")]
  [SerializeField] private Transform MovableRoad;
  [SerializeField] private GameObject RoadSectionPrefab;
  [SerializeField] private List<RoadSection> RoadSectionList = new List<RoadSection>();
  [SerializeField] private GameObject RoadEndSection;
  [SerializeField] private Transform EndScreen;
  [Header("Chicken")]
  [SerializeField] private GameObject chicken;
  [SerializeField] internal Animator chickenAnimator;
  [SerializeField] ImageAnimation ribbon;

  [Header("Car")]
  [SerializeField] private Image car;
  [SerializeField] private List<Sprite> carSprites;

  [Header("Positions")]
  [SerializeField] private Transform startPosChicken;
  [SerializeField] private Transform jumpPosChicken;
  [SerializeField] private Transform winPosChicken;
  [SerializeField] private Transform carStartPos;
  [SerializeField] private Transform carEndPos;

  private bool firstClickDone = false;
  private bool gameEnded = false;

  internal bool isSpaceButtonWorking = true;
  private Tween chickenTween;
  private Tween carTween;
  [SerializeField] private float roadMoveDistance = 250f;
  [SerializeField] private float roadMoveDuration = 0.3f;
  int currentRoad = 0;
  private int moveStepCount = 0;

  private Tween roadTween;

  internal bool roadMoveLimit = false;


  [Header("data ")]
  [SerializeField] internal int currentBet;
  [SerializeField] internal string difficulty = "easy";
  internal void CallCloseSocket()
  {
    StartCoroutine(SocketManager.CloseSocket());
  }


  internal void SpawnRoad()
  {
    if (SocketManager == null || SocketManager.InitialData == null)
    {
      Debug.LogError("GameData not found!");
      return;
    }

    Paytable paytable = SocketManager.InitialData.paytable;


    int maxCount = Mathf.Max(
        paytable.easy.Count,
        paytable.medium.Count,
        paytable.hard.Count,
        paytable.hardcore.Count
    );


    for (int i = 0; i < maxCount; i++)
    {
      GameObject obj = Instantiate(RoadSectionPrefab, MovableRoad);
      RoadSection section = obj.GetComponent<RoadSection>();

      obj.SetActive(false);
      RoadSectionList.Add(section);
    }

    RoadEndSection.transform.SetAsLastSibling();
    ActivatePaytable(SocketManager.InitialData.paytable.easy);
  }
  internal void onclickMax()
  {

  }
  internal void ActivatePaytable(List<double> multiplierList)
  {
    for (int i = 0; i < RoadSectionList.Count; i++)
    {
      if (i < multiplierList.Count)
      {
        RoadSectionList[i].gameObject.SetActive(true);
        RoadSectionList[i].SetMultiplyerData(multiplierList[i]);
      }
      else
      {
        RoadSectionList[i].gameObject.SetActive(false);
      }
    }
    RoadEndSection.transform.SetAsLastSibling();
  }
  internal void OnClickPlay()
  {
    SocketManager.AccumulateResult(currentBet, difficulty, "PLAY");
  }
  internal void OnClickGO()
  {
    SocketManager.AccumulateResult(currentBet, difficulty, "GO");
  }
  internal void OnClickCashOut()
  {
    SocketManager.AccumulateResult(currentBet, difficulty, "CASHOUT");
  }

  internal void JumpChicken()
  {
    if (gameEnded) return;
    Debug.Log("jump chicken");
    if (!firstClickDone)
    {
      firstClickDone = true;
      FirstJump();
    }
    else
    {
      PlayJumpAnimationOnly();
      MoveRoad();
      if (currentRoad - 2 >= 0)
      {
        RoadSectionList[currentRoad - 2].setBlackmaholeOFF(false);
        RoadSectionList[currentRoad - 2].setGoldenManholeCover(true);
      }
    }
  }
  #region  chicken animation


  private void FirstJump()
  {
    Debug.Log("first jump");
    chickenTween?.Kill();
    RoadSectionList[currentRoad].StopAnimation(SocketManager.ResultData.payload.isCrash);
    currentRoad++;
    chickenAnimator.Play("jump");
    audioController.PlayWLAudio("go");


    chickenTween = chicken.transform.DOMove(jumpPosChicken.position, 0.35f)
        .SetEase(Ease.OutQuad);
  }

  private void PlayJumpAnimationOnly()
  {
    audioController.PlayWLAudio("go");
    chickenAnimator.Play("jump");
    RoadSectionList[currentRoad].StopAnimation(SocketManager.ResultData.payload.isCrash);
    currentRoad++;

  }

  public void Win()
  {
    if (gameEnded) return;
    gameEnded = true;

    chickenTween?.Kill();

    chickenAnimator.Play("jump");
    MoveRoad();
    if (currentRoad - 2 >= 0)
    {
      RoadSectionList[currentRoad - 2].setBlackmaholeOFF(false);
      RoadSectionList[currentRoad - 2].setGoldenManholeCover(true);
    }

    StartCoroutine(WinGameCoroutine());
  }
  IEnumerator WinGameCoroutine()
  {

    yield return new WaitForSeconds(1f);
    audioController.PlayWLAudio("cashout");
    chickenAnimator.Play("win");
    if (currentRoad - 2 >= 0)
    {
      RoadSectionList[currentRoad - 2].setBlackmaholeOFF(false);
      RoadSectionList[currentRoad - 2].setGoldenManholeCover(true);
    }
    chicken.transform.DOMove(winPosChicken.position, 0.5f)
         .SetEase(Ease.OutBack);

    yield return new WaitForSeconds(4f);
    ResetGame();
  }
  public void Die()
  {
    if (gameEnded) return;
    gameEnded = true;
    RoadSectionList[currentRoad].StopAnimationDie();
    chickenTween?.Kill();
    chickenAnimator.Play("jump");
    MoveRoad();
    if (currentRoad - 2 >= 0)
    {
      RoadSectionList[currentRoad - 2].setBlackmaholeOFF(false);
      RoadSectionList[currentRoad - 2].setGoldenManholeCover(true);
    }

    StartCoroutine(ResetGameCoroutine());
  }
  IEnumerator ResetGameCoroutine()
  {

    yield return new WaitForSeconds(0.5f);
    audioController.PlayWLAudio("crash");
    chickenAnimator.Play("die");
    PlayCarKillAnimation();
    yield return new WaitForSeconds(4f);
    ResetGame();
  }

  private void PlayCarKillAnimation()
  {
    car.sprite = carSprites[Random.Range(0, carSprites.Count)];
    car.transform.position = carStartPos.position;
    car.gameObject.SetActive(true);

    carTween = car.transform.DOMove(carEndPos.position, 0.25f)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
          car.gameObject.SetActive(false);
        });
  }


  // private void MoveRoad()
  // {
  //   roadTween?.Kill();

  //   roadTween = MovableRoad
  //       .DOLocalMoveX(-250f, 0.3f)
  //       .SetRelative(true)
  //       .SetEase(Ease.OutQuad);
  // }

  private void MoveRoad()
  {
    roadTween?.Kill();
    chickenTween?.Kill();

    float roadEndX = RoadEndSection.transform.position.x;
    float endScreenX = EndScreen.position.x;

    if (!roadMoveLimit)
    {

      roadTween = MovableRoad
          .DOLocalMoveX(-250f, 0.3f)
          .SetRelative(true)
          .SetEase(Ease.OutQuad);
    }
    else
    {

      chickenTween = chicken.transform
          .DOLocalMoveX(250f, 0.3f)
          .SetRelative(true)
          .SetEase(Ease.OutQuad);
    }
  }
  // private void MoveRoad()
  // {
  //   moveStepCount++;

  //   roadTween?.Kill();
  //   chickenTween?.Kill();

  //   float roadEndX = RoadEndSection.transform.position.x;
  //   float endScreenX = EndScreen.position.x;


  //   float offset = moveStepCount * roadMoveDistance;

  //   if (roadEndX > endScreenX)
  //   {
  //     MovableRoad.localPosition = new Vector3(
  //         MovableRoad.localPosition.x - roadMoveDistance,
  //         MovableRoad.localPosition.y,
  //         MovableRoad.localPosition.z
  //     );

  //     roadTween = MovableRoad
  //         .DOLocalMoveX(MovableRoad.localPosition.x, roadMoveDuration)
  //         .SetEase(Ease.OutQuad);
  //   }
  //   else
  //   {
  //     chicken.transform.localPosition = new Vector3(
  //         chicken.transform.localPosition.x + roadMoveDistance,
  //         chicken.transform.localPosition.y,
  //         chicken.transform.localPosition.z
  //     );

  //     chickenTween = chicken.transform
  //         .DOLocalMoveX(chicken.transform.localPosition.x, roadMoveDuration)
  //         .SetEase(Ease.OutQuad);
  //   }
  // }

  #endregion
  internal IEnumerator ManageCashout()
  {
    yield return new WaitForSeconds(2f);
    ResetGame();
  }
  internal void ResetGame()
  {
    MovableRoad.transform.localPosition = new Vector3(-1250 + 80, MovableRoad.localPosition.y, MovableRoad.localPosition.z);
    uiManager.PlayBtn.gameObject.SetActive(true);
    uiManager.PlayBtn.interactable = true;
    chicken.transform.position = startPosChicken.position;
    chickenAnimator.Play("idle");
    currentRoad = 0;
    firstClickDone = false;
    gameEnded = false;
    ribbon.StopAnimation();
    ribbon.gameObject.GetComponent<Image>().sprite = ribbon.textureArray[0];
    foreach (var road in RoadSectionList)
    {
      road.Reset();
    }
    uiManager.ShowWinPopup(false);
    uiManager.setBtnsIntractable(true);
    moveStepCount = 0;
    roadMoveLimit = false;

  }

  // void Update()
  // {
  //   if (Input.GetKeyDown(KeyCode.Space))
  //   {
  //     if (isSpaceButtonWorking)
  //     {
  //       if (uiManager.PlayBtn.gameObject.activeInHierarchy)
  //       {
  //         OnClickPlay(); uiManager.PlayBtn.gameObject.SetActive(false); uiManager.setBtnsIntractable(false);
  //       }
  //       else
  //       {
  //         OnClickGO(); uiManager.setBtnsIntractable(false);
  //       }
  //     }
  //   }
  // }

}
