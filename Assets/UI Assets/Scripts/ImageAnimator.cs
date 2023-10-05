using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour
{
    private Image image;
    private float t = 0f;
    private bool isFadingIn = true;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (isFadingIn)
        {
            t += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0f, 1f, t));
            if (t >= 0.75f)
            {
                isFadingIn = false;
                t = 0f;
            }
        }
        else
        {
            t += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(1f, 0f, t));
            if (t >= 0.75f)
            {
                isFadingIn = true;
                t = 0f;
            }
        }
    }
}