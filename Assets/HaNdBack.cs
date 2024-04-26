
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class HaNdBack : MonoBehaviour
{
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
    public bool finished;
    // Start is called before the first frame update
    void Start()
    {
        if(nbacksuite.Count == 0)
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
    }

    // Update is called once per frame
    void Update()
    {
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
        hasStarted = true;
        finished = false;
        currnback = 0;
        setText();
    }
    public void nextNback()
    {
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
        }
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
            Debug.Log("Toucn");
            if (hasStarted && Time.time > lastCol + minTimeTouch)
            {
                nextNback();
            }
        }
        else
        {
            Debug.Log("Contact");
            Debug.Log(collision.gameObject.name);
        }
    }

    void OnTriggerExit(Collider collision)
    {

        // Restore the original shape when collision ends
    }

}