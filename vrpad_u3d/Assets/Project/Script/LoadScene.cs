using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadScene : MonoBehaviour {

    private AsyncOperation asyncOperation;

    // Use this for initialization
    void Start () {
        Invoke("ChangeScene", 1.0f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ChangeScene()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

}
