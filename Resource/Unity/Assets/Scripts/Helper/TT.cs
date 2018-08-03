using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TT : MonoBehaviour {

    // Use this for initialization
    GameObject m_Root;
    Dictionary<GameObject, Vector3> m_ChildrenPos = new Dictionary<GameObject, Vector3>();
    Dictionary<GameObject, Vector3> m_ChildrenScale = new Dictionary<GameObject, Vector3>();
    Dictionary<GameObject, Quaternion> m_ChildrenRotation = new Dictionary<GameObject, Quaternion>();
	void Start () {
        m_Root = this.gameObject;
        m_ChildrenPos.Clear();
        int childCount = gameObject.transform.childCount;
        for(int i = 0; i < childCount; ++i)
        {
            Transform child = gameObject.transform.GetChild(i);
            if (null != child)
            {
                m_ChildrenRotation.Add(child.gameObject, child.rotation);
                m_ChildrenPos.Add(child.gameObject, child.position);
                m_ChildrenScale.Add(child.gameObject, child.localScale);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        foreach(KeyValuePair<GameObject, Vector3> pair in m_ChildrenPos)
        {
            GameObject obj = pair.Key;
            Vector3 wp = pair.Value;
            if(null != obj)
            {
                obj.transform.position = wp;
            }
        }
        foreach(KeyValuePair<GameObject, Quaternion> pair in m_ChildrenRotation)
        {
            GameObject obj = pair.Key;
            Quaternion wp = pair.Value;
            if(null != obj)
            {
                obj.transform.rotation = wp;
            }
        }
        foreach(KeyValuePair<GameObject, Vector3> pair in m_ChildrenScale)
        {
            GameObject obj = pair.Key;
            Vector3 wp = pair.Value;
            if(null != obj)
            {
                obj.transform.localScale = wp;
            }
        }
	}
}
