using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowMarble : MonoBehaviour
{
    public Transform marble;
    public float smoothSpeed = .125f;
    public float cameraDistance = 5.0f;
    public Vector2 rotationSpeed = new Vector2(120.0f, 120.0f);

    public Vector3 offset;
    private float currentZoom;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    void Start()
    {
        currentZoom = cameraDistance;
        offset = new Vector3(0, 0, -currentZoom);
    }

    // Update is called once per frame
    void Update()
    {
        yaw += rotationSpeed.x * Input.GetAxis("Mouse X") * Time.deltaTime;
        pitch -= rotationSpeed.y * Input.GetAxis("Mouse Y") * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -35, 85);

        transform.LookAt(marble);
    }

    void LateUpdate()
    {
        offset = Quaternion.Euler(pitch, yaw, 0) * new Vector3(0, 0, -currentZoom);
        transform.position = marble.position + offset;
        transform.LookAt(marble.position);
    }
}
