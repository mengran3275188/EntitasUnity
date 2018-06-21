#define DISABLE_DIAGONALS

using System;
using System.Collections.Generic;
using System.IO;
using Util;

namespace Spatial
{
  public class TreeCacheFinder
  {
    private enum OperationEnum : byte
    {
      NO_OP = 0,
      UP,
      DOWN,
      LEFT,
      RIGHT,
      UP_LEFT,
      UP_RIGHT,
      DOWN_LEFT,
      DOWN_RIGHT,
    }
    private struct PreTreeNode
    {
      internal bool Wall;
      internal bool Visited;
      internal float CostF;
    }
    private struct TreeNode
    {
      internal byte CostI;
      internal byte OpToParent;
    }

    public void PreprocessMap(CellManager cellMgr, string filename)
    {
      m_Width = cellMgr.GetMaxCol();
      m_Height = cellMgr.GetMaxRow();
      byte[,] map = cellMgr.cells_arr_;

      using (FileStream fs = new FileStream(filename, FileMode.Create)) {
        // init
        fs.WriteByte((byte)(m_Width & 0x0ff));
        fs.WriteByte((byte)((m_Width & 0x0ff00) >> 8));
        fs.WriteByte((byte)(m_Height & 0x0ff));
        fs.WriteByte((byte)((m_Height & 0x0ff00) >> 8));

        int size = map.Length;
        m_Tree = new TreeNode[size];
        m_PreTree = new PreTreeNode[size];
          
        for (int i = 0; i < size; ++i) {
          m_PreTree[i].Wall = BlockType.GetBlockType(map[i / m_Width, i % m_Width]) == 0;
          m_Tree[i].OpToParent = (byte)OperationEnum.NO_OP;
          m_PreTree[i].Visited = false;
          m_PreTree[i].CostF = float.MaxValue;
          m_Tree[i].CostI = (byte)255;
        }

        // Create Trees (multiple roots are possible)
        for (int i = 0; i < size; ++i) {
          if (m_PreTree[i].Wall && !m_PreTree[i].Visited) {
            CreateTree(i);
          }
        }

        // Compact
        for (int i = 0; i < size; ++i) {
          // Round up to ensure that only the root node gets a cost of 0.
          double scaledCostF = Math.Min(254.0, Math.Ceiling(m_PreTree[i].CostF / 5.0));
          int scaledCostI = (int)scaledCostF;
          m_Tree[i].CostI = (byte)scaledCostI;
        }
        for (int i = 0; i < size; ++i) {
          fs.WriteByte(m_Tree[i].CostI);
          fs.WriteByte(m_Tree[i].OpToParent);
        }
        fs.Close();
      }
      m_PreTree = null;
      m_Tree = null;
    }
    public void PrepareForSearch(string filename)
    {
      using (FileStream fs = new FileStream(filename, FileMode.Open)) {
        int wl = fs.ReadByte();
        int wh = fs.ReadByte();
        int hl = fs.ReadByte();
        int hh = fs.ReadByte();
        if (hl >= 0 && hh >= 0) {
          m_Width = wl + (wh << 8);
          m_Height = hl + (hh << 8);
          int size = m_Width * m_Height;

          m_Tree = new TreeNode[size];
          for (int i = 0; i < size; ++i) {
            m_Tree[i].CostI = (byte)fs.ReadByte();
            m_Tree[i].OpToParent = (byte)fs.ReadByte();
          }
        }
        fs.Close();
      }
    }
    public void GetPath(CellPos start, CellPos target, List<CellPos> path)
    {
      Stack<CellPos> q = new Stack<CellPos>();
      CellPos loc = new CellPos(start.row, start.col);
      CellPos loc2 = new CellPos(target.row, target.col);
      int index, index2;
      index = GetIndex(loc);
      index2 = GetIndex(loc2);
      
      // Get both halves of path
      //path.Clear();
      //q.Clear();
      path.Add(loc);
      q.Push(loc2);
      while (loc.col != loc2.col || loc.row != loc2.row) {
        if (m_Tree[index].CostI > m_Tree[index2].CostI) {
          OperationEnum op = (OperationEnum)m_Tree[index].OpToParent;
          if (op == OperationEnum.NO_OP) {
            //op = OperationEnum.RIGHT;
            break;
          }
          loc = GetNeighbour(loc, op);
          index = GetIndex(loc);
          path.Add(loc);
        } else {
          OperationEnum op = (OperationEnum)m_Tree[index2].OpToParent;
          if (op == OperationEnum.NO_OP) {
            //op = OperationEnum.RIGHT;
            break;
          }
          loc2 = GetNeighbour(loc2, op);
          index2 = GetIndex(loc2);
          q.Push(loc2);
        }
      }

      // Skip the root of the tree (don't want to include it twice)
      path.RemoveAt(path.Count - 1);

      // Reverse order and add to solution
      while (q.Count > 0) {
        path.Add(q.Pop());
      }
    }

    // generates 8-connected neighbors
    // a diagonal move must have both cardinal neighbors free to be legal
    void GetSuccessors(CellPos s, List<OperationEnum> ops)
    {
      ops.Clear();
      int opFlags = 0;
      CellPos next = new CellPos(s.row, s.col);

      next.col++;
      if (next.col < m_Width && m_PreTree[GetIndex(next)].Wall) {
        ops.Add(OperationEnum.RIGHT);
        opFlags |= 0x01 << (int)OperationEnum.RIGHT;
      }

      next = new CellPos(s.row, s.col);
      next.col--;
      if (next.col >= 0 && m_PreTree[GetIndex(next)].Wall) {
        ops.Add(OperationEnum.LEFT);
        opFlags |= 0x01 << (int)OperationEnum.LEFT;
      }

      next = new CellPos(s.row, s.col);
      next.row++;
      if (next.row < m_Height && m_PreTree[GetIndex(next)].Wall) {
        ops.Add(OperationEnum.DOWN);
        opFlags |= 0x01 << (int)OperationEnum.DOWN;
      }

      next = new CellPos(s.row, s.col);
      next.row--;
      if (next.row >= 0 && m_PreTree[GetIndex(next)].Wall) {
        ops.Add(OperationEnum.UP);
        opFlags |= 0x01 << (int)OperationEnum.UP;
      }
#if !DISABLE_DIAGONALS
      next = new CellPos(s.row, s.col);
      next.row--;
      next.col++;
      if ((opFlags & (0x01 << (int)OperationEnum.UP)) != 0 && (opFlags & (0x01 << (int)OperationEnum.RIGHT)) != 0 && m_PreTree[GetIndex(next)].Wall) {
        ops.Add(OperationEnum.UP_RIGHT);
      }

      next = new CellPos(s.row, s.col);
      next.row--;
      next.col--;
      if ((opFlags & (0x01 << (int)OperationEnum.UP)) != 0 && (opFlags & (0x01 << (int)OperationEnum.LEFT)) != 0 && m_PreTree[GetIndex(next)].Wall) {
        ops.Add(OperationEnum.UP_LEFT);
      }

      next = new CellPos(s.row, s.col);
      next.row++;
      next.col++;
      if ((opFlags & (0x01 << (int)OperationEnum.DOWN)) != 0 && (opFlags & (0x01 << (int)OperationEnum.RIGHT)) != 0 && m_PreTree[GetIndex(next)].Wall) {
        ops.Add(OperationEnum.DOWN_RIGHT);
      }

      next = new CellPos(s.row, s.col);
      next.row++;
      next.col--;
      if ((opFlags & (0x01 << (int)OperationEnum.DOWN)) != 0 && (opFlags & (0x01 << (int)OperationEnum.LEFT)) != 0 && m_PreTree[GetIndex(next)].Wall) {
        ops.Add(OperationEnum.DOWN_LEFT);
      }
#endif
    }

    // Basicaly does a Djikstra's search to create a search m_Tree
    // anchored at the given location.
    void CreateTree(int index)
    {
      List<OperationEnum> ops = new List<OperationEnum>();
      Queue<CellPos> q = new Queue<CellPos>();

      CellPos loc = new CellPos(index / m_Width, index % m_Width);
      q.Enqueue(loc);
      m_PreTree[index].Visited = true;
      m_Tree[index].OpToParent = (byte)OperationEnum.NO_OP;
      m_PreTree[index].CostF = 0;

      while (q.Count > 0) {
        loc = q.Dequeue();
        index = GetIndex(loc);

        GetSuccessors(loc, ops);
        for (int i = 0; i < ops.Count; ++i) {
          OperationEnum op = ops[i];
          CellPos successor = GetNeighbour(loc, op);
          float edge_cost = GetCost(op);
          float succ_cost = m_PreTree[index].CostF + edge_cost;
          int index2 = GetIndex(successor);

          if (succ_cost < m_PreTree[index2].CostF) {
            m_Tree[index2].OpToParent = (byte)Reverse(op);
            m_PreTree[index2].CostF = succ_cost;
            if (m_PreTree[index2].Visited == false) {
              m_PreTree[index2].Visited = true;
              q.Enqueue(successor);
            }
          }
        }
      }
    }

    // Find the index of an unvisited node, if one exists
    // Return index if node is found, -1 otherwise
    private int FindUnvisitedNodeIndex()
    {
      int size = m_Width * m_Height;
      int index;

      // Random search
      for (int i = 0; i < 10000; ++i) {
        index = RandomUtil.Next() % size;
        if (m_PreTree[index].Wall == true && m_PreTree[index].Visited == false)
          return index;
      }

      // If cannot find after many random iterations, search linearly through all nodes
      for (int i = 0; i < size; ++i) {
        index = i;
        if (m_PreTree[index].Wall == true && m_PreTree[index].Visited == false) {
          return index;
        }
      }

      // None left!
      return -1;
    }

    private OperationEnum Reverse(OperationEnum op)
    {
      switch (op) {
        case OperationEnum.UP:
          op = OperationEnum.DOWN;
          break;
        case OperationEnum.DOWN:
          op = OperationEnum.UP;
          break;
        case OperationEnum.LEFT:
          op = OperationEnum.RIGHT;
          break;
        case OperationEnum.RIGHT:
          op = OperationEnum.LEFT;
          break;
        case OperationEnum.UP_LEFT:
          op = OperationEnum.DOWN_RIGHT;
          break;
        case OperationEnum.UP_RIGHT:
          op = OperationEnum.DOWN_LEFT;
          break;
        case OperationEnum.DOWN_LEFT:
          op = OperationEnum.UP_RIGHT;
          break;
        case OperationEnum.DOWN_RIGHT:
          op = OperationEnum.UP_LEFT;
          break;
      }
      return op;
    }

    private float GetCost(OperationEnum op)
    {
      switch (op) {
        case OperationEnum.UP:
        case OperationEnum.DOWN:
        case OperationEnum.LEFT:
        case OperationEnum.RIGHT:
          return 1.0f;
        default:
          return c_SQRT2;
      }
    }

    private CellPos GetNeighbour(CellPos pos, OperationEnum op)
    {
      CellPos r = new CellPos(pos.row, pos.col);
      switch (op) {
        case OperationEnum.UP:
          --r.row;
          break;
        case OperationEnum.DOWN:
          ++r.row;
          break;
        case OperationEnum.LEFT:
          --r.col;
          break;
        case OperationEnum.RIGHT:
          ++r.col;
          break;
        case OperationEnum.UP_LEFT:
          --r.col;
          --r.row;
          break;
        case OperationEnum.UP_RIGHT:
          ++r.col;
          --r.row;
          break;
        case OperationEnum.DOWN_LEFT:
          --r.col;
          ++r.row;
          break;
        case OperationEnum.DOWN_RIGHT:
          ++r.col;
          ++r.row;
          break;
      }
      return r;
    }

    private int GetIndex(CellPos pos)
    {
      return pos.row * m_Width + pos.col;
    }

    private const float c_SQRT2 = 1.414f;

    private int m_Width;
    private int m_Height;
    private PreTreeNode[] m_PreTree = null;
    private TreeNode[] m_Tree = null;
  }
}
