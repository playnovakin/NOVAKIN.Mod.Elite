using NOVAKIN.ModBridge;
using UnityEngine;

namespace NOVAKIN.Mod.Elite
{
    public class Initializer : IScript
    {
        public void Init()
        {
            Debug.Log("Loading Mod Elite Started");
            CallBackHandler.RegisterCallBacks();
        }
    }
}