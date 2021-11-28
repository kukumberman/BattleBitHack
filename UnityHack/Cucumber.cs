using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHack
{
	public class Cucumber : MonoBehaviour
	{
		private GameObject m_FlyingCamera = null;

		private bool m_IsFlying = false;

		private ESP m_ESP = null;
		private HierarchyWindow m_Hierarchy = null;

		private bool m_IsWindowOpen = true;

		private Rect m_WindowRect = new Rect(0, 0, 1000, 700);

		private int m_ToolbarIndex = 0;
		private string[] m_ToolbarContent = { "Hierarchy", "ESP" };

		private KeyCode m_ToggleWindowKey = KeyCode.Keypad5;

		private void Start()
		{
			m_ESP = gameObject.AddComponent<ESP>();
			m_Hierarchy = gameObject.AddComponent<HierarchyWindow>();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Keypad9))
			{
				m_IsFlying = !m_IsFlying;

				if (m_IsFlying)
				{
					m_FlyingCamera = new GameObject("1");
					m_FlyingCamera.transform.position = new Vector3(0, 50, 0);
					m_FlyingCamera.AddComponent<SimpleCameraController>().SetEulers(45, 0);
					m_FlyingCamera.AddComponent<Camera>();
				}
				else
				{
					Destroy(m_FlyingCamera);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Keypad8))
			{
				var terrain = FindObjectOfType<Terrain>();
				if (terrain)
				{
					// this doesnt not work because all objects are baked
					//RemoveGrass.Remove(terrain, 0);
					//RemoveGrass.Remove(terrain, 1);
					//RemoveGrass.Remove(terrain, 2);
					//RemoveGrass.RemoveBaked(terrain);

					// removes all visual meshes, but colliders place in under "[TerrainColliders]" gameobject
					var baker = FindObjectOfType<RuntimeTerrainBaker>();
					Destroy(baker.gameObject);

					Debug.Log("removed!");
				}
				else
				{
					Debug.Log("terrain doesnot found");
				}
			}
			else if (Input.GetKeyDown(m_ToggleWindowKey))
			{
				m_IsWindowOpen = !m_IsWindowOpen;
			}

			//if (Input.GetKeyDown(KeyCode.Keypad1))
			//{
			//	BullshitMaterials();
			//}
		}

		private void BullshitMaterials()
		{
			var f = FindObjectOfType<TestRangeTarget>().GetComponentInChildren<MeshFilter>();

			GameObject cube = new GameObject();
			cube.AddComponent<MeshFilter>().sharedMesh = f.sharedMesh;
			MeshRenderer rend = cube.AddComponent<MeshRenderer>();

			var player = FindObjectOfType<OfflinePlayer>();
			cube.transform.position = player.transform.position + Vector3.up * 5;

			var shader = Shader.Find("Hidden/Internal-Colored");
			Material a = new Material(shader);
			a.SetInt("_ZWrite", 0);
			a.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
			a.SetColor("_Color", Color.red);

			Material b = new Material(shader);
			b.SetColor("_Color", Color.green);

			Material[] mats = new Material[] { a, b };

			rend.sharedMaterials[0] = a;

			//MissingMethodException: object System.Type.InvokeMember(string, System.Reflection.BindingFlags, System.Reflection.Binder, object, object[])

			/*
			Type type = rend.GetType();
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance;

			string propName = "sharedMaterials";

			PropertyInfo p = type.GetProperty(propName, flags);
			if (p == null)
			{
				throw new ArgumentOutOfRangeException(nameof(propName), string.Format("Property {0} was not found in Type {1}", propName, type.FullName));
			}

			//type.InvokeMember(propName, flags, null, rend, new object[] { mats });
			p.SetValue(rend, mats, null); // not working

			//rend.sharedMaterial = a;

			Debug.Log("SUCCESS");
			*/
		}

		private void OnGUI()
		{
			// no lines allowed in current version :(
			//Drawing.DrawLine(Vector2.zero, new Vector2(Screen.width, Screen.height), 1);

			try
			{
				if (m_IsWindowOpen)
				{
					m_WindowRect = GUI.Window(0, m_WindowRect, DrawWindow, "Main Window");
				}

				if (Event.current.type == EventType.Repaint)
				{
					m_ESP.DrawESP();
				}
			}
			catch
			{

			}
			
		}

		private void DrawWindow(int id)
		{
			DrawContent();

			GUI.DragWindow();
		}

		private void DrawContent()
		{
			//m_ToolbarIndex = GUILayout.Toolbar(m_ToolbarIndex, m_ToolbarContent);

			DrawToolbar();

			if (m_ToolbarIndex == 0)
			{
				m_Hierarchy.DrawContent();
			}
			else if (m_ToolbarIndex == 1)
			{
				m_ESP.DrawContent();
			}
		}

		private void DrawToolbar()
		{
			GUILayout.BeginHorizontal();

			for (int i = 0; i < m_ToolbarContent.Length; i++)
			{
				if (GUILayout.Button(m_ToolbarContent[i], GUILayout.ExpandWidth(false)))
				{
					m_ToolbarIndex = i;
					break;
				}
			}

			GUILayout.EndHorizontal();
		}
	}
}
