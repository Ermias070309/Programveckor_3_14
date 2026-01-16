using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxEffect = 0.5f;

    Transform cameraTransform;
    Vector3 lastCameraPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(delta.x * parallaxEffect, delta.y * parallaxEffect, 0);
        lastCameraPosition = cameraTransform.position;
    }
}
