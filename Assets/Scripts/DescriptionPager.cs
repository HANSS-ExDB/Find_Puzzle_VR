using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DescriptionPager : MonoBehaviour
{
    [System.Serializable]
    public class PageContent
    {
        [TextArea(3, 10)]
        public string text;
        public Sprite image;
    }

    public PageContent[] pages;

    public TextMeshProUGUI descriptionText;
    public Image displayImage;

    public Button previousButton;
    public Button nextButton;
    public Button confirmButton;

    private int currentPage = 0;

    void Start()
    {
        previousButton.onClick.AddListener(OnPrevious);
        nextButton.onClick.AddListener(OnNext);
        confirmButton.onClick.AddListener(OnConfirm);
        UpdatePage();
    }

    void UpdatePage()
    {
        descriptionText.text = pages[currentPage].text;
        displayImage.sprite = pages[currentPage].image;

        previousButton.gameObject.SetActive(currentPage > 0);
        nextButton.gameObject.SetActive(currentPage < pages.Length - 1);
        confirmButton.gameObject.SetActive(currentPage == pages.Length - 1);
    }

    void OnPrevious()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    void OnNext()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    void OnConfirm()
    {
        SceneManager.LoadScene("BasicScene"); // 바꾸세요: 이동할 씬 이름
    }
}
