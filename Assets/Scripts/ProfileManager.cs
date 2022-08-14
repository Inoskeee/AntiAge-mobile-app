using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    DatabaseReference database;

    public InterfaceManager interfaceManager;
    public Text errorProvider;

    public InputField Name;
    public InputField Surname;
    public InputField Age;
    public InputField Height;
    public InputField Email;

    public GameObject symptomContent;
    public GameObject caseContent;

    [Header("Prefabs")]
    public GameObject symptom;
    public GameObject someCase;
    public List<GameObject> Symptoms;
    public List<GameObject> Cases;

    [Header("SymptomManager")]
    public InputField newCase;
    public ChangeCase changeCase;
    public Text ErrorProvider;

    private void Awake()
    {
        UpdateProfile();
    }
    private void Start()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void UpdateProfile()
    {
        foreach(var item in Symptoms)
        {
            GameObject.Destroy(item);
        }
        foreach (var item in Cases)
        {
            GameObject.Destroy(item);
        }
        SymptomsGroup symptomsGroup = GameObject.FindGameObjectWithTag("Player").GetComponent<SymptomsGroup>();
        symptomsGroup.USERS = UserHelper.Instance.Symptoms;
        Name.text = UserHelper.Instance.Name;
        Surname.text = UserHelper.Instance.Surname;
        Age.text = UserHelper.Instance.Age.ToString();
        Height.text = UserHelper.Instance.Height.ToString();
        Email.text = UserHelper.Instance.Email;

        for (int i = 0; i < UserHelper.Instance.Symptoms.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(symptom);
            obj.GetComponentInChildren<Text>().text = UserHelper.Instance.Symptoms[i];
            obj.transform.parent = symptomContent.transform;
            obj.GetComponent<SymptomModel>().removeButton.onClick.AddListener(() => interfaceManager.OpenDeleteDialog(obj.GetComponent<SymptomModel>().symptomName));
            obj.GetComponent<SymptomModel>().changeButton.onClick.AddListener(() => interfaceManager.OpenSymptomManagerChange(obj.GetComponent<SymptomModel>().symptomName));
            Symptoms.Add(obj);
        }
        for (int i = 0; i < UserHelper.Instance.Cases.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(someCase);
            obj.GetComponentInChildren<Text>().text = UserHelper.Instance.Cases[i];
            obj.transform.parent = caseContent.transform;
            obj.GetComponent<SymptomModel>().removeButton.onClick.AddListener(() => interfaceManager.OpenDeleteDialog(obj.GetComponent<SymptomModel>().symptomName));
            obj.GetComponent<SymptomModel>().changeButton.onClick.AddListener(() => interfaceManager.OpenCaseManagerChange(obj.GetComponent<SymptomModel>().symptomName));
            Cases.Add(obj);
        }
        symptomContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (70 * UserHelper.Instance.Symptoms.Count));
        symptomContent.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, 0);

        caseContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (70 * UserHelper.Instance.Cases.Count));
        caseContent.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, 0);
    }

    public void AddSymptom()
    {
        ErrorProvider.gameObject.SetActive(false);
        if (UserHelper.Instance.Symptoms.Contains(newCase.text))
        {
            ErrorProvider.text = "Такой симптом уже существует";
            ErrorProvider.gameObject.SetActive(true);
            return;
        }
        UserHelper.Instance.Symptoms.Add(newCase.text);
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Email.text));
        string hashedEmail = Convert.ToBase64String(hash);

        UserModel user = new UserModel()
        {
            Id = hashedEmail,
            Name = UserHelper.Instance.Name,
            Surname = UserHelper.Instance.Surname,
            Email = UserHelper.Instance.Email,
            Password = UserHelper.Instance.Password,
            Age = UserHelper.Instance.Age,
            Height = UserHelper.Instance.Height,
            Symptoms = UserHelper.Instance.Symptoms,
            Cases = UserHelper.Instance.Cases,
        };

        string json = JsonUtility.ToJson(user);

        newCase.text = "";
        StartCoroutine(AddSymptom_Coroutine(json, user));
    }

    private IEnumerator AddSymptom_Coroutine(string json, UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {
            interfaceManager.CloseCaseManager();
            UpdateProfile();
        }
    }

    public void AddCase()
    {
        ErrorProvider.gameObject.SetActive(false);
        if (UserHelper.Instance.Cases.Contains(newCase.text))
        {
            ErrorProvider.text = "Такое занятие уже существует";
            ErrorProvider.gameObject.SetActive(true);
            return;
        }
        UserHelper.Instance.Cases.Add(newCase.text);
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Email.text));
        string hashedEmail = Convert.ToBase64String(hash);

        UserModel user = new UserModel()
        {
            Id = hashedEmail,
            Name = UserHelper.Instance.Name,
            Surname = UserHelper.Instance.Surname,
            Email = UserHelper.Instance.Email,
            Password = UserHelper.Instance.Password,
            Age = UserHelper.Instance.Age,
            Height = UserHelper.Instance.Height,
            Symptoms = UserHelper.Instance.Symptoms,
            Cases = UserHelper.Instance.Cases,
        };

        string json = JsonUtility.ToJson(user);

        newCase.text = "";
        StartCoroutine(AddCase_Coroutine(json, user));
    }

    private IEnumerator AddCase_Coroutine(string json, UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {
            interfaceManager.CloseCaseManager();
            UpdateProfile();
        }
    }


    public void DelSymptom(Text name)
    {
        UserHelper.Instance.Symptoms.Remove(name.text);
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Email.text));
        string hashedEmail = Convert.ToBase64String(hash);

        UserModel user = new UserModel()
        {
            Id = hashedEmail,
            Name = UserHelper.Instance.Name,
            Surname = UserHelper.Instance.Surname,
            Email = UserHelper.Instance.Email,
            Password = UserHelper.Instance.Password,
            Age = UserHelper.Instance.Age,
            Height = UserHelper.Instance.Height,
            Symptoms = UserHelper.Instance.Symptoms,
            Cases = UserHelper.Instance.Cases,
        };

        string json = JsonUtility.ToJson(user);
        StartCoroutine(DelSymptom_Coroutine(json, user));
    }
    private IEnumerator DelSymptom_Coroutine(string json, UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {
            interfaceManager.CloseDeleteDialog();
            UpdateProfile();
        }
    }

    public void DelCase(Text name)
    {
        UserHelper.Instance.Cases.Remove(name.text);
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Email.text));
        string hashedEmail = Convert.ToBase64String(hash);

        UserModel user = new UserModel()
        {
            Id = hashedEmail,
            Name = UserHelper.Instance.Name,
            Surname = UserHelper.Instance.Surname,
            Email = UserHelper.Instance.Email,
            Password = UserHelper.Instance.Password,
            Age = UserHelper.Instance.Age,
            Height = UserHelper.Instance.Height,
            Symptoms = UserHelper.Instance.Symptoms,
            Cases = UserHelper.Instance.Cases,
        };

        string json = JsonUtility.ToJson(user);
        StartCoroutine(DelCase_Coroutine(json, user));
    }
    private IEnumerator DelCase_Coroutine(string json, UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {
            interfaceManager.CloseDeleteDialog();
            UpdateProfile();
        }
    }

    public void ChangeSymptom()
    {
        ErrorProvider.gameObject.SetActive(false);
        if (UserHelper.Instance.Symptoms.Contains(changeCase.newName))
        {
            ErrorProvider.text = "Такой симптом уже существует";
            ErrorProvider.gameObject.SetActive(true);
            return;
        }
        for (int i = 0; i < UserHelper.Instance.Symptoms.Count; i++)
        {
            if(UserHelper.Instance.Symptoms[i] == changeCase.oldName)
            {
                UserHelper.Instance.Symptoms[i] = changeCase.newName;
            }
        }
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Email.text));
        string hashedEmail = Convert.ToBase64String(hash);

        UserModel user = new UserModel()
        {
            Id = hashedEmail,
            Name = UserHelper.Instance.Name,
            Surname = UserHelper.Instance.Surname,
            Email = UserHelper.Instance.Email,
            Password = UserHelper.Instance.Password,
            Age = UserHelper.Instance.Age,
            Height = UserHelper.Instance.Height,
            Symptoms = UserHelper.Instance.Symptoms,
            Cases = UserHelper.Instance.Cases,
        };

        string json = JsonUtility.ToJson(user);
        StartCoroutine(ChangeSymptom_Coroutine(json, user));
    }
    private IEnumerator ChangeSymptom_Coroutine(string json, UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {
            interfaceManager.CloseCaseManager();
            UpdateProfile();
        }
    }

    public void ChangeCase()
    {
        ErrorProvider.gameObject.SetActive(false);
        if (UserHelper.Instance.Cases.Contains(changeCase.newName))
        {
            ErrorProvider.text = "Такое занятие уже существует";
            ErrorProvider.gameObject.SetActive(true);
            return;
        }
        for (int i = 0; i < UserHelper.Instance.Cases.Count; i++)
        {
            if (UserHelper.Instance.Cases[i] == changeCase.oldName)
            {
                UserHelper.Instance.Cases[i] = changeCase.newName;
            }
        }
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Email.text));
        string hashedEmail = Convert.ToBase64String(hash);

        UserModel user = new UserModel()
        {
            Id = hashedEmail,
            Name = UserHelper.Instance.Name,
            Surname = UserHelper.Instance.Surname,
            Email = UserHelper.Instance.Email,
            Password = UserHelper.Instance.Password,
            Age = UserHelper.Instance.Age,
            Height = UserHelper.Instance.Height,
            Symptoms = UserHelper.Instance.Symptoms,
            Cases = UserHelper.Instance.Cases,
        };

        string json = JsonUtility.ToJson(user);
        StartCoroutine(ChangeCase_Coroutine(json, user));
    }
    private IEnumerator ChangeCase_Coroutine(string json, UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {
            interfaceManager.CloseCaseManager();
            UpdateProfile();
        }
    }

    private void OnEnable()
    {
        errorProvider.gameObject.SetActive(false);
    }

    public void SaveClick()
    {
        errorProvider.gameObject.SetActive(false);
        int heihgt = int.Parse(Height.text);
        int age = int.Parse(Age.text);
        if(heihgt == 0) { }
        else if(heihgt < 100 || heihgt > 200)
        {
            errorProvider.text = "Рост должен быть от 100 до 200";
            errorProvider.gameObject.SetActive(true);
            return;
        }
        if (age < 10 || age > 100)
        {
            errorProvider.text = "Возраст должен быть от 10 до 100";
            errorProvider.gameObject.SetActive(true);
            return;
        }
        errorProvider.text = "Профиль сохранен!";
        errorProvider.gameObject.SetActive(true);
        UserHelper.Instance.Surname = Surname.text;
        UserHelper.Instance.Height = int.Parse(Height.text);
        UserHelper.Instance.Age = int.Parse(Age.text);
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        UserModel user = new UserModel()
        {
            Id = hashedEmail,
            Name = UserHelper.Instance.Name,
            Surname = UserHelper.Instance.Surname,
            Email = UserHelper.Instance.Email,
            Password = UserHelper.Instance.Password,
            Age = UserHelper.Instance.Age,
            Height = UserHelper.Instance.Height,
            Symptoms = UserHelper.Instance.Symptoms,
            Cases = UserHelper.Instance.Cases,
        };

        string json = JsonUtility.ToJson(user);
        StartCoroutine(SaveUser_Coroutine(json, user));
    }
    private IEnumerator SaveUser_Coroutine(string json, UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {

        }
    }

    public void Logoff()
    {
        string path = Application.persistentDataPath + "/" + "Auth.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        SceneManager.LoadScene(0);
    }
}
