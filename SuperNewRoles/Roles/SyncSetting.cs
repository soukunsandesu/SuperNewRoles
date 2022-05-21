using HarmonyLib;
using Hazel;
using SuperNewRoles.CustomOption;
using SuperNewRoles.CustomRPC;
using SuperNewRoles.Mode;
using SuperNewRoles.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperNewRoles.Roles
{
    public static class SNRSyncSetting
    {
        public static GameOptionsData OptionData;
        public static void CustomSyncSettings(this PlayerControl player)
        {
            if (!ModeHandler.isMode(ModeId.Default)) return;
            var role = player.getRole();
            var optdata = OptionData.SNRDeepCopy();
            switch (role)
            {
                case RoleId.God:
                    optdata.AnonymousVotes = !RoleClass.God.IsVoteView;
                    break;
                case RoleId.Observer:
                    optdata.AnonymousVotes = !RoleClass.Observer.IsVoteView;
                    break;
                case RoleId.EvilObserver:
                    optdata.AnonymousVotes = !RoleClass.EvilObserver.IsVoteView;
                    break;
            }
            if (player.isDead()) optdata.AnonymousVotes = false;
            optdata.RoleOptions.ShapeshifterLeaveSkin = false;
            if (player.AmOwner) PlayerControl.GameOptions = optdata;
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SyncSettings, SendOption.Reliable, player.getClientId());
            writer.WriteBytesAndSize(optdata.ToBytes(5));
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public static void CustomSyncSettings()
        {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (!p.Data.Disconnected && p.IsPlayer())
                {
                    CustomSyncSettings(p);
                }
            }
        }
        public static GameOptionsData SNRDeepCopy(this GameOptionsData opt)
        {
            var optByte = opt.ToBytes(5);
            return GameOptionsData.FromBytes(optByte);
        }
    }
}
