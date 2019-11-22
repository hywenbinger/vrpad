using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlay : MonoBehaviour
{

    private Camera m_RtCamrea;
    private Pvr_UnitySDKEyeOverlay m_overlay;
    private Transform m_holeTran;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.JoystickButton0))
        {
            //SaveImage(m_RtCamrea.targetTexture);
            //Debug.Log(m_RtCamrea.targetTexture.width+"---"+m_RtCamrea.targetTexture.height+"---"+m_RtCamrea.orthographicSize);
        }
    }

    public void Init()
    {
        GameObject m_RtCamrea_obj = GameObject.Find("Camera");
        GameObject m_overlay_obj = GameObject.Find("Quad");
        GameObject m_holeTran_obj = GameObject.Find("Hole");
        if (m_RtCamrea_obj == null || m_overlay_obj == null || m_holeTran_obj == null)
        {
            return;
        }
        m_RtCamrea = m_RtCamrea_obj.GetComponent<Camera>();
        m_overlay = m_overlay_obj.GetComponent<Pvr_UnitySDKEyeOverlay>();
        m_holeTran = m_holeTran_obj.transform;
        if (m_RtCamrea == null || m_overlay == null || m_holeTran == null)
        {
            return;
        }
        m_overlay.imageType = Pvr_UnitySDKEyeOverlay.ImageType.StandardTexture;
        m_overlay.SetTexture(m_RtCamrea.targetTexture);
        m_holeTran.gameObject.GetComponent<MeshRenderer>().material.mainTexture = m_RtCamrea.targetTexture;
    }

    public void UpdateTexture()
    {
        if (m_RtCamrea == null || m_overlay == null || m_holeTran == null)
        {
            return;
        }
        m_overlay.imageType = Pvr_UnitySDKEyeOverlay.ImageType.StandardTexture;
        m_overlay.SetTexture(m_RtCamrea.targetTexture);
        m_holeTran.gameObject.GetComponent<MeshRenderer>().material.mainTexture = m_RtCamrea.targetTexture;
    }

    public void OverlayChanged(string rotation)
    {
        
        if (m_RtCamrea == null || m_overlay == null || m_holeTran == null)
        {
            return;
        }
        bool is_H = (rotation.Equals("1") || rotation.Equals("3")) ? true : false;
        Debug.Log("is_H：" + is_H);
        if (is_H)
        {
            m_RtCamrea.orthographicSize = 0.75f;
            m_overlay.transform.localScale = new Vector3(900f, 500f, 1f);
            m_holeTran.transform.localScale = m_overlay.transform.localScale;
        }
        else
        {
            m_RtCamrea.orthographicSize = 1.35f;
            m_overlay.transform.localScale = new Vector3(500f, 900f, 1f);
            m_holeTran.transform.localScale = m_overlay.transform.localScale;
        }
        ChangeRT_Size(m_RtCamrea.targetTexture, is_H);
        m_overlay.imageType = Pvr_UnitySDKEyeOverlay.ImageType.StandardTexture;
        m_overlay.SetTexture(m_RtCamrea.targetTexture);
        m_holeTran.gameObject.GetComponent<MeshRenderer>().material.mainTexture = m_RtCamrea.targetTexture;
    }


    void ChangeRT_Size(RenderTexture rt, bool is_H)
    {
        rt.Release();
        rt.width = is_H ? 2880 : 1600;
        rt.height = is_H ? 1600 : 2880;
        rt.useMipMap = true;
        rt.autoGenerateMips = true;
        rt.filterMode = FilterMode.Trilinear;
        rt.mipMapBias = -0.7f;
        rt.Create();
        m_RtCamrea.rect = new Rect(0, 0, 1, 1);
    }

    void SaveImage(Texture saveTexture)
    {
        if (saveTexture != null)
        {
            RenderTexture aaaa = new RenderTexture((int)(saveTexture.width), (int)(saveTexture.height), 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(saveTexture, aaaa);
            RenderTexture a = RenderTexture.active;
            RenderTexture.active = aaaa;
            Texture2D screenShot = new Texture2D((int)(saveTexture.width), (int)(saveTexture.height),
                                                 TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, aaaa.width, aaaa.height), 0, 0);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = "//sdcard//DCIM//" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            System.IO.File.WriteAllBytes(filename, bytes);
            Texture2D.DestroyImmediate(screenShot);
            screenShot = null;
            RenderTexture.active = a;
        }
        else
        {
            Debug.Log("RenderTexture not ok ");
        }
    }

}
