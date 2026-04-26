namespace SkyRepaired;

[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    public Harmony harmony;
    public static Plugin Instance { get; private set; }
    public static ManualLogSource Logger => Instance.Logger;

    void Awake()
    {
        Instance = this;
        harmony = new Harmony(PluginInfo.GUID);

        harmony.PatchAll();
        Logger.LogInfo("Sky Repaired has finished patching methods. Enjoy the QOL fixes !");
    }
}
