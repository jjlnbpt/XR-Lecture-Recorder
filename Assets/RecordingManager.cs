using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Assertions;

public class RecordingManager : MonoBehaviour
{
    #region Private Members
    private Dictionary<string,GameObject> gameObjects = new Dictionary<string,GameObject>();

    private RecordingContainer recordingContainer;

    private bool recording_first_frame = true;
    private bool playback_first_frame = true;

    private bool recording = false;
    private bool playback = false;

    private int frame_count = 0;

    private Timer timer;

    private bool input_lock = true;

    private string audio_source_key;

    AudioManager audioManager;
    private Stopwatch recording_stopwatch;
    #endregion

    #region Startup and Registration

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        recordingContainer = new RecordingContainer();

        recording_stopwatch = new Stopwatch();
        timer = new Timer(2000);
        timer.Elapsed += OnStartupElapsed;
        timer.Start();
    }

    private void OnStartupElapsed(System.Object source, ElapsedEventArgs e)
    {
        input_lock = false;
    }

    public void Register(string key, GameObject gameObject)
    {
        this.gameObjects.Add(key, gameObject);

    }

    public void RegisterAudioSource(string key)
    {
        audio_source_key = key;
    }

    #endregion

    #region Control Flow
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !input_lock)
        {
            EnterRecordMode();
        }

        if (Input.GetKeyDown(KeyCode.P) && !input_lock)
        {
            EnterPlaybackMode();
        }

        Assert.IsTrue(!(recording && playback));

        if (recording) Record();
        if (playback) Play();

        if(audioManager.IsRecording() && !recording)
        {
            audioManager.EndRecording();
        }

        if(recording_stopwatch.IsRunning && !recording)
        {
            recording_stopwatch.Stop();
            recording_stopwatch.Reset();
        }
    }

    public void EnterRecordMode()
    {
        if (!input_lock)
        {
            recording = !recording;
            playback = false;
            if (recording)
            {
                recording_first_frame = true;
            }
        }
    }

    public void EnterPlaybackMode()
    {
        if (!input_lock)
        {
            recording = false;
            playback = true;
        }
    }

    #endregion

    #region Monitor API
    public bool IsCurrentlyRecording()
    {
        return recording;
    }

    public bool IsCurrentlyPlayback()
    {
        return playback;
    }

    public string CurrentRecordingTime()
    {
        TimeSpan ts = recording_stopwatch.Elapsed;
        string elapsedTime = String.Format("{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        return elapsedTime;
    }

    #endregion

    #region Recording and Playback Logic

    private void Record()
    {
        if (recording_first_frame)
        {
            Destroy(recordingContainer.audioClip);
            recordingContainer = new RecordingContainer();
            recordingContainer.audio_source_key = audio_source_key;
            CloneGameObjects();
            frame_count=0;
            recording_first_frame = false;
            recordingContainer.audioClip = audioManager.StartRecording();
            recording_stopwatch.Start();
        }


        foreach (string key in gameObjects.Keys)
        {
            Transform transform = gameObjects[key].transform;
            recordingContainer.recordedTransforms[key].Add(transform.GetWorldPose());
        }

        recordingContainer.frame_count++;

    }

    private void CloneGameObjects()
    {
        recordingContainer.clonedGameObjects.Clear();
        foreach (var key in gameObjects)
        {
            recordingContainer.clonedGameObjects.Add(key.Key, Instantiate(key.Value));
            if(recordingContainer.clonedGameObjects[key.Key].TryGetComponent<Rigidbody>(out var rb)){
                Destroy(rb);
            }
            Destroy(recordingContainer.clonedGameObjects[key.Key].GetComponent<RecordingTag>());
            if(recordingContainer.audio_source_key == key.Key)
            {
                Destroy(recordingContainer.clonedGameObjects[key.Key].GetComponent<AudioTag>());
            }
            recordingContainer.clonedGameObjects[key.Key].SetActive(false);
            recordingContainer.recordedTransforms[key.Key] = new List<Pose>();
        }
    }

    private void Play()
    {
        if (playback_first_frame)
        {
            ActivateClones();
            playback_first_frame = false;
            frame_count = recordingContainer.frame_count;
            recordingContainer.clonedGameObjects[recordingContainer.audio_source_key].GetComponent<AudioSource>().clip = recordingContainer.audioClip;
            recordingContainer.clonedGameObjects[recordingContainer.audio_source_key].GetComponent<AudioSource>().Play();
        }
        else if (frame_count == 0)
        {
            DeactivateClones();
            playback_first_frame = true;
            playback = false;
            recordingContainer.clonedGameObjects[recordingContainer.audio_source_key].GetComponent<AudioSource>().Stop();
            return;
        }

        foreach (string key in recordingContainer.clonedGameObjects.Keys)
        {
            try
            {
                Pose recordedTransform = recordingContainer.recordedTransforms[key][recordingContainer.recordedTransforms[key].Count - frame_count];
                recordingContainer.clonedGameObjects[key].transform.SetWorldPose(recordedTransform);
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
        foreach (var key in recordingContainer.clonedGameObjects)
        {
            recordingContainer.clonedGameObjects[key.Key].SetActive(true);
        }
    }

    private void DeactivateClones()
    {
        foreach (var key in recordingContainer.clonedGameObjects)
        {
            recordingContainer.clonedGameObjects[key.Key].SetActive(false);
        }
    }

    #endregion
}

