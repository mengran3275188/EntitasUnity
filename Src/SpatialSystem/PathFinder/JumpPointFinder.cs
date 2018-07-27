using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Util;
using Util.MyMath;

namespace Spatial
{
  class CellPosComparer : IEqualityComparer<CellPos>
  {
    public bool Equals(CellPos x, CellPos y)
    {
      if (null != x && null != y) {
        return x.row == y.row && x.col == y.col;
      } else {
        return x == y;
      }
    }

    public int GetHashCode(CellPos obj)
    {
      if (null == obj)
        return -1;
      return (obj.row + 1) * 1000 + obj.col + 1;
    }
  }
  class JpsPathNode
  {
    internal JpsPathNode(CellPos cellpos)
    {
      cell = cellpos;
      prev = null;
      cost = 0;
      from_start_cost = 0;
    }

    internal CellPos cell = new CellPos();
    internal JpsPathNode prev;
    internal float cost;
    internal float from_start_cost;

    internal JpsPathNode next;
  }
  class JpsPathNodeComparer : IComparer<JpsPathNode>
  {
    public int Compare(JpsPathNode x, JpsPathNode y)
    {
      if (x.cost > y.cost)
        return -1;
      else if (x.cost == y.cost)
        return 0;
      else
        return 1;
    }
  }
  public sealed class JumpPointFinder
  {
    public void Init(CellManager cellMgr)
    {
      m_CellMgr = cellMgr;
      m_Map = cellMgr.cells_arr_;
      m_MaxRows = cellMgr.GetMaxRow();
      m_MaxCols = cellMgr.GetMaxCol();

      m_PreprocessData = null;
    }
    public void PreprocessMap(string filename)
    {
      m_MinCell.row = 0;
      m_MinCell.col = 0;
      m_MaxCell.row = m_MaxRows - 1;
      m_MaxCell.col = m_MaxCols - 1;
      //预处理前先把阻挡扩大一圈
      /*
      for (int x = 0; x < m_MaxRows; ++x) {
        for (int y = 0; y < m_MaxCols; ++y) {
          bool needSet = false;
          List<CellPos> neighbors = CellManager.GetCellAdjacent(new CellPos(x, y), 0, 0, m_MaxRows - 1, m_MaxCols - 1);
          int ct = neighbors.Count;
          for (int i = 0; i < ct; ++i) {
            if (m_Map[neighbors[i].row, neighbors[i].col] == BlockType.STATIC_BLOCK) {
              needSet = true;
              break;
            }
          }
          if (needSet)
            m_Map[x, y] = BlockType.DYNAMIC_BLOCK;
        }
      }
      */
      //预处理
      m_PreprocessData = new byte[m_MaxRows * m_MaxCols * 16];
      for (int x = 0; x < m_MaxRows; ++x) {
        for (int y = 0; y < m_MaxCols; ++y) {
          int index = GetIndex(x, y);
          int ny = y - 1;
          int nx = x;
          CellPos pos = Jump(nx, ny, x, y);
          int dirIndex = GetDirIndex(nx, ny, x, y);
          m_PreprocessData[index * 16 + dirIndex * 2] = (byte)(pos.row + 1);
          m_PreprocessData[index * 16 + dirIndex * 2 + 1] = (byte)(pos.col + 1);

          nx = x - 1;
          pos = Jump(nx, ny, x, y);
          dirIndex = GetDirIndex(nx, ny, x, y);
          m_PreprocessData[index * 16 + dirIndex * 2] = (byte)(pos.row + 1);
          m_PreprocessData[index * 16 + dirIndex * 2 + 1] = (byte)(pos.col + 1);

          nx = x + 1;
          pos = Jump(nx, ny, x, y);
          dirIndex = GetDirIndex(nx, ny, x, y);
          m_PreprocessData[index * 16 + dirIndex * 2] = (byte)(pos.row + 1);
          m_PreprocessData[index * 16 + dirIndex * 2 + 1] = (byte)(pos.col + 1);

          ny = y;

          nx = x - 1;
          pos = Jump(nx, ny, x, y);
          dirIndex = GetDirIndex(nx, ny, x, y);
          m_PreprocessData[index * 16 + dirIndex * 2] = (byte)(pos.row + 1);
          m_PreprocessData[index * 16 + dirIndex * 2 + 1] = (byte)(pos.col + 1);

          nx = x + 1;
          pos = Jump(nx, ny, x, y);
          dirIndex = GetDirIndex(nx, ny, x, y);
          m_PreprocessData[index * 16 + dirIndex * 2] = (byte)(pos.row + 1);
          m_PreprocessData[index * 16 + dirIndex * 2 + 1] = (byte)(pos.col + 1);

          ny = y + 1;

          nx = x - 1;
          pos = Jump(nx, ny, x, y);
          dirIndex = GetDirIndex(nx, ny, x, y);
          m_PreprocessData[index * 16 + dirIndex * 2] = (byte)(pos.row + 1);
          m_PreprocessData[index * 16 + dirIndex * 2 + 1] = (byte)(pos.col + 1);

          nx = x;
          pos = Jump(nx, ny, x, y);
          dirIndex = GetDirIndex(nx, ny, x, y);
          m_PreprocessData[index * 16 + dirIndex * 2] = (byte)(pos.row + 1);
          m_PreprocessData[index * 16 + dirIndex * 2 + 1] = (byte)(pos.col + 1);

          nx = x + 1;
          pos = Jump(nx, ny, x, y);
          dirIndex = GetDirIndex(nx, ny, x, y);
          m_PreprocessData[index * 16 + dirIndex * 2] = (byte)(pos.row + 1);
          m_PreprocessData[index * 16 + dirIndex * 2 + 1] = (byte)(pos.col + 1);
        }
      }

      using (FileStream fs = new FileStream(filename, FileMode.Create)) {
        fs.Write(m_PreprocessData, 0, m_PreprocessData.Length);
        fs.Close();
      }
      m_PreprocessData = null;
    }
    public void LoadPreprocessData(string filename)
    {
      m_PreprocessData = new byte[m_MaxRows * m_MaxCols * 16];
      using (FileStream fs = new FileStream(filename, FileMode.Open)) {
        fs.Read(m_PreprocessData, 0, m_PreprocessData.Length);
        fs.Close();
      }
    }

    public void LoadPreprocessData(byte[] data)
    {
      m_PreprocessData = data;
    }

    public List<CellPos> VisitedCells
    {
      get { return m_VisitedCells; }
    }
    public bool RecordVisitedCells
    {
      get { return m_RecordVisitedCells; }
      set { m_RecordVisitedCells = value; }
    }
    public List<Vector3> FindPath(CellPos start, CellPos target)
    {
      return FindPath(start, target, null, null);
    }
    public List<Vector3> FindPath(CellPos start, CellPos target, Vector3 addToFirst)
    {
      return FindPath(start, target, new Vector3[] { addToFirst }, null);
    }
    public List<Vector3> FindPath(CellPos start, CellPos target, Vector3[] addToFirst, Vector3[] addToLast)
    {
      m_MinCell.row = 0;
      m_MinCell.col = 0;
      m_MaxCell.row = m_MaxRows - 1;
      m_MaxCell.col = m_MaxCols - 1;

      return FindPathImpl(start, target, addToFirst, addToLast);
    }
    public List<Vector3> FindPathInScope(CellPos start, CellPos target, CellPos minCell, CellPos maxCell)
    {
      return FindPathInScope(start, target, minCell, maxCell, null, null);
    }
    public List<Vector3> FindPathInScope(CellPos start, CellPos target, CellPos minCell, CellPos maxCell, Vector3 addToFirst)
    {
      return FindPathInScope(start, target, minCell, maxCell, new Vector3[] { addToFirst }, null);
    }
    public List<Vector3> FindPathInScope(CellPos start, CellPos target, CellPos minCell, CellPos maxCell, Vector3[] addToFirst, Vector3[] addToLast)
    {
      m_MinCell = minCell;
      m_MaxCell = maxCell;

      return FindPathImpl(start, target, addToFirst, addToLast);
    }
    private List<Vector3> FindPathImpl(CellPos start, CellPos target, Vector3[] addToFirst, Vector3[] addToLast)
    {
      m_OpenQueue.Clear();
      m_OpenNodes.Clear();
      m_CloseSet.Clear();

      m_VisitedCells.Clear();

      m_StartNode = new JpsPathNode(start);
      m_EndNode = new JpsPathNode(target);

      m_StartNode.cost = 0;
      m_StartNode.from_start_cost = 0;

      m_OpenQueue.Push(m_StartNode);
      m_OpenNodes.Add(m_StartNode.cell, m_StartNode);
      
      JpsPathNode curNode = null;
      while (m_OpenQueue.Count > 0) {
        curNode = m_OpenQueue.Pop();
        if (m_RecordVisitedCells) {
          m_VisitedCells.Add(curNode.cell);

          //LogSystem.Debug("row:{0} col:{1} cost:{2} from_start_cost:{3}", curNode.cell.row, curNode.cell.col, curNode.cost, curNode.from_start_cost);
        }
        m_OpenNodes.Remove(curNode.cell);
        m_CloseSet.Add(curNode.cell, true);

        if (isEndNode(curNode)) {
          break;
        }
        
        IdentifySuccessors(curNode);      
      }

      List<Vector3> path = new List<Vector3>();
      if (isEndNode(curNode)) {
        while (curNode != null) {
          JpsPathNode prev = curNode.prev;
          if (null != prev) {
            prev.next = curNode;
          }
          curNode = prev;
        }
        if (null != addToFirst && addToFirst.Length > 0) {
          path.AddRange(addToFirst);
        }
        curNode = m_StartNode;
        while (curNode != null) {
          Vector3 pt = m_CellMgr.GetCellCenter(curNode.cell.row, curNode.cell.col);
          path.Add(pt);
          curNode = curNode.next;
        }
        if (null != addToLast && addToLast.Length > 0) {
          path.AddRange(addToLast);
        }
      }
      return path;
    }

    private void IdentifySuccessors(JpsPathNode node)
    {
      int endX = m_EndNode.cell.row;
      int endY = m_EndNode.cell.col;
      int x = node.cell.row;
      int y = node.cell.col;

      FindNeighbors(node);
      int ct = m_Neighbors.Count;
      for (int i = 0; i < ct;++i ) {
        CellPos pos = m_Neighbors[i];
        CellPos jumpPoint = DoJump(pos.row, pos.col, x, y);
        if (jumpPoint.row >= 0 && jumpPoint.col >= 0) {
          int jx = jumpPoint.row;
          int jy = jumpPoint.col;
          if (m_CloseSet.ContainsKey(jumpPoint))
            continue;
          Vector2 pos1 = new Vector2(jx, jy);
          Vector2 pos2 = new Vector2(x, y);
          float d = (pos1 - pos2).magnitude;
          float ng = node.from_start_cost + d;

          bool isOpened = true;
          JpsPathNode jumpNode;
          if (!m_OpenNodes.TryGetValue(jumpPoint, out jumpNode)) {
            jumpNode = new JpsPathNode(jumpPoint);
            isOpened = false;
          }
          if (!isOpened || ng < jumpNode.from_start_cost) {
            jumpNode.from_start_cost = ng;
            jumpNode.cost = jumpNode.from_start_cost + CalcHeuristic(jumpPoint);
            jumpNode.prev = node;

            if (!isOpened) {
              m_OpenQueue.Push(jumpNode);
              m_OpenNodes.Add(jumpPoint, jumpNode);
            } else {
              int index = IndexOpenQueue(jumpNode);
              m_OpenQueue.Update(index, jumpNode);
            }
          }
        }
      }
    }

    private int IndexOpenQueue(JpsPathNode node)
    {
      int ct = m_OpenQueue.Count;
      for (int i = 0; i < ct; ++i) {
        JpsPathNode n = m_OpenQueue[i];
        if (n.cell.row == node.cell.row && n.cell.col == node.cell.col)
          return i;
      }
      return -1;
    }

    private CellPos DoJump(int x, int y, int px, int py)
    {
      if (null == m_PreprocessData) {
        return Jump(x, y, px, py);
      } else {
        return JumpWithPreprocess(x, y, px, py);
      }
    }

    private CellPos JumpWithPreprocess(int dirx, int diry, int x, int y)
    {
      int dirIndex = GetDirIndex(dirx, diry, x, y);
      int ix = GetIndex(x, y);

      int tx = m_PreprocessData[ix * 16 + dirIndex * 2] - 1;
      int ty = m_PreprocessData[ix * 16 + dirIndex * 2 + 1] - 1;

      int ex = m_EndNode.cell.row;
      int ey = m_EndNode.cell.col;

      if (tx < 0 && ty < 0) {
        //检测是否可达目标
        Vector3 from = m_CellMgr.GetCellCenter(x, y);
        Vector3 to = m_CellMgr.GetCellCenter(ex, ey);

        bool ret = true;
        m_CellMgr.VisitCellsCrossByLine(from, to, (row, col) => {
          byte status = m_Map[row, col];
          byte block = BlockType.GetBlockType(status);
          if (block != BlockType.NOT_BLOCK) {
            ret = false;
            return false;
          } else {
            return true;
          }
        });
        if (ret) {
          return new CellPos(ex, ey);
        }
      } else if ((ex >= x && ex <= tx || ex >= tx && ex <= x) && (ey >= y && ey <= ty || ey >= ty && ey <= y)) {
        return new CellPos(ex, ey);
      }
      return new CellPos(tx, ty);
    }

    private CellPos Jump(int x, int y, int px, int py)
    {
      int dx = x - px;
      int dy = y - py;

      for (int ct = 0; ct < 1024; ++ct) {
        if (!IsWalkable(x, y))
          return new CellPos(-1, -1);
        if (null != m_EndNode && x == m_EndNode.cell.row && y == m_EndNode.cell.col)
          return new CellPos(x, y);

        if (dx != 0 && dy != 0) {
          if ((IsWalkable(x - dx, y + dy) && !IsWalkable(x - dx, y)) ||
            (IsWalkable(x + dx, y - dy) && !IsWalkable(x, y - dy))) {
            return new CellPos(x, y);
          }
        } else {
          if (dx != 0) {
            if ((IsWalkable(x + dx, y + 1) && !IsWalkable(x, y + 1)) ||
            (IsWalkable(x + dx, y - 1) && !IsWalkable(x, y - 1))) {
              return new CellPos(x, y);
            }
          } else {
            if ((IsWalkable(x + 1, y + dy) && !IsWalkable(x + 1, y)) ||
            (IsWalkable(x - 1, y + dy) && !IsWalkable(x - 1, y))) {
              return new CellPos(x, y);
            }
          }
        }

        if (dx != 0 && dy != 0) {
          CellPos jx = Jump(x + dx, y, x, y);
          CellPos jy = Jump(x, y + dy, x, y);
          if (jx.row >= 0 && jx.col >= 0 || jy.row >= 0 && jy.col >= 0)
            return new CellPos(x, y);
        }

        if (IsWalkable(x + dx, y) || IsWalkable(x, y + dy)) {
          x += dx;
          y += dy;
        } else {
          return new CellPos(-1, -1);        
        }
      }
      return new CellPos(-1, -1);
    }

    private void FindNeighbors(JpsPathNode node)
    {
      int x = node.cell.row;
      int y = node.cell.col;

      m_Neighbors.Clear();

      JpsPathNode parent = node.prev;
      if (null != parent) {
        int px = parent.cell.row;
        int py = parent.cell.col;

        int dx = (x == px ? 0 : (x - px) / Math.Abs(x - px));
        int dy = (y == py ? 0 : (y - py) / Math.Abs(y - py));

        if (dx != 0 && dy != 0) {
          if (IsWalkable(x, y + dy))
            m_Neighbors.Add(new CellPos(x, y + dy));
          if (IsWalkable(x + dx, y))
            m_Neighbors.Add(new CellPos(x + dx, y));
          if (IsWalkable(x, y + dy) || IsWalkable(x + dx, y))
            m_Neighbors.Add(new CellPos(x + dx, y + dy));
          if (!IsWalkable(x - dx, y) && IsWalkable(x, y + dy))
            m_Neighbors.Add(new CellPos(x - dx, y + dy));
          if (!IsWalkable(x, y - dy) && IsWalkable(x + dx, y))
            m_Neighbors.Add(new CellPos(x + dx, y - dy));
        } else {
          if (dx == 0) {
            if (IsWalkable(x, y + dy)) {
              if (IsWalkable(x, y + dy))
                m_Neighbors.Add(new CellPos(x, y + dy));
              if (!IsWalkable(x + 1, y))
                m_Neighbors.Add(new CellPos(x + 1, y + dy));
              if (!IsWalkable(x - 1, y))
                m_Neighbors.Add(new CellPos(x - 1, y + dy));
            }
          } else {
            if (IsWalkable(x + dx, y)) {
              if (IsWalkable(x + dx, y))
                m_Neighbors.Add(new CellPos(x + dx, y));
              if (!IsWalkable(x, y + 1))
                m_Neighbors.Add(new CellPos(x + dx, y + 1));
              if (!IsWalkable(x, y - 1))
                m_Neighbors.Add(new CellPos(x + dx, y - 1));
            }
          }
        }
      } else {
        m_Neighbors = CellManager.GetCellAdjacent(node.cell, m_MinCell.row, m_MinCell.col, m_MaxCell.row, m_MaxCell.col);
      }
    }

    private bool IsWalkable(int x, int y)
    {
      if (x < m_MinCell.row || y < m_MinCell.col || x > m_MaxCell.row || y > m_MaxCell.col)
        return false;
      return BlockType.GetBlockType(m_Map[x, y]) == 0;
    }

    private int GetIndex(int x, int y)
    {
      return m_MaxRows * x + y;
    }

    private int GetDirIndex(int nx, int ny, int x, int y)
    {
      //这里利用坐标关系编码方向在预处理数据里的索引（索引是乱序的），直接扔掉了不应该存在的dx==0 && dy==0的情形。
      int dx = nx - x;
      int dy = ny - y;

      int xdir = dx > 0 ? 4 : (dx < 0 ? 2 : 1);
      int ydir = dy > 0 ? 256 : (dy < 0 ? 32 : 4);
      int val = xdir * ydir / 8;
      int dirIndex = 0;
      switch (val) {
        case 1:
          dirIndex = 0;
          break;
        case 2:
          dirIndex = 1;
          break;
        case 4:
          dirIndex = 2;
          break;
        case 8:
          dirIndex = 3;
          break;
        case 16:
          dirIndex = 4;
          break;
        case 32:
          dirIndex = 5;
          break;
        case 64:
          dirIndex = 6;
          break;
        case 128:
          dirIndex = 7;
          break;
      }
      return dirIndex;
    }

    private float CalcHeuristic(CellPos pos)
    {
      return Math.Abs(pos.col - m_EndNode.cell.col) + Math.Abs(pos.row - m_EndNode.cell.row);
    }

    private bool isEndNode(JpsPathNode node)
    {
      return isEndNode(node.cell);
    }

    private bool isEndNode(CellPos pos)
    {
      return isEndNode(pos.row, pos.col);
    }

    private bool isEndNode(int row, int col)
    {
      return row == m_EndNode.cell.row && col == m_EndNode.cell.col;
    }

    private CellManager m_CellMgr = null;
    private byte[,] m_Map = null;
    private int m_MaxRows;
    private int m_MaxCols;
    private byte[] m_PreprocessData = null;

    private JpsPathNode m_StartNode = null;
    private JpsPathNode m_EndNode = null;
    List<CellPos> m_Neighbors = new List<CellPos>();

    private Heap<JpsPathNode> m_OpenQueue = new Heap<JpsPathNode>(new JpsPathNodeComparer());
    private Dictionary<CellPos, JpsPathNode> m_OpenNodes = new Dictionary<CellPos, JpsPathNode>(new CellPosComparer());
    private Dictionary<CellPos, bool> m_CloseSet = new Dictionary<CellPos, bool>(new CellPosComparer());

    private List<CellPos> m_VisitedCells = new List<CellPos>();
    private CellPos m_MinCell = new CellPos();
    private CellPos m_MaxCell = new CellPos();

    private bool m_RecordVisitedCells = false;
  }
}
