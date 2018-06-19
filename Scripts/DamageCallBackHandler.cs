using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVAKIN.Mod.Elite
{
    public static class DamageCallBackHandler
    {
        public static void OnDamage(DamageInfo damageInfo)
        {
            if (damageInfo != null)
            {
                SendEventHandler.SendDamageInfoEvent(damageInfo);

                if (damageInfo.didResultInKill == true)
                {
                    SendKillMessage(damageInfo);
                    EventCallBacks.OnPlayerDeathCallBack(damageInfo.damageDestination);
                }
            }
        }

        private static void SendKillMessage(DamageInfo damageInfo)
        {
            Player sourcePlayer = PlayerManager.Instance.PlayerFromGuid(damageInfo.damageSource);
            Player destinationPlayer = PlayerManager.Instance.PlayerFromGuid(damageInfo.damageDestination);

            if (destinationPlayer != null)
            {
                string message = null;

                if (sourcePlayer == destinationPlayer)
                {
                    //Killed Self
                    message = sourcePlayer.displayName + " /wrist";
                }
                else if (sourcePlayer == null)
                {
                    //From Server
                    message = sourcePlayer.displayName + " died.";
                }
                else if (sourcePlayer != null)
                {
                    //From another Player
                    message = sourcePlayer.displayName + " killed " + destinationPlayer.displayName;
                }

                if (message != null)
                    SendEventHandler.SendToastMessageEvent(message, 0, null);
            }
        }
    }
}
