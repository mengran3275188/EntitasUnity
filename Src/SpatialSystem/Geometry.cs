using System;
using System.Collections.Generic;
using Util.MyMath;
using Util;

namespace Spatial
{
  public partial class Geometry
  {
    public const float c_MinDistance = 0.0001f;
    public const float c_FloatPrecision = 0.0001f;
    public const float c_DoublePrecision = 0.0000001f;
    public static bool IsInvalid(float v)
    {
      return float.IsNaN(v) || float.IsInfinity(v);
    }
    public static bool IsInvalid(Vector3 pos)
    {
      return IsInvalid(pos.x) || IsInvalid(pos.y) || IsInvalid(pos.z);
    }
    public static float Max(float a, float b)
    {
      return (a > b ? a : b);
    }
    public static float Min(float a, float b)
    {
      return (a < b ? a : b);
    }
    public static float DistanceSquare(float x1, float y1, float x2, float y2)
    {
      return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
    }
    public static float DistanceSquare(Vector3 p1, Vector3 p2)
    {
      return (p1.x - p2.x) * (p1.x - p2.x) + (p1.z - p2.z) * (p1.z - p2.z);
    }
    public static float Distance(Vector3 p1, Vector3 p2)
    {
      return (float)Math.Sqrt(DistanceSquare(p1, p2));
    }
    public static float CalcLength(IList<Vector3> pts)
    {
      float len = 0;
      for (int i = 0; i < pts.Count-1; ++i) {
        len += Distance(pts[i], pts[i + 1]);
      }
      return len;
    }
    public static bool IsSameFloat(float v1, float v2)
    {
      return Math.Abs(v1 - v2) < c_FloatPrecision;
    }
    public static bool IsSameDouble(double v1, double v2)
    {
      return Math.Abs(v1 - v2) < c_DoublePrecision;
    }
    public static bool IsSamePoint(Vector3 p1, Vector3 p2)
    {
      return IsSameFloat(p1.x, p2.x) && IsSameFloat(p1.z, p2.z);
    }
    public static bool IsSamePoint(Vector2 p1, Vector2 p2)
    {
      return IsSameFloat(p1.x, p2.x) && IsSameFloat(p1.y, p2.y);
    }
    /// <summary>
    /// 计算向量叉乘
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="ep"></param>
    /// <param name="op"></param>
    /// <returns>
    /// ret>0 ep在opsp矢量逆时针方向
    /// ret=0 共线
    /// ret<0 ep在opsp矢量顺时针方向
    /// </returns>
    public static float Multiply(Vector3 sp, Vector3 ep, Vector3 op)
    {
      return ((sp.x - op.x) * (ep.z - op.z) - (ep.x - op.x) * (sp.z - op.z));
    }
    /// <summary>
    /// r=DotMultiply(p1,p2,p0),得到矢量(p1-p0)和(p2-p0)的点积。
    /// 注：两个矢量必须是非零矢量
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p0"></param>
    /// <returns>
    /// r>0:两矢量夹角为锐角；
    /// r=0：两矢量夹角为直角；
    /// r<0:两矢量夹角为钝角
    /// </returns>
    public static float DotMultiply(Vector3 p1, Vector3 p2, Vector3 p0)
    {
      return ((p1.x - p0.x) * (p2.x - p0.x) + (p1.z - p0.z) * (p2.z - p0.z));
    }
    /// <summary>
    /// 判断p与线段p1p2的关系
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns>
    /// r=0 p = p1
    /// r=1 p = p2
    /// r<0 p is on the backward extension of p1p2
    /// r>1 p is on the forward extension of p1p2
    /// 0<r<1 p is interior to p1p2
    /// </returns>
    public static float Relation(Vector3 p, Vector3 p1, Vector3 p2)
    {
      return DotMultiply(p, p2, p1) / DistanceSquare(p1, p2);
    }
    /// <summary>
    /// 求垂足
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static Vector3 Perpendicular(Vector3 p, Vector3 p1, Vector3 p2)
    {
      float r = Relation(p, p1, p2);
      Vector3 tp=new Vector3();
      tp.x = p1.x + r * (p2.x - p1.x);
      tp.z = p1.z + r * (p2.z - p1.z);
      return tp;
    }
    /// <summary>
    /// 求点到线段的最小距离并返回距离最近的点。
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="np"></param>
    /// <returns></returns>
    public static float PointToLineSegmentDistance(Vector3 p, Vector3 p1, Vector3 p2,out Vector3 np)
    {
      float r=Relation(p,p1,p2);
      if(r<0)
      {
        np=p1;
        return Distance(p,p1);
      }
      if(r>1)
      {
        np=p2;
        return Distance(p,p2);
      }
      np=Perpendicular(p,p1,p2);
      return Distance(p,np);
    }
    public static float PointToLineSegmentDistanceSquare(Vector3 p, Vector3 p1, Vector3 p2, out Vector3 np)
    {
      float r = Relation(p, p1, p2);
      if (r < 0) {
        np = p1;
        return DistanceSquare(p, p1);
      }
      if (r > 1) {
        np = p2;
        return DistanceSquare(p, p2);
      }
      np = Perpendicular(p, p1, p2);
      return DistanceSquare(p, np);
    }
    /// <summary>
    /// 求点到直线的距离
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static float PointToLineDistance(Vector3 p,Vector3 p1,Vector3 p2)
    {
      return Math.Abs(Multiply(p,p2,p1))/Distance(p1,p2);
    }
    public static float PointToLineDistanceInverse(Vector3 p, Vector3 p1, Vector3 p2)
    {
      return Math.Abs(Multiply(p, p2, p1)) / Distance(p1, p2);
    }   
    /// <summary>
    /// 求点到折线的最小距离并返回最小距离点。
    /// 注：如果给定点不足以够成折线，则不会修改q的值
    /// </summary>
    /// <param name="p"></param>
    /// <param name="pts"></param>
    /// <param name="start"></param>
    /// <param name="len"></param>
    /// <param name="q"></param>
    /// <returns></returns>
    public static float PointToPolylineDistance(Vector3 p,IList<Vector3> pts, int start,int len,ref Vector3 q)
    {
      float ret = float.MaxValue;
      for (int i = start; i < start + len - 1; ++i) {
        Vector3 tq;
        float d = PointToLineSegmentDistance(p, pts[i], pts[i + 1], out tq);
        if (d < ret) {
          ret = d;
          q = tq;
        }
      }
      return ret;
    }
    public static float PointToPolylineDistanceSquare(Vector3 p, IList<Vector3> pts, int start, int len, ref Vector3 q)
    {
      float ret = float.MaxValue;
      for (int i = start; i < start + len - 1; ++i) {
        Vector3 tq;
        float d = PointToLineSegmentDistanceSquare(p, pts[i], pts[i + 1], out tq);
        if (d < ret) {
          ret = d;
          q = tq;
        }
      }
      return ret;
    }
    public static bool Intersect(Vector3 us, Vector3 ue, Vector3 vs, Vector3 ve)
    {
      return ((Max(us.x, ue.x) >= Min(vs.x, ve.x)) &&
      (Max(vs.x, ve.x) >= Min(us.x, ue.x)) &&
      (Max(us.z, ue.z) >= Min(vs.z, ve.z)) &&
      (Max(vs.z, ve.z) >= Min(us.z, ue.z)) &&
      (Multiply(vs, ue, us) * Multiply(ue, ve, us) >= 0) &&
      (Multiply(us, ve, vs) * Multiply(ve, ue, vs) >= 0));
    }
    public static bool LineIntersectRectangle(Vector3 lines, Vector3 linee, float left, float top, float right, float bottom)
    {
      if (Max(lines.x, linee.x) >= left && right >= Min(lines.x, linee.x) && Max(lines.z, linee.z) >= top && bottom >= Min(lines.z, linee.z)) {
        Vector3 lt = new Vector3(left, 0, top);
        Vector3 rt = new Vector3(right, 0, top);
        Vector3 rb = new Vector3(right, 0, bottom);
        Vector3 lb = new Vector3(left, 0, bottom);
        if (Multiply(lines, lt, linee) * Multiply(lines, rt, linee) <= 0)
          return true;
        if (Multiply(lines, lt, linee) * Multiply(lines, lb, linee) <= 0)
          return true;
        if (Multiply(lines, rt, linee) * Multiply(lines, rb, linee) <= 0)
          return true;
        if (Multiply(lines, lb, linee) * Multiply(lines, rb, linee) <= 0)
          return true;
        /*
        if (Intersect(lines, linee, lt, rt))
          return true;
        if (Intersect(lines, linee, rt, rb))
          return true;
        if (Intersect(lines, linee, rb, lb))
          return true;
        if (Intersect(lines, linee, lt, lb))
          return true;
        */
      }
      return false;
    }
    public static bool PointOnLine(Vector3 pt, Vector3 pt1, Vector3 pt2)
    {
      float dx, dy, dx1, dy1;
      bool retVal;
      dx = pt2.x - pt1.x;
      dy = pt2.z - pt1.z;
      dx1 = pt.x - pt1.x;
      dy1 = pt.z - pt1.z;
      if (pt.x == pt1.x && pt.z == pt1.z || pt.x == pt2.x && pt.z == pt2.z) {//在顶点上
        retVal = true;
      } else {
        if (Math.Abs(dx * dy1 - dy * dx1) < c_FloatPrecision) {//斜率相等//考虑计算误差
          //等于零在顶点上
          if (dx1 * (dx1 - dx) < 0 | dy1 * (dy1 - dy) < 0) {
            retVal = true;
          } else {
            retVal = false;
          }
        } else {
          retVal = false;
        }
      }
      return retVal;
    }
    /// <summary>
    /// 判断两点是否重合
    /// </summary>
    /// <param name="pt0"></param>
    /// <param name="pt1"></param>
    /// <returns></returns>
    public static bool PointOverlapPoint(Vector3 pt0, Vector3 pt1)
    {
      if (Distance(pt0, pt1) < c_MinDistance)
        return true;
      else
        return false;
    }
    public static bool RectangleOverlapRectangle(float _x1, float _y1, float _x2, float _y2, float x1, float y1, float x2, float y2)
    {
      if (_x1 > x2 + c_FloatPrecision)
        return false;
      if (_x2 < x1 - c_FloatPrecision)
        return false;
      if (_y1 > y2 + c_FloatPrecision)
        return false;
      if (_y2 < y1 - c_FloatPrecision)
        return false;
      return true;
    }
    /// <summary>
    /// 判断a是否在bc直线左侧
    /// </summary>
    public static bool PointIsLeft(Vector3 a, Vector3 b, Vector3 c)
    {
      return Multiply(c, a, b) > 0;
    }
    /// <summary>
    /// 判断a是否在bc直线左侧或线上
    /// </summary>
    public static bool PointIsLeftOn(Vector3 a, Vector3 b, Vector3 c)
    {
      return Multiply(c, a, b) >= 0;
    }
    /// <summary>
    /// 判断a、b、c三点是否共线
    /// </summary>
    public static bool PointIsCollinear(Vector3 a, Vector3 b, Vector3 c)
    {
      return Multiply(c, a, b) == 0;
    }
    public static bool IsCounterClockwise(IList<Vector3> pts, int start, int len)
    {
      bool ret = false;
      if (len >= 3) {
        float maxx = pts[start].x;
        float maxz = pts[start].z;
        float minx = maxx;
        float minz = maxz;
        int maxxi = 0;
        int maxzi = 0;
        int minxi = 0;
        int minzi = 0;
        for (int i = 1; i < len; ++i) {
          int ix = start + i;
          float x = pts[ix].x;
          float z = pts[ix].z;
          if (maxx < x) {
            maxx = x;
            maxxi = i;
          } else if (minx > x) {
            minx = x;
            minxi = i;
          }
          if (maxz < z) {
            maxz = z;
            maxzi = i;
          } else if (minz > z) {
            minz = z;
            minzi = i;
          }
        }
        int val = 0;
        for (int delt = 0; delt < len; delt++) {
          int ix0 = start + (len * 2 + maxxi - delt - 1) % len;
          int ix1 = start + maxxi;
          int ix2 = start + (maxxi + delt + 1) % len;

          float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
          if (r > 0) {
            val++;
            break;
          } else if (r < 0) {
            val--;
            break;
          }
        }
        for (int delt = 0; delt < len; delt++) {
          int ix0 = start + (len * 2 + maxzi - delt - 1) % len;
          int ix1 = start + maxzi;
          int ix2 = start + (maxzi + delt + 1) % len;

          float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
          if (r > 0) {
            val++;
            break;
          } else if (r < 0) {
            val--;
            break;
          }
        }
        for (int delt = 0; delt < len; delt++) {
          int ix0 = start + (len * 2 + minxi - delt - 1) % len;
          int ix1 = start + minxi;
          int ix2 = start + (minxi + delt + 1) % len;

          float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
          if (r > 0) {
            val++;
            break;
          } else if (r < 0) {
            val--;
            break;
          }
        }
        for (int delt = 0; delt < len; delt++) {
          int ix0 = start + (len * 2 + minzi - delt - 1) % len;
          int ix1 = start + minzi;
          int ix2 = start + (minzi + delt + 1) % len;

          float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
          if (r > 0) {
            val++;
            break;
          } else if (r < 0) {
            val--;
            break;
          }
        }
        if (val > 0)
          ret=true;
        else
          ret=false;
      }
      return ret;
    }
    /// <summary>
    /// 计算多边形形心与半径
    /// </summary>
    /// <param name="pts"></param>
    /// <param name="start"></param>
    /// <param name="len"></param>
    /// <param name="centroid"></param>
    /// <returns></returns>
    public static float CalcPolygonCentroidAndRadius(IList<Vector3> pts, int start, int len, out Vector3 centroid)
    {
      float ret = 0;
      if (len > 0) {
        float x = 0;
        float z = 0;
        for (int i = 0; i < len; ++i) {
          int ix = start + i;
          x += pts[ix].x;
          z += pts[ix].z;
        }
        x /= len;
        z /= len;
        centroid = new Vector3(x, 0, z);
        float distSqr = 0;
        for (int i = 0; i < len; ++i) {
          int ix = start + i;
          float tmp = Geometry.DistanceSquare(centroid, pts[ix]);
          if (distSqr < tmp)
            distSqr = tmp;
        }
        ret = (float)Math.Sqrt(distSqr);
      } else {
        centroid = new Vector3();
      }
      return ret;
    }
    /// <summary>
    /// 计算多边形轴对齐矩形包围盒
    /// </summary>
    /// <param name="pts"></param>
    /// <param name="start"></param>
    /// <param name="len"></param>
    /// <param name="minXval"></param>
    /// <param name="maxXval"></param>
    /// <param name="minZval"></param>
    /// <param name="maxZval"></param>
    public static void CalcPolygonBound(IList<Vector3> pts, int start, int len, out float minXval, out float maxXval, out float minZval, out float maxZval)
    {
      maxXval = pts[start].x;
      minXval = pts[start].x;
      maxZval = pts[start].z;
      minZval = pts[start].z;
      for (int i = 1; i < len; ++i) {
        int ix = start + i;
        float xv = pts[ix].x;
        float zv = pts[ix].z;
        if (maxXval < xv)
          maxXval = xv;
        else if (minXval > xv)
          minXval = xv;
        if (maxZval < zv)
          maxZval = zv;
        else if (minZval > zv)
          minZval = zv;
      }
    }
    /// <summary>
    /// 判断点是否在多边形内
    /// </summary>
    /// <typeparam name="PointT"></typeparam>
    /// <param name="pt"></param>
    /// <param name="pts"></param>
    /// <param name="start"></param>
    /// <param name="len"></param>
    /// <returns>
    /// 1  -- 在多边形内
    /// 0  -- 在多边形边上
    /// -1 -- 在多边形外 
    /// </returns>
    public static int PointInPolygon(Vector3 pt, IList<Vector3> pts, int start, int len)
    {
      float maxXval;
      float minXval;
      float maxZval;
      float minZval;
      CalcPolygonBound(pts, start, len, out minXval, out maxXval, out minZval, out maxZval);
      return PointInPolygon(pt, pts, start, len, minXval, maxXval, minZval, maxZval);
    }

    public static int PointInPolygon(Vector3 pt, IList<Vector3> pts, int start, int len, float minXval, float maxXval, float minZval, float maxZval)
    {
      if ((pt.x > maxXval) | (pt.x < minXval) | pt.z > maxZval | pt.z < minZval) {
        return -1;//在多边形外
      } else {
        int cnt = 0;
        int lastStatus;
        Vector3 pt0 = pts[start];
        if (pt0.z - pt.z > c_FloatPrecision)
          lastStatus = 1;
        else if (pt0.z - pt.z < c_FloatPrecision)
          lastStatus = -1;
        else
          lastStatus = 0;

        for (int i = 0; i < len; ++i) {
          int ix1 = start + i;
          int ix2 = start + (i + 1) % len;
          Vector3 pt1 = pts[ix1];
          Vector3 pt2 = pts[ix2];

          if (PointOnLine(pt, pt1, pt2))
            return 0;

          int status;
          if (pt2.z - pt.z > c_FloatPrecision)
            status = 1;
          else if (pt2.z - pt.z < c_FloatPrecision)
            status = -1;
          else
            status = 0;

          int temp = status - lastStatus;
          lastStatus = status;
          if (temp > 0) {
            if (!PointIsLeftOn(pt, pt1, pt2))
              cnt += temp;
          } else if (temp < 0) {
            if (PointIsLeft(pt, pt1, pt2))
              cnt += temp;
          }
        }
        if (cnt == 0)
          return -1;
        else
          return 1;
      }
    }

    /// <summary>
    /// 判断一个点与其它三个点形成的三角形之间的关系
    /// 注意：三角形的三个点A B C必须逆时针排列
    ///  -1-在三角形外 0-在三角形内 1-在三角形边ab上 2-在三角形边bc上 3-在三角形边ca上 4-在三角形顶点上
    /// </summary>
    /// <param name="pn">点</param>
    /// <param name="pa">三角形顶点A</param>
    /// <param name="pb">三角形顶点B</param>
    /// <param name="pc">三角形顶点C</param>
    /// <returns>-1-在三角形外 0-在三角形内 1-在三角形边ab上 2-在三角形边bc上 3-在三角形边ca上 4-在三角形顶点上</returns>
    public static int PointInTriangle(Vector3 pn, Vector3 pa, Vector3 pb, Vector3 pc)
    {
      //首先，判断是否在边上
      //注意：三角形的三个点A B C必须逆时针排列
      int num = 0;
      int side = 0;
      if (PointOnLine(pn, pa, pb)) {
        side = 1;
        num++;
      }
      if (PointOnLine(pn, pb, pc)) {
        side = 2;
        num++;
      }
      if (PointOnLine(pn, pc, pa)) {
        side = 3;
        num++;
      }
      if (num > 1)
        return 4;
      else if (num == 1)
        return side;
      else {
        if (SignArea(pn, pa, pb) >= 0.0f && SignArea(pn, pb, pc) >= 0.0f && SignArea(pn, pc, pa) >= 0.0f)
          return 0;
        else
          return -1;
      }
    }
    /// <summary>
    /// 计算三角形带符号面积，输入三角形三个顶点坐标
    /// </summary>
    /// <param name="na"></param>
    /// <param name="nb"></param>
    /// <param name="nc"></param>
    /// <returns></returns>
    public static float SignArea(Vector3 na, Vector3 nb, Vector3 nc)
    {
      return ((nb.x - na.x) * (nc.z - na.z) - (nb.z - na.z) * (nc.x - na.x)) / 2;
    }

    /// <summary>
    /// 将一个多边形或折线抽稀。
    /// </summary>
    /// <typeparam name="PointT"></typeparam>
    /// <param name="pts"></param>
    /// <param name="start"></param>
    /// <param name="len"></param>
    /// <param name="delta">指定的三点共线判断的距离</param>
    /// <returns></returns>
    public static IList<Vector3> Sparseness(IList<Vector3> pts, int start, int len, float delta)
    {
      Vector3 lastPt = pts[0];
      LinkedList<Vector3> links = new LinkedList<Vector3>();
      links.AddLast(lastPt);
      for (int i = 1; i < len; i++) {
        int ix = start + i;
        Vector3 pt = pts[ix];
        if (!PointOverlapPoint(pt, lastPt)) {
          links.AddLast(pt);
        }
        lastPt = pt;
      }
      for (int count = 1; count > 0; ) {
        count = 0;
        LinkedListNode<Vector3> node = links.First;
        while (node != null) {
          LinkedListNode<Vector3> node2 = node.Next;
          LinkedListNode<Vector3> node3 = null;
          if (node2 != null)
            node3 = node2.Next;
          if (node3 != null) {
            double d = PointToLineDistance(node2.Value, node.Value, node3.Value);
            if (d <= delta) {
              links.Remove(node2);
              count++;
            }
          }
          //不管是否删除结点，总是取下一结点，这样可以避免一些将圆弧抽成直线的情形。
          node = node.Next;
        }
      }
      Vector3[] rpts = new Vector3[links.Count];
      links.CopyTo(rpts, 0);
      return rpts;
    }
  }
  //坐标系为左手坐标系，0度角为z轴正方向，旋转方向为z轴正方向向x轴正向旋转（沿y轴正向看逆时针旋转）
  public partial class Geometry
  {
    public static float GetYAngle(Vector2 fvPos1, Vector2 fvPos2)
    {
      float dDistance = (float)Vector2.Distance(fvPos1, fvPos2);
      if (dDistance <= 0.0f)
        return 0.0f;

      float fACos = (fvPos2.y - fvPos1.y) / dDistance;
      if (fACos > 1.0)
        fACos = 0.0f;
      else if (fACos < -1.0)
        fACos = (float)Math.PI;
      else
        fACos = (float)Math.Acos(fACos);

      // [0~180]
      if (fvPos2.x >= fvPos1.x)
        return (float)fACos;
      //(180, 360)
      else
        return (float)(Math.PI * 2 - fACos);
    }
    public static Vector2 GetReflect(Vector2 fvPos1, Vector2 fvPos2, Vector2 v1)
    {
      // pos1 -> pos2
      Vector2 fvNormal = fvPos2 - fvPos1;
      fvNormal.Normalize();
      // pos1 -> v1
      Vector2 fvLine1 = v1 - fvPos1;
      // pos1 -> v2
      Vector2 fvLine2 = fvNormal * (2 * Vector2.Dot(fvLine1, fvNormal)) - fvLine1;
      //return v2
      return fvLine2 + fvPos1;
    }
    public static Vector2 GetRotate(Vector2 fvPos1, float angle)
    {
      // pos1 -> pos2
      Vector2 retV = new Vector2(
          (float)(fvPos1.x * Math.Cos(angle) - fvPos1.y * Math.Sin(angle)),
          (float)(fvPos1.x * Math.Sin(angle) + fvPos1.y * Math.Cos(angle))
          );
      return retV;
    }
    // 获取两向量方向性夹角
    public static float GetIntersectionAngle(Vector2 fromV, Vector2 toV)
    {
      return GetRotateAngle(fromV.x, fromV.y, toV.x, toV.y);
    }
    // 获取两向量方向性夹角
    public static float GetRotateAngle(float x1, float y1, float x2, float y2)
    {
      const float epsilon = 1.0e-6f;
      float nyPI = (float)Math.Acos(-1.0);
      float dist, dot, angle;

      // normalize
      dist = (float)Math.Sqrt(x1 * x1 + y1 * y1);
      x1 /= dist;
      y1 /= dist;
      dist = (float)Math.Sqrt(x2 * x2 + y2 * y2);
      x2 /= dist;
      y2 /= dist;
      // dot product
      dot = x1 * x2 + y1 * y2;
      if (Math.Abs(dot - 1.0) <= epsilon)
        angle = 0.0f;
      else if (Math.Abs(dot + 1.0) <= epsilon)
        angle = nyPI;
      else {
        float cross;

        angle = (float)Math.Acos(dot);
        //cross product
        cross = x1 * y2 - x2 * y1;
        // vector p2 is clockwise from vector p1 
        // with respect to the origin (0.0)
        if (cross < 0) {
          angle = 2 * nyPI - angle;
        }
      }
      //degree = angle *  180.0 / nyPI;
      return angle;
    }

    public static Vector3 GetProjection(Vector3 v, Vector3 n)
    {
      Vector3 vll = new Vector3();
      float distance_power = DistanceSquare(new Vector3(), n);
      if (distance_power <= 0) { 
        return vll; 
      }
      float dot_value = Dot(v, n) / distance_power;
      vll = Multy(n, dot_value);
      return vll;
    }

    //取得v在n的垂直方向上的投影，n为单伙向量
    public static Vector3 GetVtProjection(Vector3 v, Vector3 n)
    {
      Vector3 vt = new Vector3();
      float v_mag = (float)Math.Sqrt(v.x * v.x + v.z * v.z);
      float power_n_mag = n.x * n.x + n.z * n.z;
      float k = (v.x * n.x + v.z * n.z) / (n.x * n.x + n.z * n.z);
      vt.x = v.x - n.x * k;
      vt.z = v.z - n.z * k;
      return vt;
    }

    public static Vector3 Multy(Vector3 vect, float value)
    {
      return new Vector3(vect.x * value, vect.y * value, vect.z * value);
    }

    public static float GetVectorAngle(Vector3 from, Vector3 to)
    {
      float from_mod = GetMod(from);
      float to_mod = GetMod(to);
      if (from_mod * to_mod < 0.001) {
        return 0;
      }
      float cos_value = Dot(from, to) / (from_mod * to_mod);
      if (cos_value > 1) {
        cos_value = 1;
      }
      float degree = (float)Math.Acos(cos_value);
      return degree;
    }
    public static float GetMod(Vector3 v)
    {
      return (float)Math.Sqrt(v.x * v.x + v.z * v.z);
    }

    public static float GetDirFromVector(Vector3 vect)
    {
      return (float)Math.Atan2(vect.x, vect.z);
    }
    public static Vector3 GetVectorFromDir(float dir)
    {
      Vector3 v = new Vector3();
      v.x = (float)Math.Sin(dir);
      v.z = (float)Math.Cos(dir);
      return v;
    }
    public static bool GetRayWithCircleFirstHitPoint(Vector3 ray_start_pos, Vector3 ray_dir, Vector3 center_pos, float radius, out Vector3 hit_point)
    {
      hit_point = new Vector3();
      Vector3 e = new Vector3(center_pos.x - ray_start_pos.x, ray_start_pos.y, center_pos.z - ray_start_pos.z);
      Vector3 d = Normalize(ray_dir);
      float a = Dot(e, d);

      float ray_center_distance = Dot(e, e) - radius * radius;
      if (ray_center_distance < 0) {
        d = new Vector3(-d.x, 0, -d.z);
        a = Dot(e, d);
      }

      float sqrt_value = -ray_center_distance + a * a;
      // no hit point
      if (sqrt_value < 0) {
        return false;
      }

      float t = (float)(a - Math.Sqrt(sqrt_value));
      hit_point.x = ray_start_pos.x + t * d.x;
      hit_point.z = ray_start_pos.z + t * d.z;
      return true;
    }

    public static Vector3 Add(Vector3 v1, Vector3 v2)
    {
      return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static Vector3 GetClostestPointOnCircle(Vector3 q, Vector3 center, float r)
    {
      Vector3 p = new Vector3();
      Vector3 d = new Vector3(center.x - q.x, 0, center.z - q.z);
      if (d.x == 0 && d.z == 0) {
        d.z = 1;
      }
      float d_mod = (float)Math.Sqrt(d.x * d.x + d.z * d.z);
      float k = (d_mod - r) / d_mod;
      p.x = q.x + k * d.x;
      p.z = q.z + k * d.z;
      p.y = center.y;
      return p;
    }

    public static float Dot(Vector3 a, Vector3 b)
    {
      return (a.x * b.x + a.z * b.z);
    }
    public static Vector3 Normalize(Vector3 a)
    {
      Vector3 n = new Vector3();
      float distance = (float)Math.Sqrt(a.x * a.x + a.z * a.z);
      if (distance == 0) {
        return a;
      }
      n.x = a.x / distance;
      n.z = a.z / distance;
      return n;
    }

    public static Vector3 GetVectorDistancePoint(Vector3 start, Vector3 end, float distance_with_start)
    {
      if (Math.Abs(distance_with_start) <= 0.00001) {
        return start;
      }
      Vector3 n = Normalize(end - start);
      Vector3 result = Multy(n, distance_with_start);
      result = Add(start, result);
      result.y = start.y;
      return result;
    }
    public static Vector3 GetRandomPointInCircle(Vector3 center, float radius) 
    {
      float random_radius = RandomUtil.NextFloat() * radius;
      float random_angle = RandomUtil.NextFloat() * (float)Math.PI;
      return new Vector3(center.x + random_radius * (float)Math.Sin(random_angle), center.y, center.z + random_radius * (float)Math.Cos(random_angle));
    }
  }

  /// <summary>
  /// 判断平面线段是否与三角形相交
  /// </summary>
  public partial class Geometry
  {
    private sealed class Line 
    {
      public Line(Vector3 start, Vector3 end) 
      {
        this.Start = new Vector2(start.x, start.z);
        this.End = new Vector2(end.x, end.z);
      }
      public Vector2 Start;
      public Vector2 End;
    }

    /// <summary>  
    /// 判断两条线段是否相交。  
    /// </summary>  
    /// <param name="line1">线段1</param>  
    /// <param name="line2">线段2</param>  
    /// <returns>相交返回真，否则返回假。</returns>  
    public static bool IntersectLine(Vector3 sp, Vector3 ep, Vector3 v0, Vector3 v1, Vector3 v2)  
    {
      Line sample = new Line(sp, ep);
      if (IsIntersect(sample, new Line(v0, v1))) {
        return true;
      }
      if (IsIntersect(sample, new Line(v1, v2))) {
        return true;
      }
      if (IsIntersect(sample, new Line(v2, v0))) {
        return true;
      }
      return false;
    }

    /// <summary>
    /// 判断两个线段是否相交
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <returns>是否相交</returns>
    private static bool IsIntersect(Line one, Line two) 
    {
      return CheckCrose(one, two) && CheckCrose(two, one);
    }

    /// <summary>  
    /// 判断直线2的两点是否在直线1的两边。  
    /// </summary>  
    /// <param name="line1">直线1</param>  
    /// <param name="line2">直线2</param>  
    /// <returns></returns>  
    private static bool CheckCrose(Line line1, Line line2)  
    {
      Vector2 v1 = new Vector2();
      Vector2 v2 = new Vector2();
      Vector2 v3 = new Vector2();
      v1.x = line2.Start.x - line1.End.x;
      v1.y = line2.Start.y - line1.End.y;
      v2.x = line2.End.x - line1.End.x;
      v2.y = line2.End.y - line1.End.y;
      v3.x = line1.Start.x - line1.End.x;
      v3.y = line1.Start.y - line1.End.y;
      return (CrossMul(v1, v3) * CrossMul(v2, v3) <= 0);  
    }  

    /// <summary>  
    /// 计算两个向量的叉乘
    /// </summary>  
    /// <param name="pt1"></param>  
    /// <param name="pt2"></param>  
    /// <returns></returns>  
    private static float CrossMul(Vector2 pt1, Vector2 pt2)  
    {  
      return pt1.x * pt2.y - pt1.y * pt2.x;  
    }
  }
}
