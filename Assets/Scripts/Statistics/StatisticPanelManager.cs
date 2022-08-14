using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticPanelManager : MonoBehaviour
{
    public GameObject StatisticsPanel;

    public void OpenStatistic(GameObject gameObject)
    {
        StatisticsPanel.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void CloseStatistic(GameObject gameObject)
    {
        StatisticsPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
