using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatingCamera : MonoBehaviour
{
    public bool isEnabled;
    public Transform positionReference;    
    public Vector3 offset;    
    public bool lookAtTarget;
    public Transform targetTransform;
    [Range(0.01f, 1.0f)]
    public float smoothFactor;

    private Camera mainCam;
    private Camera specCam;
    private AudioListener alMainCam;
    private AudioListener alSpecCam;


    void Start()
    {
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        specCam = GetComponent<Camera>();

        alMainCam = mainCam.GetComponent<AudioListener>();
        alSpecCam = specCam.GetComponent<AudioListener>();

        if (isEnabled)
            ShowSpectatingCamera();
        else
            ShowMainCamera();
    }

    
    void Update()
    {
        if (isEnabled)
            ShowSpectatingCamera();
        else
            ShowMainCamera();        
    }


    private void LateUpdate()
    {
        if (positionReference)
        {
            Vector3 newPos = positionReference.position + offset;
            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
        }        

        if (lookAtTarget && targetTransform)
            transform.LookAt(targetTransform);
    }


    // Toggles between the main camera and the spectating camera
    public void SwapCamera()
    {
        if (isEnabled)
            ShowMainCamera();
        else
            ShowSpectatingCamera();
    }


    // Enables the main camera and disables the spectating camera
    public void ShowMainCamera()
    {
        mainCam.enabled = true;
        alMainCam.enabled = true;

        specCam.enabled = false;
        alSpecCam.enabled = false;

        isEnabled = false;
    }


    // Enables the spectating camera and disables the main camera
    public void ShowSpectatingCamera()
    {
        mainCam.enabled = false;
        alMainCam.enabled = false;

        specCam.enabled = true;
        alSpecCam.enabled = true;

        isEnabled = true;
    }


    // Set the transform that the spectating camera will be using as a reference for its position
    public void SetPositionReference(Transform posRef)
    {
        positionReference = posRef;
    }


    // Set the transform that the spectating camera will be looking at
    public void SetTargetToLook(Transform aTargetToLook)
    {
        targetTransform = aTargetToLook;
    }
}
