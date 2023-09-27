using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Timers;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Assertions;
/*using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;*/

public class RecordingManager : MonoBehaviour
{
    private Dictionary<string,GameObject> gameObjects = new Dictionary<string,GameObject>();
    private Dictionary<string, GameObject> clonedGameObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, Queue<Pose>> recordedTransforms = new Dictionary<string, Queue<Pose>>();

    private bool recording_first_frame = true;
    private bool playback_first_frame = true;

    private bool recording = false;
    private bool playback = false;

    private int frame_count = 0;

    private Timer timer;

    private bool input_lock = true;

    public void Register(string key, GameObject gameObject)
    {
        this.gameObjects.Add(key, gameObject);
    }

    private void Start()
    {
        timer = new Timer(2000);
        timer.Elapsed += OnStartupElapsed;
        timer.Start();
    }

    private void OnStartupElapsed(System.Object source, ElapsedEventArgs e)
    {
        input_lock = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !input_lock)
        {
            recording = true;
            playback = false;
        }

        if (Input.GetKeyDown(KeyCode.P) && !input_lock)
        {
            recording = false;
            playback = true;
        }


        Assert.IsTrue(!(recording && playback));

        if (recording) Record();
        if (playback) Play();

    }

    private void Record()
    {
        if (recording_first_frame)
        {
            CloneGameObjects();
            frame_count=0;
            recording_first_frame = false;
        }


        foreach (string key in gameObjects.Keys)
        {
            Transform transform = gameObjects[key].transform;
            recordedTransforms[key].Enqueue(transform.GetWorldPose());
        }

        frame_count++;

    }

    private void CloneGameObjects()
    {
        clonedGameObjects.Clear();
        foreach (var key in gameObjects)
        {
            clonedGameObjects.Add(key.Key, Instantiate(key.Value));
            if(clonedGameObjects[key.Key].TryGetComponent<Rigidbody>(out var rb)){
                Destroy(rb);
            }
            Destroy(clonedGameObjects[key.Key].GetComponent<RecordingTag>());
            clonedGameObjects[key.Key].SetActive(false);
            recordedTransforms[key.Key] = new Queue<Pose>();
        }
    }

    private void Play()
    {
        if (playback_first_frame)
        {
            ActivateClones();
            playback_first_frame = false;
        }
        else if (frame_count == 0)
        {
            DeactivateClones();
            playback_first_frame = true;
            playback = false;
            return;
        }

        foreach (string key in clonedGameObjects.Keys)
        {
            try
            {
                Pose recordedTransform = recordedTransforms[key].Dequeue();
                clonedGameObjects[key].transform.SetWorldPose(recordedTransform);
            }
            catch (InvalidOperationException ex)
            {
                continue;
            }
        }

        frame_count--;
    }

    private void ActivateClones()
    {
        foreach (var key in clonedGameObjects)
        {
            clonedGameObjects[key.Key].SetActive(true);
        }
    }

    private void DeactivateClones()
    {
        foreach (var key in clonedGameObjects)
        {
            clonedGameObjects[key.Key].SetActive(false);
        }
    }
}


/*class RecordedTransform
{
    private Vector3 position;
    private Vector3 localPosition;
    private Quaternion rotation;
    private Quaternion localRotation;
    private Vector3 scale;

    public RecordedTransform(Transform transform)
    {
        this.position = transform.position;
        this.localPosition = transform.localPosition;
        this.rotation = transform.rotation;
        this.localRotation = transform.localRotation;
        this.scale = transform.localScale;
    }

    private RecordedTransform(RecordedTransform transform)
    {
        this.position = transform.position;
        this.localPosition = transform.localPosition;
        this.rotation = transform.rotation;
        this.localRotation = transform.localRotation;
        this.scale = transform.scale;
    }

    public void UpdateTransform(Transform transform)
    {
        transform.position = position;
        transform.localPosition = localPosition;
        transform.rotation = rotation;
        transform.localRotation = localRotation;
        transform.localScale = scale;
    }
}*/
