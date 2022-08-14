using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArticlesManager : MonoBehaviour
{
    public DatabaseReference database;

    public string selectedArticle;

    public GameObject ArticlesPanel;
    public GameObject ArticlesList;
    public GameObject BookmarksList;
    public GameObject LoadingPanel;

    [Header("ArticleInfo")]
    public GameObject Article;
    public Text ArticleHeader;
    public Text ArticleContent;
    public Button bookmarkButton;

    public GameObject bookmark;
    public GameObject bookmarkContent;
    public List<string> bookmarksList;
    public List<GameObject> bookmarksGameObjects;

    void Start()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;

        ArticlesPanel.gameObject.SetActive(true);
        Article.gameObject.SetActive(false);
        ArticlesList.gameObject.SetActive(true);
        BookmarksList.gameObject.SetActive(false);


        foreach (var item in bookmarksGameObjects)
        {
            GameObject.Destroy(item.gameObject);
        }
        bookmarksGameObjects.Clear();
        bookmarksList.Clear();

        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        StartCoroutine(GetBookmarks_Coroutine(hashedEmail));
    }
    
    public void CloseArticle()
    {
        ArticlesPanel.gameObject.SetActive(true);
        ArticlesPanel.gameObject.SetActive(true);
        Article.gameObject.SetActive(false);
        ArticlesList.gameObject.SetActive(true);
        BookmarksList.gameObject.SetActive(false);
    }
    public void OpenArticle(ArticleHelper articleHelper)
    {
        LoadingPanel.gameObject.SetActive(true);
        StartCoroutine(GetArticle_Coroutine(articleHelper));
    }

    private IEnumerator GetArticle_Coroutine(ArticleHelper articleHelper)
    {
        var DBTask = database.Child("Articles").Child(articleHelper.articleId).GetValueAsync();
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
                selectedArticle = articleHelper.articleId;
                ArticleHeader.text = snapshot.Child("ArticleName").Value.ToString();
                ArticleContent.text = snapshot.Child("ArticleText").Value.ToString();
                if (bookmarksList.Contains(articleHelper.articleId))
                {
                    bookmarkButton.gameObject.SetActive(false);
                }
                else
                {
                    bookmarkButton.gameObject.SetActive(true);
                }
                //string url = snapshot.Child("ArticleImage").Value.ToString();
                //StartCoroutine(GetImage_Coroutine(url));
                Article.gameObject.SetActive(true);
                LoadingPanel.gameObject.SetActive(false);
                ArticlesPanel.gameObject.SetActive(false);
            }
        }
    }

    //private IEnumerator GetImage_Coroutine(string url)
    //{
    //    Debug.Log(url);
    //    UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
    //    yield return www.SendWebRequest();

    //    Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
    //    ArticleImage.sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero, 10f);

    //    Article.gameObject.SetActive(true);
    //    LoadingPanel.gameObject.SetActive(false);
    //    ArticlesPanel.gameObject.SetActive(false);
    //}

    public void AddBookmark()
    {
        if (bookmarksList.Contains(selectedArticle))
        {
            return;
        }
        bookmarksList.Add(selectedArticle);
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);
        BookmarkModel bookmarkModel = new BookmarkModel()
        {
            Id = "ok",
            bookmarks = bookmarksList
        };
        string json = JsonUtility.ToJson(bookmarkModel);
        Debug.Log(json);
        StartCoroutine(AddBookmark_Coroutine(json, hashedEmail));
    }
    private IEnumerator AddBookmark_Coroutine(string json, string hashedEmail)
    {
        var DBTask = database.Child("Bookmarks").Child(hashedEmail).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Some exeption on add");
        }
        else
        {

        }
    }

    public void DeleteBookmark(GameObject obj)
    {

        bookmarksList.Remove(obj.GetComponent<BookmarkHelper>().articleId);
        bookmarksGameObjects.Remove(obj);
        GameObject.Destroy(obj);
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);

        BookmarkModel bookmarkModel = new BookmarkModel()
        {
            Id = "ok",
            bookmarks = bookmarksList
        };
        string json = JsonUtility.ToJson(bookmarkModel);
        StartCoroutine(AddBookmark_Coroutine(json, hashedEmail));
    }

    public void GetBookmarks()
    {
        foreach (var item in bookmarksGameObjects)
        {
            GameObject.Destroy(item.gameObject);
        }
        bookmarksGameObjects.Clear();
        bookmarksList.Clear();

        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(UserHelper.Instance.Email));
        string hashedEmail = Convert.ToBase64String(hash);
        
        StartCoroutine(GetBookmarks_Coroutine(hashedEmail));
        BookmarksList.gameObject.SetActive(true);
        ArticlesList.gameObject.SetActive(false);
    }
    private IEnumerator GetBookmarks_Coroutine(string hashedEmail)
    {
        var DBTask = database.Child("Bookmarks").Child(hashedEmail).GetValueAsync();
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
                for (int i = 0; i < snapshot.Child("bookmarks").ChildrenCount; i++)
                {
                    bookmarksList.Add(snapshot.Child("bookmarks").Child(i.ToString()).Value.ToString());
                    Debug.Log(snapshot.Child("bookmarks").Child(i.ToString()).Value.ToString());
                }
                StartCoroutine(GetArticles_Coroutine());
            }
        }
    }

    private IEnumerator GetArticles_Coroutine()
    {
        var DBTask = database.Child("Articles").GetValueAsync();
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
                foreach(var item in bookmarksList)
                {
                    GameObject obj = GameObject.Instantiate(bookmark);
                    BookmarkHelper bookmarkHelper = obj.GetComponent<BookmarkHelper>();
                    bookmarkHelper.header.text = snapshot.Child(item).Child("ArticleName").Value.ToString();
                    bookmarkHelper.content.text = snapshot.Child(item).Child("Thumbnail").Value.ToString();
                    ArticleHelper articleHelper = new ArticleHelper() { articleId = item };
                    bookmarkHelper.checkArticle.onClick.AddListener(() => OpenArticle(articleHelper));
                    bookmarkHelper.deleteArticle.onClick.AddListener(() => DeleteBookmark(obj));
                    bookmarkHelper.articleId = item;
                    obj.transform.parent = bookmarkContent.transform;
                    bookmarksGameObjects.Add(obj);
                }
                
            }
        }
    }

}
