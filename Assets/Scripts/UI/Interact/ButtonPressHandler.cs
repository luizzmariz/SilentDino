using UnityEngine;
using UnityEngine.UI;

public class ButtonPressHandler : MonoBehaviour
{
    public Button targetButton; // The button to be pressed
    public KeyCode activationKey = KeyCode.Space; // Default key to trigger the button press

    void Update()
    {
        if (targetButton != null && targetButton.interactable && Input.GetKeyDown(activationKey))
        {
            targetButton.onClick.Invoke(); // Simulate button press
        }
    }
}
