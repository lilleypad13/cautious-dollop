using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SupportedRenderers
{
	//To add support for other types of renderers or materials, add them here
	//That keeps any changes separate from the main ColorMatcher script

	private GameObject obj;
	private SpriteRenderer sprite;
	private Image img;
	private Text txt;
	private TextMeshPro tmPro;
	private TextMeshProUGUI tmpUGUI;
	private Camera cam;

	public SupportedRenderers(GameObject g) {
		obj = g;
	}

	public void TrySetRenderers() {
		if(sprite == null) { sprite = obj.GetComponent<SpriteRenderer>(); }
		if(img == null) { img = obj.GetComponent<Image>(); }
		if(txt == null) { txt = obj.GetComponent<Text>(); }
		if(tmPro == null) { tmPro = obj.GetComponent<TextMeshPro>(); }
		if(tmpUGUI == null) { tmpUGUI = obj.GetComponent<TextMeshProUGUI>(); }
		if(cam == null) { cam = obj.GetComponent<Camera>(); }
	}

	public void SetColor(Color c) {
		if (sprite != null) { sprite.color = c; }
		if (img != null) { img.color = c; }
		if (txt != null) { txt.color = c; }
		if (tmPro != null) { tmPro.color = c; }
		if (tmpUGUI != null) { tmpUGUI.color = c; }
		if (cam != null) { cam.backgroundColor = c; }
	}
}

public class ColorMatcher : MonoBehaviour
{
	public static Action PaletteChanged;

	public string colorID = "";
	private SupportedRenderers rends;

	private void Start() {
		rends = new SupportedRenderers(gameObject);
		rends.TrySetRenderers();
	}

	private void OnEnable() {
		PaletteChanged += SetColor;
		if(rends == null) {
			rends = new SupportedRenderers(gameObject);
			rends.TrySetRenderers();
		}
		SetColor();
	}

	private void OnDisable() {
		PaletteChanged -= SetColor;
	}

	private void SetColor() {
		rends.SetColor(PaletteManager.GetColor(colorID));
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(ColorMatcher))]
	public class ColorMatcherEditor : Editor
	{
		private ColorMatcher script;
		private LabeledColor[] colors;
		private string[] colorLabels;
		private SerializedProperty colorID;

		private void OnEnable() {
			script = (ColorMatcher)target;
			colorID = serializedObject.FindProperty("colorID");

			colors = PaletteManager.GetDefaultColors();
			colorLabels = new string[colors.Length];
			for (int i = 0; i < colors.Length; i++) {
				string indexText = colors[i].index < 10 ? "0" + colors[i].index : colors[i].index.ToString();
				colorLabels[i] = $"{indexText} - {colors[i].name}";
			}
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			int index;
			if (colorID.stringValue == null) {
				index = 0;
			} else {
				index = Mathf.Max(0, PaletteManager.GetColorIndex(colorID.stringValue));
			}

			index = EditorGUILayout.Popup(new GUIContent("Color Key"), index, colorLabels);
			colorID.stringValue = PaletteManager.GetDefaultColors()[index].ID;
			if(script.rends == null) {
				script.rends = new SupportedRenderers(script.gameObject);
			}
			script.rends.TrySetRenderers();
			script.SetColor();

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed) { EditorUtility.SetDirty(serializedObject.targetObject); }
		}
	}
	#endif
}


