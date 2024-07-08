using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenItem : MonoBehaviour
{
    public static ScreenItem currentlySelectedScreenItem;

    [SerializeField] private RawImage screenImage;
    [SerializeField] private TMP_Text windowName;
    [SerializeField] private GameObject outer;
    private Button button;
    private string windowId;

    #region Monobehaviour Methods
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickScreenItem);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickScreenItem);
    }

    #endregion

    #region Public Methods
    public void OnClickScreenItem()
    {
        ScreenShareClassroom screenShareClassroom = FindObjectOfType<ScreenShareClassroom>();

        if (currentlySelectedScreenItem != null && currentlySelectedScreenItem != this)
        {
            currentlySelectedScreenItem.Deselect();
            screenShareClassroom.OnUnplishButtonClick();
        }

        Select();

        if (screenShareClassroom)
        {
            screenShareClassroom.OnStartShareBtnClicked(windowId);
        }
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

    #endregion

    #region Private Methods
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

    #endregion
}