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

    [Header("Autentification")]
    public InputField AuthEmail;
    public InputField AuthPassword;
    public Text AuthErrorProvider;

    private void Start()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Registration()
    {
        bool check = errorChecker();
        if (check)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Email.text));
            string hashedEmail = Convert.ToBase64String(hash);

            md5 = MD5.Create();
            hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Password.text));
            string hashedPassword = Convert.ToBase64String(hash);

            UserModel user = new UserModel()
            {
                Id = hashedEmail,
                Name = Name.text,
                Email = Email.text,
                Password = hashedPassword,
                Age = int.Parse(Age.text),
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
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(AuthEmail.text));
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
                    UserHelper.Instance.Age = int.Parse(snapshot.Child("Age").Value.ToString());
                    UserHelper.Instance.Password = snapshot.Child("Password").Value.ToString();
                    AuthErrorProvider.gameObject.SetActive(false);
                    Debug.Log("Successful Autorization");
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

    public void RegistrationManage(GameObject gameObject)
    {
        registration.SetActive(false);
        autentification.SetActive(false);
        AuthErrorProvider.gameObject.SetActive(false);
        ErrorProvider.gameObject.SetActive(false);
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
        return true;
    }
}
