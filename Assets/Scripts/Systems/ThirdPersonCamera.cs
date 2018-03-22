using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public static ThirdPersonCamera Instance;
    public static Transform cameraRotator;

    [HideInInspector]
    public Camera mainCamera;
    public bool inverted;
    public LayerMask terrainMask;
    public Transform pivotPoint;

    private Vector3 mainOffset;
    public float rightOffset;
    private float aimOffset;

    private Transform player;
    [Range(1,3)]
    private float zoom = 2;

    void Awake ()
    {
        player = GameManager.player.transform;

        mainCamera = GetComponent<Camera>();
        if (!cameraRotator)
        {
            cameraRotator = new GameObject("CameraRotator").transform;
            cameraRotator.rotation = player.rotation;
            DontDestroyOnLoad(cameraRotator);

            cameraRotator.position = pivotPoint.position;
            transform.parent = cameraRotator;
            mainOffset = transform.position - cameraRotator.position;
        }
    }
	
	void Update ()
    {
        // Update Pseudo Camera
        cameraRotator.position = pivotPoint.position;
        cameraRotator.Rotate(Mathf.Clamp(Input.GetAxis("Mouse Y") * (inverted ? 1 : -1) * Time.deltaTime * 30, -5, 5), 0, 0);
        cameraRotator.rotation = Quaternion.LookRotation
            (
                ClampCameraForward(Vector3.ProjectOnPlane(cameraRotator.forward, player.transform.right)),
                Vector3.ProjectOnPlane(player.transform.up, player.transform.right)
            );

        // Update positon
        zoom -= Input.GetAxis("Mouse ScrollWheel");
        zoom = Mathf.Clamp(zoom, 1, 3);

        if (Input.GetKeyDown(KeyCode.Q)) rightOffset = -rightOffset;
        aimOffset = Mathf.Lerp(aimOffset, rightOffset, Time.deltaTime * 3);
        transform.localPosition = (mainOffset.normalized * zoom) + (Vector3.right * aimOffset);
    }

    private Vector3 ClampCameraForward(Vector3 inForward)
    {
        float angle = Vector3.Angle(player.up, inForward);
        if (angle < 10) inForward = Vector3.SlerpUnclamped(player.up, inForward, 2f - (angle / 10));
        else if (angle > 170) inForward = Vector3.SlerpUnclamped(player.up, inForward, 2f - (angle / 170));
        return inForward;
    }

    private float ClampLocalRotation(float value)
    {
        if (value > 180 && value < 280)
            value = 280;
        else if (value < 180 && value > 80)
            value = 80;
        return value;
    }
}
