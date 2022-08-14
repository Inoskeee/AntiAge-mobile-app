using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserModel
{
    [SerializeField] public string Id;
    [SerializeField] public string Email;    //Логин
    [SerializeField] public string Password; //Пароль

    [SerializeField] public string Name;     //Имя
    [SerializeField] public string Surname;     //Имя
    [SerializeField] public int Age;     //Возраст
    [SerializeField] public int Height; //Рост

    [SerializeField] public List<string> Symptoms; //Симптомы
    [SerializeField] public List<string> Cases; //Дела
}
