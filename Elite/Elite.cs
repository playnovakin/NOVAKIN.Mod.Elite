using NOVAKIN.ModBridge;

namespace NOVAKIN.Mod.Elite
{
    public class Elite : IScript
    {
        public void Init()
        {
            CallBackHandler.RegisterCallBacks();
        }
    }
}