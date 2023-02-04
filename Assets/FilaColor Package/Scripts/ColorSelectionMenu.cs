using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorSelectionMenu : MonoBehaviour
{
	public TMP_Dropdown optionsDropdown;
	public TextMeshProUGUI descriptionText;
	public ColorSwatch swatchPrefab;
	public Transform swatchHolder;

	//Note: the prefab for ColorSwatch is set up as a Toggle
	//So once all of them are spawned in, they need to be added to a ToggleGroup
}
