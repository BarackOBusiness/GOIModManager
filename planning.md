Mod Menu (ModMenuScreen):
	- Local Position: 0 0 0
	- Mask clone (RectTransform only)
		- Local Position: -310 288 0
		- Selection
			- Default text: "Mods"
			- Description
				- Default text: "Select a mod to view its description and options.\nDouble click to enable or disable a mod."
				- Font size: 36 or lower
				- Adjust anchor max x to 0.58
	- Back button
		- Local Position: 179 380 0
	- Mods List (ScrollRect)
		- Local Position: 100 120 0  ?
		- Column clone
			- Enumerated mod buttons go here

Ideal menu transitions:
	See screenshot for guidelines on what the mod menu should look like when the mod menu is in place.
	Click mods: column fades out, title slides out, help message slides in from left and rock slides to the right (x=-55.0), back button and mod list fades in.
	
