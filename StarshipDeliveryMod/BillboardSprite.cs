using System.Collections;
using System.Collections.Generic;
using StarshipDeliveryMod;
using StarshipDeliveryMod.Patches;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public float sizeMultiplier = 0.003f;
    private Camera? mainCamera;

	void Start()
    {
        mainCamera = StartOfRoundPatch.CurrentCam;
        StartOfRoundPatch.onCameraChange += UpdateCamera;
    }
	void Update()
	{
        if (mainCamera == null)
        {
            return;
        }
            
        transform.LookAt(mainCamera.transform);

        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        float newSize = distance * sizeMultiplier;
        transform.localScale = new Vector3(newSize, newSize, newSize);
	}

    void UpdateCamera(Camera cam)
    {
        mainCamera = cam;
    }
}