using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
public class AppRunStatistics : MonoBehaviour
{
    public DatabaseReference database;


    private List<int> AppRunCount;
    private List<string> AppRunDate;

    public List<int> myRuns;
    public List<string> myDatesData;

    public DateTimeChecker dateTimeChecker;


    [Header("BarGraph")]
    public WMG_Axis_Graph barGraph;
    public WMG_Series barSeries;
    public void OnEnable()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        AppRunCount = new List<int>();
        AppRunDate = new List<string>();
        //AddData();
        UpdateChartsData();
    }

    public void OnDisable()
    {
        AppRunCount.Clear();
        AppRunDate.Clear();

        myDatesData.Clear();

        myRuns.Clear();
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
        var DBTask = database.Child("AppRun").Child(hashedEmail).GetValueAsync();
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
                            AppRunCount.Add(int.Parse(snapshot.Child(i.ToString()).Child("Count").Value.ToString()));
                            AppRunDate.Add(snapshot.Child(i.ToString()).Child("Date").Value.ToString());
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
        barGraph.groups.Clear();
        barSeries.pointValues.Clear();
        double maxBarValue = myRuns.Max();

        for (int i = 0; i < myDatesData.Count; i++)
        {
            barGraph.groups.Add(myDatesData[i]);
            barSeries.pointValues.Add(new Vector2(i + 1, (int)myRuns[i]));

        }
        barGraph.yAxis.AxisMaxValue = (float)maxBarValue;

    }
    public void GroupData()
    {
        Debug.Log(AppRunCount.Count);
        if (AppRunDate.Count <= 7)
        {
            Debug.Log("first");
            myRuns = AppRunCount;
            myDatesData = AppRunDate;
        }
        else if (AppRunDate.Count <= 14)
        {
            Debug.Log("second");
            int count = 0;
            do
            {
                if (count + 2 >= AppRunDate.Count)
                {
                    int tempFitnessCount = 0;
                    double tempImtCount = 0;
                    for (int i = count; i <= AppRunCount.Count - 1; i++)
                    {
                        tempFitnessCount += AppRunCount[i];
                    }
                    Debug.Log(tempFitnessCount);
                    Debug.Log(tempImtCount);
                    tempFitnessCount /= 2;
                    tempImtCount /= 2;
                    myRuns.Add(tempFitnessCount);
                    myDatesData.Add(AppRunDate[AppRunDate.Count - 1]);
                    count = count + 2;
                }
                else
                {
                    myRuns.Add(AppRunCount[count] + AppRunCount[count + 1]);
                    myDatesData.Add(AppRunDate[count]);
                    count = count + 2;
                }
            }
            while (count < AppRunDate.Count);
        }
        else if (AppRunDate.Count <= 31)
        {
            int count = 0;
            do
            {
                if (count + 6 >= AppRunDate.Count)
                {
                    int del = AppRunCount.Count - count - 1;
                    int tempFitnessCount = 0;
                    double tempImtCount = 0;
                    for (int i = count; i <= AppRunCount.Count - 1; i++)
                    {
                        tempFitnessCount += AppRunCount[i];
                    }
                    tempFitnessCount /= (del + 1);
                    tempImtCount /= (del + 1);
                    myRuns.Add(tempFitnessCount);
                    myDatesData.Add(AppRunDate[AppRunDate.Count - 1]);
                    count = count + 6;
                }
                else
                {
                    myRuns.Add(AppRunCount[count] + AppRunCount[count + 1] + AppRunCount[count + 2]
                        + AppRunCount[count + 3] + AppRunCount[count + 4] + AppRunCount[count + 5] +
                        +AppRunCount[count + 6]);
                    myDatesData.Add(AppRunDate[count] + " - " + AppRunDate[count + 6]);
                    count = count + 7;
                }
            }
            while (count < AppRunCount.Count);
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
        var DBTask = database.Child("AppRun").Child(hashedEmail).SetRawJsonValueAsync(json);
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
