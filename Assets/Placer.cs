using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public URTest ur;
    public GameObject locationBase;
    public GameObject locationMid;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (locationBase == null)
        {
            if (ur.actualAlignmentPointLeap == null)
            {
                for (int i = 0; i < ur.alignmentPointAvatar.transform.childCount; i++)
                {
                    String Go = ur.alignmentPointAvatar.transform.GetChild(i).name;
                    if (Go.Contains("AvatarCCHandsInteractionLeap"))
                    {
                        locationBase = ur.alignmentPointAvatar.transform.GetChild(i).transform.Find("LeftHand/Tracked Root L Hand/L Hand").gameObject;
                        locationMid = ur.alignmentPointAvatar.transform.GetChild(i).transform.Find("LeftHand/Tracked Root L Hand/L Hand/CC_Base_L_Middle1").gameObject;
                    }
                }
            }
        }
        else
        {
            float x = locationBase.transform.position.x + (locationMid.transform.position.x - locationBase.transform.position.x) / 2;
            float y = locationBase.transform.position.y + (locationMid.transform.position.y - locationBase.transform.position.y) / 2;
            float z = locationBase.transform.position.z + (locationMid.transform.position.z - locationBase.transform.position.z) / 2;
            transform.position = new Vector3(x,y,z);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // Apply the squish effect
    }

    void OnCollisionExit(Collision collision)
    {

        // Restore the original shape when collision ends
    }

}
