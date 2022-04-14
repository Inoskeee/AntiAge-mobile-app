using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewAdapter : MonoBehaviour
{
    public RectTransform prefab;
    public RectTransform content;

    private string path = "Assets/Resources/MetaData/Facts.json";
    private Facts[] facts;

    public void Updateitems()
    {
        facts = GetFactsList();
        int modelsCount = facts.Length;
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
            results[i].title = "Статья \"" + facts[i].title + "\"";
            results[i].buttonText = facts[i].description_text;
        }

        callback(results);
    }

    Facts[] GetFactsList()
    {
        string json = File.ReadAllText(path, Encoding.GetEncoding("utf-8"));
        return JsonHelper.FromJson<Facts>(json);
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

[Serializable]
public class Facts
{
    public string image_path { get; set; }
    public string title { get; set; }
    public string description_text { get; set; }
    public string text { get; set; }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}