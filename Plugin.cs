using BepInEx;
using HarmonyLib;
using BepInEx.Logging;

namespace SkyRepaired
{

    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public Harmony harmony;
        public static Plugin Instance { get; private set; }
        public static ManualLogSource Logger { get; private set; }
        public static PluginConfig Config { get; private set; }

        void Awake()
        {
            Instance = this;
            Logger = base.Logger;
            Config = new PluginConfig(base.Config);
            harmony = new Harmony(PluginInfo.GUID);

            harmony.PatchAll();
            Logger.LogInfo("Sky Repaired has finished patching methods. Enjoy the QOL fixes !");
        }
    }
}