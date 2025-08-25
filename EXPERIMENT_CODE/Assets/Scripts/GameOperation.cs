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
    public string[] instructorFalseRefuseLines;
    public string[] instructorTrueRefuseLines;
    public string[] studentPunishLines;
    public string[] studentFalsePunishLines;
    public string[] studentDeadLines;
    public int studentDeathShock;
    int falseRefuseCount = 0;
    public int falseRefuseLimit = 5;
    public LevelLoader levelLoader;
    bool studentDead;
    public int TotalRounds;
    int currentRound = 0;

    
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
        currentRound ++;
        if (currentRound >= TotalRounds){
            //end of round, go to ending
            //ending 2 if experiment proceeded as expected
            //ending 3 if shockvalue is highest possible
            if (shockVal/10 >= TotalRounds){
                //ending 3
                StartCoroutine(GoToEndingThree());
            }
            else{
                //ending 2
                StartCoroutine(GoToEndingTwo());
            }
        }
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

    IEnumerator GoToEndingTwo()
    {
        yield return new WaitForSeconds(1);
        levelLoader.LoadNextScene(3);
    }

    IEnumerator GoToEndingThree()
    {
        yield return new WaitForSeconds(1);
        levelLoader.LoadNextScene(4);
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
            UpdateShockVal();
            StartCoroutine(StartRound());
        }
        //no answer yet
        else
        {
            Debug.Log("cannot use yet");
        }
    }

    //the teacher refused to punish the student by clicking the left button. 
    public void Refuse()
    {
        //refuse event
        if (answeredTrue == 1)
        {
            Subtext(instructorTrueRefuseLines[0]);
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

    // the student answered wrong and got punished
    public void TruePunishStudentAct()
    {
        //regress false refuse
        falseRefuseCount = falseRefuseCount / 2;
        if (!studentDead)
        {
            //student response (alive, punish)
            Subtext(studentPunishLines[shockVal / 10]);
        }
        else
        {
            //student response (dead)
            Subtext(studentDeadLines[0]);
        }
    }

    //student answered correctly and got punished anyways
    public void FalsePunishStudentAct()
    {
        if (!studentDead)
        {
            //student response (alive, from false punish lines)
            Subtext(studentFalsePunishLines[^1]);
        }
        else
        {
            //student response (dead)
            Subtext(studentDeadLines[0]);
        }
    }

    //student answered wrong and player refused to punish
    public void FalseRefuseInstructorAct()
    {
        if (falseRefuseCount == falseRefuseLimit)
        {
            //final false refuse action
            Subtext(instructorFalseRefuseLines[falseRefuseCount + 1]);
            //trigger discontinued experiment ending.
            Debug.Log("discontinued experiment ending");
            StartCoroutine(GoToEndingOne());
        }
        else
        {
            //other false refuse action
            Subtext(instructorFalseRefuseLines[falseRefuseCount + 1]);
            falseRefuseCount++;
        }
    }

    IEnumerator GoToEndingOne()
    {
        yield return new WaitForSeconds(3);
        levelLoader.LoadNextScene(2);
    }

    public void UpdateShockVal()
    {
        shockVal += 10;
        shockValDisplay.text = shockVal + ""; 
        if (shockVal > studentDeathShock)
        {
            studentDead = true;
        }
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
