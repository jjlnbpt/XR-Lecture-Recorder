using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioExample : MonoBehaviour
{
    public int microphone_index = 2;

    // Start recording with built-in Microphone and play the recorded audio right away
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(Microphone.devices[microphone_index], true, 60 * 10, AudioSettings.outputSampleRate);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(Microphone.devices[microphone_index]) > 0))
        {

        }
        print("Started Playback");
        print(Microphone.IsRecording(Microphone.devices[microphone_index]));
        audioSource.Play();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && Microphone.IsRecording(Microphone.devices[2]))
        {
            Microphone.End(Microphone.devices[microphone_index]);
        }
    }


}
