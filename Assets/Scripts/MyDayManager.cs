using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MyDayManager : MonoBehaviour
{
    DatabaseReference database;

    public Text errorProvider;

    public Button GoodEmoji;
    public Button NormalEmoji;
    public Button BadEmoji;


    public GameObject symptomContent;
    public GameObject VAZContent;
    public GameObject PSYContent;
    public GameObject UROContent;
    public GameObject SKELETContent;
    public GameObject OTHERContent;
    public GameObject USERContent;

    public GameObject caseContent;

    public List<GameObject> Symptoms;
    public List<GameObject> Cases;

    public MyDayHelper myDay;

    [Header("Prefabs")]
    public GameObject symptomCheckbox;
    public GameObject caseCheckbox;

    void Awake()
    {
        SymptomsGroup symptomsGroup = GameObject.FindGameObjectWithTag("Player").GetComponent<SymptomsGroup>();
        symptomsGroup.USERS = UserHelper.Instance.Symptoms;

        myDay.date = DateTime.Now.Date.ToString("dd-MM-yyyy");
    }

    void Start()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        Color col;
        ColorUtility.TryParseHtmlString("#A6A6A6", out col);
        ColorBlock colorBlock = GoodEmoji.colors;
        colorBlock.normalColor = col;
        GoodEmoji.colors = colorBlock;
        BadEmoji.colors = colorBlock;
        NormalEmoji.colors = colorBlock;
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        StartCoroutine(UpdateMoodData_Coroutine(hashedEmail));
    }
    public void GoodEmojiClick() 
    {
        ChangeColor(GoodEmoji);
        myDay.mood = "good";
    }
    public void NormalEmojiClick()
    {
        ChangeColor(NormalEmoji);
        myDay.mood = "normal";
    }
    public void BadEmojiClick()
    {
        ChangeColor(BadEmoji);
        myDay.mood = "bad";
    }
    private void OnEnable()
    {
        errorProvider.gameObject.SetActive(false);
    }
    public void ChangeColor(Button highlited)
    {
        Color col;
        ColorUtility.TryParseHtmlString("#A6A6A6", out col);
        ColorBlock colorBlock = GoodEmoji.colors;
        colorBlock.normalColor = col;
        GoodEmoji.colors = colorBlock;
        NormalEmoji.colors = colorBlock;
        BadEmoji.colors = colorBlock;
        colorBlock.normalColor = Color.white;
        highlited.colors = colorBlock;
    }
    public void UpdateMyDay()
    {
        if(myDay.mood == "good")
        {
            GoodEmojiClick();
        }
        else if(myDay.mood == "normal")
        {
            NormalEmojiClick();
        }
        else if(myDay.mood == "bad")
        {
            BadEmojiClick();
        }

        foreach (var item in Symptoms)
        {
            GameObject.Destroy(item);
        }
        foreach (var item in Cases)
        {
            GameObject.Destroy(item);
        }
        Symptoms.Clear();
        Cases.Clear();
        SymptomsGroup symptomsGroup = GameObject.FindGameObjectWithTag("Player").GetComponent<SymptomsGroup>();


        Set_Sypmtoms(symptomsGroup.VAZ, VAZContent);
        Set_Sypmtoms(symptomsGroup.PSY, PSYContent);
        Set_Sypmtoms(symptomsGroup.URO, UROContent);
        Set_Sypmtoms(symptomsGroup.SKELET, SKELETContent);
        Set_Sypmtoms(symptomsGroup.OTHER, OTHERContent);
        Set_Sypmtoms(symptomsGroup.USERS, USERContent);


        for (int i = 0; i < UserHelper.Instance.Cases.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(caseCheckbox);
            obj.GetComponentInChildren<Text>().text = UserHelper.Instance.Cases[i];
            obj.transform.parent = caseContent.transform;
            if (myDay.cases.Contains(obj.GetComponentInChildren<Text>().text))
            {
                obj.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                obj.GetComponent<Toggle>().isOn = false;
            }
            Cases.Add(obj);
        }

        float height = symptomContent.GetComponent<VerticalLayoutGroup>().preferredHeight;
        Debug.Log(height);
        symptomContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        symptomContent.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, 0);

        caseContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (70 * UserHelper.Instance.Cases.Count));
        caseContent.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, 0);
        StartCoroutine(CalculateNewSize());
    }

    public void Set_Sypmtoms(List<string> group, GameObject gameObject)
    {
        foreach (var item in group)
        {
            GameObject obj = GameObject.Instantiate(symptomCheckbox);
            obj.GetComponentInChildren<Text>().text = item;
            obj.transform.parent = gameObject.transform;
            if (myDay.symptoms.Contains(obj.GetComponentInChildren<Text>().text))
            {
                obj.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                obj.GetComponent<Toggle>().isOn = false;
            }
            Symptoms.Add(obj);
        }
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (70 * group.Count));
        gameObject.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, 0);
    }

    public void SelectSymptom(GameObject symptom)
    {
        if(symptom.activeSelf == false)
        {
            symptom.SetActive(true);
        }
        else
        {
            symptom.SetActive(false);
        }
        StartCoroutine(CalculateNewSize());

    }

    public IEnumerator CalculateNewSize()
    {
        yield return new WaitForSeconds(0.02f);
        float height = symptomContent.GetComponent<VerticalLayoutGroup>().preferredHeight;
        symptomContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
    }

    public void SaveClick()
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);
        myDay.symptoms.Clear();
        myDay.cases.Clear();
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
        Debug.Log(json);
        errorProvider.text = "День сохранен!";
        errorProvider.gameObject.SetActive(true);

        StartCoroutine(GetCountParams_Coroutine(hashedEmail, json));
    }

    private IEnumerator GetCountParams_Coroutine(string hashedEmail, string json)
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
                int counter = 0;
                for (int i = 1; i <= snapshot.ChildrenCount; i++)
                {
                    if (snapshot.Child(i.ToString()).Child("date").Value.ToString() == myDay.date)
                    {
                        counter = i;
                    }
                }
                if (counter > 0)
                {
                    StartCoroutine(SaveMyDay_Coroutine(hashedEmail, json, counter.ToString()));
                }
                else
                {
                    string count = (snapshot.ChildrenCount + 1).ToString();
                    StartCoroutine(SaveMyDay_Coroutine(hashedEmail, json, count));
                }
            }
            else
            {
                StartCoroutine(SaveMyDay_Coroutine(hashedEmail, json, "1"));
            }
        }
    }
    private IEnumerator SaveMyDay_Coroutine(string hashedEmail, string json, string counter)
    {
        var DBTask = database.Child("MyDay").Child(hashedEmail).Child(counter).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {

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
                    string currData = DateTime.Now.Date.ToString("dd-MM-yyyy");
                    for (int i = 1; i <= snapshot.ChildrenCount; i++)
                    {
                        if (currData == snapshot.Child(i.ToString()).Child("date").Value.ToString())
                        {
                            myDay.mood = snapshot.Child(i.ToString()).Child("mood").Value.ToString();
                            for(int k = 0; k < snapshot.Child(i.ToString()).Child("symptoms").ChildrenCount; k++)
                            {
                                myDay.symptoms.Add(snapshot.Child(i.ToString()).Child("symptoms").Child(k.ToString()).Value.ToString());
                            }
                            for (int k = 0; k < snapshot.Child(i.ToString()).Child("cases").ChildrenCount; k++)
                            {
                                myDay.cases.Add(snapshot.Child(i.ToString()).Child("cases").Child(k.ToString()).Value.ToString());
                            }
                        }
                    }
                    UpdateMyDay();
                }
            }
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
        var DBTask = database.Child("Articles").SetRawJsonValueAsync(json);
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
