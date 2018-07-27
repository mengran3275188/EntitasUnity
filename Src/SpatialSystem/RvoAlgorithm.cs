using System;
using System.Collections.Generic;
using Util.MyMath;

namespace Spatial
{
  public struct OrcaLine
  {
    public Vector3 point;//半平面的参考点（Vopt+u/2）
    public Vector3 direction;//半平面分隔线的方向，半平面为该射线左边的平面
  }
  public sealed class RvoAlgorithm
  {
    /* Search for the best new velocity. */
    public Vector3 ComputeNewVelocity(ISpaceObject obj, Vector3 prefDir, float timeStep, KdObjectTree kdTree, KdObstacleTree kdObstacleTree, float maxSpeed, float neighborDist, bool isUsingAvoidanceVelocity)
    {
      ComputeNeighbors(obj, kdTree, kdObstacleTree, maxSpeed, neighborDist);
      ClearOrcaLine();
      Vector3 position = obj.GetPosition();
      float radius = (float)obj.GetRadius();
      Vector3 velocity = obj.GetVelocity();
      Vector3 newVelocity = new Vector3();

      float invTimeHorizonObst = 1.0f / m_TimeHorizonObst;

      /* Create obstacle ORCA lines. */
      for (int i = 0; i < m_ObstacleNeighbors.Count; ++i) {

        Obstacle obstacle1 = m_ObstacleNeighbors[i].Value;
        Obstacle obstacle2 = obstacle1.m_NextObstacle;

        Vector3 relativePosition1 = obstacle1.m_Point - position;
        Vector3 relativePosition2 = obstacle2.m_Point - position;

        /*
         * Check if velocity obstacle of obstacle is already taken care of by
         * previously constructed obstacle ORCA lines.
         */
        bool alreadyCovered = false;

        for (int j = 0; j < m_OrcaLines.Count; ++j) {
          if (VectorMath.det(invTimeHorizonObst * relativePosition1 - m_OrcaLines[j].point, m_OrcaLines[j].direction) - invTimeHorizonObst * radius >= -VectorMath.EPSILON && VectorMath.det(invTimeHorizonObst * relativePosition2 - m_OrcaLines[j].point, m_OrcaLines[j].direction) - invTimeHorizonObst * radius >= -VectorMath.EPSILON) {
            alreadyCovered = true;
            break;
          }
        }

        if (alreadyCovered) {
          continue;
        }

        /* Not yet covered. Check for collisions. */

        float distSq1 = VectorMath.absSq(relativePosition1);
        float distSq2 = VectorMath.absSq(relativePosition2);

        float radiusSq = VectorMath.sqr(radius);

        Vector3 obstacleVector = obstacle2.m_Point - obstacle1.m_Point;
        float s = -VectorMath.dot(relativePosition1, obstacleVector) / VectorMath.absSq(obstacleVector);
        float distSqLine = VectorMath.absSq(-relativePosition1 - s * obstacleVector);

        OrcaLine line;

        if (s < 0 && distSq1 <= radiusSq) {
          /* Collision with left vertex. Ignore if non-convex. */
          if (obstacle1.m_IsConvex) {
            line.point = new Vector3();
            line.direction = VectorMath.normalize(new Vector3(-relativePosition1.z, 0, relativePosition1.x));
            AddOrcaLine(line);
          }
          continue;
        } else if (s > 1 && distSq2 <= radiusSq) {
          /* Collision with right vertex. Ignore if non-convex
           * or if it will be taken care of by neighoring obstace */
          if (obstacle2.m_IsConvex && VectorMath.det(relativePosition2, obstacle2.m_UnitDir) >= 0) {
            line.point = new Vector3();
            line.direction = VectorMath.normalize(new Vector3(-relativePosition2.z, 0, relativePosition2.x));
            AddOrcaLine(line);
          }
          continue;
        } else if (s >= 0 && s < 1 && distSqLine <= radiusSq) {
          /* Collision with obstacle segment. */
          line.point = new Vector3();
          line.direction = -obstacle1.m_UnitDir;
          AddOrcaLine(line);
          continue;
        }

        /*
         * No collision.
         * Compute legs. When obliquely viewed, both legs can come from a single
         * vertex. Legs extend cut-off line when nonconvex vertex.
         */

        Vector3 leftLegDirection, rightLegDirection;

        if (s < 0 && distSqLine <= radiusSq) {
          /*
           * Obstacle viewed obliquely so that left vertex
           * defines velocity obstacle.
           */
          if (!obstacle1.m_IsConvex) {
            /* Ignore obstacle. */
            continue;
          }

          obstacle2 = obstacle1;

          float leg1 = VectorMath.sqrt(distSq1 - radiusSq);
          leftLegDirection = new Vector3(relativePosition1.x * leg1 - relativePosition1.z * radius, 0, relativePosition1.x * radius + relativePosition1.z * leg1) / distSq1;
          rightLegDirection = new Vector3(relativePosition1.x * leg1 + relativePosition1.z * radius, 0, -relativePosition1.x * radius + relativePosition1.z * leg1) / distSq1;
        } else if (s > 1 && distSqLine <= radiusSq) {
          /*
           * Obstacle viewed obliquely so that
           * right vertex defines velocity obstacle.
           */
          if (!obstacle2.m_IsConvex) {
            /* Ignore obstacle. */
            continue;
          }

          obstacle1 = obstacle2;

          float leg2 = VectorMath.sqrt(distSq2 - radiusSq);
          leftLegDirection = new Vector3(relativePosition2.x * leg2 - relativePosition2.z * radius, 0, relativePosition2.x * radius + relativePosition2.z * leg2) / distSq2;
          rightLegDirection = new Vector3(relativePosition2.x * leg2 + relativePosition2.z * radius, 0, -relativePosition2.x * radius + relativePosition2.z * leg2) / distSq2;
        } else {
          /* Usual situation. */
          if (obstacle1.m_IsConvex) {
            float leg1 = VectorMath.sqrt(distSq1 - radiusSq);
            leftLegDirection = new Vector3(relativePosition1.x * leg1 - relativePosition1.z * radius, 0, relativePosition1.x * radius + relativePosition1.z * leg1) / distSq1;
          } else {
            /* Left vertex non-convex; left leg extends cut-off line. */
            leftLegDirection = -obstacle1.m_UnitDir;
          }

          if (obstacle2.m_IsConvex) {
            float leg2 = VectorMath.sqrt(distSq2 - radiusSq);
            rightLegDirection = new Vector3(relativePosition2.x * leg2 + relativePosition2.z * radius, 0, -relativePosition2.x * radius + relativePosition2.z * leg2) / distSq2;
          } else {
            /* Right vertex non-convex; right leg extends cut-off line. */
            rightLegDirection = obstacle1.m_UnitDir;
          }
        }

        /*
         * Legs can never point into neighboring edge when convex vertex,
         * take cutoff-line of neighboring edge instead. If velocity projected on
         * "foreign" leg, no constraint is added.
         */

        Obstacle leftNeighbor = obstacle1.m_PrevObstacle;

        bool isLeftLegForeign = false;
        bool isRightLegForeign = false;

        if (obstacle1.m_IsConvex && VectorMath.det(leftLegDirection, -leftNeighbor.m_UnitDir) >= 0.0f) {
          /* Left leg points into obstacle. */
          leftLegDirection = -leftNeighbor.m_UnitDir;
          isLeftLegForeign = true;
        }

        if (obstacle2.m_IsConvex && VectorMath.det(rightLegDirection, obstacle2.m_UnitDir) <= 0.0f) {
          /* Right leg points into obstacle. */
          rightLegDirection = obstacle2.m_UnitDir;
          isRightLegForeign = true;
        }

        /* Compute cut-off centers. */
        Vector3 leftCutoff = invTimeHorizonObst * (obstacle1.m_Point - position);
        Vector3 rightCutoff = invTimeHorizonObst * (obstacle2.m_Point - position);
        Vector3 cutoffVec = rightCutoff - leftCutoff;

        /* Project current velocity on velocity obstacle. */

        /* Check if current velocity is projected on cutoff circles. */
        float t = (obstacle1 == obstacle2 ? 0.5f : VectorMath.dot((velocity - leftCutoff), cutoffVec) / VectorMath.absSq(cutoffVec));
        float tLeft = VectorMath.dot((velocity - leftCutoff), leftLegDirection);
        float tRight = VectorMath.dot((velocity - rightCutoff), rightLegDirection);

        if ((t < 0.0f && tLeft < 0.0f) || (obstacle1 == obstacle2 && tLeft < 0.0f && tRight < 0.0f)) {
          /* Project on left cut-off circle. */
          Vector3 unitW = VectorMath.normalize(velocity - leftCutoff);

          line.direction = new Vector3(unitW.z, 0, -unitW.x);
          line.point = leftCutoff + radius * invTimeHorizonObst * unitW;
          AddOrcaLine(line);
          continue;
        } else if (t > 1.0f && tRight < 0.0f) {
          /* Project on right cut-off circle. */
          Vector3 unitW = VectorMath.normalize(velocity - rightCutoff);

          line.direction = new Vector3(unitW.z, 0, -unitW.x);
          line.point = rightCutoff + radius * invTimeHorizonObst * unitW;
          AddOrcaLine(line);
          continue;
        }

        /*
         * Project on left leg, right leg, or cut-off line, whichever is closest
         * to velocity.
         */
        float distSqCutoff = ((t < 0.0f || t > 1.0f || obstacle1 == obstacle2) ? float.PositiveInfinity : VectorMath.absSq(velocity - (leftCutoff + t * cutoffVec)));
        float distSqLeft = ((tLeft < 0.0f) ? float.PositiveInfinity : VectorMath.absSq(velocity - (leftCutoff + tLeft * leftLegDirection)));
        float distSqRight = ((tRight < 0.0f) ? float.PositiveInfinity : VectorMath.absSq(velocity - (rightCutoff + tRight * rightLegDirection)));

        if (distSqCutoff <= distSqLeft && distSqCutoff <= distSqRight) {
          /* Project on cut-off line. */
          line.direction = -obstacle1.m_UnitDir;
          line.point = leftCutoff + radius * invTimeHorizonObst * new Vector3(-line.direction.z, 0, line.direction.x);
          AddOrcaLine(line);
          continue;
        } else if (distSqLeft <= distSqRight) {
          /* Project on left leg. */
          if (isLeftLegForeign) {
            continue;
          }

          line.direction = leftLegDirection;
          line.point = leftCutoff + radius * invTimeHorizonObst * new Vector3(-line.direction.z, 0, line.direction.x);
          AddOrcaLine(line);
          continue;
        } else {
          /* Project on right leg. */
          if (isRightLegForeign) {
            continue;
          }

          line.direction = -rightLegDirection;
          line.point = rightCutoff + radius * invTimeHorizonObst * new Vector3(-line.direction.z, 0, line.direction.x);
          AddOrcaLine(line);
          continue;
        }
      }

      int numObstLines = m_OrcaLines.Count;

      float invTimeHorizon = 1.0f / m_TimeHorizon;

      /* Create obj ORCA lines. */
      for (int i = 0; i < m_ObjNeighbors.Count; ++i) {
        KdTreeObject other = m_ObjNeighbors[i].Value;
        if (obj == other.SpaceObject)
          continue;

        Vector3 relativePosition = other.Position - position;
        Vector3 relativeVelocity = velocity - other.Velocity;
        float distSq = VectorMath.absSq(relativePosition);
        float combinedRadius = radius + other.Radius;
        float combinedRadiusSq = VectorMath.sqr(combinedRadius);

        OrcaLine line;
        Vector3 u;

        if (distSq > combinedRadiusSq) {
          /* No collision. */
          Vector3 w = relativeVelocity - invTimeHorizon * relativePosition;
          /* Vector from cutoff center to relative velocity. */
          float wLengthSq = VectorMath.absSq(w);

          float dotProduct1 = VectorMath.dot(w, relativePosition);

          if (dotProduct1 < 0.0f && VectorMath.sqr(dotProduct1) > combinedRadiusSq * wLengthSq) {
            /* Project on cut-off circle. */
            float wLength = VectorMath.sqrt(wLengthSq);
            Vector3 unitW = w / wLength;

            line.direction = new Vector3(unitW.z, 0, -unitW.x);
            u = (combinedRadius * invTimeHorizon - wLength) * unitW;
          } else {
            /* Project on legs. */
            float leg = VectorMath.sqrt(distSq - combinedRadiusSq);

            if (VectorMath.det(relativePosition, w) > 0.0f) {
              /* Project on left leg. */
              line.direction = new Vector3(relativePosition.x * leg - relativePosition.z * combinedRadius, 0, relativePosition.x * combinedRadius + relativePosition.z * leg) / distSq;
            } else {
              /* Project on right leg. */
              line.direction = -new Vector3(relativePosition.x * leg + relativePosition.z * combinedRadius, 0, -relativePosition.x * combinedRadius + relativePosition.z * leg) / distSq;
            }

            float dotProduct2 = VectorMath.dot(relativeVelocity, line.direction);

            u = dotProduct2 * line.direction - relativeVelocity;
          }
        } else {
          /* Collision. Project on cut-off circle of time timeStep. */
          float invTimeStep = 1.0f / timeStep;

          /* Vector from cutoff center to relative velocity. */
          Vector3 w = relativeVelocity - invTimeStep * relativePosition;

          float wLength = VectorMath.abs(w);
          if (wLength < VectorMath.EPSILON) {
            //LogSystem.Error("RvoAlgorithm error: Objects already overlap ! {0}({1}):{2}-{3}({4}):{5}", obj.GetID(), obj.GetObjType(), obj.GetPosition().ToString(), other.SpaceObject.GetID(), other.SpaceObject.GetObjType(), other.Position.ToString());
            continue;
          }
          Vector3 unitW = w / wLength;

          line.direction = new Vector3(unitW.z, 0, -unitW.x);
          u = (combinedRadius * invTimeStep - wLength) * unitW;
        }

        line.point = velocity + 0.5f * u;
        AddOrcaLine(line);
      }

      Vector3 avoidanceVelocity;
      if (isUsingAvoidanceVelocity)
        avoidanceVelocity = obj.GetVelocity();
      else
        avoidanceVelocity = new Vector3();

      int lineFail = LinearProgram2(m_OrcaLines, maxSpeed, prefDir, avoidanceVelocity, false, ref newVelocity);

      if (lineFail < m_OrcaLines.Count) {
        LinearProgram3(m_OrcaLines, numObstLines, lineFail, maxSpeed, avoidanceVelocity, ref newVelocity);
      }      
      return newVelocity;
    }

    private bool LinearProgram1(IList<OrcaLine> lines, int lineNo, float radius, Vector3 optVelocity, Vector3 curVelocity, bool directionOpt, ref Vector3 result)
    {
      float dotProduct = VectorMath.dot(lines[lineNo].point, lines[lineNo].direction);
      float discriminant = VectorMath.sqr(dotProduct) + VectorMath.sqr(radius) - VectorMath.absSq(lines[lineNo].point);

      if (discriminant < 0.0f) {
        /* Max speed circle fully invalidates line lineNo. */
        return false;
      }

      float sqrtDiscriminant = VectorMath.sqrt(discriminant);
      float tLeft = -dotProduct - sqrtDiscriminant;//最大速度圆与半平面分隔线的左交点（远离射线方向）
      float tRight = -dotProduct + sqrtDiscriminant;//最大速度圆与半平面分隔线的右交点（靠近射线方向）

      for (int i = 0; i < lineNo; ++i) {
        float denominator = VectorMath.det(lines[lineNo].direction, lines[i].direction);
        float numerator = VectorMath.det(lines[i].direction, lines[lineNo].point - lines[i].point);

        if (VectorMath.fabs(denominator) <= VectorMath.EPSILON) {
          /* Lines lineNo and i are (almost) parallel. */
          if (numerator < 0.0f) {
            return false;//此情形line i的半平面与line No半平面没有交集
          } else {
            continue;//此情形line i的半平面完全包含line No半平面
          }
        }

        float t = numerator / denominator;//t是line i半平面分隔线与line No分隔线的交点到line No参考点的距离（符号表示在参考点左边还是右边）

        if (denominator >= 0.0f) {
          /* Line i bounds line lineNo on the right. */
          tRight = Math.Min(tRight, t);
        } else {
          /* Line i bounds line lineNo on the left. */
          tLeft = Math.Max(tLeft, t);
        }

        if (tLeft > tRight) {
          return false;//i之前的半平面无交集
        }
      }

      if (directionOpt) {
        if (curVelocity.sqrMagnitude > VectorMath.EPSILON) {
          if (VectorMath.dot(curVelocity, lines[lineNo].direction) > 0.0f) {
            result = lines[lineNo].point + tRight * lines[lineNo].direction;
          } else {
            result = lines[lineNo].point + tLeft * lines[lineNo].direction;
          }
        } else {
          /* Optimize direction. */
          if (Math.Abs(tLeft) < VectorMath.EPSILON || VectorMath.dot(optVelocity, lines[lineNo].direction) > 0.0f && (Math.Abs(tRight) > radius * 0.5f || Math.Abs(tRight) > Math.Abs(tLeft))) {
            /* Take right extreme. */
            result = lines[lineNo].point + tRight * lines[lineNo].direction;
          } else {
            /* Take left extreme. */
            result = lines[lineNo].point + tLeft * lines[lineNo].direction;
          }
        }
      } else {
        /* Optimize closest point. */
        float t = VectorMath.dot(lines[lineNo].direction, (optVelocity - lines[lineNo].point));//预选速度矢量与line No的交点到参考点的距离（带符号）

        if (t < tLeft) {
          result = lines[lineNo].point + tLeft * lines[lineNo].direction;
        } else if (t > tRight) {
          result = lines[lineNo].point + tRight * lines[lineNo].direction;
        } else {
          result = lines[lineNo].point + t * lines[lineNo].direction;
        }
      }

      return true;
    }
    private int LinearProgram2(IList<OrcaLine> lines, float radius, Vector3 optDir, Vector3 curVelocity, bool directionOpt, ref Vector3 result)
    {
      Vector3 optVelocity = optDir * radius;
      result = optVelocity;
      for (int i = 0; i < lines.Count; ++i) {
        if (VectorMath.det(lines[i].direction, lines[i].point - result) > 0.0f) {
          /* Result does not satisfy constraint i. Compute new optimal result. */
          Vector3 tempResult = result;
          if (!LinearProgram1(lines, i, radius, directionOpt ? optDir : optVelocity, curVelocity, directionOpt, ref result)) {
            result = tempResult;
            return i;
          }
        }
      }

      return lines.Count;
    }
    private void LinearProgram3(IList<OrcaLine> lines, int numObstLines, int beginLine, float radius, Vector3 curVelocity, ref Vector3 result)
    {
      float distance = 0.0f;

      for (int i = beginLine; i < lines.Count; ++i) {
        if (VectorMath.det(lines[i].direction, lines[i].point - result) > distance) {
          /* Result does not satisfy constraint of line i. */
          //std::vector<Line> projLines(lines.begin(), lines.begin() + numObstLines);
          IList<OrcaLine> projLines = new List<OrcaLine>();
          for (int ii = 0; ii < numObstLines; ++ii) {
            projLines.Add(lines[ii]);
          }

          for (int j = numObstLines; j < i; ++j) {
            OrcaLine line;

            float determinant = VectorMath.det(lines[i].direction, lines[j].direction);

            if (VectorMath.fabs(determinant) <= VectorMath.EPSILON) {
              /* Line i and line j are parallel. */
              if (VectorMath.dot(lines[i].direction, lines[j].direction) > 0.0f) {
                /* Line i and line j point in the same direction. */
                continue;
              } else {
                /* Line i and line j point in opposite direction. */
                line.point = 0.5f * (lines[i].point + lines[j].point);
              }
            } else {
              line.point = lines[i].point + (VectorMath.det(lines[j].direction, lines[i].point - lines[j].point) / determinant) * lines[i].direction;
            }

            line.direction = VectorMath.normalize(lines[j].direction - lines[i].direction);
            projLines.Add(line);
          }

          Vector3 tempResult = result;
          if (LinearProgram2(projLines, radius, new Vector3(-lines[i].direction.z, 0, lines[i].direction.x), curVelocity, true, ref result) < projLines.Count) {
            /* This should in principle not happen.  The result is by definition
             * already in the feasible region of this linear program. If it fails,
             * it is due to small floating point error, and the current result is
             * kept.
             */
            result = tempResult;
          }

          distance = VectorMath.det(lines[i].direction, lines[i].point - result);
        }
      }
    }

    private void ComputeNeighbors(ISpaceObject obj, KdObjectTree kdTree, KdObstacleTree kdObstacleTree, float maxSpeed, float neighborDist)
    {
      m_ObstacleNeighbors.Clear();
      float range = m_TimeHorizonObst * maxSpeed + (float)obj.GetRadius();
      kdObstacleTree.Query(obj.GetPosition(), range, m_ObstacleNeighbors);

      m_ObjNeighbors.Clear();
      kdTree.Query(obj, neighborDist, (float distSqr, KdTreeObject kdObj) => {
        if (kdObj.IsAvoidable && kdObj.SpaceObject.GetObjType() == SpatialObjType.kNPC) {
          m_ObjNeighbors.Add(new KeyValuePair<float, KdTreeObject>(distSqr, kdObj));
        }
      });
    }
    private void ClearOrcaLine()
    {
      m_OrcaLines.Clear();
    }
    private void AddOrcaLine(OrcaLine line)
    {
      line.point.y = 0;
      line.direction.y = 0;
      m_OrcaLines.Add(line);
    }

    private float m_TimeHorizon = 0.1f;
    private float m_TimeHorizonObst = 0.1f;
    private IList<KeyValuePair<float, KdTreeObject>> m_ObjNeighbors = new List<KeyValuePair<float, KdTreeObject>>();
    private IList<KeyValuePair<float, Obstacle>> m_ObstacleNeighbors = new List<KeyValuePair<float, Obstacle>>();
    private IList<OrcaLine> m_OrcaLines = new List<OrcaLine>();
  }
}
