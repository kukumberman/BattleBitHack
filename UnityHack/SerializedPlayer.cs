using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHack
{
	class SerializedPlayer
	{
		public bool IsValid;

		public ulong SteamID;

		public float Health;
		public float DistanceToLocalPlayer;

		public Role Role;
		public Nation Nation;
		public Team Team;

		public string FullName;
		public string ChatName;
		public string Nickname;
		public string ScoreboardName;

		public string PrimaryName;
		public string SecondaryName;

		public bool IsFriendly;
		public bool IsAlive;

		public Vector3 HeadPosition;

		public SerializedPlayer(bool isValid)
		{
			IsValid = isValid;
		}

		public static SerializedPlayer FromNetwork(PlayerNetwork player)
		{
			try
			{
				return new SerializedPlayer(true)
				{
					SteamID = player.Owner.SteamID,
					Health = player.State.Health,
					DistanceToLocalPlayer = player.State.DistanceToLocalPlayer,

					Role = player.State.Role,
					Nation = player.State.Nation,
					Team = player.State.Team,

					FullName = player.Owner.FullName,
					ChatName = player.Owner.ChatName,
					Nickname = player.Owner.Nickname,
					ScoreboardName = player.Owner.ScoreboardName,

					PrimaryName = player.State.Primary.Item?.GetToolName(),
					SecondaryName = player.State.Secondary?.name,

					IsFriendly = player.Owner.IsFriendly,
					IsAlive = player.Owner.IsAlive,

					HeadPosition = player.State.HeadPosition,
				};
			}
			catch(Exception e)
			{
				Debug.Log(e);
				Debug.Log(player != null);
				Debug.Log(player.State != null);
				Debug.Log(player.Owner != null);
				return new SerializedPlayer(false);
			}
		}
	}
}
