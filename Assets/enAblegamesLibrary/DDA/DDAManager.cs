using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enablegames;
using System.IO;
using FullSerializer;
using UnityEngine.Events;

[DefaultExecutionOrder(-25)]
/// <summary>
/// DDAManager is responsible for managing Dynamic Difficulty Adjustment (DDA) in the game.
/// It handles observations, updating difficulty, and invoking events related to DDA.
/// </summary>
public class DDAManager : MonoBehaviour
{
    /// <summary>
    /// Event that is triggered when the difficulty is about to change.
    /// </summary>
    public UnityEvent onDifficultyChanging = new UnityEvent();

    /// <summary>
    /// Represents the singleton instance of the DDAManager class.
    /// </summary>
    public static DDAManager Instance;

    /// <summary>
    /// Represents a performance element.
    /// </summary>
    public struct PerformanceElement
    {
        /// <summary>
        /// Class representing the DDAManager.
        /// </summary>
        public string Name;

        /// <summary>
        /// The threshold value for determining a perfect performance in the game.
        /// </summary>
        public float PerfectThreshold;

        /// <summary>
        /// Represents the Good Threshold value for a specific performance element.
        /// </summary>
        public float GoodThreshold;

        /// <summary>
        /// Represents a bad threshold value used to determine performance level.
        /// </summary>
        public float BadThreshold;
        
        /// <summary>
        /// Represents a worse threshold value used to determine performance level.
        /// </summary>
        public float WorseThreshold;

    }

    /// <summary>
    /// Represents a difficulty element.
    /// </summary>
    public struct DifficultyElement
    {
        /// <summary>
        /// Name of the Difficulty Element
        /// </summary>
        public string Name;

        /// <summary>
        /// A list of performances that can affect the difficulty element
        /// </summary>
        public List<string> Performances;

        /// <summary>
        /// Represents the number of levels in a difficulty element.
        /// </summary>
        public int NumberOfLevels;

    }

    /// <summary>
    /// The file path to the .json file with information about difficulty and performances.
    /// </summary>
    [SerializeField] [Tooltip("File path to the .json file with information about difficulty and performances")]
    private string jsonFilePath;

    /// <summary>
    /// The struct that holds the observations for the game.
    /// </summary>
    public struct Observations
    {
        /// <summary>
        /// Represents the performance elements for a game.
        /// </summary>
        public List<PerformanceElement> Performances;

        /// <summary>
        /// Represents the difficulty elements for a game.
        /// </summary>
        public List<DifficultyElement> Difficulties;
    }

    /// <summary>
    /// This variable indicates whether Dynamic Difficulty Adjustment (DDA) is enabled in the game.
    /// </summary>
    [Tooltip("Whether is using DDA")]
    public bool usingDDA;


    /// <summary>
    /// This variable indicates whether Biometric Data is being used for Dynamic Difficulty Adjustment (DDA) in the game.
    /// </summary>
    [Tooltip("Whether is using BioMetric Data for DDA")]
    public bool usingBiometric;


    /// <summary>
    /// Prefab of GameObject for DDA module.
    /// </summary>
    [Tooltip("Prefab of GameObject for DDA module")]
    public GameObject DDAObject;

    /// <summary>
    /// Dictionary of performance data dda take in for calculation.
    /// </summary>
    public Dictionary<string, float> PerformanceData = new Dictionary<string, float>();


    /// <summary>
    /// Dictionary that stores the suggested difficulty values for various aspects of the game.
    /// </summary>
    public Dictionary<string, int> DifficultyValues = new Dictionary<string, int>();
    
    private Dictionary<string, ObservationModule> ObservationModules = new Dictionary<string, ObservationModule>();

    /// <summary>
    /// Represents a mapping of performance elements.
    /// </summary>
    public Dictionary<string, PerformanceElement> PerformanceMapping = new Dictionary<string, PerformanceElement>();

    /// <summary>
    /// Class representing various observations and management of Dynamic Difficulty Adjustment (DDA).
    /// </summary>
    private Observations observations;
    
    private static fsSerializer _fsSerializer = new fsSerializer();


    /// <summary>
    /// This method is called when the DDAManager is instantiated. It sets the instance to this object if the instance is null and destroys any duplicate instances.
    /// Additionally, it ensures that the DDAManager object persists across scene changes.
    /// </summary>
    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        if (Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Starts the DDAManager and initializes the DDA system if enabled.
    /// If the .json file with information about difficulty and performances exists, it is loaded and used to initialize the DDAManager.
    /// Otherwise, the DDAManager is disabled.
    /// </summary>
    void Start()
    {
        if (!usingDDA)
        {
            return;
        }
        if (File.Exists(Path.Combine(Application.streamingAssetsPath, jsonFilePath)))
        {
            Initialize();
        }
        else
        {
            usingDDA = false;
        }
    }

    //Initializing DDA system
    /// <summary>
    /// Initializes the DDA system by loading the JSON file and setting up the observation modules and difficulty values.
    /// </summary>
    void Initialize()
    {
        String filePath = Path.Combine(Application.streamingAssetsPath, jsonFilePath);
        Debug.Log("Loading Json file at " + filePath);
        StreamReader reader = new StreamReader(filePath);
        var jsonstring = reader.ReadToEnd();
        fsData data = fsJsonParser.Parse(jsonstring);

        object deserialized = null;
        _fsSerializer.TryDeserialize(data,typeof(Observations), ref deserialized);
        Observations? newobservations = deserialized as Observations?;
        if (newobservations.HasValue)
        {
            observations = newobservations.Value;
        }
        else
        {
            usingDDA = false;
            Debug.Log("Failed to parse DDA.json");
            return;
        }
        
        foreach (var d in observations.Difficulties)
        {
            GameObject newObject = Instantiate(DDAObject, transform);
            ObservationModule o = newObject.GetComponent<ObservationModule>();
            o.name = d.Name;
            o.performances = d.Performances;
            o.nol = d.NumberOfLevels;
            o.Initialize();
            ObservationModules.Add(o.name, o);
            DifficultyValues.Add(d.Name, o.GetLevel());
        }

        foreach (var p in observations.Performances)
        {
            Debug.Log("performance to add: " + p.Name);
            PerformanceData.Add(p.Name, 0f);
            PerformanceMapping.Add(p.Name, p);
        }
        onDifficultyChanging.Invoke();
    }

    /// <summary>
    /// Observes the performance and updates the current difficulty value based on the observations.
    /// </summary>
    public void Observe()
    {
        foreach (var o in ObservationModules.Values)
        {
            o.Observe();
        }
    }

    /// <summary>
    /// Updates the current difficulty based on the observations and triggers the onDifficultyChanging event.
    /// </summary>
    public void UpdateDifficulty()
    {
        Observe();
        onDifficultyChanging.Invoke();
    }

    /// <summary>
    /// Updates the current difficulty based on the observations and triggers the onDifficultyChanging event.
    /// </summary>
    public void UpdateDifficulty(string name, int value)
    {
        if (value == -2 || value == -1 || value == 0 || value == 1 || value == 2)
        {
            ObservationModules[name].Observe(value);
        }
    }
    
}


