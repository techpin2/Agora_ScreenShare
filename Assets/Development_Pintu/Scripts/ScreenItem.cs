using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenItem : MonoBehaviour
{
    [SerializeField] private RawImage screenImage;
    [SerializeField] private TMP_Text windowName;
    [SerializeField] private GameObject outer;
    private Button button;
    private string windowId;

    private static ScreenItem currentlySelectedScreenItem;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickScreenItem);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickScreenItem);
    }

    public virtual void OnClickScreenItem()
    {
        // Deselect the previously selected ScreenItem if it exists
        if (currentlySelectedScreenItem != null && currentlySelectedScreenItem != this)
        {
            currentlySelectedScreenItem.Deselect();
        }

        // Select this ScreenItem
        Select();

        // Call the ScreenShare_Child method
        ScreenShare_Child screenShare_Child = FindAnyObjectByType<ScreenShare_Child>();
        screenShare_Child.OnStartShareBtnClicked(windowId);
    }

    public void UpdateScreenItemTitle(string windowName)
    {
        string[] screenName = windowName.Split('|');
        this.windowName.text = screenName[0];
        this.windowId = screenName[screenName.Length - 1];
    }

    public void UpdateScreenItemThumbnail(Texture texture)
    {
        this.screenImage.texture = texture;
    }

    private void Select()
    {
        outer.SetActive(true);
        currentlySelectedScreenItem = this;
    }

    private void Deselect()
    {
        outer.SetActive(false);
        if (currentlySelectedScreenItem == this)
        {
            currentlySelectedScreenItem = null;
        }
    }
}