using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    public GameObject myDay;
    public GameObject Articles;
    public GameObject Fitness;
    public GameObject MentalHealth;
    public GameObject Analytics;
    public GameObject UserProfile;
    public GameObject ManageCases;
    public GameObject DeleteDialog;

    public void OpenBlock(GameObject gameObject)
    {
        myDay.gameObject.SetActive(false);
        Articles.gameObject.SetActive(false);
        Fitness.gameObject.SetActive(false);
        MentalHealth.gameObject.SetActive(false);
        Analytics.gameObject.SetActive(false);
        UserProfile.gameObject.SetActive(false);
        ManageCases.gameObject.SetActive(false);
        DeleteDialog.gameObject.SetActive(false);

        gameObject.SetActive(true);
    }
}
