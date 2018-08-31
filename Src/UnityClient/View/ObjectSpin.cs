using System;
using System.Collections.Generic;
using UnityEngine;
class ObjectSpin : MonoBehaviour
{
    public float m_DegreesX = 0;
    public float m_DegreesY = 0;
    public float m_DegreesZ = 0;

    void Start()
    {
        transform.localRotation = Quaternion.identity;
    }
    void Update()
    {
        transform.Rotate(m_DegreesX * Time.deltaTime, m_DegreesY * Time.deltaTime, m_DegreesZ * Time.deltaTime);
    }
}
