using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float t = 0f;
    private bool isFadingIn = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isFadingIn)
        {
            t += Time.deltaTime / 3;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(0f, 1f, t));
            if (t >= 1f)
            {
                isFadingIn = false;
                t = 0f;
            }
        }
        else
        {
            t += Time.deltaTime / 3;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(1f, 0f, t));
            if (t >= 1f)
            {
                isFadingIn = true;
                t = 0f;
            }
        }
    }
}
