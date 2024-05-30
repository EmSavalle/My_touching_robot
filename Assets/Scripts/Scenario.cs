using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Scenario : MonoBehaviour
{
    public bool startScenario;
    public URTest manager;
    public FakeUR fake;
    public string list; //Scenario list style : s5_h1_t1_h1... PositionTime_PositionTime_...
    public float lastTriggerTime;
    public float delay = 1000;
    public string[] subs;
    public int cpt = 0;
    private int cptSubs;
    private float lastSeq;
    bool visu,phy,record;
    private char touchType;
    public int repetitions;
    public bool replay;
    public GameObject robotRig;
    public GameObject ballRig;
    public GameObject fakeRig;
    private bool waitingInitRecord = false;
    private float initRecordTime;
    private float recordDelay = 2;
    public float delaySeq;
    private List<string> recordType = new List<string>{ "No", "Light", "Hard" };
    public int cptRecord = 0;
    private bool playingScenario;
    public List<string> recordDone =    new List<string>();
    public float touchTime = 0.25f;
    public bool playScenario;


    public QuestionnaireManager quest;
    public GameObject questionnaireHolder;
    // Start is called before the first frame update
    void Start()
    {
        subs = list.Split('_');

    }

    // Update is called once per frame
    void Update()
    {
        if (false)
        {
            /*if (record && !waitingInitRecord)
            {
                waitingInitRecord = true;
                initRecordTime = Time.time;
                cpt = 0;
            }
            if(replay && fake.recordingDone)
            {
                fake.StartReplay();
                replay = false;
            }
            if (record && waitingInitRecord && Time.time > initRecordTime + recordDelay)
            {
                if(!fake.recording)
                {
                    fake.StartRecording(recordType[cptRecord]);
                }

                if (cpt < seq.Length && (cpt == 0 || lastTriggerTime + delay < Time.time))
                {
                    string ss = seq[cpt];
                    int time = Int32.Parse(ss.Split("-")[1]);
                    string cond = ss.Split("-")[0];
                    Debug.Log("Step :" + ss);
                    if (cond.Contains("s"))
                    {
                        manager.safe();
                    }
                    else if (cond.Contains("h"))
                    {
                        manager.hover();
                    }
                    else if (cond.Contains("t"))
                    {
                        switch (recordType[cptRecord])
                        {
                            case "No":
                                manager.goTo(manager.noPos, false);
                                break;
                            case "Light":
                                manager.goTo(manager.lightPos, false);
                                break;
                            case "Hard":
                                manager.goTo(manager.hardPos, false);
                                break;
                        }

                    }
                    else
                    {
                        Debug.Log("Command unrecognized");
                    }

                    delay = time;
                    Debug.Log("Waiting " + delay.ToString());
                    lastTriggerTime = Time.time;
                    cpt += 1;
                }
                else if (cpt == seq.Length && lastTriggerTime + delay < Time.time)
                {
                    fake.StopRecording(recordType[cptRecord]);
                    cptRecord += 1;
                    cpt = 0;
                    if(cptRecord== recordType.Count)
                    {
                        record = false;
                        cptRecord = 0;
                    }
                }
            }
            if (startScenario)
            {

                if (cpt == 0 && (cptSubs == 0 || Time.time > lastSeq +  delaySeq))
                {
                    subs = list.Split('_');
                    string type = subs[cptSubs];
                    Debug.Log("Subs "+type);

                    visu = type[1] == 't';
                    phy = type[2] == 't';
                    touchType = type[3];
                    if (!phy && fake.recordingDone)
                    {
                        switch (touchType)
                        {
                            case 'n':
                                fake.StartReplay("No");
                                break;
                            case 'l':
                                fake.StartReplay("Light");
                                break;
                            case 'h':
                                fake.StartReplay("Hard");
                                break;
                        }
                        fake.replay = true;
                    }
                    robotRig.SetActive(visu);
                    ballRig.SetActive(visu);
                    fakeRig.SetActive(visu);
                    if (!phy)
                    {
                        robotRig.SetActive(false);
                        ballRig.SetActive(false);

                    }



                    Debug.Log("Sequence" + cptSubs.ToString() + ", visu " + visu.ToString() + ", physique" + phy.ToString());
                }    
                if(cpt < subs.Length && ((cpt == 0 && (cptSubs==0 || Time.time>lastSeq +delaySeq)) || (lastTriggerTime + delay < Time.time && Time.time > lastSeq + delaySeq)))
                {
                    string ss = seq[cpt];
                    int time = Int32.Parse(ss.Split("-")[1]);
                    string cond = ss.Split("-")[0];
                    Debug.Log("Step :" + ss);
                    if (cond.Contains("s") && actualControl && phy)
                    {
                        manager.safe();
                    }
                    else if (cond.Contains("h") && actualControl && phy)
                    {
                        manager.hover();
                    }
                    else if (cond.Contains("t") && actualControl && phy)
                    {
                        if(touchType == 'n')
                        {
                            manager.goTo(manager.noPos, false);
                        }
                        else if (touchType == 'l')
                        {
                            manager.goTo(manager.lightPos, false);
                        }
                        else if (touchType == 'h')
                        {
                            manager.goTo(manager.hardPos, false);
                        }
                    }

                    delay = time;
                    Debug.Log("Waiting " + delay.ToString());
                    lastTriggerTime = Time.time;
                    cpt += 1;
                }
                else if(cpt >= seq.Length)
                {
                    cpt = 0;
                    cptSubs=cptSubs+1;
                    lastSeq = Time.time;
                }
                if(cptSubs >= subs.Length)
                {

                    startScenario = false;
                    cpt = 0;
                    cptSubs = 0;

                }

            }*/
        }
        if (playScenario)
        {
            playScenario = false;
            subs = list.Split('_');
            StartCoroutine(PlayScenario());
        }
    }

    public IEnumerator PlayScenario()
    {
        string tType = "";
        playingScenario = true;
        for (int cptSeq = 0; cptSeq < subs.Length; cptSeq++)
        {
            Debug.Log("Start seq" + cptSeq.ToString());
            record = false;

            string currentSeq = subs[cptSeq];

            phy = currentSeq[1] == 't';
            touchType = currentSeq[2];
            int x1 = currentSeq[currentSeq.Length - 2] - '0';
            int x2 = currentSeq[currentSeq.Length - 1] - '0';
            repetitions = 10 * x1 + x2;
            for (int rep = 0; rep < repetitions; rep ++)
            {

                switch (touchType)
                {
                    case 'n':
                        tType = "no";
                        break;
                    case 'l':
                        tType = "light";
                        break;
                    case 'h':
                        tType = "hard";
                        break;
                    default:
                        tType = "light";
                        break;
                }
                if(!recordDone.Contains(tType) && phy)
                {
                    record = true;
                }
                manager.StartMovement(tType, phy, touchTime, record);
                while (manager.movingCoroutine)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                if (record)
                {
                    recordDone.Add(tType);
                }

                yield return new WaitForSeconds(delaySeq);

            }
            if (!questionnaireHolder.activeSelf)
            {
                questionnaireHolder.SetActive(true);
            }
            quest.StartQuestionnaire();
            while (!quest.finished)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            questionnaireHolder.SetActive(false);

        }
        playingScenario = false;
        yield break;
    }
}
