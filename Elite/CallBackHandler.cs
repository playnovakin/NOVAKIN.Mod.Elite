using UnityEngine;

namespace NOVAKIN.Mod.Elite
{
    //Callbacks do not need to be registered here, but I am including this class so
    //that it is easily visible what callbacks are available.
    public static class CallBackHandler
    {
        public static void RegisterCallBacks()
        {
            EventCallBacks.OnPlayerRequestTeamChange += OnPlayerRequestTeamChange;
            EventCallBacks.OnPlayerRequestRespawn += OnPlayerRequestRespawn;
            EventCallBacks.OnPlayerDeath += OnPlayerDeath;
            EventCallBacks.OnMapLoadStarted += OnMapLoadStarted;
            EventCallBacks.OnMapLoadCompleted += OnMapLoadCompleted;
            EventCallBacks.OnTouchFlag += OnTouchFlag;
            EventCallBacks.OnDroppedFlag += OnDroppedFlag;
        }

        public static void OnPlayerRequestTeamChange(Player player, int teamID)
        {

        }

        public static void OnPlayerRequestRespawn(Player player)
        {

        }

        public static void OnPlayerDeath(Player player)
        {

        }

        public static void OnMapLoadStarted(Map map)
        {

        }

        public static void OnMapLoadCompleted(Map map)
        {
            GameObject gameManager = new GameObject();
            gameManager.name = "GameManager";

            switch (map.Mode.ToLower())
            {
                case "deathmatch":
                    gameManager.AddComponent<GameManager>();
                    break;
                case "ctf":
                    gameManager.AddComponent<GameManagerCTF>();
                    break;
                case "rabbit":
                    gameManager.AddComponent<GameManagerRabbit>();
                    break;
                default:
                    gameManager.AddComponent<GameManager>();
                    break;
            }
        }

        public static void OnTouchFlag(Flag flag, FlagCarrier flagCarrier)
        {

        }

        public static void OnDroppedFlag(Flag flag, FlagCarrier flagCarrier)
        {

        }
    }
}
