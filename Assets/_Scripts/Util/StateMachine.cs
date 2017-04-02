using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    [Tooltip("don't touch this")]
    public static StateMachine instance = null;

	[Tooltip("whether or not the player was just in a puzzle scene")]
	public static bool wasInPuzzle = false;

    private Dictionary<string, bool> figments;
    private Dictionary<string, bool> memories;

    private static string savePath;


    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Destroy(gameObject);
        }

        savePath = UnityEngine.Application.persistentDataPath + "/savegame.dat";

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start() {
        figments = new Dictionary<string, bool>();
        memories = new Dictionary<string, bool>();
        populateDictionaries();
    }

    private void populateDictionaries() {
        figments.Add("policemanA", true);
        figments.Add("mayor", true);
        memories.Add("StovenPuzzle", true);
    }

    public void updateMemDictionary(string key, bool state) {
        memories[key] = state;
    }

    public void updateFigDictionary(string key, bool state) {
        figments[key] = state;
    }

    public bool isUnsolved(string sceneName) {
        return memories[sceneName];
    }

    /// <summary>
    /// Reset the state machine to it's default state
    /// </summary>
    public void ResetStateMachine()
    {
        figments = new Dictionary<string, bool>();
        memories = new Dictionary<string, bool>();
        populateDictionaries();
    }


    //TODO 
    ////write to file
    //static void SaveGame() {


    //    //using (MemoryStream ms = new MemoryStream()) {

    //    //    bf.Serialize(ms, data);
    //    //    ms.Position = 0;
    //    //    clone = (Dictionary<string, int>)bf.Deserialize(ms);
    //    //}



    //    BinaryFormatter bf = new BinaryFormatter();
    //    FileStream file = File.Create(savePath);
    //    bf.Serialize(file, gameData);
    //    file.Close();
    //    Debug.Log("File saved to: " + savePath);
    //}

    //static void LoadGame() {
    //    BinaryFormatter bf = new BinaryFormatter();
    //    FileStream file = File.Open(savePath, FileMode.Open);
    //    GameData gameData = (GameData)bf.Deserialize(file);
    //    file.Close();
    //}

    //[Serializable]
    //class GameData {

    //    public int currentLevel { get; set; }
    //    public List<int> disabledObjects;

    //    public GameData() {


    //    }
    //}
}
