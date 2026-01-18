using UnityEngine;
using UnityEditor;
using System.IO;

namespace VahTyah
{
    public static class LevelEditorSettings
    {
        private const string EDITOR_STYLE_PATH = "Assets/Editor/EditorStyle";
        private const string LEVEL_EDITOR_STYLE_ASSET_NAME = "LevelEditorStyleDatabase.asset";

        [MenuItem("Tools/VahTyah/Styles/Level Editor Style", false, 2)]
        public static void CreateLevelEditorStyle()
        {
            if (!Directory.Exists(EDITOR_STYLE_PATH))
            {
                Directory.CreateDirectory(EDITOR_STYLE_PATH);
                AssetDatabase.Refresh();
            }

            var existingAsset = EditorUtils.GetAsset<LevelEditorStylesDatabase>();

            if (existingAsset != null)
            {
                Selection.activeObject = existingAsset;
                EditorGUIUtility.PingObject(existingAsset);
                return;
            }

            string assetPath = Path.Combine(EDITOR_STYLE_PATH, LEVEL_EDITOR_STYLE_ASSET_NAME);
            var database = EditorUtils.CreateAsset<LevelEditorStylesDatabase>(assetPath, true);
            database.AddDefaultStyle();

            Selection.activeObject = database;
            EditorGUIUtility.PingObject(database);
        }
    }
}
