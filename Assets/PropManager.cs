using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropManager : MonoBehaviour
{

    private List<GameObject> gameObjects = new List<GameObject>();
    private RecordingManager recordingManager;

    // Start is called before the first frame update
    void Start()
    {
        recordingManager = FindObjectOfType<RecordingManager>();
    }

    public void Register(GameObject obj)
    {
        gameObjects.Add(obj);
    }

    // Update is called once per frame
    void Update()
    {
        if (recordingManager.IsCurrentlyPlayback())
        {
            foreach(var obj in gameObjects){
                obj.SetActive(false);
            }
        }
        else
        {
            foreach(var obj in gameObjects)
            {
                obj.SetActive(true);
            }
        }
    }
}
