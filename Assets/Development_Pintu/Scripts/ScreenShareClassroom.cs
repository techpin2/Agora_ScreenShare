using Agora.Rtc;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShareClassroom : MonoBehaviour
{
    [SerializeField] private Button getCaptureScreenButton;
    [SerializeField] private Button startPublishButton;
    [SerializeField] private Button stopPublishButton;
    [SerializeField] private Transform screensParent;
    [SerializeField] private ScreenItem screenItemPrefab;
    [SerializeField] private Transform videoSurfaceParent;

    private VideoSurfaceClassroom videoSurface;
    private GameObject previewGameObject;
    private ScreenCaptureSourceInfo[] _screenCaptureSourceInfos;
    #region Monobehaviour Methods

    private void Awake()
    {
        getCaptureScreenButton.onClick.AddListener(PrepareScreenCapture);
        startPublishButton.onClick.AddListener(OnPublishButtonClick);
        stopPublishButton.onClick.AddListener(OnUnplishButtonClick);
        BaseScreenAudioHandler.OnJoinAgoraChannel += OnJoinChannel;
        BaseScreenAudioHandler.OnLeaveAgoraChannel += OnLeaveChannel;
        BaseScreenAudioHandler.OnUserAgoraJoined += OnUserJoined;
        BaseScreenAudioHandler.OnUserAgoraOffline += OnUserOffline;
    }

    private void OnDestroy()
    {
        getCaptureScreenButton.onClick.RemoveListener(PrepareScreenCapture);
        startPublishButton.onClick.RemoveListener(OnPublishButtonClick);
        stopPublishButton.onClick.RemoveListener(OnUnplishButtonClick);
        BaseScreenAudioHandler.OnJoinAgoraChannel -= OnJoinChannel;
        BaseScreenAudioHandler.OnLeaveAgoraChannel -= OnLeaveChannel;
        BaseScreenAudioHandler.OnUserAgoraJoined -= OnUserJoined;
        BaseScreenAudioHandler.OnUserAgoraOffline -= OnUserOffline;
    }

    #endregion

    #region Private Methods
    private void OnJoinChannel()
    {
        getCaptureScreenButton.gameObject.SetActive(true);
    }

    private void OnLeaveChannel()
    {
        getCaptureScreenButton.gameObject.SetActive(false);
        startPublishButton.gameObject.SetActive(false);
        stopPublishButton.gameObject.SetActive(false);

        DestroyPreviewGameObject(0);
        //videoSurface?.DestroyTexture();

        foreach (Transform child in screensParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnUserOffline(uint uid)
    {
        DestroyPreviewGameObject(uid);
    }

    private void OnUserJoined(uint uid, int elapsed)
    {
        MakeVideoView(uid, BaseScreenAudioHandler.Instance.GetChannelName(), VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
    }

    private void PrepareScreenCapture()
    {
       if( BaseScreenAudioHandler.Instance.GetRTCEngine == null) return;

        SIZE t = new SIZE();
        t.width = 1280;
        t.height = 720;
        SIZE s = new SIZE();
        s.width = 640;
        s.height = 640;
        _screenCaptureSourceInfos = BaseScreenAudioHandler.Instance.GetRTCEngine.GetScreenCaptureSources(t, s, true);

        foreach (Transform child in screensParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in _screenCaptureSourceInfos)
        {
            if (item.type == ScreenCaptureSourceType.ScreenCaptureSourceType_Window)
            {
                ScreenItem screens = Instantiate(screenItemPrefab, screensParent);
                OnShowThumbButtonClicked(item, screens);
                screens.UpdateScreenItemTitle(string.Format("{0}|{1}", item.sourceTitle, item.sourceId));
            }
        }
    }

    private void OnShowThumbButtonClicked(ScreenCaptureSourceInfo _screenCaptureSourceInfos, ScreenItem item)
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

    private void MakeVideoView(uint uid, string channelId = "", VIDEO_SOURCE_TYPE videoSourceType = VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA)
    {
        var go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        var videoSurfaceClassroom = MakeImageSurface(uid.ToString());
        if (ReferenceEquals(videoSurfaceClassroom, null)) return;
        videoSurfaceClassroom.SetForUser(uid, channelId, videoSourceType);
        videoSurfaceClassroom.SetEnable(true);
    }

    private VideoSurface MakeImageSurface(string goName)
    {
        //DestroyPreviewGameObject(uint.Parse(goName));

        previewGameObject = new GameObject();

        if (previewGameObject == null)
        {
            return null;
        }
        previewGameObject.name = goName;

        // to be renderered onto
        previewGameObject.AddComponent<RawImage>();
        previewGameObject.transform.SetParent(videoSurfaceParent);

        // set up transform
        previewGameObject.transform.Rotate(180f, 0.0f, 0.0f);
        previewGameObject.transform.localPosition = Vector3.zero;
        previewGameObject.GetComponent<RectTransform>().sizeDelta=new Vector2(1280, 720);

        // configure videoSurface
        var videoSurface = previewGameObject.AddComponent<VideoSurfaceClassroom>();
        this.videoSurface = videoSurface; 
        return videoSurface;
    }

    private void OnPublishButtonClick()
    {
        ChannelMediaOptions options = new ChannelMediaOptions();
        //options.publishCameraTrack.SetValue(false);
        options.publishScreenTrack.SetValue(true);

        var ret = BaseScreenAudioHandler.Instance.GetRTCEngine.UpdateChannelMediaOptions(options);
        Debug.Log("UpdateChannelMediaOptions returns: " + ret);

        startPublishButton.gameObject.SetActive(false);
        stopPublishButton.gameObject.SetActive(true);
        getCaptureScreenButton.gameObject.SetActive(false);
    }

    public void OnUnplishButtonClick()
    {
        //videoSurface.DestroyTexture();  //To Destroy RendererTexture

        ChannelMediaOptions options = new ChannelMediaOptions();
        //options.publishCameraTrack.SetValue(true);
        options.publishScreenTrack.SetValue(false);
        var ret = BaseScreenAudioHandler.Instance.GetRTCEngine.UpdateChannelMediaOptions(options);
        Debug.Log("UpdateChannelMediaOptions returns: " + ret);

        startPublishButton.gameObject.SetActive(true);
        stopPublishButton.gameObject.SetActive(false);
        getCaptureScreenButton.gameObject.SetActive(true);
    }

    private void DestroyPreviewGameObject(uint uid)
    {
        var go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Destroy(go);
        }

        //if (previewGameObject != null)
        //{
        //    Destroy(previewGameObject);
        //}
    }

    #endregion

    #region Public Methods
    public void OnStartShareBtnClicked(string windowId)
    {
        if (BaseScreenAudioHandler.Instance.GetRTCEngine == null) return;
        BaseScreenAudioHandler.Instance.GetRTCEngine.StopScreenCapture();

        var nRet = BaseScreenAudioHandler.Instance.GetRTCEngine.StartScreenCaptureByWindowId(long.Parse(windowId), default(Rectangle), default(ScreenCaptureParameters));
        Debug.Log("StartScreenCaptureByWindowId:" + nRet);

        startPublishButton.gameObject.SetActive(true);
        stopPublishButton.gameObject.SetActive(false);

        MakeVideoView(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN);
    }

    #endregion
}
