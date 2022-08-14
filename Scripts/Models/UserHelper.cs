using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHelper : MonoBehaviour
{
    public static UserHelper Instance { get; set; }


    public string Email;    //�����
    public string Password; //������

    public string Name;     //���
    public int Age;     //�������
    public int Height; //����

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
