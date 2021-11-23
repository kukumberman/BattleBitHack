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
		}

		//private void OnGUI()
		//{
		//	GUI.Button(new Rect(100, 100, 200, 100), "Cucumber");
		//}
	}
}
