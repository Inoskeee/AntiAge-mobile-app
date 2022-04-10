using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDayModel
{
    [SerializeField] public string Id;
    [SerializeField] public string date;

    [SerializeField] public string mood;

    [SerializeField] public List<string> symptoms;
    [SerializeField] public List<string> cases;
}
