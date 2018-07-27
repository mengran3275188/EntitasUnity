using System;
using System.Collections.Generic;
using System.IO;
using Util.MyMath;
using Util;

namespace Spatial
{
    public sealed class SpatialSystem : ISpatialSystem
    {
        public SpatialSystem()
        {
            current_spatial_id_ = spatial_system_id_++;
        }

        public void Init(string map_file)
        {
            string path_file = Path.ChangeExtension(map_file, "path");
            Init(map_file, path_file);
        }

        public void Init(string map_file, string path_file)
        {
            map_file_ = HomePath.Instance.GetAbsolutePath(map_file);
            string _path_file = HomePath.Instance.GetAbsolutePath(path_file);
            if (!cell_manager_.Init(map_file_))
            {
                cell_manager_.Init(1024, 1024, 0.5f);
                LogUtil.Error("Init SpatialSystem from map file failed: {0}", map_file_);
            }
            jump_point_finder_.Init(cell_manager_);
            if (File.Exists(_path_file))
            {
                jump_point_finder_.LoadPreprocessData(_path_file);
            }
        }

        public void LoadPatch(string patch_file)
        {
            string filePath = HomePath.Instance.GetAbsolutePath(patch_file);
            if (File.Exists(filePath))
            {
                MapPatchParser patchParser = new MapPatchParser();
                patchParser.Load(filePath);
                patchParser.VisitPatches((int row, int col, byte obstacle) =>
                {
                    SetCellStatus(row, col, obstacle);
                });
            }
        }

        public void LoadObstacle(string file, bool useFileScale)
        {
            string filePath = HomePath.Instance.GetAbsolutePath(file);
            if (File.Exists(filePath))
            {
                //TiledMapParser mapParser = new TiledMapParser();
                //mapParser.LoadObstacleInfo(filePath, m_KdObstacleTree, scale);
                //m_KdObstacleTree.Build();
            }
            else
            {
                LogUtil.Error("Can't find obstacle file: {0}", filePath);
            }
            triangulation_network_finder_.Load(filePath, useFileScale);
        }

        public void Reset()
        {
            space_obj_collection_dict_.Clear();
            delete_obj_buffer_.Clear();
            add_obj_buffer_.Clear();
            cell_manager_.Reset();
            m_KdObjectTree.Clear();
            m_KdObstacleTree.Clear();
        }

        public void Tick()
        {
            long t1 = TimeUtility.Instance.GetElapsedTimeUs();
            // remove objects in delete buffer
            foreach (KeyValuePair<uint, ISpaceObject> pair in delete_obj_buffer_)
            {
                space_obj_collection_dict_.Remove(pair.Key);
            }
            delete_obj_buffer_.Clear();

            // add new obj
            foreach (KeyValuePair<uint, ISpaceObject> pair in add_obj_buffer_)
            {
                if (!space_obj_collection_dict_.ContainsKey(pair.Key))
                {
                    space_obj_collection_dict_.Add(pair.Key, pair.Value);
                }
            }
            add_obj_buffer_.Clear();

            long t2 = TimeUtility.Instance.GetElapsedTimeUs();

            IList<ISpaceObject> obj_list = null;
            if (space_obj_collection_dict_.Count > 0)
            {
                ISpaceObject[] temp = new ISpaceObject[space_obj_collection_dict_.Count];
                space_obj_collection_dict_.Values.CopyTo(temp, 0);
                obj_list = temp;
            }
            else
            {
                obj_list = new List<ISpaceObject>();
            }

            long t3 = TimeUtility.Instance.GetElapsedTimeUs();

            //构造空间索引
            if (obj_list.Count > 0)
            {
                m_KdObjectTree.Build(obj_list);
            }
            else
            {
                m_KdObjectTree.Clear();
            }

            long t4 = TimeUtility.Instance.GetElapsedTimeUs();

            bool isCountTick = false;
            long curTime = TimeUtility.Instance.GetLocalMilliseconds();
            if (m_LastCountTime + c_CountInterval < curTime)
            {
                m_LastCountTime = curTime;
                isCountTick = true;
            }
            int userCt = 0;
            int npcCt = 0;
            foreach (ISpaceObject hiter in obj_list)
            {
                if (isCountTick)
                {
                    switch (hiter.GetObjType())
                    {
                        case SpatialObjType.kUser:
                            ++userCt;
                            break;
                        case SpatialObjType.kNPC:
                            ++npcCt;
                            break;
                    }
                }
                /*m_KdTree.Query(hiter, (float)hiter.GetRadius(), (float distSqr, KdTreeObject obj) => {
                  ISpaceObject dest = obj.SpaceObject;
                  if (null != dest && IsPassableCollide(hiter, dest)) {
                    CheckCollide(hiter, dest);
                  }
                });*/
            }

            long t5 = TimeUtility.Instance.GetElapsedTimeUs();

            if (isCountTick)
            {
                LogUtil.Info("SpatialSystem object count:{0} user:{1} npc:{2} [time] add/remove {3} copy {4} kdtree {5} stat {6}", obj_list.Count, userCt, npcCt, t2 - t1, t3 - t2, t4 - t3, t5 - t4);
            }
            else if (t5 - t1 > 20000)
            {
                LogUtil.Info("SpatialSystem object count:{0} [time] add/remove {1} copy {2} kdtree {3} stat {4}", obj_list.Count, t2 - t1, t3 - t2, t4 - t3, t5 - t4);
            }
        }

        public float Width
        {
            get
            {
                return cell_manager_.GetMapWidth();
            }
        }
        public float Height
        {
            get
            {
                return cell_manager_.GetMapHeight();
            }
        }

        /** 
         * 增加物体
         * @param obj 增加的物体
         * @return 成功返回SUCCESS，失败返回其它错误码
         */
        public RetCode AddObj(ISpaceObject obj)
        {
            if (obj == null)
            {
                return RetCode.NULL_POINTER;
            }
            if (add_obj_buffer_.ContainsKey(CalcSpaceObjectKey(obj)))
            {
                return RetCode.OBJECT_EXIST;
            }
            add_obj_buffer_.Add(CalcSpaceObjectKey(obj), obj);
            return RetCode.SUCCESS;
        }

        /** 
         * 删除物体obj
         * @param objid 删除的物体
         * @return 成功返回true, 失败返回false
         */
        public bool RemoveObj(ISpaceObject obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (delete_obj_buffer_.ContainsKey(CalcSpaceObjectKey(obj)))
            {
                return false;
            }
            delete_obj_buffer_.Add(CalcSpaceObjectKey(obj), obj);
            return true;
        }

        public List<ISpaceObject> GetObjectInCircle(Vector3 center, float radius)
        {
            return GetObjectInCircle(center, radius, (distSqr, obj) => true);
        }
        public List<ISpaceObject> GetObjectInCircle(Vector3 center, float radius, MyFunc<float, ISpaceObject, bool> pred)
        {
            List<ISpaceObject> objects_in_circle = new List<ISpaceObject>();
            VisitObjectInCircle(center, radius, (float distSqr, ISpaceObject obj) =>
            {
                if (pred(distSqr, obj))
                {
                    objects_in_circle.Add(obj);
                }
            });
            return objects_in_circle;
        }
        public void VisitObjectInCircle(Vector3 center, float radius, MyAction<float, ISpaceObject> visitor)
        {
            m_KdObjectTree.Query(center, (float)radius, (float distSqr, KdTreeObject kdObj) =>
            {
                ISpaceObject obj = kdObj.SpaceObject;
                if (null != obj)
                {
                    visitor(distSqr, obj);
                }
            });
        }
        public void VisitObjectInCircle(Vector3 center, float radius, MyFunc<float, ISpaceObject, bool> visitor)
        {
            m_KdObjectTree.Query(center, (float)radius, (float distSqr, KdTreeObject kdObj) =>
            {
                ISpaceObject obj = kdObj.SpaceObject;
                if (null != obj)
                {
                    return visitor(distSqr, obj);
                }
                return true;
            });
        }

        public List<ISpaceObject> GetObjectInPolygon(IList<Vector3> polygon)
        {
            return GetObjectInPolygon(polygon, (distSqr, obj) => true);
        }
        public List<ISpaceObject> GetObjectInPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> pred)
        {
            List<ISpaceObject> objects_in_polygon = new List<ISpaceObject>();
            VisitObjectInPolygon(polygon, (float distSqr, ISpaceObject obj) =>
            {
                if (pred(distSqr, obj))
                {
                    objects_in_polygon.Add(obj);
                }
            });
            return objects_in_polygon;
        }
        public void VisitObjectInPolygon(IList<Vector3> polygon, MyAction<float, ISpaceObject> visitor)
        {
            Vector3 centroid;
            float radius = Geometry.CalcPolygonCentroidAndRadius(polygon, 0, polygon.Count, out centroid);

            if (radius > Geometry.c_FloatPrecision)
            {
                m_KdObjectTree.Query(centroid, radius, (float distSqr, KdTreeObject kdObj) =>
                {
                    ISpaceObject obj = kdObj.SpaceObject;
                    if (null != obj)
                    {
                        if (Geometry.PointInPolygon(kdObj.Position, polygon, 0, polygon.Count) >= 0)
                        {
                            visitor(distSqr, obj);
                        }
                    }
                });
            }
        }
        public void VisitObjectInPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> visitor)
        {
            Vector3 centroid;
            float radius = Geometry.CalcPolygonCentroidAndRadius(polygon, 0, polygon.Count, out centroid);

            if (radius > Geometry.c_FloatPrecision)
            {
                m_KdObjectTree.Query(centroid, radius, (float distSqr, KdTreeObject kdObj) =>
                {
                    ISpaceObject obj = kdObj.SpaceObject;
                    if (null != obj)
                    {
                        if (Geometry.PointInPolygon(kdObj.Position, polygon, 0, polygon.Count) >= 0)
                        {
                            return visitor(distSqr, obj);
                        }
                    }
                    return true;
                });
            }
        }

        public void VisitObjectOutPolygon(IList<Vector3> polygon, MyAction<float, ISpaceObject> visitor)
        {
            Vector3 centroid;
            float radius = Geometry.CalcPolygonCentroidAndRadius(polygon, 0, polygon.Count, out centroid);

            if (radius > Geometry.c_FloatPrecision)
            {
                m_KdObjectTree.Query(centroid, radius, (float distSqr, KdTreeObject kdObj) =>
                {
                    ISpaceObject obj = kdObj.SpaceObject;
                    if (null != obj)
                    {
                        if (Geometry.PointInPolygon(kdObj.Position, polygon, 0, polygon.Count) < 0)
                        {
                            visitor(distSqr, obj);
                        }
                    }
                });
            }
        }
        public void VisitObjectOutPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> visitor)
        {
            Vector3 centroid;
            float radius = Geometry.CalcPolygonCentroidAndRadius(polygon, 0, polygon.Count, out centroid);

            if (radius > Geometry.c_FloatPrecision)
            {
                m_KdObjectTree.Query(centroid, radius, (float distSqr, KdTreeObject kdObj) =>
                {
                    ISpaceObject obj = kdObj.SpaceObject;
                    if (null != obj)
                    {
                        if (Geometry.PointInPolygon(kdObj.Position, polygon, 0, polygon.Count) < 0)
                        {
                            return visitor(distSqr, obj);
                        }
                    }
                    return true;
                });
            }
        }

        public bool CanPass(ISpaceObject obj, Vector3 to)
        {
            return CanPass(obj.GetPosition(), to);
        }
        public bool CanPass(Vector3 curPos, Vector3 to)
        {
            CellPos cur_cell = new CellPos();
            cell_manager_.GetCell(curPos, out cur_cell.row, out cur_cell.col);
            CellPos to_cell = new CellPos();
            cell_manager_.GetCell(to, out to_cell.row, out to_cell.col);
            if (cur_cell.row == to_cell.row && cur_cell.col == to_cell.col)
            {
                return true;
            }
            byte curStatus = cell_manager_.GetCellStatus(cur_cell.row, cur_cell.col);
            byte block = BlockType.GetBlockType(curStatus);
            if (block != BlockType.NOT_BLOCK)
            {
                return false;
            }
            byte status = cell_manager_.GetCellStatus(to_cell.row, to_cell.col);
            block = BlockType.GetBlockType(status);
            if (block != BlockType.NOT_BLOCK)
            {
                //LogSystem.Debug("CanPass ({0},{1})->({2},{3}), target is blocked {4}", cur_cell.row, cur_cell.col, to_cell.row, to_cell.col, block);
                return false;
            }
            if (Math.Abs(cur_cell.row - to_cell.row) >= 1 || Math.Abs(cur_cell.col - to_cell.col) >= 1)
            {
                Vector3 from = cell_manager_.GetCellCenter(cur_cell.row, cur_cell.col);
                to = cell_manager_.GetCellCenter(to_cell.row, to_cell.col);

                bool ret = true;
                cell_manager_.VisitCellsCrossByLine(from, to, (row, col) =>
                {
                    status = cell_manager_.cells_arr_[row, col];
                    block = BlockType.GetBlockType(status);
                    if (block != BlockType.NOT_BLOCK)
                    {
                        ret = false;
                        //LogSystem.Debug("CanPass ({0},{1})->({2},{3}), ({4},{5}) is blocked {6}", cur_cell.row, cur_cell.col, to_cell.row, to_cell.col, row, col, block);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                });
                return ret;
            }
            return true;
        }

        public List<Vector3> FindPath(Vector3 from, Vector3 to)
        {
            List<Vector3> result;
            CellPos start = new CellPos();
            CellPos end = new CellPos();
            CellPos start0 = new CellPos();
            CellPos end0 = new CellPos();
            if (cell_manager_.GetCell(from, out start0.row, out start0.col) && cell_manager_.GetCell(to, out end0.row, out end0.col))
            {
                /*//
                start = cell_manager_.GetNearstWalkableCell(from);
                end = cell_manager_.GetNearstWalkableCell(to);
                if (start.row >= 0 && start.col >= 0 && end.row >= 0 && end.col >= 0) {
                  List<Vector3> addToFirst = new List<Vector3>();
                  List<Vector3> addToLast = new List<Vector3>();
                  if (start0.row != start.row || start0.col != start.col) {
                    addToFirst.Add(from);
                  }
                  if (end0.row != end.row || end0.col != end.col) {
                    addToLast.Add(to);
                  }
                  Vector3 st = cell_manager_.GetCellCenter(start.row, start.col);
                  Vector3 ed = cell_manager_.GetCellCenter(end.row, end.col);
                  result = triangulation_network_finder_.FindPath(st, ed, addToFirst.ToArray(), addToLast.ToArray());
                } else {
                  result = new List<Vector3>();
                }
                //*/
                result = triangulation_network_finder_.FindPath(from, to);
            }
            else
            {
                result = new List<Vector3>();
            }
            return result;
        }
        public List<Vector3> FindPathWithCellMap(Vector3 from, Vector3 to)
        {
            List<Vector3> result;
            CellPos start = new CellPos();
            CellPos end = new CellPos();
            CellPos start0 = new CellPos();
            CellPos end0 = new CellPos();
            if (cell_manager_.GetCell(from, out start0.row, out start0.col) && cell_manager_.GetCell(to, out end0.row, out end0.col))
            {
                start = cell_manager_.GetNearstWalkableCell(from);
                end = cell_manager_.GetNearstWalkableCell(to);
                if (start.row >= 0 && start.col >= 0 && end.row >= 0 && end.col >= 0)
                {
                    List<Vector3> addToFirst = new List<Vector3>();
                    List<Vector3> addToLast = new List<Vector3>();
                    if (start0.row != start.row || start0.col != start.col)
                    {
                        addToFirst.Add(from);
                    }
                    if (end0.row != end.row || end0.col != end.col)
                    {
                        addToLast.Add(to);
                    }
                    result = jump_point_finder_.FindPath(start, end, addToFirst.ToArray(), addToLast.ToArray());
                }
                else
                {
                    result = new List<Vector3>();
                }
            }
            else
            {
                result = new List<Vector3>();
            }
            return result;
        }
        public Vector3 ComputeVelocity(ISpaceObject obj, Vector3 prefDir, float stepTime, float maxSpeed, float neighborDist, bool isUsingAvoidanceVelocity)
        {
            Vector3 newVelocity = m_RvoAlgorithm.ComputeNewVelocity(obj, prefDir, stepTime, m_KdObjectTree, m_KdObstacleTree, maxSpeed, neighborDist, isUsingAvoidanceVelocity);
            return newVelocity;
        }

        public bool CanPass(int row, int col)
        {
            return cell_manager_.CanPass(row, col);
        }
        public bool CanPass(Vector3 targetPos)
        {
            return cell_manager_.CanPass(targetPos);
        }
        public bool GetNearstWalkablePosition(Vector3 from, ref Vector3 newPos)
        {
            return GetNearstWalkablePosition(from, ref newPos, 32);
        }
        public bool GetNearstWalkablePosition(Vector3 from, ref Vector3 newPos, int maxCells)
        {
            bool ret = false;
            CellPos pos = cell_manager_.GetNearstWalkableCell(from, maxCells);
            if (pos.row >= 0 && pos.col >= 0)
            {
                newPos = cell_manager_.GetCellCenter(pos.row, pos.col);
                ret = true;
            }
            return ret;
        }
        public bool AdjustToNearstWalkablePosition(Vector3 from, ref Vector3 newPos, int maxCells)
        {
            bool ret = false;
            CellPos start = new CellPos(-1, -1);
            if (cell_manager_.GetCell(from, out start.row, out start.col))
            {
                CellPos pos = cell_manager_.GetNearstWalkableCell(start, maxCells);
                if (pos.row >= 0 && pos.col >= 0 && (pos.row != start.row || pos.col != start.col))
                {
                    newPos = cell_manager_.GetCellCenter(pos.row, pos.col);
                    ret = true;
                }
            }
            return ret;
        }
        public bool GetCell(Vector3 pos, out int row, out int col)
        {
            return cell_manager_.GetCell(pos, out row, out col);
        }
        public Vector3 GetCellCenter(int row, int col)
        {
            return cell_manager_.GetCellCenter(row, col);
        }
        public bool IsCellValid(int row, int col)
        {
            return cell_manager_.IsCellValid(row, col);
        }
        public byte GetCellStatus(int row, int col)
        {
            return cell_manager_.GetCellStatus(row, col);
        }
        public void SetCellStatus(int row, int col, byte status)
        {
            cell_manager_.SetCellStatus(row, col, status);
        }
        public List<CellPos> GetCellsInPolygon(IList<Vector3> pts)
        {
            return cell_manager_.GetCellsInPolygon(pts);
        }
        public void VisitCellsInPolygon(IList<Vector3> pts, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsInPolygon(pts, visitor);
        }
        public List<CellPos> GetCellsCrossByLine(Vector3 start, Vector3 end)
        {
            return cell_manager_.GetCellsCrossByLine(start, end);
        }
        public void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsCrossByLine(start, end, visitor);
        }
        public void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyFunc<int, int, bool> visitor)
        {
            cell_manager_.VisitCellsCrossByLine(start, end, visitor);
        }
        public List<CellPos> GetCellsCrossByPolyline(IList<Vector3> pts)
        {
            return cell_manager_.GetCellsCrossByPolyline(pts);
        }
        public void VisitCellsCrossByPolyline(IList<Vector3> pts, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsCrossByPolyline(pts, visitor);
        }
        public List<CellPos> GetCellsCrossByPolygon(IList<Vector3> pts)
        {
            return cell_manager_.GetCellsCrossByPolygon(pts);
        }
        public void VisitCellsCrossByPolygon(IList<Vector3> pts, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsCrossByPolygon(pts, visitor);
        }
        public List<CellPos> GetCellsInCircle(Vector3 center, float radius)
        {
            return cell_manager_.GetCellsInCircle(center, radius);
        }
        public void VisitCellsInCircle(Vector3 center, float radius, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsInCircle(center, radius, visitor);
        }
        public List<CellPos> GetCellsCrossByCircle(Vector3 center, float radius)
        {
            return cell_manager_.GetCellsCrossByCircle(center, radius);
        }
        public void VisitCellsCrossByCircle(Vector3 center, float radius, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsCrossByCircle(center, radius, visitor);
        }

        public CellManager GetCellManager() { return cell_manager_; }
        public KdObjectTree KdObjectTree
        {
            get { return m_KdObjectTree; }
        }
        public KdObstacleTree KdObstacleTree
        {
            get { return m_KdObstacleTree; }
        }
        public JumpPointFinder JumpPointFinder
        {
            get { return jump_point_finder_; }
        }
        public TriangulationNetworkFinder TriangulationNetworkFinder
        {
            get { return triangulation_network_finder_; }
        }

        // private attributes-----------------------------------------------------
        private Dictionary<uint, ISpaceObject> space_obj_collection_dict_ = new Dictionary<uint, ISpaceObject>();
        private Dictionary<uint, ISpaceObject> delete_obj_buffer_ = new Dictionary<uint, ISpaceObject>();
        private Dictionary<uint, ISpaceObject> add_obj_buffer_ = new Dictionary<uint, ISpaceObject>();
        private CellManager cell_manager_ = new CellManager();
        private JumpPointFinder jump_point_finder_ = new JumpPointFinder();
        private TriangulationNetworkFinder triangulation_network_finder_ = new TriangulationNetworkFinder();
        private string map_file_ = "";
        private int current_spatial_id_ = 0;
        private KdObjectTree m_KdObjectTree = new KdObjectTree();
        private KdObstacleTree m_KdObstacleTree = new KdObstacleTree();
        private RvoAlgorithm m_RvoAlgorithm = new RvoAlgorithm();
        private static int spatial_system_id_ = 0;

        private const long c_CountInterval = 10000;
        private long m_LastCountTime = 0;

        private uint CalcSpaceObjectKey(ISpaceObject obj)
        {
            //注意：这里假设obj.GetID()<0x10000000，应该不会有超过这个的id
            return ((uint)obj.GetObjType() << 24) | obj.GetID();
        }
    }
} // namespace DashFire
