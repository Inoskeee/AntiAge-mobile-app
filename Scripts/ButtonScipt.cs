using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonScipt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private Color highlighted;
    [SerializeField] private Color normal;

    void Start()
    {
        GetComponent<Text>().color = normal;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.LogError("KYPCOP HABEDEH");
        GetComponent<Text>().color = highlighted;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Text>().color = normal;
    }
}
