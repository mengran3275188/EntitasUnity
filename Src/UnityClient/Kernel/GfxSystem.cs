using System;
using System.Collections.Generic;
using Util;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    // 作为逻辑层调用表现成的粘合类
    public sealed class GfxSystem
    {
        public static PublishSubscribeSystem EventForLogic
        {
            get { return GfxSystemImpl.Gfx.EventForLogic; }
        }
        public static void OnStart(IActionQueue processor)
        {
            GfxSystemImpl.Gfx.OnStart(processor);
        }
        public static void OnQuit()
        {
            GfxSystemImpl.Gfx.OnQuit();
        }
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            GfxSystemImpl.Gfx.DrawLine(start.x, start.y, start.z, end.x, end.y, end.z);
        }
        public static void DrawPoint(Vector3 pos, float radius)
        {
            GfxSystemImpl.Gfx.DrawPoint(pos.x, pos.y, pos.z, radius);
        }
        public static void DrawCircle(Vector3 center, float radius, float remainTime)
        {
            GfxSystemImpl.Gfx.DrawCircle(center.x, center.y, center.z, radius, remainTime);
        }
        public static void CreateGameObject(uint resId, string resource, Vector3 position, Vector3 eular, Vector3 scale, float recycleTime = -1)
        {
            GfxSystemImpl.Gfx.CreateGameObject(resId, resource, position.x, position.y, position.z,
                                               eular.x, eular.y, eular.z,
                                               scale.x, scale.y, scale.z, recycleTime);
        }
        public static void CreateAndAttachGameObject(uint resId, string resource, uint parentId, string path, bool isAttach,
                                                     Vector3 position, Vector3 eular, Vector3 scale, float recycleTime = -1)
        {
            GfxSystemImpl.Gfx.CreateAndAttachGameObject(resId, resource, parentId, path, isAttach,
                                                        position.x, position.y, position.z, eular.x, eular.y, eular.z, scale.x, scale.y, scale.z, recycleTime);
        }
        public static void MoveChildToBone(uint resId, string childName, string boneName)
        {
            GfxSystemImpl.Gfx.MoveChildToBone(resId, childName, boneName);
        }
        // 线程安全的方法允许返回值
        public static bool GetJoyStickDir(out float dir)
        {
            return GfxSystemImpl.Gfx.GetJoyStickDir(out dir);
        }
        public static float GetTime()
        {
            return GfxSystemImpl.Gfx.GetTime();
        }
    }
}
