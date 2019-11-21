using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    public MediaPlayerCtrl mPlayerCtrl;
    public RawImage mDisplayImage;
    public RectTransform mDisplayTran;
    public ImeManager mImeManager;
    private Texture2D mTexture2D;
    private AndroidJavaObject mJavaObj;
    private int mVirtualDisplayId = -1;

    // Use this for initialization
    void Start()
    {
        InputController.GetInstance().AddListener(ListenerEventType.APP, Back);
        InputController.GetInstance().AddListener(ListenerEventType.CANCEL, Back);
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            mJavaObj = new AndroidJavaObject("com.pvr.vrpad.PvrPadManager", jo);
            Invoke("PlayVideo", 3.0f);
            Invoke("InitSurface", 5.0f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTexture();
    }

    private void PlayVideo()
    {
        mPlayerCtrl.Play();
    }

    public void InitSurface()
    {
        mTexture2D = new Texture2D(1600, 2880, TextureFormat.RGB24, false, false);
        mVirtualDisplayId = mJavaObj.Call<int>("initSurface", (int)mTexture2D.GetNativeTexturePtr(), 1600, 2880, 560);
    }

    public void UpdateTexture()
    {
        if (mJavaObj == null)
            return;
        if (mJavaObj.Call<bool>("isUpdateFrame"))
        {
            mDisplayImage.texture = mTexture2D;
            mJavaObj.Call("updateTexture");
            GL.InvalidateState();
        }
    }

    public void Release()
    {
        if (mJavaObj == null)
            return;
        mJavaObj.Call("release");
    }

    public void EventDown(float x, float y)
    {
        if (mJavaObj == null)
            return;
        mJavaObj.Call("down", x, y);
    }

    public void EventUp(float x, float y)
    {
        if (mJavaObj == null)
            return;
        mJavaObj.Call("up", x, y);
    }

    public void OnDisplayListener(string action)
    {
        // action = 1, display added
        // action = 2, display removeded
        // action = 3, display changed
        if (action.Equals("1"))
        {
            OpenApp();
            mDisplayImage.gameObject.SetActive(true);
        }
    }

    private void OpenApp()
    {
        //mJavaObj.Call("startApp", "com.tencent.mm", "com.tencent.mm.ui.LauncherUI");
        mJavaObj.Call("startApp", "com.pvr.vrpad", "com.pvr.vrpad.AppActivity");
        //mJavaObj.Call("startApp", "com.sina.weibo", "com.sina.weibo.SplashActivity");
    }

    public void ActivityOnResume()
    {
        if (mJavaObj == null || mVirtualDisplayId == -1)
            return;
        OpenApp();
    }

    public void SetDisplayRotation(string rotation)
    {
        // ROTATION_0 = 0
        // ROTATION_90 = 1
        // ROTATION_180 = 2
        // ROTATION_270 = 3
        float r = float.Parse(rotation);
        mDisplayTran.localEulerAngles = new Vector3(0.0f, 0.0f, r * 90.0f);
        mImeManager.transform.localPosition = (r == 0.0f || r == 2.0f) ? new Vector3(276.0f, -70.0f, -133.0f) : new Vector3(256.0f, -15.0f, -145.0f);
    }

    public void Back()
    {
        if (mImeManager.IsShow())
        {
            mImeManager.Hide();
            return;
        }
        mJavaObj.Call("back");
    }

    public void OnEventChanged(string msg)
    {
        if (!string.IsNullOrEmpty(msg) && msg.Contains("|"))
        {
            string[] ss = msg.Split('|');
            int action = int.Parse(ss[0]);
            int keyCode = int.Parse(ss[1]);
            if (action == 1 && keyCode == 4 && mImeManager.IsShow())
            {
                mImeManager.Hide();
            }
        }
    }

    void OnDestory()
    {
        InputController.GetInstance().RemoveListener(ListenerEventType.APP, Back);
        InputController.GetInstance().RemoveListener(ListenerEventType.CANCEL, Back);
    }

}
