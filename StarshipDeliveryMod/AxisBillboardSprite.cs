using System.Collections;
using System.Collections.Generic;
using StarshipDeliveryMod.Patches;
using UnityEngine;

public class AxisBillboardSprite : MonoBehaviour
{
    private Camera? mainCamera;
    private Transform? axisRef;

    void Start()
    {
        mainCamera = StartOfRoundPatch.CurrentCam;
        StartOfRoundPatch.onCameraChange += UpdateCamera;
        axisRef = transform.parent;
    }

    void Update()
    {
        if(mainCamera == null || axisRef == null)
        {
            return;
        }

        float camAxisAlignment = Vector3.Dot(axisRef.up, mainCamera.transform.position - transform.position);
        Vector3 cameraPoint = mainCamera.transform.position - axisRef.up * camAxisAlignment;
        transform.LookAt(cameraPoint, axisRef.up);
    }

    void UpdateCamera(Camera cam)
    {
        mainCamera = cam;
    }
}
