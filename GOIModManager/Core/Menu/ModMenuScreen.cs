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

	Transform info;
	Transform title;
	Transform description;
	Transform modList;
	Transform modColumn;
	Transform backButton;
	Transform[] modButtons;

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
		rect = gameObject.GetComponent<RectTransform>();
	
		rect.SetParent(UI);
		rect.localPosition = new Vector3(0f, 0f, 0f);
		rect.localScale = new Vector3(1f, 1f, 1f);
		rect.anchorMin = new Vector2(0, 0);
		rect.anchorMax = new Vector2(1, 1);
		rect.sizeDelta = new Vector2(0, 0);

		info = Instantiate(UI.Find("Mask"), rect);
		info.name = "Info";
		Destroy(info.GetComponent<RectMask2D>());

		title = info.GetChild(0);
		titleText = title.GetComponent<TextMeshProUGUI>();
		description = title.GetChild(0);
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
		info.localPosition = new Vector3(infoPosition.x - 900f, infoPosition.y, 0f);
		titleText.alignment = TextAlignmentOptions.TopLeft;
		description.GetComponent<RectTransform>().anchorMax = new Vector2(0.58f, 0.884f);
		descText.alignment = TextAlignmentOptions.BaselineLeft;

		backButton.transform.localPosition = new Vector3(179f, 380f, 0f);
		modList.transform.localPosition = new Vector3(179f, -25f, 0f);
	}



	private List<Transform> PopulateModList() {
		MenuButtonHelper buttonGen = new MenuButtonHelper(UI.Find("Column/Quit"), modColumn);

		IMod[] mods = modManager.QueryMods();
		List<Transform> buttons = new List<Transform>();
	
		foreach (IMod mod in mods) {
			buttons.Add(buttonGen.AddModButton(mod));
		}

		return buttons;
	}

	private IEnumerator CreateBackButton() {
		MenuButtonHelper backButtonGenerator = new MenuButtonHelper(UI.Find("Column/Quit"), rect);
		backButton = backButtonGenerator.AddButton("Back", () => {
			StartCoroutine(MenuTransitions.ModMenuCloseRoutine());
		});

		yield return null;
	}
	
	private Transform CreateList() {
		GameObject listObj = new GameObject("Mod List", new Type[]{typeof(RectTransform), typeof(ScrollRect)});
		RectTransform listTransform = listObj.GetComponent<RectTransform>();
		listTransform.SetParent(rect);
		listTransform.localScale = new Vector3(1f, 1f, 1f);
		listTransform.sizeDelta = new Vector2(400, 600);
		listObj.GetComponent<ScrollRect>().horizontal = false;

		GameObject listViewport = new GameObject("Viewport", new Type[]{typeof(RectTransform), typeof(Mask)});
		listViewport.transform.SetParent(listObj.transform);
		listViewport.transform.localScale = new Vector3(1f, 1f, 1f);
		listViewport.GetComponent<Mask>().showMaskGraphic = false;
		listViewport.GetComponent<Mask>().enabled = false;

		listObj.GetComponent<ScrollRect>().viewport = listViewport.GetComponent<RectTransform>();

		return listObj.transform;
	}

	private Transform CreateColumn() {
		Transform column = UI.Find("Column");
		Transform viewport = modList.GetChild(0);
	
		GameObject columnObj = Instantiate(UI.Find("Column").gameObject, viewport);
		Transform columnTransform = columnObj.GetComponent<RectTransform>();
		foreach (Transform child in columnTransform) {
			Destroy(child.gameObject);
		}
		columnTransform.localPosition = new Vector3(0f, 0f, 0f);
		columnTransform.localScale = new Vector3(1f, 1f, 1f);

		return columnTransform;
	}
}
