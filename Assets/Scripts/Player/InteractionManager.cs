    using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public Image interactImage;
    public TMP_Text interactText;
    private bool infoActive = false;

    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    public GameObject crosshair;
    private InputManager inputManager;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        inputManager = GetComponent<InputManager>();
        interactText.gameObject.SetActive(false);
        interactImage.gameObject.SetActive(false);
    }

    private void InteractableInfo(Interactable interact)
    {
        if (interact == null && infoActive)
        {
            interactText.gameObject.SetActive(false);
            interactImage.gameObject.SetActive(false);
            infoActive = false;
        }
        else if (interact != null && !infoActive)
        {
            interactText.gameObject.SetActive(true);
            interactImage.gameObject.SetActive(true);
            interactText.text = interact.infoText;
            interactImage.sprite = interact.infoImage;
            infoActive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.isMenuOpen) { 
            InteractableInfo(null);
            return;
        }
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitinfo;
        if (Physics.Raycast(ray, out hitinfo, distance, mask))
        {
            if (hitinfo.collider.GetComponent<Interactable>() != null)
            {
                crosshair.GetComponent<Image>().color = Color.green;
                Interactable interactable = hitinfo.collider.GetComponent<Interactable>();
                InteractableInfo(interactable);
                if (inputManager.onFoot.Interact.triggered)
                {
                    interactable.Call();
                }
            }
            else
            {
                InteractableInfo(null);
                crosshair.GetComponent<Image>().color = Color.white;
            }
        }
        else
        {
            InteractableInfo(null);
            crosshair.GetComponent<Image>().color = Color.white;
        }
    }
}
