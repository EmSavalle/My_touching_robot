using LSL4Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using System.IO;
using System;

public class TimerChecker : MonoBehaviour
{


    public string StreamName = "Checker";
    public string StreamType = "Markers";
    public StreamOutlet outlet;
    public float[] sample = { };

    public float timing;

    // Start is called before the first frame update
    void Start()
    {
        var hash = new Hash128();
        hash.Append(StreamName);
        hash.Append(StreamType);
        hash.Append(gameObject.GetInstanceID());
        StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, LSL.LSL.IRREGULAR_RATE,
            channel_format_t.cf_float32, hash.ToString());
        outlet = new StreamOutlet(streamInfo);
        StartCoroutine(ContinuousStim());

    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator ContinuousStim()
    {
        while (true)
        {
            SendData();
            yield return new WaitForSeconds(timing);
        }
    }
    public void SendData()
    {
        if (outlet != null)
        {
            sample[0] = 33279;
            outlet.push_sample(sample);
        }
    }
}





