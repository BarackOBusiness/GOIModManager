using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace GOIModManager.Core.Menu;

class ModButton : MonoBehaviour, IPointerClickHandler {
	private IMod mod;
	private ModMenuScreen menu;
	private TextMeshProUGUI text;
	private Button button;

	private Color enabledNormal = new Color(0.2f, 1f, 0.2f, 1f);
	private Color enabledHighlight = new Color(0.4f, 1f, 0.8f, 1f);
	private Color disabledNormal = Color.black;
	private Color disabledHighlight = new Color(0.208f, 0.286f, 0.318f, 1f);

	private void Awake() {
		button = transform.GetComponent<Button>();
		text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	public void Init(IMod mod) {
		this.mod = mod;
		this.menu = Resources.FindObjectsOfTypeAll<ModMenuScreen>()[0];
		text.color = Color.white;
		InitColor();
		SetColor();
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

	private void InitColor() {
		text.color = Color.white;
	
		ColorBlock colors = button.colors;
		colors.normalColor = disabledNormal;
		colors.highlightedColor = disabledHighlight;
		colors.colorMultiplier = 1f;
		colors.fadeDuration = 0.15f;

		button.colors = colors;
	}

	private void SetColor() {
		ColorBlock colors = button.colors;
		colors.normalColor = mod.Configuration.IsEnabled? enabledNormal : disabledNormal;
		colors.highlightedColor = mod.Configuration.IsEnabled? enabledHighlight : disabledHighlight;
		button.colors = colors;
	}
}
