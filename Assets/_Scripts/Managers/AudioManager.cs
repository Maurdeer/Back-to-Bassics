using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class AudioManager : Singleton<AudioManager>
{
    private List<EventInstance> eventInstances;
    private Dictionary<string, EventInstance> ambienceInstances;
    private void Awake()
    {
        InitializeSingleton();
        eventInstances = new List<EventInstance>();
        ambienceInstances = new Dictionary<string, EventInstance>();
    }
    private void Start()
    {
        //EventInstance beachAmbienceInstance = CreateInstance(FMODEvents.Instance.beach);
        //ambienceInstances.Add("beach", beachAmbienceInstance);
        //beachAmbienceInstance.start();
    }
    public void SetAmbienceParameter(string ambienceName, string parameterName, float parameterValue)
    {
        ambienceInstances[ambienceName].setParameterByName(parameterName, parameterValue);
    }
    public void PlayOnShotSound(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    public void StartBattleMusic(EventReference music)
    {

    }
    public void EndBattleMusic(EventReference music)
    {

    }
    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);  
        return eventInstance;
    }
    public void SetAmbienceVolume(float volume)
    {
        foreach (EventInstance amb in ambienceInstances.Values)
        {
            Debug.Log("VolumeDecrease");
            amb.setVolume(volume);
        }
    }
    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
