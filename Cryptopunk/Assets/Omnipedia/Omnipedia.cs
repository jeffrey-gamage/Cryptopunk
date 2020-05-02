using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Omnipedia : MonoBehaviour
{
    [SerializeField] InputField searchBar;
    [SerializeField] GameObject linkPrefab;
    [SerializeField] Button backToSearch;
    [SerializeField] Text titleDisplay;
    [SerializeField] Text contentDisplay;
    [SerializeField] Transform linkParent;
    [SerializeField] Vector3 linkOffest;

    [SerializeField] Image searchIcon;
    [SerializeField] Text inputTextDisplay;
    [SerializeField] Text placeholderDisplay;
    [SerializeField] int maxLinks =7;
    private bool isDisplayingArticle= false;
    private List<OmniDataEntry> articleLinks;

    private void Start()
    {
        articleLinks = new List<OmniDataEntry>();
        searchBar.onValueChanged.AddListener(delegate
        {
            if (!isDisplayingArticle)
            {
                DisplaySearchResults();
            }
        });
    }

    public void DisplaySearchResults()
    {
        Debug.Log("searching...");
        ClearArticleLinks();
        int articleCount = 0;
        foreach (OmniData.Article article in OmniData.instance.articles)
        {
            if (searchBar.text.Length > 0)
            {
                if (articleCount<maxLinks&&(article.title.Contains(searchBar.text.ToUpper())||searchBar.text.ToUpper().Contains(article.title)))
                {
                    OmniDataEntry newLink = Instantiate(linkPrefab, linkParent.position + linkOffest * articleCount, Quaternion.identity,linkParent).GetComponent<OmniDataEntry>();
                    Debug.Log("found relevant article: " + article.title);
                    articleLinks.Add(newLink);
                    newLink.SetTitle(article.title);
                    articleCount++;
                }
            }
        }
    }

    private void ClearArticleLinks()
    {
        foreach (OmniDataEntry articleLink in articleLinks)
        {
            Destroy(articleLink.gameObject);
        }
        articleLinks.Clear();
    }

    internal void DisplayArticle(string title)
    {
        ClearArticleLinks();
        ToggleDisplayMode(true);
        foreach (OmniData.Article article in OmniData.instance.articles)
        {
            if (article.title == title)
            {
                titleDisplay.text = article.title;
                contentDisplay.text = article.content;
                break;
            }
        }
    }

    private void ToggleDisplayMode(bool shouldDisplayArticle)
    {
        backToSearch.enabled = shouldDisplayArticle;
        backToSearch.GetComponent<Image>().enabled = shouldDisplayArticle;
        backToSearch.GetComponentInChildren<Text>().enabled = shouldDisplayArticle;
        inputTextDisplay.enabled = !shouldDisplayArticle;
        placeholderDisplay.enabled = !shouldDisplayArticle;
        searchIcon.enabled = !shouldDisplayArticle;
        isDisplayingArticle = shouldDisplayArticle;
    }

    public void BackToSearch()
    {
        ToggleDisplayMode(false);
        titleDisplay.text = "";
        inputTextDisplay.text = "";
        contentDisplay.text = "";
    }
    
    public void Back()
    {
        SceneManager.LoadScene("desktop");
    }
}
