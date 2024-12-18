using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interact with objects in space")]
    [SerializeField] Camera mainCamera;
    [SerializeField] float interactRayDistance;
    [SerializeField] LayerMask layerMask;

    Interactable interactable;

    void LateUpdate()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactRayDistance, Color.red);

        CastInteractionRay(ray);
    }

    void CastInteractionRay(Ray ray)
    {
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, interactRayDistance, layerMask))
        {
            CheckInteractability(hitInfo);
        } 
        else
        {
            SetInteractable(null);
        }
    }

    void CheckInteractability(RaycastHit hitInfo)
    {
        if(hitInfo.collider.tag == "Interactable")
        {
            Interactable interactableObject = hitInfo.collider.GetComponent<Interactable>();
            //Debug.Log(hitInfo.collider.name + " " + interactableObject);

            if(interactableObject != null)
            {
                //Debug.Log("grewg");
                SetInteractable(interactableObject);
                ChangeInteractionText(interactableObject.interactableName);
            }
            else
            {
                SetInteractable(null);
                ChangeInteractionText("???");
            }
        }
        else
        {
            SetInteractable(null);
        }
    }

    void SetInteractable(Interactable _interactable)
    {
        if(_interactable != null)
        {
            this.interactable = _interactable; 
        }
        else
        {
            Debug.Log("gg");
            this.interactable = null;
        }
    }

    void ChangeInteractionText(string interactableName)
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        string interactionbutton = playerInput.actions["Interact"].GetBindingDisplayString(group: playerInput.currentControlScheme);

        InteractionText.instance.SetInteractionObject(interactableName, interactionbutton);
    }

    public void Interact()
    {
        if(interactable != null)
        {
            interactable.Interact();
        }
    }
}