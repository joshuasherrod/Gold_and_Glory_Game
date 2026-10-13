
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Target for camera to follow
    public Transform target;

    // How smoothly the camera tracks the target (player)
    public float smoothSpeed = 0.4f;

    // Keep camera in front of 2d scene
    public Vector3 offset = new Vector3(0f, 0f, -10f);


    // Slightly delay movement for smoothness
    private void LateUpdate()
    {
        // If no target is assigned, stop
        if (target == null)
        {
            return;
        }

        // Calculate camera position (player position + offset)
        Vector3 targetPosition = target.position + offset;

        // Pan over to follow player without jitering
        transform.position = Vector3.Lerp(
            transform.position, 
            targetPosition, 
            smoothSpeed);
    }
}
