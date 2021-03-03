using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    [Header("Set Objects for Tasks")]
    public List<GameObject> adjustableObjs = new List<GameObject>();

    [Header("Max Value to Set")]
    public float maxTargetVal;

    //public List<float> timeToSetVals = new List<float>();

    [Header("Set Task Head-up-Display")]
    public Canvas taskDisplay;

    [Header("Information")]
    public GameObject currentAdjustableObj;

    [Header("Position of User")]
    public GameObject userPosition;

    private bool userIsInteracting = false;
    private bool studyStarted = false;
    private bool taskActive = false;
    private Text taskText;
    private float startVal;
    private float targetVal;
    private float valSetByUser;
    private string[] incOrDec = {"Increase", "Decrease"};
    private string[] appMode = { "Voice", "Controller" };
    private int taskCount = 0;
    private float timeToCompleteTask;
    private List<List<string>> results = new List<List<string>>();
    private string userID;
    private bool resultsPrinted = false;

    // Start is called before the first frame update
    void Start()
    {
        userID = "ID" + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Month + System.DateTime.Now.Day + "_" + Random.Range(0, 99);
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

        if (adjustableObjs.Any<GameObject>())
        {
            ShuffleListExtension.Shuffle(adjustableObjs);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (studyStarted && !taskActive)
        {
            timeToCompleteTask = 0;
            GenerateNewTask();
            taskActive = true;
        }
        if(taskActive)
        {   
            if (valSetByUser == targetVal)
            {

                taskText.text = "Value correctly Set!";
                taskActive = false;
                List<string> _result = new List<string>();

                Debug.Log("MainScript.cs: Time to complete Task was: " + Mathf.Round(timeToCompleteTask * 1000)/ 1000 + " s.");
                AddTaskResult();
            }
            else
            {
                timeToCompleteTask += Time.deltaTime;
            }
        }
        */
    }

    private void GenerateNewTask()
    {
        startVal = 0;
        if(taskCount < adjustableObjs.Count)
        {
            int idx = Random.Range(0, incOrDec.Length);
            currentAdjustableObj = adjustableObjs[taskCount];
            float _currentVal = Mathf.Round(GetCurrentValOfObj(currentAdjustableObj) * 10) / 10;          
            float _Range;
            float _taskVal;

            startVal = _currentVal;
            // Debug.Log("MainScript.cs: Current Value of " + currentAdjustableObj.name + " is " + _currentVal);

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

            //Debug.Log("MainScript.cs: Target Value is " + targetVal);
            taskText.text = incOrDec[idx] + " " + adjustableObjs[taskCount].name + " by " + _taskVal;
            taskCount += 1;
        }
        else
        {
            Debug.Log("MainScript.cs: All Tasks completed. Move on to the next Section!");
            PrintResults();
        }
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
    
    private void AddTaskResult()
    {
        List<string> _result = new List<string>();
        float _valueChange = targetVal - startVal;
        _result.Add(userID);
        _result.Add(SceneManager.GetActiveScene().name);
        _result.Add(Random.Range(0, 1).ToString());
        _result.Add(startVal.ToString());
        _result.Add(targetVal.ToString());
        _result.Add(_valueChange.ToString());
        _result.Add((Mathf.Round(timeToCompleteTask * 1000) / 1000).ToString());
        results.Add(_result);
    }

    private void PrintResults()
    {
        if (!resultsPrinted)
        {
            string path = "Assets/Results/" + userID + ".txt";

            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, true);
            foreach (List<string> _result in results)
            {
                foreach (string _data in _result)
                {
                    writer.Write(_data + ";");
                }
                writer.WriteLine();
            }
            writer.Close();
            resultsPrinted = true;
        }
        else
        {

        }

    }

    public List<GameObject> GetAdjustableObjects()
    {
        List<GameObject> _adjustableObjects = new List<GameObject>();
        if ( adjustableObjs != null)
        {
            List<GameObject> _adjustableGameObjects = adjustableObjs;
        }
        else
        {
            Debug.Log("MainScript.cs: There are no adjustable Objects!");
        }
        return adjustableObjs;
    }

    public Vector3 GetUserPosition()
    {
        Vector3 _userPosition = new Vector3();
        if (userPosition != null)
        {
            _userPosition = userPosition.transform.position;
        }
        return _userPosition;
    }
}
