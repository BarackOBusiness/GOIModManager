using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace GOIModManager.Core.Menu;

class ModButton : MonoBehaviour, IPointerClickHandler {
	private IMod mod;
	private ModMenuScreen menu;
	private TextMeshProUGUI text;

	private Color disabledColor;
	private Color enabledColor;

	private void Awake() {
		text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		disabledColor = text.color;
		enabledColor = new Color(disabledColor.r, 1f, disabledColor.b, 1f);
	}

	public void Init(IMod mod) {
		this.mod = mod;
		this.menu = Resources.FindObjectsOfTypeAll<ModMenuScreen>()[0];
		text.color = mod.Configuration.IsEnabled? enabledColor : disabledColor;
	}

	private void UpdateConfig() {
		ModManager.ModStatusChange(mod);
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.clickCount == 1) {
			menu.SetInfoText(mod.Name, mod.Description);
		} else if (eventData.clickCount == 2) {
			mod.Toggle();
			SetColor();
			UpdateConfig();
		}
	}

	private void SetColor() {
		text.color = mod.Configuration.IsEnabled? enabledColor : disabledColor;
		// Do shit with the button component to make it display the full color range
		// And make it darken when highlighted instead of brighten like the dark buttons do
	}
}
