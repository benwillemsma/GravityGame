using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public static Transform cameraRotator;

    [HideInInspector]
    public Camera mainCamera;
    public bool inverted;

    private Transform player;

    void Awake ()
    {
        if (!Instance)
            Instance = this;
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

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
        cameraRotator.Rotate(player.up, Input.GetAxis("Mouse X") * Time.deltaTime * 20, Space.World);
        cameraRotator.position = transform.transform.position;

        // Update Real rotation
        transform.rotation = cameraRotator.rotation;
    }
}
