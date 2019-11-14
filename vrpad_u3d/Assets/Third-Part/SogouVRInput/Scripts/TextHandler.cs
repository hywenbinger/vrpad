using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextHandler : MonoBehaviour, IPointerUpHandler
{
    public ImeManager mManager;
    [SerializeField]
    private SGImeInputType mInputType;
    [SerializeField]
    private SGImeTextType mTextType;

    /// <summary>
    /// 显示输入结果的Text
    /// </summary>
    [SerializeField]
    private UnityEngine.UI.Text m_Text;

    private System.Action m_ActionInputResult;

    public void OnPointerUp(PointerEventData eventData)
    {
        LogEvent("click text", eventData);
        mManager.Show(mInputType, mTextType);
        ImeDelegateImpl_kbd kdb = mManager.mDelegate as ImeDelegateImpl_kbd;
        if (m_Text != null)
            kdb.mText = m_Text;
        else
            Debug.LogError("缺少显示输入的字符的Text");
        kdb.m_actionInputResult = m_ActionInputResult;
    }

    private void LogEvent(string prefix, PointerEventData eventData)
    {
        Debug.Log(prefix + ": " + eventData.pointerCurrentRaycast.gameObject.name + " x=" + eventData.position.x + ",y=" + eventData.position.y);
    }


    public void SetActionResult(System.Action actionResult)
    {
        m_ActionInputResult = actionResult;
    }

}
