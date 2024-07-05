using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenItem : MonoBehaviour
{
    [SerializeField] private RawImage screenImage; 
    [SerializeField] private TMP_Text windowName;
    private Button button;
    private string windowId;

    private void Awake()
    {
        button= GetComponent<Button>();
        button.onClick.AddListener(OnClickScreenItem);
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickScreenItem);
    }

    public virtual void OnClickScreenItem()
    {
        ScreenShare_Child screnShare_Child= FindAnyObjectByType<ScreenShare_Child>();
        screnShare_Child.OnStartShareBtnClicked(windowId);
    }

    public void UpdateScreenItemTitle(string windowName)
    {
        string[] name = windowName.Split('|');
        this.windowName.text = name[0];
        this.windowId= name[name.Length-1];
    }
    public void UpdateScreenItemThumbnail(Texture texture)
    {
        this.screenImage.texture = texture;
    }
} 
