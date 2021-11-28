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

		//private Material mat = null;

		private IEnumerator Start()
		{
			//shader = Shader.Find("Unlit/Color"); // null
			//shader = Shader.Find("Hidden/Internal-GUITexture"); // weird
			//Debug.Log($"shader is supported - {shader.isSupported}");

			//moddedMaterial = GetMat(Color.green);
			//moddedMaterial = null; // simply making null to achieve magenta color

			moddedMaterial = GetInternalColored(Color.green);

			while (true)
			{
				m_Camera = Camera.main;
				//m_Camera = MainCamera.Instance.CameraComponent;
				m_Targets.Clear();
				m_Targets.AddRange(FindObjectsOfType(typeof(TestRangeTarget)) as TestRangeTarget[]);

				yield return new WaitForSeconds(2);
			}
		}

		//public void OnPostRender()
		//{
		//	if (!mat)
		//	{
		//		// Unity has a built-in shader that is useful for drawing
		//		// simple colored things. In this case, we just want to use
		//		// a blend mode that inverts destination colors.
		//		var shader = Shader.Find("Hidden/Internal-Colored");
		//		mat = new Material(shader);
		//		mat.hideFlags = HideFlags.HideAndDontSave;
		//		// Set blend mode to invert destination colors.
		//		mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
		//		mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
		//		// Turn off backface culling, depth writes, depth test.
		//		mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
		//		mat.SetInt("_ZWrite", 0);
		//		mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
		//	}

		//	GL.PushMatrix();
		//	GL.LoadOrtho();

		//	// activate the first shader pass (in this case we know it is the only pass)
		//	mat.SetPass(0);
		//	// draw a quad over whole screen
		//	GL.Begin(GL.QUADS);
		//	GL.Vertex3(0, 0, 0);
		//	GL.Vertex3(1, 0, 0);
		//	GL.Vertex3(1, 1, 0);
		//	GL.Vertex3(0, 1, 0);
		//	GL.End();

		//	GL.PopMatrix();
		//}

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

				float distance = Vector3.Distance(target.transform.position, m_Camera.transform.position);
				string text = $"{target.name}\n{distance}";
				//Drawing.DrawString(pos, text);

				//Drawing.DrawBox(pos, Vector2.one * 100, 1);
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

		private Material GetInternalColored(Color color)
		{
			// doesnot displaying by default
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			Material mat = new Material(shader);
			mat.hideFlags = HideFlags.HideAndDontSave;
			// Set blend mode to invert destination colors.
			mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
			mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			// Turn off backface culling, depth writes, depth test.
			mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			mat.SetInt("_ZWrite", 0);
			mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
			mat.SetColor("_Color", color);
			return mat;
		}

		private Shader LoadFromBundle(string name)
		{
			// ThermalFriend
			// ThermalEnemy
			// shaders from bundle is not supported
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/shaders";
			AssetBundle bundle = AssetBundle.LoadFromFile(path);
			Shader shader = bundle.LoadAsset<Shader>(name);
			return shader;
		}
	}
}
