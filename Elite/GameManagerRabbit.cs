using UnityEngine;

namespace NOVAKIN.Mod.Elite
{
    public class GameManagerRabbit : GameManager
    {
        private Flag flag;
        private FlagSpawnPoint flagSpawnPoint;
        private FlagCarrier flagCarrier;
        private float flagHeldTime = 0;

        #region Startup
        protected override void Start()
        {
            base.Start();

            gameState.team1Name = "RABBIT";
            gameState.team2Name = "WOLVES";

            FlagSetup();
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

        protected virtual void FixedUpdate()
        {
            UpdateScores();
        }

        #region Flag Stuff
        private void FlagSetup()
        {
            flagSpawnPoint = FindObjectOfType<FlagSpawnPoint>();

            if (flagSpawnPoint != null)
            {
                BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Flag);
                flag = entity.GetComponent<Flag>();
                flag.flagSpawnPoint = flagSpawnPoint;
                flag.teamID = flagSpawnPoint.teamID;
                flag.gameObject.AddComponent<FlagMotor>();

                ReturnFlagHome(flag, true);
            }
            else
            {
                Debug.LogError("No FlagSpawnPoint found");
            }
        }

        private void UpdateScores()
        {
            if (flag != null && flagCarrier != null)
            {
                flagHeldTime += Time.deltaTime;

                if (flagHeldTime >= 1)
                {
                    PlayerManager.Instance.ModifyPlayerScore(flagCarrier.Robot.playerGuid, 1);

                    GameState.Instance.team1Score = PlayerManager.Instance.PlayerFromGuid(flagCarrier.Robot.playerGuid).score;

                    int highest = 0;

                    foreach (Player player in PlayerManager.Instance.Players)
                    {
                        if (player.entity != null && player.entity.isAttached && player.teamId == 2 && player.score > highest)
                        {
                            highest = player.score;
                        }
                    }

                    gameState.team2Score = highest;
                    flagHeldTime = flagHeldTime - 1;
                }
            }
            else
            {
                GameState.Instance.team1Score = 0;
            }
        }

        private void OnTouchFlag(Flag flag, FlagCarrier flagCarrier)
        {
            if (flag == this.flag)
            {
                if (flagCarrier.Robot.teamId != flag.teamID && flagCarrier.LastTossTime + 0.5f <= BoltNetwork.serverTime)
                {
                    FlagTaken(flag, flagCarrier);
                }
            }
        }

        private void ReturnFlagHome(Flag flag, bool silent)
        {
            if (flag != null && flag.entity != null && flag.entity.isAttached && flag.entity.isOwner)
            {
                flag.isHome = true;
                flag.carrier = null;
                flag.transform.position = flagSpawnPoint.transform.position;
                flag.transform.rotation = flagSpawnPoint.transform.rotation;

                flagCarrier = null;
                flagHeldTime = 0;

                if (silent == false)
                {
                    string message = "The Flag has been returned to the flag stand.";
                    SendEventHandler.SendToastMessageEvent(message, 1, "FlagReturn");
                }
            }
        }

        private void FlagTaken(Flag flag, FlagCarrier flagCarrier)
        {
            if (flag != null && flag.entity != null && flag.entity.isAttached && flag.entity.isOwner)
            {
                flag.isHome = false;
                flag.carrier = flagCarrier.Robot.entity;

                flagHeldTime = 0;
                this.flagCarrier = flagCarrier;

                if (flagCarrier != null && flagCarrier.Robot != null)
                {
                    PlayerManager.Instance.SetPlayerTeam(flagCarrier.Robot.playerGuid, 1);

                    string message = PlayerManager.Instance.PlayerFromGuid(flagCarrier.Robot.playerGuid).displayName + " has taken the flag and is now the Rabbit.";
                    SendEventHandler.SendToastMessageEvent(message, 2, "FlagTaken");
                }

                flagCarrier.OnGrabFlag(flag);
            }
        }

        private void OnDroppedFlag(Flag flag, FlagCarrier flagCarrier)
        {
            if (flag != null && flag.entity != null && flag.entity.isAttached && flag.entity.isOwner)
            {
                if (flag == this.flag)
                {
                    this.flagCarrier = null;
                    flagHeldTime = 0;

                    if (flagCarrier != null && flagCarrier.Robot != null)
                    {
                        PlayerManager.Instance.SetPlayerTeam(flagCarrier.Robot.playerGuid, 2);

                        string message = PlayerManager.Instance.PlayerFromGuid(flagCarrier.Robot.playerGuid).displayName + " dropped the flag and is no longer the Rabbit.";
                        SendEventHandler.SendToastMessageEvent(message, 1, "FlagDropped");
                    }
                }
            }
        }
        #endregion
    }
}
