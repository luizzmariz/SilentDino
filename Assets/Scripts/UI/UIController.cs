using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [SerializeField] InputAction Navigate;
    [SerializeField] InputAction Submit;
    [SerializeField] InputAction Cancel;

    public void EnableUIActions()
    {
        Navigate.Enable();
        Submit.Enable();
        Cancel.Enable();

        Navigate.performed += context => OnNavigate(context);
        Submit.performed += context => OnSubmit(context);
        Cancel.performed += context => OnCancel(context);
    }

    public void DisableUIActions()
    {
        Navigate.Disable();
        Submit.Disable();
        Cancel.Disable();
        
        Navigate.performed -= context => OnNavigate(context);
        Submit.performed -= context => OnSubmit(context);
        Cancel.performed -= context => OnCancel(context);
    }

    void OnNavigate(InputAction.CallbackContext context)
    {
        CanvaManager.instance.Navigate(context.ReadValue<Vector2>());
    }

    void OnSubmit(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            CanvaManager.instance.Submit();
        }
    }

    void OnCancel(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            CanvaManager.instance.Cancel();
        }
    }
}
