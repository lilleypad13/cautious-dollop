using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class PaletteManager : MonoBehaviour
{
	public bool dontDestroyOnLoad = true;
	public Color errorColor = Color.magenta;

	public Palette defaultPalette;
	public Palette[] altPalettes;

	private static PaletteManager instance;
	[SerializeField] private static Palette chosenPalette;

	private static Palette ChosenPalette { 
		get {
			if (chosenPalette == null) {
				chosenPalette = instance.defaultPalette;
			}
			return chosenPalette;
		} 
	}

	private void Awake() {
		instance = this;

		if (dontDestroyOnLoad) {
			DontDestroyOnLoad(gameObject);
		}
	}

	private void OnValidate() {
		if(instance == null) {
			instance = this;
		}
	}

	public static void SetChosenPalette(Palette p) {
		chosenPalette = p;
		ColorMatcher.PaletteChanged?.Invoke();
	}
	public static void SetChosenPalette(int i) {
		if(i < GetPalettes().Length && i >= 0) {
			chosenPalette = GetPalettes()[i];
			ColorMatcher.PaletteChanged?.Invoke();
		} else {
			Debug.LogWarning($"No Palette {i} to set ({GetPalettes().Length} palettes exist)!");
		}
	}
	public static void SetChosenPalette(string paletteName) {
		Palette p = GetPalettes().FirstOrDefault(x => x.name == paletteName);
		if(p != null) {
			chosenPalette = p;
			ColorMatcher.PaletteChanged?.Invoke();
		} else {
			Debug.LogWarning($"No Palette '{paletteName}' to set!");
		}
	}

	public Palette GetChosenPalette() {
		return ChosenPalette;
	}
	public static Color GetColor(string id) {
		if (Application.isPlaying) {
			LabeledColor c = ChosenPalette.colorSet.FirstOrDefault(x => x.ID == id);
			return c == null ? instance.errorColor : c.color;
		} else {
			LabeledColor c = instance.defaultPalette.colorSet.FirstOrDefault(x => x.ID == id);
			return c == null ? instance.errorColor : c.color;
		}

	}

	public static int GetColorIndex(string id) {
		LabeledColor c = instance.defaultPalette.colorSet.FirstOrDefault(x => x.ID == id);
		return c == null ? -1 : c.index;
	}

	public static Palette[] GetPalettes() {
		Palette[] allPalettes = new Palette[instance.altPalettes.Length + 1];
		allPalettes[0] = instance.defaultPalette;
		instance.altPalettes.CopyTo(allPalettes, 1);
		return allPalettes;
	}

	public static LabeledColor[] GetCurrentColors() {
		return ChosenPalette.colorSet;
	}

	public static LabeledColor[] GetDefaultColors() {
		return instance.defaultPalette.colorSet;
	}

	private bool HasInstance() {
		return instance != null;
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(PaletteManager))]
	public class PaletteMgrEditor : Editor
	{
		private readonly float HEIGHT = EditorGUIUtility.singleLineHeight;
		private const float PADDING = 2f;

		private PaletteManager script;
		private SerializedProperty defaultPalette;
		private SerializedProperty defaultColorSet;
		private SerializedProperty altPalettes;

		//The "Add" button on defaultPalette's ColorSet MUST be overridden
		//	or else the GUID gets copied and the syncing won't work
		private ReorderableList defaultList;

		//Need to override the "Add" button to do the initial copying of the default palette
		private ReorderableList altPalettesList;

		private void OnEnable() {
			script = (PaletteManager)target;

			defaultPalette = serializedObject.FindProperty("defaultPalette");
			defaultColorSet = defaultPalette.FindPropertyRelative("colorSet");
			defaultList = new ReorderableList(serializedObject, defaultColorSet, true, false, true, true);
			defaultList.drawElementCallback = DrawColor;
			defaultList.onAddCallback = AddNewDefaultColor;
			defaultList.onReorderCallback = ReorderDefaultColors;
		
			altPalettes = serializedObject.FindProperty("altPalettes");
			altPalettesList = new ReorderableList(serializedObject, altPalettes, true, false, true, true);
			altPalettesList.drawElementCallback = DrawPalette;
			altPalettesList.onAddCallback = AddNewAltPalette;
			altPalettesList.elementHeightCallback = PaletteHeight;
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			script.dontDestroyOnLoad = EditorGUILayout.Toggle(new GUIContent("Don't Destroy on Load"), script.dontDestroyOnLoad);
			if(script.HasInstance()) {
				GUI.enabled = false;
				EditorGUILayout.TextField(new GUIContent("Currrent Palette"), script.GetChosenPalette().name);
				GUI.enabled = true;
			}
			EditorGUILayout.LabelField(script.defaultPalette.name, EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(defaultPalette.FindPropertyRelative("name"));
			EditorGUILayout.PropertyField(defaultPalette.FindPropertyRelative("description"));
			defaultList.DoLayoutList();

			EditorGUILayout.Space();
			altPalettesList.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
			if(script.altPalettes != null) {
				for(int i = 0; i < script.altPalettes.Length; i++) {
					SyncColors(script.altPalettes[i]);
				}
			}
			if (GUI.changed) { EditorUtility.SetDirty(script); }
		}
		private void SyncColors(Palette p) {
			LabeledColor[] defaultSet = script.defaultPalette.colorSet;

			if(defaultSet.Length > p.colorSet.Length) { //Default is longer = something was added
				p.AddColor(LabeledColor.Copy(defaultSet[defaultSet.Length - 1]));
			} else if (script.defaultPalette.colorSet.Length < p.colorSet.Length) { //Default is shorter = something was removed
				//NOTE - CURRENTLY NOT WORKING
				for (int i = 0; i < defaultSet.Length; i++) {
					LabeledColor match = p.colorSet.FirstOrDefault(x => x.ColorsArePaired(defaultSet[i]));
					if(match == null) {
						p.RemoveColor(match);
						break;
					}
				}
			}

			//Do this every time so indices stay synced even after removal
			for(int i = 0; i < p.colorSet.Length; i++) {
				LabeledColor match = defaultSet.FirstOrDefault(x => x.ColorsArePaired(p.colorSet[i]));
				if(match != null) {
					p.colorSet[i].name = match.name;
					p.colorSet[i].index = match.index;
				}
			}

			p.colorSet = p.colorSet.OrderBy(x => x.index).ToArray();
		}

		#region Default ReorderableList
		private void DrawColor(Rect position, int i, bool isActive, bool isFocused) {
			SerializedProperty property = defaultList.serializedProperty.GetArrayElementAtIndex(i);
			SerializedProperty color = property.FindPropertyRelative("color");
			SerializedProperty index = property.FindPropertyRelative("index");
			SerializedProperty name = property.FindPropertyRelative("name");

			int indent = EditorGUI.indentLevel * 15;

			Rect colorRect = new Rect(position);
			colorRect.width = 60;

			Rect indexRect = new Rect(position);
			indexRect.position = colorRect.position + Vector2.right * (colorRect.width + PADDING);
			indexRect.width = 20; //width of 2 chars

			Rect nameRect = new Rect(position);
			nameRect.position = indexRect.position + Vector2.right * (indexRect.width + PADDING);
			nameRect.width -= colorRect.width + indexRect.width + (PADDING * 2);

			colorRect.width += indent;
			indexRect.width += indent;
			EditorGUI.PropertyField(colorRect, color, GUIContent.none, true);
			EditorGUI.LabelField(indexRect, IntText(index.intValue));
			EditorGUI.PropertyField(nameRect, name, GUIContent.none, true);
		}
		private string IntText(int i) {
			if (Mathf.Abs(i) < 10) {
				if (i < 0) {
					return "-0" + i;
				} else {
					return "0" + i;
				}
			} else {
				return i.ToString();
			}
		}
		private void AddNewDefaultColor(object target) {
			int index = defaultColorSet.arraySize;
			defaultColorSet.arraySize++;
			defaultList.index = index;

			int otherDefaultNames = script.defaultPalette.colorSet.Where(x => x.name.Contains("New Color")).Count();
			string colorName = "New Color" + (otherDefaultNames > 0 ? $" ({otherDefaultNames})" : "");

			SerializedProperty element = defaultList.serializedProperty.GetArrayElementAtIndex(index);
			if(element.FindPropertyRelative("color").colorValue.a == 0) {
				Color c = element.FindPropertyRelative("color").colorValue;
				c = new Color(c.r, c.b, c.g, 1.0f);
				element.FindPropertyRelative("color").colorValue = c;
			}
			element.FindPropertyRelative("name").stringValue = colorName;
			element.FindPropertyRelative("id").stringValue = GUID.Generate().ToString();
			element.FindPropertyRelative("index").intValue = index;
		}
		private void ReorderDefaultColors(ReorderableList list) {
			SerializedProperty property = list.serializedProperty;
			for(int i = 0; i < property.arraySize; i++) {
				property.GetArrayElementAtIndex(i).FindPropertyRelative("index").intValue = i;
			}
		}

		#endregion

		#region Alt Palette ReorderableList
		private void DrawPalette(Rect position, int index, bool isActive, bool isFocused) {
			var element = altPalettesList.serializedProperty.GetArrayElementAtIndex(index);

			EditorGUI.BeginChangeCheck();
			SerializedProperty name = element.FindPropertyRelative("name");
			SerializedProperty description = element.FindPropertyRelative("description");
			SerializedProperty colorSet = element.FindPropertyRelative("colorSet");

			EditorGUI.BeginProperty(position, new GUIContent(name.stringValue), element);
			position.height = HEIGHT;
			element.isExpanded = EditorGUI.Foldout(position, element.isExpanded, name.stringValue, false, EditorStyles.foldoutHeader);

			if (element.isExpanded) {
				EditorGUI.indentLevel++;
				Rect pos = new Rect(position);
				pos.position += Vector2.up * HEIGHT;
				pos.height = HEIGHT;
				EditorGUI.PropertyField(pos, name);

				pos.position += Vector2.up * (HEIGHT + PADDING);
				pos.height = HEIGHT;
				EditorGUI.PropertyField(pos, description);

				pos.position += Vector2.up * (HEIGHT + PADDING);
				pos.height = HEIGHT;
				EditorGUI.BeginProperty(pos, new GUIContent("Color Set"), colorSet);
				pos.height = HEIGHT;
				colorSet.isExpanded = EditorGUI.Foldout(pos, colorSet.isExpanded, new GUIContent("Color Set"), false, EditorStyles.foldoutHeader);

				if (colorSet.isExpanded) {
					EditorGUI.indentLevel++;

					for (int i = 0; i < colorSet.arraySize; i++) {
						pos.position += Vector2.up * (HEIGHT + PADDING);
						pos.height = HEIGHT;
						EditorGUI.PropertyField(pos, colorSet.GetArrayElementAtIndex(i));
					}
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
				EditorGUI.EndProperty();
			}
			EditorGUI.EndProperty();

			if (EditorGUI.EndChangeCheck()) { element.serializedObject.ApplyModifiedProperties(); }
		}
		private float PaletteHeight(int index) {
			SerializedProperty property = altPalettesList.serializedProperty.GetArrayElementAtIndex(index);
			SerializedProperty colorSet = property.FindPropertyRelative("colorSet");

			if (property.isExpanded) {
				if (colorSet.isExpanded) {
					return HEIGHT * (colorSet.arraySize + 4.5f + (colorSet.arraySize == 0 ? 1 : 0)) + (PADDING * 3);
				} else {
					return HEIGHT * 4 + PADDING; //1 for header, 1 for name, 1 for description, 1 for array name
				}
			} else {
				return HEIGHT;
			}
		}
		private void AddNewAltPalette(object target) {
			int index = altPalettes.arraySize;
			altPalettes.arraySize++;
			altPalettesList.index = index;

			SerializedProperty element = altPalettesList.serializedProperty.GetArrayElementAtIndex(index);
			element.FindPropertyRelative("name").stringValue = "New Palette";
			SerializedProperty elementColors = element.FindPropertyRelative("colorSet");
			elementColors.ClearArray();
			elementColors.arraySize = defaultColorSet.arraySize;
			for(int i = 0; i < defaultColorSet.arraySize; i++) {
				SerializedProperty currElement = elementColors.GetArrayElementAtIndex(i);
				SerializedProperty currDefault = defaultColorSet.GetArrayElementAtIndex(i);
				currElement.FindPropertyRelative("color").colorValue = currDefault.FindPropertyRelative("color").colorValue;
				currElement.FindPropertyRelative("index").intValue = currDefault.FindPropertyRelative("index").intValue;
				currElement.FindPropertyRelative("name").stringValue = currDefault.FindPropertyRelative("name").stringValue;
				currElement.FindPropertyRelative("id").stringValue = currDefault.FindPropertyRelative("id").stringValue;
			}

			serializedObject.ApplyModifiedProperties();
		}
		#endregion
	}
	#endif
}