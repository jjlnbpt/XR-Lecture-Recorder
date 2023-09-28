using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<RecordingManager>().RegisterAudioSource(this.gameObject.name);
    }
}
