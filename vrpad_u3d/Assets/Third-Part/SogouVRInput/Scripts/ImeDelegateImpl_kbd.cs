using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class SGViewGather
{
    private GameObject[] mViews;

    public bool isShow = false;

    public SGViewGather(GameObject[] param)
    {
        mViews = param;
    }
    public void SetActive(bool bActive)
    {
        foreach (GameObject view in mViews)
        {
            view.SetActive(bActive);
        }
        isShow = bActive;
    }
    public bool FindName(string name)
    {
        foreach (GameObject view in mViews)
        {
            if (view.name == name)
            {
                return true;
            }
        }
        return false;
    }
    public void SetTexture(Texture2D tex)
    {
        foreach (GameObject view in mViews)
        {
            Renderer rend = view.GetComponent<Renderer>();
            if(rend != null)
                rend.material.mainTexture = tex;
        }
    }
}

public class SGMouseTracker
{
    private bool mDownOld = false;
    private Vector2 mPtOld = new Vector2();
    private SGImeMotionEventType mEvent;
    private const float mTrackRadius = 10.0f;
    private long mTimeDown;
    private bool mLongPressed = false;
    private long mIntervelLongPress = 100;

    public bool Track(Vector2 pt, bool bDown)
    {
        bool bRes = false;
        if (bDown)
        {
            if (mDownOld)
            {
                mEvent = SGImeMotionEventType.ACTION_MOVE;
                if ( !mLongPressed )
                {
                    long timeDiff = DateTime.Now.Ticks - mTimeDown;
                    if (timeDiff > mIntervelLongPress)
                    {
                        mLongPressed = true;
                        mEvent = SGImeMotionEventType.ACTION_LONGPRESS;
                        bRes = true; //force sendmessage
                    }
                }
            }
            else
            {
                mEvent = SGImeMotionEventType.ACTION_DOWN;
                mTimeDown = DateTime.Now.Ticks;
                mLongPressed = false;
            }
        }
        else
        {
            if (mDownOld)
            {
                mEvent = SGImeMotionEventType.ACTION_UP;
            }
            else
            {
                //mEvent = SGImeMotionEventType.ACTION_HOVER_MOVE;
                mEvent = SGImeMotionEventType.ACTION_MOVE; //c++代码只识别move事件
            }
        }
        if (mDownOld != bDown)
        {
            bRes = true;
        }
        else if ( PointDist(mPtOld, pt) > mTrackRadius )
        {
            bRes = true;
        }
        mDownOld = bDown;

        if (bRes)
        {
            mPtOld = pt;
        }
        return bRes;
    }

    public bool TrackOuter()
    {
        bool bRes = false;
        if (mEvent != SGImeMotionEventType.ACTION_OUTSIDE)
        {
            SGImeMotionEventType eventOld = mEvent;
            mEvent = SGImeMotionEventType.ACTION_OUTSIDE;
        }
        return bRes;
    }

    public Vector2 GetPoint()
    {
        return mPtOld;
    }
    public SGImeMotionEventType GetEvent()
    {
        return mEvent;
    }

    private float PointDist(Vector2 ptNew, Vector2 ptOld)
    {
        return Math.Abs(ptNew[0] - ptOld[0]) + Math.Abs(ptNew[1] - ptOld[1]);
    }
}

public class ImeDelegateImpl_kbd : ImeDelegateBase

{
    public Text mText;
    public GameObject[] mKbdViews;
    public SGViewGather mKbdView;
    public ImeManager mManager;
    private Texture2D mTexture;
    private Vector2 mTextureSize = new Vector2(780,390);
    private Vector2 mPtKbd = new Vector2();
    private SGMouseTracker mTracker = new SGMouseTracker();

    public System.Action m_actionInputResult;
    //ImeDelegateBase
    public override void OnIMEShow(Vector2 vSize)
    {
        Debug.Log("OnIMEShow");
        mTextureSize = vSize;
        CreateTexture(vSize);
        mManager.Draw(mTexture);
        mKbdView.SetActive(true);
        //KeyBoardManager.GetInstance().ClearText();

        /**Pico   根据VR输入的特性实际添加**/
        VRSogouRayInput rayInput = GetComponent<VRSogouRayInput>();
        if (rayInput != null)
        {
            rayInput.SetActionEvent(CheckRaycastHitEvent);
            rayInput.enabled = true;
        }
    }
    public override void OnIMEHide()
    {
        Debug.Log("OnIMEHide");
        mKbdView.SetActive(false);
        VRSogouRayInput rayInput = GetComponent<VRSogouRayInput>();
        if (rayInput != null)
        {
            rayInput.enabled = false;
        }
        //KeyBoardManager.GetInstance().Close();
    }
    public override void OnIMECommit(string strCommit)
    {
        //mText.text += strCommit;
        //if(mKbdView.isShow)
        //    KeyBoardManager.GetInstance().Append(strCommit);
    }
    public override void OnIMEKey(SGImeKey key)
    {
        switch (key)
        {
            case SGImeKey.KEYCODE_DEL:
                //String strText = mText.text;
                //mText.text = strText.Remove(strText.Length - 1);
                //KeyBoardManager.GetInstance().DoBackspace();
                break;
            case SGImeKey.KEYCODE_ENTER:
                //mText.text = "";
                if (m_actionInputResult != null)
                    m_actionInputResult();
                break;
        }
    }
    public override void OnIMEError(SGImeError nType, string strErr)
    {
    }

    //MonoBehaviour
    void Start()
    {
        mKbdView = new SGViewGather(mKbdViews);
        //CreateTexture(mTextureSize);
#if UNITY_EDITOR
#else
        mKbdView.SetActive(false);
#endif
    }
    void Update()
    {
        /**设备上不希望做Mouse检测**/
#if UNITY_EDITOR 
        //CheckMouseEvent();
#endif
        if(mTexture != null)
            mManager.Draw(mTexture);
    }

    //other
    private void CreateTexture(Vector2 vSize)
    {
        if (mTexture)
        {
            return;
        }
#if UNITY_EDITOR
#else
        // Create a texture
        mTexture = new Texture2D((int)vSize.x, (int)vSize.y, TextureFormat.RGBA32, false);
        // Set point filtering just so we can see the pixels clearly
        mTexture.filterMode = FilterMode.Trilinear;
        // Call Apply() so it's actually uploaded to the GPU
        mTexture.Apply();

        Debug.Log("texture created");

        // Set texture onto cube
        //GameObject cube = GameObject.Find("Cube");
        //cube.GetComponent<Renderer>().material.mainTexture = mTexture;

        // Set texture onto kbdview
        mKbdView.SetTexture(mTexture);
#endif
    }

    private void DispatchMessageToAndroid(SGImeMotionEventType type, Vector2 pt)
    {
        if (null != mManager)
        {
            mManager.OnTouch(pt.x, pt.y, type);
        }
    }

    private void LogEvent(string prefix, PointerEventData eventData)
    {
        Debug.Log(prefix + ": " + eventData.pointerCurrentRaycast.gameObject.name + " x=" + eventData.position.x + ",y=" + eventData.position.y);
    }

    private void CheckMouseEvent()
    {
        if (Point2UV(Input.mousePosition, ref mPtKbd))
        {
            if (mTracker.Track(mPtKbd, Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
            {
                DispatchMessageToAndroid(mTracker.GetEvent(), mTracker.GetPoint());
            }
        } else if (mTracker.TrackOuter() )
        {
            DispatchMessageToAndroid(mTracker.GetEvent(), mTracker.GetPoint());
        }
    }


    /**Pico  根据VR输入的特性实际修改 原方法见上**/
    private void CheckRaycastHitEvent(RaycastHit hitInfo)
    {
        if (Point2UV(hitInfo, ref mPtKbd))
        {
            if (mTracker.Track(mPtKbd, InputController.GetInstance().TriggerDown || InputController.GetInstance().Triggering || Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.JoystickButton0) || Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
            {
                DispatchMessageToAndroid(mTracker.GetEvent(), mTracker.GetPoint());
            }

        }
        else if (mTracker.TrackOuter())
        {
            DispatchMessageToAndroid(mTracker.GetEvent(), mTracker.GetPoint());
        }
    }


    /**Pico  根据VR输入的特性实际修改 原方法见下**/
    private bool Point2UV(RaycastHit hitInfo, ref Vector2 ptUV)
    {
        bool bRes = false;
        string name = hitInfo.collider.gameObject.name;
        if (mKbdView.FindName(name))
        {
            GameObject kbd = hitInfo.collider.gameObject;
            Vector3 vecKbd = kbd.transform.InverseTransformPoint(hitInfo.point);
            Vector2 pixelUV = hitInfo.textureCoord;
            Renderer rend = hitInfo.transform.GetComponent<Renderer>();
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 texSize = new Vector2(780, 390);
            ptUV.x = pixelUV.x * texSize.x;
            ptUV.y = (1 - pixelUV.y) * texSize.y;
            //Debug.Log("ray click " + name + ": 3d point=" + vecKbd.ToString() + " uv=(" + pixelUV.x + "," + pixelUV.y + ") org=(" + ptUV.ToString() + ")" + " w=" + texSize.x + ",h=" + texSize.y);
            bRes = true;
        }
        return bRes;
    }


    private bool Point2UV(Vector3 ptScreen, ref Vector2 ptUV)
    {
        Ray ray = Camera.main.ScreenPointToRay(ptScreen);
        RaycastHit hitInfo;
        bool bRes = false;
        if (Physics.Raycast(ray, out hitInfo))
        {
            string name = hitInfo.collider.gameObject.name;
            if (mKbdView.FindName(name))
            {
                GameObject kbd = hitInfo.collider.gameObject;
                Vector3 vecKbd = kbd.transform.InverseTransformPoint(hitInfo.point);
                Vector2 pixelUV = hitInfo.textureCoord;
                Renderer rend = hitInfo.transform.GetComponent<Renderer>();
                Texture2D tex = rend.material.mainTexture as Texture2D;
                ptUV.x = pixelUV.x * mTextureSize.x;
                ptUV.y = (1 - pixelUV.y) * mTextureSize.y;
                //Debug.Log("ray click " + name + ": 3d point=" + vecKbd.ToString() + " uv=(" + pixelUV.x + "," + pixelUV.y + ") org=(" + ptUV.ToString() + ")" + " w=" + texSize.x + ",h=" + texSize.y);
                bRes = true;
            }
        }
        return bRes;
    }
}
