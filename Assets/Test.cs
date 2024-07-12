using Agora.Rtc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string appID;
    public static IRtcEngineEx mRtcEngine1;
    public static IRtcEngineEx mRtcEngine2;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    //void Start()
    //{
    //    if (mRtcEngine == null)
    //    {
    //        mRtcEngine = IRtcEngineEx.GetEngine(appID);
    //    }

    //    mRtcEngine.SetMultiChannelWant(true);

    //    if (mRtcEngine == null)
    //    {
    //        Debug.Log("engine is null");
    //        return;
    //    }

    //    mRtcEngine.EnableVideo();
    //    mRtcEngine.EnableVideoObserver();
    //}

    //private void OnApplicationQuit()
    //{
    //    IRtcEngine.Destroy();
    //}
}
