using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FitnessStatisticsManager : MonoBehaviour
{
    public DatabaseReference database;


    private List<int> FitnessCount;
    private List<double> ImtCount;
    private List<string> FitnessDate;

    public List<int> myFitnessData;
    public List<double> myImtData;
    public List<string> myDatesData;

    public DateTimeChecker dateTimeChecker;

    [Header("LineGraph")]
    public WMG_Axis_Graph lineGraph;
    public WMG_Series lineSeries;


    [Header("BarGraph")]
    public WMG_Axis_Graph barGraph;
    public WMG_Series barSeries;
    public void OnEnable()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        FitnessCount = new List<int>();
        ImtCount = new List<double>();
        FitnessDate = new List<string>();
        //AddData();
        UpdateChartsData();
    }

    public void OnDisable()
    {
        FitnessCount.Clear();
        ImtCount.Clear();
        FitnessDate.Clear();

        myDatesData.Clear();
        myImtData.Clear();
        myFitnessData.Clear();
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
        var DBTask = database.Child("Fitness").Child(hashedEmail).GetValueAsync();
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
                        if(currentDate >= startDate && currentDate <= endDate)
                        {
                            FitnessCount.Add(int.Parse(snapshot.Child(i.ToString()).Child("PhysicalComplex").ChildrenCount.ToString()));
                            ImtCount.Add(double.Parse(snapshot.Child(i.ToString()).Child("Imt").Value.ToString()));
                            FitnessDate.Add(snapshot.Child(i.ToString()).Child("Date").Value.ToString());
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
        barGraph.groups.Clear();
        barSeries.pointValues.Clear();
        int maxValue = 0;
        double maxBarValue = 0;

        for(int i = 0; i < myDatesData.Count; i++)
        {
            maxValue += myFitnessData[i];
            lineGraph.groups.Add(myDatesData[i]);
            lineSeries.pointValues.Add(new Vector2(0, myFitnessData[i]));

            maxBarValue += myImtData[i];
            barGraph.groups.Add(myDatesData[i]);
            barSeries.pointValues.Add(new Vector2(i+1, (int)myImtData[i]));

        }
        lineGraph.yAxis.AxisMaxValue = maxValue;
        barGraph.yAxis.AxisMaxValue = (float)maxBarValue;

    }
    public void GroupData()
    {
        Debug.Log(FitnessCount.Count);
        if (FitnessDate.Count <= 7)
        {
            Debug.Log("first");
            myFitnessData = FitnessCount;
            myImtData = ImtCount;
            myDatesData = FitnessDate;
        }
        else if (FitnessDate.Count <= 14)
        {
            Debug.Log("second");
            int count = 0;
            do
            {
                if (count + 2 >= FitnessDate.Count)
                {
                    int tempFitnessCount = 0;
                    double tempImtCount = 0;
                    for (int i = count; i <= FitnessCount.Count-1; i++)
                    {
                        tempFitnessCount += FitnessCount[i];
                        tempImtCount += ImtCount[i];
                    }
                    Debug.Log(tempFitnessCount);
                    Debug.Log(tempImtCount);
                    tempFitnessCount /= 2;
                    tempImtCount /= 2;
                    myFitnessData.Add(tempFitnessCount);
                    myImtData.Add(tempImtCount);
                    myDatesData.Add(FitnessDate[FitnessDate.Count - 1]);
                    count = count + 2;
                }
                else
                {
                    myFitnessData.Add(FitnessCount[count] + FitnessCount[count + 1]);
                    myImtData.Add((ImtCount[count] + ImtCount[count + 1]) / 2);
                    myDatesData.Add(FitnessDate[count]);
                    count = count + 2;
                }
            }
            while (count < FitnessDate.Count);
        }
        else if (FitnessDate.Count <= 31)
        {
            int count = 0;
            do
            {
                if (count + 6 >= FitnessDate.Count)
                {
                    int del = FitnessCount.Count - count - 1;
                    int tempFitnessCount = 0;
                    double tempImtCount = 0;
                    for (int i = count; i <= FitnessCount.Count-1; i++)
                    {
                        tempFitnessCount += FitnessCount[i];
                        tempImtCount += ImtCount[i];
                    }
                    tempFitnessCount /= (del + 1);
                    tempImtCount /= (del + 1);
                    myFitnessData.Add(tempFitnessCount);
                    myImtData.Add(tempImtCount);
                    myDatesData.Add(FitnessDate[FitnessDate.Count - 1]);
                    count = count + 6;
                }
                else
                {
                    myFitnessData.Add(FitnessCount[count] + FitnessCount[count + 1] + FitnessCount[count + 2]
                        + FitnessCount[count + 3] + FitnessCount[count + 4] + FitnessCount[count + 5] +
                        +FitnessCount[count + 6]);
                    myImtData.Add((ImtCount[count] + ImtCount[count + 1] + ImtCount[count + 2]
                        + ImtCount[count + 3] + ImtCount[count + 4] + ImtCount[count + 5] +
                        +ImtCount[count + 6]) / 7);
                    myDatesData.Add(FitnessDate[count] + " - " + FitnessDate[count + 6]);
                    count = count + 7;
                }
            }
            while (count < FitnessCount.Count);
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
        var DBTask = database.Child("Fitness").Child(hashedEmail).SetRawJsonValueAsync(json);
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
