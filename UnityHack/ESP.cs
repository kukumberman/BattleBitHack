using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHack
{
	public class ESP : MonoBehaviour
	{
		private bool m_DrawEnemies = true;
		private bool m_DrawTeam = true;
		private bool m_DrawExtendedInfo = false;

		private int m_DistanceThreshold = 200;
		private int m_FontSize = 10;

		private Color m_EnemyColor = Color.red;
		private Color m_TeamColor = Color.green;

		private Camera m_Camera = null;
		private List<PlayerNetwork> m_Players = new List<PlayerNetwork>();
		private List<SerializedPlayer> m_SerializedPlayers = new List<SerializedPlayer>();

		private SerializedPlayer m_SelectedPlayer = null;

		private GUIStyle m_TextStyle = null;

		private int m_DistanceThresholdIncremental = 10;

		private IEnumerator Start()
		{
			var wait = new WaitForSeconds(1);

			while (true)
			{
				try
				{
					Cache();
				}
				catch { }
				yield return wait;
			}
		}

		private void Cache()
		{
			m_Players.Clear();
			m_SerializedPlayers.Clear();

			m_Camera = MainCamera.Instance.CameraComponent;

			m_Players.AddRange(ActiveRoom.IterateAllPlayers(true));

			for (int i = 0; i < m_Players.Count; i++)
			{
				var player = m_Players[i];

				if (!player.isMine && !player.Owner.IsFriendly)
				{
					foreach(var rend in player.GetComponentsInChildren<SkinnedMeshRenderer>())
					{
						rend.sharedMaterial = null;
					}
				}

				m_SerializedPlayers.Add(SerializedPlayer.FromNetwork(player));
			}

			// todo change material (chams)
		}

		public void DrawContent()
		{
			DrawSettings();

			GUILayout.BeginHorizontal();

			GUILayout.BeginVertical(GUILayout.Width(500));
			DrawPlayerList();
			GUILayout.EndVertical();

			GUILayout.BeginVertical(GUILayout.Width(500));
			DrawPlayerInspector();
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();
		}

		public void DrawESP()
		{
			if (m_Camera == null) return;

			m_TextStyle = new GUIStyle(GUI.skin.label);
			m_TextStyle.fontSize = m_FontSize;
			m_TextStyle.alignment = TextAnchor.MiddleCenter;

			foreach (PlayerNetwork player in m_Players)
			{
				DrawPlayerESP(player);
			}
		}

		private void DrawSettings()
		{
			GUILayout.Box("ESP Settings");
			m_DrawEnemies = GUILayout.Toggle(m_DrawEnemies, nameof(m_DrawEnemies));
			m_DrawTeam = GUILayout.Toggle(m_DrawTeam, nameof(m_DrawTeam));
			m_DrawExtendedInfo = GUILayout.Toggle(m_DrawExtendedInfo, nameof(m_DrawExtendedInfo));

			GUILayout.Label($"{nameof(m_FontSize)} - {m_FontSize}");
			m_FontSize = DrawIncrementalLabel(nameof(m_FontSize), m_FontSize, 2);

			GUILayout.Label($"{nameof(m_DistanceThreshold)} - {m_DistanceThreshold}");
			m_DistanceThreshold = DrawIncrementalLabel(nameof(m_DistanceThreshold), m_DistanceThreshold, m_DistanceThresholdIncremental);

			// error
			//m_DistanceThreshold = (int)GUILayout.HorizontalSlider(m_DistanceThreshold, 50, 200);
		}

		private void DrawPlayerList()
		{
			GUILayout.Box($"Found {m_Players.Count} players");

			for (int i = 0; i < m_SerializedPlayers.Count; i++)
			{
				GUILayout.BeginHorizontal();

				if (GUILayout.Button(m_SerializedPlayers[i].FullName))
				{
					m_SelectedPlayer = m_SerializedPlayers[i];
				}

				if (GUILayout.Button("JSON", GUILayout.ExpandWidth(false)))
				{
					HierarchyWindow.Stringify(m_Players[i].transform);
				}

				GUILayout.EndHorizontal();
			}
		}

		private void DrawPlayerInspector()
		{
			GUILayout.Box("Inspector");

			if (m_SelectedPlayer == null)
			{
				GUILayout.Label("Player is not selected");
				return;
			}

			GUILayout.Toggle(m_SelectedPlayer.IsValid, nameof(m_SelectedPlayer.IsValid));

			GUILayout.Label($"{nameof(m_SelectedPlayer.SteamID)} - {m_SelectedPlayer.SteamID}");
			GUILayout.Label($"{nameof(m_SelectedPlayer.Health)} - {m_SelectedPlayer.Health}");
			GUILayout.Label($"{nameof(m_SelectedPlayer.DistanceToLocalPlayer)} - {m_SelectedPlayer.DistanceToLocalPlayer}");

			GUILayout.Label($"{nameof(m_SelectedPlayer.Role)} - {m_SelectedPlayer.Role}");
			GUILayout.Label($"{nameof(m_SelectedPlayer.Nation)} - {m_SelectedPlayer.Nation}");
			GUILayout.Label($"{nameof(m_SelectedPlayer.Team)} - {m_SelectedPlayer.Team}");

			GUILayout.Label($"{nameof(m_SelectedPlayer.FullName)} - {m_SelectedPlayer.FullName}");
			GUILayout.Label($"{nameof(m_SelectedPlayer.ChatName)} - {m_SelectedPlayer.ChatName}");
			GUILayout.Label($"{nameof(m_SelectedPlayer.Nickname)} - {m_SelectedPlayer.Nickname}");
			GUILayout.Label($"{nameof(m_SelectedPlayer.ScoreboardName)} - {m_SelectedPlayer.ScoreboardName}");

			GUILayout.Label($"{nameof(m_SelectedPlayer.PrimaryName)} - {m_SelectedPlayer.PrimaryName}");
			GUILayout.Label($"{nameof(m_SelectedPlayer.SecondaryName)} - {m_SelectedPlayer.SecondaryName}");

			GUILayout.Label($"{nameof(m_SelectedPlayer.IsFriendly)} - {m_SelectedPlayer.IsFriendly}");
			GUILayout.Label($"{nameof(m_SelectedPlayer.IsAlive)} - {m_SelectedPlayer.IsAlive}");
		}

		private void DrawPlayerESP(PlayerNetwork player)
		{
			float distance = player.State.DistanceToLocalPlayer;
			if (distance > m_DistanceThreshold) return;

			bool sameTeam = player.Owner.IsFriendly;

			if (sameTeam && !m_DrawTeam) return;
			if (!sameTeam && !m_DrawEnemies) return;

			Vector3 worldPos = player.transform.position; // ?
			Vector3 screenPos = m_Camera.WorldToScreenPoint(worldPos);
			if (screenPos.z < 0) return;
			screenPos.y = Screen.height - screenPos.y;

			GUI.color = sameTeam ? m_TeamColor : m_EnemyColor;

			if (m_DrawExtendedInfo)
			{
				string fullName = player.Owner.FullName;
				float health = player.State.Health;

				string text = $"{fullName}\n{health}\n{distance}";
				Drawing.DrawString(screenPos, text, m_TextStyle);
			}
			else
			{
				Drawing.DrawString(screenPos, "*", m_TextStyle);
			}
		}

		private static int DrawIncrementalLabel(string label, int value, int incrementalValue)
		{
			GUILayout.BeginHorizontal();

			GUILayout.Label(label, GUILayout.ExpandWidth(false));

			if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
			{
				value -= incrementalValue;
			}

			if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
			{
				value += incrementalValue;
			}

			GUILayout.EndHorizontal();

			return value;
		}
	}
}
