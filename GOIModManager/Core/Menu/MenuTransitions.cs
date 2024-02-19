using System.Collections;
using TMPro;
using UnityEngine;

namespace GOIModManager.Core.Menu;

public static class MenuTransitions {
	private static Transform rock;
	private static Vector3 rockStartPos;
	private static RectTransform titleMask;
	private static Vector3 titleStartPos;
	private static GameObject menu;
	private static GameObject modMenu;
	private static Loader loader;
	private static bool isModMenuOpen = false;
	
	public static IEnumerator ModMenuOpenRoutine(GameObject menu, GameObject modMenu) {
		loader = Resources.FindObjectsOfTypeAll<Loader>()[0];
		loader.hammerAnim.Play("HammerDown");

		rock = loader.rock;
		rockStartPos = rock.position;

		titleMask = loader.titleMask;
		titleStartPos = titleMask.position;

		MenuTransitions.menu = menu;
		MenuTransitions.modMenu = modMenu;

		menu.SetActive(false);

		TextMeshProUGUI[] items = loader.menu.GetComponentsInChildren<TextMeshProUGUI>();
		for (float t = 0f; t <= 1.0001f; t += 0.05f) {
			titleMask.position = titleStartPos + new Vector3(Mathf.SmoothStep(0f, -67.5f, t), 0f, 0f);
			// rock.position = rockStartPos + new Vector3(Mathf.SmoothStep(0f, -15f, t), 0f, 0f);

			// Animation should be consistent on all systems
			yield return new WaitForFixedUpdate();
		}
		modMenu.SetActive(true);
		isModMenuOpen = true;
	}
	
	public static IEnumerator ModMenuCloseRoutine() {
		if (!isModMenuOpen) yield break;

		titleMask.sizeDelta = new Vector2(0f, 216f);
		modMenu.SetActive(false);

		loader.hammerAnim.Play("HammerUp");

		for (float t = 1f; t >= -0.0001f; t -= 0.05f) {
			titleMask.position = titleStartPos + new Vector3(Mathf.SmoothStep(0f, -900f, t), 0f, 0f);
			// rock.position = rockStartPos + new Vector3(Mathf.SmoothStep(0f, -15f, t), 0f, 0f);

			yield return new WaitForFixedUpdate();
		}
	
		TextMeshProUGUI[] items = menu.GetComponentsInChildren<TextMeshProUGUI>();
		for (int i = 0; i < items.Length; i++) {
			Color c = items[i].color;
			c.a = 0;
			items[i].color = c;
		}
		menu.SetActive(true);

		isModMenuOpen = false;
	}
}
