using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class PageFlipperUI : MonoBehaviour
{
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;

    private TMP_Text text;
    private int currentPage = 1;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void SetText(string desc)
    {
        if (text == null) return;
        text.text = desc;
        CheckText(text.textInfo);
    }

    private void CheckText(TMP_TextInfo info)
    {
        nextPageButton.gameObject.SetActive(false);
        if (text != null && text.overflowMode == TextOverflowModes.Page && info.pageCount > 1)
        {
            nextPageButton.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        nextPageButton.onClick.RemoveAllListeners();
        prevPageButton.onClick.RemoveAllListeners();

        nextPageButton.onClick.AddListener(HandleNextPage);
        prevPageButton.onClick.AddListener(HandlePrevPage);
    }

    void HandleNextPage()
    {
        if (text == null || text.overflowMode != TextOverflowModes.Page) return;

        text.pageToDisplay = ++currentPage;
        if (currentPage == text.textInfo.pageCount) nextPageButton.gameObject.SetActive(false);
        if (prevPageButton.gameObject.activeSelf == false) prevPageButton.gameObject.SetActive(true);
    }

    void HandlePrevPage()
    {
        if (text == null || text.overflowMode != TextOverflowModes.Page) return;

        text.pageToDisplay = --currentPage;
        if (currentPage == 1) prevPageButton.gameObject.SetActive(false);
        if (nextPageButton.gameObject.activeSelf == false) nextPageButton.gameObject.SetActive(true);
    }
}
