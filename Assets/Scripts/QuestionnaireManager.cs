using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

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
    public bool finished;
    // Start is called before the first frame update
    void Start()
    {


        // Add some sample data
        AddQuestionnaires("How coherent were the physical and visual stimulation?", "Completely incoherent", "Totally coherent");
        currentQuestions = -1;
        NextQuestion();
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
    }

    public void OnValidate()
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
        else if(slider.percentage != -1)
        {
            writeAnswer(questionnaires[currentQuestions][0], slider.percentage);
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
        questionnaire.text = currQuestion;
        minanswer.text = currminValue;
        maxanswer.text = currmaxValue;
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
