using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class DialogGraphUtility
{
	public static TextField CreateTextField (string title = null, string value = null, EventCallback <ChangeEvent<string>> onValueChanged = null)
	{
		TextField textField = new TextField (title) {value = value};

		if (onValueChanged != null)
			textField.RegisterValueChangedCallback (onValueChanged);

		return textField;
	}

	public static TextField CreateTextField (string value = null, EventCallback <ChangeEvent<string>> onValueChanged = null)
	{
		TextField textField = new TextField() {value = value};

		if (onValueChanged != null)
			textField.RegisterValueChangedCallback (onValueChanged);

		return textField;
	}

	public static TextField CreateTextArea (string value = null, EventCallback <ChangeEvent<string>> onValueChanged = null)
	{
		TextField textArea = CreateTextField (value, onValueChanged);
		textArea.multiline = true;

		return textArea;
	}
}
