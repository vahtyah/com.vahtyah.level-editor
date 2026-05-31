using UnityEngine;

public abstract class SceneEditorControllerBase : MonoBehaviour
{
    public static SceneEditorControllerBase Instance { get; private set; }

    public SceneEditorControllerBase()
    {
        Instance = this;
    }

    public abstract void LoadLevel(Object levelObject, int index);
    public abstract void ClearLevel();
    
    public abstract void ApplyLevel();
    public abstract void DiscardLevel();
}