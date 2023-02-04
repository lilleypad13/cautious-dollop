using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorSwatch : MonoBehaviour
{
	public Image colorImg;
	public TextMeshProUGUI colorLabel;

	public void SetColor(LabeledColor color) {
		colorImg.color = color.color;
		colorLabel.text = color.name;
	}
}
