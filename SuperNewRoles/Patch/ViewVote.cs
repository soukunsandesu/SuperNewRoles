using HarmonyLib;
using Hazel;
using SuperNewRoles.CustomOption;
using SuperNewRoles.CustomRPC;
using SuperNewRoles.Roles;
using SuperNewRoles.Mode;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperNewRoles.Patch
{
    public static class ViewVote
    {
        public static GameOptionsData OptionData;
        public static void ViewVoteSettings(this PlayerControl player)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (!ModeHandler.isMode(ModeId.Default)) return;
            var role = player.getRole();
            var optdata = OptionData.DeepCopy2();
            switch (role)
            {
                case RoleId.God:
                    optdata.AnonymousVotes = !RoleClass.God.IsVoteView;
                    break;
                case RoleId.Observer:
                    optdata.AnonymousVotes = !RoleClass.Observer.IsVoteView;
                    break;
                    foreach (PlayerControl p in RoleClass.Observer.ObserverPlayer)
                    {
                        if (p.isDead())
                        {
                            optdata.AnonymousVotes = !RoleClass.SubObserver.IsVoteView;
                        }
                    }
            }
            if (player.isDead()) optdata.AnonymousVotes = false;
            optdata.RoleOptions.ShapeshifterLeaveSkin = false;
            if (player.AmOwner) PlayerControl.GameOptions = optdata;
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SyncSettings, SendOption.Reliable, player.getClientId());
            writer.WriteBytesAndSize(optdata.ToBytes(5));
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public static float KillCoolSet(float cool)
        {
            if (cool <= 0)
            {
                return 0.001f;
            } else
            {
                return cool;
            }
        }

        public static void ViewVoteSettings()
        {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (!p.Data.Disconnected && p.IsPlayer())
                {
                    ViewVoteSettings(p);
                }
            }
        }
        public static GameOptionsData DeepCopy2(this GameOptionsData opt)
        {
            var optByte = opt.ToBytes(5);
            return GameOptionsData.FromBytes(optByte);
        }
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.StartGame))]
        public class StartGame
        {
            public static void Prefix()
            {
             //   BotHandler.CreateBot();
            }
            public static void Postfix()
            {
                if (!AmongUsClient.Instance.AmHost) return;
                OptionData = PlayerControl.GameOptions.DeepCopy2();
            }
        }
    }
}
