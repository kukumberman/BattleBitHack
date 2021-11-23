using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace UnityHack
{
	public class Cucumber : MonoBehaviour
	{
		private void Start()
		{
			string message = "Cucumber started";
			Debug.Log(message);
			
			//string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "cuucmber.log");
			//File.WriteAllText(path, message);
		}

		//private void OnGUI()
		//{
		//	GUI.Button(new Rect(100, 100, 200, 100), "Cucumber");
		//}
	}
}
