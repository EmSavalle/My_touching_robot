using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class FakeUR : MonoBehaviour
{
    public Transform[] joints;
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
    public bool blockMimic = false;
    public bool replayNo, replayLight, replayHard;
    public Scenario scenario;
    public UnityCommunicator comms;

    public List<string> recordings = new List<string>();

    public bool sendTouch;

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
        if (!replaying && !blockMimic)
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
        data.jointPositions = new List<Quaternion>(); // Replace with your actual joint positions

        // Populate data.jointPositions with the real robot's joint positions
        for (int i = 0; i < 6; i++)
        {
            data.jointPositions.Add(real.joints[i].localRotation);
        }
        if (sendTouch)
        {
            data.sendTouch = true;
            sendTouch = false;
        }
        else
        {
            data.sendTouch = false;
        }
        //Debug.Log("Recording"+","+ recordedPositions.Count.ToString());
        if (type == "")
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
            if (rp[currentPositionIndex].sendTouch)
            {
                comms.SendMarker(UnityCommunicator.OVMarker.TouchVisual);
            }
            currentPositionIndex++;
            yield return new WaitForSeconds(Time.deltaTime); // Simulate real-time replay
        }
        replaying = false;
        yield break;
    }

    public void SaveData(string filePath)
    {
        if (filePath.Contains(".txt"))
        {
            filePath.Replace(".txt", "");
        }
        Save(filePath + "No.txt", recordedPositionsNo);
        Save(filePath + "Light.txt", recordedPositionsLight);
        Save(filePath + "Hard.txt", recordedPositionsHard);
    }
    public void LoadData(string filePath)
    {
        if (filePath.Contains(".txt"))
        {
            filePath.Replace(".txt", "");
        }
        recordedPositionsNo=Load(filePath + "No.txt");
        recordedPositionsLight=Load(filePath + "Light.txt");
        recordedPositionsHard=Load(filePath + "Hard.txt");
        scenario.recordDone.Add("no");
        scenario.recordDone.Add("light");
        scenario.recordDone.Add("hard");

    }
    public void Save(string filePath, List<RobotPositionData> dataList)
    {
        string saveContent = "";
        foreach (RobotPositionData r in dataList)
        {
            for (int i = 0; i < 6; i++)
            {
               
                saveContent = saveContent + r.jointPositions[i].x.ToString() + ";" + r.jointPositions[i].y.ToString() + ";" + r.jointPositions[i].z.ToString() + ";" + r.jointPositions[i].w.ToString() + ";"+r.sendTouch.ToString()+"\n";
            }
            
        }
        File.WriteAllText(filePath, saveContent);
    }

    public List<RobotPositionData> Load(string filePath)
    {
        List<RobotPositionData> readpos = new List<RobotPositionData>();
        List<List<float[]>> dataGroups = new List<List<float[]>>();
        RobotPositionData rr = new RobotPositionData();
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            List<float[]> currentGroup = new List<float[]>();
            int lineCounter = 0;
            bool b = false;
            List<Quaternion> jointPositions = new List<Quaternion>();
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(';');
                b = values[values.Count()-1].Contains("rue");
                values = values.Take(values.Count() - 1).ToArray();
                float[] floatValues = Array.ConvertAll(values, float.Parse);
                float w, x, y, z;
                x = floatValues[0];
                y = floatValues[1];
                z = floatValues[2];
                w = floatValues[3];
                Quaternion q = new Quaternion(x, y, z, w);
                jointPositions.Add(q);
                lineCounter++;

                if (lineCounter == 6)
                {
                    rr = new RobotPositionData();
                    rr.timestamp = 1;
                    rr.jointPositions = new List<Quaternion>(jointPositions);
                    rr.sendTouch = b;
                    b = false;
                    readpos.Add(rr);
                    jointPositions = new List<Quaternion>();
                    lineCounter = 0;
                }
            }

        }
        return readpos;
    }

}


// Data structure to store robot position data
[System.Serializable]
public struct RobotPositionData
{
    public float timestamp;
    public List<Quaternion> jointPositions;
    public bool sendTouch;
}


