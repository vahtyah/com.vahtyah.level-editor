using UnityEngine;
using UnityEditor;

namespace VahTyah
{
    [CustomEditor(typeof(LevelEditorStylesDatabase))]
    public class LevelEditorStylesDatabaseEditor : Editor
    {
        private static class Styles
        {
            public static readonly float ButtonHeight = 24f;
            public static readonly float ButtonSpacing = 4f;
            public static readonly float ButtonPadding = 8f;
            
            public static readonly Color ButtonNormalColor = new Color(0.25f, 0.25f, 0.25f, 1f);
            public static readonly Color ButtonHoverColor = new Color(0.35f, 0.35f, 0.35f, 1f);
            public static readonly Color ButtonActiveColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            public static readonly Color ButtonBorderColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            private static GUIStyle _buttonStyle;
            public static GUIStyle ButtonStyle
            {
                get
                {
                    if (_buttonStyle == null)
                    {
                        _buttonStyle = new GUIStyle(GUI.skin.button)
                        {
                            fontStyle = FontStyle.Bold,
                            alignment = TextAnchor.MiddleCenter,
                            fixedHeight = ButtonHeight
                        };
                    }
                    return _buttonStyle;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            // Draw default inspector
            DrawDefaultInspector();
            
            EditorGUILayout.Space(Styles.ButtonSpacing);
            
            // Draw styled button
            DrawButton("Add Default Style", () =>
            {
                var database = (LevelEditorStylesDatabase)target;
                Undo.RecordObject(database, "Add Default Style");
                database.AddDefaultStyle();
                EditorUtility.SetDirty(database);
            });
        }

        private void DrawButton(string label, System.Action onClick)
        {
            Rect buttonRect = GUILayoutUtility.GetRect(0, Styles.ButtonHeight, GUILayout.ExpandWidth(true));
            buttonRect.x += Styles.ButtonPadding;
            buttonRect.width -= Styles.ButtonPadding * 2;

            Event currentEvent = Event.current;
            bool isHovered = buttonRect.Contains(currentEvent.mousePosition);
            bool isMouseDown = currentEvent.type == EventType.MouseDown && currentEvent.button == 0;
            bool isMouseUp = currentEvent.type == EventType.MouseUp && currentEvent.button == 0;

            // Draw button background
            if (currentEvent.type == EventType.Repaint)
            {
                Color bgColor = isHovered ? 
                    (GUIUtility.hotControl == GUIUtility.GetControlID(FocusType.Passive) ? Styles.ButtonActiveColor : Styles.ButtonHoverColor) 
                    : Styles.ButtonNormalColor;
                
                DrawRoundedRect(buttonRect, bgColor, Styles.ButtonBorderColor, 4f);
                
                // Draw label
                GUI.Label(buttonRect, label, Styles.ButtonStyle);
            }

            // Handle click
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            
            if (isHovered && isMouseDown)
            {
                GUIUtility.hotControl = controlId;
                currentEvent.Use();
            }
            
            if (GUIUtility.hotControl == controlId && isMouseUp)
            {
                GUIUtility.hotControl = 0;
                if (isHovered)
                {
                    onClick?.Invoke();
                    GUI.changed = true;
                }
                currentEvent.Use();
            }

            // Repaint on hover state change
            if (isHovered)
            {
                EditorWindow.focusedWindow?.Repaint();
            }
        }

        private void DrawRoundedRect(Rect rect, Color bgColor, Color borderColor, float radius)
        {
            // Background
            GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, true, 0, bgColor, Vector4.one * 100, radius);
            
            // Border
            GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, true, 0, borderColor, Vector4.one, radius);
        }
    }
}
