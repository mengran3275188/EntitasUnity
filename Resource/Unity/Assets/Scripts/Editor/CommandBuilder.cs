using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CommandBuild : Editor {

    [MenuItem("Custom/Build")]
    public static void Build()
    {
        string path = Application.streamingAssetsPath;

        string[] files = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);

        string resFile =  Path.Combine(path, "Files.ini");
        using(StreamWriter sw = new StreamWriter(resFile, false))
        {
            foreach (var file in files)
            {
                if(!file.EndsWith(".meta"))
                    sw.WriteLine(Util.HomePath.RelativePath(path, file));
            }
        }
    }
}
