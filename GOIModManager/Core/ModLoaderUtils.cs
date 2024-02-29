using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace GOIModManager.Core;

public static class ModLoaderUtils {
	internal static List<IMod> LoadMods(Dictionary<IMod, bool> states) {
		List<IMod> loadedMods = new List<IMod>();
		string[] modFiles = Directory.GetFiles(ModManager.modPath, "*.dll");

		foreach (string assembly in modFiles) {
			Assembly modAssembly = Assembly.LoadFrom(assembly);
			Debug.Log($"Found assembly {modAssembly.FullName}");
			foreach (Type type in modAssembly.GetTypes()) {
				if (typeof(IMod).IsAssignableFrom(type)) {
					try {
						IMod modInstance = (IMod)Activator.CreateInstance(type);
						states[modInstance] = LoadModConfig(modInstance);
						loadedMods.Add(modInstance);
						// PrintModConfig(modInstance); // For debug
					} catch (Exception err) {
						Debug.Log($"Failed to load mod {modAssembly.FullName}: {err}");
					}
				}
			}
		}

		return loadedMods;
	}

	internal static void GenerateModConfig(IMod mod) {
		Debug.Log("Config file doesn't exist, creating one.");
		try {
			Directory.CreateDirectory(GetModConfigPath(mod));
			File.CreateText(GetModConfig(mod));
			WriteModConfig(mod);
		} catch (Exception err) {
			Debug.Log($"Failed to create new config file for mod {mod.Name}: {err}");
		}
	}
	
	internal static bool LoadModConfig(IMod mod) {
		Debug.Log($"Loading the config file for {mod.Name}...");
	
		if (!Directory.Exists(GetModConfigPath(mod))) {
			GenerateModConfig(mod);
		}

		try {
			string json = File.ReadAllText(GetModConfig(mod));
			mod.Configuration.Deserialize(json);
		} catch (Exception err) {
			Debug.Log($"Failed to read config file for mod {mod.Name}: {err}");
		}
		return mod.Configuration.IsEnabled;
	}

	internal static void WriteModConfig(IMod mod) {
		Debug.Log("Writing mod config");
		
		try {
			File.WriteAllText(GetModConfig(mod), mod.Configuration.Serialize());
		} catch (Exception err) {
			Debug.Log($"Failed to write config file for mod {mod.Name}: {err}");
		}
	}

	internal static void PrintModConfig(IMod mod) {
		Type configType = mod.Configuration.GetType();
		FieldInfo[] fields = configType.GetFields();

		foreach (FieldInfo field in fields) {
			object value = field.GetValue(mod.Configuration);
			Debug.Log($"Property: {field.Name}, Value: {value}");

			var attribute = field.GetCustomAttribute<ConfigurationItemAttribute>();
			Debug.Log($"Setting name: {attribute.Name}, Setting description: {attribute.Description}");
		}
	}

	public static string GetModConfigPath(IMod mod) {
		return ModManager.modConfigPath + "/" + mod.Name;
	}

	public static string GetModConfig(IMod mod) {
		return ModManager.modConfigPath + "/" + mod.Name + "/" + mod.Name + ".json";
	}
}
