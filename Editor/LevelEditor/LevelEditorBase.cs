using System.Reflection;
using UnityEngine;
using System;
using UnityEditor;
using VahTyah.LevelEditor;

namespace VahTyah.LevelEditor
{
    public abstract class LevelEditorBase : EditorWindow, IHasCustomMenu
    {
        public static EditorWindow Window;
        public static LevelEditorBase Instance;

        protected ResizableSeparator ResizableSidebar;
        protected LevelsHandlerBase LevelHandler;

        private const string LEVEL_EDITOR_SCENE_PATH = "Assets/_Game/LevelEditor/Editor/Scene/LevelEditor.unity";
        private const string LEVEL_EDITOR_SCENE_NAME = "LevelEditor";

        [MenuItem("Tools/Level Editor")]
        static void ShowWindow()
        {
            Type childType = GetChildType();
            Window = GetWindow(childType);
            Window.titleContent = new GUIContent("Level Editor");
            Window.minSize = new Vector2(200, 200);
            Window.Show();
        }

        [MenuItem("Tools/Level Editor", true)]
        static bool ValidateMenuItem()
        {
            Type childType = GetChildType();
            return (childType != null);
        }

        static Type GetChildType()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type classType in assembly.GetTypes())
                {
                    if (classType.IsSubclassOf(typeof(LevelEditorBase)))
                    {
                        return classType;
                    }
                }
            }

            return null;
        }

        protected virtual void OnEnable()
        {
            LevelHandler = GetLevelHandler;
            Instance = this;
            ResizableSidebar = new ResizableSeparator("editor_sidebar_width", 240);
        }

        protected virtual void OnDisable()
        {
        }

        protected abstract LevelsHandlerBase GetLevelHandler { get; }

        protected virtual void OnGUI()
        {
            if (!LevelEditorUtils.IsInScene(LEVEL_EDITOR_SCENE_NAME))
            {
                DrawSceneRequiredMessage();
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(ResizableSidebar.CurrentWidth));
            LevelHandler.DisplayReordableList();
            LevelHandler.DrawToolbar();
            EditorGUILayout.EndVertical();

            ResizableSidebar.DrawResizeSeparator();

            EditorGUILayout.BeginVertical();
            DrawContent();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawContent()
        {
        }


        private void DrawSceneRequiredMessage()
        {
            EditorGUILayout.Space(20);

            EditorGUILayout.HelpBox(
                "Level Editor requires the LevelEditor scene to be open.\n\n" +
                "Please open the LevelEditor scene to use this tool.",
                MessageType.Warning
            );

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Open LevelEditor Scene", GUILayout.Height(30)))
            {
                LevelsHandlerBase.IsLastLevelOpened = false;
                LevelEditorUtils.OpenScene(LEVEL_EDITOR_SCENE_PATH);
            }
        }

        public virtual void DisplaySettings()
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Levels Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            LevelHandler.CustomList.Settings.Display();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Settings"), false, OpenSettings);
        }

        private void OpenSettings()
        {
            LevelEditorSettingsWindow.ShowWindow();
        }
    }
}

public class LevelEditorSettingsWindow : EditorWindow
{
    private const string WINDOW_TITLE = "Level Editor Features";

    private Vector2 scrollPos;

    public static void ShowWindow()
    {
        var window = GetWindow<LevelEditorSettingsWindow>(WINDOW_TITLE);
        window.minSize = new Vector2(350, 450);
        window.maxSize = new Vector2(350, 600);
        window.Show();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        LevelEditorBase.Instance.DisplaySettings();
        EditorGUILayout.EndScrollView();
    }
}