using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityClient.Kernel;

public class GameLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameKernel.OnStart(Application.streamingAssetsPath);
	}
	
	// Update is called once per frame
	void Update () {
        GameKernel.OnTick();
	}

    private void OnDestroy()
    {
        GameKernel.OnQuit();
    }
}
