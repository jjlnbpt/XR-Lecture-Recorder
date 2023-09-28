using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RecordingButton : MonoBehaviour
{
    private Button button;

    private RecordingManager recordingManager;
    [SerializeField]
    private ColorBlock recordingColors;

    private ColorBlock originalColors;

    private string buttonText = "Record";

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();

        originalColors = button.colors;

        recordingManager = FindObjectOfType<RecordingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Text textField = button.GetComponentInChildren<Text>();

        if (recordingManager.IsCurrentlyRecording())
        {
            textField.text = "Recording (" + recordingManager.CurrentRecordingTime() + ")"; 
            button.colors = recordingColors;
        } 
        else
        {
            textField.text = buttonText;
            button.colors = originalColors;
        }
    }
}
