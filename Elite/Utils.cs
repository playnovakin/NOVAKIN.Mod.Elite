using System.Collections.Generic;
using UnityEngine;

namespace NOVAKIN.Mod.Elite
{
    public static class Utils
    {
        public static SpawnPoint RandomSpawnPoint(int teamID)
        {
            SpawnPoint randomSpawnPoint = null;

            List<SpawnPoint> teamSpawnPoints = new List<SpawnPoint>();

            foreach (SpawnPoint spawnpoint in Map.Instance.SpawnPoints)
            {
                if (spawnpoint.teamID == teamID && spawnpoint.IsClear())
                {
                    teamSpawnPoints.Add(spawnpoint);
                }
            }

            if (teamSpawnPoints.Count > 0)
            {
                randomSpawnPoint = teamSpawnPoints[UnityEngine.Random.Range(0, teamSpawnPoints.Count - 1)];
            }

            return randomSpawnPoint;
        }

        public static Vector3 RandomSpawnPosition(int teamID)
        {
            SpawnPoint randomSpawnPoint = RandomSpawnPoint(teamID);

            if (randomSpawnPoint != null)
            {
                RaycastHit hit;

                if (Physics.Raycast(randomSpawnPoint.transform.position + Vector3.up, -randomSpawnPoint.transform.up, out hit, 1f))
                    return hit.point;
            }


            return randomSpawnPoint.transform.position;
        }

        public static void SpawnPlayerAtPosition(Player player, Vector3 position, Quaternion rotation)
        {
            //Make sure player controlled entities are destroyed.
            PlayerManager.Instance.DestroyPlayerControlledEntities(player);

            LoadOut loadOut = new LoadOut();
            loadOut.itemNames = new string[] { "Scout", "PlasmaFusor", "GrenadeLauncher" };

            LoadOutToken loadOutToken = new LoadOutToken(loadOut);

            GameObject baseMech = DatabaseManager.Instance.PrefabDatabase.GetPrefabByName("BaseMech");
            BoltEntity entity = BoltNetwork.Instantiate(baseMech, loadOutToken, position, rotation);

            if (entity != null)
            {
                Unit unit = entity.GetComponent<Unit>();
                unit.Setup(player.guid, player.teamId);
                unit.gameObject.AddComponent<AbilityVelocityRedirection>();

                if (player.connection == null)
                {
                    entity.TakeControl();
                }
                else
                {
                    entity.AssignControl(player.connection);
                }

                player.SetControlledEntity(entity);
            }
        }
    }
}