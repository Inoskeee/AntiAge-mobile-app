using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHelper : MonoBehaviour
{
    public static UserHelper Instance { get; set; }


    public string Email;    //Логин
    public string Password; //Пароль

    public string Name;     //Имя
    public int Age;     //Возраст
    public int Height; //Рост

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
