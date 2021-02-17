using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class ObjectInteraction : MonoBehaviour
{
    public Transform grabTransform;
    public Canvas valDisplay;

    public float startPos;
    public float startRot;
    public float currentPosOffset;
    public float currentRotOffset;
    public float controllerPosVal;
    public float controllerRotVal;
    public float maxVal =  10.0f;

    public float valRange;
    public Text valText;

    private bool interactionActive;

    // Start is called before the first frame update
    void Start()
    {
        if (valDisplay)
        {
            valText = valDisplay.transform.Find("ValText").GetComponent<Text>();
        }

        if (grabTransform.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable _grabObj))
        {
            if (transform.tag == "Fader")
            {
                startPos = _grabObj.transform.position.y;
                if (_grabObj.transform.TryGetComponent<ConfigurableJoint>(out ConfigurableJoint _joint))
                {
                    valRange = _joint.linearLimit.limit * 2;
                }
                SetPosValue();
            }
            else if (transform.tag == "Potentiometer")
            {
                startRot = _grabObj.transform.rotation.y;
                if (_grabObj.transform.TryGetComponent<HingeJoint>(out HingeJoint _joint))
                {
                    Quaternion _currentRotEuler = _grabObj.transform.rotation;
                    Quaternion _minRotEuler = _currentRotEuler;
                    Quaternion _maxRotEuler = _currentRotEuler;
                    _minRotEuler = Quaternion.Euler(_joint.limits.min, 0, 0) * _minRotEuler;
                    _maxRotEuler = Quaternion.Euler(_joint.limits.max, 0, 0) * _maxRotEuler;
                    valRange = _maxRotEuler.x - _minRotEuler.x;
                }
                SetRotValue();
            }
            else
            {
                Debug.Log("ObjectInteraction.cs: Tag not set on " + transform.name + ".");
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        SetCurrentOffset();

    }


    private void SetCurrentOffset()
    {
        if (transform.tag == "Fader")
        {
            currentPosOffset = grabTransform.position.y - startPos;
            SetPosValue();
        }
        else if (transform.tag == "Potentiometer")
        {
            currentRotOffset = startRot - grabTransform.rotation.y;
            SetRotValue();
        }
    }
    private void SetPosValue()
    {
        controllerPosVal = Mathf.Round((maxVal * (currentPosOffset + valRange / 2) / valRange) * 10) / 10;
        if (valText)
        {
            valText.text = controllerPosVal.ToString();
        }
    }
    private void SetRotValue()
    {
        controllerRotVal = Mathf.Round(maxVal * (currentRotOffset + valRange / 2) / valRange * 10) / 10;
        if (valText)
        {
            valText.text = controllerRotVal.ToString();
        }
    }
    public float GetCurrentValue()
    {
        float _currentVal;
        if (transform.tag == "Potentiometer")
        {
            _currentVal = controllerRotVal;
        }
        else if (transform.tag == "Fader")
        {
            _currentVal = controllerPosVal;
        }
        else
        {
            _currentVal = 0;
        }

        return _currentVal;
    }

    public void SetNewValue()
    {
        MainScript _Main = GameObject.Find("[Main]").GetComponent<MainScript>();
        if (_Main.currentAdjustableObj != null)
        {
            if (transform.name == _Main.currentAdjustableObj.name)
            {

                _Main.SetValueOfUser(GetCurrentValue());
            }
        }

    }

}
