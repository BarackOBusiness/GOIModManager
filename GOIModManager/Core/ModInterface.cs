using System;
using Newtonsoft.Json;

namespace GOIModManager.Core;

/// <summary>
/// The interface a modder will need to implement to "talk to the mod manager."
/// </summary>
public interface IMod {
	/// <summary>
	/// A user-friendly name for the mod
	/// </summary>
	public string Name { get; }
	/// <summary>
	/// A concise description of the mod's function
	/// </summary>
	public string Description { get; }
	/// <summary>
	/// An implementation of ModConfiguration for storing data for the mod.
	/// See documentation below for more information on that.
	/// </summary>
	/// <remarks>
	/// A common pattern that will be seen here is a static field in the mod class
	/// that is of the type that inherits from ModConfiguration, and this interface member
	/// simply points to that field, this is a flexible way of letting your monobehaviours
	/// actually access the settings you've defined while satisfying the interface contract.
	/// </remarks>
	public ModConfiguration Configuration { get; }

	/// <summary>
	/// This is called if your mod is enabled on its creation.
	/// </summary>
	void Init();
	/// <summary>
	/// This is called if your mod is enabled on its destruction.
	/// This occurs when the game is closed.
	/// </summary>
	void Deinit();
	/// <summary>
	/// This is called when your mod is enabled or disabled in the mod menu,
	/// this function should always set your configuration's IsEnabled value to its opposite
	/// and handle the cases for whether it's enabled or disabled,
	/// but nonetheless this is a toggle to keep the interface minimal and flexible,
	/// a mod author's implementation is up to them.
	/// </summary>
	/// <remarks>
	/// I personally create an Enable and Disable method which is to be handled here,
	/// and the Init and Deinit methods also call these along with whatever other
	/// cleanup they must do.
	/// </remarks>
	void Toggle();
}

/// <summary>
/// This class is the abstract parent class that a modder must use to define
/// mod configuration parameters as well as simple persistent values for the functioning
/// of the mod.
/// Larger scale persistent data such as information that is collected from the game
/// that is necessary for the functioning of the mod should instead be written to files
/// in the mod's configuration folder as created by the mod manager.
/// Do NOT override virtual methods written here.
/// </summary>
public abstract class ModConfiguration {
	/// <summary>
	/// Persistent data used by the mod manager to determine the startup state of a given mod.
	/// Must be implemented by the mod author (as of right now).
	/// </summary>
	public abstract bool IsEnabled { get; set; }

	public virtual void ApplySetting(string fieldName, object value) {
		var property = GetType().GetProperty(fieldName);
		if (property != null && property.CanWrite) {
			property.SetValue(this, value);
		}
	}

	public virtual string Serialize() {
		return JsonConvert.SerializeObject(this, Formatting.Indented);
	}

	public virtual void Deserialize(string json) {
		JsonConvert.PopulateObject(json, this);
	}
}

/// <summary>
/// This attribute is used to attach necessary information to your mod's configuration parameters.
/// A mod setting is hereby defined to be a field implemented in a class inheriting 
/// the ModConfiguration class that has a ConfigurationItem attribute applied to it.
/// Not following this convention is non standard and will not be respected by the mod manager.
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
