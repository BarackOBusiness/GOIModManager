using BepInEx;
using UnityEngine;

namespace GOIModManager;

[BepInProcess("GettingOverIt.exe")]
[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
public class GOIModManager : BaseUnityPlugin 
{
    void Awake() {
        new GameObject("Mod Manager", typeof(Core.ModManager));
    }
}
