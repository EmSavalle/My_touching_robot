using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Scenario : MonoBehaviour
{
    public bool startScenario;
    public URTest manager;
    public string list; //Scenario list style : s5_h1_t1_h1... PositionTime_PositionTime_...
    public bool actualControl = false;
    public float lastTriggerTime;
    public float delay = 1000;
    public string[] subs;
    public int cpt = 0;
    // Start is called before the first frame update
    void Start()
    {
        subs = list.Split('_');
    }

    // Update is called once per frame
    void Update()
    {
        if (startScenario)
        {
            if (cpt == 0)
            {
                subs = list.Split('_');
            }
            
            if(cpt < subs.Length && (cpt == 0 || lastTriggerTime + delay < Time.time))
            {
                string ss = subs[cpt];
                int time = Int32.Parse(ss.Split("-")[1]);
                string cond = ss.Split("-")[0];
                Debug.Log("Step :" + ss);
                if (cond.Contains("s") && actualControl)
                {
                    manager.safe();
                }
                else if (cond.Contains("h") && actualControl)
                {
                    manager.hover();
                }
                else if (cond.Contains("t") && actualControl)
                {
                    manager.goTo(manager.target,false);
                }
                
                delay = time;
                Debug.Log("Waiting " + delay.ToString());
                lastTriggerTime = Time.time;
                cpt += 1;
            }
            else if(cpt >= subs.Length)
            {
                startScenario = false;
                cpt = 0;
            }
            
        }
    }
}
