using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Event : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IInitializePotentialDragHandler
{

    private const int KEY_EVENT_DOWN = 0;
    private const int KEY_EVENT_UP = 1;
    private bool mIsHover;
    private bool mIsDown;
    public Main mMainScript;

    // Use this for initialization
    void Start()
    {
        PUIEventListener.Get(gameObject).onHover += (GameObject obj, bool ishover) => { mIsHover = ishover; };
    }

    // Update is called once per frame
    void Update()
    {
        if (mIsHover && !mIsDown)
        {
            DispatchMessageToAndroid(KEY_EVENT_UP, null);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DispatchMessageToAndroid(KEY_EVENT_DOWN, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DispatchMessageToAndroid(KEY_EVENT_UP, eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        DispatchMessageToAndroid(KEY_EVENT_DOWN, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }

    private void DispatchMessageToAndroid(int actionType, PointerEventData eventData)
    {
        Vector2 size = this.gameObject.GetComponent<RectTransform>().sizeDelta;
        Vector3 point = this.gameObject.transform.InverseTransformPoint(PvrInputMoudle.CurrentRaycastResult.worldPosition);
        float x = (point.x + size.x / 2) / size.x;
        float y = (size.y / 2 - point.y) / size.y;
        if (Application.platform == RuntimePlatform.Android)
        {
            if (actionType == KEY_EVENT_DOWN)
            {
                mIsDown = true;
                mMainScript.EventDown(x, y);
            }
            else if (actionType == KEY_EVENT_UP)
            {
                mMainScript.EventUp(x, y);
                mIsDown = false;
            }
        }
        else
        {
            //Debug.Log(actionType + "---------------------------------x = " + x + ", y = " + y);
        }
    }
}
