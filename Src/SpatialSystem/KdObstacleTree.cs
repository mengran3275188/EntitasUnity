using System;
using System.Collections.Generic;
using Util;
using Util.MyMath;

namespace Spatial
{
  public sealed class Obstacle
  {
    public Vector3 m_Point;
    public Vector3 m_UnitDir;
    public bool m_IsConvex;
    public Obstacle m_PrevObstacle;
    public Obstacle m_NextObstacle;
  }
  public struct VectorMath
  {
    public const float EPSILON = 0.0001f;
    public static float absSq(Vector3 v)
    {
      return v.x * v.x + v.z * v.z;
    }
    public static Vector3 normalize(Vector3 v)
    {
      float pow = absSq(v);
      if (pow < EPSILON) {
        return v;
      } else {
        return v / sqrt(pow);
      }
    }
    public static float dot(Vector3 a, Vector3 b)
    {
      return a.x * b.x + a.z * b.z;
    }
    public static float sqrt(float a)
    {
      return (float)Math.Sqrt(a);
    }
    public static float fabs(float a)
    {
      return Math.Abs(a);
    }
    public static float distSqPointLineSegment(Vector3 a, Vector3 b, Vector3 c)
    {
      Vector3 pt;
      return Geometry.PointToLineSegmentDistance(c, a, b, out pt);
    }
    public static float sqr(float p)
    {
      return p * p;
    }
    public static float det(Vector3 v1, Vector3 v2)
    {
      return v1.x * v2.z - v1.z * v2.x;
    }
    public static float abs(Vector3 v)
    {
      return (float)Math.Sqrt(absSq(v));
    }
    public static float leftOf(Vector3 a, Vector3 b, Vector3 c)
    {
      return det(a - c, b - a);
    }
  }
  public sealed class KdObstacleTree
  {
    private struct FloatPair
    {
      public FloatPair(float a, float b)
      {
        m_A = a;
        m_B = b;
      }
      public static bool operator <(FloatPair lhs, FloatPair rhs)
      {
        return (lhs.m_A < rhs.m_A || !(rhs.m_A < lhs.m_A) && lhs.m_B < rhs.m_B);
      }
      public static bool operator <=(FloatPair lhs, FloatPair rhs)
      {
        return (lhs.m_A == rhs.m_A && lhs.m_B == rhs.m_B) || lhs < rhs;
      }
      public static bool operator >(FloatPair lhs, FloatPair rhs)
      {
        return !(lhs <= rhs);
      }
      public static bool operator >=(FloatPair lhs, FloatPair rhs)
      {
        return !(lhs < rhs);
      }
      public float m_A;
      public float m_B;
    }
    public class ObstacleTreeNode
    {
      public ObstacleTreeNode m_Left;
      public Obstacle m_Obstacle;
      public ObstacleTreeNode m_Right;
    };
    public List<Obstacle> Obstacles
    {
      get { return m_Obstacles; }
      set { m_Obstacles = value; }
    }
    public ObstacleTreeNode ObstacleTree
    {
      get { return m_ObstacleTree; }
      set { m_ObstacleTree = value; }
    }

    public int AddObstacle(IList<Vector3> vertices)
    {
      if (vertices.Count < 2) {
        return -1;
      }

      int obstacleNo = m_Obstacles.Count;

      for (int i = 0; i < vertices.Count; ++i) {
        Obstacle obstacle = new Obstacle();
        obstacle.m_Point = vertices[i];
        if (i != 0) {
          obstacle.m_PrevObstacle = m_Obstacles[m_Obstacles.Count - 1];
          obstacle.m_PrevObstacle.m_NextObstacle = obstacle;
        }
        if (i == vertices.Count - 1) {
          obstacle.m_NextObstacle = m_Obstacles[obstacleNo];
          obstacle.m_NextObstacle.m_PrevObstacle = obstacle;
        }
        obstacle.m_UnitDir = VectorMath.normalize(vertices[(i == vertices.Count - 1 ? 0 : i + 1)] - vertices[i]);

        if (vertices.Count == 2) {
          obstacle.m_IsConvex = true;
        } else {
          obstacle.m_IsConvex = (VectorMath.leftOf(vertices[(i == 0 ? vertices.Count - 1 : i - 1)], vertices[i], vertices[(i == vertices.Count - 1 ? 0 : i + 1)]) >= 0);
        }

        m_Obstacles.Add(obstacle);
      }

      return obstacleNo;
    }
    public void Build()
    {
      IList<Obstacle> obstacles = new List<Obstacle>(m_Obstacles.Count);

      for (int i = 0; i < m_Obstacles.Count; ++i) {
        obstacles.Add(m_Obstacles[i]);
      }

      m_ObstacleTree = BuildRecursive(obstacles);
    }
    public void Clear()
    {
      m_Obstacles.Clear();
    }
    public List<KeyValuePair<float, Obstacle>> Query(Vector3 pos, float range)
    {
      List<KeyValuePair<float, Obstacle>> list = new List<KeyValuePair<float, Obstacle>>();
      if (null != m_ObstacleTree) {
        //queryObstacleTreeRecursive(pos, list, range * range, m_ObstacleTree);
        queryObstacleTreeImpl(pos, list, range * range);
      }
      return list;
    }
    public void Query(Vector3 pos, float range,IList<KeyValuePair<float, Obstacle>> list)
    {
      if (null != m_ObstacleTree) {
        //queryObstacleTreeRecursive(pos, list, range * range, m_ObstacleTree);
        queryObstacleTreeImpl(pos, list, range * range);
      }
    }
    public bool CheckVisibility(Vector3 q1, Vector3 q2, float radius)
    {
      return CheckVisibilityRecursive(q1, q2, radius, m_ObstacleTree);
    }
    private ObstacleTreeNode BuildRecursive(IList<Obstacle> obstacles)
    {
      if (obstacles.Count == 0) {
        return null;
      } else {
        ObstacleTreeNode node = new ObstacleTreeNode();

        int optimalSplit = 0;
        int minLeft = obstacles.Count;
        int minRight = obstacles.Count;

        for (int i = 0; i < obstacles.Count; ++i) {
          int leftSize = 0;
          int rightSize = 0;

          Obstacle obstacleI1 = obstacles[i];
          Obstacle obstacleI2 = obstacleI1.m_NextObstacle;

          /* Compute optimal split node. */
          for (int j = 0; j < obstacles.Count; ++j) {
            if (i == j) {
              continue;
            }

            Obstacle obstacleJ1 = obstacles[j];
            Obstacle obstacleJ2 = obstacleJ1.m_NextObstacle;

            float j1LeftOfI = VectorMath.leftOf(obstacleI1.m_Point, obstacleI2.m_Point, obstacleJ1.m_Point);
            float j2LeftOfI = VectorMath.leftOf(obstacleI1.m_Point, obstacleI2.m_Point, obstacleJ2.m_Point);

            if (j1LeftOfI >= -VectorMath.EPSILON && j2LeftOfI >= -VectorMath.EPSILON) {
              ++leftSize;
            } else if (j1LeftOfI <= VectorMath.EPSILON && j2LeftOfI <= VectorMath.EPSILON) {
              ++rightSize;
            } else {
              ++leftSize;
              ++rightSize;
            }

            if (new FloatPair(Math.Max(leftSize, rightSize), Math.Min(leftSize, rightSize)) >= new FloatPair(Math.Max(minLeft, minRight), Math.Min(minLeft, minRight))) {
              break;
            }
          }

          if (new FloatPair(Math.Max(leftSize, rightSize), Math.Min(leftSize, rightSize)) < new FloatPair(Math.Max(minLeft, minRight), Math.Min(minLeft, minRight))) {
            minLeft = leftSize;
            minRight = rightSize;
            optimalSplit = i;
          }
        }

        {
          /* Build split node. */
          IList<Obstacle> leftObstacles = new List<Obstacle>(minLeft);
          for (int n = 0; n < minLeft; ++n) leftObstacles.Add(null);
          IList<Obstacle> rightObstacles = new List<Obstacle>(minRight);
          for (int n = 0; n < minRight; ++n) rightObstacles.Add(null);

          int leftCounter = 0;
          int rightCounter = 0;
          int i = optimalSplit;

          Obstacle obstacleI1 = obstacles[i];
          Obstacle obstacleI2 = obstacleI1.m_NextObstacle;

          for (int j = 0; j < obstacles.Count; ++j) {
            if (i == j) {
              continue;
            }

            Obstacle obstacleJ1 = obstacles[j];
            Obstacle obstacleJ2 = obstacleJ1.m_NextObstacle;

            float j1LeftOfI = VectorMath.leftOf(obstacleI1.m_Point, obstacleI2.m_Point, obstacleJ1.m_Point);
            float j2LeftOfI = VectorMath.leftOf(obstacleI1.m_Point, obstacleI2.m_Point, obstacleJ2.m_Point);

            if (j1LeftOfI >= -VectorMath.EPSILON && j2LeftOfI >= -VectorMath.EPSILON) {
              leftObstacles[leftCounter++] = obstacles[j];
            } else if (j1LeftOfI <= VectorMath.EPSILON && j2LeftOfI <= VectorMath.EPSILON) {
              rightObstacles[rightCounter++] = obstacles[j];
            } else {
              /* Split obstacle j. */
              float t = VectorMath.det(obstacleI2.m_Point - obstacleI1.m_Point, obstacleJ1.m_Point - obstacleI1.m_Point) / VectorMath.det(obstacleI2.m_Point - obstacleI1.m_Point, obstacleJ1.m_Point - obstacleJ2.m_Point);

              Vector3 splitpoint = obstacleJ1.m_Point + t * (obstacleJ2.m_Point - obstacleJ1.m_Point);

              Obstacle newObstacle = new Obstacle();
              newObstacle.m_Point = splitpoint;
              newObstacle.m_PrevObstacle = obstacleJ1;
              newObstacle.m_NextObstacle = obstacleJ2;
              newObstacle.m_IsConvex = true;
              newObstacle.m_UnitDir = obstacleJ1.m_UnitDir;

              m_Obstacles.Add(newObstacle);

              obstacleJ1.m_NextObstacle = newObstacle;
              obstacleJ2.m_PrevObstacle = newObstacle;

              if (j1LeftOfI > 0.0f) {
                leftObstacles[leftCounter++] = obstacleJ1;
                rightObstacles[rightCounter++] = newObstacle;
              } else {
                rightObstacles[rightCounter++] = obstacleJ1;
                leftObstacles[leftCounter++] = newObstacle;
              }
            }
          }

          node.m_Obstacle = obstacleI1;
          node.m_Left = BuildRecursive(leftObstacles);
          node.m_Right = BuildRecursive(rightObstacles);
          return node;
        }
      }
    }
    private void queryObstacleTreeImpl(Vector3 pos, IList<KeyValuePair<float, Obstacle>> list, float rangeSq)
    {
      if (m_ObstacleTree == null) {
        return;
      } else {
        m_QueryStack.Push(m_ObstacleTree);
        while (m_QueryStack.Count > 0) {
          ObstacleTreeNode node = m_QueryStack.Pop();
          if (null != node) {
            Obstacle obstacle1 = node.m_Obstacle;
            Obstacle obstacle2 = obstacle1.m_NextObstacle;

            float agentLeftOfLine = VectorMath.leftOf(obstacle1.m_Point, obstacle2.m_Point, pos);

            m_QueryStack.Push(agentLeftOfLine >= 0.0f ? node.m_Left : node.m_Right);

            float distSqLine = VectorMath.sqr(agentLeftOfLine) / VectorMath.absSq(obstacle2.m_Point - obstacle1.m_Point);

            if (distSqLine < rangeSq) {
              if (agentLeftOfLine < 0.0f) {
                /*
                 * Try obstacle at this node only if agent is on right side of
                 * obstacle (and can see obstacle).
                 */
                insertObstacleNeighbor(pos, list, node.m_Obstacle, rangeSq);
              }

              /* Try other side of line. */
              m_QueryStack.Push(agentLeftOfLine >= 0.0f ? node.m_Right : node.m_Left);
            }
          }
        }
      }
    }
    private void insertObstacleNeighbor(Vector3 pos, IList<KeyValuePair<float, Obstacle>> list, Obstacle obstacle, float rangeSq)
    {
      Obstacle nextObstacle = obstacle.m_NextObstacle;

      float distSq = VectorMath.distSqPointLineSegment(obstacle.m_Point, nextObstacle.m_Point, pos);

      if (distSq < rangeSq) {
        list.Add(new KeyValuePair<float, Obstacle>(distSq, obstacle));

        int i = list.Count - 1;
        while (i != 0 && distSq < list[i - 1].Key) {
          list[i] = list[i - 1];
          --i;
        }
        list[i] = new KeyValuePair<float, Obstacle>(distSq, obstacle);
      }
    }
    private bool CheckVisibilityRecursive(Vector3 q1, Vector3 q2, float radius, ObstacleTreeNode node)
    {
      if (node == null) {
        return true;
      } else {
        Obstacle obstacle1 = node.m_Obstacle;
        Obstacle obstacle2 = obstacle1.m_NextObstacle;

        float q1LeftOfI = VectorMath.leftOf(obstacle1.m_Point, obstacle2.m_Point, q1);
        float q2LeftOfI = VectorMath.leftOf(obstacle1.m_Point, obstacle2.m_Point, q2);
        float invLengthI = 1.0f / VectorMath.absSq(obstacle2.m_Point - obstacle1.m_Point);

        if (q1LeftOfI >= 0.0f && q2LeftOfI >= 0.0f) {
          return CheckVisibilityRecursive(q1, q2, radius, node.m_Left) && ((VectorMath.sqr(q1LeftOfI) * invLengthI >= VectorMath.sqr(radius) && VectorMath.sqr(q2LeftOfI) * invLengthI >= VectorMath.sqr(radius)) || CheckVisibilityRecursive(q1, q2, radius, node.m_Right));
        } else if (q1LeftOfI <= 0.0f && q2LeftOfI <= 0.0f) {
          return CheckVisibilityRecursive(q1, q2, radius, node.m_Right) && ((VectorMath.sqr(q1LeftOfI) * invLengthI >= VectorMath.sqr(radius) && VectorMath.sqr(q2LeftOfI) * invLengthI >= VectorMath.sqr(radius)) || CheckVisibilityRecursive(q1, q2, radius, node.m_Left));
        } else if (q1LeftOfI >= 0.0f && q2LeftOfI <= 0.0f) {
          /* One can see through obstacle from left to right. */
          return CheckVisibilityRecursive(q1, q2, radius, node.m_Left) && CheckVisibilityRecursive(q1, q2, radius, node.m_Right);
        } else {
          float point1LeftOfQ = VectorMath.leftOf(q1, q2, obstacle1.m_Point);
          float point2LeftOfQ = VectorMath.leftOf(q1, q2, obstacle2.m_Point);
          float invLengthQ = 1.0f / VectorMath.absSq(q2 - q1);

          return (point1LeftOfQ * point2LeftOfQ >= 0.0f && VectorMath.sqr(point1LeftOfQ) * invLengthQ > VectorMath.sqr(radius) && VectorMath.sqr(point2LeftOfQ) * invLengthQ > VectorMath.sqr(radius) && CheckVisibilityRecursive(q1, q2, radius, node.m_Left) && CheckVisibilityRecursive(q1, q2, radius, node.m_Right));
        }
      }
    }

    private ObstacleTreeNode m_ObstacleTree = null;
    private List<Obstacle> m_Obstacles = new List<Obstacle>();
    private Stack<ObstacleTreeNode> m_QueryStack = new Stack<ObstacleTreeNode>(4096);
  }
}
