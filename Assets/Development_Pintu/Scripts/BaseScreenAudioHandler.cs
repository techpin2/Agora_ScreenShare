using Agora.Rtc;
using UnityEngine;
using Agora_RTC_Plugin.API_Example.Examples.Advanced.ScreenShare;
using Agora_RTC_Plugin.API_Example;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System;

public class BaseScreenAudioHandler : MonoBehaviour
{
    public static BaseScreenAudioHandler Instance;
    public static Action OnJoinAgoraChannel;
    public static Action<uint> OnLeaveAgoraChannel;
    public static Action<uint, int> OnUserAgoraJoined;
    public static Action<uint> OnUserAgoraOffline;

    [FormerlySerializedAs("appIdInput")]
    [SerializeField]
    private AppIdInput _appIdInput;

    [Header("_____________Basic Configuration_____________")]
    [FormerlySerializedAs("APP_ID")]
    [SerializeField]
    private string _appID = "";

    [FormerlySerializedAs("TOKEN")]
    [SerializeField]
    private string _token = "";

    [FormerlySerializedAs("CHANNEL_NAME")]
    [SerializeField]
    private string _channelName = "";

    internal IRtcEngine RtcEngine = null;
    public IRtcEngine GetRTCEngine => RtcEngine;

    [Header("AddOn")]
    [SerializeField] private Button joinButton;
    [SerializeField] private Button leaveButton;

    #region Monobehaviour_Method

    private void Awake()
    {
        Instance = this;
        joinButton.onClick.AddListener(JoinChannel);
        leaveButton.onClick.AddListener(LeaveChannel);
    }


    private void OnDestroy()
    {
        joinButton.onClick?.RemoveListener(JoinChannel);
        leaveButton.onClick.RemoveListener(LeaveChannel);
    }

    private void Start()
    {
        LoadAssetData();
        if (CheckAppId())
        {
            InitEngine();
            SetBasicConfiguration();
        }
    }


    #endregion
    
    private bool CheckAppId()
    {
        return _appID.Length > 10;
    }

    private void LoadAssetData()
    {
        if (_appIdInput == null) return;
        _appID = _appIdInput.appID;
        _token = _appIdInput.token;
        _channelName = _appIdInput.channelName;
    }

    private void InitEngine()
    {
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngineContext context = new RtcEngineContext(_appID, 0,
                                    CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                                    AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
        RtcEngine.Initialize(context);
        RtcEngine.InitEventHandler(handler);
    }

    private void SetBasicConfiguration()
    {
        RtcEngine.EnableAudio();
        RtcEngine.EnableVideo();
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
    }

    internal string GetChannelName()
    {
        return _channelName;
    }

    #region Private Methods

    private void JoinChannel()
    {
        UpdateJoinLeaveButtons(true);
        var ret = RtcEngine.JoinChannel(_token, _channelName);
        Debug.Log("JoinChannel returns: " + ret);
    }

    private void LeaveChannel()
    {
        UpdateJoinLeaveButtons(false);
        RtcEngine.StopScreenCapture();
        RtcEngine.LeaveChannel();
        Debug.Log("Leave Chanel ");
    }

    private void UpdateJoinLeaveButtons(bool enable)
    {
        if (joinButton != null && leaveButton != null)
        {
            joinButton.gameObject.SetActive(!enable);
            leaveButton.gameObject.SetActive(enable);
        }
    }

    #endregion

    #region -- Agora Event ---

    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly BaseScreenAudioHandler _desktopScreenShare;

        internal UserEventHandler(BaseScreenAudioHandler desktopScreenShare)
        {
            _desktopScreenShare = desktopScreenShare;
        }

        public override void OnError(int err, string msg)
        {
            Debug.Log(string.Format("OnError err: {0}, msg: {1}", err, msg));
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            int build = 0;
            Debug.Log(string.Format("sdk version: ${0}",
                _desktopScreenShare.RtcEngine.GetVersion(ref build)));
            Debug.Log(
                string.Format("OnJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}",
                                connection.channelId, connection.localUid, elapsed));

            OnJoinAgoraChannel?.Invoke();
        }

        public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log("OnRejoinChannelSuccess");
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            Debug.Log("OnLeaveChannel"+connection.localUid);
            OnLeaveAgoraChannel?.Invoke(connection.localUid);
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
        {
            Debug.Log("OnClientRoleChanged");
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.Log(string.Format("OnUserJoined uid: ${0} elapsed: ${1}", uid, elapsed));
            OnUserAgoraJoined?.Invoke(uid, elapsed);
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Debug.Log(string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid, (int)reason));
            OnUserAgoraOffline?.Invoke(uid);
        }
    }

    #endregion
}
