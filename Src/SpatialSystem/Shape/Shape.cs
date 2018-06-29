using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace SpatialSystem
{
    public  class BoxCollider
    {
        public Bounds bounds
        {
            get { return m_Bounds; }
        }
        public BoxCollider()
        {
        }
        public BoxCollider(Vector3 center, Vector3 size)
        {
            m_Center = center;
            m_Size = size;
            UpdateBounds();
        }

        public void UpdatePosition(Vector3 center)
        {
            m_Center = center;
            UpdateBounds();
        }
        public void UpdateSize(Vector3 size)
        {
            m_Size = size;
            UpdateBounds();
        }
        public bool Intersects(BoxCollider boxCollider)
        {
            return m_Bounds.Intersects(boxCollider.bounds);
        }
        private void UpdateBounds()
        {
            m_Bounds.center = m_Center;
            m_Bounds.size = m_Size;
        }
        private Vector3 m_Center;
        private Vector3 m_Size;
        private Bounds m_Bounds;
    }
}
