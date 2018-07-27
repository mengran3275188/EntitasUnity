using System;
using System.Collections.Generic;
using Util.MyMath;

namespace Spatial
{
    internal static class MapDataUtil
    {
        internal static void MarkObstacleArea(List<Vector3> pts, CellManager cellMgr, byte obstacle)
        {
            List<CellPos> cells = cellMgr.GetCellsInPolygon(pts);
            foreach(CellPos cell in cells)
            {
                cellMgr.SetCellStatus(cell.row, cell.col, obstacle);
            }
        }
        internal static void MarkObstacleLine(List<Vector3> pts, CellManager cellMgr, byte obstacle)
        {
            List<CellPos> cells = cellMgr.GetCellsCrossByPolyline(pts);
            foreach(CellPos cell in cells)
            {
                cellMgr.SetCellStatus(cell.row, cell.col, obstacle);
            }
        }
    }
}
