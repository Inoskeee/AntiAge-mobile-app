using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    public List<ToggleGroup> questions;
    public TestCheckerManager checker;

    public Text errorProvider;
    public List<int> answers;
    void OnEnable()
    {

    }


    public void NextButton()
    {
        errorProvider.gameObject.SetActive(false);
        foreach (var item in questions)
        {
            if (item.GetFirstActiveToggle() == null) 
            { 
                errorProvider.gameObject.SetActive(true);
                errorProvider.text = "Ответьте на все вопросы";
                return;
            }
            else
            {
                answers.Add(int.Parse(item.GetFirstActiveToggle().name));
            }
        }
        Debug.Log("Ok");
        checker.ClickNext();
    }
}
