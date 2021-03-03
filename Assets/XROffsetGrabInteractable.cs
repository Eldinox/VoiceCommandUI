using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    [Header("Set Parent of Interactable")]
    public GameObject parentObj;

    [Header("Set Interaction Audio Source")]
    public AudioSource interactionAudio;

    private Vector3 initialAttachLocalPos;
    private Quaternion initialAttachLocalRot;

    // Start is called before the first frame update
    void Start()
    {
        if (!attachTransform)
        {
            GameObject grab = new GameObject("Grab Pivot");
            grab.transform.SetParent(transform, false);
            attachTransform = grab.transform;
        }

        initialAttachLocalPos = attachTransform.localPosition;
        initialAttachLocalRot = attachTransform.localRotation;

        if (!parentObj)
        {
            Debug.Log("XROffsetGrabInteractable.cs: Set parent for gameObject " + transform.name + "!");
        }
    }

    protected override void OnSelectEntering(XRBaseInteractor interactor)
    {
        if (interactor is XRDirectInteractor)
        {
            attachTransform.position = interactor.transform.position;
            attachTransform.rotation = interactor.transform.rotation;
        }
        else
        {
            attachTransform.localPosition = initialAttachLocalPos;
            attachTransform.localRotation = initialAttachLocalRot;
        }

        base.OnSelectEntering(interactor);
    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        if (parentObj.TryGetComponent<ObjectInteraction>(out ObjectInteraction _parentObject))
        {
            _parentObject.SetNewVal();
        }
        base.OnSelectExited(interactor);
    }

    protected override void OnHoverEntering(XRBaseInteractor interactor)
    {
        if(parentObj.TryGetComponent<ObjectInteraction>(out ObjectInteraction _objInteraction)){
            _objInteraction.SetObjectHighlight(true);
            Debug.Log("Highlight Object!");
        }
        base.OnHoverEntering(interactor);
    }

    protected override void OnHoverEntered(XRBaseInteractor interactor)
    {
        if(parentObj.TryGetComponent<ObjectInteraction>(out ObjectInteraction _objInteraction))
        {
            interactionAudio.clip = _objInteraction.hoverSound;
            interactionAudio.Play();
        }

        base.OnHoverEntered(interactor);
    }

    protected override void OnHoverExited(XRBaseInteractor interactor)
    {
        if (parentObj.TryGetComponent<ObjectInteraction>(out ObjectInteraction _objInteraction))
        {
            _objInteraction.SetObjectHighlight(false);
            Debug.Log("Highlight Object!");
        }
        base.OnHoverExited(interactor);
    }

}
