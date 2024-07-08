using Agora.Rtc;
using UnityEngine;
using UnityEngine.UI;

public class VoiceChatClassroom : MonoBehaviour
{
    [SerializeField] private Toggle micToggle;

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
    private void OnChannelJoin()
    {
        micToggle.gameObject.SetActive(true);
    }

    private void OnChannelLeave()
    {
        micToggle.gameObject.SetActive(false);
        micToggle.isOn = false;
    }

    private void StopPublishAudio()
    {
        var options = new ChannelMediaOptions();
        options.publishMicrophoneTrack.SetValue(false);
        var nRet = BaseScreenAudioHandler.Instance.GetRTCEngine.UpdateChannelMediaOptions(options);
        Debug.Log("UpdateChannelMediaOptions: " + nRet);
    }

    private void StartPublishAudio()
    {
        var options = new ChannelMediaOptions();
        options.publishMicrophoneTrack.SetValue(true);
        var nRet = BaseScreenAudioHandler.Instance.GetRTCEngine.UpdateChannelMediaOptions(options);
        Debug.Log("UpdateChannelMediaOptions: " + nRet);
    }

    #endregion
}
