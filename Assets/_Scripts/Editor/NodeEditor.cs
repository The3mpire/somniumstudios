using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Somnium
{
    public class NodeEditor : EditorWindow
    {
        private List<Node> nodes = new List<Node>();

        [MenuItem("Window/Basic Node Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(NodeEditor));
        }

        protected virtual void OnGUI()
        {
            DrawNodes();

            if (GUILayout.Button("Create Node"))
            {
                nodes.Add(new Node());
            }
        }

        


        void DrawNodes()
        {
            testRect = new Rect(5, 5, position.width - 140, position.height - 5);
            GUILayout.BeginVertical();
            {
                GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(testRect.height), GUILayout.MaxWidth(position.width - 140));
                EditorZoomArea.Begin(zoom, new Rect(testRect.x + 5, testRect.y, position.width - 150, testRect.height - 10));
                BeginWindows();
                {
                    

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        Node temp = nodes[i];
                        temp.pos = GUILayout.Window(i, new Rect(temp.pos.x + zoomDelta.x, temp.pos.y + zoomDelta.y, temp.pos.width, temp.pos.height), temp.DrawNode, "Window");

                    }
                    //HandleRightClickEvent();
                    HandleEvents();
                }
                EndWindows();
                EditorZoomArea.End();
            }
            GUILayout.EndVertical();
        }

        Vector2 zoomOrigin = Vector2.zero;
        Vector2 zoomDelta = Vector2.zero;
        Rect testRect = new Rect(5, 5, 400, 680);
        float zoom = 1;

        void HandleEvents()
        {
            Vector2 mousePos = Event.current.mousePosition;

            if(Event.current.type == EventType.ScrollWheel)
            {
                Vector2 delta = Event.current.delta;
                Vector2 zoomedMousePos = (mousePos - testRect.min) / zoom + zoomOrigin;

                float oldZoomScale = zoom;

                float zoomDelta = -delta.y / 150f;
                zoom += zoomDelta * 4;
                zoom = Mathf.Clamp(zoom, 0.1f, 2.0f);

                zoomOrigin += (zoomedMousePos - zoomOrigin) - (oldZoomScale / zoom) * (zoomedMousePos - zoomOrigin);

                Event.current.Use();
            }
        }
    }

    public class Node:ScriptableObject
    {
        public int ID { get; set; }
        public Rect pos { get; set; }
        public Node Parent { get; set; }
        public Node Child { get; set; }
        public List<Node> children{ get; private set; }

        public Node()
        {
            pos = new Rect(100, 100, 120, 120);
            children = new List<Node>();
        }

        public virtual void DrawNode(int id = 0)
        {
            ID = id;
            if (GUILayout.Button("Attach"))
            {

            }
            if (GUILayout.Button("Detach"))
            {
            }
            if (GUILayout.Button("Delete"))
            {

            }
            GUI.DragWindow();
        }

        //actually there's probably no reason to make this a virtual void
        //still leaving it like that just in case you want to change something depending on the node type
        public virtual void DrawNodeCurve(Node node1, Node node2)
        {
            /*
            Vector3 start = node1.Parent(node2);
            Vector3 end = node2.Left(node1);
            Vector3 startTan = start + Vector3.right * 50;
            Vector3 endTan = end + Vector3.left * 50;
            Handles.DrawBezier(start, end, startTan, endTan, Color.black, null, 3);
            */
        }
    }

    public class NodeUtil
    {
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

    /// <summary>
    /// A small class designed to do some simple math on Unity Rects
    /// Code based off of article found at:
    /// http://martinecker.com/martincodes/unity-editor-window-zooming/
    /// 
    /// (Site may be down)
    /// </summary>
    public static class RectExtensions
    {
        /// <summary>
        /// Scales a rect by a given amount around its center point
        /// </summary>
        /// <param name="rect">The given rect</param>
        /// <param name="scale">The scale factor</param>
        /// <returns>The given rect scaled around its center</returns>
        public static Rect ScaleSizeBy(this Rect rect, float scale) { return rect.ScaleSizeBy(scale, rect.center); }

        /// <summary>
        /// Scales a rect by a given amount and around a given point
        /// </summary>
        /// <param name="rect">The rect to size</param>
        /// <param name="scale">The scale factor</param>
        /// <param name="pivotPoint">The point to scale around</param>
        /// <returns>The rect, scaled around the given pivot point</returns>
        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;

            //"translate" the top left to something like an origin
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;

            //Scale the rect
            result.xMin *= scale;
            result.yMin *= scale;
            result.xMax *= scale;
            result.yMax *= scale;

            //"translate" the top left back to its original position
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;

            return result;
        }
    }

    /// <summary>
    /// A simple class providing static access to functions that will provide a 
    /// zoomable area similar to Unity's built in BeginVertical and BeginArea
    /// Systems. Code based off of article found at:
    /// http://martinecker.com/martincodes/unity-editor-window-zooming/
    ///  
    /// (Site may be down)
    /// </summary>
    public class EditorZoomArea
    {
        private static Matrix4x4 prevMatrix;

        public static Rect Begin(float zoomScale, Rect screenCoordsArea)
        {
            GUI.EndGroup(); //End the group that Unity began so we're not bound by the EditorWindow

            Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, screenCoordsArea.center);
            clippedArea.y += 21; //Account for the window tab

            GUI.BeginGroup(clippedArea);

            prevMatrix = GUI.matrix;

            //Perform scaling
            Matrix4x4 translation = Matrix4x4.TRS(clippedArea.center, Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
            GUI.matrix = translation * scale * translation.inverse;

            return clippedArea;
        }

        /// <summary>
        /// Ends the zoom area
        /// </summary>
        public static void End()
        {
            GUI.matrix = prevMatrix;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0, 21, Screen.width, Screen.height));
        }
    }
}
