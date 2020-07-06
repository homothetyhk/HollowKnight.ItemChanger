using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger
{
    public struct Platform
    {
        public static Dictionary<string, List<Platform>> Platforms;

        public string sceneName;
        public float x;
        public float y;

        public void Deploy()
        {
            GameObject plat = ObjectCache.SmallPlatform;
            plat.transform.position = new Vector3(x, y);
            plat.SetActive(true);
        }
    }
}
