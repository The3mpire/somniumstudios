using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Somnium
{
    public class StateMachine : MonoBehaviour, IDialogState
    {

        [HideInInspector]
        public static StateMachine instance = null;

        [Tooltip("whether or not the player was just in a puzzle scene")]
        public static bool wasInPuzzle = false;

        private Dictionary<string, bool> figments;
        private Dictionary<string, bool> memories;

        private Dictionary<string, int> flags;

        private static string savePath;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            savePath = UnityEngine.Application.persistentDataPath + "/savegame.dat";

            DontDestroyOnLoad(gameObject);
        }

        // Use this for initialization
        void Start()
        {
            figments = new Dictionary<string, bool>();
            memories = new Dictionary<string, bool>();
            flags = new Dictionary<string, int>();
            populateDictionaries();
        }

        private void populateDictionaries()
        {
            figments.Add("policemanA", true);
            figments.Add("mayor", true);
            memories.Add("StovenPuzzle", true);
            flags.Add("StovenPuzzle", 0);
        }

        public void updateMemDictionary(string key, bool state)
        {
            memories[key] = state;
            //Don't want to break anything, so just add the key to the flag dictionary.
            flags[key] = Convert.ToInt32(!state);
        }

        public void updateFigDictionary(string key, bool state)
        {
            figments[key] = state;
            //Don't want to break anything, so just add the key to the flag dictionary.
            flags[key] = Convert.ToInt32(!state);
        }

        public bool isUnsolved(string sceneName)
        {
            return memories[sceneName];
        }

        /// <summary>
        /// Reset the state machine to it's default state
        /// </summary>
        public void ResetStateMachine()
        {
            figments = new Dictionary<string, bool>();
            memories = new Dictionary<string, bool>();
            flags = new Dictionary<string, int>();
            populateDictionaries();
        }

        /// <summary>
        /// This is part of the DialogState interface, DialogManager uses it to set flags when choices are made.
        /// </summary>
        public void SetFlag(string flagName, int flagValue)
        {
            flags[flagName] = flagValue;
        }

        /// <summary>
        /// DialogManager uses this to check whether or not to display certain dialogs.
        /// </summary>
        public int GetFlag(string flagName)
        {
            return flags[flagName];
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
}
