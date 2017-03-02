using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Xml;
using System.IO;

namespace Somnium
{
    public class DialogEditor : EditorWindow
    {
        List<NodeWindow> windows = new List<NodeWindow>();

        /// <summary>
        /// Register a delegate to tell the view how to add a window.
        /// </summary>
        Func<NodeWindow> CreateDelegate { get; set; }

        [MenuItem("Window/Dialog Editor")]
        static void ShowEditor()
        {
            DialogEditor editor = GetWindow<DialogEditor>();
        }

        void OnGUI()
        {
            BeginWindows();

            if (GUILayout.Button("Create Node"))
            {
                if (CreateDelegate != null)
                {
                    windows.Add(CreateDelegate());
                }
            }

            for (int i = 0; i < windows.Count; i++)
            {
                NodeWindow window = windows[i];
                window.Rectangle = GUI.Window(i, window.Rectangle, window.DrawNodeWindow, window.Title);
            }

            EndWindows();
        }

        void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        }

        
    }

    internal sealed class EditorModel
    {
        DialogManager.Dialog rootDialog;


    }

    internal sealed class EditorController
    {

    }

    public abstract class NodeWindow
    {
        private Rect rectangle;
        private string title;

        public NodeWindow(string title, Rect rect)
        {
            this.title = title;
            this.rectangle = rect;
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public Rect Rectangle
        {
            get
            {
                return rectangle;
            }
            set
            {
                this.rectangle = value;
            }
        }

        public abstract void DrawNodeWindow(int id);

        public static void CreateScrollingTextArea(Rect areaRect, ref string text, ref Vector2 scrollPosition)
        {
            //Text Area
            GUILayout.BeginArea(areaRect);
            //GUILayout.BeginArea(new Rect(0, 15, Rectangle.width - 5, Rectangle.height / 2));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(areaRect.height), GUILayout.ExpandHeight(false));
            //GUI.skin.box.wordWrap = true;
            text = GUILayout.TextArea(text, GUILayout.Width(areaRect.width - 5), GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }

    internal sealed class DialogNode : NodeWindow
    {
        Vector2 scrollPosition = Vector2.zero;
        string dialogText = "Enter text here.";

        public DialogNode(string title, Rect rect) : base(title, rect)
        {
        }

        public override void DrawNodeWindow(int id)
        {
            NodeWindow.CreateScrollingTextArea(new Rect(0, 15, Rectangle.width - 5, Rectangle.height / 2), ref dialogText, ref scrollPosition);

            GUI.DragWindow();
        }
    }
}