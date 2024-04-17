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
    public TextMeshProUGUI questionnaire, minanswer, maxanswer;
    public int selected;
    public int currentQuestions;
    public List<List<string>> questionnaires = new List<List<string>>();
    private string currQuestion;
    private string currminValue;
    private string currmaxValue;

    // Start is called before the first frame update
    void Start()
    {


        // Add some sample data
        AddQuestionnaires("Do you like it ?", "No", "Yes");
        AddQuestionnaires("Button2", "Action2", "Parameter2");
        AddQuestionnaires("Button3", "Action3", "Parameter3");
        currentQuestions = -1;
        NextQuestion();
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
            confirm.unselected();
            selected = -1;
            NextQuestion();
        }
        else
        {
            confirm.unselected();
        }
    }
    public void NextQuestion()
    {
        currentQuestions += 1;
        if (currentQuestions < questionnaires.Count)
        {
            currQuestion = questionnaires[currentQuestions][0];
            currminValue = questionnaires[currentQuestions][1];
            currmaxValue = questionnaires[currentQuestions][2];
            setQuestion();
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
            writer.WriteLine(questname+":"+value.ToString());
        }
    }
}
