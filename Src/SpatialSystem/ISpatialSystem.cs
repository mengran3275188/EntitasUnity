using System;
using System.Collections.Generic;
using Util;
using Util.MyMath;

namespace Spatial
{
    public enum RetCode
    {
        SUCCESS = 0,
        OBJECT_EXIST,
        NULL_POINTER,
        UNKNOW_OBJECT_TYPE,
    }
    public interface ISpatialSystem
    {
        float Width { get; }
        float Height { get; }
        /// <summary>
        /// 将物体obj加入到空间系统新增列表，物体将在下一帧加入到管理队列中，
        /// 通过区域等查询接口也只能在下一帧后才能取到该物体
        /// </summary>
        /// <param name="obj">待新增的物体</param>
        /// <returns></returns>
        RetCode AddObj(ISpaceObject obj);

        /// <summary>
        /// 将物体obj加入到空间系统的删除缓冲列表, 物体将在当前帧之后被删除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool RemoveObj(ISpaceObject obj);

        /// <summary>
        /// 取得圆心在center位置，半径为radius的圆内的物体
        /// </summary>
        /// <param name="center">圆心位置</param>
        /// <param name="radius">半径</param>
        /// <returns>圆形区域内的物体</returns>
        List<ISpaceObject> GetObjectInCircle(Vector3 center, float radius);
        List<ISpaceObject> GetObjectInCircle(Vector3 center, float radius, MyFunc<float, ISpaceObject, bool> pred);
        void VisitObjectInCircle(Vector3 center, float radius, MyAction<float, ISpaceObject> visitor);
        void VisitObjectInCircle(Vector3 center, float radius, MyFunc<float, ISpaceObject, bool> visitor);

        /// <summary>
        /// 查询位置在指定多边形内的对象，不考虑对象半径。
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        List<ISpaceObject> GetObjectInPolygon(IList<Vector3> polygon);
        List<ISpaceObject> GetObjectInPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> pred);
        void VisitObjectInPolygon(IList<Vector3> polygon, MyAction<float, ISpaceObject> visitor);
        void VisitObjectInPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> visitor);
        void VisitObjectOutPolygon(IList<Vector3> polygon, MyAction<float, ISpaceObject> visitor);
        void VisitObjectOutPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> visitor);

        /// <summary>
        /// 查找从from到to的路径
        /// </summary>
        /// <param name="from">出发位置</param>
        /// <param name="to">结果位置</param>
        /// <returns>从from到to的路点，没有找到路径时为空</returns>
        List<Vector3> FindPath(Vector3 from, Vector3 to);

        /// <summary>
        /// 查找从from到to的路径
        /// </summary>
        /// <param name="from">出发位置</param>
        /// <param name="to">结果位置</param>
        /// <returns>从from到to的路点，没有找到路径时为空</returns>
        List<Vector3> FindPathWithCellMap(Vector3 from, Vector3 to);

        /// <summary>
        /// 计算对象避让其它对象后的速度矢量
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prefDir">优先的移动方向，需要是单位向量</param>
        /// <param name="stepTime"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="neighborDist"></param>
        /// <param name="isUsingAvoidanceVelocity"></param>
        /// <returns></returns>
        Vector3 ComputeVelocity(ISpaceObject obj, Vector3 prefDir, float stepTime, float maxSpeed, float neighborDist, bool isUsingAvoidanceVelocity);

        /// <summary>
        /// 判断物体obj是否可以走到to的位置，检测物体是否被阻挡挡住
        /// </summary>
        /// <param name="obj">物体</param>
        /// <param name="to">所要去的位置</param>
        /// <returns>过以走到to时返回true, 反之返回false</returns>
        bool CanPass(ISpaceObject obj, Vector3 to);
        bool CanPass(Vector3 from, Vector3 to);

        bool CanPass(int row, int col);
        bool CanPass(Vector3 targetPos);
        bool GetNearstWalkablePosition(Vector3 from, ref Vector3 pos);
        bool GetNearstWalkablePosition(Vector3 from, ref Vector3 pos, int maxCells);
        bool AdjustToNearstWalkablePosition(Vector3 from, ref Vector3 newPos, int maxCells);

        bool GetCell(Vector3 pos, out int row, out int col);
        Vector3 GetCellCenter(int row, int col);
        bool IsCellValid(int row, int col);
        /// <summary>
        /// 得到指定格子的阻挡信息
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        byte GetCellStatus(int row, int col);
        /// <summary>
        /// 设定指定格子的阻挡信息
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="status"></param>
        void SetCellStatus(int row, int col, byte status);
        /// <summary>
        /// 得到指定多边形包含的格子（以格子中心点计算）
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        List<CellPos> GetCellsInPolygon(IList<Vector3> pts);
        void VisitCellsInPolygon(IList<Vector3> pts, MyAction<int, int> visitor);
        /// <summary>
        /// 得到与指定线段相交的格子
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<CellPos> GetCellsCrossByLine(Vector3 start, Vector3 end);
        void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyAction<int, int> visitor);
        void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyFunc<int, int, bool> visitor);
        /// <summary>
        /// 得到与指定折线的边相交的格子
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        List<CellPos> GetCellsCrossByPolyline(IList<Vector3> pts);
        void VisitCellsCrossByPolyline(IList<Vector3> pts, MyAction<int, int> visitor);
        /// <summary>
        /// 得到与指定多边形的边相交的格子
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        List<CellPos> GetCellsCrossByPolygon(IList<Vector3> pts);
        void VisitCellsCrossByPolygon(IList<Vector3> pts, MyAction<int, int> visitor);
        /// <summary>
        /// 得到包含在圆内的格子（以格子中心计）
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        List<CellPos> GetCellsInCircle(Vector3 center, float radius);
        void VisitCellsInCircle(Vector3 center, float radius, MyAction<int, int> visitor);
        /// <summary>
        /// 得到与指定圆的边相交的格子（不保证精确相交，用圆心距粗略估算）
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        List<CellPos> GetCellsCrossByCircle(Vector3 center, float radius);
        void VisitCellsCrossByCircle(Vector3 center, float radius, MyAction<int, int> visitor);
    }
}
