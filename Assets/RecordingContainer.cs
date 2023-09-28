using System.Collections.Generic;
using UnityEngine;

public class RecordingContainer
{
    public Dictionary<string, GameObject> clonedGameObjects = new Dictionary<string, GameObject>();
    public Dictionary<string, List<Pose>> recordedTransforms = new Dictionary<string, List<Pose>>();
    public string audio_source_key;
    public AudioClip audioClip;
    public int frame_count = 0;

    public RecordingContainer()
    {
       
    }
}