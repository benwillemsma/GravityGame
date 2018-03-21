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

    private Vector3 mainOffset;
    public float rightOffset;
    private float aimOffset;

    private Transform player;
    [Range(1,3)]
    private float zoom = 1;

    void Awake ()
    {
        player = GameManager.player.transform;

        mainCamera = GetComponent<Camera>();
        if (!cameraRotator)
        {
            cameraRotator = new GameObject("CameraRotator").transform;
            cameraRotator.position = player.GetComponent<Player>().FPCamera.transform.position;
            cameraRotator.rotation = player.rotation;
            transform.parent = cameraRotator;
            mainOffset = transform.position - cameraRotator.position;
            DontDestroyOnLoad(cameraRotator);
        }
    }
	
	void Update ()
    {
        // Update Pseudo Camera
        cameraRotator.position = player.GetComponent<Player>().FPCamera.transform.position;
        cameraRotator.rotation = Quaternion.LookRotation
            (Vector3.ProjectOnPlane(cameraRotator.forward, player.transform.right)
            , Vector3.ProjectOnPlane(player.transform.up, player.transform.right));
        cameraRotator.Rotate(Input.GetAxis("Mouse Y") * (inverted ? 1 : -1) * Time.deltaTime * 50, 0, 0);

        // Update positon
        zoom -= Input.GetAxis("Mouse ScrollWheel");
        zoom = Mathf.Clamp(zoom, 1, 3);

        if (Input.GetKeyDown(KeyCode.Q)) rightOffset = -rightOffset;
        aimOffset = Mathf.Lerp(aimOffset, rightOffset, Time.deltaTime * 3);
        transform.localPosition = (mainOffset.normalized * zoom) + (Vector3.right * aimOffset);
    }
}
