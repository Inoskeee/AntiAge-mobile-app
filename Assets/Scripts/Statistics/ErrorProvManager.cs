using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorProvManager : MonoBehaviour
{
    public GameObject enabledObj;

    private void OnDisable()
    {
        enabledObj.SetActive(true);
    }

    private void OnEnable()
    {
        enabledObj.SetActive(false);
    }
}
