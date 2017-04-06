using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NPCMovement : MonoBehaviour {

    [Tooltip("The movement speed of the NPC")]
    [SerializeField]
    private float moveSpeed;

    [Tooltip("Where the NPC should go")]
    [SerializeField]
    private Vector3 target;

    [Tooltip("Whether or not the sprite fades away once reaching it's target")]
    [SerializeField]
    private bool fadeAway;
    [Tooltip("How long the sprite renderer takes to fad away")]
    [SerializeField]
    private float fadeTime = 1f;

    private SpriteRenderer sr;

    private Tweener tween;

    // Use this for initialization
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Moves the character towards position at the default speed
    /// </summary>
	public void MoveCharacter()
    {
        MoveCharacter(target, moveSpeed);
    }

    /// <summary>
    /// Moves the character
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="duration"></param>
    public void MoveCharacter(Vector3 targetPosition, float duration)
    {
        tween = transform.DOMove(targetPosition, duration).SetSpeedBased(true).SetAutoKill(true);
        if (fadeAway)
             StartCoroutine(FadeAway(tween));
    }

    private IEnumerator FadeAway(Tweener t)
    {
        yield return t.WaitForKill();
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Fade out the alpha channel of the object
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOut()
    {
        Color itemColor = sr.material.color;
        while (itemColor.a > 0)
        {
            itemColor.a -= 0.01f;
            sr.material.color = itemColor;
            yield return new WaitForSeconds(fadeTime);
        }
        gameObject.SetActive(false);
    }
}
