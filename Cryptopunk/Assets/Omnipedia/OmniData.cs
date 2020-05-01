using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniData : MonoBehaviour
{
    [SerializeField] TextAsset[] references;
    internal List<Article> articles;
    public static OmniData instance;
    internal struct Article
    {
        internal string title;
        internal string content;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(OmniData.instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            articles = new List<Article>();
            foreach (TextAsset reference in references)
            {
                ParseReference(reference.text);
            }
        }
    }

    private void ParseReference(string referenceText)
    {
        string nextLine = FileReaderUtils.GetNextLine(ref referenceText);
        while(nextLine!="++ENDFILE++")
        {
            Article newArticle;
            newArticle.title = nextLine;
            newArticle.content = FileReaderUtils.GetNextLine(ref referenceText);
            articles.Add(newArticle);
            Debug.Log("loaded article " + newArticle.title);
            nextLine = FileReaderUtils.GetNextLine(ref referenceText);
        }
    }
}
