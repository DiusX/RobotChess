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
            t += Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(0f, 0.75f, t));
            if (t >= 0.75f)
            {
                isFadingIn = false;
                t = 0f;
            }
        }
        else
        {
            t += Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(0.75f, 0f, t));
            if (t >= 0.75f)
            {
                isFadingIn = true;
                t = 0f;
            }
        }
    }
}
