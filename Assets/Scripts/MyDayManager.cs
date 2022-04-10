using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MyDayManager : MonoBehaviour
{
    DatabaseReference database;

    public Button GoodEmoji;
    public Button NormalEmoji;
    public Button BadEmoji;


    public GameObject symptomContent;
    public GameObject caseContent;

    public List<GameObject> Symptoms;
    public List<GameObject> Cases;

    public MyDayHelper myDay;

    [Header("Prefabs")]
    public GameObject symptomCheckbox;
    public GameObject caseCheckbox;

    void Awake()
    {
        UpdateMyDay();
        myDay.date = DateTime.Now.Date.ToString("dd-MM-yyyy");
    }
    void Start()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void GoodEmojiClick() 
    {
        myDay.mood = "good";
    }
    public void NormalEmojiClick()
    {
        myDay.mood = "normal";
    }
    public void BadEmojiClick()
    {
        myDay.mood = "bad";
    }

    public void UpdateMyDay()
    {
        foreach (var item in Symptoms)
        {
            GameObject.Destroy(item);
        }
        foreach (var item in Cases)
        {
            GameObject.Destroy(item);
        }

        for (int i = 0; i < UserHelper.Instance.Symptoms.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(symptomCheckbox);
            obj.GetComponentInChildren<Text>().text = UserHelper.Instance.Symptoms[i];
            obj.transform.parent = symptomContent.transform;
            obj.GetComponent<Toggle>().isOn = false;
            Symptoms.Add(obj);
        }
        for (int i = 0; i < UserHelper.Instance.Cases.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(caseCheckbox);
            obj.GetComponentInChildren<Text>().text = UserHelper.Instance.Cases[i];
            obj.transform.parent = caseContent.transform;
            obj.GetComponent<Toggle>().isOn = false;
            Cases.Add(obj);
        }
    }

    public void SaveClick()
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        foreach (var item in Symptoms)
        {
            if (item.GetComponent<Toggle>().isOn)
            {
                myDay.symptoms.Add(item.GetComponentInChildren<Text>().text);
            }
        }
        foreach (var item in Cases)
        {
            if (item.GetComponent<Toggle>().isOn)
            {
                myDay.cases.Add(item.GetComponentInChildren<Text>().text);
            }
        }
        MyDayModel model = new MyDayModel()
        {
            Id = hashedEmail,
            date = myDay.date,
            mood = myDay.mood,
            symptoms = myDay.symptoms,
            cases = myDay.cases,
        };
        string json = JsonUtility.ToJson(model);
        StartCoroutine(SaveMyDay_Coroutine(json, model));
    }
    private IEnumerator SaveMyDay_Coroutine(string json, MyDayModel model)
    {
        var DBTask = database.Child("MyDay").Child(model.Id).Child(model.date).SetRawJsonValueAsync(json);
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
