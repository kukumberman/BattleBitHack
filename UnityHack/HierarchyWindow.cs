using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityHack;

public class HierarchyWindow : MonoBehaviour
{
	private List<Transform> sceneTopObjects = new List<Transform>();
	private List<Transform> currentChain = new List<Transform>();
	private List<Transform> display = new List<Transform>();
	private List<Component> components = new List<Component>();

	private Rect mainWindowRect = new Rect(0, 0, 0, 0);
	private Vector2 mainWindowSize = new Vector2(1000, 750);
	private Vector2 mainWindowMinimizedSize = new Vector2(250, 100);

	private bool isWindowOpen = true;

	private Pagination hierarchyPagination = new Pagination(10);

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

		display.AddRange(sceneTopObjects);

		hierarchyPagination.UpdateLength(display.Count);
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

			GUILayout.BeginHorizontal();

			DrawDisplay();

			DrawComponentsList();

			GUILayout.EndHorizontal();
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
		GUILayout.BeginVertical(GUILayout.Width(500));

		GUILayout.Box("Hierarchy");

		var selected = hierarchyPagination.Paginate(display, out int start);

		for (int i = 0; i < selected.Count; i++)
		{
			if (DrawDisplayEntry(start + i))
			{
				break;
			}
		}

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("<"))
		{
			hierarchyPagination.Back();
		}

		GUILayout.Label($"{hierarchyPagination.CurrentIndex + 1}/{hierarchyPagination.TotalPages()}");

		if (GUILayout.Button(">"))
		{
			hierarchyPagination.Next();
		}

		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	private bool DrawDisplayEntry(int index)
	{
		Transform entry = display[index];

		GUILayout.BeginHorizontal();

		if (entry.childCount > 0)
		{
			if (GUILayout.Button("▶", GUILayout.ExpandWidth(false)))
			{
				HandleDisplayClick(index);

				hierarchyPagination.MoveToStart();

				return true;
			}
		}
		else
		{
			GUILayout.Space(30);
		}

		//GUILayout.Label(entry.name);
		GUILayout.Label($"[{index}] {entry.name}");

		if (GUILayout.Button("A", GUILayout.ExpandWidth(false)))
		{
			entry.gameObject.SetActive(!entry.gameObject.activeSelf);
		}

		if (GUILayout.Button("#", GUILayout.ExpandWidth(false)))
		{
			FetchComponents(index);
		}

		GUILayout.EndHorizontal();

		return false;
	}

	private void DrawComponentsList()
	{
		GUILayout.BeginVertical(GUILayout.Width(500));

		GUILayout.Box("Components");

		foreach (Component c in components)
		{
			if (!c) continue;

			GUILayout.Label(c.GetType().Name);
		}

		GUILayout.EndVertical();
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

		hierarchyPagination.MoveToStart();
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

		hierarchyPagination.UpdateLength(display.Count);
	}

	private void FetchComponents(int index)
	{
		Transform entry = display[index];

		components.Clear();

		components.AddRange(entry.GetComponents<Component>());
	}
}
