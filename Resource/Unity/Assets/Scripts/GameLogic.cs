using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityClient.Kernel;

public class GameLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = new WWW(Application.streamingAssetsPath + "/Files.ini");
        while (!www.isDone) { }
        if (null != www.error)
            Debug.LogError(www.error);

        using(MemoryStream ms = new MemoryStream(www.bytes))
        {
            using(StreamReader sr = new StreamReader(ms))
            {
                while(sr.Peek() >= 0)
                {
                    string filePath = sr.ReadLine();
                    var filewww = new WWW(Application.streamingAssetsPath + "/" + filePath);
                    while (!filewww.isDone) { }

                    string targetPath = Path.Combine(Application.persistentDataPath, filePath);

                    string dirPath = targetPath.Substring(0, targetPath.LastIndexOf('/'));
                    if(!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    File.WriteAllBytes(targetPath, filewww.bytes);
                }
            }
        }
        GameKernel.OnStart(Application.persistentDataPath);
#else
        GameKernel.OnStart(Application.streamingAssetsPath);
#endif
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        GameKernel.Update();
    }
    void FixedUpdate ()
    {
        GameKernel.FixUpdate();
	}

    private void OnDestroy()
    {
        GameKernel.OnQuit();
    }
}
