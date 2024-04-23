using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    private FMOD.Studio.EventInstance fmodStudioEvent;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayEvent(string eventName)
    {
        fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance(eventName);
        fmodStudioEvent.start();
        fmodStudioEvent.release();
    }
}
