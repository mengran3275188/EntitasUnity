using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Util;
using Util.MyMath;

namespace Spatial
{
  public sealed class TiledData
  {
    public bool IsPolygon
    {
      get { return m_IsPolygon; }
    }
    public List<Vector3> GetPoints()
    {
      return m_PointList;
    }

    public bool CollectDataFromXml(XmlNode node)
    {
      float baseX = 0;
      float baseY = 0;
      List<float> datas = null;
      if (node.Name != "object") {
        return false;
      }
      XmlElement nodeElement = (XmlElement)node;
      baseX = (float)Convert.ToDouble(nodeElement.GetAttribute("x"));
      baseY = (float)Convert.ToDecimal(nodeElement.GetAttribute("y"));

      datas = new List<float>();
      XmlNode polylineData = node.SelectSingleNode("polyline");
      if (polylineData != null) {
        XmlElement polylineElement = (XmlElement)polylineData;
        string pointsData = polylineElement.GetAttribute("points");
        datas = Converter.ConvertNumericList<float>(pointsData);
        m_IsPolygon = false;
      } else {
        XmlNode polygonData = node.SelectSingleNode("polygon");
        if (polygonData != null) {
          XmlElement polygonElement = (XmlElement)polygonData;
          string pointsData = polygonElement.GetAttribute("points");
          datas.AddRange(Converter.ConvertNumericList<float>(pointsData));
          m_IsPolygon = true;
        }
      }
      ParseData(datas, baseX, baseY);
      return true;
    }

    public TiledData(float w, float h)
    {
      map_width_ = w;
      map_height_ = h;
    }

    private void ParseData(List<float> datas,float baseX,float baseY)
    {
      List<Vector3> pts = new List<Vector3>();
      for (int i = 0; i < datas.Count - 1; ++i) {
        Vector3 pos = new Vector3();
        pos.x = (float)(baseX + datas[i]);
        pos.z = map_height_ - (float)(baseY + datas[i + 1]);
        pts.Add(pos);
        i++;
      }
      //抽稀
      IList<Vector3> pts2 = Geometry.Sparseness(pts, 0, pts.Count, 1.0f);
      m_PointList.Clear();
      m_PointList.AddRange(pts2);
      if (m_PointList.Count > 2) {
        //检查是否是逆时针顺序
        if (!Geometry.IsCounterClockwise(m_PointList,0,m_PointList.Count)) {
          pts = m_PointList;
          m_PointList = new List<Vector3>();
          for (int ix = pts.Count - 1; ix >= 0; --ix) {
            m_PointList.Add(pts[ix]);
          }
        }
      }
    }

    private float map_width_ = 0;
    private float map_height_ = 0;
    private bool m_IsPolygon = false;
    private List<Vector3> m_PointList = new List<Vector3>();
  }
}
