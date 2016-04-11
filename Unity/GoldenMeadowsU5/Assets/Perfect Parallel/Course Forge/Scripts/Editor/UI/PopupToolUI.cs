using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge.UI
{
	/// <summary>
	/// Course Forge file tool, tool to provide with help information
	/// </summary>
	public class PopupToolUI : ToolUI
    {
        #region Fields
        static List<string> messages = new List<string>();
        static List<string> actions = new List<string>();
        static List<string> values = new List<string>();
        #endregion

        #region External Methods
        /// <summary>
        /// Clear the messages
        /// </summary>
        public static void Clear()
        {
            messages.Clear();
            actions.Clear();
            values.Clear();
        }
        /// <summary>
        /// Add message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="action"></param>
        /// <param name="value"></param>
        public static void Add(string message, string action = "ok", string value = null)
        {
            messages.Add(message);
            actions.Add(action);
            values.Add(value);
        }
        /// <summary>
        /// Count of the messages
        /// </summary>
        public static int Count()
        {
            return messages.Count;
        }
        #endregion

        #region Methods
        public override void OnUI(bool selected)
		{
            if (selected == false) return;

            BeginUI();
            MoveToRight();
            Move(-5, 3);
            MovePixels(-3, 0);

            for (int i = 0; i < messages.Count; ++i)
            {
                float textHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(messages[i]), 5 * 48.0f) / 48.0f + 0.25f;
                float buttonHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(actions[i]), 5 * 48.0f - 12) / 48.0f + 0.25f;
                Background(5, textHeight + buttonHeight);

                if (values[i] == null) HelpBox(messages[i], MessageType.Warning, 5, textHeight + buttonHeight);
                else if (values[i] == "info") HelpBox(messages[i], MessageType.Info, 5, textHeight + buttonHeight);
                else HelpBox(messages[i], MessageType.Error, 5, textHeight + buttonHeight);

                Move(0, textHeight);

                if (Button(actions[i], 5, buttonHeight))
                {
                    if (actions[i] == "Focus")
                    {
                        GameObject gameObject = GameObject.Find(values[i]);
                        if (gameObject != null)
                        {
                            EditorGUIUtility.PingObject(gameObject);
                            Selection.activeGameObject = gameObject;

                            SceneView.lastActiveSceneView.FrameSelected();
                        }
                    }

                    messages.RemoveAt(i);
                    actions.RemoveAt(i);
                    values.RemoveAt(i);
                    break;
                }
                Move(0, buttonHeight);

                MovePixels(0, 1);
            }

            EndUI();
        }
		#endregion
	}
}
