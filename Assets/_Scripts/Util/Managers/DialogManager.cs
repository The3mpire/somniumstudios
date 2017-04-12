using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Collections;

namespace Somnium
{
    public class DialogManager : MonoBehaviour
    {

        #region Member Vars

        /// <summary>
        /// The instance of the dialog manager, provides access to manager methods and properties.
        /// </summary>
        public static DialogManager Instance { get; private set; }

        /// <summary>
        /// Whether or not the manager is currently running a DisplayDialog routine.
        /// Used for handling the scheduling of displaying other dialogs.
        /// </summary>
        public bool RunningDisplayRoutine { get; private set; }

        private IDialogState state;

        /// <summary>
        /// dialogs that the manager is currently working on displaying.
        /// </summary>
        private Queue<Dialog> dialogQueue = new Queue<Dialog>();

        [SerializeField]
        [Tooltip("This tag name is used to find the statemachine in a scene")]
        private string stateMachineTagName;

        [SerializeField]
        [Tooltip("The canvas containing the dialog canvas.")]
        private Canvas dialogCanvas;

        [SerializeField]
        private DialogSettings dialogSettings;

        [SerializeField]
        private ChoiceSettings choiceSettings;

        [Serializable]
        private struct DialogSettings
        {
            [Tooltip("Panel that contains dialog elements.")]
            public RectTransform dialogPanel;

            [Tooltip("The text box containing speech text.")]
            public Text textBox;

            [Tooltip("UI image panel used for displaying profile sprites.")]
            public Image profileImage;

            [Tooltip("Max amount of chars to display in textbox at a single time.")]
            [Range(1, 512)]
            public int charsPerBox;

            [Tooltip("Rate at which characters appear in the textbox")]
            [Range(1, 60)]
            public float charRate;

            public char pageDelimiter;

            public Image nextIndicator;
        }

        [Serializable]
        private struct ChoiceSettings
        {
            [Tooltip("Panel that contains choice elements.")]
            public RectTransform choicePanel;

            [Tooltip("Prefab to use when instantiating buttons for choice selection.")]
            public Button choiceButtonPrefab;

            [Tooltip("Initial position within the choicePanel that a choice button should start at")]
            public Vector2 initPosition;
        }

        [SerializeField]
        private string nextButtonName;

        /// <summary>
        /// When a choice is selected through the use of the DialogManager, 
        /// subscribers will be notified of the index of that choice, and the
        /// value that the choice contained.
        /// </summary>
        public event Action<int, object> ChoiceSelectedEvent;

        public event Action NewPageEvent;

        /// <summary>
        /// Allows access to the dialog panel's sprite profile image.
        /// </summary>
        public Sprite ProfileSprite
        {
            get
            {
                return dialogSettings.profileImage.sprite;
            }
            set
            {
                dialogSettings.profileImage.sprite = value;
            }
        }

        #endregion

        #region Initialization

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            dialogCanvas.enabled = false;
            IDialogState s = GameObject.FindGameObjectWithTag(stateMachineTagName).GetComponent<IDialogState>();
            if (s != null)
            {
                state = s;
            } else
            {
                Debug.LogError("DialogManager could not find a IDialogState component on the gameobject specified by the tag name: " + stateMachineTagName);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts a dialog interaction using dialog data in the specified file starting at the dialog with the given dialog ID.
        /// Cannot start a dialog if the manager is already running a dialog file.
        /// </summary>
        /// <param name="dialogFilePath">Path to the specially formatted json dialog file.</param>
        /// <param name="dialogId">Id of the dialog that the interaction should start at.</param>
        public void StartDialogAt(string dialogFilePath, int dialogId)
        {
            if (!RunningDisplayRoutine)
            {
                Dialog d = LoadDialogFile(dialogFilePath);
                Dialog search = DialogGraph.BreadthFirst(dialogQueue.Peek(), (n) => { return n.Id == dialogId; });
                if (search == null)
                {
                    Debug.LogError("Could not find Dialog with ID of: " + dialogId);
                }
                else
                {
                    StartCoroutine(ScheduleNextDialog(d));
                }
            }
        }

        /// <summary>
        /// Starts a dialog interaction using dialog data in the specified file starting at the root dialog object.
        /// Cannot start a dialog if the manager is already running a dialog file.
        /// </summary>
        /// <param name="dialogFilePath">Path to the specially formatted json dialog file.</param>
        public void StartDialog(string dialogFilePath)
        {
            if (!RunningDisplayRoutine)
            {
                Dialog d = LoadDialogFile(dialogFilePath);
                StartCoroutine(ScheduleNextDialog(d));
            }
        }

        #endregion

        #region Display Routines

        /// <summary>
        /// Waits until the dialog manager has finished the Display Dialog routine, then starts the specified dialog.
        /// </summary>
        /// <param name="d">Dialog to schedule</param>
        IEnumerator ScheduleNextDialog(Dialog d)
        {
            yield return new WaitWhile(() => { return RunningDisplayRoutine; });
            dialogQueue.Enqueue(d);
            StartCoroutine(DisplayDialog(dialogSettings, choiceSettings));
        }

        /// <summary>
        /// Main coroutine for managing the displaying of dialog.
        /// This method will not return until both the DisplayText routine and the DisplayChoices (if the dialog had choices) routine finish.
        /// </summary>
        /// <param name="d">The dialog data to display.</param>
        /// <param name="dialogSettings">Dialog settings to use for this routine.</param>
        /// <param name="choiceSettings">Choices settings to use for this routine.</param>
        IEnumerator DisplayDialog(DialogSettings dialogSettings, ChoiceSettings choiceSettings)
        {
            RunningDisplayRoutine = true;
            dialogCanvas.enabled = true;
            PlayerController c = FindObjectOfType<PlayerController>();
            if (c != null) c.ControlsEnabled = false;
            while (dialogQueue.Count > 0)
            {
                Dialog d = dialogQueue.Dequeue();
                Dialog failDialog;
                CheckConditions(d.Conditions, out failDialog);
                d = failDialog != null ? failDialog : d;
                yield return StartCoroutine(DisplayText(d.DialogText, dialogSettings));
                if (d.Choices != null && d.Choices.Count > 0)
                {
                    yield return StartCoroutine(DisplayChoices(d.Choices, choiceSettings));
                }
            }
            if (c != null) c.ControlsEnabled = true;
            dialogCanvas.enabled = false;
            RunningDisplayRoutine = false;
        }

        private bool CheckConditions(List<Condition> conditions, out Dialog failDialog)
        {
            foreach (Condition c in conditions)
            {
                if (state.GetFlag(c.Flag) != c.Value)
                {
                    failDialog = c.FailDialog;
                    return false;
                }
            }
            failDialog = null;
            return true;
        }


        /// <summary>
        /// Handles displaying the text of a dialog.
        /// If a page delimiter is reached, wait for the player to press the button specified by nextButtonName and afterwards, reset the text box.
        /// If the number of characters written to the text box is greater than the number of characters allowed in the text box, wait for player input and reset the box.
        /// </summary>
        /// <param name="text">Text to write to the text box.</param>
        /// <param name="dialogSettings">DialogSettings struct to use for controlling the display of the text.</param>
        IEnumerator DisplayText(string text, DialogSettings dialogSettings)
        {
            //Set the panel to active so the panel that contains the text and profile picture are visible.
            dialogSettings.dialogPanel.gameObject.SetActive(true);

            //Reset the text in the textbox.
            dialogSettings.textBox.text = "";

            //Split the text up based on the page delimiter.
            Queue<string> textQueue = new Queue<string>(text.Split(dialogSettings.pageDelimiter));

            int currentPage = 0;

            while (textQueue.Count != 0)
            {
                string page = textQueue.Dequeue();

                if(page.Length> dialogSettings.charsPerBox)
                {
                    Debug.LogError("There are too many characters in page " + currentPage + " set to be displayed. Consider splitting the text onto a new page.");
                }

                for(int i = 0; i < page.Length; i++)
                {
                    dialogSettings.nextIndicator.gameObject.SetActive(false);
                    if (Input.GetButton(nextButtonName))
                    {
                        dialogSettings.textBox.text = page;
                        yield return new WaitUntil(() => { return !Input.GetButton(nextButtonName); });
                        break;
                    } else
                    {
                        dialogSettings.textBox.text += page[i];
                    }
                    yield return new WaitForSeconds(1 / dialogSettings.charRate);
                }

                //Wait for player to press interact buton before continuing with next page.
                dialogSettings.nextIndicator.gameObject.SetActive(true);
                yield return StartCoroutine(WaitForPlayerInput(dialogSettings));
                currentPage++;
            }


            /*
            for (int i =0; i < text.Length; i++)
            {
                char c = text[i];
                if (c.Equals(dialogSettings.pageDelimiter))
                {
                    yield return StartCoroutine(WaitForPlayerInput(dialogSettings));
                }
                else
                {
                    if (i > dialogSettings.charsPerBox)
                    {
                        yield return StartCoroutine(WaitForPlayerInput(dialogSettings));
                    }
                    dialogSettings.textBox.text += c;
                    yield return new WaitForSeconds(1 / dialogSettings.charRate);
                }
            }
            */

            //dialogSettings.nextIndicator.gameObject.SetActive(true);
            //yield return new WaitUntil(() => Input.GetButtonDown(nextButtonName));
            dialogSettings.nextIndicator.gameObject.SetActive(false);
            dialogSettings.textBox.text = "";
            dialogSettings.dialogPanel.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
        }

        private IEnumerator WaitForPlayerInput(DialogSettings dialogSettings)
        {
            dialogSettings.nextIndicator.gameObject.SetActive(true);
            yield return new WaitUntil(() => Input.GetButtonDown(nextButtonName));
            dialogSettings.nextIndicator.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
            dialogSettings.textBox.text = "";
            if (NewPageEvent != null)
            {
                NewPageEvent();
            }
            yield return new WaitUntil(() => { return !Input.GetButton(nextButtonName); });
        }

        /// <summary>
        /// Handles displaying the choices contained in a dialog object.
        /// Once the choices are displayed, waits for the player to click on one, then schedules the nextDialog Dialog object contained in that choice to run.
        /// When the player selects a choice, a ChoiceSelectedEvent will fire and pass the index of that choice and the value that the choice contained.
        /// </summary>
        /// <param name="choices">List of Choice objects contained in the dialog object used in DisplayDialog.</param>
        /// <param name="choiceSettings">Choice settings for controlling the placement/displaying of the choices.</param>
        IEnumerator DisplayChoices(List<Choice> choices, ChoiceSettings choiceSettings)
        {
            choiceSettings.choicePanel.gameObject.SetActive(true);
            Vector2 buttonPosition = choiceSettings.initPosition;
            bool choiceSelected = false;
            List<Button> buttons = new List<Button>();
            for (int i = 0; i < choices.Count; i++)
            {
                Choice choice = choices[i];
                //Inefficient, should be object pooling at creation time of DialogManager, oh well.
                Button b = Instantiate(choiceSettings.choiceButtonPrefab, choiceSettings.choicePanel.transform, false);
                b.GetComponentInChildren<Text>().text = choice.Text;
                b.GetComponent<RectTransform>().localPosition = buttonPosition;
                buttons.Add(b);
                int j = i;

                if (NewPageEvent != null) {
                    NewPageEvent();
                }

                b.onClick.AddListener(() => {
                    choiceSelected = true;
                    if (ChoiceSelectedEvent != null)
                    {
                        ChoiceSelectedEvent(j, choice.Value);
                    }
                    ChoiceSelectionListener(choice);
                });
                Rect r = b.GetComponent<RectTransform>().rect;
                buttonPosition = new Vector2(0, buttonPosition.y - r.height);
            }
            yield return new WaitUntil(() => { return choiceSelected; });
            foreach (Button b in buttons)
            {
                DestroyImmediate(b.gameObject);
            }
            choiceSettings.choicePanel.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// Helper method to schedule the nextDialog from the choice chosen in DisplayChoices routine.
        /// </summary>
        /// <param name="c">Selected choice.</param>
        private void ChoiceSelectionListener(Choice c)
        {
            Dialog d = c.NextDialog;
            string flag = c.Flag;
            int value = c.Value;
            state.SetFlag(flag, value);
            if (d != null)
            {
                dialogQueue.Enqueue(d);
            }
        }

        #endregion

        #region Data Loading

        /// <summary>
        /// Reads in a dialog file and returns a root Dialog Node.
        /// </summary>
        static Dialog LoadDialogFile(string path)
        {
            TextAsset json = Resources.Load<TextAsset>(path);
            List<JsonDialog> data = JsonConvert.DeserializeObject<List<JsonDialog>>(json.text);
            List<Dialog> dialogs = ConvertJsonDialog(data);
            return dialogs[0];
        }

        /// <summary>
        /// Converts array-formatted json dialog data to a graph.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static List<Dialog> ConvertJsonDialog(List<JsonDialog> data)
        {
            List<Dialog> dialogs = new List<Dialog>();
            List<Choice> choices = new List<Choice>();
            foreach (JsonDialog item in data)
            {
                dialogs.Add(new Dialog(item.Id, item.DialogText));
            }
            for (int i = 0; i < data.Count; i++)
            {
                JsonDialog item = data[i];
                foreach (JsonChoice choice in item.Choices)
                {
                    dialogs[i].Choices.Add(new Choice(choice.Text, choice.Flag, choice.Value, dialogs[choice.NextDialog]));
                }
                foreach (JsonCondition condition in item.Conditions)
                {
                    dialogs[i].Conditions.Add(new Condition(condition.Flag, condition.Value, dialogs[condition.FailDialog]));
                }
            }
            return dialogs;
        }

        static void SaveDialogToFile(string path, Dialog data)
        {
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
        }

        #endregion

        #region Data Types

        /// <summary>
        /// Contains utility methods for searching through a Dialog graph.
        /// </summary>
        internal static class DialogGraph
        {
            /// <summary>
            /// Performs a depth-first search on the Dialog object using the comparer. 
            /// If the comparer returns true, that i the object that will be returned.
            /// </summary>
            /// <param name="root">Node to start searching from.</param>
            /// <param name="comparer">Func that takes in a Dialog object, and returns either true or false.</param>
            /// <returns>Object that caused comparer to return true, otherwise null.</returns>
            public static Dialog DepthFirst(Dialog root, Func<Dialog, bool> comparer)
            {
                Stack<Dialog> stack = new Stack<Dialog>();
                HashSet<Dialog> visited = new HashSet<Dialog>();

                stack.Push(root);
                while (root != null && stack.Count > 0)
                {
                    root = stack.Pop();
                    if (root != null && !visited.Contains(root))
                    {
                        if (comparer(root))
                        {
                            return root;
                        }
                        visited.Add(root);
                        foreach (Choice c in root.Choices)
                        {
                            stack.Push(c.NextDialog);
                        }
                    }
                }
                return null;
            }

            /// <summary>
            /// Performs a breadth-first search on the Dialog object using the comparer. 
            /// If the comparer returns true, that i the object that will be returned.
            /// </summary>
            /// <param name="root">Node to start searching from.</param>
            /// <param name="comparer">Func that takes in a Dialog object, and returns either true or false.</param>
            /// <returns>Object that caused comparer to return true, otherwise null.</returns>
            public static Dialog BreadthFirst(Dialog root, Func<Dialog, bool> comparer)
            {
                Queue<Dialog> queue = new Queue<Dialog>();
                HashSet<Dialog> visited = new HashSet<Dialog>();

                queue.Enqueue(root);
                while (root != null && queue.Count > 0)
                {
                    root = queue.Dequeue();
                    if (root != null && !visited.Contains(root))
                    {
                        if (comparer(root))
                        {
                            return root;
                        }
                        visited.Add(root);
                        foreach (Choice c in root.Choices)
                        {
                            queue.Enqueue(c.NextDialog);
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Because I'm lazy and didn't do inheritance. This is the type of dialog object returned by reading the dialog json file.
        /// </summary>
        internal sealed class JsonDialog
        {
            public int Id { get; set; }
            public string DialogText { get; set; }
            public List<JsonChoice> Choices { get; private set; }
            public List<JsonCondition> Conditions { get; private set; }

            public JsonDialog(int id, string text, JsonChoice[] choices, JsonCondition[] conditions)
            {
                this.Id = id;
                this.DialogText = text;
                this.Choices = new List<JsonChoice>(choices);
                this.Conditions = new List<JsonCondition>(conditions);

            }

            public int CompareTo(int other)
            {
                return Id.CompareTo(other);
            }
        }

        /// <summary>
        /// Because I'm lazy and didn't do inheritance. This is the type of choice object returned by reading the dialog json file.
        /// </summary>
        internal sealed class JsonChoice
        {
            public string Text { get; set; }
            public string Flag { get; set; }
            public int Value { get; set; }
            public int NextDialog { get; set; }

            public JsonChoice(string text, string flag, int value, int nextDialog)
            {
                this.Text = text;
                this.Flag = flag;
                this.Value = value;
                this.NextDialog = nextDialog;
            }
        }

        internal sealed class JsonCondition
        {
            public string Flag { get; set; }
            public int Value { get; set; }
            public int FailDialog { get; set; }
        }

        /// <summary>
        /// Type of Dialog object that this manager uses for controlling a dialog interactions.
        /// </summary>
        internal sealed class Dialog : IComparable<int>
        {
            /// <summary>
            /// Id (aka index from the json file) of the Dialog.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Text of the dialog.
            /// </summary>
            public string DialogText { get; set; }

            /// <summary>
            /// List of choices for the dialog (by default, the json dialogs always have choices, but its just an empty array).
            /// </summary>
            public List<Choice> Choices { get; private set; }

            public List<Condition> Conditions { get; private set; }

            public Dialog(int id, string text) : this(id, text, new Choice[0], new Condition[0]) { }

            public Dialog(int id, string text, Choice[] choices, Condition[] conditions)
            {
                this.Id = id;
                this.DialogText = text;
                this.Choices = new List<Choice>(choices);
                this.Conditions = new List<Condition>(conditions);
            }

            public int CompareTo(int other)
            {
                return Id.CompareTo(other);
            }
        }

        internal sealed class Choice
        {
            /// <summary>
            /// Prompt text for the choice, displays on choice buttons.
            /// </summary>
            public string Text { get; set; }

            public string Flag { get; set; }

            /// <summary>
            /// Value of the Choice. Can be any primitive object.
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// Dialog to display when this choice is chosen.
            /// </summary>
            public Dialog NextDialog { get; set; }

            public Choice(string text, string flag, int value, Dialog nextDialog)
            {
                this.Text = text;
                this.Flag = flag;
                this.Value = value;
                this.NextDialog = nextDialog;
            }
        }

        internal sealed class Condition
        {
            public string Flag { get; set; }
            public int Value { get; set; }
            public Dialog FailDialog { get; set; }

            public Condition(string flag, int value, Dialog failDialog)
            {
                this.Flag = flag;
                this.Value = value;
                this.FailDialog = failDialog;
            }
        }

        #endregion
    }

    public interface IDialogState {

        void SetFlag(string flagName, int flagValue);

        int GetFlag(string flagName);

    }
}
