using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public static FirstPersonCamera Instance;
    public static Transform cameraRotator;

    [HideInInspector]
    public Camera mainCamera;
    public bool inverted;

    private Transform player;

    void Awake ()
    {
        player = GameManager.player.transform;

        mainCamera = GetComponent<Camera>();
        if (!cameraRotator)
        {
            cameraRotator = new GameObject("CameraRotator").transform;
            cameraRotator.rotation = player.rotation;
            DontDestroyOnLoad(cameraRotator);
        }
    }
	
	void Update ()
    {
        // Update Pseudo Rotation
        cameraRotator.rotation = Quaternion.LookRotation
            (Vector3.ProjectOnPlane(cameraRotator.forward, player.transform.right)
            , Vector3.ProjectOnPlane(player.transform.up, player.transform.right));
        cameraRotator.Rotate(Input.GetAxis("Mouse Y") * (inverted ? 1 : -1) * Time.deltaTime * 20, 0, 0);
        cameraRotator.position = transform.position;

        // Update Real rotation
        transform.rotation = cameraRotator.rotation;
    }
}
