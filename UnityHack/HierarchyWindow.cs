using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HierarchyWindow : MonoBehaviour
{
	private List<Transform> sceneTopObjects = new List<Transform>();

	private List<Transform> currentChain = new List<Transform>();

	private List<Transform> display = new List<Transform>();

	private List<Component> components = new List<Component>();

	private Vector2 scrollDisplay = Vector2.zero;

	private Vector2 scrollComponents = Vector2.zero;

	private Rect mainWindowRect = new Rect(0, 0, 750, 500);

	private Vector2 mainWindowSize = new Vector2(750, 500);

	private Vector2 mainWindowMinimizedSize = new Vector2(250, 100);

	private bool isWindowOpen = true;

	private void Start()
	{
		Run();
	}

    private void Run()
	{
		var scene = SceneManager.GetActiveScene();

		var arr = scene.GetRootGameObjects();

		foreach(var go in arr)
		{
			sceneTopObjects.Add(go.transform);
		}

		// bullshit at runtime

		//var arr = FindObjectsOfType(typeof(GameObject)) as GameObject[];

		//foreach (var go in arr)
		//{
		//	if (go.transform.parent == null)
		//	{
		//		sceneTopObjects.Add(go.transform);
		//	}
		//}

		display.AddRange(sceneTopObjects);
	}

	private void OnGUI()
	{
		Rect rect = isWindowOpen ? new Rect(mainWindowRect.position, mainWindowSize) : new Rect(mainWindowRect.position, mainWindowMinimizedSize);

		mainWindowRect = GUI.Window(0, rect, DrawMainWindow, "Main Window");
	}

	private void OnClickRefreshHierarchy()
	{
		sceneTopObjects.Clear();

		Run();

		HandleBreadcrumbClick(-1);

		components.Clear();
	}

	private void DrawMainWindow(int id)
	{
		isWindowOpen = GUILayout.Toggle(isWindowOpen, "Show buttons");

		if (isWindowOpen)
		{
			if (GUILayout.Button($"Refresh Hierarchy {sceneTopObjects.Count}", GUILayout.ExpandWidth(false)))
			{
				OnClickRefreshHierarchy();
			}

			if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
			{
				sceneTopObjects.Clear();
				currentChain.Clear();
				display.Clear();
				components.Clear();
			}

			DrawBreadcrumb();

			DrawDisplay();

			DrawComponentsList();
		}

		GUI.DragWindow();
	}

	private void DrawBreadcrumb()
	{
		GUILayout.Box("Breadcrumb");

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("#", GUILayout.ExpandWidth(false)))
		{
			HandleBreadcrumbClick(-1);
		}

		for (int i = 0; i < currentChain.Count; i++)
		{
			string name = currentChain[i].name;

			if (GUILayout.Button(name, GUILayout.ExpandWidth(false)))
			{
				HandleBreadcrumbClick(i);
			}
		}

		GUILayout.EndHorizontal();
	}

	private void DrawDisplay()
	{
		GUILayout.Box("Hierarchy");

		//scrollDisplay = GUILayout.BeginScrollView(scrollDisplay, false, true, GUILayout.Height(250));
		//scrollDisplay = GUILayout.BeginScrollView(scrollDisplay);
		//GUILayout.BeginVertical();

		for (int i = 0; i < display.Count; i++)
		{
			DrawDisplayEntry(i);
		}

		//GUILayout.EndVertical();
		//GUILayout.EndScrollView();
	}

	private void DrawDisplayEntry(int index)
	{
		Transform entry = display[index];

		GUILayout.BeginHorizontal();

		if (entry.childCount > 0)
		{
			if (GUILayout.Button("▶", GUILayout.ExpandWidth(false)))
			{
				HandleDisplayClick(index);
			}
		}
		else
		{
			GUILayout.Space(30);
		}

		GUILayout.Label(entry.name);

		if (GUILayout.Button("A", GUILayout.ExpandWidth(false)))
		{
			entry.gameObject.SetActive(!entry.gameObject.activeSelf);
		}

		if (GUILayout.Button("#", GUILayout.ExpandWidth(false)))
		{
			FetchComponents(index);
		}

		GUILayout.EndHorizontal();
	}

	private void DrawComponentsList()
	{
		GUILayout.Box("Components");

		//scrollComponents = GUILayout.BeginScrollView(scrollComponents, false, true);

		foreach (Component c in components)
		{
			if (!c) continue;

			GUILayout.Label(c.GetType().Name);
		}

		//GUILayout.EndScrollView();
	}

	private void HandleBreadcrumbClick(int index)
	{
		if (index == -1)
		{
			display.Clear();

			display.AddRange(sceneTopObjects);

			currentChain.Clear();
		}
		else
		{
			if (index == currentChain.Count - 1)
			{
				return;
			}

			currentChain.RemoveRange(index + 1, currentChain.Count - index - 1);

			display.Clear();

			Transform clickedAt = currentChain[index];

			AddChildrensToDisplay(clickedAt);
		}
	}

	private void HandleDisplayClick(int index)
	{
		Transform clickedAt = display[index];

		int count = clickedAt.childCount;

		if (count == 0)
		{
			return;
		}

		currentChain.Add(clickedAt);

		display.Clear();

		AddChildrensToDisplay(clickedAt);
	}

	private void AddChildrensToDisplay(Transform parent)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);

			display.Add(child);
		}
	}

	private void FetchComponents(int index)
	{
		Transform entry = display[index];

		components.Clear();

		components.AddRange(entry.GetComponents<Component>());
	}
}
