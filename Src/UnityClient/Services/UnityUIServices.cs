using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityClient
{
    public class UnityUIServices : Service
    {
        public UnityUIServices(Contexts contexts) : base(contexts)
        {
        }
        public void Init()
        {
            m_UIRoot = GameObject.Find("GameLogic/Canvas/TempUIRoot").transform;
        }
        public void LoadUI(string path)
        {
            GameObject o =Resources.Load(path) as GameObject;
            GameObject ui = GameObject.Instantiate(o);

            ui.transform.SetParent(m_UIRoot, false);
        }
        public void Cleanup()
        {
            for(int i = m_UIRoot.transform.childCount - 1; i >= 0; --i)
            {
                GameObject.DestroyImmediate(m_UIRoot.GetChild(i).gameObject);
            }
        }
        public void ShowGamemap(GameEntity[] chunkEntities)
        {
            Transform mapTransform = m_UIRoot.Find("GameMap");
            if (null == mapTransform)
            {
                GameObject mapObject = new GameObject();
                mapObject.name = "GameMap";
                mapObject.transform.SetParent(m_UIRoot, false);
                GenerateGameMap(mapObject.transform, chunkEntities);
            }
            else
            {
                mapTransform.gameObject.SetActive(!mapTransform.gameObject.activeSelf);
            }

        }
        private void GenerateGameMap(Transform root, GameEntity[] entities)
        {
            Bounds bounds = new Bounds();
            foreach(var chunkEntity in entities)
            {
                UnityChunk unityChunk = chunkEntity.chunk.Value as UnityChunk;
                bounds.Encapsulate(unityChunk.transform.position);
            }
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Vector3 size = max - min;

            float mapXSize = 1000;
            float mapYSize = size.z / size.x * mapXSize;

            float scale = (mapXSize / size.x) / 2;

            foreach(var chunkEntity in entities)
            {
                UnityChunk unityChunk = chunkEntity.chunk.Value as UnityChunk;
                string chunkName = unityChunk.m_ChunkName;
                Vector3 position = unityChunk.transform.position;
                float yAngle = unityChunk.transform.rotation.eulerAngles.y;

                GameObject chunkMap = Resources.Load("BlocksUI/" + chunkName) as GameObject;
                if(null == chunkMap)
                {
                    Util.LogUtil.Error("can not load chunk map {0}.", chunkName);
                    continue;
                }
                GameObject chunkMapObj = GameObject.Instantiate(chunkMap);

                chunkMapObj.transform.SetParent(root, false);

                Image chunkImage = chunkMapObj.GetComponent<Image>();

                float xPercent = (position.x - min.x) / size.x;
                float yPercent = (position.z - min.z) / size.z;

                chunkImage.rectTransform.rotation = Quaternion.Euler(0, 0, yAngle);
                chunkImage.rectTransform.anchoredPosition = new Vector2(xPercent * mapXSize - mapXSize / 2, yPercent * mapYSize - mapYSize / 2);
                chunkImage.rectTransform.localScale *= scale;
            }
        }

        private Transform m_UIRoot;
    }
}
