using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationButton : MonoBehaviour
{
    public void ActiveGameobject()
    {
        gameObject.SetActive(true);
    }

    public void UnActiveGameobject()
    {
        gameObject.SetActive(false);
    }
}
