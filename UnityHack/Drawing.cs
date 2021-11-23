using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHack
{
	public static class Drawing
	{
		public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

		private static Texture2D lineTex = new Texture2D(1, 1);

		//public static Color Color
		//{
		//	get { return GUI.color; }
		//	set { GUI.color = value; }
		//}

		public static void DrawSolidBox(Vector2 position, Vector2 size, bool centered = true)
		{
			Vector2 upperLeft = centered ? position - size / 2f : position;
			GUI.DrawTexture(new Rect(upperLeft, size), Texture2D.whiteTexture, ScaleMode.StretchToFill);
		}

		public static void DrawString(Vector2 position, string label, bool centered = true)
		{
			var content = new GUIContent(label);
			var size = StringStyle.CalcSize(content);
			var upperLeft = centered ? position - size / 2f : position;
			GUI.Label(new Rect(upperLeft, size), content);
		}

		public static void DrawLine(Vector2 a, Vector2 b, float thickness)
		{
			Matrix4x4 matrix = GUI.matrix;

			float angle = Vector3.Angle(b - a, Vector2.right);
			//angle = Vector2.SignedAngle(a, b);

			if (a.y > b.y) angle = -angle;

			GUIUtility.ScaleAroundPivot(new Vector2((b - a).magnitude, thickness), new Vector2(a.x, a.y + 0.5f));
			GUIUtility.RotateAroundPivot(angle, a);
			GUI.DrawTexture(new Rect(a.x, a.y, 1, 1), lineTex);

			GUI.matrix = matrix;

			/*
			Ignoring invalid matrix assinged to GUI.matrix - the matrix needs to be invertible. Did you scale by 0 on Z-axis?
			UnityEngine.GUIUtility:ScaleAroundPivot (UnityEngine.Vector2,UnityEngine.Vector2)
			Render:DrawLine (UnityEngine.Vector2,UnityEngine.Vector2,single) (at Assets/Scripts/BBR/Render.cs:39)
			*/
		}

		public static void DrawBox(Vector2 position, Vector2 size, float thickness, bool centered = true)
		{
			Vector2 upperLeft = centered ? position - size / 2f : position;
			DrawBox(upperLeft.x, upperLeft.y, size.x, size.y, thickness);
		}

		public static void DrawBox(float x, float y, float w, float h, float thickness)
		{
			Vector2 a = new Vector2(x, y);
			Vector2 b = new Vector2(x + w, y);
			Vector2 c = new Vector2(x + w, y + h);
			Vector2 d = new Vector2(x, y + h);

			DrawLine(a, b, thickness);
			DrawLine(b, c, thickness);
			DrawLine(c, d, thickness);
			DrawLine(d, a, thickness);
		}
	}
}
