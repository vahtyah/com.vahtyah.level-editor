using System.Reflection;
using UnityEngine;
using System;
using UnityEditor;
using VahTyah.LevelEditor;
using Object = UnityEngine.Object;

namespace VahTyah.LevelEditor
{
    public abstract class LevelEditorBase : EditorWindow, IHasCustomMenu
    {
        private const string DEFAULT_LEVEL_EDITOR_SCENE_PATH = "Assets/LevelEditor/Editor/Scenes/LevelEditor.unity";
        private const string DEFAULT_LEVEL_EDITOR_SCENE_NAME = "LevelEditor";

        public static EditorWindow Window;
        public static LevelEditorBase Instance;

        protected ResizableSeparator ResizableSidebar;
        protected LevelsHandlerBase LevelHandler;
        protected LevelEditorBaseSettings BaseSettings;

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
            EnsureBaseSettings();
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
            EnsureBaseSettings();

            if (!LevelEditorUtils.IsInScene(BaseSettings.LevelEditorSceneName))
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
                LevelEditorUtils.OpenScene(BaseSettings.LevelEditorScenePath);
            }
        }

        public virtual void DisplaySettings()
        {
            EnsureBaseSettings();

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Levels Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);

            BaseSettings.Display();
            GUILayout.Space(10);

            EditorGUILayout.LabelField("CustomList Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);
            LevelHandler.CustomList.Settings.Display();
        }

        private void EnsureBaseSettings()
        {
            if (BaseSettings == null)
            {
                BaseSettings = new LevelEditorBaseSettings(
                    DEFAULT_LEVEL_EDITOR_SCENE_PATH,
                    DEFAULT_LEVEL_EDITOR_SCENE_NAME
                );
            }
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Settings"), false, OpenSettings);
        }

        private void OpenSettings()
        {
            LevelEditorSettingsWindow.ShowWindow();
        }

        public virtual void OpenLevel(Object levelObject, int index)
        {
            
        }
    }

    public class LevelEditorBaseSettings
    {
        private const string PREFS_SCENE_PATH = "VahTyah.LevelEditor.ScenePath";
        private const string PREFS_SCENE_NAME = "VahTyah.LevelEditor.SceneName";

        private readonly string defaultScenePath;
        private readonly string defaultSceneName;

        public string LevelEditorScenePath { get; private set; }
        public string LevelEditorSceneName { get; private set; }

        public LevelEditorBaseSettings(string defaultScenePath, string defaultSceneName)
        {
            this.defaultScenePath = defaultScenePath;
            this.defaultSceneName = defaultSceneName;
            Load();
        }

        public void Display()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Scene Settings", EditorStyles.miniBoldLabel);
            string scenePath = EditorGUILayout.TextField("Scene Path", LevelEditorScenePath);
            string sceneName = EditorGUILayout.TextField("Scene Name", LevelEditorSceneName);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset Scene Defaults", GUILayout.Width(150)))
            {
                LevelEditorScenePath = defaultScenePath;
                LevelEditorSceneName = defaultSceneName;
                Save();
            }

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                LevelEditorScenePath = string.IsNullOrWhiteSpace(scenePath) ? defaultScenePath : scenePath.Trim();
                LevelEditorSceneName = string.IsNullOrWhiteSpace(sceneName) ? defaultSceneName : sceneName.Trim();
                Save();
            }
        }

        private void Save()
        {
            EditorPrefs.SetString(PREFS_SCENE_PATH, LevelEditorScenePath);
            EditorPrefs.SetString(PREFS_SCENE_NAME, LevelEditorSceneName);
        }

        private void Load()
        {
            LevelEditorScenePath = EditorPrefs.GetString(PREFS_SCENE_PATH, defaultScenePath);
            LevelEditorSceneName = EditorPrefs.GetString(PREFS_SCENE_NAME, defaultSceneName);

            if (string.IsNullOrWhiteSpace(LevelEditorScenePath))
            {
                LevelEditorScenePath = defaultScenePath;
            }

            if (string.IsNullOrWhiteSpace(LevelEditorSceneName))
            {
                LevelEditorSceneName = defaultSceneName;
            }
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
