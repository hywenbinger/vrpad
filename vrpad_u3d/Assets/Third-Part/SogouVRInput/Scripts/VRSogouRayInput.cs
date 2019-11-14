using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSogouRayInput : MonoBehaviour {

    Ray m_Ray;
    RaycastHit m_HitResult;
    System.Action<RaycastHit> m_ActionRayCast;
	
	// Update is called once per frame
	void Update () {
        m_Ray = PvrInputMoudle.ray;
        //Debug.Log("RAY:  "+m_Ray.origin + "       "+m_Ray.direction);
        if (Physics.Raycast(m_Ray,out m_HitResult))
        {
            //Debug.Log("检测到碰撞");
            if (m_ActionRayCast != null)
                m_ActionRayCast(m_HitResult);
        }
    }

    public void SetActionEvent(System.Action<RaycastHit> actionRayCastHit)
    {
        m_ActionRayCast = actionRayCastHit;
    }

}
 