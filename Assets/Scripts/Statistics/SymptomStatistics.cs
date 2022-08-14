using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SymptomStatistics : MonoBehaviour
{
    public DatabaseReference database;


    public Dictionary<string, int> VAZ_Count = new Dictionary<string, int>();
    public Dictionary<string, int> PSY_Count = new Dictionary<string, int>();
    public Dictionary<string, int> URO_Count = new Dictionary<string, int>();
    public Dictionary<string, int> SKELET_Count = new Dictionary<string, int>();
    public Dictionary<string, int> OTHER_Count = new Dictionary<string, int>();
    public Dictionary<string, int> USERS_Count = new Dictionary<string, int>();
    private List<string> SymptomDate;

    public DateTimeChecker dateTimeChecker;

    public SymptomsGroup symptomsGroup;

    [Header("VAZ Graph")]
    public WMG_Axis_Graph vazGraph;
    public WMG_Series vazSeries;
    [Header("PSY Graph")]
    public WMG_Axis_Graph psyGraph;
    public WMG_Series psySeries;
    [Header("URO Graph")]
    public WMG_Axis_Graph uroGraph;
    public WMG_Series uroSeries;
    [Header("SKELET Graph")]
    public WMG_Axis_Graph skeletGraph;
    public WMG_Series skeletSeries;
    [Header("OTHER Graph")]
    public WMG_Axis_Graph otherGraph;
    public WMG_Series otherSeries;
    [Header("USER Graph")]
    public WMG_Axis_Graph userGraph;
    public WMG_Series userSeries;
    public void OnEnable()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        SymptomDate = new List<string>();
        symptomsGroup = GameObject.FindGameObjectWithTag("Player").GetComponent<SymptomsGroup>();
        //AddData();
        UpdateChartsData();
    }

    public void OnDisable()
    {
        VAZ_Count = new Dictionary<string, int>();
        PSY_Count = new Dictionary<string, int>();
        URO_Count = new Dictionary<string, int>();
        SKELET_Count = new Dictionary<string, int>();
        OTHER_Count = new Dictionary<string, int>();
        USERS_Count = new Dictionary<string, int>();
        SymptomDate.Clear();
    }
    public void UpdateChartsData()
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        foreach (var item in symptomsGroup.VAZ)
        {
            VAZ_Count.Add(item, 0);
        }
        foreach (var item in symptomsGroup.PSY)
        {
            PSY_Count.Add(item, 0);
        }
        foreach (var item in symptomsGroup.URO)
        {
            URO_Count.Add(item, 0);
        }
        foreach (var item in symptomsGroup.SKELET)
        {
            SKELET_Count.Add(item, 0);
        }
        foreach (var item in symptomsGroup.OTHER)
        {
            OTHER_Count.Add(item, 0);
        }
        foreach (var item in symptomsGroup.USERS)
        {
            USERS_Count.Add(item, 0);
        }

        StartCoroutine(UpdateChartsData_Coroutine(hashedEmail));
    }

    private IEnumerator UpdateChartsData_Coroutine(string hashedEmail)
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
                            for(int k = 0; k < snapshot.Child(i.ToString()).Child("symptoms").ChildrenCount; k++)
                            {
                                string symptom = snapshot.Child(i.ToString()).Child("symptoms").Child(k.ToString()).Value.ToString();
                                if (symptomsGroup.VAZ.Contains(symptom)) 
                                {
                                    if (VAZ_Count.ContainsKey(symptom))
                                    {
                                        VAZ_Count[symptom]++;
                                    }
                                    else
                                    {
                                        VAZ_Count.Add(symptom, 1);
                                    }
                                }
                                else if (symptomsGroup.PSY.Contains(symptom))
                                {
                                    if (PSY_Count.ContainsKey(symptom))
                                    {
                                        PSY_Count[symptom]++;
                                    }
                                    else
                                    {
                                        PSY_Count.Add(symptom, 1);
                                    }
                                }
                                else if (symptomsGroup.URO.Contains(symptom))
                                {
                                    if (URO_Count.ContainsKey(symptom))
                                    {
                                        URO_Count[symptom]++;
                                    }
                                    else
                                    {
                                        URO_Count.Add(symptom, 1);
                                    }
                                }
                                else if (symptomsGroup.SKELET.Contains(symptom))
                                {
                                    if (SKELET_Count.ContainsKey(symptom))
                                    {
                                        SKELET_Count[symptom]++;
                                    }
                                    else
                                    {
                                        SKELET_Count.Add(symptom, 1);
                                    }
                                }
                                else if (symptomsGroup.OTHER.Contains(symptom))
                                {
                                    if (OTHER_Count.ContainsKey(symptom))
                                    {
                                        OTHER_Count[symptom]++;
                                    }
                                    else
                                    {
                                        OTHER_Count.Add(symptom, 1);
                                    }
                                }
                                else if (symptomsGroup.USERS.Contains(symptom))
                                {
                                    if (USERS_Count.ContainsKey(symptom))
                                    {
                                        USERS_Count[symptom]++;
                                    }
                                    else
                                    {
                                        USERS_Count.Add(symptom, 1);
                                    }
                                }

                            }
                        }
                    }
                    GraphCreate();
                }
            }
        }
    }

    public void GraphCreate()
    {
        SpecificGraphSet(VAZ_Count, vazGraph, vazSeries);
        SpecificGraphSet(PSY_Count, psyGraph, psySeries);
        SpecificGraphSet(URO_Count, uroGraph, uroSeries);
        SpecificGraphSet(SKELET_Count, skeletGraph, skeletSeries);
        SpecificGraphSet(OTHER_Count, otherGraph, otherSeries);
        SpecificGraphSet(USERS_Count, userGraph, userSeries);
    }

    public void SpecificGraphSet(Dictionary<string, int> dict, WMG_Axis_Graph graph, WMG_Series series)
    {
        graph.groups.Clear();
        series.pointValues.Clear();
        if(dict.Count > 0)
        {
            double maxBarValue = dict.Values.Max();
            int count = 1;
            foreach (var item in dict)
            {
                graph.groups.Add(item.Key);
                series.pointValues.Add(new Vector2(count, item.Value));
                count++;
            }
            graph.yAxis.AxisMaxValue = (float)maxBarValue;
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
