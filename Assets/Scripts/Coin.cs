using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Set coin value
    public int value = 1;

    // Coin collection sound effect
    [SerializeField] private AudioClip collectSound;

    // Called when another collider touches coin
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        // Look for player component on object that collected the coin
        Player player = collision.GetComponentInParent<Player>();

        if (player != null)
        {
            // Add coin value to player's coin count
            player.AddCoin(value);
            
            // Play coin sound
            if (collectSound != null)
                {
                AudioSource.PlayClipAtPoint(
                    collectSound,
                    Camera.main.transform.position,
                    15f);
                }
            // Remove coin from scene after it is collected
            Destroy(gameObject);
        }
    }
}