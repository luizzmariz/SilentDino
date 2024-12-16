using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public static FadeScreen instance = null;

    Animator animator;

    public bool fadeOcurring;
    public bool screenIsFaded;

    void Awake()
    {
        if(instance == null) 
        {
			instance = this;
		} 
        else if(instance != this) 
        {
			Destroy(gameObject);
		}

        animator = GetComponent<Animator>();

        fadeOcurring = false;
        screenIsFaded = false;
    }

    public void FadeInScreen()
    {
        if(!screenIsFaded && !fadeOcurring)
        {
            fadeOcurring = true;
            animator.SetTrigger("FadeIn");
        }
    }

    public void FadeOutScreen()
    {
        if(screenIsFaded && fadeOcurring)
        {
            animator.SetTrigger("FadeOut");
        }
    }

    public void FadeFinished()
    {
        if(!screenIsFaded)
        {
            screenIsFaded = true;
        }
        else
        {
            fadeOcurring = false;
            screenIsFaded = false;
        }
    }
}
