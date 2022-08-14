using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MentalHealthManager : MonoBehaviour
{
    public DatabaseReference database;

    public GameObject mentalHealthPanel;
    public GameObject mindTraining;

    public MentalHealthHelper mentalHealthHelper;

    public GameObject psychicalTests;

    [Header("ActivityChecker")]
    public List<MindTrainingModel> mindModels;

    public void OpenComplex(GameObject gameObject)
    {
        mindTraining.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    public void CloseComplex(GameObject gameObject)
    {
        gameObject.SetActive(false);
        mindTraining.SetActive(true);
        SetMentalInfo();
    }
    public void OpenMindTraining()
    {
        mentalHealthPanel.SetActive(false);
        mindTraining.SetActive(true);
    }
    public void CloseMindTraining()
    {
        mentalHealthPanel.SetActive(true);
        mindTraining.SetActive(false);
    }

    private void OnEnable()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        mentalHealthHelper.Date = DateTime.Now.Date.ToString("dd-MM-yyyy");
        UpdateMentalInfo();
    }

    public void OpenPsychicalTests()
    {
        mentalHealthPanel.SetActive(false);
        psychicalTests.SetActive(true);
    }
    public void ClosePsychicalTests()
    {
        mentalHealthPanel.SetActive(true);
        psychicalTests.SetActive(false);
    }
    public void OpenTest(GameObject test)
    {
        test.SetActive(true);
        psychicalTests.SetActive(false);
    }
    public void UpdateMentalInfo()
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);
        
        StartCoroutine(GetMentalHealth_Coroutine(hashedEmail));
    }
    private IEnumerator GetMentalHealth_Coroutine(string hashedEmail)
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
                    if (DateTime.Now.Date.ToString("dd-MM-yyyy") == snapshot.Child(snapshot.ChildrenCount.ToString()).Child("Date").Value.ToString())
                    {
                        for (int i = 0; i < snapshot.Child(snapshot.ChildrenCount.ToString()).Child("TrainingActivities").ChildrenCount; i++)
                        {
                            if (!mentalHealthHelper.TrainingActivities.Contains(snapshot.Child(snapshot.ChildrenCount.ToString()).Child("TrainingActivities").Child(i.ToString()).Value.ToString()))
                            {
                                mentalHealthHelper.TrainingActivities.Add(snapshot.Child(snapshot.ChildrenCount.ToString()).Child("TrainingActivities").Child(i.ToString()).Value.ToString());
                                foreach (var item in mindModels)
                                {
                                    if (item.practiceId == snapshot.Child(snapshot.ChildrenCount.ToString()).Child("TrainingActivities").Child(i.ToString()).Value.ToString())
                                    {
                                        item.checkbox.isOn = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
            }
        }
    }

    public void SetMentalInfo()
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        mentalHealthHelper.TrainingActivities.Clear();
        foreach (var item in mindModels)
        {
            if (item.checkbox.isOn == true)
            {
                mentalHealthHelper.TrainingActivities.Add(item.practiceId);
            }
        }

        MentalHealthModel mentalHealth = new MentalHealthModel
        {
            Date = mentalHealthHelper.Date,
            TrainingActivities = mentalHealthHelper.TrainingActivities
        };
        string json = JsonUtility.ToJson(mentalHealth);
        Debug.Log(json);
        StartCoroutine(GetCountParams_Coroutine(hashedEmail, json));
    }
    private IEnumerator GetCountParams_Coroutine(string hashedEmail, string json)
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
                int counter = 0;
                for (int i = 1; i <= snapshot.ChildrenCount; i++)
                {
                    if (snapshot.Child(i.ToString()).Child("Date").Value.ToString() == mentalHealthHelper.Date)
                    {
                        counter = i;
                    }
                }
                if (counter > 0)
                {
                    StartCoroutine(SetMentalInfo_Coroutine(hashedEmail, json, counter.ToString()));
                }
                else
                {
                    string count = (snapshot.ChildrenCount + 1).ToString();
                    StartCoroutine(SetMentalInfo_Coroutine(hashedEmail, json, count));
                }
            }
            else
            {
                StartCoroutine(SetMentalInfo_Coroutine(hashedEmail, json, "1"));
            }
        }
    }
    private IEnumerator SetMentalInfo_Coroutine(string hashedEmail, string json, string count)
    {
        var DBTask = database.Child("MentalHealth").Child(hashedEmail).Child(count).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {
            UpdateMentalInfo();
        }
    }

}
