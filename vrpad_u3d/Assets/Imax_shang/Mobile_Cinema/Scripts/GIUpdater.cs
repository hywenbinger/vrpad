using UnityEngine;
using System.Collections;

public class GIUpdater : MonoBehaviour {

	Renderer renderer;

	void Start() {
		renderer = GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update () {		
		if (renderer != null) {
			RendererExtensions.UpdateGIMaterials(renderer);
		}	
	}
}
