using System;
using System.Collections.Generic;
using Util;
using Util.MyMath;

namespace Spatial
{
    class TnsPathNode
    {
        internal TnsPathNode(TriangleNode node)
        {
            triangle_node = node;
            prev = null;
            cost = 0;
            from_start_cost = 0;
        }

        internal TriangleNode triangle_node = null;
        internal TnsPathNode prev;
        internal float cost;
        internal float from_start_cost;

        internal TnsPathNode next;
        internal int new_point_index;     //当前三角形引入的新顶点索引
        internal int edge_flags = 0x03;   //当前三角形引入的新顶点产生的路径边沿是否左边沿 
    }
    class TnsPathNodeComparer : IComparer<TnsPathNode>
    {
        public int Compare(TnsPathNode x, TnsPathNode y)
        {
            if (x.cost > y.cost)
                return -1;
            else if (x.cost == y.cost)
                return 0;
            else
                return 1;
        }
    }
    public class TriangulationNetworkFinder
    {
        public bool RecordVisitedNodes
        {
            get { return m_RecordVisitedNodes; }
            set { m_RecordVisitedNodes = value; }
        }
        public List<TriangleNode> VisitedNodes
        {
            get { return m_VisitedNodes; }
        }
        public IList<TriangleNode> TriangulationNetwork
        {
            get { return m_TriangulationNetwork; }
        }
        public KdGeometryTree<TriangleNode> KdTree
        {
            get { return m_KdTree; }
            set { m_KdTree = value; }
        }
        public float OffsetToEdge
        {
            get { return m_OffsetToEdge; }
            set { m_OffsetToEdge = value; }
        }
        public float SparseDistance
        {
            get { return m_SparseDistance; }
            set { m_SparseDistance = value; m_SparseDistanceSqr = m_SparseDistance * m_SparseDistance; }
        }
        public void Load(string filename, bool useFileScale)
        {
            float width;
            float height;
            m_Triangulation.PreTriangles.Clear();
            m_TriangulationNetwork = NavmeshMapParser.BuildTriangleFromFile(filename, useFileScale, out width, out height);
            m_KdTree.Build(m_TriangulationNetwork);
        }
        public bool CanPass(Vector3 start, Vector3 target)
        {
            bool pass = false;
            TriangleNode stNode = null, edNode = null;
            m_KdTree.Query(start, m_SparseDistance * 2, (float distSqr, TriangleNode node) =>
            {
                if (0 <= Geometry.PointInTriangle(start, node.Points[0], node.Points[1], node.Points[2]))
                {
                    stNode = node;
                    return false;
                }
                return true;
            });
            m_KdTree.Query(target, m_SparseDistance * 2, (float distSqr, TriangleNode node) =>
            {
                if (0 <= Geometry.PointInTriangle(target, node.Points[0], node.Points[1], node.Points[2]))
                {
                    edNode = node;
                    return false;
                }
                return true;
            });
            if (null != stNode && null != edNode)
            {
                if (stNode == edNode)
                    pass = true;
                else
                    pass = IsLineAcross(start, target, stNode, edNode);
            }
            return pass;
        }
        public List<Vector3> FindPath(Vector3 start, Vector3 target)
        {
            return FindPath(start, target, null, null);
        }
        public List<Vector3> FindPath(Vector3 start, Vector3 target, Vector3 addToFirst)
        {
            return FindPath(start, target, new Vector3[] { addToFirst }, null);
        }
        public List<Vector3> FindPath(Vector3 start, Vector3 target, Vector3[] addToFirst, Vector3[] addToLast)
        {
            m_OpenQueue.Clear();
            m_OpenNodes.Clear();
            m_CloseSet.Clear();

            m_VisitedNodes.Clear();

            TriangleNode stNode = null, edNode = null;
            m_CacheNodesForStart.Clear();
            m_CacheNodesForEnd.Clear();

            m_KdTree.Query(start, m_SparseDistance * 2, (float distSqr, TriangleNode node) =>
            {
                if (0 <= Geometry.PointInTriangle(start, node.Points[0], node.Points[1], node.Points[2]))
                {
                    stNode = node;
                    return false;
                }
                else
                {
                    m_CacheNodesForStart.Add(node);
                }
                return true;
            });
            m_KdTree.Query(target, m_SparseDistance * 2, (float distSqr, TriangleNode node) =>
            {
                if (0 <= Geometry.PointInTriangle(target, node.Points[0], node.Points[1], node.Points[2]))
                {
                    edNode = node;
                    return false;
                }
                else
                {
                    m_CacheNodesForEnd.Add(node);
                }
                return true;
            });

            Vector3 nearstStartPt = new Vector3();
            bool useNearstStart = false;
            if (null == stNode)
            {
                TriangleNode nearstNode = null;
                float nearstDistSqr = float.MaxValue;
                for (int ix = 0; ix < m_CacheNodesForStart.Count; ++ix)
                {
                    TriangleNode node = m_CacheNodesForStart[ix];
                    for (int i = 0; i < 3; ++i)
                    {
                        if (node.Neighbors[i] == null)
                        {
                            Vector3 pt;
                            float dSqr = Geometry.PointToLineSegmentDistanceSquare(start, node.Points[i], node.Points[(i + 1) % 3], out pt);
                            if (dSqr < nearstDistSqr)
                            {
                                nearstNode = node;
                                nearstDistSqr = dSqr;
                                nearstStartPt = pt;
                            }
                        }
                    }
                }
                if (null != nearstNode)
                {
                    stNode = nearstNode;
                    useNearstStart = true;
                }
            }
            Vector3 nearstEndPt = new Vector3();
            bool useNearstEnd = false;
            if (null == edNode)
            {
                TriangleNode nearstNode = null;
                float nearstDistSqr = float.MaxValue;
                for (int ix = 0; ix < m_CacheNodesForEnd.Count; ++ix)
                {
                    TriangleNode node = m_CacheNodesForEnd[ix];
                    for (int i = 0; i < 3; ++i)
                    {
                        if (node.Neighbors[i] == null)
                        {
                            Vector3 pt;
                            float dSqr = Geometry.PointToLineSegmentDistanceSquare(target, node.Points[i], node.Points[(i + 1) % 3], out pt);
                            if (dSqr < nearstDistSqr)
                            {
                                nearstNode = node;
                                nearstDistSqr = dSqr;
                                nearstEndPt = pt;
                            }
                        }
                    }
                }
                if (null != nearstNode)
                {
                    edNode = nearstNode;
                    useNearstEnd = true;
                }
            }
            if (null == stNode || null == edNode)
            {
                return new List<Vector3>();
            }
            m_StartNode = stNode;
            m_EndNode = edNode;
            TnsPathNode startNode = new TnsPathNode(stNode);

            startNode.cost = 0;
            startNode.from_start_cost = 0;

            m_OpenQueue.Push(startNode);
            m_OpenNodes.Add(startNode.triangle_node, startNode);

            TnsPathNode curNode = null;
            while (m_OpenQueue.Count > 0)
            {
                curNode = m_OpenQueue.Pop();
                if (m_RecordVisitedNodes)
                {
                    m_VisitedNodes.Add(curNode.triangle_node);

                    //LogSystem.Debug("cost:{0} from_start_cost:{1}", curNode.cost, curNode.from_start_cost);
                }
                m_OpenNodes.Remove(curNode.triangle_node);
                m_CloseSet.Add(curNode.triangle_node, true);

                if (curNode.triangle_node == edNode)
                {
                    break;
                }

                IdentifySuccessors(curNode);
            }

            List<Vector3> path = new List<Vector3>();
            if (curNode.triangle_node == edNode)
            {
                //建立路径邻接信息与路点三角形的穿出边信息
                TnsPathNode next = null;
                while (curNode != null)
                {
                    TnsPathNode prev = curNode.prev;
                    if (null != prev)
                    {
                        int ptIx = GetSideIndex(curNode.triangle_node, prev.triangle_node);
                        if (ptIx >= 0)
                        {
                            ptIx = (ptIx + 2) % 3;
                            curNode.new_point_index = ptIx;
                        }
                        if (null != next)
                        {
                            int sideIx = GetSideIndex(curNode.triangle_node, next.triangle_node);
                            if (sideIx == ptIx)
                            {
                                curNode.edge_flags = 0x01;
                            }
                            else
                            {
                                curNode.edge_flags = 0x02;
                            }
                        }
                        prev.next = curNode;
                    }
                    next = curNode;
                    curNode = prev;
                }
                //拐点法化简与平滑路径
                if (null != addToFirst && addToFirst.Length > 0)
                {
                    path.AddRange(addToFirst);
                }
                path.Add(start);
                Vector3 curPnt;
                Vector3 targetPnt;
                if (useNearstStart)
                {
                    path.Add(nearstStartPt);
                    curPnt = nearstStartPt;
                }
                else
                {
                    curPnt = start;
                }
                if (useNearstEnd)
                {
                    targetPnt = nearstEndPt;
                }
                else
                {
                    targetPnt = target;
                }
                curNode = startNode;
                while (null != curNode)
                {
                    TnsPathNode nextNode;
                    curPnt = GetNextWayPoint(curPnt, targetPnt, curNode, out nextNode);
                    int ct = path.Count;
                    if (Geometry.DistanceSquare(path[ct - 1], curPnt) > m_SparseDistance * m_SparseDistance)
                    {
                        path.Add(curPnt);
                    }
                    curNode = nextNode;
                }
                if (useNearstEnd)
                {
                    path.Add(nearstEndPt);
                }
                /// path.Add(target);
                if (null != addToLast && addToLast.Length > 0)
                {
                    path.AddRange(addToLast);
                }
            }
            return path;
        }

        private bool IsLineAcross(Vector3 sp, Vector3 ep, TriangleNode start, TriangleNode end)
        {
            bool across = true;
            TriangleNode current = start;
            List<int> cacheList = new List<int>();
            do
            {
                int acrossIndex = -1;
                do
                {
                    TriangleNode assit = current.Neighbors[0];
                    if (null != assit && !cacheList.Contains(assit.Id) && IntersectLine(sp, ep, assit))
                    {
                        acrossIndex = 0;
                        break;
                    }
                    assit = current.Neighbors[1];
                    if (null != assit && !cacheList.Contains(assit.Id) && IntersectLine(sp, ep, assit))
                    {
                        acrossIndex = 1;
                        break;
                    }
                    assit = current.Neighbors[2];
                    if (null != assit && !cacheList.Contains(assit.Id) && IntersectLine(sp, ep, assit))
                    {
                        acrossIndex = 2;
                        break;
                    }
                } while (false);
                if (acrossIndex < 0)
                {
                    across = false;
                    break;
                }
                else
                {
                    cacheList.Add(current.Id);
                    current = current.Neighbors[acrossIndex];
                }
            } while (current.Id != end.Id);

            return across;
        }

        private bool IntersectLine(Vector3 start, Vector3 target, TriangleNode node)
        {
            return null != node && Geometry.IntersectLine(start, target, node.Points[0], node.Points[1], node.Points[2]);
        }

        //查找下一个拐点
        private Vector3 GetNextWayPoint(Vector3 curPnt, Vector3 target, TnsPathNode curNode, out TnsPathNode nextNode)
        {
            TnsPathNode lastNode1, lastNode2, next;
            Vector3 lpt1, lpt2;
            int ix = curNode.new_point_index;
            if (curNode.edge_flags == 0x02)
            {
                lpt1 = curNode.triangle_node.Points[(ix + 2) % 3];
                lpt2 = curNode.triangle_node.Points[ix];
            }
            else if (curNode.edge_flags == 0x01)
            {
                lpt1 = curNode.triangle_node.Points[ix];
                lpt2 = curNode.triangle_node.Points[(ix + 1) % 3];
            }
            else
            {
                if (null != curNode.next)
                {//起点
                    int sideIx = GetSideIndex(curNode.triangle_node, curNode.next.triangle_node);
                    lpt1 = curNode.triangle_node.Points[sideIx];
                    lpt2 = curNode.triangle_node.Points[(sideIx + 1) % 3];
                }
                else if (null != curNode.prev)
                {//终点
                    int sideIx = GetSideIndex(curNode.triangle_node, curNode.prev.triangle_node);
                    lpt1 = curNode.triangle_node.Points[sideIx];
                    lpt2 = curNode.triangle_node.Points[(sideIx + 1) % 3];
                }
                else
                {//起点也是终点
                    lpt1 = curNode.triangle_node.Points[ix];
                    lpt2 = curNode.triangle_node.Points[(ix + 1) % 3];
                }
            }
            Vector3 lastPnt1 = lpt1 * (1 - m_OffsetToEdge) + lpt2 * m_OffsetToEdge;
            Vector3 lastPnt2 = lpt2 * (1 - m_OffsetToEdge) + lpt1 * m_OffsetToEdge;
            lastNode1 = lastNode2 = curNode;
            curNode = curNode.next;
            while (null != curNode)
            {
                Vector3 testPnt1, testPnt2;
                next = curNode.next;
                ix = curNode.new_point_index;
                if ((curNode.edge_flags & 0x02) != 0)
                {//左边是边沿
                    if (curNode.triangle_node == m_EndNode)
                    {
                        testPnt1 = target;
                        testPnt2 = target;
                    }
                    else
                    {
                        Vector3 tpt1 = curNode.triangle_node.Points[(ix + 2) % 3];
                        Vector3 tpt2 = curNode.triangle_node.Points[ix];
                        testPnt1 = tpt1 * (1 - m_OffsetToEdge) + tpt2 * m_OffsetToEdge;
                        testPnt2 = tpt2 * (1 - m_OffsetToEdge) + tpt1 * m_OffsetToEdge;
                    }

                    if (Geometry.PointIsLeftOn(testPnt2, lastPnt2, curPnt))
                    {
                        if (!Geometry.PointIsLeftOn(curPnt, lastPnt1, testPnt2))
                        {
                            nextNode = lastNode1.next;
                            return lastPnt1;
                        }
                        lastPnt2 = testPnt2;
                        lastNode2 = curNode;
                    }
                }
                if ((curNode.edge_flags & 0x01) != 0)
                {//右边是边沿
                    if (curNode.triangle_node == m_EndNode)
                    {
                        testPnt1 = target;
                        testPnt2 = target;
                    }
                    else
                    {
                        Vector3 tpt1 = curNode.triangle_node.Points[ix];
                        Vector3 tpt2 = curNode.triangle_node.Points[(ix + 1) % 3];
                        testPnt1 = tpt1 * (1 - m_OffsetToEdge) + tpt2 * m_OffsetToEdge;
                        testPnt2 = tpt2 * (1 - m_OffsetToEdge) + tpt1 * m_OffsetToEdge;
                    }

                    if (Geometry.PointIsLeftOn(curPnt, lastPnt1, testPnt1))
                    {
                        if (!Geometry.PointIsLeftOn(curPnt, testPnt1, lastPnt2))
                        {
                            nextNode = lastNode2.next;
                            return lastPnt2;
                        }
                        lastPnt1 = testPnt1;
                        lastNode1 = curNode;
                    }
                }

                curNode = next;
            }

            nextNode = null;
            return target;
        }

        private void IdentifySuccessors(TnsPathNode node)
        {
            FindNeighbors(node);

            int ct = m_Neighbors.Count;
            for (int i = 0; i < ct; ++i)
            {
                TriangleNode triNode = m_Neighbors[i];
                if (m_CloseSet.ContainsKey(triNode))
                    continue;
                Vector3 pos1 = node.triangle_node.Position;
                Vector3 pos2 = triNode.Position;
                float d = Geometry.Distance(pos1, pos2);
                float ng = node.from_start_cost + d;

                bool isOpened = true;
                TnsPathNode nextNode;
                if (!m_OpenNodes.TryGetValue(triNode, out nextNode))
                {
                    nextNode = new TnsPathNode(triNode);
                    isOpened = false;
                }
                if (!isOpened || ng < nextNode.from_start_cost)
                {
                    nextNode.from_start_cost = ng;
                    nextNode.cost = nextNode.from_start_cost + CalcHeuristic(triNode);
                    nextNode.prev = node;

                    if (!isOpened)
                    {
                        m_OpenQueue.Push(nextNode);
                        m_OpenNodes.Add(triNode, nextNode);
                    }
                    else
                    {
                        int index = m_OpenQueue.IndexOf(nextNode);
                        m_OpenQueue.Update(index, nextNode);
                    }
                }
            }
        }

        private int GetSideIndex(TriangleNode cur, TriangleNode other)
        {
            int ret = -1;
            for (int ix = 0; ix < 3; ++ix)
            {
                if (cur.Neighbors[ix] == other)
                {
                    ret = ix;
                    break;
                }
            }
            return ret;
        }

        private void FindNeighbors(TnsPathNode node)
        {
            m_Neighbors.Clear();
            TriangleNode triNode = node.triangle_node;
            for (int i = 0; i < 3; ++i)
            {
                TriangleNode neighbor = triNode.Neighbors[i];
                if (null != neighbor)
                {
                    m_Neighbors.Add(neighbor);
                }
            }
        }

        private float CalcHeuristic(TriangleNode triNode)
        {
            double xDelta = Math.Abs(m_EndNode.Position.x - triNode.Position.x);
            double yDelta = Math.Abs(m_EndNode.Position.z - triNode.Position.z);
            return (float)Math.Sqrt(xDelta * xDelta + yDelta * yDelta);

            //return Math.Abs(m_EndNode.Position.X - triNode.Position.X) + Math.Abs(m_EndNode.Position.Z - triNode.Position.Z);
        }

        private TriangleNode m_StartNode;
        private TriangleNode m_EndNode;
        private List<TriangleNode> m_Neighbors = new List<TriangleNode>();

        private Heap<TnsPathNode> m_OpenQueue = new Heap<TnsPathNode>(new TnsPathNodeComparer());
        private Dictionary<TriangleNode, TnsPathNode> m_OpenNodes = new Dictionary<TriangleNode, TnsPathNode>();
        private Dictionary<TriangleNode, bool> m_CloseSet = new Dictionary<TriangleNode, bool>();

        private List<TriangleNode> m_VisitedNodes = new List<TriangleNode>();
        private bool m_RecordVisitedNodes = false;
        private float m_OffsetToEdge = 0.1f;
        private float m_SparseDistance = 1.0f;
        private float m_SparseDistanceSqr = 1.0f;

        private List<TriangleNode> m_CacheNodesForStart = new List<TriangleNode>();
        private List<TriangleNode> m_CacheNodesForEnd = new List<TriangleNode>();

        private IList<TriangleNode> m_TriangulationNetwork = null;
        private KdGeometryTree<TriangleNode> m_KdTree = new KdGeometryTree<TriangleNode>();
        private Triangulation m_Triangulation = new Triangulation();
    }
}
