using Agora.Rtc;
using Agora_RTC_Plugin.API_Example;
using UnityEngine;
using UnityEngine.UI;

public class VoiceChatClassroom : MonoBehaviour
{
    [SerializeField] private Toggle micToggle;

    private uint localUID;

    #region Monobehaviour Method
    private void Awake()
    {
        micToggle.onValueChanged.AddListener(OnToggleValueChangeed);
        BaseScreenAudioHandler.OnJoinAgoraChannel += OnChannelJoin;
        BaseScreenAudioHandler.OnLeaveAgoraChannel += OnChannelLeave;
    }

    private void OnDestroy()
    {
        micToggle.onValueChanged.RemoveListener(OnToggleValueChangeed);
        BaseScreenAudioHandler.OnJoinAgoraChannel -= OnChannelJoin;
        BaseScreenAudioHandler.OnLeaveAgoraChannel -= OnChannelLeave;
    }

    private void Update()
    {
        PermissionHelper.RequestMicrophontPermission();
    }

    #endregion

    #region Private Methods
    private void OnToggleValueChangeed(bool isOn)
    {
        if (isOn)
        {
            StopPublishAudio();
        }
        else
        {
            StartPublishAudio();
        }
    }
    private void OnChannelJoin(uint uid)
    {
        localUID = uid;
        micToggle.gameObject.SetActive(true);
        StopPublishAudio();
    }

    private void OnChannelLeave(uint localUid)
    {
        micToggle.gameObject.SetActive(false);
        micToggle.isOn = true;
    }

    private void StopPublishAudio()
    {
        var options = new ChannelMediaOptions();
        options.publishMicrophoneTrack.SetValue(false);
        //options.publishScreenCaptureAudio.SetValue(true);

        //options.publishCameraTrack.SetValue(true);
        ////options.publishScreenTrack.SetValue(false);

        var nRet = BaseScreenAudioHandler.Instance.GetRTCEngine.UpdateChannelMediaOptions(options);
        //var nRet = BaseScreenAudioHandler.Instance.GetRTCEngine.EnableLocalAudio(false);

        //var nRet = BaseScreenAudioHandler.Instance.GetRTCEngine.MuteLocalAudioStream(true);

        BaseScreenAudioHandler.Instance.GetRTCEngine.AdjustUserPlaybackSignalVolume(localUID, 0);
        //Debug.Log("UpdateChannelMediaOptions: " + nRet);
    }

    private void StartPublishAudio()
    {
        var options = new ChannelMediaOptions();
        options.publishMicrophoneTrack.SetValue(true);
        //options.publishScreenCaptureAudio.SetValue(true);

        //options.publishScreenTrack.SetValue(true);
        ////options.publishCameraTrack.SetValue(false);

        var nRet = BaseScreenAudioHandler.Instance.GetRTCEngine.UpdateChannelMediaOptions(options);
        //var nRet = BaseScreenAudioHandler.Instance.GetRTCEngine.EnableLocalAudio(true);
        
        //Debug.Log("UpdateChannelMediaOptions: " + nRet);
        
        BaseScreenAudioHandler.Instance.GetRTCEngine.AdjustUserPlaybackSignalVolume(localUID, 100);
    }

    #endregion
}
