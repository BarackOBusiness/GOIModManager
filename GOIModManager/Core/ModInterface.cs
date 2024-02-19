using System;
using Newtonsoft.Json;

namespace GOIModManager.Core;

public interface IMod {
	public string Name { get; }
	public string Description { get; }
	public ModConfiguration Configuration { get; }

	void Init();
	void Deinit();
	void Toggle();
}

public abstract class ModConfiguration {
	public abstract bool IsEnabled { get; set; }

	public virtual string Serialize() {
		return JsonConvert.SerializeObject(this, Formatting.Indented);
	}

	public virtual void Deserialize(string json) {
		JsonConvert.PopulateObject(json, this);
	}
}

/// <summary>
/// This attribute is used to attach information to your mod's configuration parameters
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ConfigurationItemAttribute : Attribute {
	public string Name { get; set; }
	public string Description { get; set; }

	/// <param name="name">The name of your configuration parameter</param>
	/// <param name="description">A brief description of what this configuration parameter does</param>
	public ConfigurationItemAttribute(string name, string description) {
		Name = name;
		Description = description;
	}
}
