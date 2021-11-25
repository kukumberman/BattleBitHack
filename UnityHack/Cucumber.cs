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
		private GameObject m_Camera = null;

		private bool m_IsFlying = false;

		private void Start()
		{
			string message = "Cucumber started";
			Debug.Log(message);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Keypad9))
			{
				m_IsFlying = !m_IsFlying;

				if (m_IsFlying)
				{
					m_Camera = new GameObject("1");
					m_Camera.transform.position = new Vector3(0, 50, 0);
					m_Camera.AddComponent<SimpleCameraController>().SetEulers(45, 0);
					m_Camera.AddComponent<Camera>();
				}
				else
				{
					Destroy(m_Camera);
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
		}

		//private void OnGUI()
		//{
		//	GUI.Button(new Rect(100, 100, 200, 100), "Cucumber");
		//}
	}
}
