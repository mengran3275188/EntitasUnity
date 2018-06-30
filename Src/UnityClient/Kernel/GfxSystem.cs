﻿using System;
using System.Collections.Generic;
using Util;
using Entitas.Data;

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
        public static void OnTick()
        {
            GfxSystemImpl.Gfx.OnTick();
        }
        public static void OnQuit()
        {
            GfxSystemImpl.Gfx.OnQuit();
        }
        public static void Instantiate(uint resId, string path)
        {
            GfxSystemImpl.Gfx.Instantiate(resId, path);
        }
        public static void DestoryResource(uint resId)
        {
            GfxSystemImpl.Gfx.DestroyResource(resId);
        }
        public static void CreateAndAttachGameObject(uint resId, string resource, uint parentId, string path, float recycleTime, bool isAttach)
        {
            GfxSystemImpl.Gfx.CreateAndAttachGameObject(resId, resource, parentId, path, recycleTime, isAttach);
        }
        public static void UpdatePosition(uint resId, float x, float y, float z)
        {
            GfxSystemImpl.Gfx.UpdatePosition(resId, x, y, z);
        }
        public static void UpdateRotation(uint resId, float rotation)
        {
            GfxSystemImpl.Gfx.UpdateRotation(resId, rotation);
        }
        public static void PlayAnimation(uint resId, string animName)
        {
            GfxSystemImpl.Gfx.PlayAnimation(resId, animName);
        }
        public static void PlayAnimation(uint resId, string animName, float speed, float weight, int layer, int wrapMode, int blendMode, int playMode, long crossFadeTime)
        {
            GfxSystemImpl.Gfx.PlayAnimation(resId, animName, speed,  weight, layer, wrapMode, blendMode, playMode, crossFadeTime);
        }
        public static void ListenKeyPressState(params Keyboard.Code[] codes)
        {
            GfxSystemImpl.Gfx.ListenKeyPressState(codes);
        }
        public static void UpdateCamera(float x, float y, float z)
        {
            GfxSystemImpl.Gfx.UpdateCamera(x, y, z);
        }
        public static void SetAnimationSpeed(uint resId, string animName, float speed)
        {
            GfxSystemImpl.Gfx.SetAnimationSpeed(resId, animName, speed);
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
        public static bool IsKeyPressed(Keyboard.Code c)
        {
            return GfxSystemImpl.Gfx.IsKeyPressed(c);
        }
        public static float GetCameraYaw()
        {
            return GfxSystemImpl.Gfx.GetCameraYaw();
        }
    }
}
