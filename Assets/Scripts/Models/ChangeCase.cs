using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCase : MonoBehaviour
{
    public string oldName;
    public string newName;

    public InputField nameCase;

    private void Update()
    {
        newName = nameCase.text;
    }
}
