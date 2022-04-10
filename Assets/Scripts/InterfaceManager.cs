using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public ProfileManager profileManager;
    public MyDayManager dayManager;

    public GameObject profile;
    public GameObject myDay;
    public GameObject caseManager;
    public GameObject deleteDialog;

    [Header("Case Manager")]
    public Text header;
    public InputField caseField;
    public Button saveButton;
    public ChangeCase changeCase;

    [Header("Delete Dialog")]
    public Text deleteHeader;
    public Button yesButton;
    public void OpenProfile()
    {
        myDay.SetActive(false);
        profile.SetActive(true);
    }

    public void CloseProfile()
    {
        profile.SetActive(false);
        myDay.SetActive(true);
        dayManager.UpdateMyDay();
    }

    public void OpenCaseManagerAdd()
    {
        caseManager.SetActive(true);
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(() => profileManager.AddCase());

        header.text = "Добавить новое дело";

    }
    public void OpenSymptomManagerAdd()
    {
        caseManager.SetActive(true);
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(() => profileManager.AddSymptom());

        header.text = "Добавить новый симптом";
    }

    public void OpenCaseManagerChange(Text text)
    {
        caseManager.SetActive(true);
        changeCase.oldName = text.text;
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(() => profileManager.ChangeCase());
        header.text = "Изменение дела";
        caseField.text = text.text;
    }
    public void OpenSymptomManagerChange(Text text)
    {
        caseManager.SetActive(true);
        changeCase.oldName = text.text;
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(() => profileManager.ChangeSymptom());
        header.text = "Изменение симптома";
        caseField.text = text.text;
    }

    public void OpenDeleteDialog(Text text)
    {
        deleteDialog.SetActive(true);
        yesButton.onClick.RemoveAllListeners();
        if(text.gameObject.tag == "Symptom")
        {
            yesButton.onClick.AddListener(() => profileManager.DelSymptom(text));
        }
        else if(text.gameObject.GetComponentInParent<GameObject>().tag == "Symptom")
        {
            yesButton.onClick.AddListener(() => profileManager.DelCase(text));
        }
        deleteHeader.text = $"Вы действительно ходите удалить {text.text}?";
    }
    public void CloseDeleteDialog()
    {
        deleteDialog.SetActive(false);
        profile.SetActive(true);
    }
    public void CloseCaseManager()
    {
        caseManager.SetActive(false);
        profile.SetActive(true);
    }
}
