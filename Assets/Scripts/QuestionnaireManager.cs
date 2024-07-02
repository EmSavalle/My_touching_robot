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
    public bool informations;
    List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();

    public float delayAnswer = 0.1f;
    private float lastResponse;

    // Start is called before the first frame update
    void Start()
    {

        currentQuestions = -1;
        NextQuestion();
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
    }
    public void SetupQuestionnaire(bool emb, bool nasa)
    {
        questionnaires = new List<List<string>>();
        // Add some sample data
        if (emb)
        {
            AddQuestionnaires("B-I felt out of my body?", "Never", "Always");
            AddQuestionnaires("B-I felt as if my (real) body were drifting toward the virtual body or as if the virtual body were drifting toward my (real) body", "Never", "Always");
            AddQuestionnaires("B-I felt as if the movements of the virtual body were influencing my own movements", "Never", "Always");
            AddQuestionnaires("B-It felt as if my (real) body were turning into an ‘avatar’ body?", "Never", "Always");
            AddQuestionnaires("B-At some point it felt as if my real body was starting to take on the posture or shape of the virtual body that I saw", "Never", "Always");
            AddQuestionnaires("B-I felt like I was wearing different clothes from when I came to the laboratory?", "Never", "Always");
            AddQuestionnaires("B-I felt as if my body had changed", "Never", "Always");
            AddQuestionnaires("B-I felt a tactile sensation on my hand when I saw the robot touching the virtual hand", "Never", "Always");
            AddQuestionnaires("B-I felt that my own body could be affected by the robot", "Never", "Always");
            AddQuestionnaires("B-I felt as if the virtual body was my body", "Never", "Always");
            AddQuestionnaires("B-At some point it felt that the virtual body resembled my own (real) body, in terms of shape, skin tone or other visual features.", "Never", "Always");
            AddQuestionnaires("B-I felt as if my body was located where I saw the virtual body", "Never", "Always");
            AddQuestionnaires("B-I felt like I could control the virtual body as if it was my own body", "Never", "Always");
            AddQuestionnaires("B-It seemed as if I felt the touch of the robot in the location where I saw the virtual hand touched", "Never", "Always");
            AddQuestionnaires("B-It seemed as if the touch I felt was caused by the robot touching the virtual hand", "Never", "Always");
            AddQuestionnaires("B-It seemed as if my hand was touching the virtual table", "Never", "Always");
        }

        if (nasa)
        {
            AddQuestionnaires("B-How mentally demanding was the task?", "Very Low", "Very High");
            AddQuestionnaires("B-How physically demanding was the task?", "Very Low", "Very High");
            AddQuestionnaires("B-How hurried or rushed was the pace of the task?", "Very Low", "Very High");
            AddQuestionnaires("B-How successful were you in accomplishing what you were asked to do?", "Very Low", "Very High");
            AddQuestionnaires("B-How hard did you have to work to accomplish your level of performance? ", "Very Low", "Very High");
            AddQuestionnaires("B-How insecure, discouraged, irritated, stressed, and annoyed were you? ", "Very Low", "Very High");
        }
        AddQuestionnaires("N-Break Time", "Please place your hand on the indicated position.\nValidate when you are ready to continue", "Take your time");
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
            case "B":
                if (lastResponse + delayAnswer < Time.time)
                {
                    int currButton = -1;
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        Button b = buttons[i];
                        if (b.selected)
                        {
                            currButton = i;
                        }
                    }
                    if (currButton == -1)
                    {
                        buttons[buttons.Length / 2].setSelected();
                        selected = buttons[buttons.Length / 2].value;
                        currButton = buttons.Length / 2;
                    }
                    if (value < 0)
                    {
                        if (currButton > 0)
                        {
                            buttons[currButton].unselected();
                            buttons[currButton-1].setSelected();
                            selected = buttons[currButton - 1].value;
                        }
                    }
                    else if (value > 0)
                    {
                        if (currButton < buttons.Length-1)
                        {
                            buttons[currButton].unselected();
                            buttons[currButton + 1].setSelected();
                            selected = buttons[currButton + 1].value;
                        }
                    }
                    lastResponse = Time.time;
                }
                break;
            }
    }
    public void OnValidate()
    {
        if(currentQuestions != -1)
        {

            if (selected != -1)
            {
                if (!informations)
                {
                    writeAnswer(questionnaires[currentQuestions][0], selected);
                }
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
                if (!informations)
                {
                    writeAnswer(questionnaires[currentQuestions][0], slider.percentage);
                }
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
    public void AddQuestionnaires(string question, string minval, string maxval)
    {
        // Create a new list to hold button data
        List<string> questions = new List<string>();
        questions.Add(question);
        questions.Add(minval);
        questions.Add(maxval);

        // Add the button data to the list
        questionnaires.Add(questions);
    }

    public void setInformationPannel(bool nb, int nbn)
    {
        EmptyQuestionnaires();
        string text = "";
        if (nb)
        {
            text = "N-Please look at your hand during the trial.\n";
            text += "a " + nbn.ToString() + "-back task will begin.\n";
            text += "Please press the trigger if the number displayed is the same as the number seen " + nbn.ToString() + " stimulations ago\n";
            if(nbn == 2)
            {
                text += "Example : 5 8 6 2 |6| 1 3 8 |3| 2 6 8\n";
            }
            else if (nbn == 1)
            {
                text += "Example : 5 6 |6| 1 8 |8| 2 6 8\n";
            }
            else if (nbn == 3)
            {
                text += "Example : 5 6 8 2 |6| 3 8 2 |3| 2 6 8 |2|\n";
            }
        }
        else
        {
            text = "N-Please look at your left hand during the trial.\n There are no other task during this trial";
        }
        List<string> questions = new List<string>();
        questions.Add(text);
        questions.Add("");
        questions.Add("");

        questionnaires = new List<List<string>>();
        questionnaires.Add(questions);

    }
    // Function to add button data
    public void EmptyQuestionnaires()
    {
        // Add the button data to the list
        questionnaires = new List<List<string>>();
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
