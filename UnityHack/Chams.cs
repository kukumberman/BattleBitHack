using System;
using System.Collections;
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

		private List<TestRangeTarget> m_Targets = new List<TestRangeTarget>();

		private Camera m_Camera = null;

		private IEnumerator Start()
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

			while (true)
			{
				m_Camera = Camera.main;
				//m_Camera = MainCamera.Instance.CameraComponent;
				m_Targets.Clear();
				m_Targets.AddRange(FindObjectsOfType(typeof(TestRangeTarget)) as TestRangeTarget[]);

				yield return new WaitForSeconds(2);
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Keypad5))
			{
				isNormal = !isNormal;

				ReplaceTargets();
			}
		}

		private void OnGUI()
		{
			DrawTargets();
		}

		public void DrawTargets()
		{
			if (m_Targets.Count == 0) return;

			for (int i = 0; i < m_Targets.Count; i++)
			{
				var target = m_Targets[i];

				if (!target) continue;

				Vector3 pos = m_Camera.WorldToScreenPoint(target.transform.position);
				if (pos.z < 0) continue;
				pos.y = Screen.height - pos.y;

				GUI.color = Color.red;
				//Drawing.DrawBox(pos, Vector2.one * 100, 1);

				float distance = Vector3.Distance(target.transform.position, m_Camera.transform.position);
				string text = $"{target.name}\n{distance}";
				Drawing.DrawString(pos, text);
			}
		}

		public void ReplaceTargets()
		{
			//var targets = FindObjectsOfType(typeof(TestRangeTarget)) as TestRangeTarget[];

			Material material = isNormal ? defaultMaterial : moddedMaterial;

			foreach (var target in m_Targets)
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
