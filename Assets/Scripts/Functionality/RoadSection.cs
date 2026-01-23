using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class RoadSection : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text multiplyerTxt;

    [Header("Objects")]
    [SerializeField] private GameObject manholeCover;
    [SerializeField] private GameObject GoldenmanholeCover;
    [SerializeField] private Image randomCar;
    [SerializeField] private Image barricading;

    [Header("Car Sprites")]
    [SerializeField] private List<Sprite> carSprites;

    [Header("Car Speed List (Higher = Faster)")]
    [SerializeField] private List<int> carSpeeds;

    [Header("Positions")]
    [SerializeField] private Transform carStartPos;
    [SerializeField] private Transform carEndPos;
    [SerializeField] private Transform barricadingStopPos;

    private Tween carTween;
    private Tween spawnTween;

    private bool canSpawn = true;
    private bool carMoving = false;
    private float currentDuration;

    private RectTransform carRect;
    private RectTransform barricadeRect;


    #region Public API
    private void Awake()
    {
        carRect = randomCar.GetComponent<RectTransform>();
        barricadeRect = barricading.GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        PlayRandomeCarAnimation(true);
    }
    internal void SetMultiplyerData(double multiplyer)
    {
        multiplyerTxt.text = multiplyer.ToString();
    }

    internal void PlayRandomeCarAnimation(bool isPlaying)
    {
        if (isPlaying)
        {
            canSpawn = true;
            StartRandomSpawn();
        }
        else
        {
            StopAll();
        }
    }

    internal void StopAnimation(bool win)
    {
        canSpawn = false;
        spawnTween?.Kill();
        canSpawn = false;
        // if (!carMoving)
        //     return;


        MoveBarricading();
        SpeedUpAndFinish();
        //  HandleCarStopLogic();

    }
    internal void StopAnimationDie()
    {
        canSpawn = false;
        spawnTween?.Kill();
        canSpawn = false;
        if (!carMoving)
            return;


        // MoveBarricading();
        SpeedUpAndFinish();
        //  HandleCarStopLogic();

    }

    #endregion

    #region Car Logic

    private void StartRandomSpawn()
    {
        float waitTime = Random.Range(3f, 8f);

        spawnTween = DOVirtual.DelayedCall(waitTime, () =>
        {
            if (!canSpawn) return;

            SpawnAndMoveCar();
        });
    }

    private void SpawnAndMoveCar()
    {
        if (carMoving) return;

        carMoving = true;
        randomCar.gameObject.SetActive(true);

        carRect.anchoredPosition = carStartPos.GetComponent<RectTransform>().anchoredPosition;

        randomCar.sprite = carSprites[Random.Range(0, carSprites.Count)];

        int speed = carSpeeds[Random.Range(0, carSpeeds.Count)];

        float distance = Mathf.Abs(
            carEndPos.GetComponent<RectTransform>().anchoredPosition.y -
            carStartPos.GetComponent<RectTransform>().anchoredPosition.y
        );

        currentDuration = distance / speed;

        carTween = carRect
            .DOAnchorPosY(
                carEndPos.GetComponent<RectTransform>().anchoredPosition.y,
                currentDuration
            )
            .SetEase(Ease.Linear)
            .OnComplete(OnCarFinished);
    }

    private void OnCarFinished()
    {
        carMoving = false;
        randomCar.gameObject.SetActive(false);

        if (canSpawn)
            StartRandomSpawn();
    }

    private void SpeedUpAndFinish()
    {
        carTween?.Kill();

        carTween = carRect
            .DOAnchorPosY(
                carEndPos.GetComponent<RectTransform>().anchoredPosition.y,
                0.25f
            )
            .SetEase(Ease.Linear)
            .OnComplete(OnCarFinished);
    }



    private void HandleCarStopLogic()
    {
        canSpawn = false;
        float barricadeY = barricadingStopPos.GetComponent<RectTransform>().anchoredPosition.y;
        float carY = carRect.anchoredPosition.y;

        carTween?.Kill();

        if (carY < barricadeY)
        {
            carRect.DOAnchorPosY(barricadeY - 200f, 0.2f);
        }
        else
        {
            SpeedUpAndFinish();
        }
    }



    #endregion

    #region Barricading

    private void MoveBarricading()
    {
        barricadeRect
            .DOAnchorPos(
                barricadingStopPos.GetComponent<RectTransform>().anchoredPosition,
                0.5f
            )
            .SetEase(Ease.OutQuad);
    }



    #endregion

    private void StopAll()
    {
        canSpawn = false;
        carMoving = false;

        spawnTween?.Kill();
        carTween?.Kill();

        randomCar.gameObject.SetActive(false);
    }

    internal void setBlackmaholeOFF(bool isOff)
    {
        manholeCover.SetActive(isOff);
    }
    internal void setGoldenManholeCover(bool isOff)
    {
        if (isOff)
        {
            GoldenmanholeCover.transform.localRotation = Quaternion.Euler(0, 90f, 0);
            GoldenmanholeCover.SetActive(true);




            GoldenmanholeCover.transform
                .DOLocalRotate(new Vector3(0, 0, 0), 0.4f)
                .SetEase(Ease.OutBack);
        }
        else
        {
            GoldenmanholeCover.SetActive(isOff);

        }
    }

    internal void Reset()
    {
        barricading.transform.position = carStartPos.position;
        setGoldenManholeCover(false);
        setBlackmaholeOFF(true);
        canSpawn = true;
        // carMoving = true;
        StartRandomSpawn();
    }
}
