using UnityEngine;
using TMPro;

public class InteractionText : MonoBehaviour
{
    public static InteractionText instance = null;

    TMP_Text interactionText;

    string interactionObject;
    string interactionButton;
    bool showText;



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

        interactionText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        ClearInteractionText();
    }
    
    void ClearInteractionText()
    {
        this.interactionObject = "";
        this.interactionButton = "";
        showText = false;

        ChangeInteractionText();
    }

    public void SetInteractionObject(string interactionObject, string interactionButton)
    {
        this.interactionObject = interactionObject;
        this.interactionButton = interactionButton;
        showText = true;
        
        ChangeInteractionText();
    }

    void ChangeInteractionText()
    {
        if(showText)
        {
            interactionText.text = "Press " + interactionButton + " to interact with " + interactionObject;
        }
        else
        {
            interactionText.text = "";
        }
    } 
}
