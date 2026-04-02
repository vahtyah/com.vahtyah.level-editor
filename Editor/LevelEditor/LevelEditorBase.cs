using System.Reflection;
using UnityEngine;
using System;
using UnityEditor;
using VahTyah.LevelEditor;
using VahTyah.Core;
using Object = UnityEngine.Object;

namespace VahTyah.LevelEditor
{
    public abstract class LevelEditorBase : EditorWindow, IHasCustomMenu
    {
        private const string DEFAULT_LEVEL_EDITOR_SCENE_PATH = LevelEditorStylesDatabase.DEFAULT_LEVEL_EDITOR_SCENE_PATH;
        private const string DEFAULT_LEVEL_EDITOR_SCENE_NAME = LevelEditorStylesDatabase.DEFAULT_LEVEL_EDITOR_SCENE_NAME;

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

        protected void OnFocus()
        {
            LevelHandler?.FocusOnSelectedLevel();
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
        private readonly string defaultScenePath;
        private readonly string defaultSceneName;
        private LevelEditorStylesDatabase stylesDatabase;

        public string LevelEditorScenePath => GetDatabase() != null
            ? stylesDatabase.LevelEditorScenePath
            : defaultScenePath;
        public string LevelEditorSceneName => GetDatabase() != null
            ? stylesDatabase.LevelEditorSceneName
            : defaultSceneName;

        public LevelEditorBaseSettings(string defaultScenePath, string defaultSceneName)
        {
            this.defaultScenePath = defaultScenePath;
            this.defaultSceneName = defaultSceneName;
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            if (stylesDatabase == null)
            {
                stylesDatabase = EditorUtils.GetAsset<LevelEditorStylesDatabase>();
            }
        }

        private LevelEditorStylesDatabase GetDatabase()
        {
            LoadDatabase();
            return stylesDatabase;
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
