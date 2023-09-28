using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private string active_microphone;

    private void Start()
    {
        active_microphone = Microphone.devices[0];
    }

    public string[] GetMicrophoneNames()
    {
        return Microphone.devices;
    }

    public void SetActiveMicrophone(string name)
    {
        active_microphone = name;
    }

    public string GetActiveMocrophone()
    {
        return active_microphone;
    }

    public AudioClip StartRecording()
    {
        return Microphone.Start(active_microphone, false, 60 * 59, AudioSettings.outputSampleRate);
    }

    public bool IsRecording()
    {
        return Microphone.IsRecording(active_microphone);
    }

    public void EndRecording()
    {
        Microphone.End(active_microphone);
    }


}
