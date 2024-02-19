using System.Collections;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using GOIModManager.Core.Menu;

namespace GOIModManager;

[BepInProcess("GettingOverIt.exe")]
[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
public class GOIModManager : BaseUnityPlugin 
{
    private Core.ModManager ModManager;

    void Awake() {
        ModManager = new GameObject("Mod Manager", typeof(Core.ModManager)).GetComponent<Core.ModManager>();

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Loader") {
            StartCoroutine(SetupMenu());
        }
    }

    private IEnumerator SetupMenu() {
        while (GameObject.Find("/Canvas/Column/NewGame") == null) {
            yield return null;
        }

        // Setup menu
        Transform UI = GameObject.Find("/Canvas").transform;
        Transform column = UI.Find("Column");
        GameObject templateButton = column.Find("Quit").gameObject;

        // Pass template information to buttonhelper for later
        MenuButtonHelper modButtonGen = new MenuButtonHelper(templateButton.transform, column);
        
        GameObject modMenu = Instantiate(column.gameObject, UI);
        foreach(Transform child in modMenu.transform) {
            Destroy(child.gameObject);
        }
        modMenu.transform.localPosition = column.localPosition;
        modMenu.transform.localScale = new Vector3(1f, 1f, 1f);
        modMenu.name = "Mods Column";
        modMenu.SetActive(false);

        MenuButtonHelper backButtonGen = new MenuButtonHelper(templateButton.transform, modMenu.transform);
        Transform backButton = backButtonGen.AddButton("Back", () => {
            StartCoroutine(MenuTransitions.ModMenuCloseRoutine());
        });
        
        Transform modMenuButton = modButtonGen.AddButton("Mods", () => 
        {
            StartCoroutine(MenuTransitions.ModMenuOpenRoutine(column.gameObject, modMenu));
        });
        modMenuButton.SetSiblingIndex(column.Find("NewGame").GetSiblingIndex() + 1);
    }
}
