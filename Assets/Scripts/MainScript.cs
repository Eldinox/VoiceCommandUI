using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    [Header("Set Objects for Tasks")]
    public List<GameObject> adjustableObjs = new List<GameObject>();
    public float maxTargetVal;
    public List<float> timeToSetVals = new List<float>();
    public Canvas taskDisplay;
    public GameObject currentAdjustableObj;

    private bool userIsInteracting = false;
    private bool studyStarted = false;
    private bool taskActive = false;
    private Text taskText;
    private float targetVal;
    private float valSetByUser;
    private string[] incOrDec = {"Increase", "Decrease"};

    // Start is called before the first frame update
    void Start()
    {
        currentAdjustableObj = null;

        if (!adjustableObjs.Any())
        {
            Debug.Log("MainScript: No adjustable Objects for Tasks given!");
        }
        if (!taskDisplay)
        {
            Debug.Log("MainScript: No Canvas Set to display new Tasks, select one first!");
        }
        else
        {
            if (taskDisplay.transform.Find("TaskText").TryGetComponent<Text>(out Text _taskText))
            {
                taskText = _taskText;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (studyStarted && !taskActive)
        {
            ShuffleListExtension.Shuffle(adjustableObjs);
            GenerateNewTask();
            taskActive = true;
        }
        if(taskActive)
        {
            if (valSetByUser == targetVal)
            {
                taskText.text = "Value correctly Set!";
                taskActive = false;
            }

        }
    }

    private void GenerateNewTask()
    {
        int idx = Random.Range(0, incOrDec.Length);
        currentAdjustableObj = adjustableObjs[0];
        float _currentVal = GetCurrentValOfObj(currentAdjustableObj);
        float _Range;
        float _taskVal;

        Debug.Log("MainScript.cs: Current Value of " + currentAdjustableObj.name + " is " + _currentVal);

        if (incOrDec[idx] == "Increase")
        {
            _Range = maxTargetVal - _currentVal;
        }
        else if (incOrDec[idx] == "Decrease")
        {
            _Range = _currentVal * -1;

        }
        else
        {
            _Range = 0;
        }

        _taskVal = Mathf.Round(Random.Range(0, _Range) * 10) / 10;
        targetVal = _currentVal + _taskVal;

        Debug.Log("MainScript.cs: Target Value is " + targetVal);
        taskText.text = incOrDec[idx] + adjustableObjs[0].name + " by " + _taskVal;
    }

    public void SetStudyStarted()
    {
        if(studyStarted == false)
        {
            studyStarted = true;
        }
        else
        {
            Debug.Log("MainScript: Study already started, check for your Tasks!");
        }
    }

    private float GetCurrentValOfObj(GameObject _obj)
    {
        float _currentVal = 0.0f;
        if(_obj.TryGetComponent<ObjectInteraction>(out ObjectInteraction _objStats))
        {
            _currentVal = _objStats.GetCurrentValue();
        }
        else
        {
            Debug.Log("MainScript: No ObjectInteraction-Script found on " + _obj.name + ".");
        }

        return _currentVal;
    }

    public void SetValueOfUser(float _valOfUser)
    {
        if (taskActive)
        {
            valSetByUser = _valOfUser;
        }
    }
}
