using System;
using UnityEngine;

namespace SuperNewRoles.Roles
{
    public class NiceHawk
    {
        public static void TimerEnd()
        {
            /**
            if (PlayerControl.LocalPlayer.isRole(CustomRPC.RoleId.Hawk))
            {
                MapBehaviour.Instance.Close();
                FastDestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(true);
                FastDestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActive(true);
                FastDestroyableSingleton<HudManager>.Instance.SabotageButton.gameObject.SetActive(true);
            }
            **/
        }
        public class FixedUpdate
        {
            public static void Postfix()
            {
                if (RoleClass.NiceHawk.Timer >= 0.1 && !RoleClass.IsMeeting)
                {
                    Camera.main.orthographicSize = RoleClass.NiceHawk.CameraDefault * 3f;
                    FastDestroyableSingleton<HudManager>.Instance.UICamera.orthographicSize = RoleClass.NiceHawk.Default * 3f;
                }
                else
                {
                    Camera.main.orthographicSize = RoleClass.NiceHawk.CameraDefault;
                    FastDestroyableSingleton<HudManager>.Instance.UICamera.orthographicSize = RoleClass.NiceHawk.Default;
                }
                if (RoleClass.NiceHawk.timer1 >= 0.1 && !RoleClass.IsMeeting)
                {
                    var TimeSpanDate = new TimeSpan(0, 0, 0, (int)10);
                    RoleClass.NiceHawk.timer1 = (float)((Roles.RoleClass.NiceHawk.Timer2 + TimeSpanDate) - DateTime.Now).TotalSeconds;
                    CachedPlayer.LocalPlayer.transform.localPosition = RoleClass.NiceHawk.Postion;
                    SuperNewRolesPlugin.Logger.LogInfo(RoleClass.NiceHawk.timer1);
                }
            }
        }
    }
}
