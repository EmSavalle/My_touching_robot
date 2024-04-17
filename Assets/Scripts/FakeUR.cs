using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeUR : MonoBehaviour
{
    Transform[] joints;
    Quaternion[] joints_init;
    public UR5 real;

    public List<RobotPositionData> recordedPositions = new List<RobotPositionData>();
    public List<RobotPositionData> recordedPositionsNo = new List<RobotPositionData>();
    public List<RobotPositionData> recordedPositionsLight = new List<RobotPositionData>();
    public List<RobotPositionData> recordedPositionsHard = new List<RobotPositionData>();
    private int currentPositionIndex = 0;

    public bool recording;
    public bool replay;
    private bool recordingStarted;
    public bool recordingDone;
    public bool replaying;
    public bool replayNo, replayLight, replayHard;

    public List<string> recordings = new List<string>();

    private string defaultType = "";
    // Start is called before the first frame update
    void Start()
    {
        joints = new Transform[6];

        joints_init = new Quaternion[6];

        joints[0] = transform.Find("Fix/Base");

        joints[1] = transform.Find("Fix/Base/Shoulder");

        joints[2] = transform.Find("Fix/Base/Shoulder/Elbow");

        joints[3] = transform.Find("Fix/Base/Shoulder/Elbow/Wrist1");

        joints[4] = transform.Find("Fix/Base/Shoulder/Elbow/Wrist1/Wrist2");

        joints[5] = transform.Find("Fix/Base/Shoulder/Elbow/Wrist1/Wrist2/Wrist3");

        for (int i = 0; i < 6; i++)
        {
            joints_init[i] = joints[i].transform.localRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (replayNo)
        {
            replayNo = false;
            ReplayCoroutine("No");
        }
        if (replayLight)
        {
            replayLight = false;
            ReplayCoroutine("Light");
        }
        if (replayHard)
        {
            replayHard = false;
            ReplayCoroutine("Hard");
        }
        if (!replaying)
        {
            for (int i = 0; i < 6; i++)
            {
                joints[i].localRotation = real.joints[i].localRotation;
            }
        }/*
        if (recording && !recordingStarted)
        {
            StartRecording(defaultType);
            recordingStarted = true;
        }
        if (!recording && recordingStarted)
        {
            StopCoroutine(RecordingCoroutine(defaultType));
            recordingStarted = false;
            recordingDone = true;
        }
        if (replay)
        {
            //Debug.Log("Replaying");
            StartReplay();
            replay = false;
        }*/
    }




    // Use this function to start recording
    public void StartRecording(string type)
    {
        recording = true;
        StartCoroutine(RecordingCoroutine(type));
    }    // Use this function to start recording
    public void StopRecording(string type)
    {
        recordings.Add(type);
        recording = false;
        StopCoroutine(RecordingCoroutine(type));
    }

    // Use this function to start replaying
    public void StartReplay(string type)
    {
        Debug.Log("Replaying " + type + " touch");
        StartCoroutine(ReplayCoroutine(type));
    }

    // Capture and record the robot's position during its movement
    public IEnumerator RecordingCoroutine(string type)
    {
        while (recording)
        {
            Debug.Log("Recording");
            RecordRobotPosition(type);
            yield return new WaitForSeconds(Time.deltaTime); // Adjust based on your recording frequency
        }
        yield break;
    }

    public void RecordRobotPosition(string type)
    {
        RobotPositionData data = new RobotPositionData();
        data.timestamp = Time.time;
        data.jointPositions = new Quaternion[6]; // Replace with your actual joint positions

        // Populate data.jointPositions with the real robot's joint positions
        for (int i = 0; i < 6; i++)
        {
            data.jointPositions[i]= real.joints[i].localRotation;
        }

        //Debug.Log("Recording"+","+ recordedPositions.Count.ToString());
        if(type == "")
        {
            recordedPositions.Add(data);
        }
        else if (type == "No" || type == "no")
        {
            recordedPositionsNo.Add(data);
        }
        else if (type == "Light" || type == "light")
        {
            recordedPositionsLight.Add(data);
        }
        else if (type == "Hard" || type == "hard")
        {
            recordedPositionsHard.Add(data);
        }
    }

    // Replay the recorded movement on the virtual robot
    public IEnumerator ReplayCoroutine(string type)
    {
        replaying = true;
        currentPositionIndex = 0;
        List<RobotPositionData> rp = new List<RobotPositionData>();
        switch (type)
        {
            case "No":
                rp = recordedPositionsNo;
                break;
            case "no":
                rp = recordedPositionsNo;
                break;
            case "Light":
                rp = recordedPositionsLight;
                break;
            case "light":
                rp = recordedPositionsLight;
                break;
            case "Hard":
                rp = recordedPositionsHard;
                break;
            case "hard":
                rp = recordedPositionsHard;
                break;
            default:
                rp = recordedPositions;
                break;
        }
        //Debug.Log("Replaying "+ currentPositionIndex.ToString()+","+ recordedPositions.Count.ToString());
        while (currentPositionIndex < rp.Count)
        {
            for (int i = 0; i < 6; i++)
            {
                
                joints[i].localRotation = rp[currentPositionIndex].jointPositions[i];
            }

            currentPositionIndex++;
            yield return new WaitForSeconds(Time.deltaTime); // Simulate real-time replay
        }
        replaying = false;
        yield break;
    }


}


// Data structure to store robot position data
[System.Serializable]
public struct RobotPositionData
{
    public float timestamp;
    public Quaternion[] jointPositions;
}


