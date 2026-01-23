using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderToggle : MonoBehaviour, IPointerClickHandler
{
    [Header("State")]
    [SerializeField] private bool isOn;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField]
    private AnimationCurve ease =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Events")]
    public UnityEvent OnToggleOn;
    public UnityEvent OnToggleOff;

    private Slider slider;
    private Coroutine animationRoutine;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        // Slider setup
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;

        SetInstant(isOn);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }

    public void Toggle()
    {
        SetState(!isOn);
    }

    public void SetState(bool value)
    {
        if (isOn == value)
            return;

        isOn = value;

        if (isOn)
            OnToggleOn?.Invoke();
        else
            OnToggleOff?.Invoke();

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float start = slider.value;
        float end = isOn ? 1f : 0f;
        float time = 0f;

        while (time < animationDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = ease.Evaluate(time / animationDuration);
            slider.value = Mathf.Lerp(start, end, t);
            yield return null;
        }

        slider.value = end;
    }

    private void SetInstant(bool value)
    {
        slider.value = value ? 1f : 0f;
    }

    public bool IsOn => isOn;
}
