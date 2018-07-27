using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Util;
using Spatial;
using UnityEngine;

namespace UnityClient
{
    /*
    public class SpatialSystem : Singleton<SpatialSystem>
    {
        public void Load(string navmesh)
        {
            var gameContext = Contexts.sharedInstance.game;

            float width, height;
            NavmeshMapParser mapParser = new NavmeshMapParser();
            mapParser.ParseTileDataWithNavmesh(HomePath.Instance.GetAbsolutePath(navmesh), out width, out height);

            m_CellMgr = new CellManager();
            m_CellMgr.Init(width, height, 0.5f);

            mapParser.GenerateObstacleInfoWithNavmesh(m_CellMgr);
            JumpPointFinder finder = new JumpPointFinder();
            finder.Init(m_CellMgr);
        }
        public bool CanPass(float curPosX, float curPosZ, float toPosX, float toPosZ)
        {
            CellPos cur_cell = new CellPos();
            m_CellMgr.GetCell(curPosX, curPosZ, out cur_cell.row, out cur_cell.col);
            CellPos to_cell = new CellPos();
            m_CellMgr.GetCell(toPosX, toPosZ, out to_cell.row, out to_cell.col);
            if (cur_cell.row == to_cell.row && cur_cell.col == to_cell.col)
            {
                return true;
            }
            byte curStatus = m_CellMgr.GetCellStatus(cur_cell.row, cur_cell.col);
            byte block = BlockType.GetBlockType(curStatus);
            if (block != BlockType.NOT_BLOCK)
            {
                return true;
            }
            byte status = m_CellMgr.GetCellStatus(to_cell.row, to_cell.col);
            block = BlockType.GetBlockType(status);
            if (block != BlockType.NOT_BLOCK)
            {
                //LogUtil.Debug("CanPass ({0},{1})->({2},{3}), target is blocked {4}", cur_cell.row, cur_cell.col, to_cell.row, to_cell.col, block);
                return false;
            }
            if (Math.Abs(cur_cell.row - to_cell.row) >= 1 || Math.Abs(cur_cell.col - to_cell.col) >= 1)
            {
                Vector3 from = m_CellMgr.GetCellCenter(cur_cell.row, cur_cell.col);
                Vector3 to = m_CellMgr.GetCellCenter(to_cell.row, to_cell.col);

                bool ret = true;
                m_CellMgr.VisitCellsCrossByLine(from, to, (row, col) =>
                {
                    status = m_CellMgr.cells_arr_[row, col];
                    block = BlockType.GetBlockType(status);
                    if (block != BlockType.NOT_BLOCK)
                    {
                        ret = false;
                        //LogSystem.Debug("CanPass ({0},{1})->({2},{3}), ({4},{5}) is blocked {6}", cur_cell.row, cur_cell.col, to_cell.row, to_cell.col, row, col, block);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                });
                return ret;
            }
            return true;
        }
        public bool CanPass(Vector3 curPos, Vector3 to)
        {
            return CanPass(curPos.x, curPos.z, to.x, to.z);
        }
        public Vector3 GetNearestWalkablePos(Vector3 pos)
        {
            CellPos cell = m_CellMgr.GetNearstWalkableCell(pos);
            if (null != cell)
                return m_CellMgr.GetCellCenter(cell.row, cell.col);
            return pos;
        }
        private CellManager m_CellMgr;
    }
    */
}
