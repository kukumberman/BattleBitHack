using System;
using System.Collections.Generic;
using System.Linq;
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
		}

		private void OnGUI()
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
