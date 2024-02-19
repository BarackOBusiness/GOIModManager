using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GOIModManager.Core.Menu;

// Class for buttons in the main menu
class MenuButtonHelper : MonoBehaviour {
	private GameObject template { get; set; }
	private Transform menu { get; set; }

	public MenuButtonHelper(Transform _base, Transform parent) {
		template = _base.gameObject;
		menu = parent;
	}

	public Transform AddButton(string name, UnityEngine.Events.UnityAction unityAction) {
		GameObject tmpButton = Instantiate(template, menu);
		tmpButton.name = name;

		Transform buttonText = tmpButton.transform.GetChild(0);
		Destroy(buttonText.GetComponent<I2.Loc.Localize>());
		buttonText.GetComponent<TextMeshProUGUI>().text = name;

		Button button = tmpButton.GetComponent<Button>();
		button.onClick = new Button.ButtonClickedEvent();
		button.onClick.AddListener(unityAction);

		return tmpButton.transform;
	}

	public Transform AddModButton(IMod mod) {
		GameObject tmpButton = Instantiate(template, menu);
		tmpButton.name = mod.Name + " Button";

		Transform buttonText = tmpButton.transform.GetChild(0);
		Destroy(buttonText.GetComponent<I2.Loc.Localize>());
		buttonText.GetComponent<TextMeshProUGUI>().text = mod.Name;
		
		ModButton handler = tmpButton.AddComponent<ModButton>();
		handler.Init(mod);

		Button button = tmpButton.GetComponent<Button>();
		button.onClick = new Button.ButtonClickedEvent();

		return tmpButton.transform;
	}
}
