using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public static class DialogGraphUtility
{
	public static Port CreatePort (DialogNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
	{
		return node.InstantiatePort (Orientation.Horizontal, portDirection, capacity, typeof (float));
	}

	public static Button CreateButton (string text, Action onClick = null)
	{
		Button button = new Button (onClick) {text = text};

		return button;
	}

	public static Foldout CreateFoldout (string title, bool collapsed = false)
	{
		Foldout f = new Foldout()
		{
			text = title,
			value = !collapsed
		};

		return f;
	}

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
