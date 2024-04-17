using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infos : MonoBehaviour
{
    public int participant;
    public string logFile;
    // Start is called before the first frame update
    void Start()
    {
        logFile = participant.ToString() + ".txt";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
