using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using SuperNewRoles.Buttons;
using SuperNewRoles.CustomOption;
using SuperNewRoles.CustomRPC;
using SuperNewRoles.Helpers;
using SuperNewRoles.Mode;
using SuperNewRoles.Mode.SuperHostRoles;
using SuperNewRoles.Roles;
using SuperNewRoles.Sabotage;
using UnityEngine;

namespace SuperNewRoles.Patch
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.StartGame))]
    public class StartGame
    {
        public static void Postfix(PlayerControl __instance)
        {
            MapOptions.RandomMap.Prefix();
            FixedUpdate.IsProDown = ConfigRoles.CustomProcessDown.Value;
        }
    }
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
    public class AbilityUpdate
    {
        public static void Postfix(AbilityButton __instance)
        {
            if (CachedPlayer.LocalPlayer.Data.Role.IsSimpleRole && __instance.commsDown.active)
            {
                __instance.commsDown.SetActive(false);
            }
        }
    }

    [HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
    class DebugManager
    {
        public static void Postfix(ControllerManager __instance)
        {
            if (AmongUsClient.Instance.GameState == AmongUsClient.GameStates.Started)
            {
                if (AmongUsClient.Instance.AmHost && Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift))
                {
                    RPCHelper.StartRPC(CustomRPC.CustomRPC.SetHaison).EndRPC();
                    RPCProcedure.SetHaison();
                    ShipStatus.RpcEndGame(GameOverReason.HumansByTask, false);
                    MapUtilities.CachedShipStatus.enabled = false;
                }
            }
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class FixedUpdate
    {
        static void setBasePlayerOutlines()
        {
            foreach (PlayerControl target in CachedPlayer.AllPlayers)
            {
                var rend = target.MyRend();
                if (target == null || rend == null) continue;
                if (rend.material.GetFloat("_Outline") == 0f) continue;
                rend.material.SetFloat("_Outline", 0f);
            }
        }

        private static bool ProDown = false;
        public static bool IsProDown;

        public static void Postfix(PlayerControl __instance)
        {
            if (__instance != PlayerControl.LocalPlayer) return;
            if (IsProDown)
            {
                ProDown = !ProDown;
                if (ProDown)
                {
                    return;
                }
            }
            if (AmongUsClient.Instance.GameState == AmongUsClient.GameStates.Started)
            {
                var MyRole = PlayerControl.LocalPlayer.getRole();
                setBasePlayerOutlines();
                VentAndSabo.VentButtonVisibilityPatch.Postfix(__instance);
                var ThisMode = ModeHandler.GetMode();
                if (ThisMode == ModeId.Default)
                {
                    SabotageManager.Update();
                    SetNameUpdate.Postfix(__instance);
                    Jackal.JackalFixedPatch.Postfix(__instance, MyRole);
                    JackalSeer.JackalSeerFixedPatch.Postfix(__instance, MyRole);
                    if (PlayerControl.LocalPlayer.isAlive())
                    {
                        if (PlayerControl.LocalPlayer.isImpostor()) { SetTarget.ImpostorSetTarget(); }
                        switch (MyRole)
                        {
                            case RoleId.Researcher:
                                Researcher.ReseUseButtonSetTargetPatch.Postfix(PlayerControl.LocalPlayer);
                                break;
                            case RoleId.Pursuer:
                                Pursuer.PursureUpdate.Postfix();
                                break;
                            case RoleId.Levelinger:
                                if (RoleClass.Levelinger.IsPower(RoleClass.Levelinger.LevelPowerTypes.Pursuer))
                                {
                                    if (!RoleClass.Pursuer.arrow.arrow.active)
                                    {
                                        RoleClass.Pursuer.arrow.arrow.SetActive(true);
                                    }
                                    Pursuer.PursureUpdate.Postfix();
                                }
                                else
                                {
                                    if (RoleClass.Pursuer.arrow.arrow.active)
                                    {
                                        RoleClass.Pursuer.arrow.arrow.SetActive(false);
                                    }
                                }
                                break;
                            case RoleId.Hawk:
                                Hawk.FixedUpdate.Postfix();
                                break;
                            case RoleId.NiceHawk:
                                NiceHawk.FixedUpdate.Postfix();
                                break;
                            case RoleId.MadHawk:
                                MadHawk.FixedUpdate.Postfix();
                                break;
                            case RoleId.Vampire:
                                Vampire.FixedUpdate.Postfix();
                                break;
                            case RoleId.DarkKiller:
                                DarkKiller.FixedUpdate.Postfix();
                                break;
                            case RoleId.Vulture:
                                Vulture.FixedUpdate.Postfix();
                                break;
                            case RoleId.Mafia:
                                Mafia.FixedUpdate();
                                break;
                            case RoleId.SerialKiller:
                                SerialKiller.FixedUpdate();
                                break;
                            default:
                                Minimalist.FixedUpdate.Postfix(MyRole);
                                break;
                        }
                    }
                    else
                    {
                        switch (MyRole)
                        {
                            case RoleId.Bait:
                                if (!RoleClass.Bait.Reported)
                                {
                                    Bait.BaitUpdate.Postfix(__instance);
                                }
                                break;
                            case RoleId.SideKiller:
                                if (!RoleClass.SideKiller.IsUpMadKiller)
                                {
                                    var sideplayer = RoleClass.SideKiller.getSidePlayer(PlayerControl.LocalPlayer);
                                    if (sideplayer != null)
                                    {
                                        sideplayer.RPCSetRoleUnchecked(RoleTypes.Impostor);
                                        RoleClass.SideKiller.IsUpMadKiller = true;

                                    }
                                }
                                break;
                        }
                    }
                }
                else if (ThisMode == ModeId.SuperHostRoles)
                {
                    Mode.SuperHostRoles.FixedUpdate.Update();
                    switch (MyRole)
                    {
                        case RoleId.Mafia:
                            Mafia.FixedUpdate();
                            break;
                    }
                    SerialKiller.SHRFixedUpdate(MyRole);
                }
                else if (ThisMode == ModeId.NotImpostorCheck)
                {
                    if (AmongUsClient.Instance.AmHost)
                    {
                        BlockTool.FixedUpdate();
                    }
                    Mode.NotImpostorCheck.NameSet.Postfix();
                }
                else
                {
                    if (AmongUsClient.Instance.AmHost)
                    {
                        BlockTool.FixedUpdate();
                    }
                    ModeHandler.FixedUpdate(__instance);
                }
            }
        }
    }
}
