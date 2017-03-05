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
        /// The instance of the dialog manager, singleton pattern.
        /// </summary>
        private static DialogManager instance;

        //private Dialog currentDialog;

        /// <summary>
        /// Whether or not the manager is currently running a DisplayDialog routine.
        /// Used for handling the scheduling of displaying other dialogs.
        /// </summary>
        private bool runningDisplayRoutine;

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
            [Range(1,512)]
            public int charsPerBox;

            [Tooltip("Rate at which characters appear in the textbox")]
            [Range(1,60)]
            public float charRate;
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

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            dialogCanvas.enabled = false;
        }

        private void Start()
        {
            StartDialog("Assets/Resources/DialogFiles/TestDialog.json");
            //StartDialogAt("Assets/Resources/DialogFiles/TestDialog.json", 2);
        }

        /// <summary>
        /// Starts a dialog interaction using dialog data in the specified file starting at the dialog with the given dialog ID.
        /// </summary>
        /// <param name="dialogFilePath">Path to the specially formatted json dialog file.</param>
        /// <param name="dialogId">Id of the dialog that the interaction should start at.</param>
        public void StartDialogAt(string dialogFilePath, int dialogId)
        {
            Dialog d = LoadDialogFile(dialogFilePath);
            Dialog search = DialogGraph.BreadthFirst(d, (n) => { return n.Id == dialogId; });
            if (search == null)
            {
                Debug.LogError("Could not find Dialog with ID of: " + dialogId);
            } else
            {
                StartCoroutine(ScheduleNextDialog(search));
            }
        }

        /// <summary>
        /// Starts a dialog interaction using dialog data in the specified file starting at the root dialog object.
        /// </summary>
        /// <param name="dialogFilePath">Path to the specially formatted json dialog file.</param>
        public void StartDialog(string dialogFilePath)
        {
            Dialog d = LoadDialogFile(dialogFilePath);
            StartCoroutine(ScheduleNextDialog(d));
        }

        private void StartDialog(Dialog dialog)
        {
            StartCoroutine(DisplayDialog(dialog, dialogSettings, choiceSettings));
        }

        IEnumerator DisplayDialog(Dialog d, DialogSettings dialogSettings, ChoiceSettings choiceSettings)
        {
            runningDisplayRoutine = true;
            dialogCanvas.enabled = true;
            yield return StartCoroutine(DisplayText(d.DialogText, dialogSettings));
            if (d.Choices != null && d.Choices.Count > 0)
            {
                yield return StartCoroutine(DisplayChoices(d.Choices, choiceSettings));
            } 
            dialogCanvas.enabled = false;
            runningDisplayRoutine = false;
        }

        IEnumerator DisplayText(string text, DialogSettings dialogSettings)
        {
            dialogSettings.dialogPanel.gameObject.SetActive(true);
            int i = 0;
            dialogSettings.textBox.text = "";
            foreach(char c in text)
            {
                if (i > dialogSettings.charsPerBox)
                {
                    yield return new WaitUntil(() => Input.GetButtonDown(nextButtonName));
                    dialogSettings.textBox.text = "";
                    i = 0;
                }
                dialogSettings.textBox.text += c;
                i++;
                yield return new WaitForSeconds(1/dialogSettings.charRate);
            }
            yield return new WaitUntil(() => Input.GetButtonDown(nextButtonName));
            dialogSettings.dialogPanel.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
        }

        IEnumerator DisplayChoices(List<Choice> choices, ChoiceSettings choiceSettings)
        {
            choiceSettings.choicePanel.gameObject.SetActive(true);
            Vector2 buttonPosition = choiceSettings.initPosition;
            bool choiceSelected = false;
            for(int i = 0; i < choices.Count; i++)
            {
                Choice c = choices[i];
                //Inefficient, should be object pooling at creation time of DialogManager, oh well.
                Button b = Instantiate(choiceSettings.choiceButtonPrefab, choiceSettings.choicePanel.transform, false);
                b.GetComponentInChildren<Text>().text = c.Text;
                b.GetComponent<RectTransform>().localPosition = buttonPosition;
                int j = i;
                Choice choice = choices[i];
                b.onClick.AddListener(()=> {
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
            choiceSettings.choicePanel.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
        }

        private void ChoiceSelectionListener(Choice c)
        {
            Dialog d = c.NextDialog;
            if (d != null)
            {
                StartCoroutine(ScheduleNextDialog(d));
            }
        }

        IEnumerator ScheduleNextDialog(Dialog d)
        {
            yield return new WaitWhile(() => { return runningDisplayRoutine; });
            StartDialog(d);
        }

        /// <summary>
        /// Reads in a dialog file and returns a root Dialog Node.
        /// </summary>
        static Dialog LoadDialogFile(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Dialog root = JsonConvert.DeserializeObject<Dialog>(json);
                return root;
            }
            else
            {
                Debug.LogError("File at path: " + path + " does not exist.");
                return null;
            }
        }

        static void SaveDialogToFile(string path, Dialog data)
        {
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
        }

        sealed class DialogGraph
        {
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

        sealed class Dialog : IComparable<int>
        {
            public int Id { get; set; }
            public string DialogText { get; set; }

            public List<Choice> Choices { get; private set; }

            public Dialog(int id, string text, Choice[] choices)
            {
                this.Id = id;
                this.DialogText = text;
                this.Choices = new List<Choice>(choices);
            }

            public int CompareTo(int other)
            {
                return Id.CompareTo(other);
            }
        }

        sealed class Choice
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public Dialog NextDialog { get; set; }

            public Choice(string text, object value, Dialog nextDialog)
            {
                this.Text = text;
                this.Value = value;
                this.NextDialog = nextDialog;
            }
        }
    }
}
