using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    // Array for heart image objects displayed on health bar
    public Image[] hearts;

    // Update number of visible hearts based on player's health
    public void SetHealth(int health)
    {
        // One heart = 25 health (out of 100)
        int heartsRemaining = Mathf.CeilToInt(health / 25f);

        // Check each heart in the array
        for (int i = 0; i < hearts.Length; i++)
        {
            // Display hearts remaining
            hearts[i].gameObject.SetActive(i < heartsRemaining);
        }
    }
}
