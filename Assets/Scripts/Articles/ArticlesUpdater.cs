using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticlesUpdater : MonoBehaviour
{
    //DatabaseReference database;


    public GameObject article;
    public GameObject articleContent;
    public ArticlesManager manager;

    public List<GameObject> articles = new List<GameObject>();

    private void OnEnable()
    {
        //database = FirebaseDatabase.DefaultInstance.RootReference;
        foreach(var item in articles)
        {
            GameObject.Destroy(item.gameObject);
        }
        articles.Clear();
        StartCoroutine(GetArticles_Coroutine());
    }

    private IEnumerator GetArticles_Coroutine()
    {
        var DBTask = manager.database.Child("Articles").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on check");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot.Exists)
            {
                for(int i = 1; i <= snapshot.ChildrenCount; i++)
                {
                    GameObject obj = GameObject.Instantiate(article);
                    ArticleHelper articleHelper = obj.GetComponent<ArticleHelper>();
                    articleHelper.header.text = snapshot.Child("Article" + i.ToString()).Child("ArticleName").Value.ToString();
                    articleHelper.content.text = snapshot.Child("Article" + i.ToString()).Child("Thumbnail").Value.ToString();
                    articleHelper.checkArticle.onClick.AddListener(() => manager.OpenArticle(articleHelper));
                    articleHelper.articleId = "Article"+i.ToString();
                    obj.transform.parent = articleContent.transform;
                    articles.Add(obj);
                }
                ChangeContent();
            }
        }
    }

    public void ChangeContent()
    {
        articleContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (380 * articles.Count));
        articleContent.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, 0);
    }
    public void OpenArticles()
    {
        manager.BookmarksList.gameObject.SetActive(false);
        manager.ArticlesList.gameObject.SetActive(true);
    }
}
