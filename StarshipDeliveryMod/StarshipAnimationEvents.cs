using System.Collections;
using System.Collections.Generic;
using StarshipDeliveryMod;
using StarshipDeliveryMod.Patches;
using UnityEngine;

//Used to be called by the AnimationClips as AnimationEvents
public class StarshipAnimationEvents : MonoBehaviour
{
    private GameObject landingFxPrefab = null!;
    private GameObject liftoffFxPrefab = null!;
    private Vector3 spawnPosition;

    private AudioSource? audioSrc;
    private AudioClip? landingSFX;
    private AudioClip? liftoffSFX;
    private AudioClip? sonicBoomSFX;

    public void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        audioSrc.maxDistance = 2000;
        landingSFX = StarshipDelivery.Ressources.LoadAsset<AudioClip>("assets/audioclip/starshiplanding.wav");
        liftoffSFX = StarshipDelivery.Ressources.LoadAsset<AudioClip>("assets/audioclip/starshipliftoff.wav");
        sonicBoomSFX = StarshipDelivery.Ressources.LoadAsset<AudioClip>("assets/audioclip/sonicboom.wav");
    }

    public void InitFX(GameObject _landingFxPrefab, GameObject _liftoffFxPrefab, Transform _liftoffPosition)
    {
        landingFxPrefab = _landingFxPrefab;
        liftoffFxPrefab = _liftoffFxPrefab;
        spawnPosition = _liftoffPosition.position + new Vector3(0, -1.42f, 0);

    }

    public void Landing()
    {
        GameObject effect = Instantiate<GameObject>(landingFxPrefab, spawnPosition, Quaternion.Euler(-90, 0, 0));
        Destroy(effect, 8f);
    }

    public void Liftoff()
    {
        GameObject effect = Instantiate<GameObject>(liftoffFxPrefab, spawnPosition, Quaternion.Euler(-90, 0, 0));
        Destroy(effect, 8f);
    }

    public void BigCamShake()
    {
        Transform? playerCam = StartOfRoundPatch.CurrentCam?.transform.Find("PlayerAudioListener");
        if (playerCam == null)
            return;

        float distanceToCam = Vector3.Distance(transform.position, playerCam.position);
        if(distanceToCam < 60f)
        {
            HUDManager.Instance.ShakeCamera(ScreenShakeType.VeryStrong);
        }
    }

    public void LongCamShake()
    {
        Transform? playerCam = StartOfRoundPatch.CurrentCam?.transform.Find("PlayerAudioListener");
        if (playerCam == null)
            return;

        float distanceToCam = Vector3.Distance(transform.position, playerCam.position);
        if(distanceToCam < 60f)
        {
            HUDManager.Instance.ShakeCamera(ScreenShakeType.Long);
        }
    }
    void LandingSFX()
    {
        if(audioSrc != null)
        {
            audioSrc.spatialBlend = 1f;
            audioSrc.PlayOneShot(landingSFX);
        }
    }

    void LiftoffSFX()
    {
        if(audioSrc != null)
        {
            audioSrc.spatialBlend= 1f;
            audioSrc.PlayOneShot(liftoffSFX);
        }
    }

    void SonicBoomSFX()
    {
        if(audioSrc != null)
        {
            audioSrc.spatialBlend= 0.2f;
            audioSrc.PlayOneShot(sonicBoomSFX);
        }
    }
}