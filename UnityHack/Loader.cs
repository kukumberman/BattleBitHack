using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHack
{
    public class Loader
    {
        private static GameObject Instance;

        public static void Init()
        {
            Instance = new GameObject();
            Instance.AddComponent<Cucumber>();

            UnityEngine.Object.DontDestroyOnLoad(Instance);
        }

        public static void Unload()
		{
			UnityEngine.Object.Destroy(Instance);
		}
    }
}
