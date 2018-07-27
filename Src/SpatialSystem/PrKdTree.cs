using System;
using System.Collections.Generic;
using Util;
using Util.MyMath;

namespace Spatial
{
  public class PrKdTreeObject
  {
    public ISpaceObject SpaceObject;
    public Vector3 Position;
    public float Radius;
    public Vector3 Velocity;
    public bool IsAvoidable;
    
    internal float MaxX;
    internal float MinX;
    internal float MaxZ;
    internal float MinZ;
    internal bool Indexed;

    public PrKdTreeObject(ISpaceObject obj)
    {
      CopyFrom(obj);
    }
    public void CopyFrom(ISpaceObject obj)
    {
      if (null != obj) {
        SpaceObject = obj;
        Position = obj.GetPosition();
        Radius = (float)obj.GetRadius();
        Velocity = obj.GetVelocity();
        IsAvoidable = obj.IsAvoidable();
        MaxX = Position.x + Radius;
        MinX = Position.x - Radius;
        MaxZ = Position.z + Radius;
        MinZ = Position.z - Radius;
        Indexed = false;
      } else {
        SpaceObject = null;
        Position = new Vector3();
        Radius = 0;
        Velocity = new Vector3();
        IsAvoidable = false;
        MaxX = MinX = 0;
        MaxZ = MinZ = 0;
        Indexed = false;
      }
    }
  }
  public sealed class PrKdTree
  {
    public const int c_MaxLeafSize = 4;

    private struct KdTreeNode
    {
      internal int m_Left;
      internal int m_Right;
      internal int m_Begin;
      internal int m_End;
      internal int m_Level;
      internal float m_MaxX;
      internal float m_MaxZ;
      internal float m_MinX;
      internal float m_MinZ;
    }

    public PrKdTree(float width)
    {
      m_Width = width;
    }

    public void Build(IList<ISpaceObject> objs)
    {
      if (null == m_Objects || m_Objects.Length < objs.Count) {
        m_ObjectNum = 0;
        m_Objects = new PrKdTreeObject[objs.Count * 2];
        foreach (ISpaceObject obj in objs) {
          m_Objects[m_ObjectNum++] = new PrKdTreeObject(obj);
        }
      } else {
        m_ObjectNum = 0;
        foreach (ISpaceObject obj in objs) {
          if (null == m_Objects[m_ObjectNum])
            m_Objects[m_ObjectNum] = new PrKdTreeObject(obj);
          else
            m_Objects[m_ObjectNum].CopyFrom(obj);
          ++m_ObjectNum;
        }
      }
      if (m_ObjectNum > 0) {
        if (null == m_KdTree || m_MaxNodeNum < m_ObjectNum * c_MaxLevel) {
          m_KdTree = new KdTreeNode[m_ObjectNum * c_MaxLevel];
          for (int i = 0; i < m_KdTree.Length; ++i) {
            m_KdTree[i] = new KdTreeNode();
          }
          m_MaxNodeNum = m_ObjectNum * c_MaxLevel;
        }
        BuildImpl();
      }
    }
    public void Clear()
    {
      m_ObjectNum = 0;
    }

    public void Query(ISpaceObject obj, float range, MyAction<float, PrKdTreeObject> visitor)
    {
      Query(obj.GetPosition(), range, visitor);
    }

    public void Query(Vector3 pos, float range, MyAction<float, PrKdTreeObject> visitor)
    {
      if (null != m_KdTree && m_ObjectNum > 0 && m_KdTree.Length > 0) {
        float rangeSq = Sqr(range);
        QueryImpl(pos, range, rangeSq, (float distSqr, PrKdTreeObject obj) => { visitor(distSqr, obj); return true; });
      }
    }

    public void Query(ISpaceObject obj, float range, MyFunc<float, PrKdTreeObject, bool> visitor)
    {
      Query(obj.GetPosition(), range, visitor);
    }

    public void Query(Vector3 pos, float range, MyFunc<float, PrKdTreeObject, bool> visitor)
    {
      if (null != m_KdTree && m_ObjectNum > 0 && m_KdTree.Length > 0) {
        float rangeSq = Sqr(range);
        QueryImpl(pos, range, rangeSq, visitor);
      }
    }

    public void VisitTree(MyAction<float, float, float, float, int, int, PrKdTreeObject[]> visitor)
    {
      VisitTree((float x0, float z0, float x1, float z1, int begin, int end, PrKdTreeObject[] objs) => { visitor(x0, z0, x1, z1, begin, end, objs); return true; });
    }

    public void VisitTree(MyFunc<float, float, float, float, int, int, PrKdTreeObject[], bool> visitor)
    {
      if (null != m_KdTree && m_ObjectNum > 0 && m_KdTree.Length > 0) {
        VisitTreeImpl(visitor);
      }
    }

    private void BuildImpl()
    {
      int nextUnusedNode=1;
      m_KdTree[0].m_Level = 0;
      m_KdTree[0].m_MinX = 0;
      m_KdTree[0].m_MinZ = 0;
      m_KdTree[0].m_MaxX = m_Width;
      m_KdTree[0].m_MaxZ = m_Width;

      m_BuildStack.Push(0);
      m_BuildStack.Push(m_ObjectNum);
      m_BuildStack.Push(0);
      while (m_BuildStack.Count >= 3) {
        int begin = m_BuildStack.Pop();
        int end = m_BuildStack.Pop();
        int node = m_BuildStack.Pop();
        
        if (end - begin > c_MaxLeafSize) {
          int lvl = m_KdTree[node].m_Level;
          float x0 = m_KdTree[node].m_MinX;
          float z0 = m_KdTree[node].m_MinZ;
          float x1 = m_KdTree[node].m_MaxX;
          float z1 = m_KdTree[node].m_MaxZ;
          float x = (x0 + x1) / 2;
          float z = (z0 + z1) / 2;

          bool isVertical = ((lvl % 2) == 0 ? true : false);

          float lx0 = x0;
          float lz0 = z0;
          float lx1 = (isVertical ? x : x1);
          float lz1 = (isVertical ? z1 : z);

          float rx0 = (isVertical ? x : x0);
          float rz0 = (isVertical ? z0 : z);
          float rx1 = x1;
          float rz1 = z1;

          m_KdTree[node].m_Left = nextUnusedNode;
          m_KdTree[nextUnusedNode].m_Level = lvl + 1;
          m_KdTree[nextUnusedNode].m_MinX = lx0;
          m_KdTree[nextUnusedNode].m_MinZ = lz0;
          m_KdTree[nextUnusedNode].m_MaxX = lx1;
          m_KdTree[nextUnusedNode].m_MaxZ = lz1;
          ++nextUnusedNode;
          m_KdTree[node].m_Right = nextUnusedNode;
          m_KdTree[nextUnusedNode].m_Level = lvl + 1;
          m_KdTree[nextUnusedNode].m_MinX = rx0;
          m_KdTree[nextUnusedNode].m_MinZ = rz0;
          m_KdTree[nextUnusedNode].m_MaxX = rx1;
          m_KdTree[nextUnusedNode].m_MaxZ = rz1;
          ++nextUnusedNode;

          int begin0 = begin;
          int left = begin;
          int right = end;

          while (left < right) {
            while (left < right) {
              PrKdTreeObject obj = m_Objects[left];
              if (Contained(obj, lx0, lz0, lx1, lz1)) {
                ++left;
              } else if (!Contained(obj, rx0, rz0, rx1, rz1)) {
                obj.Indexed = true;
                break;
              } else {
                break;
              }
            }
            while (left < right) {
              PrKdTreeObject obj = m_Objects[right - 1];
              if (Contained(obj, rx0, rz0, rx1, rz1)) {
                --right;
              } else if (!Contained(obj, lx0, lz0, lx1, lz1)) {
                obj.Indexed = true;
                break;
              } else {
                break;
              }
            }
            if (left < right) {
              if (m_Objects[left].Indexed || m_Objects[right - 1].Indexed) {
                if (m_Objects[left].Indexed) {
                  PrKdTreeObject tmp = m_Objects[begin];
                  m_Objects[begin] = m_Objects[left];
                  m_Objects[left] = tmp;
                  ++begin;
                  ++left;
                }
                if (left < right && m_Objects[right - 1].Indexed) {
                  PrKdTreeObject tmp = m_Objects[begin];
                  m_Objects[begin] = m_Objects[right - 1];
                  m_Objects[right - 1] = tmp;
                  ++begin;
                  if (begin >= left) {
                    ++left;
                  }
                }
              } else {
                PrKdTreeObject tmp = m_Objects[left];
                m_Objects[left] = m_Objects[right - 1];
                m_Objects[right - 1] = tmp;
                ++left;
                --right;
              }
            }
          }

          m_KdTree[node].m_Begin = begin0;
          m_KdTree[node].m_End = begin;

          if (left > begin) {
            m_BuildStack.Push(m_KdTree[node].m_Left);
            m_BuildStack.Push(left);
            m_BuildStack.Push(begin);
          }

          if (end > left) {
            m_BuildStack.Push(m_KdTree[node].m_Right);
            m_BuildStack.Push(end);
            m_BuildStack.Push(left);
          }
        } else {
          m_KdTree[node].m_Begin = begin;
          m_KdTree[node].m_End = end;
          m_KdTree[node].m_Left = 0;
          m_KdTree[node].m_Right = 0;
        }
      }
    }

    private void QueryImpl(Vector3 pos, float range, float rangeSq, MyFunc<float, PrKdTreeObject, bool> visitor)
    {
      float _x0 = pos.x - range;
      float _x1 = pos.x + range;
      float _z0 = pos.z - range;
      float _z1 = pos.z + range;

      m_QueryStack.Push(0);
      while (m_QueryStack.Count > 0) {
        int node = m_QueryStack.Pop();

        int begin = m_KdTree[node].m_Begin;
        int end = m_KdTree[node].m_End;
        int lvl = m_KdTree[node].m_Level;
        int left = m_KdTree[node].m_Left;
        int right = m_KdTree[node].m_Right;

        if (end > begin) {
          for (int i = begin; i < end; ++i) {
            PrKdTreeObject obj = m_Objects[i];
            if (Geometry.RectangleOverlapRectangle(pos.x - range, pos.z - range, pos.x + range, pos.z + range, obj.MinX, obj.MinZ, obj.MaxX, obj.MaxZ)) {
              float distSq = Geometry.DistanceSquare(pos, m_Objects[i].Position);
              if (!visitor(distSq, m_Objects[i])) {
                m_QueryStack.Clear();
                return;
              }
            }
          }
        }
        
        bool isVertical = ((lvl % 2) == 0 ? true : false);
        float x0 = m_KdTree[node].m_MinX;
        float z0 = m_KdTree[node].m_MinZ;
        float x1 = m_KdTree[node].m_MaxX;
        float z1 = m_KdTree[node].m_MaxZ;

        if (isVertical) {
          if (left > 0) {
            float lx0 = x0;
            float lx1 = (x0 + x1) / 2;
            if (Overlap(_x0, _x1, lx0, lx1)) {
              m_QueryStack.Push(left);
            }
          }
          if (right>0) {
            float rx0 = (x0 + x1) / 2;
            float rx1 = x1;
            if (Overlap(_x0, _x1, rx0, rx1)) {
              m_QueryStack.Push(right);
            }
          }
        } else {
          if (left > 0) {
            float lz0 = z0;
            float lz1 = (z0 + z1) / 2;
            if (Overlap(_z0, _z1, lz0, lz1)) {
              m_QueryStack.Push(left);
            }
          }
          if (right > 0) {
            float rz0 = (z0 + z1) / 2;
            float rz1 = z1;
            if (Overlap(_z0, _z1, rz0, rz1)) {
              m_QueryStack.Push(right);
            }
          }
        }
      }
    }

    private void VisitTreeImpl(MyFunc<float, float, float, float, int, int, PrKdTreeObject[], bool> visitor)
    {
      m_QueryStack.Push(0);
      while (m_QueryStack.Count > 0) {
        int node = m_QueryStack.Pop();

        int begin = m_KdTree[node].m_Begin;
        int end = m_KdTree[node].m_End;
        int lvl = m_KdTree[node].m_Level;
        int left = m_KdTree[node].m_Left;
        int right = m_KdTree[node].m_Right;

        float x0 = m_KdTree[node].m_MinX;
        float z0 = m_KdTree[node].m_MinZ;
        float x1 = m_KdTree[node].m_MaxX;
        float z1 = m_KdTree[node].m_MaxZ;
        bool isVertical = ((lvl % 2) == 0 ? true : false);


        if (isVertical) {
          float splitValue = 0.5f * (x0 + x1);
          if (!visitor(splitValue, z0, splitValue, z1, begin, end, m_Objects)) {
            m_QueryStack.Clear();
            return;
          }
        } else {
          float splitValue = 0.5f * (z0 + z1);
          if (!visitor(x0, splitValue, x1, splitValue, begin, end, m_Objects)) {
            m_QueryStack.Clear();
            return;
          }
        }
        if (left > 0)
          m_QueryStack.Push(left);
        if (right > 0)
          m_QueryStack.Push(right);
      }
    }
        
    private static bool Overlap(float _x1, float _x2, float x1, float x2)
    {
      if (_x1 > x2 + Geometry.c_FloatPrecision)
        return false;
      if (_x2 < x1 - Geometry.c_FloatPrecision)
        return false;
      return true;
    }

    private static bool Contained(PrKdTreeObject obj, float x0, float z0, float x1, float z1)
    {
      float _x0 = obj.Position.x - obj.Radius;
      float _x1 = obj.Position.x + obj.Radius;
      float _z0 = obj.Position.z - obj.Radius;
      float _z1 = obj.Position.z + obj.Radius;
      return Contained(_x0, _z0, _x1, _z1, x0, z0, x1, z1);
    }

    private static bool Contained(float _x1, float _y1, float _x2, float _y2, float x1, float y1, float x2, float y2)
    {
      if (_x1 > x1 + Geometry.c_FloatPrecision &&
          _x2 < x2 - Geometry.c_FloatPrecision &&
          _y1 > y1 + Geometry.c_FloatPrecision &&
          _y2 < y2 - Geometry.c_FloatPrecision)
        return true;
      else
        return false;
    }

    private static float Sqr(float v)
    {
      return v * v;
    }

    private float m_Width = 128.0f;
    private const int c_MaxLevel = 12;

    private PrKdTreeObject[] m_Objects = null;
    private int m_ObjectNum = 0;
    private KdTreeNode[] m_KdTree = null;
    private int m_MaxNodeNum = 0;

    private Stack<int> m_BuildStack = new Stack<int>(4096);
    private Stack<int> m_QueryStack = new Stack<int>(4096);
  }
}
