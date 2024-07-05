using UnityEngine;
using Agora_RTC_Plugin.API_Example.Examples.Advanced.ScreenShare;
using Agora.Rtc;

public class ScrenShare_Child : ScreenShare
{
    [Header("Addon")]
    [SerializeField] ScreenItem screenItem;
    [SerializeField] Transform parent;

    public override void PrepareScreenCapture()
    {
        base.PrepareScreenCapture();

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in _screenCaptureSourceInfos)
        {
            if (item.type == ScreenCaptureSourceType.ScreenCaptureSourceType_Window)
            {
                ScreenItem screenItems = Instantiate(screenItem, parent);
                OnShowThumbButtonClicked(item, screenItems);
                screenItems.UpdateScreenItemTitle(string.Format("{0}|{1}", item.sourceTitle, item.sourceId));
            }
        }
    }

    public void OnShowThumbButtonClicked(ScreenCaptureSourceInfo _screenCaptureSourceInfos, ScreenItem item)
    {
        ThumbImageBuffer thumbImageBuffer = _screenCaptureSourceInfos.thumbImage;
        if (thumbImageBuffer.buffer.Length == 0) return;
        Texture2D texture = null;
#if UNITY_STANDALONE_OSX
            texture = new Texture2D((int)thumbImageBuffer.width, (int)thumbImageBuffer.height, TextureFormat.RGBA32, false);
#elif UNITY_STANDALONE_WIN
        texture = new Texture2D((int)thumbImageBuffer.width, (int)thumbImageBuffer.height, TextureFormat.BGRA32, false);
#endif
        texture.LoadRawTextureData(thumbImageBuffer.buffer);
        texture.Apply();

        item.UpdateScreenItemThumbnail(texture);
    }

    public void OnStartShareBtnClicked(string windowId)
    {
        if (RtcEngine == null) return;
        RtcEngine.StopScreenCapture();
        var nRet = RtcEngine.StartScreenCaptureByWindowId(long.Parse(windowId), default(Rectangle), default(ScreenCaptureParameters));
        Debug.Log("StartScreenCaptureByWindowId:" + nRet);

        UpdatePublishUnPublishButtons(true);
        MakeVideoView(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN);
    }
}
