using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDayandNight : MonoBehaviour
{
    public static readonly string DAWN = "Dawn";
    public static readonly string DAY = "Day";
    public static readonly string AFTERNOON = "Afternoon";
    public static readonly string NIGHT = "Night";
    public static readonly string RAIN = "Rain";

    private Animator animator;
    private Rigidbody2D rb2d;

    private string currentAnimationState;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        animator.CrossFade(newState, 0.1f);
        currentAnimationState = newState;
    }
}
