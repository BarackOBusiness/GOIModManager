using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GOIModManager.Core.Menu;

// Class for the mod menu itself
class ModMenuScreen : MonoBehaviour {
	// Necessary context information
	private Transform UI = GameObject.Find("/Canvas").transform;
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
		info.localPosition = infoPosition + new Vector3(Mathf.SmoothStep(-900f, 0f, t), 0f, 0f);
	}

	public void SlideInfoOut(float t) {
		info.localPosition = infoPosition + new Vector3(Mathf.SmoothStep(-900f, 0f, t), 0f, 0f);
	}

	// TODO: Make this not inactive by default
	// create a method to be called instead of setting inactive for closing the mod menu
	// give ownership of the back button back to this class
	// make this class handle positioning the back button
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

	private void DefaultText() {
		titleText.text = "Mods";
		descText.fontSize = 32f;
		descText.text = "Select a mod to view its description and options.\nDouble click to enable or disable a mod.";
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
