using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public class NavmeshExportWindow : EditorWindow
{

    private static Vector2 m_WinMinSize = new Vector2(315.0f, 400.0f);
    private static Rect m_WinPosition = new Rect(100.0f, 100.0f, 315.0f, 400.0f);
    public string DebugInfo;

    private Vector3 SceneTopLeft;
    private Vector3 SceneBottomRight;
    private float PhotoSize;
    private Texture2D Photo;
    private string SavePath;

    private GameObject PhotoCameraAsset;
    private GameObject PhotoCameraObj;
    private Camera PhotoCamera;

    private bool IsAdvancedConfig = false;
    private bool IsHDR = true;
    private bool IsSceneCamera = false;

    [MenuItem("Custom/Navigation/Export navmesh")]
    private static void Init()
    {
        NavmeshExportWindow window = EditorWindow.GetWindow<NavmeshExportWindow>("ScenePhoto", true, typeof(EditorWindow));
        window.position = m_WinPosition;
        window.minSize = m_WinMinSize;
        window.wantsMouseMove = true;
        window.Show();

        window.Initialize();
    }
    private void Initialize()
    {
        SceneTopLeft = Vector3.zero;
        SceneBottomRight = new Vector3(96, 0.0f, 96);
        PhotoSize = 1536.0f;
        SavePath = Path.Combine(Application.dataPath, "/Scene.jpg");
        PhotoCamera = null;
        if (PhotoCameraObj != null)
        {
            GameObject.DestroyImmediate(PhotoCameraObj);
        }
        if (PhotoCameraAsset != null)
        {
            PhotoCameraAsset = null;
        }
        if (Photo != null)
        {
            GameObject.DestroyImmediate(Photo);
            Photo = null;
        }
    }
    private void OnDestroy()
    {
        Initialize();
    }
    private void OnGUI()
    {
        SceneTopLeft = EditorGUILayout.Vector3Field("SceneTopLeft", SceneTopLeft);
        SceneBottomRight = EditorGUILayout.Vector3Field("SceneBottomRight", SceneBottomRight);
        //PhotoSize = EditorGUILayout.Vector2Field("PhotoSize", PhotoSize);

        EditorGUILayout.BeginHorizontal();
        SavePath = EditorGUILayout.TextField("SerizlizeFile:", SavePath);
        if (GUILayout.Button("Select", GUILayout.MaxWidth(50)))
        {
            SavePath = EditorUtility.SaveFolderPanel(
              "Select Path to save photo",
              SavePath,
              "Scene");
        }
        EditorGUILayout.EndHorizontal();
        IsAdvancedConfig = EditorGUILayout.Foldout(IsAdvancedConfig, "AdvancedConfig");
        if (IsAdvancedConfig)
        {
            IsHDR = EditorGUILayout.Toggle("IsHDR", IsHDR);
            IsSceneCamera = EditorGUILayout.Toggle("IsSceneCamera", IsSceneCamera);
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Take Photo", GUILayout.MaxWidth(80)))
        {
            TakePhoto();
            ExportNavmesh();
        }
        EditorGUILayout.EndHorizontal();
        Rect winRect = this.position;
        float textureFildSize = Mathf.Min(winRect.width, winRect.height);
        Photo = EditorGUILayout.ObjectField(
          Photo,
          typeof(Texture),
          false,
          GUILayout.MaxWidth(textureFildSize),
          GUILayout.MaxHeight(textureFildSize)
          ) as Texture2D;
        this.Repaint();
    }
    private void TakePhoto()
    {
        List<GameObject> airWallMeshes = new List<GameObject>();
        try
        {
            if (PhotoCameraObj == null)
            {
                if (IsSceneCamera)
                    PhotoCamera = SceneView.lastActiveSceneView.camera;
                else
                    PhotoCamera = Camera.main;
            }

            if (Photo != null)
            {
                GameObject.DestroyImmediate(Photo);
                Photo = null;
            }

            if (PhotoCamera == null)
            {
                EditorUtility.DisplayDialog(
                  "Error",
                  "Photo Camera Miss! Are you miss PhotoCamera.prefab?",
                  "OK");
                return;
            }
            Vector3 tSceneSize = SceneBottomRight - SceneTopLeft;
            Vector3 tSceneCenter = (SceneTopLeft + SceneBottomRight) / 2;

            PhotoCamera.orthographic = true;
            PhotoCamera.aspect = 1.0f;
            PhotoCamera.allowHDR = IsHDR;
            PhotoCamera.orthographicSize = Mathf.Max(tSceneSize.x, tSceneSize.y) / 2;
            PhotoCamera.farClipPlane = 10000;
            //PhotoCamera.rect = new Rect(0, 0, tSceneSize.x, tSceneSize.z);
            PhotoCamera.transform.position = tSceneCenter + new Vector3(0, 1000.0f, 0);
            PhotoCamera.transform.LookAt(tSceneCenter);

            RenderTexture currentActiveRT = RenderTexture.active;

            int photoSizeX = (int)PhotoSize;
            int photoSizeY = (int)(PhotoSize / tSceneSize.x * tSceneSize.z);
            RenderTexture tCameraRT = new RenderTexture(photoSizeX, photoSizeY, 24);
            PhotoCamera.targetTexture = tCameraRT;
            RenderTexture.active = tCameraRT;
            PhotoCamera.Render();

            Photo = new Texture2D(photoSizeX, photoSizeY, TextureFormat.RGB24, false);
            Photo.ReadPixels(new Rect(0, 0, photoSizeX, photoSizeY), 0, 0);
            Photo.Apply();

            RenderTexture.active = null;
            PhotoCamera.targetTexture = null;
            RenderTexture.active = currentActiveRT;

            byte[] bytes;
            bytes = Photo.EncodeToPNG();
            System.IO.File.WriteAllBytes(GetPhotoName(), bytes);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("ScenePhoto.TackPhoto failed.ex:" + ex.Message);
        }
        finally
        {
            foreach (GameObject child in airWallMeshes)
            {
                child.transform.parent = null;
                child.SetActive(false);
                GameObject.DestroyImmediate(child);
            }
        }
    }

    private void ExportNavmesh()
    {
        Vector3 tSceneSize = SceneBottomRight - SceneTopLeft;
        float scale = PhotoSize / tSceneSize.x;
        NavmeshExportEditor.ExportMavmesh((int)tSceneSize.x, (int)tSceneSize.z, scale, GetNavmeshName());
    }
    private string GetPhotoName()
    {
        string tPhotoTimeFormat = "yyyyMMddHHmmss";
        return string.Format("{0}/{1}_{2}.png",
          SavePath,
          "Photo",
          DateTime.Now.ToString(tPhotoTimeFormat));
    }
    private string GetNavmeshName()
    {
        string tNavmeshTimeFormat = "yyyyMMddHHmmss";
        return string.Format("{0}/{1}_{2}.navmesh",
          SavePath,
          "Photo",
          DateTime.Now.ToString(tNavmeshTimeFormat));
    }


    // nav mesh

    public class PointEx
    {
        public List<int> indices = new List<int>();
        public Vector3 Vertex;
    }
    public class Triangle
    {
        public Triangle(int id, int v0, int v1, int v2)
        {
            Id = id;
            Vertices = new int[3] { v0, v1, v2 };
            Neighbors = new int[3];
            Codes = new uint[3];

        }
        public int[] Vertices;
        public int Id = 0;
        public int[] Neighbors;
        public uint[] Codes;
    }

    public class NavmeshExportEditor
    {

        public static void ExportMavmesh(int lenth, int width, float scale, string outFile)
        {
            GenNavmeshBin(lenth, width, scale, outFile);
        }


        private static string GenNavmeshBin(int lenth, int width, float scale, string outFile)
        {
            UnityEngine.AI.NavMeshTriangulation navtri = UnityEngine.AI.NavMesh.CalculateTriangulation();

            Dictionary<int, int> indexmap = new Dictionary<int, int>();
            List<Vector3> repos = new List<Vector3>();
            for (int i = 0; i < navtri.vertices.Length; i++)
            {
                int ito = -1;
                for (int j = 0; j < repos.Count; j++)
                {
                    if (Vector3.Distance(navtri.vertices[i], repos[j]) < 0.01)
                    {
                        ito = j;
                        break;
                    }
                }
                if (ito < 0)
                {
                    indexmap[i] = repos.Count;
                    repos.Add(navtri.vertices[i]);
                }
                else
                {
                    indexmap[i] = ito;
                }
            }
            List<Triangle> triangles = new List<Triangle>();
            Dictionary<uint, List<Triangle>> triangleNeighbors = new Dictionary<uint, List<Triangle>>();
            for (int i = 0; i < navtri.indices.Length / 3; i++)
            {
                int i0 = indexmap[navtri.indices[i * 3 + 0]];
                int i1 = indexmap[navtri.indices[i * 3 + 1]];
                int i2 = indexmap[navtri.indices[i * 3 + 2]];

                bool isRepeat = false;
                foreach (Triangle triangle in triangles)
                {
                    if (triangle.Vertices[0] == i0 && triangle.Vertices[1] == i1 && triangle.Vertices[i] == i2)
                    {
                        isRepeat = true;
                        break;
                    }
                }
                if (!isRepeat)
                {

                    uint c0 = GetSideCode(i0, i1);
                    uint c1 = GetSideCode(i1, i2);
                    uint c2 = GetSideCode(i2, i0);


                    Triangle triangle = new Triangle(triangles.Count, i0, i1, i2);
                    triangle.Codes[0] = c0;
                    triangle.Codes[1] = c1;
                    triangle.Codes[2] = c2;
                    triangles.Add(triangle);

                    AddTriangleNeighbors(triangleNeighbors, c0, triangle);
                    AddTriangleNeighbors(triangleNeighbors, c1, triangle);
                    AddTriangleNeighbors(triangleNeighbors, c2, triangle);
                }
            }
            foreach (KeyValuePair<uint, List<Triangle>> pair in triangleNeighbors)
            {
                uint code = pair.Key;
                List<Triangle> tris = pair.Value;
                if (tris.Count == 2)
                {
                    SetTriangleNeighbor(code, tris[0], tris[1]);
                    SetTriangleNeighbor(code, tris[1], tris[0]);
                }
                else if (tris.Count > 2)
                {
                    Debug.LogError("repeated triangle !!!");
                }
            }

            using (System.IO.FileStream fs = new System.IO.FileStream(outFile, System.IO.FileMode.Create))
            {
                BinaryWriter writer = new BinaryWriter(fs);
                writer.Write(System.Text.Encoding.UTF8.GetBytes("SGNAVMESH_01"));
                writer.Write(lenth);
                writer.Write(width);
                writer.Write(scale);
                writer.Write(triangles.Count);

                foreach (var triangle in triangles)
                {
                    writer.Write(triangle.Id);
                    for (int i = 0; i < 3; ++i)
                    {
                        writer.Write(repos[triangle.Vertices[i]].x * 100f);
                        writer.Write(repos[triangle.Vertices[i]].z * 100f);
                    }
                    for (int i = 0; i < 3; ++i)
                    {
                        writer.Write(triangle.Neighbors[i]);
                    }

                }
                writer.Close();
                fs.Close();
            }

            string outnav = "";
            return outnav;
        }
        private static string GenNavmeshObj(int length, int width, float scale)
        {
            int style = 1;

            UnityEngine.AI.NavMeshTriangulation navtri = UnityEngine.AI.NavMesh.CalculateTriangulation();

            Dictionary<int, int> indexmap = new Dictionary<int, int>();
            List<Vector3> repos = new List<Vector3>();
            for (int i = 0; i < navtri.vertices.Length; i++)
            {
                int ito = -1;
                for (int j = 0; j < repos.Count; j++)
                {
                    if (Vector3.Distance(navtri.vertices[i], repos[j]) < 0.01)
                    {
                        ito = j;
                        break;
                    }
                }
                if (ito < 0)
                {
                    indexmap[i] = repos.Count;
                    repos.Add(navtri.vertices[i]);
                }
                else
                {
                    indexmap[i] = ito;
                }
            }

            //关系是 index 公用的三角形表示他们共同组成多边形
            //多边形之间的连接用顶点位置识别
            List<int> polylast = new List<int>();
            List<int[]> polys = new List<int[]>();
            for (int i = 0; i < navtri.indices.Length / 3; i++)
            {
                int i0 = navtri.indices[i * 3 + 0];
                int i1 = navtri.indices[i * 3 + 1];
                int i2 = navtri.indices[i * 3 + 2];

                if (polylast.Contains(i0) || polylast.Contains(i1) || polylast.Contains(i2))
                {
                    if (polylast.Contains(i0) == false)
                        polylast.Add(i0);
                    if (polylast.Contains(i1) == false)
                        polylast.Add(i1);
                    if (polylast.Contains(i2) == false)
                        polylast.Add(i2);
                }
                else
                {
                    if (polylast.Count > 0)
                    {
                        polys.Add(polylast.ToArray());
                    }
                    polylast.Clear();
                    polylast.Add(i0);
                    polylast.Add(i1);
                    polylast.Add(i2);
                }
            }
            if (polylast.Count > 0)
                polys.Add(polylast.ToArray());

            string outnav = "";
            if (style == 0)
            {
                outnav = "{\"v\":[\n";
                for (int i = 0; i < repos.Count; i++)
                {
                    if (i > 0)
                        outnav += ",\n";

                    outnav += "[" + repos[i].x + "," + repos[i].y + "," + repos[i].z + "]";
                }
                outnav += "\n],\"p\":[\n";

                for (int i = 0; i < polys.Count; i++)
                {
                    string outs = indexmap[polys[i][0]].ToString();
                    for (int j = 1; j < polys[i].Length; j++)
                    {
                        outs += "," + indexmap[polys[i][j]];
                    }

                    if (i > 0)
                        outnav += ",\n";

                    outnav += "[" + outs + "]";
                }
                outnav += "\n]}";
            }
            else if (style == 1)
            {
                outnav = "";
                for (int i = 0; i < repos.Count; i++)
                {//unity 对obj 做了 x轴 -1
                    outnav += "v " + (repos[i].x * -1) + " " + repos[i].y + " " + repos[i].z + "\r\n";
                }
                outnav += "\r\n";
                for (int i = 0; i < polys.Count; i++)
                {
                    outnav += "f";
                    //逆向
                    for (int j = polys[i].Length - 1; j >= 0; j--)
                    {
                        outnav += " " + (indexmap[polys[i][j]] + 1);
                    }




                    outnav += "\r\n";
                }
            }
            return outnav;
        }
        private static void TryAddVertex(List<PointEx> points, Vector3 vertex, int index)
        {
            foreach (PointEx point in points)
            {
                if (IsSameVector(point.Vertex, vertex))
                {
                    point.indices.Add(index);
                    return;
                }
            }
            PointEx pointEx = new PointEx();
            pointEx.Vertex = vertex;
            pointEx.indices.Add(index);
            points.Add(pointEx);
        }
        private static void AddTriangleNeighbors(Dictionary<uint, List<Triangle>> triangleNeighbors, uint code, Triangle triangle)
        {
            List<Triangle> neighbors;
            if (!triangleNeighbors.TryGetValue(code, out neighbors))
            {
                neighbors = new List<Triangle>();
                triangleNeighbors.Add(code, neighbors);
            }
            neighbors.Add(triangle);
        }
        private static void SetTriangleNeighbor(uint code, Triangle triangle, Triangle neighbor)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (triangle.Codes[i] == code)
                {
                    triangle.Neighbors[i] = neighbor.Id;
                }
            }
        }
        private static uint GetSideCode(int pt1, int pt2)
        {
            uint p1 = (uint)pt1;
            uint p2 = (uint)pt2;
            return p1 < p2 ? p1 + (p2 << 16) : p2 + (p1 << 16);
        }
        private static bool IsSameVector(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }
        private static void WriteInt(FileStream fs, int val)
        {
            byte a = (byte)(val & 0x0ff);
            byte b = (byte)((val & 0x0ff00) >> 8);
            byte c = (byte)((val & 0x0ff0000) >> 16);
            byte d = (byte)((val & 0x0ff000000) >> 24);
            fs.WriteByte(a);
            fs.WriteByte(b);
            fs.WriteByte(c);
            fs.WriteByte(d);
        }
    }
}