using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        // Find the main camera in the scene.
        mainCameraTransform = Camera.main.transform;

        if (mainCameraTransform == null)
        {
            Debug.LogError("Main camera not found in the scene.");
        }
    }

    private void LateUpdate()
    {
        // Make the canvas face the camera's position while keeping its up direction unchanged.
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
    }
}