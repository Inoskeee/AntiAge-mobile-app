using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DateTimeChecker : MonoBehaviour
{
    public Text TimeFrom;
    public Text TimeTo;

    public GameObject errorProvider;
    public Text errorText;

    public string startDate;
    public string endDate;
    void Update()
    {
        Regex regex = new Regex(@"^(0[1-9]|[12][0-9]|3[01])[- ..](0[1-9]|1[012])[- ..](19|20)\d\d$");

        if (regex.IsMatch(TimeFrom.text) && regex.IsMatch(TimeTo.text))
        {
            DateTime date1 = DateTime.Parse(TimeFrom.text);
            DateTime date2 = DateTime.Parse(TimeTo.text);
            int days = (date2 - date1).Days;
            if(days < 7 || days > 366)
            {
                errorProvider.gameObject.SetActive(true);
                errorText.text = "Дата начала и конца не должна быть меньше недели и больше года";
            }
            else
            {
                errorProvider.gameObject.SetActive(false);
                startDate = TimeFrom.text;
                endDate = TimeTo.text;
            }
        }
        else if(TimeFrom.text == "" && TimeTo.text == "")
        {
            errorProvider.gameObject.SetActive(true);
            errorText.text = "Ведите даты в формате дд.мм.гггг";
        }
        else if (!regex.IsMatch(TimeFrom.text))
        {
            errorProvider.gameObject.SetActive(true);
            errorText.text = "Ведите начальную дату в формате дд.мм.гггг";
        }
        else if (!regex.IsMatch(TimeTo.text))
        {
            errorProvider.gameObject.SetActive(true);
            errorText.text = "Ведите конечную дату в формате дд.мм.гггг";
        }

        //DateTime dt1 = DateTime.Parse("25.09.2001");
        //DateTime btw = DateTime.Parse("30.09.2001");
        //DateTime dt2 = DateTime.Parse("29.09.2001");
        //if(btw >= dt1 && btw <= dt2)
        //{
        //    Debug.Log("true");
        //}
    }
}
