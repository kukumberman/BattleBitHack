using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHack
{
	public class RemoveGrass
	{
        public static void Remove(Terrain terrain, int layer)
		{
            int w = terrain.terrainData.detailWidth;
            int h = terrain.terrainData.detailHeight;

            int[,] map = terrain.terrainData.GetDetailLayer(0, 0, w, h, layer);

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    map[x, y] = 0;
                }
            }

            terrain.terrainData.SetDetailLayer(0, 0, layer, map);

        }

        public static void RemoveFoo(Terrain terrain)
		{
            int w = terrain.terrainData.detailWidth;
            int h = terrain.terrainData.detailHeight;

            int num = terrain.terrainData.detailPrototypes.Length;

            int[,] empty = new int[w, h];

			for (int i = 0; i < num; i++)
			{
                terrain.terrainData.SetDetailLayer(0, 0, i, empty);
            }
        }

        public static void RemoveBaked(Terrain terrain)
		{
            DetailPrototype[] grass = terrain.terrainData.detailPrototypes;

			for (int i = 0; i < grass.Length; i++)
			{
                UnityEngine.Object.Destroy(grass[i].prototype);
			}
		}
	}
}
