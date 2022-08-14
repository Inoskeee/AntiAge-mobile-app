using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestCheckerManager : MonoBehaviour
{
    public List<TestManager> questions;

    public Button NextButton;
    public Button FinishButton;
    public Button BackButton;

    public GameObject test;
    public GameObject testsList;

    public Text errorProvider;

    [Header("Result")]
    public GameObject Result;
    public Text resultText;
    void OnEnable()
    {
        questions[0].gameObject.SetActive(true);
        questions[1].gameObject.SetActive(false);
        NextButton.gameObject.SetActive(true);
        FinishButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        foreach (var question in questions)
        {
            question.answers.Clear();
            foreach (var item in question.questions)
            {
                if(item.GetFirstActiveToggle() != null)
                {
                    item.GetFirstActiveToggle().isOn = false;
                }
            }
        }
    }

    private void OnDisable()
    {
        questions[0].gameObject.SetActive(false);
        questions[1].gameObject.SetActive(false);
        Result.gameObject.SetActive(false);
    }

    public void ClickNext()
    {
        questions[0].gameObject.SetActive(false);
        questions[1].gameObject.SetActive(true);
        NextButton.gameObject.SetActive(false);
        FinishButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }

    public void ClickBack()
    {
        questions[0].gameObject.SetActive(true);
        questions[1].gameObject.SetActive(false);
        NextButton.gameObject.SetActive(true);
        FinishButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
    }

    public void ClickOkey()
    {
        test.SetActive(false);
        testsList.SetActive(true);
    }
    public void ClickFinish()
    {
        errorProvider.gameObject.SetActive(false);
        foreach (var item in questions[1].questions)
        {
            if (item.GetFirstActiveToggle() == null)
            {
                errorProvider.gameObject.SetActive(true);
                errorProvider.text = "Ответьте на все вопросы";
                return;
            }
            else
            {
                questions[1].answers.Add(int.Parse(item.GetFirstActiveToggle().name));
            }
        }
        int firstSum = questions[0].answers.Sum();
        int secondSum = questions[1].answers.Sum();

        string res = "";
        if(firstSum <= 7)
        {
            res = "Отсутствие достоверно выраженных симптомов тревоги\n";
        }
        else if(firstSum <= 10)
        {
            res = "Субклинически выраженная тревога\n";
        }
        else if(firstSum >= 11)
        {
            res = "Клинически выраженная тревога\n";
        }

        if (secondSum <= 7)
        {
            res += "Отсутствие достоверно выраженных симптомов депрессии";
        }
        else if (secondSum <= 10)
        {
            res += "Субклинически выраженная депрессия";
        }
        else if (secondSum >= 11)
        {
            res += "Клинически выраженная депрессия";
        }
        resultText.text = res;
        Result.gameObject.SetActive(true);
    }
    public void FinishSecondTest()
    {
        errorProvider.gameObject.SetActive(false);
        foreach (var item in questions[1].questions)
        {
            if (item.GetFirstActiveToggle() == null)
            {
                errorProvider.gameObject.SetActive(true);
                errorProvider.text = "Ответьте на все вопросы";
                return;
            }
            else
            {
                questions[1].answers.Add(int.Parse(item.GetFirstActiveToggle().name));
            }
        }
        int firstSum = (questions[0].answers[2]+ questions[0].answers[3]+ questions[0].answers[5]+
            questions[0].answers[6] + questions[0].answers[8]+ questions[0].answers[11]+ questions[0].answers[12]+
            questions[0].answers[13]+ questions[0].answers[16] + questions[0].answers[17]) - (questions[0].answers[0]+
            questions[0].answers[1]+ questions[0].answers[4]+ questions[0].answers[7] + questions[0].answers[9]+
            questions[0].answers[10] + questions[0].answers[14] + questions[0].answers[15] + questions[0].answers[18]+
            questions[0].answers[19]) + 50;
        int secondSum = (questions[1].answers[1]+ questions[1].answers[2]+ questions[1].answers[3] + questions[1].answers[4] +
            questions[1].answers[7]+ questions[1].answers[8] + questions[1].answers[10]+ questions[1].answers[11]+ questions[1].answers[13]+
            questions[1].answers[14]+ questions[1].answers[16]+ questions[1].answers[17]+ questions[1].answers[19])-(questions[1].answers[0]+
            questions[1].answers[5]+ questions[1].answers[6]+ questions[1].answers[9]+ questions[1].answers[12]+
            questions[1].answers[15]+ questions[1].answers[18])+35;

        string firstRes = "";
        string secRes= "";

        if(firstSum <= 30)
        {
            firstRes = "Низкая реактивная тревожность - " + firstSum.ToString() + " баллов";
        }
        else if(firstSum > 30 && firstSum <= 45)
        {
            firstRes = "Умеренная реактивная тревожность - " + firstSum.ToString() + " баллов";
        }
        else
        {
            firstRes = "Высокая реактивная тревожность - " + firstSum.ToString() + " баллов";
        }

        if (secondSum <= 30)
        {
            secRes = "Низкая личностная тревожность - " + secondSum.ToString() + " баллов";
        }
        else if (secondSum > 30 && secondSum <= 45)
        {
            secRes = "Умеренная личностная тревожность - " + secondSum.ToString() + " баллов";
        }
        else
        {
            secRes = "Высокая личностная тревожность - " + secondSum.ToString() + " баллов";
        }
        resultText.text = firstRes+"\n"+secRes;
        Result.gameObject.SetActive(true);
    }
}
