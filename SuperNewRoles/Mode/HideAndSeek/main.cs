using System;
using System.Collections.Generic;
using System.Text;
using SuperNewRoles.EndGame;
using UnityEngine;
////using static SuperNewRoles.EndGame.CheckGameEndPatch;

namespace SuperNewRoles.Mode.HideAndSeek
{
    class main
    {
        public static bool EndGameCheck(ShipStatus __instance)
        {
            // (/statistics.CrewAlive == 0)
            {
                SuperNewRolesPlugin.Logger.LogInfo("[HAS]ENDDED!!!");
                __instance.enabled = false;
                ShipStatus.RpcEndGame(GameOverReason.ImpostorByKill, false);
                return true;
            }
            //else if (GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
            {
                SuperNewRolesPlugin.Logger.LogInfo("[HAS]TASKEND!");
                __instance.enabled = false;
                ShipStatus.RpcEndGame(GameOverReason.HumansByTask, false);
                return true;
            }
            //else
            {
                return false;
            }
        }
        public static void ClearAndReload() { }
    }
}
