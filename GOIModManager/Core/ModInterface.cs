using System;
using Newtonsoft.Json;

namespace GOIModManager.Core;

public interface IMod {
	public string Name { get; }
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

// TODO: move this to somewhere not core
public struct ConfigurationItem<T> {
	public ConfigurationItem(string description, T value) {
		Description = description;
		Value = value;
	}

	public string Description { get; }
	public T Value { get; }
}
