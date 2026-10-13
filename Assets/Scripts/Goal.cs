using UnityEngine;

public class Goal : MonoBehaviour
{
    // Reference to GameManager that controls victory screen
    [SerializeField] private GameManager gameManager;

    // When another collider touches goal
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check for player component on object touching goal
        Player player = collision.GetComponentInParent<Player>();

        // Make sure it's the player
        if (player != null)
        {
            // Tell GameManager level is complete
            gameManager.LevelComplete();
        }
    }
}
