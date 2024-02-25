using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace GOIModManager.Core;

public class ModManager : MonoBehaviour {
	public static ModManager Instance { get; private set; }
	public static string modPath { get; } = Application.dataPath + "/Mods";
	public static string modConfigPath { get; } = Application.dataPath + "/ModConfig";

	private IMod[] mods;
	private Dictionary<IMod, bool> modStates = new Dictionary<IMod, bool>();

	public IMod[] QueryMods() {
		return mods;
	}

	public static void ModStatusChange(IMod mod) {
		ModLoaderUtils.WriteModConfig(mod);
	}

	private void Awake() {
		Instance = this;
		DontDestroyOnLoad(gameObject);

		if (!Directory.Exists(modPath))
			Directory.CreateDirectory(modPath);

		if (!Directory.Exists(modConfigPath))
			Directory.CreateDirectory(modConfigPath);
	
		Debug.Log("Loading mods...");
		mods = ModLoaderUtils.LoadMods(modStates).ToArray();
		Debug.Log("Initializing mods...");
		InitializeMods();
	}

	private void OnDestroy() {
		Debug.Log("Shutting down mods...");
		ShutdownMods();
	}

	private void InitializeMods() {
		foreach (IMod mod in mods) {
			if (modStates[mod] == true)	{		
				Debug.Log($"Initializing mod {mod.Name}");
				mod.Init();
			}
		}
	}

	private void ShutdownMods() {
		foreach (IMod mod in mods) {
			Debug.Log($"Shutting down mod {mod.Name}");
			ModLoaderUtils.WriteModConfig(mod);
			mod.Deinit();
		}
	}
}
