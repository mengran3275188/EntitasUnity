using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Util;
using Util.MyMath;

namespace Spatial
{
    public sealed class TiledMapParser
    {
        public enum GeometryTypeEnum : byte
        {
            WalkArea,
            ObstacleArea,
            ObstacleLine,
            WalkTriangle,
        }
        public List<TiledData> WalkAreaList
        {
            get { return walk_area_list_; }
        }
        public List<TiledData> ObstacleAreaList
        {
            get { return obstacle_area_list_; }
        }
        public List<TiledData> ObstacleLineList
        {
            get { return obstacle_line_list_; }
        }
        public List<TiledData> WalkTriangleList
        {
            get { return walk_triangle_list_; }
        }
        public bool ParseTiledData(string xml_file, float width, float height)
        {
            map_width_ = width;
            map_height_ = height;

            XmlDocument xmldoc = new XmlDocument();
            System.IO.Stream ms = null;
            try
            {
                ms = FileReaderProxy.ReadFileAsMemoryStream(xml_file);
                if (ms == null) { return false; }
                xmldoc.Load(ms);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                LogUtil.Error("config xml file {0} not find!\n{1}", xml_file, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                LogUtil.Error("parse xml file {0} error!\n{1}", xml_file, ex.Message);
                return false;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
            }

            XmlNode root = xmldoc.SelectSingleNode("map");
            XmlNodeList objgroups = root.SelectNodes("objectgroup");

            foreach (XmlNode objgroup in objgroups)
            {
                string groupName = objgroup.Attributes["name"].Value;
                foreach (XmlNode obj in objgroup.ChildNodes)
                {
                    if (groupName == "walktriangle")
                    {
                        ParseObject(obj, walk_triangle_list_, null);
                    }
                    else if (groupName == "obstacle")
                    {
                        ParseObject(obj, obstacle_area_list_, obstacle_line_list_);
                    }
                    else
                    {
                        ParseObject(obj, walk_area_list_, null);
                    }
                }
            }
            return true;
        }

        public void GenerateObstacleInfo(CellManager cellMgr)
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
            foreach (TiledData data in walk_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MapDataUtil.MarkObstacleArea(pts, cellMgr, BlockType.NOT_BLOCK);
            }
            //标记阻挡区
            foreach (TiledData data in obstacle_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MapDataUtil.MarkObstacleArea(pts, cellMgr, BlockType.STATIC_BLOCK);
            }
            //标记阻挡线
            foreach (TiledData data in obstacle_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleLine(pts, cellMgr, BlockType.STATIC_BLOCK);
            }
        }

        public void SaveObstacleInfo(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                //可行走区
                foreach (TiledData data in walk_area_list_)
                {
                    List<Vector3> pts = data.GetPoints();
                    int ct = pts.Count;
                    fs.WriteByte((byte)GeometryTypeEnum.WalkArea);
                    WriteInt(fs, ct);
                    for (int i = 0; i < ct; ++i)
                    {
                        int x = (int)(pts[i].x * 100.0f);
                        int z = (int)(pts[i].z * 100.0f);
                        WriteInt(fs, x);
                        WriteInt(fs, z);
                    }
                }
                //阻挡区
                foreach (TiledData data in obstacle_area_list_)
                {
                    List<Vector3> pts = data.GetPoints();
                    int ct = pts.Count;
                    fs.WriteByte((byte)GeometryTypeEnum.ObstacleArea);
                    WriteInt(fs, ct);
                    for (int i = 0; i < ct; ++i)
                    {
                        int x = (int)(pts[i].x * 100.0f);
                        int z = (int)(pts[i].z * 100.0f);
                        WriteInt(fs, x);
                        WriteInt(fs, z);
                    }
                }
                //阻挡线
                foreach (TiledData data in obstacle_line_list_)
                {
                    List<Vector3> pts = data.GetPoints();
                    int ct = pts.Count;
                    fs.WriteByte((byte)GeometryTypeEnum.ObstacleLine);
                    WriteInt(fs, ct);
                    for (int i = 0; i < ct; ++i)
                    {
                        int x = (int)(pts[i].x * 100.0f);
                        int z = (int)(pts[i].z * 100.0f);
                        WriteInt(fs, x);
                        WriteInt(fs, z);
                    }
                }
                //行走三角形
                foreach (TiledData data in walk_triangle_list_)
                {
                    List<Vector3> pts = data.GetPoints();
                    int ct = pts.Count;
                    fs.WriteByte((byte)GeometryTypeEnum.WalkTriangle);
                    WriteInt(fs, ct);
                    for (int i = 0; i < ct; ++i)
                    {
                        int x = (int)(pts[i].x * 100.0f);
                        int z = (int)(pts[i].z * 100.0f);
                        WriteInt(fs, x);
                        WriteInt(fs, z);
                    }
                }

                fs.Close();
            }
        }

        public void LoadObstacleInfo(string filename, KdObstacleTree tree, float scale)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                while (fs.Position < fs.Length)
                {
                    GeometryTypeEnum type = (GeometryTypeEnum)fs.ReadByte();
                    int ct = ReadInt(fs);
                    if (ct > 0)
                    {
                        List<Vector3> pts = new List<Vector3>();
                        for (int i = 0; i < ct; ++i)
                        {
                            float x = (float)ReadInt(fs);
                            float z = (float)ReadInt(fs);
                            Vector3 pt = new Vector3(x / 100.0f, 0, z / 100.0f);
                            pts.Add(pt);
                        }
                        switch (type)
                        {
                            case GeometryTypeEnum.WalkArea:
                            case GeometryTypeEnum.ObstacleArea:
                                AddObstacle(pts, true, tree, scale);
                                break;
                            case GeometryTypeEnum.ObstacleLine:
                                AddObstacle(pts, false, tree, scale);
                                break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                fs.Close();
            }
        }

        private void MarkObstacleLine(List<Vector3> pts, CellManager cellMgr, byte obstacle)
        {
            List<CellPos> pos_list = cellMgr.GetCellsCrossByPolyline(pts);
            foreach (CellPos pos in pos_list)
            {
                cellMgr.SetCellStatus(pos.row, pos.col, obstacle);
            }
        }

        private void AddObstacle(List<Vector3> pts, bool isPolygon, KdObstacleTree tree, float scale)
        {
            if (isPolygon)
            {
                List<Vector3> pts2 = new List<Vector3>();
                foreach (Vector3 pt in pts)
                {
                    pts2.Add(pt * scale);
                }
                pts2.Add(pts[0] * scale);
                tree.AddObstacle(pts2);
            }
            else
            {
                List<Vector3> pts2 = new List<Vector3>();
                foreach (Vector3 pt in pts)
                {
                    pts2.Add(pt * scale);
                }
                tree.AddObstacle(pts2);
            }
        }

        private void ParseObject(XmlNode obj, List<TiledData> polygons, List<TiledData> polylines)
        {
            TiledData td = new TiledData(map_width_, map_height_);
            if (td.CollectDataFromXml(obj))
            {
                if (td.IsPolygon)
                {
                    if (null != polygons)
                        polygons.Add(td);
                }
                else
                {
                    if (null != polylines)
                        polylines.Add(td);
                }
            }
        }

        private List<TiledData> walk_area_list_ = new List<TiledData>();
        private List<TiledData> obstacle_area_list_ = new List<TiledData>();
        private List<TiledData> obstacle_line_list_ = new List<TiledData>();
        private List<TiledData> walk_triangle_list_ = new List<TiledData>();
        private IList<TriangleNode> navmesh_triangle_list = new List<TriangleNode>();

        private float map_width_ = 0;
        private float map_height_ = 0;

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
        private static int ReadInt(FileStream fs)
        {
            int a = fs.ReadByte();
            int b = fs.ReadByte();
            int c = fs.ReadByte();
            int d = fs.ReadByte();
            int val = (d << 24) + (c << 16) + (b << 8) + a;
            return val;
        }
    }
    public class MapPatchParser
    {
        public void Load(string filename)
        {
            if (!FileReaderProxy.Exists(filename))
            {
                return;
            }
            try
            {
                using (MemoryStream ms = FileReaderProxy.ReadFileAsMemoryStream(filename))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        while (ms.Position <= ms.Length - c_RecordSize)
                        {
                            short row = br.ReadInt16();
                            short col = br.ReadInt16();
                            byte obstacle = br.ReadByte();
                            byte oldObstacle = br.ReadByte();
                            Update(row, col, obstacle, oldObstacle);
                        }
                        br.Close();
                    }
                    ms.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        public void Save(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        foreach (KeyValuePair<int, ObstacleInfo> pair in m_PatchDatas)
                        {
                            byte obstacle = pair.Value.m_UpdatedValue;
                            byte oldObstacle = pair.Value.m_OldValue;
                            if (obstacle != oldObstacle)
                            {
                                int key = pair.Key;
                                int row, col;
                                DecodeKey(key, out row, out col);
                                bw.Write((short)row);
                                bw.Write((short)col);
                                bw.Write(obstacle);
                                bw.Write(oldObstacle);
                            }
                        }
                        bw.Flush();
                        bw.Close();
                    }
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                LogUtil.Error("{0}\n{1}", e.Message, e.StackTrace);
            }
        }
        public bool Exist(int row, int col)
        {
            if (row >= 10000 || col >= 10000)
            {
                throw new Exception("Can't support huge map (row>=10000 || col>=10000)");
            }
            int key = EncodeKey(row, col);
            return m_PatchDatas.ContainsKey(key);
        }
        public void Update(int row, int col, byte obstacle)
        {
            if (row >= 10000 || col >= 10000)
            {
                throw new Exception("Can't support huge map (row>=10000 || col>=10000)");
            }
            int key = EncodeKey(row, col);
            if (m_PatchDatas.ContainsKey(key))
            {
                m_PatchDatas[key].m_UpdatedValue = obstacle;
            }
        }
        public void Update(int row, int col, byte obstacle, byte oldObstacle)
        {
            if (row >= 10000 || col >= 10000)
            {
                throw new Exception("Can't support huge map (row>=10000 || col>=10000)");
            }
            int key = EncodeKey(row, col);
            if (m_PatchDatas.ContainsKey(key))
            {
                m_PatchDatas[key].m_OldValue = oldObstacle;
                m_PatchDatas[key].m_UpdatedValue = obstacle;
            }
            else
            {
                m_PatchDatas.Add(key, new ObstacleInfo(oldObstacle, obstacle));
            }
        }
        public void VisitPatches(MyAction<int, int, byte> visitor)
        {
            foreach (KeyValuePair<int, ObstacleInfo> pair in m_PatchDatas)
            {
                byte obstacle = pair.Value.m_UpdatedValue;
                byte oldObstacle = pair.Value.m_OldValue;
                if (obstacle != oldObstacle)
                {
                    int key = pair.Key;
                    int row, col;
                    DecodeKey(key, out row, out col);
                    visitor(row, col, obstacle);
                }
            }
        }

        private int EncodeKey(int row, int col)
        {
            return row * 10000 + col;
        }
        private void DecodeKey(int key, out int row, out int col)
        {
            row = key / 10000;
            col = key % 10000;
        }
        private class ObstacleInfo
        {
            public byte m_OldValue;
            public byte m_UpdatedValue;

            public ObstacleInfo(byte oldValue, byte updatedValue)
            {
                m_OldValue = oldValue;
                m_UpdatedValue = updatedValue;
            }
        }
        private SortedDictionary<int, ObstacleInfo> m_PatchDatas = new SortedDictionary<int, ObstacleInfo>();

        private const int c_RecordSize = sizeof(short) + sizeof(short) + sizeof(byte) * 2;
    }
}
