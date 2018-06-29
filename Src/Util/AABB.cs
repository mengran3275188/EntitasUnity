using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    // represents an axis aligned bounding box
    public struct Bounds
    {
        public Vector3 m_Center;

        public Vector3 m_Extents;

        public Bounds(Vector3 center, Vector3 size)
        {
            m_Center = center;
            m_Extents = size * 0.5f;
        }
        public override int GetHashCode()
        {
            return center.GetHashCode() ^ (extents.GetHashCode() << 2);
        }
        public override bool Equals(object other)
        {
            if (!(other is Bounds)) return false;

            Bounds rhs = (Bounds)other;
            return center.Equals(rhs.center) && extents.Equals(rhs.extents);
        }

        public Vector3 center { get { return m_Center; } set { m_Center = value; } }
        public Vector3 size { get { return m_Extents * 2.0f; } set { m_Extents = value * 0.5f; } }
        public Vector3 extents { get { return m_Extents; } set { m_Extents = value; } }
        public Vector3 min { get { return center - extents; } set { SetMinMax(value, max); } }
        public Vector3 max { get { return center + extents; } set { SetMinMax(min, value); } }

        public static bool operator==(Bounds lhs, Bounds rhs)
        {
            return (lhs.center == rhs.center && lhs.extents == rhs.extents);
        }
        public static bool operator!=(Bounds lhs, Bounds rhs)
        {
            return !(lhs == rhs);
        }
        public void SetMinMax(Vector3 min, Vector3 max)
        {
            extents = (max - min) * 0.5f;
            center = min + extents;
        }

        public bool Intersects(Bounds bounds)
        {
            return (min.x <= bounds.max.x) && (max.x > bounds.min.x) && (min.y <= bounds.max.y) && (max.y >= bounds.min.y) &&
                (min.z <= bounds.max.z) && (max.z >= bounds.min.z);
        }
        public override string ToString()
        {
            return String.Format("Center: {0}, Extents : {1}", m_Center, m_Extents);
        }
    }
}
