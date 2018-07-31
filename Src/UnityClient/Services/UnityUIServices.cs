using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityClient
{
    public class UnityUIServices : Service
    {
        public UnityUIServices(Contexts contexts) : base(contexts)
        {
        }
        public void LoadUI(string path)
        {
            GameObject o =Resources.Load(path) as GameObject;
            GameObject ui = GameObject.Instantiate(o);
        }
    }
}
