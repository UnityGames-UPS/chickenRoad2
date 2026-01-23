using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] SlotBehaviour slotmanager;
    [SerializeField] ImageAnimation ribbon;
    public void JumpAnimationComplete()
    {
        slotmanager.chickenAnimator.Play("idle");
    }
    public void ribbonAnimation()
    {
        ribbon.StartAnimation();
    }
}
