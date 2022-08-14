using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FitnessManager : MonoBehaviour
{
    public DatabaseReference database;


    public GameObject fitnessPanel;
    public GameObject physicalActivities;
    public GameObject otherActivities;
    public GameObject imtInfoDialog;
    public GameObject changeWeightDialog;
    public GameObject userProfile;

    public FitnessHelper fitnessHelper;

    [Header("PanelInfo")]
    public Text weight;
    public Text height;
    public Text imt;
    public Text imtValue;

    [Header("UpdateWeight")]
    public InputField newWeight;
    public Text errorProvider;

    [Header("ActivityChecker")]
    public List<PhysicalComplexModel> physicalModels;

    public void OpenComplex(GameObject gameObject)
    {
        if (gameObject.tag == "PhysicalActivity")
        {
            physicalActivities.SetActive(false);
        }
        else if (gameObject.tag == "OtherActivity")
        {
            otherActivities.SetActive(false);
        }
        gameObject.SetActive(true);
    }
    public void CloseComplex(GameObject gameObject)
    {
        gameObject.SetActive(false);
        if(gameObject.tag == "PhysicalActivity")
        {
            physicalActivities.SetActive(true);
            SetPhysicalInfo();
        }
        else if(gameObject.tag == "OtherActivity")
        {
            otherActivities.SetActive(true);
        }
    }
    public void OpenPhysicalActivities()
    {
        fitnessPanel.SetActive(false);
        physicalActivities.SetActive(true);
    }
    public void ClosePhysicalActivities()
    {
        fitnessPanel.SetActive(true);
        physicalActivities.SetActive(false);
    }
    public void OpenOtherActivities()
    {
        fitnessPanel.SetActive(false);
        otherActivities.SetActive(true);
    }
    public void CloseOtherActivities()
    {
        fitnessPanel.SetActive(true);
        otherActivities.SetActive(false);
    }
    public void OpenIMTDialog()
    {
        imtInfoDialog.SetActive(true);
    }
    public void CloseIMTDialog()
    {
        imtInfoDialog.SetActive(false);
    }
    public void OpenChangeWeight()
    {
        changeWeightDialog.SetActive(true);
        newWeight.text = "";
    }
    public void CloseChangeWeight()
    {
        changeWeightDialog.SetActive(false);
        newWeight.text = "";
    }
    public void OpenChangeHeight()
    {
        userProfile.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        fitnessHelper.Date = DateTime.Now.Date.ToString("dd-MM-yyyy");
        UpdatePhysicalInfo();
    }

    public void UpdatePhysicalInfo()
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        height.text = "Мой рост: " + UserHelper.Instance.Height; 
        StartCoroutine(GetWeight_Coroutine(hashedEmail));
    }

    private IEnumerator GetWeight_Coroutine(string hashedEmail)
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
                if(snapshot.ChildrenCount == 0)
                {
                    OpenChangeWeight();
                }
                else
                {
                    fitnessHelper.Weight = snapshot.Child(snapshot.ChildrenCount.ToString()).Child("Weight").Value.ToString();
                    weight.text = "Мой вес: " + fitnessHelper.Weight;
                    if(DateTime.Now.Date.ToString("dd-MM-yyyy") == snapshot.Child(snapshot.ChildrenCount.ToString()).Child("Date").Value.ToString())
                    {
                        for (int i = 0; i < snapshot.Child(snapshot.ChildrenCount.ToString()).Child("PhysicalComplex").ChildrenCount; i++)
                        {
                            if (!fitnessHelper.PhysicalComplex.Contains(snapshot.Child(snapshot.ChildrenCount.ToString()).Child("PhysicalComplex").Child(i.ToString()).Value.ToString()))
                            {
                                fitnessHelper.PhysicalComplex.Add(snapshot.Child(snapshot.ChildrenCount.ToString()).Child("PhysicalComplex").Child(i.ToString()).Value.ToString());
                                foreach (var item in physicalModels)
                                {
                                    if (item.ActivityId == snapshot.Child(snapshot.ChildrenCount.ToString()).Child("PhysicalComplex").Child(i.ToString()).Value.ToString())
                                    {
                                        item.checkbox.isOn = true;
                                    }
                                }
                            }
                        }
                    }

                    double myIMT = System.Math.Round(double.Parse(fitnessHelper.Weight) / (UserHelper.Instance.Height * UserHelper.Instance.Height) * 10000, 2);
                    fitnessHelper.Imt = myIMT.ToString();
                    imt.text = "Ваш IMT:" + myIMT;
                    if(myIMT < 18.5)
                    {
                        imtValue.text = "Ниже нормы";
                        imtValue.color = Color.red;
                    }
                    else if(myIMT < 25)
                    {
                        imtValue.text = "Норма";
                        imtValue.color = Color.green;
                    }
                    else if (myIMT < 30)
                    {
                        imtValue.text = "Избыточный вес";
                        imtValue.color = Color.red;
                    }
                    else if (myIMT < 35)
                    {
                        imtValue.text = "Ожирение 1 степени";
                        imtValue.color = Color.red;
                    }
                    else if (myIMT < 40)
                    {
                        imtValue.text = "Ожирение 2 степени";
                        imtValue.color = Color.red;
                    }
                    else if (myIMT >= 40)
                    {
                        imtValue.text = "Ожирение 3 степени";
                        imtValue.color = Color.red;
                    }
                }
            }
            else
            {
                OpenChangeWeight();
            }
        }
    }

    public void SetPhysicalInfo()
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        fitnessHelper.PhysicalComplex.Clear();
        foreach(var item in physicalModels)
        {
            if(item.checkbox.isOn == true)
            {
                fitnessHelper.PhysicalComplex.Add(item.ActivityId);
            }
        }

        FitnessModel fitnessModel = new FitnessModel
        {
            Date = fitnessHelper.Date,
            PhysicalComplex = fitnessHelper.PhysicalComplex,
            Weight = fitnessHelper.Weight,
            Imt = fitnessHelper.Imt,
        };
        string json = JsonUtility.ToJson(fitnessModel);
        StartCoroutine(GetCountParams_Coroutine(hashedEmail, json));
    }
    private IEnumerator GetCountParams_Coroutine(string hashedEmail, string json)
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
                int counter = 0;
                for(int i = 1; i <= snapshot.ChildrenCount; i++)
                {
                    if(snapshot.Child(i.ToString()).Child("Date").Value.ToString() == fitnessHelper.Date)
                    {
                        counter = i;
                    }
                }
                if(counter > 0)
                {
                    StartCoroutine(SetFitnessInfo_Coroutine(hashedEmail, json, counter.ToString()));
                }
                else
                {
                    string count = (snapshot.ChildrenCount + 1).ToString();
                    StartCoroutine(SetFitnessInfo_Coroutine(hashedEmail, json, count));
                }
            }
            else
            {
                StartCoroutine(SetFitnessInfo_Coroutine(hashedEmail, json, "1"));
            }
        }
    }
    private IEnumerator SetFitnessInfo_Coroutine(string hashedEmail, string json, string count)
    {
        var DBTask = database.Child("Fitness").Child(hashedEmail).Child(count).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {
            changeWeightDialog.gameObject.SetActive(false);
            UpdatePhysicalInfo();
        }
    }


    public void UpdateUserWeight()
    {
        errorProvider.gameObject.SetActive(false);
        if (int.Parse(newWeight.text) < 10 || int.Parse(newWeight.text) > 200)
        {
            errorProvider.text = "Введите значение веса от 10 до 200 кг";
            errorProvider.gameObject.SetActive(true);
        }
        else
        {
            fitnessHelper.Weight = newWeight.text;
            SetPhysicalInfo();
        }
    }
}
