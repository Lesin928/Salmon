using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private InGameController inGameController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inGameController.CompleteGoal();
        }
    }
}
