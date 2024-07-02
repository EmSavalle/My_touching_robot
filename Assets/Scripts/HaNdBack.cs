
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
    public List<List<int>> listOfNb1 = new List<List<int>>();
    public List<List<int>> listOfNb3 = new List<List<int>>();
    public int numberList = -1;
    public int numberList1 = -1;
    public int numberList3 = -1;
    public bool finished;
    public XRController rightHand,leftHand;
    // Start is called before the first frame update
    void Start()
    {
        listOfNb = new List<List<int>>
        {
            new List<int> { 4, 4, 2, 6, 8, 1, 9, 7, 1, 1, 1, 1, 8, 3, 3, 9, 9, 9, 9, 6, 6, 3, 3, 8, 4, 4, 2, 7, 7, 7, 8, 4, 1, 3, 7, 3, 3, 2, 7, 4, 4, 4, 3, 3, 9, 2, 3, 2, 4, 5 },
            new List<int> { 6, 6, 3, 3, 3, 3, 7, 7, 7, 3, 5, 4, 7, 7, 9, 3, 1, 6, 1, 1, 1, 7, 2, 2, 2, 5, 1, 7, 2, 6, 6, 6, 1, 1, 2, 2, 3, 4, 8, 7, 9, 2, 1, 3, 8, 8, 8, 5, 9, 6 },
            new List<int> { 9, 4, 3, 2, 4, 9, 9, 2, 7, 9, 2, 3, 8, 6, 3, 8, 8, 3, 7, 8, 3, 2, 4, 1, 2, 4, 1, 3, 1, 2, 3, 1, 9, 8, 6, 9, 8, 6, 7, 5, 7, 6, 6, 5, 6, 8, 1, 3, 6, 7 },
            new List<int> { 7, 1, 3, 8, 6, 8, 8, 6, 5, 2, 6, 4, 1, 6, 5, 1, 6, 5, 1, 1, 5, 8, 8, 5, 8, 9, 2, 8, 2, 8, 7, 7, 5, 7, 7, 8, 7, 1, 4, 4, 6, 5, 4, 6, 4, 5, 1, 1, 7, 6 }

        }; 
        listOfNb1 = new List<List<int>>
        {
            new List<int> { 4, 4, 2, 6, 8, 1, 9, 7, 1, 1, 1, 1, 8, 3, 3, 9, 9, 9, 9, 6, 6, 3, 3, 8, 4, 4, 2, 7, 7, 7, 8, 4, 1, 3, 7, 3, 3, 2, 7, 4, 4, 4, 3, 3, 9, 2, 3, 2, 4, 5 },
            new List<int> { 6, 6, 3, 3, 3, 3, 7, 7, 7, 3, 5, 4, 7, 7, 9, 3, 1, 6, 1, 1, 1, 7, 2, 2, 2, 5, 1, 7, 2, 6, 6, 6, 1, 1, 2, 2, 3, 4, 8, 7, 9, 2, 1, 3, 8, 8, 8, 5, 9, 6 }

        }; 
        listOfNb3 = new List<List<int>>
        {
            new List<int> { 9, 4, 3, 2, 4, 9, 9, 2, 7, 9, 2, 3, 8, 6, 3, 8, 8, 3, 7, 8, 3, 2, 4, 1, 2, 4, 1, 3, 1, 2, 3, 1, 9, 8, 6, 9, 8, 6, 7, 5, 7, 6, 6, 5, 6, 8, 1, 3, 6, 7 },
            new List<int> { 7, 1, 3, 8, 6, 8, 8, 6, 5, 2, 6, 4, 1, 6, 5, 1, 6, 5, 1, 1, 5, 8, 8, 5, 8, 9, 2, 8, 2, 8, 7, 7, 5, 7, 7, 8, 7, 1, 4, 4, 6, 5, 4, 6, 4, 5, 1, 1, 7, 6 }

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

    public void startNBack(int nb_number)
    {
        if(nb_number == 1)
        {
            numberList1 += 1;
            nbacksuite = listOfNb1[numberList1];
        }
        else
        {

            numberList3 += 1;
            nbacksuite = listOfNb3[numberList3];
        }
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
