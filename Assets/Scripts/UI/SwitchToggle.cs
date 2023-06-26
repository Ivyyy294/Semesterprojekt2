using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Image))]
public class SwitchToggle : MonoBehaviour
{
	[SerializeField] AudioAsset audioOnToggle;
	[SerializeField] Sprite offSprite;
	[SerializeField] Sprite onSprite;
	public bool active;

	Image image;

	public void Toggle()
	{
		active = !active;
		audioOnToggle?.Play();
	}
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent <Image>();
    }

	private void OnValidate()
	{
		if (image == null)
			image = GetComponent <Image>();

		if (active && onSprite != null)
			image.sprite = onSprite;
		else if (offSprite != null)
			image.sprite = offSprite;
	}

	// Update is called once per frame
	void Update()
    {
        if (active)
			image.sprite = onSprite;
		else
			image.sprite = offSprite;
    }
}
