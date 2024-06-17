
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class HaNdBack : MonoBehaviour
{
    public Infos info;
    public URTest ur;
    public GameObject locationBase;
    public GameObject locationMid;
    public TextMeshProUGUI text;
    public bool hasStarted = false;
    public List<int> nbacksuite;
    public int currnback=0;
    private float minTimeTouch = 0.5f;
    private float lastCol = 0;
    public GameObject alignmentPointAvatar;

    public bool pressed = false;
    private List<bool> results = new List<bool>();
    public List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();

    public List<List<int>> listOfNb = new List<List<int>>();
    public int numberList = -1;
    public bool finished;
    public XRController rightHand,leftHand;
    // Start is called before the first frame update
    void Start()
    {
        listOfNb = new List<List<int>>
        {
            new List<int> { 3, 7, 2, 2, 5, 2, 2, 6, 1, 1, 7, 7, 7, 7, 2, 4, 4, 5, 5, 8, 9, 9, 9, 9, 8, 8, 8, 9, 9, 9, 5, 8, 7, 1, 9, 7, 9, 9, 9, 4, 9, 2, 8, 2, 9, 1, 3, 5, 6, 3 },
            new List<int> { 3, 3, 9, 8, 9, 2, 3, 3, 3, 6, 7, 2, 4, 7, 2, 2, 3, 3, 3, 9, 9, 1, 9, 7, 7, 5, 5, 5, 7, 7, 7, 7, 1, 1, 2, 5, 5, 8, 8, 5, 5, 1, 5, 9, 2, 5, 2, 3, 2, 8 },
            new List<int> { 5, 4, 8, 1, 6, 1, 2, 1, 2, 5, 6, 5, 6, 5, 6, 6, 6, 6, 7, 1, 6, 2, 9, 5, 9, 5, 6, 7, 6, 7, 6, 7, 4, 4, 4, 4, 8, 8, 1, 3, 3, 9, 6, 6, 8, 7, 1, 3, 4, 9 },
            new List<int> { 6, 4, 3, 3, 1, 2, 1, 8, 3, 5, 3, 9, 4, 9, 4, 9, 1, 6, 4, 6, 4, 4, 3, 6, 8, 3, 8, 3, 3, 9, 7, 9, 7, 9, 4, 9, 4, 9, 8, 7, 1, 3, 8, 4, 8, 4, 2, 7, 7, 9 },
            new List<int> { 1, 7, 2, 3, 4, 2, 3, 9, 3, 6, 9, 8, 6, 2, 2, 6, 3, 2, 5, 1, 6, 5, 2, 6, 5, 2, 1, 5, 1, 1, 6, 1, 1, 6, 1, 3, 3, 1, 1, 1, 9, 5, 5, 5, 1, 6, 7, 6, 1, 3 },
            new List<int> { 9, 3, 4, 8, 5, 3, 8, 5, 1, 6, 5, 6, 5, 8, 3, 5, 9, 4, 5, 9, 4, 5, 2, 4, 4, 4, 4, 2, 4, 3, 2, 3, 3, 4, 9, 3, 4, 4, 3, 4, 8, 5, 1, 2, 6, 9, 7, 3, 6, 8 }
        };

        if (nbacksuite.Count == 0)
        {
            for(int i = 0; i < 10; i++)
            {
                nbacksuite.Add(-1);
            }
        }
        if (!hasStarted)
        {
            text.text = "";
        }
        finished = false;

        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            pressed = true;
        }
        Debug.Log("Devices " + inputDevices.Count.ToString());
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);

        
        foreach (var device in inputDevices)
        {
            bool triggerValue;

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
            {
                Debug.Log("Trigger pressed n-back");
                pressed = true;
            }
        }
        


        if (locationBase == null)
        {
            
            for (int i = 0; i < alignmentPointAvatar.transform.childCount; i++)
            {
                String Go = alignmentPointAvatar.transform.GetChild(i).name;
                if (Go.Contains("AvatarCCHandsInteractionLeap"))
                {
                    locationBase = alignmentPointAvatar.transform.GetChild(i).transform.Find("LeftHand/Tracked Root L Hand/L Hand").gameObject;
                    locationMid = alignmentPointAvatar.transform.GetChild(i).transform.Find("LeftHand/Tracked Root L Hand/L Hand/CC_Base_L_Middle1").gameObject;
                }
            }
        }
        else
        {
            float x = locationBase.transform.position.x + (locationMid.transform.position.x - locationBase.transform.position.x) / 2;
            float y = locationBase.transform.position.y + (locationMid.transform.position.y - locationBase.transform.position.y) / 2;
            float z = locationBase.transform.position.z + (locationMid.transform.position.z - locationBase.transform.position.z) / 2;
            transform.position = new Vector3(x, y, z);
        }
    }
    public void startNBack()
    {
        numberList += 1;
        nbacksuite = listOfNb[numberList];
        hasStarted = true;
        finished = false;
        currnback = 0;
        setText();
    }
    public void stopNBack()
    {
        if (!finished)
        {
            writeAnswer();
        }
        hasStarted = false;
        finished = true;
        currnback = 0;
        setText();
    }
    public void nextNback()
    {
        results.Add(pressed);
        pressed = false;
        lastCol = Time.time;
        if (currnback < nbacksuite.Count) { 
            int v = nbacksuite[currnback];
            if (v != -1)
            {
                setText(v);
            }
            else
            {
                setText();
            }
            currnback += 1;
        }
        else
        {
            finished = true;
            writeAnswer();
        }
    }
    private void writeAnswer()
    {
        String value = "";
        foreach (bool b in results)
        {
            if (b)
            {
                value = value + "1";
            }
            else
            {
                value = value + "0";
            }
        }
        using (StreamWriter writer = File.AppendText(info.nbFile))
        {
            writer.WriteLine(info.currcond + ":" + value);
        }
        results = new List<bool>();
    }
    public void setText()
    {
        text.text = "";
    }
    public void setText(int v)
    {
        text.text = v.ToString();
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Contact")
        {
            if (hasStarted && Time.time > lastCol + minTimeTouch)
            {
                nextNback();
            }
        }
    }

    void OnTriggerExit(Collider collision)
    {

        // Restore the original shape when collision ends
    }

}
