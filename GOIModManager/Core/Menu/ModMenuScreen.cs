using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// TODO Here:
// The mod info should be part of a parent object named Mod Selection
// Mod Selection should contain also a panel (that is hidden until a mod is selected)
// which is a translucent scrollable white panel with a rectmask2d that is parent to
// a column verticallayoutgroup with all of the configuration options
// this is a gameobject with a class that handles instantiating the text and input fields
// SUBJECT TO CHANGE.
namespace GOIModManager.Core.Menu;

// Class for the mod menu itself
class ModMenuScreen : MonoBehaviour {
	// Necessary context information
	private Transform UI = GameObject.Find("/Canvas").transform;
	private ModManager modManager = Resources.FindObjectsOfTypeAll<ModManager>()[0];
	private RectTransform rect;

	// Used as a baseline to animate the slide in
	public static Vector3 infoPosition = new Vector3(-260f, 288f, 0f);
	public static float rockX = -55f;

	RectTransform info;
	RectTransform title;
	RectTransform modList;
	RectTransform modColumn;
	RectTransform backButton;
	RectTransform description;
	RectTransform modSelection;
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

	public void SlideSelection(float t) {
		modSelection.localPosition = infoPosition + new Vector3(Mathf.SmoothStep(-Screen.width, 0f, t), 0f, 0f);
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

		modSelection = CreateSelectionScreen();

		info = Instantiate(UI.Find("Mask"), modSelection) as RectTransform;
		info.localPosition = Vector3.zero;
		info.name = "Info";
		Destroy(info.GetComponent<RectMask2D>());

		title = info.GetChild(0) as RectTransform;
		titleText = title.GetComponent<TextMeshProUGUI>();
		description = title.GetChild(0) as RectTransform;
		descText = description.GetComponent<TextMeshProUGUI>();

		StartCoroutine(CreateBackButton());

		modList = CreateList();
		modColumn = CreateColumn();

		modButtons = PopulateModList().ToArray();

		Hide();
		
		InitLayout();
		DefaultText();
	}

	private void InitLayout() {
		modSelection.localPosition = new Vector3(infoPosition.x - Screen.width, infoPosition.y, 0f);
		modSelection.GetChild(0).localPosition = new Vector3(-130.1916f, -443.2612f, 0f);
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

	private RectTransform CreateSelectionScreen() {
		RectTransform selectionScreen = new GameObject("Mod Selection", new Type[]{ typeof(RectTransform), typeof(VerticalLayoutGroup) }).transform as RectTransform;
		selectionScreen.SetParent(rect);
		selectionScreen.localScale = Vector3.one;
		RectTransform settingsScreen = new GameObject("Mod Config", new Type[]{ typeof(RectTransform), typeof(RectMask2D), typeof(Image), typeof(ScrollRect) }).transform as RectTransform;
		settingsScreen.SetParent(selectionScreen);
		settingsScreen.localScale = Vector3.one;
		settingsScreen.sizeDelta = new Vector2(1000f, 750f);
		settingsScreen.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.1f);
		return selectionScreen;
	}

	private IEnumerator CreateBackButton() {
		MenuButtonHelper backButtonGenerator = new MenuButtonHelper(UI.Find("Column/Quit"), rect);
		backButton = backButtonGenerator.AddButton("Back", () => {
			StartCoroutine(MenuTransitions.ModMenuCloseRoutine());
		}) as RectTransform;

		yield return null;
	}
	
	private RectTransform CreateList() {
		RectTransform listPanel = new GameObject("Mod List", new Type[]{ typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(ScrollRect) }).transform as RectTransform;
		listPanel.SetParent(rect);
		listPanel.localScale = Vector3.one;
		listPanel.sizeDelta = new Vector2(600, 800);
		listPanel.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

		return listPanel;
	}

	private RectTransform CreateColumn() {
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
