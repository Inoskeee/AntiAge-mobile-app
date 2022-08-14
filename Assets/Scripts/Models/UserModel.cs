using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserModel
{
    [SerializeField] public string Id;
    [SerializeField] public string Email;    //�����
    [SerializeField] public string Password; //������

    [SerializeField] public string Name;     //���
    [SerializeField] public string Surname;     //���
    [SerializeField] public int Age;     //�������
    [SerializeField] public int Height; //����

    [SerializeField] public List<string> Symptoms; //��������
    [SerializeField] public List<string> Cases; //����
}
