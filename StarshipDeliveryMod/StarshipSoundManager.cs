using System;
using System.Collections;
using System.Collections.Generic;
using StarshipDeliveryMod.Patches;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

namespace StarshipDeliveryMod;
internal class StarshipSoundManager : MonoBehaviour
{
    private float dropShipVolume = 1;
    private float musicVolume0 = 1;
    private float musicVolume1 = 1;
    private GameObject? musicObj;
    private GameObject? farMusicObj;
    private AudioMixer? audioMixer;
    void Start()
    {
        StartOfRoundPatch.onEnteringDungeon += ReduceStarshipVolume;
        musicObj = transform.Find("Music").gameObject;
        farMusicObj = musicObj.transform.Find("Music (1)").gameObject;

        musicVolume0 = musicObj.GetComponent<AudioSource>().volume;
        musicVolume1 = farMusicObj.GetComponent<AudioSource>().volume;

        dropShipVolume = dropShipVolume * (ConfigSettings.sfxVolume.Value / 100);
        StarshipDelivery.mls.LogInfo(">>> Ship SFX volume set to " + ConfigSettings.sfxVolume.Value.ToString() + "% ( " + dropShipVolume + " )");

        ReduceStarshipVolume(false);
        ReplaceAudioMixerGroup();
    }

    void ReduceStarshipVolume(bool _inDungeon)
    {
        AudioSource source = GetComponent<AudioSource>();

        if(_inDungeon)
        {
            source.volume = dropShipVolume * 0.1f;
            musicObj.GetComponent<AudioSource>().volume = musicVolume0 * 0.3f;
            farMusicObj.GetComponent<AudioSource>().volume = musicVolume1 * 0.3f;
            StarshipDelivery.mls.LogInfo("Volume reduced");
        }
        else
        {
            source.volume = dropShipVolume;
            musicObj.GetComponent<AudioSource>().volume = musicVolume0;
            farMusicObj.GetComponent<AudioSource>().volume = musicVolume1;
            StarshipDelivery.mls.LogInfo("Volume increased");
        }
    }

    void ReplaceAudioMixerGroup()
    {
        audioMixer = StarshipDelivery.Ressources.LoadAsset<AudioMixer>("assets/audiomixercontroller/starshipmixer.mixer");
        AudioMixerGroup sfxGroup = audioMixer.FindMatchingGroups("Master/SFXs")[0];
        AudioMixerGroup musicGroup = audioMixer.FindMatchingGroups("Master/DropshipMusic")[0];

        GetComponent<AudioSource>().outputAudioMixerGroup = sfxGroup;

        if(ConfigSettings.enableMusicEffects?.Value == true)
        {
            musicObj.GetComponent<AudioSource>().outputAudioMixerGroup = musicGroup;
            musicObj.GetComponent<AudioSource>().pitch = 0.82f;
            farMusicObj.GetComponent<AudioSource>().outputAudioMixerGroup = musicGroup;
            farMusicObj.GetComponent<AudioSource>().pitch = 0.82f;
        }
    }
}