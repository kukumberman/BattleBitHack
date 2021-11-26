using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityHack;
using System;
using System.IO;
using SimpleJSON;

[Serializable]
public class SerializedEntry
{
	public string Name = "";
	public List<string> Components = new List<string>();
	public List<SerializedEntry> Childrens = new List<SerializedEntry>();
	public int Bullshit = 0;
}

public class HierarchyWindow : MonoBehaviour
{
	private List<Transform> sceneTopObjects = new List<Transform>();
	private List<Transform> currentChain = new List<Transform>();
	private List<Transform> display = new List<Transform>();
	private List<Component> components = new List<Component>();

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

	private void OnClickRefreshHierarchy()
	{
		sceneTopObjects.Clear();

		Run();

		HandleBreadcrumbClick(-1);

		components.Clear();
	}

	public void DrawContent()
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

		DrawHierarchyPagination();

		GUILayout.EndVertical();
	}

	private void DrawHierarchyPagination()
	{
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

		GUILayout.Label($"{index}", GUILayout.ExpandWidth(false));
		GUILayout.Label(entry.name);

		string n = entry.gameObject.activeSelf ? "Y" : "N";
		if (GUILayout.Button(n, GUILayout.ExpandWidth(false)))
		{
			entry.gameObject.SetActive(!entry.gameObject.activeSelf);
		}

		if (GUILayout.Button("JSON", GUILayout.ExpandWidth(false)))
		{
			Stringify(entry);
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

	private void Stringify(Transform obj)
	{
		// JsonUtility doesn't serializes "Childrens property" + no .dll by default in game folder
		//string json = JsonUtility.ToJson(Foo(obj);, true);
		string json = JSONEncoder.Encode(Bullshit(obj));

		string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		string path = Path.Combine(folder, $"{obj.name}.json");
		File.WriteAllText(path, json);
	}

	private SerializedEntry Foo(Transform obj)
	{
		SerializedEntry e = new SerializedEntry();
		e.Name = obj.name;

		foreach (Component c in obj.GetComponents<Component>())
		{
			if (!c) continue;

			e.Components.Add(c.GetType().Name);
		}

		for (int i = 0, length = obj.childCount; i < length; i++)
		{
			Transform child = obj.GetChild(i);

			e.Childrens.Add(Foo(child));
		}

		e.Bullshit = e.Childrens.Count;

		return e;
	}

	private JObject Bullshit(Transform obj)
	{
		Dictionary<string, JObject> keys = new Dictionary<string, JObject>();
		keys["Name"] = JObject.CreateString(obj.name);

		List<JObject> components = new List<JObject>();
		foreach (Component c in obj.GetComponents<Component>())
		{
			if (!c) continue;
			components.Add(JObject.CreateString(c.GetType().Name));
		}
		keys["Components"] = JObject.CreateArray(components);

		List<JObject> childrens = new List<JObject>();
		for (int i = 0, length = obj.childCount; i < length; i++)
		{
			Transform child = obj.GetChild(i);
			childrens.Add(Bullshit(child));
		}
		keys["Childrens"] = JObject.CreateArray(childrens);

		JObject e = JObject.CreateObject(keys);

		return e;
	}
}
