using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.InputSystem;

public class QuestionnaireManager : MonoBehaviour
{
    public Infos info;
    public Button[] buttons;
    public Button confirm;
    public VRSlider slider;
    public TextMeshProUGUI questionnaire, minanswer, maxanswer;
    public int selected;
    public int currentQuestions;
    public List<List<string>> questionnaires = new List<List<string>>();
    private string currQuestion;
    private string currminValue;
    private string currmaxValue;
    private string answerType;
    public bool finished;
    private bool preventRepetition = false;
    List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();
    // Start is called before the first frame update
    void Start()
    {


        // Add some sample data
        AddQuestionnaires("S-How coherent were the physical and visual stimulation?", "Completely incoherent", "Totally coherent");
        AddQuestionnaires("S-Test?", "Completely incoherent", "Totally coherent");
        AddQuestionnaires("N-Break Time", "Validate when you are ready", "Take your time");
        currentQuestions = -1;
        NextQuestion();
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
    }
    public void StartQuestionnaire()
    {
        currentQuestions = -1;
        NextQuestion();
        finished = false;
    }
    void Update()
    {
        foreach (Button b in buttons)
        {
            if (b.selected && selected != b.value)
            {
                selected = b.value;
                foreach (Button b2 in buttons)
                {
                    if (b2.value != b.value)
                    {
                        b2.unselected();
                    }
                }
            }
        }
        if (confirm.selected)
        {
            Debug.Log(selected);
            OnValidate();
        }
        foreach (var device in inputDevices)
        {
            bool triggerValue;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue && !preventRepetition)
            {
                preventRepetition = true;
                Debug.Log(selected);
                OnValidate();
            }
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && !triggerValue )
            {
                preventRepetition = false;
            }
            // Check if the device has a trackpad
            bool hasTrackpad = device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 trackpadValue);

            if (hasTrackpad)
            {
                // Check if the trackpad button is pressed
                bool isPressed;
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out isPressed) && isPressed)
                {
                    // Determine if pressed on the left or right side
                    if (trackpadValue.x < 0)
                    {
                        Debug.Log("Trackpad pressed on the left side : " + trackpadValue.x.ToString());
                    }
                    else
                    {
                        Debug.Log("Trackpad pressed on the right side : " + trackpadValue.x.ToString());
                    }
                    changeAnswer(trackpadValue.x);
                }
            }
        }
    }
    public void changeAnswer(float value)
    {
        switch (answerType)
        {
            case "S":
                slider.changePos(value);
                break;
        }
    }
    public void OnValidate()
    {
        if(currentQuestions != -1)
        {

            if (selected != -1)
            {
                writeAnswer(questionnaires[currentQuestions][0], selected);
                foreach (Button b2 in buttons)
                {
                    b2.unselected();
                }
                confirm.reset();
                slider.reset();
                selected = -1;
                NextQuestion();
            }
            else if (slider.percentage != -1)
            {
                writeAnswer(questionnaires[currentQuestions][0], slider.percentage);
                confirm.reset();
                slider.reset();
                selected = -1;
                NextQuestion();
            }
            else if (answerType=="N")
            {
                confirm.reset();
                slider.reset();
                selected = -1;
                NextQuestion();
            }
            else
            {
                confirm.reset();
            }
        }
    }
    public void NextQuestion()
    {
        currentQuestions += 1;
        selected = -1;
        if (currentQuestions < questionnaires.Count)
        {
            currQuestion = questionnaires[currentQuestions][0];
            currminValue = questionnaires[currentQuestions][1];
            currmaxValue = questionnaires[currentQuestions][2];
            setQuestion();
        }
        else
        {
            finished = true;
        }
    }
    public void setQuestion()
    {
        questionnaire.text = currQuestion.Remove(0, 2);
        minanswer.text = currminValue;
        maxanswer.text = currmaxValue;
        switch(currQuestion.Substring(0, 1))
        {
            case "S":
                foreach(Button b in buttons)
                {
                    b.gameObject.SetActive(false);
                }
                slider.gameObject.SetActive(true);
                answerType = "S";
                break;
            case "B":
                foreach (Button b in buttons)
                {
                    b.gameObject.SetActive(true);
                }
                slider.gameObject.SetActive(false);
                answerType = "B";
                break;
            case "N":
                foreach (Button b in buttons)
                {
                    b.gameObject.SetActive(false);
                }
                slider.gameObject.SetActive(false);
                answerType = "N";
                break;
            default:
                foreach (Button b in buttons)
                {
                    b.gameObject.SetActive(false);
                }
                slider.gameObject.SetActive(true);
                answerType = "S";
                break;
        }
    }

    // Function to add button data
    private void AddQuestionnaires(string question, string minval, string maxval)
    {
        // Create a new list to hold button data
        List<string> questions = new List<string>();
        questions.Add(question);
        questions.Add(minval);
        questions.Add(maxval);

        // Add the button data to the list
        questionnaires.Add(questions);
    }

    private void writeAnswer(string questname, int value)
    {

        using (StreamWriter writer = File.AppendText(info.logFile))
        {
            writer.WriteLine(questname + ":" + value.ToString());
        }
    }
    private void writeAnswer(string questname, float value)
    {

        using (StreamWriter writer = File.AppendText(info.logFile))
        {
            writer.WriteLine(questname + ":" + value.ToString());
        }
    }
    private void writeAnswer(string questname, double value)
    {

        using (StreamWriter writer = File.AppendText(info.logFile))
        {
            writer.WriteLine(questname + ":" + value.ToString());
        }
    }
}
