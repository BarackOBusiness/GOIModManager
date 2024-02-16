using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace GOIModManager.Core;

class ModManager : MonoBehaviour {
	public static ModManager Instance { get; private set; }
	public static string modPath { get; } = Application.dataPath + "/Mods";
	public static string modConfigPath { get; } = Application.dataPath + "/ModConfig";

	private IMod[] mods;
	private Dictionary<IMod, bool> modStates = new Dictionary<IMod, bool>();

	public ModManager() {
		Instance = this;
		DontDestroyOnLoad(gameObject);

		if (!Directory.Exists(modPath))
			Directory.CreateDirectory(modPath);

		if (!Directory.Exists(modConfigPath))
			Directory.CreateDirectory(modConfigPath);
	}

	private void Awake() {
		Debug.Log("Loading mods...");
		mods = LoadMods().ToArray();
		Debug.Log("Initializing mods...");
		InitializeMods();
	}

	private void OnDestroy() {
		Debug.Log("Shutting down mods...");
		ShutdownMods();
	}

	private List<IMod> LoadMods() {
		List<IMod> loadedMods = new List<IMod>();
		string[] modFiles = Directory.GetFiles(modPath, "*.dll");

		foreach (string mod in modFiles) {
			Debug.Log($"Found assembly {mod}");
			Assembly modAssembly = Assembly.LoadFrom(mod);
			foreach (Type type in modAssembly.GetTypes()) {
				if (typeof(IMod).IsAssignableFrom(type)) {
					IMod modInstance = (IMod)Activator.CreateInstance(type);
					ValidateModConfig(ref modInstance);
					modStates[modInstance] = LoadModStatus(ref modInstance);
					loadedMods.Add(modInstance);
				}
			}
		}

		return loadedMods;
	}

	private void ValidateModConfig(ref IMod mod) {
		string modConfig = modConfigPath + $"/{mod.Name}.json";
		// Debug.Log($"This is the config file for {mod.Name}: {modConfig}");

		if (!File.Exists(modConfig))
			File.CreateText(modConfig);
	}

	private bool LoadModStatus(ref IMod mod) {
		string modConfig = modConfigPath + $"/{mod.Name}.json";
		Debug.Log($"Loading the config file for {mod.Name}...");

		if (File.ReadAllText(modConfig).Length == 0) {
			Debug.Log("Config file is empty.");
			return true; // Default to running the mod
		}

		mod.Configuration.Deserialize(File.ReadAllText(modConfig));
		Debug.Log($"Read config, IsEnabled is set to: {mod.Configuration.IsEnabled}");
		return mod.Configuration.IsEnabled;
	}

	private void WriteModStatus(IMod mod) {
		string modConfig = modConfigPath + $"/{mod.Name}.json";
		Debug.Log($"Writing the config file for {mod.Name}");

		string json = mod.Configuration.Serialize();
		File.WriteAllText(modConfig, json);
		Debug.Log("Wrote config");
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
			WriteModStatus(mod);
			mod.Deinit();
		}
	}
}
