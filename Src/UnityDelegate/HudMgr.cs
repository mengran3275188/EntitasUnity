using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityDelegate
{
    internal class HudMgr : Util.Singleton<HudMgr>
    {
        private class HudInfo
        {
            public GameObject HudObject;
            public RectTransform SelfTranform;
            public Transform Owner;
            public float DeleteTime;
        }
        public void OnStart()
        {
            m_UIRoot = GameObject.Find("GameLogic/Canvas");
        }
        public void AddHudText(string text, Transform owner, long remainTime)
        {
            var hudObject = ResourceSystem.NewObject("HudText") as GameObject;
            hudObject.transform.parent = m_UIRoot.transform;
            hudObject.transform.localPosition = Vector3.zero;

            HudInfo hudInfo = new HudInfo();
            hudInfo.HudObject = hudObject;
            UnityEngine.UI.Text uiText = hudObject.GetComponentInChildren<UnityEngine.UI.Text>();
            if(null != uiText)
                uiText.text = text;

            hudInfo.SelfTranform = hudObject.GetComponentInChildren<RectTransform>();
            hudInfo.Owner = owner;
            hudInfo.DeleteTime = Time.time + remainTime / 1000.0f;

            m_Texts.Add(hudInfo);
        }
        public void AddHudHead(uint resId, Transform owner)
        {
            var hudHeadObject = ResourceSystem.NewObject("HudHead") as GameObject;
            hudHeadObject.transform.parent = m_UIRoot.transform;

            HudInfo hudInfo = new HudInfo();
            hudInfo.HudObject = hudHeadObject;

            hudInfo.SelfTranform = hudHeadObject.GetComponentInChildren<RectTransform>();
            hudInfo.Owner = owner;
            hudInfo.DeleteTime = -1;

            m_Texts.Add(hudInfo);
            m_HudHeadInfos.Add(resId, hudInfo);
        }
        public void UpdateHudHead(uint resId, float hpCur, float hpMax)
        {
            HudInfo hudInfo;
            if(m_HudHeadInfos.TryGetValue(resId, out hudInfo))
            {
                UnityEngine.UI.Slider slider = hudInfo.HudObject.GetComponentInChildren<UnityEngine.UI.Slider>();
                if(null != slider)
                {
                    slider.value = hpCur / hpMax;
                }
            }
        }
        public void RemoveHudHead(uint resId)
        {
            HudInfo hudInfo;
            if(m_HudHeadInfos.TryGetValue(resId, out hudInfo))
            {
                m_Texts.Remove(hudInfo);
            }
            m_HudHeadInfos.Remove(resId);
        }
        public void Tick()
        {
            float time = Time.time;
            Camera mainCamera = CameraManager.Instance.GetMainCamera();
            if (null == mainCamera)
                return;

            int count = m_Texts.Count;
            for(int i = count - 1; i >= 0; i--)
            {
                HudInfo textInfo = m_Texts[i];

                if(null == textInfo.HudObject)
                {
                    m_Texts.Remove(textInfo);
                    continue;
                }

                if(null == textInfo.Owner)
                {
                    m_Texts.Remove(textInfo);
                    ResourceSystem.RecycleObject(textInfo.HudObject);
                    continue;
                }
                if(textInfo.DeleteTime > 0 && time > textInfo.DeleteTime)
                {
                    ResourceSystem.RecycleObject(textInfo.HudObject);
                    m_Texts.Remove(textInfo);
                }

                Vector2 screenPosition = mainCamera.WorldToViewportPoint(textInfo.Owner.position + Vector3.up * 2);

                textInfo.SelfTranform.anchorMax = screenPosition;
                textInfo.SelfTranform.anchorMin = screenPosition;
                textInfo.SelfTranform.anchoredPosition = new Vector2(screenPosition.x * 0.5f, -screenPosition.y);

            }
        }


        private GameObject m_UIRoot;
        private List<HudInfo> m_Texts = new List<HudInfo>();
        private Dictionary<uint, HudInfo> m_HudHeadInfos = new Dictionary<uint, HudInfo>();
    }
}
