using System;
using System.Collections.Generic;
using System.IO;
using Util.MyMath;

namespace Spatial
{
    public sealed class NavmeshMapParser
    {

        private const string NAVMESH_VERSION = "SGNAVMESH_01";
        public static IList<TriangleNode> BuildTriangleFromFile(string sFilePath, bool useFileScale, out float width, out float height)
        {
            width = 0;
            height = 0;
            string sFileName = sFilePath.Replace("obs", "navmesh");
            if (!File.Exists(sFileName))
            {
                return null;
            }
            Dictionary<int, TriangleNode> TriangleNodeMap = new Dictionary<int, TriangleNode>();
            List<TriangleNode> TriangleNodeList = new List<TriangleNode>();
            try
            {
                using (FileStream fs = new FileStream(sFileName, FileMode.Open))
                {
                    using (BinaryReader binReader = new BinaryReader(fs))
                    {
                        // 读取版本号
                        string fileVersion = new string(binReader.ReadChars(NAVMESH_VERSION.Length));
                        if (fileVersion != NAVMESH_VERSION)
                            return null;
                        width = binReader.ReadInt32();
                        height = binReader.ReadInt32();
                        float scale = binReader.ReadSingle();
                        if (!useFileScale)
                            scale = 1.0f;

                        // 读取导航三角形数量
                        int navCount = binReader.ReadInt32();
                        for (int i = 0; i < navCount; i++)
                        {
                            TriangleNode node = new TriangleNode();
                            node.Read(binReader, scale);
                            TriangleNodeList.Add(node);
                            TriangleNodeMap.Add(node.Id, node);
                        }
                        binReader.Close();
                    }
                    fs.Close();
                }

                //build Neighbors node//顺时针
                int ct = TriangleNodeList.Count;
                for (int i = 0; i < ct; ++i)
                {
                    TriangleNode node = TriangleNodeList[i];
                    TriangleNode outnode = null;
                    if (TriangleNodeMap.TryGetValue(node.NeighborsId[0], out outnode))
                    {
                        node.Neighbors[2] = outnode;
                    }
                    if (TriangleNodeMap.TryGetValue(node.NeighborsId[1], out outnode))
                    {
                        node.Neighbors[1] = outnode;
                    }
                    if (TriangleNodeMap.TryGetValue(node.NeighborsId[2], out outnode))
                    {
                        node.Neighbors[0] = outnode;
                    }
                    node.CalcGeometryInfo();
                }
            }
            catch
            {
            }
            finally
            {
            }
            if (TriangleNodeList.Count == 0)
            {
                return null;
            }
            //排序//
            TriangleNodeList.Sort((TriangleNode node1, TriangleNode node2) =>
            {
                if (node1.Position.x < node2.Position.x)
                {
                    return -1; //左值小于右值，返回-1，为升序，如果返回1，就是降序  
          }
                else if (node1.Position.x == node2.Position.x)
                {//左值等于右值，返回0 
              if (node1.Position.z == node2.Position.z)
                    {
                        return 0;
                    }
                    else if (node1.Position.z < node2.Position.z)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 1;//左值大于右值，返回1，为升序，如果返回-1，就是降序  
          }
            });

            IList<TriangleNode> ret = TriangleNodeList.ToArray();
            TriangleNodeList.Clear();
            TriangleNodeMap.Clear();
            return ret;
        }
        public bool ParseTileDataWithNavmeshUseFileScale(string file, float width, float height)
        {
            navmesh_triangle_list = BuildTriangleFromFile(file, true, out width, out height);
            return true;
        }
        public bool ParseTileDataWithNavmesh(string file, out float width, out float height)
        {
            navmesh_triangle_list = BuildTriangleFromFile(file, false, out width, out height);
            return true;
        }
        public void GenerateObstacleInfoWithNavmesh(CellManager cellMgr)
        {
            //标记所有格子为阻挡
            for (int row = 0; row < cellMgr.GetMaxRow(); row++)
            {
                for (int col = 0; col < cellMgr.GetMaxCol(); col++)
                {
                    cellMgr.SetCellStatus(row, col, BlockType.STATIC_BLOCK);
                }
            }
            //打开可行走区
            foreach(TriangleNode node in navmesh_triangle_list)
            {
                List<Vector3> pts = new List<Vector3>(node.Points);
                MapDataUtil.MarkObstacleArea(pts, cellMgr, BlockType.NOT_BLOCK);
            }
        }
        private IList<TriangleNode> navmesh_triangle_list = new List<TriangleNode>();
    }
}
