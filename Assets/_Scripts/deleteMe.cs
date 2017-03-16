using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace ZoaProto {
    public class GameController : Controller<GameMessage> {

        [SerializeField]
        private GameObject playerCharacter;

        /// <summary>
        /// The path that the save data will be saved to.
        /// </summary>
        private static String savePath;

        private GameData gameData;

        [SerializeField]
        private int newGameScene;

        void Awake() {
            savePath = UnityEngine.Application.persistentDataPath + "/savegame.dat";
            gameData = new GameData();
        }

        public override void OnNotification(GameMessage message, GameObject obj, params object[] opData) {
            switch (message) {
                case GameMessage.NEW_GAME:
                    HandleNewGame();
                    break;
                case GameMessage.SAVE_GAME:
                    HandleSaveGame();
                    break;
                case GameMessage.LOAD_GAME:
                    HandleLoadGame();
                    break;
                case GameMessage.SPAWN_PLAYER:
                    HandleSpawnPlayer((Vector3)opData[0]);
                    break;
                case GameMessage.LOAD_SCENE:
                    HandleLoadScene(opData);
                    break;
                case GameMessage.PAUSE_GAME:
                    HandlePauseGame((bool)opData[0]);
                    break;
                case GameMessage.STOP_GAME:
                    HandleStopGame();
                    break;
                case GameMessage.SCHEDULE_RESTART:
                    HandleScheduleRestart((float)opData[0]);
                    break;
                case GameMessage.PERSIST_DISABLE:
                    HandlePersistDisable(obj);
                    break;
                case GameMessage.SET_PLAYER_ACTIVE:
                    HandleDisablePlayer((bool)opData[0]);
                    break;
                case GameMessage.UPDATE_SCORE:
                    HandleUpdateScore((int)opData[0]);
                    break;
            }
        }

        private void HandleLoadScene(object[] opData) {
            int scene = 0;
            float delay = 0f;
            foreach (object o in opData) {
                if (o is int) {
                    scene = (int)o;
                }
                if (o is float) {
                    delay = (float)o;
                }
            }
            StartCoroutine(ScheduleLoadScene(scene, delay));
        }

        private void HandleUpdateScore(int v) {
            gameData.score += v;
            app.NotifyUI(UIMessage.SCORE, null, gameData.score);
        }

        private void HandleSaveGame() {
            GameData data = GatherGameData();
            if (data != null) {
                gameData = data;
                SaveGameData(data, savePath);
            }
            else {
                Debug.LogError("Game data was null");
            }
        }

        private void HandleDisablePlayer(bool v) {
            PlayerBehavior player = FindObjectOfType<PlayerBehavior>();
            if (!player) {
                return;
            }
            if (!v) {
                player.gameObject.layer = LayerMask.NameToLayer("Default");
                player.SetBehaviorEnabled(v);
            }
            else {
                player.gameObject.layer = LayerMask.NameToLayer("Player");
                player.SetBehaviorEnabled(v);
            }
        }

        private void HandleClearDisabled(List<int> disabled) {
            Persistable[] objs = GameObject.FindObjectsOfType<Persistable>();
            for (int i = 0; i < objs.Length; i++) {
                if (disabled.Contains(objs[i].GetNameHash())) {
                    objs[i].gameObject.SetActive(false);
                }
            }

        }

        private void HandlePersistDisable(GameObject obj) {
            Persistable persist = obj.GetComponent<Persistable>();
            if (persist) {
                gameData.disabledObjects.Add(persist.GetNameHash());
            }
            else {
                Debug.LogError(obj.name + " does not have a Persistable Component");
            }
        }

        private void HandleScheduleRestart(float v) {
            StartCoroutine(ScheduleLoadScene(SceneManager.GetActiveScene().buildIndex, v));
        }

        IEnumerator ScheduleLoadScene(int scene, float delay) {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(scene);
            if (scene == gameData.currentLevel) {
                gameData = LoadGameData(savePath);
            }
        }

        private void HandleLoadGame() {
            gameData = LoadGameData(savePath);
            SceneManager.LoadScene(gameData.currentLevel);
        }

        private void HandleNewGame() {
            gameData = new GameData();
            SceneManager.LoadScene(newGameScene);
        }

        private void HandleSpawnPlayer(Vector3 spawnLocation) {
            GameObject player = Instantiate(playerCharacter);
            int score = 0;
            if (SceneManager.GetActiveScene().buildIndex == gameData.currentLevel) {
                spawnLocation = new Vector3(gameData.playerData.posX, gameData.playerData.posY, 0f);
                Damageable dam = player.GetComponent<Damageable>();
                int mHealth = gameData.playerData.maxHealth;
                int mShield = gameData.playerData.maxShield;
                dam.SetMaxHealth(mHealth);
                dam.SetHealth(mHealth);
                dam.SetMaxShieldPoints(mShield);
                dam.SetShieldPoints(mShield);
                HandleClearDisabled(gameData.disabledObjects);
                score = gameData.score;
            }

            app.NotifyUI(UIMessage.SCORE, null, score);
            player.transform.position = spawnLocation;
        }

        static void SaveGameData(GameData gameData, string savePath) {
            if (gameData == null) {
                throw new Exception("Game data was null");
            }
            if (savePath == "" || savePath == null) {
                throw new Exception("Save path was empty");
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(savePath);
            bf.Serialize(file, gameData);
            file.Close();
            Debug.Log("File saved to: " + savePath);
        }

        GameData GatherGameData() {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) {
                GameData game = new GameData();
                game.currentLevel = SceneManager.GetActiveScene().buildIndex;
                game.playerData.posX = player.transform.position.x;
                game.playerData.posY = player.transform.position.y;
                game.playerData.posZ = 0f;
                Damageable dam = player.GetComponent<Damageable>();
                game.playerData.maxHealth = dam.GetMaxHealth();
                game.playerData.maxShield = dam.GetMaxShieldPoints();
                game.playerData.health = game.playerData.maxHealth;
                game.playerData.shield = game.playerData.maxShield;
                game.disabledObjects = gameData.disabledObjects;
                game.score = gameData.score;
                return game;
            }
            else {
                Debug.LogError("Could not find player in scene.");
                return null;
            }
        }

        static GameData LoadGameData(string savePath) {
            if (File.Exists(savePath)) {
                Debug.Log("Loading game...");
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(savePath, FileMode.Open);
                GameData gameData = (GameData)bf.Deserialize(file);
                file.Close();
                return gameData;
            }
            else {
                Debug.Log("There is no save game to load");
                return null;
            }
        }

        /// <summary>
        /// Sets the time scale to zero if true, 1 if false and notifies the UI that a pause event has occurred.
        /// </summary>
        /// <param name="isPaused"></param>
        void HandlePauseGame(bool isPaused) {
            Time.timeScale = isPaused ? 0 : 1;
            app.NotifyUI(UIMessage.PAUSE_MENU, gameObject, isPaused);
            PlayerBehavior pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
            if (pb) {
                pb.SetBehaviorEnabled(!isPaused);
            }
        }

        /// <summary>
        /// Handles disabling UI elements, and loading the main menu.
        /// </summary>
        void HandleStopGame() {
            app.NotifyUI(UIMessage.PAUSE_MENU, gameObject, false);
            HandlePauseGame(false);
            SceneManager.LoadScene(0);
        }

        [Serializable]
        class GameData {
            public int score;
            public int currentLevel { get; set; }
            public PlayerData playerData { get; set; }
            public List<int> disabledObjects;

            public GameData() {
                playerData = new PlayerData();
                disabledObjects = new List<int>();
            }
        }

        [Serializable]
        class PlayerData {
            public float posX;
            public float posY;
            public float posZ;
            public int maxHealth;
            public int maxShield;
            public int health;
            public int shield;
        }
    }
}