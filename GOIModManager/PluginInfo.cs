using GOIModManager;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

[assembly: AssemblyVersion(PluginInfo.VERSION)]
[assembly: AssemblyTitle(PluginInfo.NAME + " (" + PluginInfo.GUID + ")")]
[assembly: AssemblyProduct(PluginInfo.NAME)]

namespace GOIModManager {
	internal static class PluginInfo {
		public const string GUID = "goiplugins.core.modman";
		public const string NAME = "Getting Over It Mod Manager";
		public const string VERSION = "0.1.0";
	}
}
