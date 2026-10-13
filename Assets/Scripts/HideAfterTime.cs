using UnityEngine;

// Display level number for 3 seconds at beginning of level
public class HideAfterTime : MonoBehaviour
{
    public float displayTime = 3f;

    private void Start()
    {
        Invoke(nameof(Hide), displayTime);
    }

    // Hide after 3 seconds
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
