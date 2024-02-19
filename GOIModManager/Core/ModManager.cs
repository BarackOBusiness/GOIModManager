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

	public IMod[] QueryMods() {
		return mods;
	}

	public static void ModStatusChange(IMod mod) {
		WriteModConfig(mod);
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
					modStates[modInstance] = LoadModConfig(ref modInstance);
					loadedMods.Add(modInstance);
					PrintModConfig(modInstance);
				}
			}
		}

		return loadedMods;
	}

	private static bool LoadModConfig(ref IMod mod) {
		string modConfig = modConfigPath + $"/{mod.Name}.json";
		Debug.Log($"Loading the config file for {mod.Name}...");

		if (!File.Exists(modConfig)) {
			Debug.Log("Config file doesn't exist.");
			File.CreateText(modConfig);
			return true; // Default to running the mod, even if the mod creator doesn't want it to
		}

		mod.Configuration.Deserialize(File.ReadAllText(modConfig));
		Debug.Log($"Read config, IsEnabled is set to: {mod.Configuration.IsEnabled}");
		return mod.Configuration.IsEnabled;
	}

	private static void PrintModConfig(IMod mod) {
		FieldInfo[] fields = mod.Configuration.GetType().GetFields();
		Debug.Log("Trying to print mod configuration fields");

		foreach (FieldInfo field in fields) {
			Debug.Log($"Configuration item {field.Name} of {mod.Name} is set to {field.GetValue(mod)}");
		}
	}

	private static void WriteModConfig(IMod mod) {
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
			WriteModConfig(mod);
			mod.Deinit();
		}
	}
}
