using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.IO;

public class AuthManager : MonoBehaviour
{

    DatabaseReference database;

    public GameObject registration;
    public GameObject autentification;


    [Header("Registration")]
    public InputField Name;
    public InputField Age;
    public InputField Email;
    public InputField Password;
    public Text ErrorProvider;
    public Toggle PersonalData;

    [Header("Autentification")]
    public InputField AuthEmail;
    public InputField AuthPassword;
    public Text AuthErrorProvider;
    public SymptomsGroup symptomsGroup;

    private void Start()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        string path = Application.persistentDataPath + "/" + "Auth.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            UserModel user = JsonUtility.FromJson<UserModel>(json);
            StartCoroutine(Autorization_Coroutine(user));
        }
    }

    public void Registration()
    {
        bool check = errorChecker();
        if (check)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Email.text.ToLower()));
            string hashedEmail = Convert.ToBase64String(hash);

            md5 = MD5.Create();
            hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Password.text));
            string hashedPassword = Convert.ToBase64String(hash);

            UserModel user = new UserModel()
            {
                Id = hashedEmail,
                Name = Name.text,
                Email = Email.text.ToLower(),
                Password = hashedPassword,
                Age = int.Parse(Age.text),
                Symptoms = new List<string> {},
                Cases = new List<string> {}
            };

            string json = JsonUtility.ToJson(user);
            StartCoroutine(CheckUserExist_Coroutine(json, user));
    
        }
    }
    public void Authorization()
    {
        if (String.IsNullOrEmpty(AuthEmail.text))
        {
            AuthErrorProvider.text = "Введите Email";
            AuthErrorProvider.gameObject.SetActive(true);
            return;
        }
        if (String.IsNullOrEmpty(AuthPassword.text))
        {
            AuthErrorProvider.text = "Введите пароль";
            AuthErrorProvider.gameObject.SetActive(true);
            return;
        }

        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(AuthEmail.text.ToLower()));
        string hashedEmail = Convert.ToBase64String(hash);

        md5 = MD5.Create();
        hash = md5.ComputeHash(Encoding.UTF8.GetBytes(AuthPassword.text));
        string hashedPassword = Convert.ToBase64String(hash);

        UserModel user = new UserModel()
        {
            Id = hashedEmail,
            Password = hashedPassword
        };
        StartCoroutine(Autorization_Coroutine(user));
    }
    private IEnumerator CheckUserExist_Coroutine(string json, UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.Log("Some exeption on check");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot.Exists)
            {
                Debug.Log("isExist");
                ErrorProvider.text = "Пользователь с таким Email уже существует";
                ErrorProvider.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Okey");
                //yield return new WaitForSeconds(2);
                StartCoroutine(Registration_Coroutine(json, user));
            }
        }
    }

    private IEnumerator Registration_Coroutine(string json, UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on reg");
        }
        else
        {
            RegistrationManage(autentification);
            ErrorProvider.gameObject.SetActive(false);
        }
    }
    private IEnumerator Autorization_Coroutine(UserModel user)
    {
        var DBTask = database.Child("Users").Child(user.Id).GetValueAsync();
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
                if (user.Password == snapshot.Child("Password").Value.ToString())
                {
                    UserHelper.Instance.Email = snapshot.Child("Email").Value.ToString();
                    UserHelper.Instance.Name = snapshot.Child("Name").Value.ToString();
                    UserHelper.Instance.Surname = snapshot.Child("Surname").Value.ToString();
                    UserHelper.Instance.Age = int.Parse(snapshot.Child("Age").Value.ToString());
                    UserHelper.Instance.Height = int.Parse(snapshot.Child("Height").Value.ToString());
                    UserHelper.Instance.Password = snapshot.Child("Password").Value.ToString();

                    for (int i = 0; i < snapshot.Child("Symptoms").ChildrenCount; i++)
                    {
                        UserHelper.Instance.Symptoms.Add(snapshot.Child("Symptoms").Child(i.ToString()).Value.ToString());
                    }
                    for (int i = 0; i < snapshot.Child("Cases").ChildrenCount; i++)
                    {
                        UserHelper.Instance.Cases.Add(snapshot.Child("Cases").Child(i.ToString()).Value.ToString());
                    }
                    AuthErrorProvider.gameObject.SetActive(false);
                    Debug.Log("Successful Autorization");
                    StartCoroutine(GetCountParams_Coroutine(user.Id));
                    AuthErrorProvider.text = "Выполняется вход...";
                    AuthErrorProvider.gameObject.SetActive(true);
                    string path = Application.persistentDataPath + "/" + "Auth.json";
                    File.WriteAllText(path, JsonUtility.ToJson(user));
                    yield return new WaitForSeconds(2);
                    SceneManager.LoadScene(1);
                }
                else
                {
                    AuthErrorProvider.text = "Вы ввели неверный логин или пароль";
                    AuthErrorProvider.gameObject.SetActive(true);
                }
            }
            else
            {
                AuthErrorProvider.text = "Вы ввели неверный логин или пароль";
                AuthErrorProvider.gameObject.SetActive(true);
            }
        }
    }
    private IEnumerator GetCountParams_Coroutine(string hashedEmail)
    {
        var DBTask = database.Child("AppRun").Child(hashedEmail).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on check");
        }
        else
        {
            Debug.Log("Ok");
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot.Exists)
            {
                int counter = 0;
                RunAppModel runApp = new RunAppModel()
                {
                    Date = DateTime.Now.Date.ToString("dd-MM-yyyy"),
                    Count = 0
                };
                for (int i = 1; i <= snapshot.ChildrenCount; i++)
                {
                    if (snapshot.Child(i.ToString()).Child("Date").Value.ToString() == runApp.Date)
                    {
                        counter = i;
                    }
                }
                if (counter > 0)
                {
                    runApp.Count = int.Parse(snapshot.Child(counter.ToString()).Child("Count").Value.ToString()) + 1;
                    string json = JsonUtility.ToJson(runApp);
                    StartCoroutine(SaveMyEnter_Coroutine(hashedEmail, json, counter.ToString()));
                }
                else
                {
                    string json = JsonUtility.ToJson(runApp);
                    string count = (snapshot.ChildrenCount + 1).ToString();
                    StartCoroutine(SaveMyEnter_Coroutine(hashedEmail, json, count));
                }
            }
            else
            {
                RunAppModel runApp = new RunAppModel()
                {
                    Date = DateTime.Now.Date.ToString("dd-MM-yyyy"),
                    Count = 1
                };
                string json = JsonUtility.ToJson(runApp);
                StartCoroutine(SaveMyEnter_Coroutine(hashedEmail, json, "1"));
            }
        }
    }
    private IEnumerator SaveMyEnter_Coroutine(string hashedEmail, string json, string counter)
    {
        var DBTask = database.Child("AppRun").Child(hashedEmail).Child(counter).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {

        }
    }
    public void RegistrationManage(GameObject gameObject)
    {
        registration.SetActive(false);
        autentification.SetActive(false);
        AuthErrorProvider.gameObject.SetActive(false);
        ErrorProvider.gameObject.SetActive(false);
        Name.text = "";
        Age.text = "";
        Email.text = "";
        Password.text = "";
        gameObject.SetActive(true);
    }

    public bool errorChecker()
    {
        if (String.IsNullOrEmpty(Name.text))
        {
            ErrorProvider.text = "Введите имя";
            ErrorProvider.gameObject.SetActive(true);
            return false;
        }
        if (String.IsNullOrEmpty(Age.text))
        {
            ErrorProvider.text = "Введите возраст";
            ErrorProvider.gameObject.SetActive(true);
            return false;
        }
        if (String.IsNullOrEmpty(Email.text))
        {
            ErrorProvider.text = "Введите Email";
            ErrorProvider.gameObject.SetActive(true);
            return false;
        }
        if (String.IsNullOrEmpty(Password.text))
        {
            ErrorProvider.text = "Введите пароль";
            ErrorProvider.gameObject.SetActive(true);
            return false;
        }
        string cond = @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)";
        if(!Regex.IsMatch(Email.text, cond))
        {
            ErrorProvider.text = "Введите корректный Email";
            ErrorProvider.gameObject.SetActive(true);
            return false;
        }
        if(Password.text.Length < 5)
        {
            ErrorProvider.text = "Введите пароль от 5 до 10 символов";
            ErrorProvider.gameObject.SetActive(true);
            return false;
        }
        if (PersonalData.isOn == false)
        {
            ErrorProvider.text = "Дайте согласие на обработку персональных данных";
            ErrorProvider.gameObject.SetActive(true);
            return false;
        }
        return true;
    }

    public void ApprovalOpen(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    public void ApprovalClose(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
