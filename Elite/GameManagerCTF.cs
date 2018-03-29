using System.Collections;
using UnityEngine;

namespace NOVAKIN.Mod.Elite
{
    public class GameManagerCTF : GameManager
    {
        private Flag flagTeam1;
        private Flag flagTeam2;
        private FlagSpawnPoint flagSpawnPointTeam1;
        private FlagSpawnPoint flagSpawnPointTeam2;

        private float flagReturnTime = 45.0f;

        #region Startup
        protected override void Start()
        {
            base.Start();

            FlagSetup();
            StartCoroutine(FlagRoutine());
        }

        protected override void RegisterCallBacks()
        {
            base.RegisterCallBacks();

            EventCallBacks.OnTouchFlag += OnTouchFlag;
            EventCallBacks.OnDroppedFlag += OnDroppedFlag;
        }

        protected override void DeRegisterCallBacks()
        {
            base.DeRegisterCallBacks();

            EventCallBacks.OnTouchFlag -= OnTouchFlag;
            EventCallBacks.OnDroppedFlag -= OnDroppedFlag;
        }
        #endregion

        #region Flag Stuff
        IEnumerator FlagRoutine()
        {
            while (true)
            {
                if (flagTeam1 != null && flagTeam1.isHome == false && flagTeam1.carrier == null &&
                    flagTeam1.droppedTime + flagReturnTime < BoltNetwork.serverTime)
                    ReturnFlagHome(flagTeam1, false);


                if (flagTeam2 != null && flagTeam2.isHome == false && flagTeam2.carrier == null &&
                    flagTeam2.droppedTime + flagReturnTime < BoltNetwork.serverTime)
                    ReturnFlagHome(flagTeam2, false);

                yield return new WaitForFixedUpdate();
            }
        }

        protected void FlagSetup()
        {
            foreach (FlagSpawnPoint flagSpawnPoint in FindObjectsOfType<FlagSpawnPoint>())
            {
                if (flagSpawnPoint.teamID == 1)
                {
                    flagSpawnPointTeam1 = flagSpawnPoint;

                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Flag);
                    flagTeam1 = entity.GetComponent<Flag>();
                    flagTeam1.flagSpawnPoint = flagSpawnPoint;
                    flagTeam1.teamID = flagSpawnPoint.teamID;
                    flagTeam1.gameObject.AddComponent<FlagMotor>();

                    ReturnFlagHome(flagTeam1, true);
                }
                else if (flagSpawnPoint.teamID == 2)
                {
                    flagSpawnPointTeam2 = flagSpawnPoint;

                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Flag);
                    flagTeam2 = entity.GetComponent<Flag>();
                    flagTeam2.flagSpawnPoint = flagSpawnPoint;
                    flagTeam2.teamID = flagSpawnPoint.teamID;
                    flagTeam2.gameObject.AddComponent<FlagMotor>();

                    ReturnFlagHome(flagTeam2, true);
                }
            }

            if (flagSpawnPointTeam1 == null)
            {
                Debug.LogError("No FlagSpawnPoint found for team 1");
            }

            if (flagSpawnPointTeam2 == null)
            {
                Debug.LogError("No FlagSpawnPoint found for team 2");
            }
        }

        protected void OnTouchFlag(Flag flag, FlagCarrier flagCarrier)
        {
            if (flag != null && flagCarrier != null && flagCarrier.Robot != null && flag == flagTeam1 || flag == flagTeam2)
            {
                if (flagCarrier.Robot.teamId != flag.teamID && flagCarrier.LastTossTime + 0.5f <= BoltNetwork.serverTime)
                {
                    FlagTaken(flag, flagCarrier);
                }
                else if (flag.isHome == true && flagCarrier.Robot.teamId == flag.teamID && flagCarrier.HeldFlag != null)
                {
                    ReturnFlagHome(flagCarrier.HeldFlag, true);
                    CaptureFlag(flagCarrier.HeldFlag, flagCarrier);
                }
                else if (flag.isHome == false && flagCarrier.Robot.teamId == flag.teamID)
                {
                    ReturnFlagHome(flag, false);
                }
            }
        }


        protected void ReturnFlagHome(Flag flag, bool silent)
        {
            if (flag != null && flag.entity != null && flag.entity.isAttached && flag.entity.isOwner)
            {
                flag.isHome = true;
                flag.carrier = null;

                if (flag.teamID == 1)
                {
                    flag.transform.position = flagSpawnPointTeam1.transform.position;
                    flag.transform.rotation = flagSpawnPointTeam1.transform.rotation;

                    if (silent == false)
                    {
                        string message = GameState.Instance.state.Team1Name + "'s Flag has been returned to the flag stand.";
                        SendEventHandler.SendToastMessageEvent(message, 1, "FlagReturn");
                    }
                }
                else if (flag.teamID == 2)
                {
                    flag.transform.position = flagSpawnPointTeam2.transform.position;
                    flag.transform.rotation = flagSpawnPointTeam2.transform.rotation;

                    if (silent == false)
                    {
                        string message = GameState.Instance.state.Team2Name + "'s Flag has been returned to the flag stand.";
                        SendEventHandler.SendToastMessageEvent(message, 2, "FlagReturn");
                    }
                }
            }
        }

        protected void FlagTaken(Flag flag, FlagCarrier flagCarrier)
        {
            if (flag != null && flag.entity != null && flag.entity.isAttached && flag.entity.isOwner)
            {
                flag.isHome = false;
                flag.carrier = flagCarrier.Robot.entity;

                if (flagCarrier != null && flagCarrier.Robot != null)
                {
                    if (flag.teamID == 1)
                    {
                        string message = PlayerManager.Instance.PlayerFromGuid(flagCarrier.Robot.PlayerGuid).displayName + " has taken " + gameState.team1Name + "'s flag!";
                        SendEventHandler.SendToastMessageEvent(message, 2, "FlagTaken");
                    }
                    else if (flag.teamID == 2)
                    {
                        string message = PlayerManager.Instance.PlayerFromGuid(flagCarrier.Robot.PlayerGuid).displayName + " has taken " + gameState.team2Name + "'s flag!";
                        SendEventHandler.SendToastMessageEvent(message, 1, "FlagTaken");
                    }
                }

                flagCarrier.OnGrabFlag(flag);
            }
        }

        protected void CaptureFlag(Flag flag, FlagCarrier flagCarrier)
        {
            if (flag != null && flag.entity != null && flag.entity.isAttached && flag.entity.isOwner)
            {
                if (flagCarrier != null && flagCarrier.Robot != null)
                {
                    if (flag.teamID == 1)
                    {
                        GameState.Instance.team2Score++;
                        string message = PlayerManager.Instance.PlayerFromGuid(flagCarrier.Robot.PlayerGuid).displayName + " has captured " + gameState.team1Name + "'s flag.";
                        SendEventHandler.SendToastMessageEvent(message, 2, "FlagCapture");
                    }
                    else if (flag.teamID == 2)
                    {
                        GameState.Instance.team1Score++;
                        string message = PlayerManager.Instance.PlayerFromGuid(flagCarrier.Robot.PlayerGuid).displayName + " has captured " + gameState.team2Name + "'s flag.";
                        SendEventHandler.SendToastMessageEvent(message, 1, "FlagCapture");
                    }

                    flagCarrier.OnCaptureFlag(flag);
                }
            }
        }

        protected void OnDroppedFlag(Flag flag, FlagCarrier flagCarrier)
        {
            if (flag != null && flag.entity != null && flag.entity.isAttached && flag.entity.isOwner)
            {
                if (flag == flagTeam1)
                {
                    if (flagCarrier != null && flagCarrier.Robot != null)
                    {
                        string message = PlayerManager.Instance.PlayerFromGuid(flagCarrier.Robot.PlayerGuid).displayName + " has dropped " + gameState.team1Name + "'s flag.";
                        SendEventHandler.SendToastMessageEvent(message, 2, "FlagDropped");
                    }
                    else
                    {
                        string message = GameState.Instance.state.Team1Name + "'s Flag has been Dropped";
                        SendEventHandler.SendToastMessageEvent(message, 2, "FlagDropped");
                    }
                }
                else if (flag == flagTeam2)
                {
                    if (flagCarrier != null && flagCarrier.Robot != null)
                    {
                        string message = PlayerManager.Instance.PlayerFromGuid(flagCarrier.Robot.PlayerGuid).displayName + " has dropped " + gameState.team2Name + "'s flag.";
                        SendEventHandler.SendToastMessageEvent(message, 1, "FlagDropped");
                    }
                    else
                    {
                        string message = GameState.Instance.state.Team2Name + "'s Flag has been Dropped";
                        SendEventHandler.SendToastMessageEvent(message, 1, "FlagDropped");
                    }
                }
            }
        }
        #endregion
    }
}
