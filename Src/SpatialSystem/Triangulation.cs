using System;
using System.Collections.Generic;
using System.IO;
using Util;
using Util.MyMath;

namespace Spatial
{
    public class TriangleNode : IGeometry
    {
        public int Id = 0;
        public Vector3[] Points = new Vector3[3];
        public TriangleNode[] Neighbors = new TriangleNode[3];
        public int[] NeighborsId = new int[3];
        public Vector3 Position { get; internal set; }
        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinZ { get; private set; }
        public float MaxZ { get; private set; }
        public void CalcGeometryInfo()
        {
            Position = new Vector3((Points[0].x + Points[1].x + Points[2].x) / 3, 0, (Points[0].z + Points[1].z + Points[2].z) / 3);
            float minX, minZ, maxX, maxZ;
            minX = maxX = Points[0].x;
            minZ = maxZ = Points[0].z;
            for (int i = 1; i < 3; ++i)
            {
                if (minX > Points[i].x)
                    minX = Points[i].x;
                else if (maxX < Points[i].x)
                    maxX = Points[i].x;
                if (minZ > Points[i].z)
                    minZ = Points[i].z;
                else if (maxZ < Points[i].z)
                    maxZ = Points[i].z;
            }
            MinX = minX;
            MaxX = maxX;
            MinZ = minZ;
            MaxZ = maxZ;
        }
        public bool Indexed { get; set; }

        public void Read(BinaryReader binReader, float scale)
        {
            Id = binReader.ReadInt32();
            // 读取多边形的顶点//转换为顺时针
            Points[0] = new Vector3(binReader.ReadSingle() * scale / 100.0f, 0f, binReader.ReadSingle() * scale / 100.0f);
            Points[2] = new Vector3(binReader.ReadSingle() * scale / 100.0f, 0f, binReader.ReadSingle() * scale / 100.0f);
            Points[1] = new Vector3(binReader.ReadSingle() * scale / 100.0f, 0f, binReader.ReadSingle() * scale / 100.0f);

            // 读取邻居节点
            for (int neighborId = 0; neighborId < 3; neighborId++)
            {
                this.NeighborsId[neighborId] = binReader.ReadInt32();
            }
        }
    }
    //todo:支持动态添加新的多边形
    public class Triangulation
    {
        private struct point_t
        {
            internal double x;
            internal double y;
        }
        private class segment_t
        {
            internal point_t v0;
            internal point_t v1;
            internal bool is_inserted;
            internal int root0;
            internal int root1;
            internal int next;
            internal int prev;

            internal void CopyFrom(segment_t other)
            {
                v0 = other.v0;
                v1 = other.v1;
                is_inserted = other.is_inserted;
                root0 = other.root0;
                root1 = other.root1;
                next = other.next;
                prev = other.prev;
            }
        }
        private class trap_t
        {
            internal int lseg;
            internal int rseg;
            internal point_t hi;
            internal point_t lo;
            internal int u0;
            internal int u1;
            internal int d0;
            internal int d1;
            internal int sink;
            internal int usave;
            internal int uside;
            internal int state;

            internal void CopyFrom(trap_t other)
            {
                lseg = other.lseg;
                rseg = other.rseg;
                hi = other.hi;
                lo = other.lo;
                u0 = other.u0;
                u1 = other.u1;
                d0 = other.d0;
                d1 = other.d1;
                sink = other.sink;
                usave = other.usave;
                uside = other.uside;
                state = other.state;
            }
        }
        private class node_t
        {
            internal int nodetype;
            internal int segnum;
            internal point_t yval;
            internal int trnum;
            internal int parent;
            internal int left;
            internal int right;

            internal void CopyFrom(node_t other)
            {
                nodetype = other.nodetype;
                segnum = other.segnum;
                yval = other.yval;
                trnum = other.trnum;
                parent = other.parent;
                left = other.left;
                right = other.right;
            }
        }
        private class monchain_t
        {
            internal int vnum;
            internal int next;
            internal int prev;
            internal bool marked;

            internal void CopyFrom(monchain_t other)
            {
                vnum = other.vnum;
                next = other.next;
                prev = other.prev;
                marked = other.marked;
            }
        }
        private class vertexchain_t
        {
            internal point_t pt;
            internal int[] vnext;
            internal int[] vpos;
            internal int nextfree;

            internal vertexchain_t()
            {
                vnext = new int[4];
                vpos = new int[4];
            }
            internal void CopyFrom(vertexchain_t other)
            {
                pt = other.pt;
                Array.Copy(other.vnext, vnext, vnext.Length);
                Array.Copy(other.vpos, vpos, vpos.Length);
                nextfree = other.nextfree;
            }
        }
        private class triangle_t
        {
            internal TriangleNode triangle;
            internal uint[] codes = new uint[3];
        }

        public List<IList<Vector3>> PreTriangles
        {
            get { return pre_triangles; }
        }
        public IList<TriangleNode> Load(string filename, float scale)
        {
            bool walkAreaExist = false;
            List<IList<Vector3>> ptss = new List<IList<Vector3>>();
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                while (fs.Position < fs.Length)
                {
                    List<Vector3> pts = new List<Vector3>();
                    int type = fs.ReadByte();
                    int ct = ReadInt(fs);
                    for (int i = 0; i < ct; i++)
                    {
                        int x = ReadInt(fs);
                        int z = ReadInt(fs);
                        Vector3 pt = new Vector3((float)x * scale / 100.0f, 0, (float)z * scale / 100.0f);
                        pts.Add(pt);
                    }
                    if (ct < 3)
                        continue;

                    if (type == (int)TiledMapParser.GeometryTypeEnum.WalkArea)
                    {
                        List<Vector3> pts2 = new List<Vector3>();
                        for (int i = pts.Count - 1; i >= 0; --i)
                            pts2.Add(pts[i]);

                        if (!walkAreaExist)
                            ptss.Add(pts2);
                        //walkAreaExist = true;
                    }
                    else if (type == (int)TiledMapParser.GeometryTypeEnum.ObstacleArea)
                    {
                        //阻挡多边形的顶点按顺时针序排列
                        ptss.Add(pts);
                    }
                    else if (type == (int)TiledMapParser.GeometryTypeEnum.WalkTriangle)
                    {
                        pre_triangles.Add(pts);
                    }
                }

                fs.Close();
            }
            return Build(ptss.ToArray());
        }
        public IList<TriangleNode> Build(params IList<Vector3>[] ptss)
        {
            results.Clear();
            triangles.Clear();
            triangle_neighbors.Clear();

            int segnum = 0;
            int num = ptss.Length;
            for (int ccount = 0; ccount < num; ++ccount)
            {
                segnum += ptss[ccount].Count;
            }
            int trinum = pre_triangles.Count;
            for (int ix = 0; ix < trinum; ++ix)
            {
                segnum += pre_triangles[ix].Count;
            }
            seg.Clear();
            for (int ix = 0; ix <= segnum; ++ix)
            {
                seg.Add(new segment_t());
            }

            int i = 1;
            for (int ccount = 0; ccount < num; ++ccount)
            {

                IList<Vector3> pts = ptss[ccount];
                int npoints = pts.Count;
                int first = i;
                int last = first + npoints - 1;

                for (int j = 0; j < npoints; ++j, ++i)
                {
                    seg[i].v0.x = pts[j].x;
                    seg[i].v0.y = pts[j].z;

                    if (i == last)
                    {
                        seg[i].next = first;
                        seg[i].prev = i - 1;
                        seg[i - 1].v1 = seg[i].v0;
                    }
                    else if (i == first)
                    {
                        seg[i].next = i + 1;
                        seg[i].prev = last;
                        seg[last].v1 = seg[i].v0;
                    }
                    else
                    {
                        seg[i].prev = i - 1;
                        seg[i].next = i + 1;
                        seg[i - 1].v1 = seg[i].v0;
                    }

                    seg[i].is_inserted = false;
                }
            }
            //在进行行走区三角剖分时，预先指定的三角形行走区按阻挡先处理（亦即从行走区排除，之后再加回来），阻挡多边形的顶点按顺时针序排列
            for (int ccount = 0; ccount < trinum; ++ccount)
            {

                IList<Vector3> pts = pre_triangles[ccount];
                int npoints = pts.Count;
                int first = i;
                int last = first + npoints - 1;

                for (int j = npoints - 1; j >= 0; --j, ++i)
                {
                    seg[i].v0.x = pts[j].x;
                    seg[i].v0.y = pts[j].z;

                    if (i == last)
                    {
                        seg[i].next = first;
                        seg[i].prev = i - 1;
                        seg[i - 1].v1 = seg[i].v0;
                    }
                    else if (i == first)
                    {
                        seg[i].next = i + 1;
                        seg[i].prev = last;
                        seg[last].v1 = seg[i].v0;
                    }
                    else
                    {
                        seg[i].prev = i - 1;
                        seg[i].next = i + 1;
                        seg[i - 1].v1 = seg[i].v0;
                    }

                    seg[i].is_inserted = false;

                    if (j < npoints - 2)
                    {
                        add_triangle(first, i, i - 1);
                    }
                }
            }

            /*
            int n = i - 1;
            initialise(n);
            construct_trapezoids(n);

            int nmonpoly = monotonate_trapezoids(n);
            int tnum = triangulate_monotone_polygons(n, nmonpoly);
            */

            //建立三角形网的邻接关系
            foreach (KeyValuePair<uint, List<triangle_t>> pair in triangle_neighbors)
            {
                uint code = pair.Key;
                List<triangle_t> tris = pair.Value;
                if (tris.Count == 2)
                {
                    set_triangle_neighbor(code, tris[0], tris[1].triangle);
                    set_triangle_neighbor(code, tris[1], tris[0].triangle);
                }
                else if (tris.Count > 2)
                {
                    LogUtil.Error("repeated triangle !!!");
                }
            }

            IList<TriangleNode> ret = results.ToArray();
            permute.Clear();
            seg.Clear();
            qs.Clear();
            tr.Clear();

            vert.Clear();
            mchain.Clear();
            mon.Clear();
            visited.Clear();

            results.Clear();
            triangles.Clear();
            triangle_neighbors.Clear();
            return ret;
        }

        private void initialise(int n)
        {
            for (int i = 1; i <= n; ++i)
                seg[i].is_inserted = false;

            generate_random_ordering(n);
        }
        /* Main routine to perform trapezoidation */
        private void construct_trapezoids(int nseg)
        {
            int i;
            int root, h;
            /* Add the first segment and get the query structure and trapezoid */
            /* list initialised */
            root = init_query_structure(choose_segment());

            for (i = 1; i <= nseg; i++)
                seg[i].root0 = seg[i].root1 = root;

            for (h = 1; h <= math_logstar_n(nseg); h++)
            {
                for (i = math_N(nseg, h - 1) + 1; i <= math_N(nseg, h); i++)
                    add_segment(choose_segment());

                /* Find a new root for each of the segment endpoints */
                for (i = 1; i <= nseg; i++)
                    find_new_roots(i);
            }

            for (i = math_N(nseg, math_logstar_n(nseg)) + 1; i <= nseg; i++)
                add_segment(choose_segment());
        }
        /* Main routine to get monotone polygons from the trapezoidation of 
         * the polygon.
         */
        private int monotonate_trapezoids(int n)
        {
            int i;
            int tr_start;

            vert.Clear();
            for (int ix = 0; ix < seg.Count; ++ix)
            {
                vert.Add(new vertexchain_t());
            }
            visited.Clear();
            for (int ix = 0; ix < tr.Count; ++ix)
            {
                visited.Add(false);
            }
            mchain.Clear();
            for (int ix = 0; ix < tr.Count; ++ix)
            {
                mchain.Add(new monchain_t());
            }
            mon.Clear();
            for (int ix = 0; ix < seg.Count; ++ix)
            {
                mon.Add(0);
            }

            /* First locate a trapezoid which lies inside the polygon */
            /* and which is triangular */
            for (i = 0; i < tr.Count; i++)
                if (inside_polygon(tr[i]))
                    break;
            tr_start = i;

            /* Initialise the mon data-structure and start spanning all the */
            /* trapezoids within the polygon */

#if false
      for (i = 1; i <= n; i++)
      {
        mchain[i].prev = i - 1;
        mchain[i].next = i + 1;
        mchain[i].vnum = i;
        vert[i].pt = seg[i].v0;
        vert[i].vnext[0] = i + 1;	/* next vertex */
        vert[i].vpos[0] = i;	    /* locn. of next vertex */
        vert[i].nextfree = 1;
      }
      mchain[1].prev = n;
      mchain[n].next = 1;
      vert[n].vnext[0] = 1;
      vert[n].vpos[0] = n;
      chain_idx = n;
      mon_idx = 0;
      mon[0] = 1;			/* position of any vertex in the first */
				              /* chain  */
#else

            for (i = 1; i <= n; i++)
            {
                mchain[i].prev = seg[i].prev;
                mchain[i].next = seg[i].next;
                mchain[i].vnum = i;
                vert[i].pt = seg[i].v0;
                vert[i].vnext[0] = seg[i].next; /* next vertex */
                vert[i].vpos[0] = i;              /* locn. of next vertex */
                vert[i].nextfree = 1;
            }

            chain_idx = n;
            mon_idx = 0;
            mon[0] = 1;         /* position of any vertex in the first */
                                /* chain  */

#endif

            /* traverse the polygon */
            if (tr[tr_start].u0 > 0)
                traverse_polygon(0, tr_start, tr[tr_start].u0, TR_FROM_UP);
            else if (tr[tr_start].d0 > 0)
                traverse_polygon(0, tr_start, tr[tr_start].d0, TR_FROM_DN);

            /* return the number of polygons created */
            return newmon();
        }
        /* For each monotone polygon, find the ymax and ymin (to determine the */
        /* two y-monotone chains) and pass on this monotone polygon for greedy */
        /* triangulation. */
        /* Take care not to triangulate duplicate monotone polygons */
        private int triangulate_monotone_polygons(int nvert, int nmonpoly)
        {
            point_t ymax, ymin;
            int p, vfirst, posmax, posmin, v;
            int vcount;
            bool processed = false;

            for (int i = 0; i < nmonpoly; i++)
            {
                vcount = 1;
                processed = false;
                vfirst = mchain[mon[i]].vnum;
                ymax = ymin = vert[vfirst].pt;
                posmax = posmin = mon[i];
                mchain[mon[i]].marked = true;
                p = mchain[mon[i]].next;
                while ((v = mchain[p].vnum) != vfirst)
                {
                    if (mchain[p].marked)
                    {
                        processed = true;
                        break;      /* break from while */
                    }
                    else
                        mchain[p].marked = true;

                    if (_greater_than(vert[v].pt, ymax))
                    {
                        ymax = vert[v].pt;
                        posmax = p;
                    }
                    if (_less_than(vert[v].pt, ymin))
                    {
                        ymin = vert[v].pt;
                        posmin = p;
                    }
                    p = mchain[p].next;
                    vcount++;
                }

                if (processed) /* Go to next polygon */
                    continue;

                if (vcount == 3) /* already a triangle */
                {
                    int pt1 = mchain[p].vnum;
                    int pt2 = mchain[mchain[p].next].vnum;
                    int pt3 = mchain[mchain[p].prev].vnum;
                    add_triangle(pt1, pt2, pt3);
                }
                else
                {   /* triangulate the polygon */
                    v = mchain[mchain[posmax].next].vnum;
                    if (_equal_to(vert[v].pt, ymin))
                    {   /* LHS is a single line */
                        triangulate_single_polygon(nvert, posmax, TRI_LHS);
                    }
                    else
                        triangulate_single_polygon(nvert, posmax, TRI_RHS);
                }
            }

            return triangles.Count;
        }

        /* (v0, v1) is the new diagonal to be added to the polygon. Find which */
        /* chain to use and return the positions of v0 and v1 in p and q */
        private void get_vertex_positions(int v0, int v1, out int ip, out int iq)
        {
            vertexchain_t vp0, vp1;
            int i;
            double angle, temp;
            int tp = 0, tq = 0;

            vp0 = vert[v0];
            vp1 = vert[v1];

            /* p is identified as follows. Scan from (v0, v1) rightwards till */
            /* you hit the first segment starting from v0. That chain is the */
            /* chain of our interest */

            angle = -4.0;
            for (i = 0; i < 4; i++)
            {
                if (vp0.vnext[i] <= 0)
                    continue;
                if ((temp = get_angle(vp0.pt, (vert[vp0.vnext[i]].pt), vp1.pt)) > angle)
                {
                    angle = temp;
                    tp = i;
                }
            }

            ip = tp;

            /* Do similar actions for q */

            angle = -4.0;
            for (i = 0; i < 4; i++)
            {
                if (vp1.vnext[i] <= 0)
                    continue;
                if ((temp = get_angle(vp1.pt, (vert[vp1.vnext[i]].pt), vp0.pt)) > angle)
                {
                    angle = temp;
                    tq = i;
                }
            }

            iq = tq;
        }
        /* v0 and v1 are specified in anti-clockwise order with respect to 
         * the current monotone polygon mcur. Split the current polygon into 
         * two polygons using the diagonal (v0, v1) 
         */
        private int make_new_monotone_poly(int mcur, int v0, int v1)
        {
            int p, q, ip, iq;
            int mnew = newmon();
            int i, j, nf0, nf1;
            vertexchain_t vp0, vp1;

            vp0 = vert[v0];
            vp1 = vert[v1];

            get_vertex_positions(v0, v1, out ip, out iq);

            p = vp0.vpos[ip];
            q = vp1.vpos[iq];

            /* At this stage, we have got the positions of v0 and v1 in the */
            /* desired chain. Now modify the linked lists */

            i = new_chain_element();    /* for the new list */
            j = new_chain_element();

            mchain[i].vnum = v0;
            mchain[j].vnum = v1;

            mchain[i].next = mchain[p].next;
            mchain[mchain[p].next].prev = i;
            mchain[i].prev = j;
            mchain[j].next = i;
            mchain[j].prev = mchain[q].prev;
            mchain[mchain[q].prev].next = j;

            mchain[p].next = q;
            mchain[q].prev = p;

            nf0 = vp0.nextfree;
            nf1 = vp1.nextfree;

            vp0.vnext[ip] = v1;

            vp0.vpos[nf0] = i;
            vp0.vnext[nf0] = mchain[mchain[i].next].vnum;
            vp1.vpos[nf1] = j;
            vp1.vnext[nf1] = v0;

            vp0.nextfree++;
            vp1.nextfree++;

            mon[mcur] = p;
            mon[mnew] = i;
            return mnew;
        }
        /* recursively visit all the trapezoids */
        private int traverse_polygon(int mcur, int trnum, int from, int dir)
        {
            int mnew;
            int v0, v1;
            int retval = 0;
            bool do_switch = false;

            if ((trnum <= 0) || visited[trnum])
                return 0;
            visited[trnum] = true;
            trap_t t = tr[trnum];

            /* We have much more information available here. */
            /* rseg: goes upwards   */
            /* lseg: goes downwards */

            /* Initially assume that dir = TR_FROM_DN (from the left) */
            /* Switch v0 and v1 if necessary afterwards */


            /* special cases for triangles with cusps at the opposite ends. */
            /* take care of this first */
            if ((t.u0 <= 0) && (t.u1 <= 0))
            {
                if ((t.d0 > 0) && (t.d1 > 0))
                { /* downward opening triangle */
                    v0 = tr[t.d1].lseg;
                    v1 = t.lseg;
                    if (from == t.d1)
                    {
                        do_switch = true;
                        mnew = make_new_monotone_poly(mcur, v1, v0);
                        traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                        traverse_polygon(mnew, t.d0, trnum, TR_FROM_UP);
                    }
                    else
                    {
                        mnew = make_new_monotone_poly(mcur, v0, v1);
                        traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                        traverse_polygon(mnew, t.d1, trnum, TR_FROM_UP);
                    }
                }
                else
                {
                    retval = SP_NOSPLIT;    /* Just traverse all neighbours */
                    traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                    traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                    traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                    traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                }
            }
            else if ((t.d0 <= 0) && (t.d1 <= 0))
            {
                if ((t.u0 > 0) && (t.u1 > 0))
                { /* upward opening triangle */
                    v0 = t.rseg;
                    v1 = tr[t.u0].rseg;
                    if (from == t.u1)
                    {
                        do_switch = true;
                        mnew = make_new_monotone_poly(mcur, v1, v0);
                        traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                        traverse_polygon(mnew, t.u0, trnum, TR_FROM_DN);
                    }
                    else
                    {
                        mnew = make_new_monotone_poly(mcur, v0, v1);
                        traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                        traverse_polygon(mnew, t.u1, trnum, TR_FROM_DN);
                    }
                }
                else
                {
                    retval = SP_NOSPLIT;    /* Just traverse all neighbours */
                    traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                    traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                    traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                    traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                }
            }
            else if ((t.u0 > 0) && (t.u1 > 0))
            {
                if ((t.d0 > 0) && (t.d1 > 0))
                { /* downward + upward cusps */
                    v0 = tr[t.d1].lseg;
                    v1 = tr[t.u0].rseg;
                    retval = SP_2UP_2DN;
                    if (((dir == TR_FROM_DN) && (t.d1 == from)) ||
                        ((dir == TR_FROM_UP) && (t.u1 == from)))
                    {
                        do_switch = true;
                        mnew = make_new_monotone_poly(mcur, v1, v0);
                        traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                        traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                        traverse_polygon(mnew, t.u0, trnum, TR_FROM_DN);
                        traverse_polygon(mnew, t.d0, trnum, TR_FROM_UP);
                    }
                    else
                    {
                        mnew = make_new_monotone_poly(mcur, v0, v1);
                        traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                        traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                        traverse_polygon(mnew, t.u1, trnum, TR_FROM_DN);
                        traverse_polygon(mnew, t.d1, trnum, TR_FROM_UP);
                    }
                }
                else
                {   /* only downward cusp */
                    if (_equal_to(t.lo, seg[t.lseg].v1))
                    {
                        v0 = tr[t.u0].rseg;
                        v1 = seg[t.lseg].next;

                        retval = SP_2UP_LEFT;
                        if ((dir == TR_FROM_UP) && (t.u0 == from))
                        {
                            do_switch = true;
                            mnew = make_new_monotone_poly(mcur, v1, v0);
                            traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.d0, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.u1, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.d1, trnum, TR_FROM_UP);
                        }
                        else
                        {
                            mnew = make_new_monotone_poly(mcur, v0, v1);
                            traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                            traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                            traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.u0, trnum, TR_FROM_DN);
                        }
                    }
                    else
                    {
                        v0 = t.rseg;
                        v1 = tr[t.u0].rseg;
                        retval = SP_2UP_RIGHT;
                        if ((dir == TR_FROM_UP) && (t.u1 == from))
                        {
                            do_switch = true;
                            mnew = make_new_monotone_poly(mcur, v1, v0);
                            traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.d1, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.d0, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.u0, trnum, TR_FROM_DN);
                        }
                        else
                        {
                            mnew = make_new_monotone_poly(mcur, v0, v1);
                            traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                            traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.u1, trnum, TR_FROM_DN);
                        }
                    }
                }
            }
            else if ((t.u0 > 0) || (t.u1 > 0))
            { /* no downward cusp */
                if ((t.d0 > 0) && (t.d1 > 0))
                { /* only upward cusp */
                    if (_equal_to(t.hi, seg[t.lseg].v0))
                    {
                        v0 = tr[t.d1].lseg;
                        v1 = t.lseg;
                        retval = SP_2DN_LEFT;
                        if (!((dir == TR_FROM_DN) && (t.d0 == from)))
                        {
                            do_switch = true;
                            mnew = make_new_monotone_poly(mcur, v1, v0);
                            traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                            traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                            traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.d0, trnum, TR_FROM_UP);
                        }
                        else
                        {
                            mnew = make_new_monotone_poly(mcur, v0, v1);
                            traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.u1, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.d1, trnum, TR_FROM_UP);
                        }
                    }
                    else
                    {
                        v0 = tr[t.d1].lseg;
                        v1 = seg[t.rseg].next;

                        retval = SP_2DN_RIGHT;
                        if ((dir == TR_FROM_DN) && (t.d1 == from))
                        {
                            do_switch = true;
                            mnew = make_new_monotone_poly(mcur, v1, v0);
                            traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.u1, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.d0, trnum, TR_FROM_UP);
                        }
                        else
                        {
                            mnew = make_new_monotone_poly(mcur, v0, v1);
                            traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                            traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.d1, trnum, TR_FROM_UP);
                        }
                    }
                }
                else
                {   /* no cusp */
                    if (_equal_to(t.hi, seg[t.lseg].v0) &&
                        _equal_to(t.lo, seg[t.rseg].v0))
                    {
                        v0 = t.rseg;
                        v1 = t.lseg;
                        retval = SP_SIMPLE_LRDN;
                        if (dir == TR_FROM_UP)
                        {
                            do_switch = true;
                            mnew = make_new_monotone_poly(mcur, v1, v0);
                            traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.d1, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.d0, trnum, TR_FROM_UP);
                        }
                        else
                        {
                            mnew = make_new_monotone_poly(mcur, v0, v1);
                            traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                            traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.u1, trnum, TR_FROM_DN);
                        }
                    }
                    else if (_equal_to(t.hi, seg[t.rseg].v1) &&
                       _equal_to(t.lo, seg[t.lseg].v1))
                    {
                        v0 = seg[t.rseg].next;
                        v1 = seg[t.lseg].next;

                        retval = SP_SIMPLE_LRUP;
                        if (dir == TR_FROM_UP)
                        {
                            do_switch = true;
                            mnew = make_new_monotone_poly(mcur, v1, v0);
                            traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.d1, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.d0, trnum, TR_FROM_UP);
                        }
                        else
                        {
                            mnew = make_new_monotone_poly(mcur, v0, v1);
                            traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                            traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                            traverse_polygon(mnew, t.u0, trnum, TR_FROM_DN);
                            traverse_polygon(mnew, t.u1, trnum, TR_FROM_DN);
                        }
                    }
                    else
                    {   /* no split possible */
                        retval = SP_NOSPLIT;
                        traverse_polygon(mcur, t.u0, trnum, TR_FROM_DN);
                        traverse_polygon(mcur, t.d0, trnum, TR_FROM_UP);
                        traverse_polygon(mcur, t.u1, trnum, TR_FROM_DN);
                        traverse_polygon(mcur, t.d1, trnum, TR_FROM_UP);
                    }
                }
            }

            return retval;
        }
        /* A greedy corner-cutting algorithm to triangulate a y-monotone 
         * polygon in O(n) time.
         * Joseph O-Rourke, Computational Geometry in C.
         */
        private void triangulate_single_polygon(int nvert, int posmax, int side)
        {
            int v;
            int ri = 0; /* reflex chain */
            int[] rc = new int[seg.Count];
            int endv, tmp, vpos;

            if (side == TRI_RHS)
            {   /* RHS segment is a single segment */
                rc[0] = mchain[posmax].vnum;
                tmp = mchain[posmax].next;
                rc[1] = mchain[tmp].vnum;
                ri = 1;

                vpos = mchain[tmp].next;
                v = mchain[vpos].vnum;

                if ((endv = mchain[mchain[posmax].prev].vnum) == 0)
                    endv = nvert;
            }
            else
            {   /* LHS is a single segment */
                tmp = mchain[posmax].next;
                rc[0] = mchain[tmp].vnum;
                tmp = mchain[tmp].next;
                rc[1] = mchain[tmp].vnum;
                ri = 1;

                vpos = mchain[tmp].next;
                v = mchain[vpos].vnum;

                endv = mchain[posmax].vnum;
            }

            while ((v != endv) || (ri > 1))
            {
                if (ri > 0)
                {   /* reflex chain is non-empty */
                    if (cross(vert[v].pt, vert[rc[ri - 1]].pt,
                        vert[rc[ri]].pt) > 0)
                    {           /* convex corner: cut if off */
                        int pt1 = rc[ri - 1];
                        int pt2 = rc[ri];
                        int pt3 = v;
                        add_triangle(pt1, pt2, pt3);
                        ri--;
                    }
                    else
                    {   /* non-convex */
                        /* add v to the chain */
                        ri++;
                        rc[ri] = v;
                        vpos = mchain[vpos].next;
                        v = mchain[vpos].vnum;
                    }
                }
                else
                {   /* reflex-chain empty: add v to the */
                    /* reflex chain and advance it  */
                    rc[++ri] = v;
                    vpos = mchain[vpos].next;
                    v = mchain[vpos].vnum;
                }
            } /* end-while */

            /* reached the bottom vertex. Add in the triangle formed */
            int ept1 = rc[ri - 1];
            int ept2 = rc[ri];
            int ept3 = v;
            add_triangle(ept1, ept2, ept3);
            ri--;
        }

        /* Initilialise the query structure (Q) and the trapezoid table (T) 
        * when the first segment is added to start the trapezoidation. The
        * query-tree starts out with 4 trapezoids, one S-node and 2 Y-nodes
        *    
        *                4
        *   -----------------------------------
        *  		  \
        *  	1	   \        2
        *  		    \
        *   -----------------------------------
        *                3
        */
        private int init_query_structure(int segnum)
        {
            int i1, i2, i3, i4, i5, i6, i7, root;
            int t1, t2, t3, t4;
            segment_t s = seg[segnum];

            q_idx = tr_idx = 1;
            tr.Clear();
            for (int i = 0; i <= 4 * seg.Count; ++i)
            {
                tr.Add(new trap_t());
            }
            qs.Clear();
            for (int i = 0; i <= 8 * seg.Count; ++i)
            {
                qs.Add(new node_t());
            }

            i1 = newnode();
            qs[i1].nodetype = T_Y;
            qs[i1].yval = _max(s.v0, s.v1); /* root */
            root = i1;

            qs[i1].right = i2 = newnode();
            qs[i2].nodetype = T_SINK;
            qs[i2].parent = i1;

            qs[i1].left = i3 = newnode();
            qs[i3].nodetype = T_Y;
            qs[i3].yval = _min(s.v0, s.v1); /* root */
            qs[i3].parent = i1;

            qs[i3].left = i4 = newnode();
            qs[i4].nodetype = T_SINK;
            qs[i4].parent = i3;

            qs[i3].right = i5 = newnode();
            qs[i5].nodetype = T_X;
            qs[i5].segnum = segnum;
            qs[i5].parent = i3;

            qs[i5].left = i6 = newnode();
            qs[i6].nodetype = T_SINK;
            qs[i6].parent = i5;

            qs[i5].right = i7 = newnode();
            qs[i7].nodetype = T_SINK;
            qs[i7].parent = i5;

            t1 = newtrap();     /* middle left */
            t2 = newtrap();     /* middle right */
            t3 = newtrap();     /* bottom-most */
            t4 = newtrap();     /* topmost */

            tr[t1].hi = tr[t2].hi = tr[t4].lo = qs[i1].yval;
            tr[t1].lo = tr[t2].lo = tr[t3].hi = qs[i3].yval;
            tr[t4].hi.y = (double)(INFINITY);
            tr[t4].hi.x = (double)(INFINITY);
            tr[t3].lo.y = (double)-1 * (INFINITY);
            tr[t3].lo.x = (double)-1 * (INFINITY);
            tr[t1].rseg = tr[t2].lseg = segnum;
            tr[t1].u0 = tr[t2].u0 = t4;
            tr[t1].d0 = tr[t2].d0 = t3;
            tr[t4].d0 = tr[t3].u0 = t1;
            tr[t4].d1 = tr[t3].u1 = t2;

            tr[t1].sink = i6;
            tr[t2].sink = i7;
            tr[t3].sink = i4;
            tr[t4].sink = i2;

            tr[t1].state = tr[t2].state = ST_VALID;
            tr[t3].state = tr[t4].state = ST_VALID;

            qs[i2].trnum = t4;
            qs[i4].trnum = t3;
            qs[i6].trnum = t1;
            qs[i7].trnum = t2;

            s.is_inserted = true;
            return root;
        }
        /* Add in the new segment into the trapezoidation and update Q and T
        * structures. First locate the two endpoints of the segment in the
        * Q-structure. Then start from the topmost trapezoid and go down to
        * the  lower trapezoid dividing all the trapezoids in between .
        */
        private void add_segment(int segnum)
        {
            int tu = 0, tl = 0, sk = 0, tfirst = 0, tlast = 0, tnext = 0;
            int tfirstr = 0, tlastr = 0, tfirstl = 0, tlastl = 0;
            int i1 = 0, i2 = 0, t = 0, tn = 0;
            point_t tpt;
            bool tritop = false, tribot = false;
            bool is_swapped = false;
            int tmptriseg = 0;

            segment_t s = new segment_t();
            s.CopyFrom(seg[segnum]);
            if (_greater_than(s.v1, s.v0))
            { /* Get higher vertex in v0 */
                int tmp;
                tpt = s.v0;
                s.v0 = s.v1;
                s.v1 = tpt;
                tmp = s.root0;
                s.root0 = s.root1;
                s.root1 = tmp;
                is_swapped = true;
            }

            if ((is_swapped) ? !inserted(segnum, LASTPT) : !inserted(segnum, FIRSTPT))
            { /* insert v0 in the tree */
                int tmp_d;

                tu = locate_endpoint(s.v0, s.v1, s.root0);
                tl = newtrap();     /* tl is the new lower trapezoid */
                tr[tl].state = ST_VALID;
                tr[tl].CopyFrom(tr[tu]);
                tr[tu].lo.y = tr[tl].hi.y = s.v0.y;
                tr[tu].lo.x = tr[tl].hi.x = s.v0.x;
                tr[tu].d0 = tl;
                tr[tu].d1 = 0;
                tr[tl].u0 = tu;
                tr[tl].u1 = 0;

                if (((tmp_d = tr[tl].d0) > 0) && (tr[tmp_d].u0 == tu))
                    tr[tmp_d].u0 = tl;
                if (((tmp_d = tr[tl].d0) > 0) && (tr[tmp_d].u1 == tu))
                    tr[tmp_d].u1 = tl;

                if (((tmp_d = tr[tl].d1) > 0) && (tr[tmp_d].u0 == tu))
                    tr[tmp_d].u0 = tl;
                if (((tmp_d = tr[tl].d1) > 0) && (tr[tmp_d].u1 == tu))
                    tr[tmp_d].u1 = tl;

                /* Now update the query structure and obtain the sinks for the */
                /* two trapezoids */

                i1 = newnode();     /* Upper trapezoid sink */
                i2 = newnode();     /* Lower trapezoid sink */
                sk = tr[tu].sink;

                qs[sk].nodetype = T_Y;
                qs[sk].yval = s.v0;
                qs[sk].segnum = segnum; /* not really reqd ... maybe later */
                qs[sk].left = i2;
                qs[sk].right = i1;

                qs[i1].nodetype = T_SINK;
                qs[i1].trnum = tu;
                qs[i1].parent = sk;

                qs[i2].nodetype = T_SINK;
                qs[i2].trnum = tl;
                qs[i2].parent = sk;

                tr[tu].sink = i1;
                tr[tl].sink = i2;
                tfirst = tl;
            }
            else
            {   /* v0 already present */
                /* Get the topmost intersecting trapezoid */
                tfirst = locate_endpoint(s.v0, s.v1, s.root0);
                tritop = true;
            }

            if ((is_swapped) ? !inserted(segnum, FIRSTPT) : !inserted(segnum, LASTPT))
            { /* insert v1 in the tree */
                int tmp_d;

                tu = locate_endpoint(s.v1, s.v0, s.root1);

                tl = newtrap();     /* tl is the new lower trapezoid */
                tr[tl].state = ST_VALID;
                tr[tl].CopyFrom(tr[tu]);
                tr[tu].lo.y = tr[tl].hi.y = s.v1.y;
                tr[tu].lo.x = tr[tl].hi.x = s.v1.x;
                tr[tu].d0 = tl;
                tr[tu].d1 = 0;
                tr[tl].u0 = tu;
                tr[tl].u1 = 0;

                if (((tmp_d = tr[tl].d0) > 0) && (tr[tmp_d].u0 == tu))
                    tr[tmp_d].u0 = tl;
                if (((tmp_d = tr[tl].d0) > 0) && (tr[tmp_d].u1 == tu))
                    tr[tmp_d].u1 = tl;

                if (((tmp_d = tr[tl].d1) > 0) && (tr[tmp_d].u0 == tu))
                    tr[tmp_d].u0 = tl;
                if (((tmp_d = tr[tl].d1) > 0) && (tr[tmp_d].u1 == tu))
                    tr[tmp_d].u1 = tl;

                /* Now update the query structure and obtain the sinks for the */
                /* two trapezoids */

                i1 = newnode();     /* Upper trapezoid sink */
                i2 = newnode();     /* Lower trapezoid sink */
                sk = tr[tu].sink;

                qs[sk].nodetype = T_Y;
                qs[sk].yval = s.v1;
                qs[sk].segnum = segnum; /* not really reqd ... maybe later */
                qs[sk].left = i2;
                qs[sk].right = i1;

                qs[i1].nodetype = T_SINK;
                qs[i1].trnum = tu;
                qs[i1].parent = sk;

                qs[i2].nodetype = T_SINK;
                qs[i2].trnum = tl;
                qs[i2].parent = sk;

                tr[tu].sink = i1;
                tr[tl].sink = i2;
                tlast = tu;
            }
            else
            {   /* v1 already present */
                /* Get the lowermost intersecting trapezoid */
                tlast = locate_endpoint(s.v1, s.v0, s.root1);
                tribot = true;
            }

            /* Thread the segment into the query tree creating a new X-node */
            /* First, split all the trapezoids which are intersected by s into */
            /* two */

            t = tfirst;         /* topmost trapezoid */

            while ((t > 0) && _greater_than_equal_to(tr[t].lo, tr[tlast].lo))
            { /* traverse from top to bot */
                int t_sav, tn_sav;
                sk = tr[t].sink;
                i1 = newnode();     /* left trapezoid sink */
                i2 = newnode();     /* right trapezoid sink */

                if (i1 < 0 || i1 >= qs.Count || i2 < 0 || i2 >= qs.Count)
                {
                    LogUtil.Error("add_segment: error\n");
                    break;
                }

                qs[sk].nodetype = T_X;
                qs[sk].segnum = segnum;
                qs[sk].left = i1;
                qs[sk].right = i2;

                qs[i1].nodetype = T_SINK;   /* left trapezoid (use existing one) */
                qs[i1].trnum = t;
                qs[i1].parent = sk;

                qs[i2].nodetype = T_SINK;   /* right trapezoid (allocate new) */
                qs[i2].trnum = tn = newtrap();
                tr[tn].state = ST_VALID;
                qs[i2].parent = sk;

                if (t == tfirst)
                    tfirstr = tn;
                if (_equal_to(tr[t].lo, tr[tlast].lo))
                    tlastr = tn;

                tr[tn].CopyFrom(tr[t]);
                tr[t].sink = i1;
                tr[tn].sink = i2;
                t_sav = t;
                tn_sav = tn;

                /* error */

                if ((tr[t].d0 <= 0) && (tr[t].d1 <= 0))
                { /* case cannot arise */
                    LogUtil.Error("add_segment: error\n");
                    break;
                }

                /* only one trapezoid below. partition t into two and make the */
                /* two resulting trapezoids t and tn as the upper neighbours of */
                /* the sole lower trapezoid */
                else if ((tr[t].d0 > 0) && (tr[t].d1 <= 0))
                {           /* Only one trapezoid below */
                    if ((tr[t].u0 > 0) && (tr[t].u1 > 0))
                    {           /* continuation of a chain from abv. */
                        if (tr[t].usave > 0)
                        { /* three upper neighbours */
                            if (tr[t].uside == S_LEFT)
                            {
                                tr[tn].u0 = tr[t].u1;
                                tr[t].u1 = -1;
                                tr[tn].u1 = tr[t].usave;

                                tr[tr[t].u0].d0 = t;
                                tr[tr[tn].u0].d0 = tn;
                                tr[tr[tn].u1].d0 = tn;
                            }
                            else
                            {   /* intersects in the right */
                                tr[tn].u1 = -1;
                                tr[tn].u0 = tr[t].u1;
                                tr[t].u1 = tr[t].u0;
                                tr[t].u0 = tr[t].usave;

                                tr[tr[t].u0].d0 = t;
                                tr[tr[t].u1].d0 = t;
                                tr[tr[tn].u0].d0 = tn;
                            }

                            tr[t].usave = tr[tn].usave = 0;
                        }
                        else
                        {   /* No usave.... simple case */
                            tr[tn].u0 = tr[t].u1;
                            tr[t].u1 = tr[tn].u1 = -1;
                            tr[tr[tn].u0].d0 = tn;
                        }
                    }
                    else
                    {   /* fresh seg. or upward cusp */
                        int tmp_u = tr[t].u0;
                        int td0, td1;
                        if (((td0 = tr[tmp_u].d0) > 0) &&
                          ((td1 = tr[tmp_u].d1) > 0))
                        { /* upward cusp */
                            if ((tr[td0].rseg > 0) &&
                              !is_left_of(tr[td0].rseg, s.v1))
                            {
                                tr[t].u0 = tr[t].u1 = tr[tn].u1 = -1;
                                tr[tr[tn].u0].d1 = tn;
                            }
                            else
                            {   /* cusp going leftwards */
                                tr[tn].u0 = tr[tn].u1 = tr[t].u1 = -1;
                                tr[tr[t].u0].d0 = t;
                            }
                        }
                        else
                        {   /* fresh segment */
                            tr[tr[t].u0].d0 = t;
                            tr[tr[t].u0].d1 = tn;
                        }
                    }

                    if (Geometry.IsSameDouble(tr[t].lo.y, tr[tlast].lo.y) &&
                      Geometry.IsSameDouble(tr[t].lo.x, tr[tlast].lo.x) && tribot)
                    { /* bottom forms a triangle */

                        if (is_swapped)
                            tmptriseg = seg[segnum].prev;
                        else
                            tmptriseg = seg[segnum].next;

                        if ((tmptriseg > 0) && is_left_of(tmptriseg, s.v0))
                        {
                            /* L-R downward cusp */
                            tr[tr[t].d0].u0 = t;
                            tr[tn].d0 = tr[tn].d1 = -1;
                        }
                        else
                        {
                            /* R-L downward cusp */
                            tr[tr[tn].d0].u1 = tn;
                            tr[t].d0 = tr[t].d1 = -1;
                        }
                    }
                    else
                    {
                        if ((tr[tr[t].d0].u0 > 0) && (tr[tr[t].d0].u1 > 0))
                        {
                            if (tr[tr[t].d0].u0 == t)
                            { /* passes thru LHS */
                                tr[tr[t].d0].usave = tr[tr[t].d0].u1;
                                tr[tr[t].d0].uside = S_LEFT;
                            }
                            else
                            {
                                tr[tr[t].d0].usave = tr[tr[t].d0].u0;
                                tr[tr[t].d0].uside = S_RIGHT;
                            }
                        }
                        tr[tr[t].d0].u0 = t;
                        tr[tr[t].d0].u1 = tn;
                    }

                    t = tr[t].d0;
                }
                else if ((tr[t].d0 <= 0) && (tr[t].d1 > 0))
                {           /* Only one trapezoid below */
                    if ((tr[t].u0 > 0) && (tr[t].u1 > 0))
                    {           /* continuation of a chain from abv. */
                        if (tr[t].usave > 0)
                        { /* three upper neighbours */
                            if (tr[t].uside == S_LEFT)
                            {
                                tr[tn].u0 = tr[t].u1;
                                tr[t].u1 = -1;
                                tr[tn].u1 = tr[t].usave;

                                tr[tr[t].u0].d0 = t;
                                tr[tr[tn].u0].d0 = tn;
                                tr[tr[tn].u1].d0 = tn;
                            }
                            else
                            {   /* intersects in the right */
                                tr[tn].u1 = -1;
                                tr[tn].u0 = tr[t].u1;
                                tr[t].u1 = tr[t].u0;
                                tr[t].u0 = tr[t].usave;

                                tr[tr[t].u0].d0 = t;
                                tr[tr[t].u1].d0 = t;
                                tr[tr[tn].u0].d0 = tn;
                            }

                            tr[t].usave = tr[tn].usave = 0;
                        }
                        else
                        {   /* No usave.... simple case */
                            tr[tn].u0 = tr[t].u1;
                            tr[t].u1 = tr[tn].u1 = -1;
                            tr[tr[tn].u0].d0 = tn;
                        }
                    }
                    else
                    {   /* fresh seg. or upward cusp */
                        int tmp_u = tr[t].u0;
                        int td0, td1;
                        if (((td0 = tr[tmp_u].d0) > 0) &&
                          ((td1 = tr[tmp_u].d1) > 0))
                        {   /* upward cusp */
                            if ((tr[td0].rseg > 0) &&
                              !is_left_of(tr[td0].rseg, s.v1))
                            {
                                tr[t].u0 = tr[t].u1 = tr[tn].u1 = -1;
                                tr[tr[tn].u0].d1 = tn;
                            }
                            else
                            {
                                tr[tn].u0 = tr[tn].u1 = tr[t].u1 = -1;
                                tr[tr[t].u0].d0 = t;
                            }
                        }
                        else
                        {   /* fresh segment */
                            tr[tr[t].u0].d0 = t;
                            tr[tr[t].u0].d1 = tn;
                        }
                    }

                    if (Geometry.IsSameDouble(tr[t].lo.y, tr[tlast].lo.y) &&
                      Geometry.IsSameDouble(tr[t].lo.x, tr[tlast].lo.x) && tribot)
                    {       /* bottom forms a triangle */
                        int tmpseg = 0;

                        if (is_swapped)
                            tmptriseg = seg[segnum].prev;
                        else
                            tmptriseg = seg[segnum].next;

                        if ((tmpseg > 0) && is_left_of(tmpseg, s.v0))
                        {
                            /* L-R downward cusp */
                            tr[tr[t].d1].u0 = t;
                            tr[tn].d0 = tr[tn].d1 = -1;
                        }
                        else
                        {
                            /* R-L downward cusp */
                            tr[tr[tn].d1].u1 = tn;
                            tr[t].d0 = tr[t].d1 = -1;
                        }
                    }
                    else
                    {
                        if ((tr[tr[t].d1].u0 > 0) && (tr[tr[t].d1].u1 > 0))
                        {
                            if (tr[tr[t].d1].u0 == t)
                            { /* passes thru LHS */
                                tr[tr[t].d1].usave = tr[tr[t].d1].u1;
                                tr[tr[t].d1].uside = S_LEFT;
                            }
                            else
                            {
                                tr[tr[t].d1].usave = tr[tr[t].d1].u0;
                                tr[tr[t].d1].uside = S_RIGHT;
                            }
                        }
                        tr[tr[t].d1].u0 = t;
                        tr[tr[t].d1].u1 = tn;
                    }

                    t = tr[t].d1;
                }
                else
                {
                    /* two trapezoids below. Find out which one is intersected by */
                    /* this segment and proceed down that one */
                    int tmpseg = tr[tr[t].d0].rseg;
                    double y0, yt;
                    point_t tmppt;
                    bool i_d0, i_d1;

                    i_d0 = i_d1 = false;
                    if (Geometry.IsSameDouble(tr[t].lo.y, s.v0.y))
                    {
                        if (tr[t].lo.x > s.v0.x)
                            i_d0 = true;
                        else
                            i_d1 = true;
                    }
                    else
                    {
                        tmppt.y = y0 = tr[t].lo.y;
                        yt = (y0 - s.v0.y) / (s.v1.y - s.v0.y);
                        tmppt.x = s.v0.x + yt * (s.v1.x - s.v0.x);

                        if (_less_than(tmppt, tr[t].lo))
                            i_d0 = true;
                        else
                            i_d1 = true;
                    }

                    /* check continuity from the top so that the lower-neighbour */
                    /* values are properly filled for the upper trapezoid */

                    if ((tr[t].u0 > 0) && (tr[t].u1 > 0))
                    {   /* continuation of a chain from abv. */
                        if (tr[t].usave > 0)
                        { /* three upper neighbours */
                            if (tr[t].uside == S_LEFT)
                            {
                                tr[tn].u0 = tr[t].u1;
                                tr[t].u1 = -1;
                                tr[tn].u1 = tr[t].usave;

                                tr[tr[t].u0].d0 = t;
                                tr[tr[tn].u0].d0 = tn;
                                tr[tr[tn].u1].d0 = tn;
                            }
                            else
                            {   /* intersects in the right */
                                tr[tn].u1 = -1;
                                tr[tn].u0 = tr[t].u1;
                                tr[t].u1 = tr[t].u0;
                                tr[t].u0 = tr[t].usave;

                                tr[tr[t].u0].d0 = t;
                                tr[tr[t].u1].d0 = t;
                                tr[tr[tn].u0].d0 = tn;
                            }

                            tr[t].usave = tr[tn].usave = 0;
                        }
                        else
                        {   /* No usave.... simple case */
                            tr[tn].u0 = tr[t].u1;
                            tr[tn].u1 = -1;
                            tr[t].u1 = -1;
                            tr[tr[tn].u0].d0 = tn;
                        }
                    }
                    else
                    {   /* fresh seg. or upward cusp */
                        int tmp_u = tr[t].u0;
                        int td0, td1;
                        if (((td0 = tr[tmp_u].d0) > 0) &&
                          ((td1 = tr[tmp_u].d1) > 0))
                        {   /* upward cusp */
                            if ((tr[td0].rseg > 0) &&
                              !is_left_of(tr[td0].rseg, s.v1))
                            {
                                tr[t].u0 = tr[t].u1 = tr[tn].u1 = -1;
                                tr[tr[tn].u0].d1 = tn;
                            }
                            else
                            {
                                tr[tn].u0 = tr[tn].u1 = tr[t].u1 = -1;
                                tr[tr[t].u0].d0 = t;
                            }
                        }
                        else
                        {   /* fresh segment */
                            tr[tr[t].u0].d0 = t;
                            tr[tr[t].u0].d1 = tn;
                        }
                    }

                    if (Geometry.IsSameDouble(tr[t].lo.y, tr[tlast].lo.y) &&
                      Geometry.IsSameDouble(tr[t].lo.x, tr[tlast].lo.x) && tribot)
                    {
                        /* this case arises only at the lowest trapezoid.. i.e.
                        tlast, if the lower endpoint of the segment is
                        already inserted in the structure */

                        tr[tr[t].d0].u0 = t;
                        tr[tr[t].d0].u1 = -1;
                        tr[tr[t].d1].u0 = tn;
                        tr[tr[t].d1].u1 = -1;

                        tr[tn].d0 = tr[t].d1;
                        tr[t].d1 = tr[tn].d1 = -1;

                        tnext = tr[t].d1;
                    }
                    else if (i_d0)
                    { /* intersecting d0 */
                        tr[tr[t].d0].u0 = t;
                        tr[tr[t].d0].u1 = tn;
                        tr[tr[t].d1].u0 = tn;
                        tr[tr[t].d1].u1 = -1;

                        /* new code to determine the bottom neighbours of the */
                        /* newly partitioned trapezoid */

                        tr[t].d1 = -1;

                        tnext = tr[t].d0;
                    }
                    else
                    {   /* intersecting d1 */
                        tr[tr[t].d0].u0 = t;
                        tr[tr[t].d0].u1 = -1;
                        tr[tr[t].d1].u0 = t;
                        tr[tr[t].d1].u1 = tn;

                        /* new code to determine the bottom neighbours of the */
                        /* newly partitioned trapezoid */

                        tr[tn].d0 = tr[t].d1;
                        tr[tn].d1 = -1;

                        tnext = tr[t].d1;
                    }

                    t = tnext;
                }

                tr[t_sav].rseg = tr[tn_sav].lseg = segnum;
            } /* end-while */

            /* Now combine those trapezoids which share common segments. We can */
            /* use the pointers to the parent to connect these together. This */
            /* works only because all these new trapezoids have been formed */
            /* due to splitting by the segment, and hence have only one parent */

            tfirstl = tfirst;
            tlastl = tlast;
            merge_trapezoids(segnum, tfirstl, tlastl, S_LEFT);
            merge_trapezoids(segnum, tfirstr, tlastr, S_RIGHT);

            seg[segnum].is_inserted = true;
        }
        /* Update the roots stored for each of the endpoints of the segment.
        * This is done to speed up the location-query for the endpoint when
        * the segment is inserted into the trapezoidation subsequently
        */
        private void find_new_roots(int segnum)
        {
            segment_t s = seg[segnum];
            if (s.is_inserted)
                return;

            s.root0 = locate_endpoint(s.v0, s.v1, s.root0);
            s.root0 = tr[s.root0].sink;

            s.root1 = locate_endpoint(s.v1, s.v0, s.root1);
            s.root1 = tr[s.root1].sink;
        }

        /* This is query routine which determines which trapezoid does the 
        * point v lie in. The return value is the trapezoid number. 
        */
        private int locate_endpoint(point_t v, point_t vo, int r)
        {
            node_t rptr = qs[r];

            switch (rptr.nodetype)
            {
                case T_SINK:
                    return rptr.trnum;
                case T_Y:
                    if (_greater_than(v, rptr.yval)) /* above */
                        return locate_endpoint(v, vo, rptr.right);
                    else if (_equal_to(v, rptr.yval))
                    { /* the point is already */
                      /* inserted. */
                        if (_greater_than(vo, rptr.yval)) /* above */
                            return locate_endpoint(v, vo, rptr.right);
                        else
                            return locate_endpoint(v, vo, rptr.left); /* below */
                    }
                    else
                        return locate_endpoint(v, vo, rptr.left); /* below */
                case T_X:
                    if (_equal_to(v, seg[rptr.segnum].v0) || _equal_to(v, seg[rptr.segnum].v1))
                    {
                        if (Geometry.IsSameDouble(v.y, vo.y))
                        { /* horizontal segment */
                            if (vo.x < v.x)
                                return locate_endpoint(v, vo, rptr.left); /* left */
                            else
                                return locate_endpoint(v, vo, rptr.right); /* right */
                        }
                        else if (is_left_of(rptr.segnum, vo))
                            return locate_endpoint(v, vo, rptr.left); /* left */
                        else
                            return locate_endpoint(v, vo, rptr.right); /* right */
                    }
                    else if (is_left_of(rptr.segnum, v))
                        return locate_endpoint(v, vo, rptr.left); /* left */
                    else
                        return locate_endpoint(v, vo, rptr.right); /* right */
                default:
                    //Haggu !!!!!
                    break;
            }
            return 0;
        }
        /* Thread in the segment into the existing trapezoidation. The 
        * limiting trapezoids are given by tfirst and tlast (which are the
        * trapezoids containing the two endpoints of the segment. Merges all
        * possible trapezoids which flank this segment and have been recently
        * divided because of its insertion
        */
        private void merge_trapezoids(int segnum, int tfirst, int tlast, int side)
        {
            bool cond;
            int t, tnext;
            int ptnext;

            /* First merge polys on the LHS */
            t = tfirst;
            while ((t > 0) && _greater_than_equal_to(tr[t].lo, tr[tlast].lo))
            {
                if (side == S_LEFT)
                {
                    cond = ((((tnext = tr[t].d0) > 0) && (tr[tnext].rseg == segnum)) ||
                    (((tnext = tr[t].d1) > 0) && (tr[tnext].rseg == segnum)));
                }
                else
                {
                    cond = ((((tnext = tr[t].d0) > 0) && (tr[tnext].lseg == segnum)) ||
                    (((tnext = tr[t].d1) > 0) && (tr[tnext].lseg == segnum)));
                }
                if (cond)
                {
                    if ((tr[t].lseg == tr[tnext].lseg) &&
                      (tr[t].rseg == tr[tnext].rseg))
                    { /* good neighbours */
                      /* merge them */
                      /* Use the upper node as the new node i.e. t */

                        ptnext = qs[tr[tnext].sink].parent;

                        if (qs[ptnext].left == tr[tnext].sink)
                            qs[ptnext].left = tr[t].sink;
                        else
                            qs[ptnext].right = tr[t].sink;  /* redirect parent */


                        /* Change the upper neighbours of the lower trapezoids */

                        if ((tr[t].d0 = tr[tnext].d0) > 0)
                            if (tr[tr[t].d0].u0 == tnext)
                                tr[tr[t].d0].u0 = t;
                            else if (tr[tr[t].d0].u1 == tnext)
                                tr[tr[t].d0].u1 = t;

                        if ((tr[t].d1 = tr[tnext].d1) > 0)
                            if (tr[tr[t].d1].u0 == tnext)
                                tr[tr[t].d1].u0 = t;
                            else if (tr[tr[t].d1].u1 == tnext)
                                tr[tr[t].d1].u1 = t;

                        tr[t].lo = tr[tnext].lo;
                        tr[tnext].state = ST_INVALID; /* invalidate the lower */
                                                      /* trapezium */
                    }
                    else            /* not good neighbours */
                        t = tnext;
                }
                else            /* do not satisfy the outer if */
                    t = tnext;

            } /* end-while */
        }
        /* Retun true if the vertex v is to the left of line segment no.
        * segnum. Takes care of the degenerate cases when both the vertices
        * have the same y--cood, etc.
        */
        private bool is_left_of(int segnum, point_t v)
        {
            segment_t s = seg[segnum];
            double area;

            if (_greater_than(s.v1, s.v0))
            { /* seg. going upwards */
                if (Geometry.IsSameDouble(s.v1.y, v.y))
                {
                    if (v.x < s.v1.x)
                        area = 1.0;
                    else
                        area = -1.0;
                }
                else if (Geometry.IsSameDouble(s.v0.y, v.y))
                {
                    if (v.x < s.v0.x)
                        area = 1.0;
                    else
                        area = -1.0;
                }
                else
                    area = cross(s.v0, s.v1, v);
            }
            else
            {   /* v0 > v1 */
                if (Geometry.IsSameDouble(s.v1.y, v.y))
                {
                    if (v.x < s.v1.x)
                        area = 1.0;
                    else
                        area = -1.0;
                }
                else if (Geometry.IsSameDouble(s.v0.y, v.y))
                {
                    if (v.x < s.v0.x)
                        area = 1.0;
                    else
                        area = -1.0;
                }
                else
                    area = cross(s.v1, s.v0, v);
            }

            if (area > 0.0)
                return true;
            else
                return false;
        }
        /* Returns true if the corresponding endpoint of the given segment is */
        /* already inserted into the segment tree. Use the simple test of */
        /* whether the segment which shares this endpoint is already inserted */
        private bool inserted(int segnum, int whichpt)
        {
            if (whichpt == FIRSTPT)
                return seg[seg[segnum].prev].is_inserted;
            else
                return seg[seg[segnum].next].is_inserted;
        }
        private void add_triangle(int pt1, int pt2, int pt3)
        {
            uint c1 = get_side_code(pt1, pt2);
            uint c2 = get_side_code(pt2, pt3);
            uint c3 = get_side_code(pt3, pt1);

            TriangleNode node = new TriangleNode();
            node.Points[0] = new Vector3((float)seg[pt1].v0.x, 0, (float)seg[pt1].v0.y);
            node.Points[1] = new Vector3((float)seg[pt2].v0.x, 0, (float)seg[pt2].v0.y);
            node.Points[2] = new Vector3((float)seg[pt3].v0.x, 0, (float)seg[pt3].v0.y);
            results.Add(node);
            node.Id = results.Count;

            triangle_t triangle = new triangle_t();
            triangle.triangle = node;
            triangle.codes[0] = c1;
            triangle.codes[1] = c2;
            triangle.codes[2] = c3;
            triangles.Add(triangle);

            add_triangle_neighbors(c1, triangle);
            add_triangle_neighbors(c2, triangle);
            add_triangle_neighbors(c3, triangle);
        }
        private void add_triangle_neighbors(uint code, triangle_t triangle)
        {
            List<triangle_t> neighbors = new List<triangle_t>();
            if (triangle_neighbors.TryGetValue(code, out neighbors))
            {
                neighbors.Add(triangle);
            }
            else
            {
                neighbors = new List<triangle_t>();
                neighbors.Add(triangle);
                triangle_neighbors.Add(code, neighbors);
            }
        }
        private void set_triangle_neighbor(uint code, triangle_t triangle, TriangleNode neighbor)
        {
            TriangleNode node = triangle.triangle;
            for (int i = 0; i < 3; ++i)
            {
                if (triangle.codes[i] == code)
                {
                    node.Neighbors[i] = neighbor;
                }
            }
        }

        private int newnode()
        {
            if (q_idx < qs.Count)
            {
                return q_idx++;
            }
            else
            {
                return -1;
            }
        }
        private int newtrap()
        {
            if (tr_idx < tr.Count)
            {
                tr[tr_idx].lseg = -1;
                tr[tr_idx].rseg = -1;
                tr[tr_idx].state = ST_VALID;
                return tr_idx++;
            }
            else
            {
                return -1;
            }
        }
        private void generate_random_ordering(int n)
        {
            permute.Clear();
            choose_idx = 1;
            permute.Add(0);
            for (int i = 1; i <= n; ++i)
                permute.Add(i);

            for (int i = 1; i <= n; ++i)
            {
                int m = i + RandomUtil.Next() % (n + 1 - i);
                int t = permute[i];
                permute[i] = permute[m];
                permute[m] = t;
            }
        }
        private int choose_segment()
        {
            return permute[choose_idx++];
        }

        /* Function returns true if the trapezoid lies inside the polygon */
        private bool inside_polygon(trap_t t)
        {
            int rseg = t.rseg;

            if (t.state == ST_INVALID)
                return false;

            if ((t.lseg <= 0) || (t.rseg <= 0))
                return false;

            if (((t.u0 <= 0) && (t.u1 <= 0)) ||
                ((t.d0 <= 0) && (t.d1 <= 0))) /* triangle */
                return (_greater_than(seg[rseg].v1, seg[rseg].v0));

            return false;
        }
        private int newmon()
        {
            return ++mon_idx;
        }
        private int new_chain_element()
        {
            return ++chain_idx;
        }

        private int choose_idx;
        private List<int> permute = new List<int>();

        private int q_idx;
        private int tr_idx;

        private List<node_t> qs = new List<node_t>();           /* Query structure */
        private List<trap_t> tr = new List<trap_t>();           /* Trapezoid structure */
        private List<segment_t> seg = new List<segment_t>();        /* Segment table */

        private List<monchain_t> mchain = new List<monchain_t>();
        /* Table to hold all the monotone */
        /* polygons . Each monotone polygon */
        /* is a circularly linked list */

        private List<vertexchain_t> vert = new List<vertexchain_t>();
        /* chain init. information. This */
        /* is used to decide which */
        /* monotone polygon to split if */
        /* there are several other */
        /* polygons touching at the same */
        /* vertex */

        private List<int> mon = new List<int>();
        private List<bool> visited = new List<bool>();
        private int chain_idx;
        private int mon_idx;
        private int op_idx;

        private List<IList<Vector3>> pre_triangles = new List<IList<Vector3>>();
        private List<triangle_t> triangles = new List<triangle_t>();
        private Dictionary<uint, List<triangle_t>> triangle_neighbors = new Dictionary<uint, List<triangle_t>>();
        private List<TriangleNode> results = new List<TriangleNode>();

        #region 常量部分

        /* Node types */
        private const int T_X = 1;
        private const int T_Y = 2;
        private const int T_SINK = 3;
        private const int FIRSTPT = 1;      /* checking whether pt. is inserted */
        private const int LASTPT = 2;
        private const int INFINITY = 1 << 30;
        private const double C_EPS = 1.0e-7;
        /* tolerance value: Used for making */
        /* all decisions about collinearity or */
        /* left/right of segment. Decrease */
        /* this value if the input points are */
        /* spaced very close together */


        private const int S_LEFT = 1;       /* for merge-direction */
        private const int S_RIGHT = 2;

        private const int ST_VALID = 1; /* for trapezium state */
        private const int ST_INVALID = 2;

        private const int SP_SIMPLE_LRUP = 1;   /* for splitting trapezoids */
        private const int SP_SIMPLE_LRDN = 2;
        private const int SP_2UP_2DN = 3;
        private const int SP_2UP_LEFT = 4;
        private const int SP_2UP_RIGHT = 5;
        private const int SP_2DN_LEFT = 6;
        private const int SP_2DN_RIGHT = 7;
        private const int SP_NOSPLIT = -1;

        private const int TR_FROM_UP = 1;       /* for traverse-direction */
        private const int TR_FROM_DN = 2;

        private const int TRI_LHS = 1;
        private const int TRI_RHS = 2;

        #endregion

        #region 静态成员部分

        private static uint get_side_code(int pt1, int pt2)
        {
            uint p1 = (uint)pt1;
            uint p2 = (uint)pt2;
            return p1 < p2 ? p1 + (p2 << 16) : p2 + (p1 << 16);
        }
        private static point_t _max(point_t v0, point_t v1)
        {
            if (v0.y > v1.y + C_EPS)
            {
                return v0;
            }
            else if (Geometry.IsSameDouble(v0.y, v1.y))
            {
                if (v0.x > v1.x + C_EPS)
                    return v0;
                else
                    return v1;
            }
            else
            {
                return v1;
            }
        }
        private static point_t _min(point_t v0, point_t v1)
        {
            if (v0.y < v1.y - C_EPS)
            {
                return v0;
            }
            else if (Geometry.IsSameDouble(v0.y, v1.y))
            {
                if (v0.x < v1.x)
                    return v0;
                else
                    return v1;
            }
            else
            {
                return v1;
            }
        }
        private static bool _greater_than(point_t v0, point_t v1)
        {
            if (v0.y > v1.y + C_EPS)
                return true;
            else if (v0.y < v1.y - C_EPS)
                return false;
            else
                return (v0.x > v1.x);
        }
        private static bool _equal_to(point_t v0, point_t v1)
        {
            return (Geometry.IsSameDouble(v0.y, v1.y) && Geometry.IsSameDouble(v0.x, v1.x));
        }
        private static bool _greater_than_equal_to(point_t v0, point_t v1)
        {
            if (v0.y > v1.y + C_EPS)
                return true;
            else if (v0.y < v1.y - C_EPS)
                return false;
            else
                return (v0.x >= v1.x);
        }
        private static bool _less_than(point_t v0, point_t v1)
        {
            if (v0.y < v1.y - C_EPS)
                return true;
            else if (v0.y > v1.y + C_EPS)
                return false;
            else
                return (v0.x < v1.x);
        }
        private static double cross(point_t v0, point_t v1, point_t v2)
        {
            return (((v1).x - (v0).x) * ((v2).y - (v0).y) - ((v1).y - (v0).y) * ((v2).x - (v0).x));
        }
        private static double dot(point_t v0, point_t v1)
        {
            return ((v0).x * (v1).x + (v0).y * (v1).y);
        }
        private static double cross_sine(point_t v0, point_t v1)
        {
            return ((v0).x * (v1).y - (v1).x * (v0).y);
        }
        private static double length(point_t v0)
        {
            return (Math.Sqrt((v0).x * (v0).x + (v0).y * (v0).y));
        }
        private static double get_angle(point_t vp0, point_t vpnext, point_t vp1)
        {
            point_t v0, v1;

            v0.x = vpnext.x - vp0.x;
            v0.y = vpnext.y - vp0.y;

            v1.x = vp1.x - vp0.x;
            v1.y = vp1.y - vp0.y;

            if (cross_sine(v0, v1) >= 0)    /* sine is positive */
                return dot(v0, v1) / length(v0) / length(v1);
            else
                return (-1.0 * dot(v0, v1) / length(v0) / length(v1) - 2);
        }
        private static double log2(double val)
        {
            return Math.Log(val, 2);
        }
        private static int math_logstar_n(int n)
        {
            int i;
            double v = (double)n;
            for (i = 0; v >= 1; ++i)
                v = log2(v);

            return (i - 1);
        }
        private static int math_N(int n, int h)
        {
            double v = (int)n;

            for (int i = 0; i < h; ++i)
                v = log2(v);

            return (int)Math.Ceiling((double)1.0 * n / v);
        }
        private static void WriteInt(FileStream fs, int val)
        {
            byte a = (byte)(val & 0x0ff);
            byte b = (byte)((val & 0x0ff00) >> 8);
            byte c = (byte)((val & 0x0ff0000) >> 16);
            byte d = (byte)((val & 0x0ff000000) >> 24);
            fs.WriteByte(a);
            fs.WriteByte(b);
            fs.WriteByte(c);
            fs.WriteByte(d);
        }
        private static int ReadInt(FileStream fs)
        {
            int a = fs.ReadByte();
            int b = fs.ReadByte();
            int c = fs.ReadByte();
            int d = fs.ReadByte();
            int val = (d << 24) + (c << 16) + (b << 8) + a;
            return val;
        }

        #endregion

        private const string NAVMESH_VERSION = "SGNAVMESH_01";
        public IList<TriangleNode> BuildTriangleFromFile(string sFilePath, float scale)
        {
            string sFileName = sFilePath.Replace("obs", "navmesh");
            if (!File.Exists(sFileName))
            {
                return null;
            }
            Dictionary<int, TriangleNode> TriangleNodeMap = new Dictionary<int, TriangleNode>();
            List<TriangleNode> TriangleNodeList = new List<TriangleNode>();
            try
            {
                using (FileStream fs = new FileStream(sFileName, FileMode.Open))
                {
                    using (BinaryReader binReader = new BinaryReader(fs))
                    {
                        // 读取版本号
                        string fileVersion = new string(binReader.ReadChars(NAVMESH_VERSION.Length));
                        if (fileVersion != NAVMESH_VERSION)
                            return null;
                        // 读取导航三角形数量
                        int navCount = binReader.ReadInt32();
                        for (int i = 0; i < navCount; i++)
                        {
                            TriangleNode node = new TriangleNode();
                            node.Read(binReader, scale);
                            TriangleNodeList.Add(node);
                            TriangleNodeMap.Add(node.Id, node);
                        }
                        binReader.Close();
                    }
                    fs.Close();
                }

                //build Neighbors node//顺时针
                int ct = TriangleNodeList.Count;
                for (int i = 0; i < ct; ++i)
                {
                    TriangleNode node = TriangleNodeList[i];
                    TriangleNode outnode = null;
                    if (TriangleNodeMap.TryGetValue(node.NeighborsId[0], out outnode))
                    {
                        node.Neighbors[2] = outnode;
                    }
                    if (TriangleNodeMap.TryGetValue(node.NeighborsId[1], out outnode))
                    {
                        node.Neighbors[1] = outnode;
                    }
                    if (TriangleNodeMap.TryGetValue(node.NeighborsId[2], out outnode))
                    {
                        node.Neighbors[0] = outnode;
                    }
                    node.CalcGeometryInfo();
                }
            }
            catch
            {
            }
            finally
            {
            }
            if (TriangleNodeList.Count == 0)
            {
                return null;
            }
            //排序//
            TriangleNodeList.Sort((TriangleNode node1, TriangleNode node2) =>
            {
                if (node1.Position.x < node2.Position.x)
                {
                    return -1; //左值小于右值，返回-1，为升序，如果返回1，就是降序  
          }
                else if (node1.Position.x == node2.Position.x)
                {//左值等于右值，返回0 
              if (node1.Position.z == node2.Position.z)
                    {
                        return 0;
                    }
                    else if (node1.Position.z < node2.Position.z)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 1;//左值大于右值，返回1，为升序，如果返回-1，就是降序  
          }
            });

            IList<TriangleNode> ret = TriangleNodeList.ToArray();
            TriangleNodeList.Clear();
            TriangleNodeMap.Clear();
            return ret;
        }
    }
}
