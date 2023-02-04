using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Palette
{
	public string name;
	public string description;
	public LabeledColor[] colorSet;

	public void AddColor(LabeledColor color) {
		List<LabeledColor> list = colorSet.ToList();
		list.Add(color);
		colorSet = list.ToArray();
	}

	public void RemoveColor(LabeledColor color) {
		LabeledColor match = colorSet.FirstOrDefault(x => x.ColorsArePaired(color));
		if(match != null) {
			List<LabeledColor> list = colorSet.ToList();
			list.Remove(match);
			colorSet = list.ToArray();
		}
	}
}

[Serializable]
public class LabeledColor
{
	public Color color = Color.black;
	public string name;
	public int index;

	[SerializeField] private string id;

	public string ID { get { return id; } }

	private LabeledColor(Color c, string n, int i, string guid) {
		color = c;
		name = n;
		index = i;
		id = guid;
	}

	public bool ColorsArePaired(LabeledColor other) {
		return id.Equals(other.id);
	}

	public static LabeledColor Copy(LabeledColor source) {
		return new LabeledColor(source.color, source.name, source.index, source.id);
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LabeledColor))]
public class LabeledColorDrawer : PropertyDrawer
{
	private const float PADDING = 5f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
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
		EditorGUI.LabelField(nameRect, name.stringValue);
	}

	private string IntText(int i) {
		if(Mathf.Abs(i) < 10) {
			if(i < 0) {
				return "-0" + i;
			} else {
				return "0" + i;
			}
		} else {
			return i.ToString();
		}
	}
}

[CustomPropertyDrawer(typeof(Palette))]
public class PaletteDrawer : PropertyDrawer
{
	private readonly float HEIGHT = EditorGUIUtility.singleLineHeight;
	private const float PADDING = 2f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		SerializedProperty name = property.FindPropertyRelative("name");
		SerializedProperty description = property.FindPropertyRelative("description");
		SerializedProperty colorSet = property.FindPropertyRelative("colorSet");

		EditorGUI.BeginProperty(position, new GUIContent(name.stringValue), property);
		position.height = HEIGHT;
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, name.stringValue, false, EditorStyles.foldoutHeader);

		if (property.isExpanded) {
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
			EditorGUI.PropertyField(pos, colorSet);
		}
		EditorGUI.indentLevel--;
		EditorGUI.EndProperty();
		if (GUI.changed) { 
			for(int i = 0; i < colorSet.arraySize; i++) {
				colorSet.GetArrayElementAtIndex(i).FindPropertyRelative("index").intValue = i;
			}
			EditorUtility.SetDirty(property.serializedObject.targetObject); 
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		SerializedProperty colorSet = property.FindPropertyRelative("colorSet");

		if (property.isExpanded) {
			if (colorSet.isExpanded) {
				return HEIGHT * (colorSet.arraySize + 6 + (colorSet.arraySize == 0 ? 1 : 0)) + (PADDING * 3);
			} else {
				return HEIGHT * 5 + PADDING; //1 for header, 1 for name, 1 for description, 1 for array name
			}
		} else {
			return HEIGHT;
		}

	}
}
#endif
