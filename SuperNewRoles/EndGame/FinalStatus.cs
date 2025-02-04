using System;
using System.Collections.Generic;
using System.Text;
using SuperNewRoles.Patch;
using UnityEngine;

namespace SuperNewRoles.EndGame
{
    class FinalStatusPatch
    {
        public static class FinalStatusData
        {
            public static List<Tuple<Vector3, bool>> localPlayerPositions = new();
            public static List<DeadPlayer> deadPlayers = new();
            public static Dictionary<int, FinalStatus> FinalStatuses = new();

            public static void ClearFinalStatusData()
            {
                localPlayerPositions = new List<Tuple<Vector3, bool>>();
                deadPlayers = new List<DeadPlayer>();
                FinalStatuses = new Dictionary<int, FinalStatus>();
            }
        }
    }
    enum FinalStatus
    {
        Alive,
        Kill,
        Exiled,
        NekomataExiled,
        SheriffKill,
        SheriffMisFire,
        MeetingSheriffKill,
        MeetingSheriffMisFire,
        SelfBomb,
        BySelfBomb,
        Ignite,
        Disconnected,
        Dead,
        Sabotage
    }
}
