using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOVAKIN.Mod.Elite
{
    public class GameManager : MonoBehaviour
    {
        protected GameState gameState;
        protected Dictionary<Player, DateTime> playersAwaitingRespawn = new Dictionary<Player, DateTime>();

        #region Startup
        protected virtual void Start()
        {
            gameState = GameState.Instance;

            RegisterCallBacks();

            PlayerManager.Instance.BeginRoundReset();

            StartCoroutine(MainRoutine());
        }

        protected virtual void RegisterCallBacks()
        {
            EventCallBacks.OnPlayerDeath += OnPlayerDeath;
            EventCallBacks.OnPlayerRequestRespawn += OnPlayerRequestRespawn;
            EventCallBacks.OnPlayerRequestTeamChange += OnPlayerRequestTeamChange;
        }

        protected virtual void DeRegisterCallBacks()
        {
            EventCallBacks.OnPlayerDeath -= OnPlayerDeath;
            EventCallBacks.OnPlayerRequestRespawn -= OnPlayerRequestRespawn;
            EventCallBacks.OnPlayerRequestTeamChange -= OnPlayerRequestTeamChange;
        }

        protected virtual void OnDestroy()
        {
            DeRegisterCallBacks();
        }
        #endregion

        #region Round Management
        protected virtual IEnumerator MainRoutine()
        {
            gameState.currentRoundLength = ServerManager.instance.serverSettingsData.serverSettings.roundLength * 60;

            SendCountdownMessage(15);
            yield return new WaitForSeconds(5);
            SendCountdownMessage(10);
            yield return new WaitForSeconds(5);
            SendCountdownMessage(5);
            yield return new WaitForSeconds(1);
            SendCountdownMessage(4);
            yield return new WaitForSeconds(1);
            SendCountdownMessage(3);
            yield return new WaitForSeconds(1);
            SendCountdownMessage(2);
            yield return new WaitForSeconds(1);
            SendCountdownMessage(1);
            yield return new WaitForSeconds(1);
            SendCountdownMessage(0);

            gameState.currentRoundStarted = true;

            while (gameState.currentRoundTime < gameState.currentRoundLength)
            {
                gameState.currentRoundTime += Time.deltaTime;
                yield return null;
            }

            gameState.currentRoundTime = 0;
            gameState.currentRoundEnded = true;

            EndRound();

            yield return new WaitForSeconds(5);

            ServerManager.instance.LoadMap(ServerManager.instance.NextMap());
        }

        protected virtual void SendCountdownMessage(int timeRemaining)
        {
            string message = timeRemaining + " Seconds until round begins.";

            if (timeRemaining <= 0)
                message = "Round Has Begun";

            SendEventHandler.SendChatMessageEvent(System.Guid.Empty, "<color=#FF0066FF>" + message + "</color>", false, 0, null);
        }

        protected virtual void EndRound()
        {
            SendEventHandler.SendEndOfRoundEvent();
            SendEventHandler.SendChatMessageEvent(System.Guid.Empty, "<color=#FF0066FF>" + "Round Has Ended" + "</color>", false, 0, null);

            PlayerManager.Instance.EndRoundReset();
        }
        #endregion

        #region Spawn Management
        public virtual void OnPlayerDeath(Player player)
        {
            AddPlayerToSpawnQueue(player);
        }

        public virtual void OnPlayerRequestRespawn(Player player)
        {
            TrySpawnPlayer(player, false);
        }

        public virtual void OnPlayerRequestTeamChange(Player player, int teamID)
        {
            PlayerManager.Instance.DestroyPlayerControlledEntities(player);
            PlayerManager.Instance.SetPlayerTeam(player, teamID);

            AddPlayerToSpawnQueue(player);
        }

        protected virtual void AddPlayerToSpawnQueue(Player player)
        {
            if (player != null && player.teamId != 0)
            {
                if (playersAwaitingRespawn.ContainsKey(player))
                    playersAwaitingRespawn.Remove(player);

                playersAwaitingRespawn.Add(player, DateTime.Now.AddSeconds(3.0f));
                SendEventHandler.SendPlayerRespawnAvailableEvent(player.guid, 3.0f, 3.0f);
            }
        }

        protected virtual void TrySpawnPlayer(Player player, bool immediate)
        {
            if (gameState.currentRoundStarted == true && gameState.currentRoundEnded == false)
            {
                if (player.teamId > 0 && playersAwaitingRespawn.ContainsKey(player))
                {
                    if (immediate || playersAwaitingRespawn[player] <= DateTime.Now)
                    {
                        SpawnPoint spawnPoint = Utils.RandomSpawnPoint(player.teamId);

                        if (spawnPoint != null)
                        {
                            Vector3 spawnPosition = spawnPoint.transform.position;
                            Quaternion spawnRotation = spawnPoint.transform.rotation;

                            //Sometimes mappers can be sloppy and put the spawnpoint slightly under the ground so...
                            //Do a RayCast 1 unit above the spawnpoint downwards, if we hit anything, move the spawnposition up 1 unit.
                            RaycastHit hit;

                            if (Physics.SphereCast(spawnPoint.transform.position + Vector3.up, 0.55f, -spawnPoint.transform.up, out hit, 1f))
                                spawnPosition += Vector3.up;

                            Utils.SpawnPlayerAtPosition(player, spawnPosition, spawnRotation);

                            playersAwaitingRespawn.Remove(player);
                            SendEventHandler.SendPlayerRespawnEvent(player.guid);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
