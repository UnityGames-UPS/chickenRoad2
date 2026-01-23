using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    [SerializeField] private SlotBehaviour slotmanager;
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered ****************");
        slotmanager.roadMoveLimit = true;
    }
}
