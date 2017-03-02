using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using Newtonsoft.Json;

namespace Somnium
{
    public class DialogManager : MonoBehaviour
    {
        private static DialogManager instance;

        [SerializeField]
        [Tooltip("The canvas containing the dialog canvas.")]
        private Canvas dialogCanvas;

        [SerializeField]
        [Tooltip("The profile image of the character speaking.")]
        private Image profileImage;

        [SerializeField]
        [Tooltip("The text box containing speech text.")]
        private Text textBox;

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
        }

        /// <summary>
        /// Reads in a dialog file and returns a root Dialog Node.
        /// </summary>
        public static Dialog LoadDialog(string path)
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

        public static void SaveDialog(string path, Dialog data)
        {
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
        }

        public sealed class DialogGraph
        {

            public Dialog DepthFirst(Dialog root, Func<Dialog, bool> comparer)
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

            public Dialog BreadthFirst(Dialog root, Func<Dialog, bool> comparer)
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

        public sealed class Dialog : IComparable<int>
        {
            public int Id { get; set; }
            public string DialogText { get; set; }
            public string DialogType { get; set; }

            public List<Choice> Choices { get; private set; }

            public Dialog(string text, string type) : this(text, type, new Choice[0])
            {

            }

            public Dialog(string text, string type, Choice[] choices)
            {
                this.DialogText = text;
                this.DialogType = type;
                this.Choices = new List<Choice>(choices);
            }

            public int CompareTo(int other)
            {
                return Id.CompareTo(other);
            }
        }

        public sealed class Choice
        {
            public Dialog Parent { get; private set; }
            public string Text { get; set; }
            public object Value { get; set; }
            public Dialog NextDialog { get; set; }

            public Choice(string text, object value) : this(text, value, null)
            {

            }

            public Choice(string text, object value, Dialog nextDialog)
            {
                this.Text = text;
                this.Value = value;
                this.NextDialog = nextDialog;
            }
        }
    }
}
