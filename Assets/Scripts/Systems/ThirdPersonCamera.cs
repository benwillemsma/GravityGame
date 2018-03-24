using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public static ThirdPersonCamera Instance;
    public static Transform cameraRotator;
    
    public LayerMask terrainMask;
    public Transform pivotPoint;

    private Vector3 mainOffset;
    private float aimOffset;

    private Player player;
    private float desiredZoom = 2;
    private float zoom = 2;

    void Awake ()
    {
        player = GameManager.player;

        Instance = this;
        if (!cameraRotator)
        {
            cameraRotator = new GameObject("CameraRotator").transform;
            cameraRotator.rotation = player.transform.rotation;
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
        cameraRotator.Rotate(Input.GetAxis("Mouse Y") * (player.InvertedCamera ? 1 : -1) * Time.deltaTime * player.CameraSensitivity, 0, 0);
        cameraRotator.rotation = Quaternion.LookRotation
            (
                ClampCameraForward(Vector3.ProjectOnPlane(cameraRotator.forward, player.transform.right)),
                Vector3.ProjectOnPlane(player.transform.up, player.transform.right)
            );

        // Update positon
        if (Input.GetButton("Aiming"))
            zoom = Mathf.Lerp(zoom, 1, Time.deltaTime * 6);
        else
        {
            desiredZoom -= Input.GetAxis("Mouse ScrollWheel");
            desiredZoom = Mathf.Clamp(desiredZoom, 1, 3);
            zoom = Mathf.Lerp(zoom, desiredZoom, Time.deltaTime * 6);
        }

        if (Input.GetKeyDown(KeyCode.Q)) player.CameraOffset = -player.CameraOffset;
        aimOffset = Mathf.Lerp(aimOffset, player.CameraOffset, Time.deltaTime * 3);
        transform.localPosition = (mainOffset.normalized * zoom) + (Vector3.right * aimOffset);
    }

    private Vector3 ClampCameraForward(Vector3 inForward)
    {
        float forwardCheck = Vector3.Dot(inForward, player.transform.forward);
        if (forwardCheck < 0.1736f)
        {
            if (forwardCheck < 0f) inForward = Vector3.Reflect(inForward, player.transform.forward);
            float upCheck = Vector3.Dot(inForward,player.transform.up);
            if (upCheck > 0) inForward = player.transform.rotation * new Vector3(0, 0.9848f, 0.1736f);
            else inForward = player.transform.rotation * new Vector3(0, -0.9848f, 0.1736f);
        }
        return inForward;
    }
}
