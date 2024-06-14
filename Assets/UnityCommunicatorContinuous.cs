using LSL4Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using System.IO;
using System;

public class UnityCommunicatorContinuous : MonoBehaviour
{
    public enum OVMarker { ApproachHaptic, ApproachVisual, TouchHaptic, TouchVisual, TrialNBackHaptic, TrialHaptic, TrialNBackVisu, TrialVisu, EndOfTrial }
    public Dictionary<OVMarker, string> convMarkerStr = new Dictionary<OVMarker, string>();
    public Dictionary<OVMarker, int> convMarkerInt = new Dictionary<OVMarker, int>();

    public string StreamName = "LSL4Unity.Samples.SimpleCollisionEvent";
    public string StreamType = "Markers";
    public StreamOutlet outlet;
    public float[] sample = {  };

    public Infos info;

    public bool save;
    public string saveFileName;

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

        //Trial Marker
        convMarkerStr.Add(OVMarker.TrialHaptic, "OVTK_StimulationId_Label_00");
        convMarkerInt.Add(OVMarker.TrialHaptic, 33024);
        convMarkerStr.Add(OVMarker.TrialNBackHaptic, "OVTK_StimulationId_Label_01");
        convMarkerInt.Add(OVMarker.TrialNBackHaptic, 33025);
        convMarkerStr.Add(OVMarker.TrialVisu, "OVTK_StimulationId_Label_02");
        convMarkerInt.Add(OVMarker.TrialVisu, 33026);
        convMarkerStr.Add(OVMarker.TrialNBackVisu, "OVTK_StimulationId_Label_03");
        convMarkerInt.Add(OVMarker.TrialNBackVisu, 33027);
        convMarkerStr.Add(OVMarker.EndOfTrial, "OVTK_StimulationId_Label_04");
        convMarkerInt.Add(OVMarker.EndOfTrial, 33028);
        //Stimulation Marker
        convMarkerStr.Add(OVMarker.ApproachHaptic, "OVTK_StimulationId_Label_10");
        convMarkerInt.Add(OVMarker.ApproachHaptic, 33029);
        convMarkerStr.Add(OVMarker.ApproachVisual, "OVTK_StimulationId_Label_11");
        convMarkerInt.Add(OVMarker.ApproachVisual, 33030);
        convMarkerStr.Add(OVMarker.TouchHaptic, "OVTK_StimulationId_Label_12");
        convMarkerInt.Add(OVMarker.TouchHaptic, 33031);
        convMarkerStr.Add(OVMarker.TouchVisual, "OVTK_StimulationId_Label_13");
        convMarkerInt.Add(OVMarker.TouchVisual, 33032);

        saveFileName += info.participant.ToString() + ".txt";
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void savePressure(float pressure)
    {
        File.AppendAllText(saveFileName, "Pressure:" + pressure.ToString() + ":" + Time.time.ToString() + "\n");

    }
    public void savePressure(string pressure)
    {
        File.AppendAllText(saveFileName, "Pressure:" + pressure + ":" + Time.time.ToString() + "\n");

    }
    public void SendMarker(OVMarker type)
    {
        int stim = convMarkerInt[type];

        if (outlet != null)
        {
            sample[0] = stim;
            outlet.push_sample(sample);
            savePressure(stim);
        }
    }
    public void SendData(float pressure)
    {

        if (outlet != null)
        {
            sample[0] = pressure;
            outlet.push_sample(sample);
            savePressure(pressure);
        }
    }
}





