using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewAdapter : MonoBehaviour
{
    public RectTransform prefab;
    public int countText;
    public RectTransform content;

    public void Updateitems()
    {
        int modelsCount = countText;
        StartCoroutine(GetItems(modelsCount, results => OnReceivedModels(results)));
    }

    void OnReceivedModels(ScrollViewModel[] models)
    {
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach(var model in models)
        {
            var instance = GameObject.Instantiate(prefab.gameObject) as GameObject;
            instance.transform.SetParent(content, false);
            InitializeItemView(instance, model);
        }
    }

    void InitializeItemView(GameObject viewGameObject, ScrollViewModel model)
    {
        ScrollViewItem view = new ScrollViewItem(viewGameObject.transform);
        view.titleText.text = model.title;
        view.clickButton.GetComponentInChildren<Text>().text = model.buttonText;
        view.clickButton.onClick.AddListener(() => { Debug.Log(view.titleText.text + " is clicked!"); });
    }

    IEnumerator GetItems(int count, System.Action<ScrollViewModel[]> callback)
    {
        yield return new WaitForSeconds(1f);
        
        var results = new ScrollViewModel[count];
        for(int i = 0; i < count; i++)
        {
            results[i] = new ScrollViewModel();
            results[i].title = $"Статья {i + 1}";
            results[i].buttonText = $"Информация о статье № {i + 1}";
        }

        callback(results);
    }
}

public class ScrollViewModel
{
    public string title;
    public string buttonText;
}

public class ScrollViewItem
{
    public Text titleText;
    public Button clickButton;

    public ScrollViewItem(Transform rootView)
    {
        titleText = rootView.Find("TitleText").GetComponent<Text>();
        clickButton = rootView.Find("ClickButton").GetComponent<Button>();
    }
}