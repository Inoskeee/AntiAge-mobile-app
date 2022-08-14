using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MentalStatisticsManager : MonoBehaviour
{
    public DatabaseReference database;


    private List<int> MentalCount;
    private List<string> MoodCount;
    private List<string> MentalDate;

    public List<int> myMentalData;
    public List<string> myDatesData;
    public int goodMood;
    public int normalMood;
    public int badMood;


    public DateTimeChecker dateTimeChecker;

    [Header("LineGraph")]
    public WMG_Axis_Graph lineGraph;
    public WMG_Series lineSeries;


    [Header("PieChart")]
    public WMG_Pie_Graph pieChart;
    public void OnEnable()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        MentalCount = new List<int>();
        MentalDate = new List<string>();
        //AddData();
        UpdateChartsData();
    }

    public void OnDisable()
    {
        MentalCount.Clear();
        MentalDate.Clear();

        myDatesData.Clear();
        myMentalData.Clear();
        goodMood = 0;
        normalMood = 0;
        badMood = 0;
    }
    public void UpdateChartsData()
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        StartCoroutine(UpdateChartsData_Coroutine(hashedEmail));
    }

    private IEnumerator UpdateChartsData_Coroutine(string hashedEmail)
    {
        var DBTask = database.Child("MentalHealth").Child(hashedEmail).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on check");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot.Exists)
            {
                if (snapshot.ChildrenCount == 0)
                {

                }
                else
                {
                    DateTime startDate = DateTime.Parse(dateTimeChecker.startDate);
                    DateTime endDate = DateTime.Parse(dateTimeChecker.endDate);
                    for (int i = 1; i <= snapshot.ChildrenCount; i++)
                    {
                        DateTime currentDate = DateTime.Parse(snapshot.Child(i.ToString()).Child("Date").Value.ToString());
                        if (currentDate >= startDate && currentDate <= endDate)
                        {
                            MentalCount.Add(int.Parse(snapshot.Child(i.ToString()).Child("TrainingActivities").ChildrenCount.ToString()));
                            MentalDate.Add(snapshot.Child(i.ToString()).Child("Date").Value.ToString());
                        }
                    }
                    StartCoroutine(UpdateMoodData_Coroutine(hashedEmail));
                }
            }
        }
    }
    private IEnumerator UpdateMoodData_Coroutine(string hashedEmail)
    {
        var DBTask = database.Child("MyDay").Child(hashedEmail).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on check");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot.Exists)
            {
                if (snapshot.ChildrenCount == 0)
                {

                }
                else
                {
                    DateTime startDate = DateTime.Parse(dateTimeChecker.startDate);
                    DateTime endDate = DateTime.Parse(dateTimeChecker.endDate);
                    for (int i = 1; i <= snapshot.ChildrenCount; i++)
                    {
                        DateTime currentDate = DateTime.Parse(snapshot.Child(i.ToString()).Child("date").Value.ToString());
                        if (currentDate >= startDate && currentDate <= endDate)
                        {
                            if(snapshot.Child(i.ToString()).Child("mood").Value.ToString() == "good")
                            {
                                goodMood += 1;
                            }
                            if (snapshot.Child(i.ToString()).Child("mood").Value.ToString() == "normal")
                            {
                                normalMood += 1;
                            }
                            if (snapshot.Child(i.ToString()).Child("mood").Value.ToString() == "bad")
                            {
                                badMood += 1;
                            }
                        }
                    }
                    GroupData();
                    GraphCreate();
                }
            }
        }
    }


    public void GraphCreate()
    {
        lineGraph.groups.Clear();
        lineSeries.pointValues.Clear();
        pieChart.sliceValues.Clear();
        pieChart.sliceLabels.Clear();
        int maxValue = 0;

        for (int i = 0; i < myDatesData.Count; i++)
        {
            maxValue += myMentalData[i];
            lineGraph.groups.Add(myDatesData[i]);
            lineSeries.pointValues.Add(new Vector2(0, myMentalData[i]));

        }
        lineGraph.yAxis.AxisMaxValue = maxValue;

        pieChart.sliceLabels.Add("Хорошее");
        pieChart.sliceLabels.Add("Нормальное");
        pieChart.sliceLabels.Add("Плохое");

        pieChart.sliceValues.Add(goodMood);
        pieChart.sliceValues.Add(normalMood);
        pieChart.sliceValues.Add(badMood);
    }
    public void GroupData()
    {
        if (MentalDate.Count <= 7)
        {
            myMentalData = MentalCount;
            myDatesData = MentalDate;
        }
        else if (MentalDate.Count <= 14)
        {
            int count = 0;
            do
            {
                if (count + 2 >= MentalDate.Count)
                {
                    int tempFitnessCount = 0;
                    for (int i = count; i <= MentalCount.Count - 1; i++)
                    {
                        tempFitnessCount += MentalCount[i];
                    }
                    tempFitnessCount /= 2;
                    myMentalData.Add(tempFitnessCount);
                    myDatesData.Add(MentalDate[MentalDate.Count - 1]);
                    count = count + 2;
                }
                else
                {
                    myMentalData.Add(MentalCount[count] + MentalCount[count + 1]);
                    myDatesData.Add(MentalDate[count]);
                    count = count + 2;
                }
            }
            while (count < MentalDate.Count);
        }
        else if (MentalDate.Count <= 31)
        {
            int count = 0;
            do
            {
                if (count + 6 >= MentalDate.Count)
                {
                    Debug.Log(count);
                    Debug.Log(MentalDate.Count);
                    int del = MentalCount.Count - count - 1;
                    int tempFitnessCount = 0;
                    for (int i = count; i <= MentalCount.Count - 1; i++)
                    {
                        tempFitnessCount += MentalCount[i];
                    }
                    tempFitnessCount /= (del + 1);
                    myMentalData.Add(tempFitnessCount);
                    myDatesData.Add(MentalDate[MentalDate.Count - 1]);
                    count = count + 6;
                }
                else
                {
                    myMentalData.Add(MentalCount[count] + MentalCount[count + 1] + MentalCount[count + 2]
                        + MentalCount[count + 3] + MentalCount[count + 4] + MentalCount[count + 5] +
                        +MentalCount[count + 6]);
                    myDatesData.Add(MentalDate[count] + " - " + MentalDate[count + 6]);
                    count = count + 7;
                }
            }
            while (count < MentalCount.Count);
        }
    }

    public void AddData()
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        string path = Application.streamingAssetsPath + "/" + "FitnessTest.json";
        string json = File.ReadAllText(path);
        Debug.Log(json);
        StartCoroutine(SetFitnessInfo_Coroutine(hashedEmail, json));
    }


    private IEnumerator SetFitnessInfo_Coroutine(string hashedEmail, string json)
    {
        var DBTask = database.Child("MyDay").Child(hashedEmail).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {

        }
    }
}
