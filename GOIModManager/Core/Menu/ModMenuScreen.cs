using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GOIModManager.Core.Menu;

// Class for the mod menu itself
class ModMenuScreen : MonoBehaviour {
	// Necessary context information
	private Transform UI = GameObject.Find("/Canvas").transform;
	private ModManager modManager = Resources.FindObjectsOfTypeAll<ModManager>()[0];
	private RectTransform rect;

	// Used as a baseline to animate the slide in
	public static Vector3 infoPosition = new Vector3(-310f, 288f, 0f);
	public static float rockX = -55f;

	RectTransform info;
	RectTransform title;
	RectTransform description;
	RectTransform modList;
	RectTransform modColumn;
	RectTransform backButton;
	RectTransform[] modButtons;

	TextMeshProUGUI titleText;
	TextMeshProUGUI descText;

	// To be called after the opening animation for the mod menu has finished
	public void Show() {
		backButton.gameObject.SetActive(true);
		modList.gameObject.SetActive(true);
	}

	// To be called after the closing animation for the mod menu has finished
	public void Hide() {
		backButton.gameObject.SetActive(false);
		modList.gameObject.SetActive(false);
	}

	public void SlideInfoIn(float t) {
		info.localPosition = infoPosition + new Vector3(Mathf.SmoothStep(-1400f, 0f, t), 0f, 0f);
	}

	public void SlideInfoOut(float t) {
		info.localPosition = infoPosition + new Vector3(Mathf.SmoothStep(-1400f, 0f, t), 0f, 0f);
	}

	public void SetInfoText(string title, string description) {
		titleText.text = title;
		descText.text = description;
	}

	public void DefaultText() {
		titleText.text = "Mods";
		descText.fontSize = 32f;
		descText.text = "Select a mod to view its description and options.\nDouble click to enable or disable a mod.";
	}
	void Awake() {
		rect = transform as RectTransform;
	
		rect.SetParent(UI);
		rect.localPosition = Vector3.zero;
		rect.localScale = Vector3.one;
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.sizeDelta = Vector2.one;

		info = Instantiate(UI.Find("Mask"), rect) as RectTransform;
		info.name = "Info";
		Destroy(info.GetComponent<RectMask2D>());

		title = info.GetChild(0) as RectTransform;
		titleText = title.GetComponent<TextMeshProUGUI>();
		description = title.GetChild(0) as RectTransform;
		descText = description.GetComponent<TextMeshProUGUI>();

		StartCoroutine(CreateBackButton());

		modList = CreateList() as RectTransform;
		modColumn = CreateColumn() as RectTransform;

		modButtons = PopulateModList().ToArray();

		Hide();
		
		InitLayout();
		DefaultText();
	}

	private void InitLayout() {
		info.localPosition = new Vector3(infoPosition.x - 900f, infoPosition.y, 0f);
		titleText.alignment = TextAlignmentOptions.TopLeft;
		description.GetComponent<RectTransform>().anchorMax = new Vector2(0.58f, 0.884f);
		descText.alignment = TextAlignmentOptions.BaselineLeft;

		backButton.transform.localPosition = new Vector3(179f, 380f, 0f);
		modList.GetComponent<RectTransform>().anchoredPosition = new Vector2(478f, -100f);
	}

	private List<RectTransform> PopulateModList() {
		MenuButtonHelper buttonGen = new MenuButtonHelper(UI.Find("Column/Quit"), modColumn);

		IMod[] mods = modManager.QueryMods();
		List<RectTransform> buttons = new List<RectTransform>();
	
		foreach (IMod mod in mods) {
			buttons.Add(buttonGen.AddModButton(mod) as RectTransform);
		}

		return buttons;
	}

	private IEnumerator CreateBackButton() {
		MenuButtonHelper backButtonGenerator = new MenuButtonHelper(UI.Find("Column/Quit"), rect);
		backButton = backButtonGenerator.AddButton("Back", () => {
			StartCoroutine(MenuTransitions.ModMenuCloseRoutine());
		}) as RectTransform;

		yield return null;
	}
	
	private Transform CreateList() {
		RectTransform listPanel = new GameObject("Mod List", new Type[]{ typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(ScrollRect) }).transform as RectTransform;
		listPanel.SetParent(rect);
		listPanel.localScale = Vector3.one;
		listPanel.sizeDelta = new Vector2(600, 800);
		listPanel.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

		return listPanel;
	}

	private Transform CreateColumn() {
		Transform column = UI.Find("Column");
		Transform viewport = modList;

		Debug.Log($"{modList.GetComponent<RectTransform>().anchoredPosition}");
		GameObject columnObj = Instantiate(UI.Find("Column").gameObject, viewport);
		RectTransform columnTransform = columnObj.GetComponent<RectTransform>();
		modList.GetComponent<ScrollRect>().content = columnTransform;
		foreach (Transform child in columnTransform) {
			Destroy(child.gameObject);
		}
		columnTransform.localPosition = new Vector3(0f, 0f, 0f);
		columnTransform.localScale = new Vector3(1f, 1f, 1f);
		Debug.Log($"{modList.GetComponent<RectTransform>().anchoredPosition}");

		return columnTransform;
	}
}
