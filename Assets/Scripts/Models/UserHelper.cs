using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHelper : MonoBehaviour
{
    public static UserHelper Instance { get; set; }


    public string Email;    //�����
    public string Password; //������

    public string Name;     //���
    public string Surname;     //���
    public int Age;     //�������
    public int Height; //����
    public List<string> Symptoms; //��������
    public List<string> Cases; //����

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
