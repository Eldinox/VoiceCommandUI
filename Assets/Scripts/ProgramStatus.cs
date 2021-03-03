using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Linq;

public class ProgramStatus : MonoBehaviour
{
    [Header("VR RIG")]
    public GameObject vrRig;

    [Header("Current Adjustable Objects")]
    public List<GameObject> adjustableObjs;

    [Header("Set Names of Scenes")]
    public List<string> studyScenes = new List<string>();
    public List<string> inputTypes = new List<string>();
    public List<List<string>> orderOfStudy = new List<List<string>>();

    [Header("User ID")]
    public string userID;

    [Header("Button to start Study")]
    public Button startButton;

    [Header("Task Display")]
    public GameObject taskDisplay;

    public float maxTargetVal;

    public float countDownBeforeStartOfStudy;

    public GameObject[] objectsNotToDestroy;

    private int currentTaskIdx;
    private bool studyStarted = false;
    private bool taskInProgress = false;
    private bool studyCompleted = false;
    private bool countdownActive = false;
    private float countDownTimer;

    // Taken from MainScript.cs
    private float startVal;
    private int taskCount;
    public GameObject currentAdjustableObj;
    private string[] incOrDec = { "Increase", "Decrease" };
    private Text taskText;
    private float targetVal;
    private float valSetByUser;
    private float timeToCompleteTask;
    private bool sceneSetup = false;
    private GameObject currentMainScript;

    static ProgramStatus GlobalProgramStatus;

    private void Awake()
    {
        userID = "ID" + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Month + System.DateTime.Now.Day + "_" + UnityEngine.Random.Range(0, 99);
        GenerateStudyOrder();
    }

    private void Start()
    {
        if (GlobalProgramStatus != null)
        {
            Destroy(this.gameObject);
            return;
        }
        GlobalProgramStatus = this;
        GameObject.DontDestroyOnLoad(this.gameObject);

        foreach(GameObject _gameObj in objectsNotToDestroy)
        {
            GameObject.DontDestroyOnLoad(_gameObj);
        }

        currentTaskIdx = 0;

        if (taskDisplay.transform.Find("TaskText").TryGetComponent<Text>(out Text _taskText))
        {
            taskText = _taskText;
        }
    }
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (studyCompleted == false)
        {
            if (studyStarted == true)
            {
                if (taskInProgress == false)
                {
                    if (!countdownActive)
                    {
                        if (!adjustableObjs.Any())
                        {
                            GetAdjustableObjectsOfCurrentScene();
                        }
                        else if (adjustableObjs.Any())
                        {
                            SetUserPosition();
                            countdownActive = true;
                            countDownTimer = countDownBeforeStartOfStudy + 1;
                        }

                        if (!taskDisplay.activeSelf)
                        {
                            taskDisplay.SetActive(true);
                        }
                    }
                    else
                    {
                        if (countDownTimer <= 1)
                        {
                            taskDisplay.SetActive(false);
                            if (adjustableObjs.Any())
                            {
                                GenerateNewTask();
                                taskInProgress = true;
                                countdownActive = false;
                            }
                        }
                        else
                        {
                            countDownTimer -= Time.deltaTime;
                            taskDisplay.transform.Find("TaskText").GetComponent<Text>().text = "Study starts in:\n" + (Mathf.Floor(countDownTimer)).ToString();
                        }
                    }
                }
                else if (taskInProgress == true)
                {
                    if (valSetByUser == targetVal)
                    {

                        taskText.text = "Value correctly Set!";
                        taskInProgress = false;
                        List<string> _result = new List<string>();

                        Debug.Log("MainScript.cs: Time to complete Task was: " + Mathf.Round(timeToCompleteTask * 1000) / 1000 + " s.");
                        //AddTaskResult();
                    }
                    else
                    {
                        timeToCompleteTask += Time.deltaTime;
                    }
                }
            }
        } 
        else
        {
            Debug.Log("ProgramStatus.cs: Thank you for participating in this study. Please fill out the last form and send your results to xy!");
        }
    }

    private void GenerateStudyOrder()
    {
        if (studyScenes != null)
        {
            foreach (string _studyScene in studyScenes)
            {
                foreach (string _inputType in inputTypes)
                {
                    List<string> _case = new List<string>();
                    _case.Add(_studyScene);
                    _case.Add(_inputType);
                    orderOfStudy.Add(_case);
                }
            }
        }
        else
        {
            Debug.Log("ProgramStatus.cs: No scene names given!");
        }
        ShuffleListExtension.Shuffle(orderOfStudy);
    }

    public void StartStudy()
    {
        if (studyStarted == false && taskInProgress == false && studyCompleted == false)
        {
            studyStarted = true;
            SceneManager.LoadScene(orderOfStudy[currentTaskIdx][0]);    
        }
    }

    public void TaskCompleted()
    {
        taskInProgress = false;
        if (currentTaskIdx < orderOfStudy.Count)
        {
            currentTaskIdx += 1;
        }
        else
        {
            studyCompleted = true;
        }
    }

    public void StartCountdown()
    {

    }

    public void GetAdjustableObjectsOfCurrentScene()
    {
        GameObject[] _sceneObjs = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject _gameObj in _sceneObjs)
        {
            if (_gameObj.name == "[Main]")
            {
                currentMainScript = _gameObj;
                adjustableObjs = _gameObj.GetComponent<MainScript>().GetAdjustableObjects();
                ShuffleListExtension.Shuffle(adjustableObjs);
            }
        }
    }

    public void SetUserPosition()
    {
        if (currentMainScript.TryGetComponent<MainScript>(out MainScript _mainScript))
        {
            vrRig.transform.position = _mainScript.GetUserPosition();
        }
    }

    private void GenerateNewTask()
    {
        startVal = 0;
        if (taskCount < adjustableObjs.Count)
        {
            int idx = UnityEngine.Random.Range(0, incOrDec.Length);
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

            _taskVal = Mathf.Round(UnityEngine.Random.Range(0, _Range) * 10) / 10;
            targetVal = _currentVal + _taskVal;

            //Debug.Log("MainScript.cs: Target Value is " + targetVal);
            taskDisplay.SetActive(true);
            taskText.text = incOrDec[idx] + " " + adjustableObjs[taskCount].name + " by " + _taskVal;
            taskCount += 1;
        }
        else
        {
            Debug.Log("MainScript.cs: All Tasks completed. Move on to the next Section!");
            // PrintResults();
        }
    }

    private float GetCurrentValOfObj(GameObject _obj)
    {
        float _currentVal = 0.0f;
        if (_obj.TryGetComponent<ObjectInteraction>(out ObjectInteraction _objStats))
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
        if (taskInProgress)
        {
            valSetByUser = _valOfUser;
        }
    }
}
