using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingTag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<RecordingManager>().Register(this.gameObject.name, this.gameObject);
    }

}
