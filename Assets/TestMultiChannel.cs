using Agora.Rtc;
using Agora_RTC_Plugin.API_Example;
using UnityEngine;
using UnityEngine.Serialization;

public class TestMultiChannel : MonoBehaviour
{
    [SerializeField]
    private string _appID = "";

    [FormerlySerializedAs("TOKEN")]
    [SerializeField]
    private string _token = "";

    [FormerlySerializedAs("CHANNEL_NAME")]
    [SerializeField]
    private string _channelName = "";
    internal IRtcEngineEx RtcEngine = null;

    private uint _uid1 = 123;

    public void StartAudioChannel()
    {
        InitEngine();
        //JoinChannel1();
    }

    private void Update()
    {
        PermissionHelper.RequestMicrophontPermission();
    }

    private void InitEngine()
    {
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngineEx();
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngineContext context = new RtcEngineContext(_appID, 0,
            CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
            AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);

        RtcEngine.Initialize(context);
        RtcEngine.InitEventHandler(handler);
        RtcEngine.EnableAudio();
    }

    private void JoinChannel1()
    {
        //Audio
        ChannelMediaOptions channelMediaOptions = new ChannelMediaOptions();
        channelMediaOptions.publishMicrophoneTrack.SetValue(true);
        channelMediaOptions.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        RtcEngine.JoinChannelEx("", new RtcConnection(_channelName, _uid1), channelMediaOptions);       
    }

    public void MicToggle()
    {
        JoinChannel1();
        //RtcEngine.AdjustUserPlaybackSignalVolume(_uid1, 100);
    }

    #region -- Agora Event ---

    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly TestMultiChannel _sample;

        internal UserEventHandler(TestMultiChannel sample)
        {
            _sample = sample;
        }

        public override void OnError(int err, string msg)
        {
            Debug.Log(string.Format("OnError err: {0}, msg: {1}", err, msg));
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            int build = 0;
            Debug.Log(string.Format("sdk version: ${0}",
                _sample.RtcEngine.GetVersion(ref build)));
            Debug.Log(
                string.Format("OnJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}",
                    connection.channelId, connection.localUid, elapsed));
        }

        public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log("OnRejoinChannelSuccess");
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            Debug.Log("OnLeaveChannel");
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole,
            CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
        {
            Debug.Log("OnClientRoleChanged");
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.Log(
                string.Format("OnUserJoined uid: ${0} elapsed: ${1}", uid, elapsed));
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Debug.Log(string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
                (int)reason));
        }
    }

    #endregion
}
