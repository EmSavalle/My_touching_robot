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
    bool visu,phy,record,nb,questionnaire, information,embQ,nasaQ;
    private char touchType;
    public int repetitions, nb_number;
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

    public HaNdBack nback;

    public QuestionnaireManager quest;
    public QuestionnaireManager informationPannel;
    public GameObject questionnaireHolder;
    public GameObject informationPannelHolder;
    [Header("Communication")]
    public UnityCommunicator comms;

    public List<Trial> conditions;

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
        foreach (Trial t in conditions)
        {
            Debug.Log("Start trial");
            record = false;


            phy = t.phy;
            if (!phy)
            {
                fake.blockMimic = true;
                manager.robotRig.SetActive(false);
                manager.ballRig.GetComponent<MeshRenderer>().enabled = false;
                manager.fakeRig.SetActive(true);
            }
            else
            {
                fake.blockMimic = false;
                manager.robotRig.SetActive(true);
                manager.ballRig.GetComponent<MeshRenderer>().enabled = true;
                manager.fakeRig.SetActive(false);
            }
            if (t.touchType == TouchType.Hard)
            {
                tType = "hard";
            }
            else if (t.touchType == TouchType.Light)
            {
                tType = "light";
            }
            else if (t.touchType == TouchType.No)
            {
                tType = "no";
            }
            else
            {
                tType = "hard";
            }

            nb = t.nb;
            nb_number = t.nb_number;
            repetitions = t.repet;
            questionnaire = t.questionnaire;
            information = t.information;
            nasaQ = t.workloadQ;
            embQ = t.embodimentQ;
            if (nb)
            {
                if (phy)
                {
                    if (nb_number == 1)
                    {
                        comms.SendMarker(UnityCommunicator.OVMarker.TrialNBackHapticLow);
                    }
                    else
                    {
                        comms.SendMarker(UnityCommunicator.OVMarker.TrialNBackHapticHigh);
                    }
                }
                else
                {
                    if (nb_number == 1)
                    {
                        comms.SendMarker(UnityCommunicator.OVMarker.TrialNBackVisuLow);
                    }
                    else
                    {
                        comms.SendMarker(UnityCommunicator.OVMarker.TrialNBackVisuHigh);
                    }
                }
            }
            else 
            { 
                if (phy)
                {
                    comms.SendMarker(UnityCommunicator.OVMarker.TrialHaptic);
                }
                else
                {

                    comms.SendMarker(UnityCommunicator.OVMarker.TrialVisu);
                }
            }

            if (information)
            {
                if (!informationPannelHolder.activeSelf)
                {
                    informationPannelHolder.SetActive(true);
                }
                informationPannel.EmptyQuestionnaires();
                informationPannel.setInformationPannel(nb, nb_number);
                informationPannel.StartQuestionnaire();
                while (!informationPannel.finished)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                informationPannelHolder.SetActive(false);
            }

            Debug.Log("Checking Nback");
            if (nb)
            {
                nback.startNBack();
            }
            for (int rep = 0; rep < repetitions; rep ++)
            {                
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
            if (nb)
            {
                nback.stopNBack();
            }
            comms.SendMarker(UnityCommunicator.OVMarker.EndOfTrial);
            if (questionnaire)
            {
                if (!questionnaireHolder.activeSelf)
                {
                    questionnaireHolder.SetActive(true);
                }
                quest.SetupQuestionnaire(embQ, nasaQ);
                quest.StartQuestionnaire();
                while (!quest.finished)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                questionnaireHolder.SetActive(false);
            }
            

            manager.robotRig.SetActive(true);
            fake.blockMimic = false;
            manager.ballRig.GetComponent<MeshRenderer>().enabled = true;

        }

        if (false)
        {
            /* Pr� Trial struct
             * for (int cptSeq = 0; cptSeq < subs.Length; cptSeq++)
            {
                Debug.Log("Start seq" + cptSeq.ToString());
                record = false;

                string currentSeq = subs[cptSeq];

                phy = currentSeq[0] == 't';
                touchType = currentSeq[1];
                nb = currentSeq[2] == 't';
                int x1 = currentSeq[currentSeq.Length - 2] - '0';
                int x2 = currentSeq[currentSeq.Length - 1] - '0';
                repetitions = 10 * x1 + x2;
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
                        tType = "hard";
                        break;
                }
                nback.startNBack();
                for (int rep = 0; rep < repetitions; rep++)
                {
                    if (!recordDone.Contains(tType) && phy)
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
                nback.stopNBack();
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
            */
        }
        playingScenario = false;
        yield break;
    }
}


[System.Serializable]
public struct Trial
{
    public bool phy;
    public bool nb;
    public int nb_number;
    public int repet;
    public TouchType touchType;
    public string name;
    public bool information;
    public bool questionnaire;
    public bool embodimentQ;
    public bool workloadQ;
}

public enum TouchType { Hard,Light,No};