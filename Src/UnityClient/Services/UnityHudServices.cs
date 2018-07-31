using System;
using System.Collections.Generic;
using Entitas.Data;
using UnityDelegate;
using UnityEngine;

namespace UnityClient
{
      public class UnityHudServices : Service
      {
        private class HudInfo
        {
            public GameObject Obj;
            public RectTransform SelfTransform;
            public Transform Owner;
            public float DeleteTime;
        }
        private Transform UIRoot
        {
            get
            {
                if (null == m_UIRoot)
                    m_UIRoot = GameObject.Find("GameLogic/Canvas/TempUIRoot").transform;
                return m_UIRoot;
            }
        }
        private enum Guidance
        {
            Up,
            Down,
            Left,
            Right,
            LeftUp,
            RightUp,
            LeftDown,
            RightDown,
        }
        private class DamageInfo
        {
            public GameObject Obj;
            public RectTransform SelfTransform;
            public Vector3 Position;
            public float DeleteTime;

            public float Delay;
            public float YQuickness;
            public float FloatSpeed;
            public Guidance Movement;
            public float YCounterVail;
            public float XCounterVail;
        }
        public UnityHudServices(Contexts contexts) : base(contexts)
        {
        }
        public void AddDamageText(GameEntity entity, string text, long remainTime)
        {
            UnityView view = entity.view.Value as UnityView;
            if(null != view)
            {
                var hudObject = ResourceSystem.NewObject("HudText") as GameObject;
                hudObject.transform.SetParent(UIRoot, false);
                hudObject.transform.localPosition = Vector3.zero;

                DamageInfo hudInfo = new DamageInfo();
                hudInfo.Obj = hudObject;
                UnityEngine.UI.Text uiText = hudObject.GetComponentInChildren<UnityEngine.UI.Text>();
                if(null != uiText)
                    uiText.text = text;

                hudInfo.SelfTransform = hudObject.GetComponentInChildren<RectTransform>();
                hudInfo.Position = view.gameObject.transform.position + Vector3.up * 2;
                hudInfo.DeleteTime = Time.time + remainTime / 1000.0f;

                hudInfo.FloatSpeed = m_FloatSpeed;
                hudInfo.Delay = m_DelayStay + Time.time;
                hudInfo.Movement = Guidance.LeftUp;

                m_Damages.Add(hudInfo);
            }
        }
        public void AddHudHead(GameEntity entity)
        {
            UnityView view = entity.view.Value as UnityView;
            if(null != view)
            {
                var hudHeadObject = ResourceSystem.NewObject("HudHead") as GameObject;
                hudHeadObject.transform.SetParent(UIRoot, false);

                HudInfo hudInfo = new HudInfo();
                hudInfo.Obj = hudHeadObject;

                hudInfo.SelfTransform = hudHeadObject.GetComponentInChildren<RectTransform>();
                hudInfo.Owner = view.gameObject.transform;
                hudInfo.DeleteTime = -1;

                m_HudHeadInfos.Add(view.gameObject.GetInstanceID(), hudInfo);
            }
        }
        public void UpdateHudHead(GameEntity entity, float hpCur, float hpMax)
        {
            UnityView view = entity.view.Value as UnityView;
            if(null != view)
            {
                HudInfo hudInfo;
                if(m_HudHeadInfos.TryGetValue(view.gameObject.GetInstanceID(), out hudInfo))
                {
                    UnityEngine.UI.Slider slider = hudInfo.Obj.GetComponentInChildren<UnityEngine.UI.Slider>();
                    if(null != slider)
                    {
                        slider.value = hpCur / hpMax;
                    }
                }
            }
        }
        public void RemoveHudHead(GameEntity entity)
        {
            UnityView view = entity.view.Value as UnityView;
            if(null != view)
            {
                HudInfo hudInfo;
                if (m_HudHeadInfos.TryGetValue(view.gameObject.GetInstanceID(), out hudInfo))
                {
                    ResourceSystem.RecycleObject(hudInfo.Obj);
                    m_HudHeadInfos.Remove(view.gameObject.GetInstanceID());
                }
            }
        }
        public void Tick(ICameraService cameraService)
        {
            float time = Time.time;
            int count = m_Damages.Count;
            for(int i = count - 1; i >= 0; i--)
            {
                DamageInfo textInfo = m_Damages[i];

                if(null == textInfo.Obj)
                {
                    m_Damages.Remove(textInfo);
                    continue;
                }

                if(textInfo.DeleteTime > 0 && time > textInfo.DeleteTime)
                {
                    ResourceSystem.RecycleObject(textInfo.Obj);
                    m_Damages.Remove(textInfo);
                }

                bool canMove = Time.time > textInfo.Delay;

                int mov = ScreenPosition(textInfo.Position, cameraService);
                if(canMove)
                {
                    textInfo.YQuickness += Time.deltaTime;
                    float floatSpeed = textInfo.FloatSpeed;
                    switch(textInfo.Movement)
                    {
                        case Guidance.Up:
                            textInfo.YCounterVail += (((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier) * textInfo.YQuickness;
                            break;
                        case Guidance.Down:
                            textInfo.YCounterVail -= (((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier) * textInfo.YQuickness;
                            break;
                        case Guidance.Left:
                            textInfo.XCounterVail -= ((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier;
                            break;
                        case Guidance.Right:
                            textInfo.XCounterVail += ((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier;
                            break;
                        case Guidance.RightUp:
                            textInfo.YCounterVail += (((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier) * textInfo.YQuickness;
                            textInfo.XCounterVail += ((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier;
                            break;
                        case Guidance.RightDown:
                            textInfo.YCounterVail -= (((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier) * textInfo.YQuickness;
                            textInfo.XCounterVail += ((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier;
                            break;
                        case Guidance.LeftUp:
                            textInfo.YCounterVail += (((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier) * textInfo.YQuickness;
                            textInfo.XCounterVail -= ((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier;
                            break;
                        case Guidance.LeftDown:
                            textInfo.YCounterVail -= (((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier) * textInfo.YQuickness;
                            textInfo.XCounterVail -= ((Time.deltaTime * floatSpeed) * mov) * m_FactorMultiplier;
                            break;
                    }
                }

                Vector2 viewPosition = cameraService.WorldToViewportPoint(textInfo.Position);
                Vector2 finalPosition = new Vector2(viewPosition.x * 0.5f + textInfo.XCounterVail, -viewPosition.y - textInfo.YCounterVail);

                textInfo.SelfTransform.anchorMax = viewPosition;
                textInfo.SelfTransform.anchorMin = viewPosition;
                textInfo.SelfTransform.anchoredPosition = finalPosition;
            }
            foreach(var pair in m_HudHeadInfos)
            {
                int id = pair.Key;
                HudInfo hudInfo = pair.Value;

                Vector2 screenPosition = cameraService.WorldToViewportPoint(hudInfo.Owner.position + Vector3.up * 2);

                hudInfo.SelfTransform.anchorMax = screenPosition;
                hudInfo.SelfTransform.anchorMin = screenPosition;
                hudInfo.SelfTransform.anchoredPosition = new Vector2(screenPosition.x * 0.5f, -screenPosition.y);
            }
        }

        private int ScreenPosition(Vector3 pos, ICameraService cameraService)
        {
            return (int)(cameraService.WorldToScreenPoint(pos).y - cameraService.WorldToScreenPoint(pos - Vector3.up * 2).y);
        }

        private float m_FloatSpeed = 12.7f;
        private float m_DelayStay = 0.3f;
        private float m_FactorMultiplier = 0.05f;

        private Transform m_UIRoot;
        private List<DamageInfo> m_Damages = new List<DamageInfo>();
        private Dictionary<int, HudInfo> m_HudHeadInfos = new Dictionary<int, HudInfo>();
      }
}
