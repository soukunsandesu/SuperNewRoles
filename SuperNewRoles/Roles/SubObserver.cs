using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using SuperNewRoles.Patches;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SuperNewRoles;
using SuperNewRoles.Roles;
using SuperNewRoles.Helpers;

namespace SuperNewRoles.Roles
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    public class SubObserver
    {
        //生存判定
        public static bool ObserverAlive()
        {
            foreach (PlayerControl p in RoleClass.Observer.ObserverPlayer)
            {
                if (p.isAlive())
                {
                    SuperNewRolesPlugin.Logger.LogInfo("選管が生きていると判定されました");
                    RoleClass.SubObserver.IsVoteView = false;
                    return true;
                }
            }
            SuperNewRolesPlugin.Logger.LogInfo("選管が生きていないと判定されました");
            return false;
        }
    }
}
