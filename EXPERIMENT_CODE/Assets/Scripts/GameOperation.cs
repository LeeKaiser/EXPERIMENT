using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class GameOperation : MonoBehaviour
{
    public int shockVal = 10;
    int answer = 1;
    int response = 1;
    int answeredTrue = 0; // 0 is no answer yet, 1 is correct, 2 is incorrect
    public TextMeshProUGUI subText;
    public TextMeshProUGUI answerDisplay;
    public TextMeshProUGUI responseDisplay;
    public TextMeshProUGUI shockValDisplay;
    public string[] instructorLines;
    public string[] studentLines;
    public int studentDeathShock;
    int falseRefuseCount = 0;
    public int falseRefuseLimit = 5;
    

    
    // Start is called before the first frame update
    void Start()
    {
        Introduction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Introduction()
    {
        StartCoroutine(StartRound());
    }
    IEnumerator StartRound()
    {
        answerDisplay.text = "-";
        responseDisplay.text = "-";
        answeredTrue = 0;
        answer = 0;
        yield return new WaitForSeconds(3f);
        //answer and response generation
        answer = Random.Range(1, 4);
        answerDisplay.text = answer + "";
        //checks if student should be dead or not
        if (shockVal <= studentDeathShock)
        {
            StartCoroutine(ResponseGeneration());
            
        }
        else
        {
            StartCoroutine(NonResponseGeneration());
            
        }

    }

    IEnumerator ResponseGeneration()
    {
        yield return new WaitForSeconds(Random.Range(0f, 3f));
        response = Random.Range(1, 4);
        responseDisplay.text = response + "";
        if (answer == response)
        {
            answeredTrue = 1;
        }
        else
        {
            answeredTrue = 2;
        }
    }

    IEnumerator NonResponseGeneration()
    {
        yield return new WaitForSeconds(3);
        response = 0;
        responseDisplay.text = "x";
        answeredTrue = 2;
    }

    public void Punish()
    {
        //punishment event
        if (answeredTrue == 2)
        {
            TruePunishStudentAct();
            UpdateShockVal();
            StartCoroutine(StartRound());
        }
        //false punishment event
        else if (answeredTrue == 1)
        {
            FalsePunishStudentAct();
            StartCoroutine(StartRound());
        }
        //no answer yet
        else
        {
            Debug.Log("cannot use yet");
        }
    }

    public void Refuse()
    {
        //refuse event
        if (answeredTrue == 1)
        {
            Subtext(instructorLines[0]);
            StartCoroutine(StartRound());
        }
        //False refuse event
        else if (answeredTrue == 2)
        {
            FalseRefuseInstructorAct();
        }
        //no answer yet
        else
        {
            Debug.Log("cannot use yet");
        }
    }

    public void TruePunishStudentAct()
    {
        falseRefuseCount = falseRefuseCount / 2;
        if (shockVal <= studentDeathShock)
        {
            Subtext(studentLines[shockVal / 10]);
        }
        else
        {
            Subtext(studentLines[0]);
        }
    }

    public void FalsePunishStudentAct()
    {
        if (shockVal <= studentDeathShock)
        {
            Subtext(studentLines[^1]);
        }
        else
        {
            Subtext(studentLines[0]);
        }
    }

    public void FalseRefuseInstructorAct()
    {
        if (falseRefuseCount == falseRefuseLimit)
        {
            Subtext(instructorLines[falseRefuseCount + 1]);
            //trigger discontinued experiment ending.
            Debug.Log("discontinued experiment ending");
        }
        else
        {
            Subtext(instructorLines[falseRefuseCount + 1]);
            falseRefuseCount++;
        }
    }

    public void UpdateShockVal()
    {
        shockVal += 10;
        shockValDisplay.text = shockVal + ""; 
    }

    public void Subtext(string subtxt)
    {
        //decide the amount of time the text should display
        answeredTrue = 0;
        int textLength = subtxt.Length;
        float DisplayTime = 1 + textLength / 20f;
        Debug.Log(DisplayTime);
        subText.text = subtxt;
        StartCoroutine(RemoveSubtext(DisplayTime));
        
    }

    IEnumerator RemoveSubtext(float duration)
    {
        yield return new WaitForSeconds(duration);
        subText.text = "";
        if (answer == response)
        {
            answeredTrue = 1;
        } else if(answer == 0)
        {
            answeredTrue = 0;
        }
        else
        {
            answeredTrue = 2;
        }
    }
}
