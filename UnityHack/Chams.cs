using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHack
{
	public class Chams : MonoBehaviour
	{
		private Material defaultMaterial = null;
		private Material moddedMaterial = null;

		private bool isNormal = true;

		private Shader shader = null;

		private void Start()
		{
			//shader = Shader.Find("Hidden/Internal-Colored");
			//shader = Shader.Find("Unlit/Color");
			shader = Shader.Find("Hidden/Internal-GUITexture");
			
			moddedMaterial = GetMat(Color.green);

			//string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/shaders";
			//AssetBundle bundle = AssetBundle.LoadFromFile(path);
			//shader = bundle.LoadAsset<Shader>("ThermalEnemy");
			//moddedMaterial = new Material(shader);

			Debug.Log($"shader is supported - {shader.isSupported}");
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Keypad5))
			{
				isNormal = !isNormal;

				ReplaceTargets();
			}
		}

		public void ReplaceTargets()
		{
			var targets = FindObjectsOfType(typeof(TestRangeTarget)) as TestRangeTarget[];

			Material material = isNormal ? defaultMaterial : moddedMaterial;

			foreach (var target in targets)
			{
				var renderer = target.GetComponentInChildren<MeshRenderer>();
				if (!defaultMaterial)
				{
					defaultMaterial = renderer.material;
				}
				renderer.sharedMaterial = material;

				//renderer.sharedMaterial.SetInt("_ZTest", isNormal ? 4 : 8);
			}
		}

		private Material GetMat(Color color)
		{
			Material mat = new Material(shader);
			
			//mat.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;
			//mat.SetInt("_Cull", 0);
			//mat.SetInt("_ZWrite", 0);
			//mat.SetInt("_ZTest", 0);
			mat.SetColor("_Color", color);
			return mat;
		}
	}
}
