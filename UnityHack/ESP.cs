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

		private int m_DistanceThreshold = 100;

		private Color m_EnemyColor = Color.red;
		private Color m_TeamColor = Color.green;

		private Camera m_Camera = null;
		private List<PlayerNetwork> m_Players = new List<PlayerNetwork>();
		private List<SerializedPlayer> m_SerializedPlayers = new List<SerializedPlayer>();

		private SerializedPlayer m_SelectedPlayer = null;

		private IEnumerator Start()
		{
			var wait = new WaitForSeconds(2);

			while (true)
			{
				Cache();
				yield return wait;
			}
		}

		private void Cache()
		{
			m_Camera = Camera.main;
			m_Players.Clear();
			m_Players.AddRange(ActiveRoom.IterateAllPlayers(true));

			m_SerializedPlayers.Clear();
			for (int i = 0; i < m_Players.Count; i++)
			{
				m_SerializedPlayers.Add(SerializedPlayer.FromNetwork(m_Players[i]));
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
			GUILayout.Label($"{nameof(m_DistanceThreshold)} - {m_DistanceThreshold}");
			//m_DistanceThreshold = (int)GUILayout.HorizontalSlider(m_DistanceThreshold, 50, 200);
		}

		private void DrawPlayerList()
		{
			GUILayout.Box($"Found {m_Players.Count} players");

			for (int i = 0; i < m_SerializedPlayers.Count; i++)
			{
				if (GUILayout.Button(m_SerializedPlayers[i].FullName))
				{
					m_SelectedPlayer = m_SerializedPlayers[i];
				}
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

			Vector3 headPos = player.State.HeadPosition;
			Vector3 screenPos = m_Camera.WorldToScreenPoint(headPos);
			if (screenPos.z < 0) return;
			screenPos.y = Screen.height - screenPos.y;

			GUI.color = sameTeam ? m_TeamColor : m_EnemyColor;

			string fullName = player.Owner.FullName;
			float health = player.State.Health;

			string text = $"{fullName}\n{health}\n{distance}";
			Drawing.StringStyle.alignment = TextAnchor.MiddleCenter;
			Drawing.DrawString(screenPos, text);
		}
	}
}
