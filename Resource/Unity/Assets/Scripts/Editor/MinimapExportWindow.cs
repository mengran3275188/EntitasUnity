using System.IO;
using UnityEngine;
using UnityEditor;

public class MinimapExportWindow : EditorWindow
{
    private static Vector2 m_WinMinSize = new Vector2(315.0f,  400.0f);
    private static Rect m_WinPosition = new Rect(100.0f, 100.0f, 315.0f, 400.0f);

    private string m_SavePath;

    [MenuItem("Custom/Minimap/Export minimap")]
    private static void Init()
    {
        MinimapExportWindow window = EditorWindow.GetWindow<MinimapExportWindow>("Minimap Exporter", true, typeof(EditorWindow));
        window.position = m_WinPosition;
        window.minSize = m_WinMinSize;
        window.wantsMouseMove = true;
        window.Show();
        window.Initialize();
    }
    private void Initialize()
    {
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        m_SavePath = EditorGUILayout.TextField("SaveFolder", m_SavePath);
        if(GUILayout.Button("Select", GUILayout.MaxWidth(50)))
        {
            m_SavePath = EditorUtility.SaveFolderPanel(
                "Select path to save minimap",
                m_SavePath,
                "Scene");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("FixPivot", GUILayout.MaxWidth(80)))
        {
            FixPivot();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Generate", GUILayout.MaxWidth(80)))
        {
            GenerateMinimap();
        }
    }
    private void FixPivot()
    {
        string[] blockPaths = Directory.GetFiles(Application.dataPath + "/" + "Resources/Blocks/", "*.prefab");
        foreach (var path in blockPaths)
        {
            string assetPath = GetAssetPath(path);
            GameObject blockObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            blockObject.transform.position = Vector3.zero;
            Bounds bounds = GetBounds(blockObject);

            if(blockObject.transform.childCount != 1)
            {
                Debug.LogError(assetPath + " has more than one child!");
                continue;
            }
            Transform blockChild = blockObject.transform.GetChild(0);
            Vector3 blockChildPosition = blockChild.position - bounds.center;
            blockChildPosition.y = 0;
            blockChild.position = blockChildPosition; 

        }
        AssetDatabase.SaveAssets();
    }
    private void GenerateMinimap()
    {
        string[] blockPaths = Directory.GetFiles(Application.dataPath + "/" + "Resources/Blocks/", "*.prefab");
        foreach(var path in blockPaths)
        {
            string assetPath = GetAssetPath(path);
            GameObject blockObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject blockInstance = GameObject.Instantiate(blockObject);
            blockInstance.transform.position = Vector3.zero;
            blockInstance.transform.rotation = Quaternion.identity;
            Bounds bounds = GetBounds(blockInstance);

            Vector3 size = bounds.max - bounds.min;
            Vector3 center = bounds.center;

            Camera mainCamera = Camera.main;
            mainCamera.orthographic = true;
            mainCamera.aspect = 1.0f;
            mainCamera.orthographicSize = Mathf.Max(size.x, size.z) / 2;
            mainCamera.farClipPlane = 10000;
            mainCamera.transform.position = center + new Vector3(0, 1000.0f, 0);
            mainCamera.transform.LookAt(center);

            RenderTexture currentActiveRt = RenderTexture.active;

            int photoSizeX = (int)size.x * 10;
            int photoSizeY = (int)size.z * 10;
            RenderTexture tCameraRT = new RenderTexture(photoSizeX, photoSizeY, 24);
            mainCamera.targetTexture = tCameraRT;
            RenderTexture.active = tCameraRT;
            mainCamera.Render();

            Texture2D photo = new Texture2D(photoSizeX, photoSizeY, TextureFormat.RGBA32, false);
            photo.ReadPixels(new Rect(0, 0, photoSizeX, photoSizeY), 0, 0);
            photo.Apply();

            RenderTexture.active = null;
            mainCamera.targetTexture = null;
            RenderTexture.active = currentActiveRt;

            byte[] bytes;
            bytes = photo.EncodeToPNG();
            System.IO.File.WriteAllBytes(Path.Combine(m_SavePath, blockObject.name + ".png"), bytes);

            GameObject.DestroyImmediate(blockInstance);
        }
        AssetDatabase.Refresh();
    }

    private Bounds GetBounds(GameObject obj)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach(Collider collider in colliders)
        {
            bounds.Encapsulate(collider.bounds);
        }
        /*
        MeshFilter[] mfs = obj.GetComponentsInChildren<MeshFilter>();
        foreach(MeshFilter mf in mfs)
        {
            if (null == mf.sharedMesh)
                break;
            Vector3 position = mf.transform.position;
            Bounds tempBounds = mf.sharedMesh.bounds;
            tempBounds.center = position;
            bounds.Encapsulate(tempBounds);
        }
        */
        return bounds;
    }
    private string GetAssetPath(string path)
    {
        return path.Substring(path.LastIndexOf("Assets"));
    }
}
