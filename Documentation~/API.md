# com.vahtyah.level-editor — API Reference

> Version 1.0.0. Auto-generated from the compiled public API by `VahTyah → Generate API Docs`. Do not edit by hand — edit the source or `Documentation~/Examples.md` instead.

This package ships as obfuscated DLLs. This file documents the full public surface so it can be used without decompiling.

## Usage Examples

A reusable level-editor EditorWindow (CustomList of levels, search/paginate/reorder, per-row
validation icons) plus a scene-side controller contract. Two things a game wires up: an optional
validation hook on its level data, and — the scene controller that loads a level into the scene.

### Deep-validate your level data (`IValidatableLevel`)

Implement this interface on your `LevelData` ScriptableObject to get a per-row error icon in the
level list, refreshed on save. It is opt-in: a `LevelData` that does not implement it still works
(null-checks only, no icon). `IValidatableLevel` is in the global namespace.

```csharp
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Level")]
public class LevelData : ScriptableObject, IValidatableLevel
{
    public int   par;
    public GameObject[] pieces;

    // Return false when invalid; append a human-readable reason per problem.
    public bool Validate(List<string> errors)
    {
        bool ok = true;
        if (par <= 0)            { errors.Add("Par must be > 0"); ok = false; }
        if (pieces == null || pieces.Length == 0) { errors.Add("No pieces assigned"); ok = false; }
        return ok;
    }
}
```

Icons refresh at **save time** (`Ctrl+S` / `AssetDatabase.SaveAssets()`), plus on Add/Duplicate/Delete
and the window's **Global Validation** button — so editing level data from code is caught too. Toggle
it with *Auto Validate (on save)* in the window's Settings panel.

### Scene-side loader (`SceneEditorControllerBase`)

Subclass this MonoBehaviour in the level-editing scene to apply/clear the level being edited. It is
in the global namespace and exposes a static `Instance`.

```csharp
using UnityEngine;

public class SceneEditorController : SceneEditorControllerBase
{
    public override void LoadLevel(Object levelObject, int index)
    {
        var level = (LevelData)levelObject;
        // spawn/preview `level` in the scene for editing
    }

    public override void ApplyLevel()   { /* write scene state back into the level asset */ }
    public override void DiscardLevel() { /* revert unsaved scene edits */ }
    public override void ClearLevel()   { /* tear down the preview */ }
}
```

### The editor window (`LevelEditorBase`) — extension points

`LevelEditorBase` (namespace `VahTyah.LevelEditor`, an `EditorWindow`) is the window base. A concrete
window subclasses it; the handler that binds a database is **config-driven** via `LevelEditorConfig`
(specify the level / database / handler type names as strings), so no `LevelsHandlerBase` subclass is
required — it falls back to `DefaultLevelsHandler`. Key members to override (see API Reference for
exact signatures):

- `OnEnable()` / `OnDisable()` — always call `base` (base wires the handler and disposes validation).
- `DrawContent()` — draw your window body.
- `OpenLevel(Object levelObject, int index)` — react to a level being opened.
- `DisplaySettings()` — extend the settings panel.
- `LevelHandler` / `GetLevelHandler` — the active `LevelsHandlerBase` (list, add, validate).

`LevelEditorConfig` fields: `levelTypeName`, `levelDatabaseTypeName`, `levelHandlerTypeName`
(optional), `levelFolderPath`, `levelPropertyName`, `duplicatePostfix`; `GetLevelType()` /
`GetLevelDatabaseType()` / `GetLevelHandlerType()` resolve them (the last returns null → use
`DefaultLevelsHandler`).

## API Reference

### namespace `<global>`

#### interface `IValidatableLevel`

Hợp đồng tùy chọn cho LevelData của từng game: cho phép Global Validation kiểm tra sâu nội dung level. LevelData không implement interface này vẫn dùng được (chỉ bỏ qua phần deep-validate, giữ null-check).  

```csharp
public abstract bool Validate(List<string> errors);
```

#### class `LevelEditorSettingsWindow`

*: EditorWindow*  

```csharp
public LevelEditorSettingsWindow();
public static void ShowWindow();
```

#### abstract class `SceneEditorControllerBase`

*: MonoBehaviour*  
Scene-side base controller that bridges the Level Editor window and the LevelEditor scene. Subclass in the game to load/clear/apply/discard a level inside the scene.  

```csharp
public SceneEditorControllerBase();
public static SceneEditorControllerBase Instance { get; }
public abstract void ApplyLevel();
public abstract void ClearLevel();
public abstract void DiscardLevel();
public abstract void LoadLevel(Object levelObject, int index);
```

- `Instance` — The most recently constructed controller instance in the scene.
- `LoadLevel` — Load the given level asset (at its list index) into the editor scene for editing.
- `ClearLevel` — Clear the currently loaded level from the editor scene.
- `ApplyLevel` — Apply / commit the current scene edits to the loaded level.
- `DiscardLevel` — Discard the current scene edits for the loaded level.

### namespace `VahTyah.LevelEditor`

#### class `CustomListSettings`

Persisted feature toggles for a ReorderableListView (header, search, footer buttons, navigation, drag, auto-validate). Loaded from and saved to PlayerPrefs under a key prefix.  

```csharp
public CustomListSettings(string prefsKeyPrefix = "CustomList_");
public bool AutoFocusOnOutsideClick { get; set; }
public bool EnableAutoValidate { get; set; }
public bool EnableElementRemoveButton { get; set; }
public bool EnableFooterAddButton { get; set; }
public bool EnableFooterReloadButton { get; set; }
public bool EnableFooterRemoveButton { get; set; }
public bool EnableHeader { get; set; }
public bool EnableKeyboardNavigation { get; set; }
public bool EnableScrollWheelNavigation { get; set; }
public bool EnableSearch { get; set; }
public bool IgnoreDragEvents { get; set; }
public void Display();
public void ResetToDefault();
```

- `EnableAutoValidate` — When enabled, re-validate a level and refresh its error icon each time it is saved; when off, only the Global Validation button updates.
- `Display` — Draw the settings toggles in the inspector/settings panel and persist any change to PlayerPrefs.
- `ResetToDefault` — Reset all toggles to their default values (does not save until Display detects a change).

#### interface `IEditorPanel`

A tabbed panel hosted by PanelNavigator. Implement to add a named tab to the editor content area.  

```csharp
public abstract void Draw(Rect rect);
public abstract void Initialize();
public abstract void OnDisable();
public abstract void OnEnable();
```

#### abstract class `LevelEditorBase`

*: EditorWindow, IHasCustomMenu*  
Base class for the Level Editor window. Subclass it (one subclass is auto-discovered for the Tools/Level Editor menu) and override DrawContent to render the main editing area. Hosts the level sidebar (list + toolbar) and the resizable splitter.  

```csharp
protected LevelEditorBase();
protected LevelEditorBaseSettings BaseSettings;
public static LevelEditorBase Instance;
protected LevelsHandlerBase LevelHandler;
protected ResizableSeparator ResizableSidebar;
public static EditorWindow Window;
protected LevelsHandlerBase GetLevelHandler { get; }
public void AddItemsToMenu(GenericMenu menu);
public virtual void DisplaySettings();
protected virtual void DrawContent();
protected virtual void OnDisable();
protected virtual void OnEnable();
protected void OnFocus();
protected virtual void OnGUI();
public virtual void OpenLevel(Object levelObject, int index);
```

- `Window` — The open Level Editor window.
- `Instance` — The active LevelEditorBase instance.
- `ResizableSidebar` — Draggable splitter controlling the sidebar width.
- `LevelHandler` — Handler that manages the level list, toolbar, and validation.
- `BaseSettings` — Resolved editor scene path/name settings.
- `GetLevelHandler` — Create the level handler instance. By default instantiates the type from the config; override to supply a custom handler.
- `OnFocus` — On window focus, scroll the list to the currently selected level.
- `OnEnable` — Create the level handler and sidebar splitter. Override to add setup; call base first.
- `OnDisable` — Dispose the level handler (unhooks validation events). Override to add teardown; call base.
- `OnGUI` — Draw the window: a warning when the editor scene is not open, otherwise the sidebar, splitter, and DrawContent.
- `DrawContent` — Override to draw the main editing area (right of the sidebar). Empty by default.
- `DisplaySettings` — Draw the level settings panel (the list feature toggles). Shown by the settings window.
- `AddItemsToMenu` — Add the window's context menu items (a "Settings" entry that opens the settings window).
- `OpenLevel` — Load the given level into the scene via SceneEditorControllerBase. Override to customize opening.

#### class `LevelEditorBaseSettings`

Resolves the Level Editor scene path and name from the styles database, falling back to the defaults passed to the constructor when no database exists.  

```csharp
public LevelEditorBaseSettings(string defaultScenePath, string defaultSceneName);
public string LevelEditorSceneName { get; }
public string LevelEditorScenePath { get; }
```

- `LevelEditorScenePath` — Editor scene path from the database, or the default when none is set.
- `LevelEditorSceneName` — Editor scene name from the database, or the default when none is set.

#### class `LevelEditorConfig`

Configuration for the Level Handler. Specify type names (simple or assembly-qualified) so that no LevelsHandlerBase subclass is required — use DefaultLevelsHandler instead.  

```csharp
public LevelEditorConfig();
public string duplicatePostfix;
public string levelDatabaseTypeName;
public string levelFolderPath;
public string levelHandlerTypeName;
public string levelPropertyName;
public string levelTypeName;
public Type GetLevelDatabaseType();
public Type GetLevelHandlerType();
public Type GetLevelType();
```

- `GetLevelDatabaseType` — Resolve the configured levels-database ScriptableObject type (null if unresolvable).
- `GetLevelType` — Resolve the configured single-level ScriptableObject type (null if unresolvable).
- `GetLevelHandlerType` — Resolves the configured LevelsHandlerBase subclass type. Returns null if empty or unresolvable — caller should fall back to DefaultLevelsHandler.

#### class `LevelEditorStyleData`

A complete visual style for the Level Editor, grouping the window background, the reorderable-list style, and the panel/tab-bar style.  

```csharp
public LevelEditorStyleData();
public LevelEditorStyleData.CustomListStyle customListStyle;
public LevelEditorStyleData.GlobalBackground globalBackground;
public LevelEditorStyleData.PanelNavigatorStyles panelNavigatorStyles;
public static LevelEditorStyleData CreateDefaultStyleData();
```

- `CreateDefaultStyleData` — Create style data with defaults chosen from the current editor skin (pro = dark).

#### class `LevelEditorStyleData.CustomListStyle`

Full styling for the reorderable list: feature flags, dimensions, per-component styles, and messages.  

```csharp
public CustomListStyle();
public LevelEditorStyleData.CustomListStyle.DragHandle dragHandle;
public LevelEditorStyleData.CustomListStyle.Element element;
public string emptyListMessage;
public bool enableAddDropdown;
public bool enableElementRemoveButton;
public bool enableFooterAddButton;
public bool enableFooterReloadButton;
public bool enableFooterRemoveButton;
public bool enableHeader;
public bool enableKeyboardNavigation;
public bool enableScrollWheelNavigation;
public bool enableSearch;
public LevelEditorStyleData.CustomListStyle.FooterButtons footerButtons;
public LevelEditorStyleData.CustomListStyle.Header header;
public bool ignoreDragEvents;
public LevelEditorStyleData.CustomListStyle.ElementList list;
public float minHeight;
public float minWidth;
public string noResultsMessage;
public LevelEditorStyleData.CustomListStyle.Pagination pagination;
public LevelEditorStyleData.CustomListStyle.RemoveElementButton removeElementButton;
public LevelEditorStyleData.CustomListStyle.SearchField searchField;
public bool stretchHeight;
public bool stretchWidth;
public static LevelEditorStyleData.CustomListStyle CreateDefaultStyles(bool isDarkMode);
```

#### class `LevelEditorStyleData.CustomListStyle.DragHandle`

Style for the per-row drag handle grip.  

```csharp
public DragHandle();
public float allocatedHorizontalSpace;
public float height;
public float paddingBottom;
public float paddingLeft;
public float width;
```

#### class `LevelEditorStyleData.CustomListStyle.Element`

Style for a single row, including selected / unselected / hover backgrounds.  

```csharp
public Element();
public float collapsedElementHeight;
public float headerPaddingBottom;
public float headerPaddingLeft;
public float headerPaddingRight;
public float headerPaddingTop;
public LayerConfiguration hoverBackgroundConfig;
public LayerConfiguration selectedBackgroundConfig;
public Color textColor;
public LayerConfiguration unselectedBackgroundConfig;
```

#### class `LevelEditorStyleData.CustomListStyle.ElementList`

Style for the list body (the area containing the element rows).  

```csharp
public ElementList();
public LayerConfiguration backgroundConfig;
public float contentPaddingBottom;
public float contentPaddingLeft;
public float contentPaddingRight;
public float contentPaddingTop;
```

#### class `LevelEditorStyleData.CustomListStyle.FooterButtons`

Style for the footer button bar (add / remove / reload).  

```csharp
public FooterButtons();
public LayerConfiguration backgroundConfig;
public float buttonsHeight;
public float buttonsWidth;
public float height;
public float marginRight;
public float paddingLeft;
public float paddingRight;
public float paddingTop;
public float spaceBetweenButtons;
```

#### class `LevelEditorStyleData.CustomListStyle.Header`

Style for the list header bar.  

```csharp
public Header();
public LayerConfiguration backgroundConfig;
public float contentPaddingBottom;
public float contentPaddingLeft;
public float contentPaddingRight;
public float contentPaddingTop;
public float height;
public Color textColor;
```

#### class `LevelEditorStyleData.CustomListStyle.Pagination`

Style for the pagination bar and its page buttons.  

```csharp
public Pagination();
public LayerConfiguration backgroundConfig;
public float buttonsHeight;
public float buttonsWidth;
public float contentPaddingBottom;
public float contentPaddingLeft;
public float contentPaddingRight;
public float contentPaddingTop;
public float height;
```

#### class `LevelEditorStyleData.CustomListStyle.RemoveElementButton`

Style for the per-row remove ("X") button.  

```csharp
public RemoveElementButton();
public float allocatedHorizontalSpace;
public int fontSize;
public float height;
public float paddingLeft;
public float paddingRight;
public string text;
public Color textColor;
public float width;
```

#### class `LevelEditorStyleData.CustomListStyle.SearchField`

Style for the search field and its clear button.  

```csharp
public SearchField();
public LayerConfiguration backgroundConfig;
public Color clearButtonHoverColor;
public string clearButtonText;
public Color clearButtonTextColor;
public float clearButtonWidth;
public float contentPaddingBottom;
public float contentPaddingLeft;
public float contentPaddingRight;
public float contentPaddingTop;
public float height;
```

#### class `LevelEditorStyleData.GlobalBackground`

Layered background style for the editor window / content area.  

```csharp
public GlobalBackground();
public LayerConfiguration backgroundConfig;
public static LevelEditorStyleData.GlobalBackground CreateDefaultStyles(bool isDarkMode);
```

#### class `LevelEditorStyleData.PanelNavigatorStyles`

Style for the PanelNavigator tab bar (menu bar height, active/inactive tab styles).  

```csharp
public PanelNavigatorStyles();
public LayerConfiguration activeTabStyle;
public LayerConfiguration inactiveTabStyle;
public float menuBarHeight;
public static LevelEditorStyleData.PanelNavigatorStyles CreateDefaultStyles(bool isDarkMode);
```

#### class `LevelEditorStylesDatabase`

*: ScriptableObject*  
ScriptableObject holding the Level Editor's theme styles, editor scene path/name, and the LevelEditorConfig. Resolves the active LevelEditorStyleData for the chosen theme.  

```csharp
public LevelEditorStylesDatabase();
public const string DEFAULT_LEVEL_EDITOR_SCENE_NAME;
public const string DEFAULT_LEVEL_EDITOR_SCENE_PATH;
public int CustomStyleCount { get; }
public int CustomStyleIndex { get; set; }
public LevelEditorConfig LevelEditorConfiguration { get; }
public string LevelEditorSceneName { get; set; }
public string LevelEditorScenePath { get; set; }
public LevelEditorThemeMode ThemeMode { get; set; }
public void AddDefaultStyle();
public static LevelEditorStyleData GetDefaultDarkStyle();
public static LevelEditorStyleData GetDefaultLightStyle();
public static LevelEditorStyleData GetDefaultStyle();
public LevelEditorStyleData GetStyle();
```

- `LevelEditorConfiguration` — The level editor configuration (level types, database property, folder, handler) for this database.
- `LevelEditorScenePath` — Project path of the LevelEditor scene; falls back to the default when unset.
- `LevelEditorSceneName` — Name of the LevelEditor scene; falls back to the default when unset.
- `CustomStyleCount` — Number of user-defined custom styles.
- `ThemeMode` — Selected theme mode, stored locally in EditorPrefs per asset GUID (not serialized in the asset).
- `CustomStyleIndex` — Index of the active custom style when ThemeMode is Custom (stored in EditorPrefs).
- `AddDefaultStyle` — Append the built-in dark and light styles to the custom styles list.
- `GetStyle` — Resolve the active LevelEditorStyleData according to ThemeMode.
- `GetDefaultStyle` — Built-in style matching the current editor skin (dark for pro skin, otherwise light).
- `GetDefaultDarkStyle` — Built-in dark style.
- `GetDefaultLightStyle` — Built-in light style.

#### class `LevelEditorStylesDatabaseEditor`

*: Editor*  
Custom inspector for LevelEditorStylesDatabase: theme selection plus type/scene/folder pickers for the level editor config.  

```csharp
public LevelEditorStylesDatabaseEditor();
public virtual void OnInspectorGUI();
```

#### enum `LevelEditorThemeMode`

Theme mode for the Level Editor: built-in Light/Dark, System (follows the editor skin), or a Custom style from the asset.  

```csharp
enum LevelEditorThemeMode : int
{
    Light = 0,
    Dark = 1,
    System = 2,
    Custom = 3,
}
```

#### static class `LevelEditorUtils`

Scene helpers for the Level Editor: check whether the editor scene is active and open it safely.  

```csharp
public static bool IsInScene(string sceneName);
public static void OpenScene(string scenePath, bool askToSave = true);
```

- `IsInScene` — Returns true if the currently active scene has the given name.
- `OpenScene` — Open the scene at the given path. If in play mode, exits play mode first and opens once back in edit mode. When askToSave is true, prompts to save the current modified scene first.

#### abstract class `LevelsHandlerBase`

Lớp cơ sở quản lý danh sách level cho Level Editor: nạp levels database, dựng ReorderableListView, xử lý add/remove/duplicate/reorder/swap/rename, copy-paste values, và validation (icon lỗi + cache). Kế thừa và override các thành viên protected để tuỳ biến hành vi cho từng game.  

```csharp
public LevelsHandlerBase();
public LevelsHandlerBase.AddElementCallbackDelegate addElementCallback;
public LevelsHandlerBase.AddElementWithDropdownCallbackDelegate addElementWithDropdownCallback;
public LevelsHandlerBase.DisplayContextMenuCallbackDelegate displayContextMenuCallback;
public LevelsHandlerBase.OnClearSelectionCallbackDelegate onClearSelectionCallback;
public LevelsHandlerBase.OnLevelChangedCallbackDelegate onLevelChangedCallback;
public LevelsHandlerBase.OnLevelOpenedCallbackDelegate onLevelOpenedCallback;
public LevelsHandlerBase.OnRenameAllCallbackDelegate onRenameAllCallback;
public LevelsHandlerBase.RemoveElementCallbackDelegate removeElementCallback;
public ReorderableListView CustomList { get; }
public string DuplicatePostfix { get; }
public Type GetLevelType { get; }
public bool IgnoreDragEvents { get; set; }
public static bool IsLastLevelOpened { get; set; }
public string LevelFolderPath { get; }
public int LevelsCount { get; }
public Object SelectedLevel { get; set; }
public int SelectedLevelIndex { get; }
public SerializedProperty SelectedLevelProperty { get; }
public void AddLevel();
public void DeleteLevel(int levelIndex);
public void DisplayReordableList();
public virtual void Dispose();
protected virtual void DrawButtonToolbar();
public void DrawGlobalValidationButton();
public void DrawRenameLevelsButton();
public void DrawToolbar();
public void DuplicateLevel(int sourceIndex);
public void FocusOnSelectedLevel();
protected virtual string FormatLevelNumberForDisplay(int number);
protected virtual string FormatLevelNumberForFile(int number);
protected virtual string GetLevelLabel(SerializedProperty elementProperty, int elementIndex);
public virtual string GetLevelLabel(Object levelObject, int index);
protected virtual Object GetLevelObject(SerializedProperty element);
protected virtual GUIContent GetLevelValidationIcon(int index);
public virtual void GlobalValidation();
protected virtual void OnAddElementWithDropdown(Rect buttonRect);
protected virtual void OnElementDoubleClicked(int index);
protected virtual void OnReload();
protected virtual bool OnSearchFilter(SerializedProperty prop, int index, string query);
public void OpenLastActiveLevel();
public virtual void OpenLevel(int index);
public void RenameLevels();
public void ReopenLevel();
protected virtual void SetContextMenu(int index, GenericMenu menu);
public virtual void SetLevelLabels();
protected virtual void SetLevelObject(SerializedProperty element, Object level);
protected virtual void SetupCallbacks();
public void UpdateCurrentLevelLabel(string label);
```

- `IsLastLevelOpened` — Cờ đánh dấu đã tự mở lại level active gần nhất trong phiên (tránh mở lại nhiều lần).
- `SelectedLevelProperty` — SerializedProperty của level đang chọn, hoặc null nếu không có level nào được chọn.
- `CustomList` — ReorderableListView của sidebar (danh sách level).
- `LevelsCount` — Số lượng level trong database.
- `GetLevelType` — Type của level asset, lấy từ LevelEditorConfig.
- `LevelFolderPath` — Thư mục (project-relative) chứa các level asset, lấy từ config (mặc định "Assets/Data/Levels").
- `DuplicatePostfix` — Hậu tố thêm vào tên file khi duplicate level (mặc định rỗng → chỉ lấy số kế tiếp).
- `GetLevelObject` — Resolve the level Object from a serialized array element. Default treats the element as a direct object reference; override for wrapper types (e.g. SmartReference) that store a guid instead of a direct reference.
- `SetLevelObject` — Assign a level Object into a serialized array element. Default sets the direct object reference; override for wrapper types (e.g. SmartReference).
- `SetupCallbacks` — Đăng ký toàn bộ callback của ReorderableListView. Override để thêm/đổi callback (gọi base).
- `GetLevelValidationIcon` — Icon cảnh báo vẽ ở cuối row nếu level tại index đang lỗi validation, ngược lại null.
- `OnReload` — Gọi khi list reload; dựng lại nhãn các level. Override để thêm xử lý reload.
- `GetLevelLabel` — Trả về nhãn hiển thị của một phần tử theo index (dùng cache nhãn). Override để đổi cách đặt nhãn.
- `SetContextMenu` — Dựng context menu chuột phải cho level tại index (Set Position, Swap, Duplicate, Copy/Paste, Delete). Override để tuỳ biến.
- `OnAddElementWithDropdown` — Xử lý nút Add ở chế độ dropdown. Mặc định bắn addElementWithDropdownCallback; override để hiện menu chọn loại level.
- `OnElementDoubleClicked` — Xử lý double-click một level: ping asset trong Project window. Override để đổi hành vi.
- `OnSearchFilter` — Predicate lọc tìm kiếm: khớp theo nhãn level hoặc theo số thứ tự. Override để đổi tiêu chí tìm.
- `OpenLevel` — Mở level tại index: lưu index vào PlayerPrefs và nạp vào scene. Override để tuỳ biến.
- `ReopenLevel` — Mở lại level đang chọn.
- `AddLevel` — Tạo một level asset mới (đặt tên theo số kế tiếp), thêm vào database, chọn và validate nó.
- `DuplicateLevel` — Nhân bản level tại sourceIndex thành asset mới (kèm DuplicatePostfix), thêm vào cuối và chọn nó.
- `DeleteLevel` — Xoá level tại levelIndex (có hộp thoại xác nhận): xoá asset, gỡ khỏi database và dọn icon lỗi.
- `UpdateCurrentLevelLabel` — Cập nhật nhãn hiển thị của level đang chọn (chỉ đổi cache nhãn, không đổi tên asset).
- `SetLevelLabels` — Dựng lại toàn bộ nhãn hiển thị của các level theo thứ tự hiện tại. Override để đổi cách đặt nhãn.
- `RenameLevels` — Đổi tên file mọi level về đúng format "Level_<số>" theo thứ tự (qua tên tạm để tránh đụng tên).
- `GlobalValidation` — Quét validation toàn bộ level và log chi tiết từng lỗi (hành vi của nút Global Validation).
- `Dispose` — Flush cache validation còn dở và gỡ subscription event. Gọi từ LevelEditorBase khi đóng window / trước domain reload.
- `DisplayReordableList` — Vẽ danh sách level (tự mở lại level active gần nhất ở lần đầu).
- `FocusOnSelectedLevel` — Cuộn danh sách tới trang chứa level đang chọn.
- `DrawRenameLevelsButton` — Vẽ nút "Rename Levels".
- `DrawGlobalValidationButton` — Vẽ nút "Global Validation".
- `DrawToolbar` — Vẽ toolbar dưới danh sách level (bọc DrawButtonToolbar).
- `DrawButtonToolbar` — Vẽ các nút của toolbar (mặc định: Rename + Global Validation). Override để thêm/đổi nút.
- `OpenLastActiveLevel` — Lần đầu trong phiên: chọn và mở lại level active gần nhất (theo PlayerPrefs), nếu không đang Play.
- `FormatLevelNumberForFile` — Format số level dùng cho TÊN FILE (mặc định số thô). Override để zero-pad, v.v.
- `FormatLevelNumberForDisplay` — Format số level dùng cho HIỂN THỊ (mặc định zero-pad theo tổng số level). Override để đổi.

#### delegate `LevelsHandlerBase.AddElementCallbackDelegate`

*: MulticastDelegate*  
Gọi sau khi một level mới được thêm.  

```csharp
delegate void AddElementCallbackDelegate();
```

#### delegate `LevelsHandlerBase.AddElementWithDropdownCallbackDelegate`

*: MulticastDelegate*  
Gọi khi nút Add dùng chế độ dropdown; nhận rect của nút để định vị menu.  

```csharp
delegate void AddElementWithDropdownCallbackDelegate(Rect buttonRect);
```

#### delegate `LevelsHandlerBase.DisplayContextMenuCallbackDelegate`

*: MulticastDelegate*  
Cho phép thêm mục vào context menu chuột phải của một level.  

```csharp
delegate void DisplayContextMenuCallbackDelegate(GenericMenu genericMenu);
```

#### delegate `LevelsHandlerBase.OnClearSelectionCallbackDelegate`

*: MulticastDelegate*  
Gọi khi bỏ chọn level đang chọn.  

```csharp
delegate void OnClearSelectionCallbackDelegate();
```

#### delegate `LevelsHandlerBase.OnLevelChangedCallbackDelegate`

*: MulticastDelegate*  
Gọi khi nội dung danh sách level thay đổi.  

```csharp
delegate void OnLevelChangedCallbackDelegate();
```

#### delegate `LevelsHandlerBase.OnLevelOpenedCallbackDelegate`

*: MulticastDelegate*  
Gọi khi một level được mở, kèm index của nó.  

```csharp
delegate void OnLevelOpenedCallbackDelegate(int levelIndex);
```

#### delegate `LevelsHandlerBase.OnRenameAllCallbackDelegate`

*: MulticastDelegate*  
Gọi sau khi đổi tên toàn bộ level.  

```csharp
delegate void OnRenameAllCallbackDelegate();
```

#### delegate `LevelsHandlerBase.RemoveElementCallbackDelegate`

*: MulticastDelegate*  
Gọi sau khi một level bị xoá.  

```csharp
delegate void RemoveElementCallbackDelegate();
```

#### class `PanelNavigator`

Draws a tab bar for a set of named IEditorPanels and renders the active one, styled from the LevelEditor styles database.  

```csharp
public PanelNavigator(Dictionary<string, IEditorPanel> panels);
public void Cleanup();
public void Draw(Rect rect);
public void OnEnable();
```

- `OnEnable` — Forward the enable signal to every hosted panel.
- `Cleanup` — Forward the disable signal to every hosted panel (call on window disable).
- `Draw` — Draw the active panel content and the tab bar within the given rect.

#### class `ReorderableListView`

Custom reorderable list widget for editor windows: paginated, searchable, drag-to-reorder, with per-row icons and configurable header/footer/navigation via Settings and callbacks. Backed by a SerializedProperty array, a List<SerializedProperty>, or a raw IList.  

```csharp
public ReorderableListView(IList elements, ReorderableListView.GetLabelDelegate getLabelCallback);
public ReorderableListView(SerializedObject serializedObject, SerializedProperty elements, string labelPropertyName);
public ReorderableListView(SerializedObject serializedObject, SerializedProperty elements, ReorderableListView.GetLabelDelegate getLabelCallback);
public ReorderableListView(SerializedObject serializedObject, List<SerializedProperty> propertyList, string labelPropertyName);
public ReorderableListView(SerializedObject serializedObject, List<SerializedProperty> propertyList, ReorderableListView.GetLabelDelegate getLabelCallback);
public readonly CustomListSettings Settings;
public ReorderableListView.AddElementCallbackDelegate addElementCallback;
public ReorderableListView.AddElementWithDropdownCallbackDelegate addElementWithDropdownCallback;
public ReorderableListView.DisplayContextMenuCallbackDelegate displayContextMenuCallback;
public ReorderableListView.ElementDoubleClickedDelegate elementDoubleClickedCallback;
public ReorderableListView.GetElementIconDelegate getElementIconCallback;
public ReorderableListView.GetHeaderLabelCallbackDelegate getHeaderLabelCallback;
public ReorderableListView.ListChangedCallbackDelegate listChangedCallback;
public ReorderableListView.ListReorderedCallbackDelegate listReorderedCallback;
public ReorderableListView.ListReorderedCallbackWithDetailsDelegate listReorderedCallbackWithDetails;
public ReorderableListView.ListUndoCallbackDelegate listUndoCallback;
public ReorderableListView.ReloadCallbackDelegate reloadCallback;
public ReorderableListView.RemoveElementCallbackDelegate removeElementCallback;
public ReorderableListView.SearchFilterDelegate searchFilterCallback;
public ReorderableListView.SelectionChangedCallbackDelegate selectionChangedCallback;
public LevelEditorStyleData CurrentCustomStyle { get; }
public bool IgnoreDragEvents { get; set; }
public float MinHeight { get; set; }
public float MinWidth { get; set; }
public EditorWindow ParentWindow { get; set; }
public int SelectedIndex { get; set; }
public bool StretchHeight { get; set; }
public bool StretchWidth { get; set; }
public int ArraySize();
public void Display();
public void FocusSelectionPage();
public SerializedProperty GetElement(int index);
public void LoadCustomStyle(int index = 0);
public void MoveElement(int srcIndex, int destIndex);
public void ReloadList();
public void RequestRepaint();
```

- `Settings` — Feature toggles (header, search, footer buttons, navigation, auto-validate) for this list.
- `ParentWindow` — Editor window used for repaint; falls back to the shared Level Editor window when unset.
- `CurrentCustomStyle` — The style data currently resolved from the LevelEditor styles database (or defaults).
- `RequestRepaint` — Request a repaint of the parent editor window.
- `LoadCustomStyle` — Load styling and dimensions from the LevelEditor styles database, falling back to defaults.
- `GetElement` — Returns the SerializedProperty for the element at index (null for IList-backed lists).
- `ArraySize` — Returns the element count of the backing data source.
- `MoveElement` — Move an element from one index to another (registering Undo) and fire the changed callback.
- `Display` — Draw the list and process its input for the current event; call from the window's OnGUI.
- `FocusSelectionPage` — Switch the current page so the selected element is visible.
- `ReloadList` — Refresh the serialized data from disk and fire the reload and changed callbacks.

#### delegate `ReorderableListView.AddElementCallbackDelegate`

*: MulticastDelegate*  
Invoked when the footer add button requests a new element.  

```csharp
delegate void AddElementCallbackDelegate();
```

#### delegate `ReorderableListView.AddElementWithDropdownCallbackDelegate`

*: MulticastDelegate*  
Invoked when the add button uses dropdown mode; receives the button rect for positioning a menu.  

```csharp
delegate void AddElementWithDropdownCallbackDelegate(Rect buttonRect);
```

#### delegate `ReorderableListView.DisplayContextMenuCallbackDelegate`

*: MulticastDelegate*  
Invoked to build the right-click context menu for the element at the given index.  

```csharp
delegate void DisplayContextMenuCallbackDelegate(int index);
```

#### delegate `ReorderableListView.ElementDoubleClickedDelegate`

*: MulticastDelegate*  
Invoked when an element is double-clicked.  

```csharp
delegate void ElementDoubleClickedDelegate(int index);
```

#### delegate `ReorderableListView.GetElementIconDelegate`

*: MulticastDelegate*  
Returns the icon drawn at the end of a row (e.g. a validation warning), or null for none.  

```csharp
delegate GUIContent GetElementIconDelegate(int elementIndex);
```

#### delegate `ReorderableListView.GetHeaderLabelCallbackDelegate`

*: MulticastDelegate*  
Returns the text drawn in the list header.  

```csharp
delegate string GetHeaderLabelCallbackDelegate();
```

#### delegate `ReorderableListView.GetLabelDelegate`

*: MulticastDelegate*  
Returns the display label for an element (used when no label property is supplied).  

```csharp
delegate string GetLabelDelegate(SerializedProperty elementProperty, int elementIndex);
```

#### delegate `ReorderableListView.ListChangedCallbackDelegate`

*: MulticastDelegate*  
Invoked after the list contents change (add/remove/reorder/duplicate).  

```csharp
delegate void ListChangedCallbackDelegate();
```

#### delegate `ReorderableListView.ListReorderedCallbackDelegate`

*: MulticastDelegate*  
Invoked after elements are reordered.  

```csharp
delegate void ListReorderedCallbackDelegate();
```

#### delegate `ReorderableListView.ListReorderedCallbackWithDetailsDelegate`

*: MulticastDelegate*  
Invoked after a reorder, with the source and destination indices.  

```csharp
delegate void ListReorderedCallbackWithDetailsDelegate(int srcIndex, int destIndex);
```

#### delegate `ReorderableListView.ListUndoCallbackDelegate`

*: MulticastDelegate*  
Invoked to register an Undo step before a mutating action, with its label.  

```csharp
delegate void ListUndoCallbackDelegate(string undoMessage);
```

#### delegate `ReorderableListView.ReloadCallbackDelegate`

*: MulticastDelegate*  
Invoked when the list is reloaded from the serialized data.  

```csharp
delegate void ReloadCallbackDelegate();
```

#### delegate `ReorderableListView.RemoveElementCallbackDelegate`

*: MulticastDelegate*  
Invoked when the footer remove button requests removing the selected element.  

```csharp
delegate void RemoveElementCallbackDelegate();
```

#### delegate `ReorderableListView.SearchFilterDelegate`

*: MulticastDelegate*  
Predicate deciding whether an element matches the current search query.  

```csharp
delegate bool SearchFilterDelegate(SerializedProperty elementProperty, int elementIndex, string searchQuery);
```

#### delegate `ReorderableListView.SelectionChangedCallbackDelegate`

*: MulticastDelegate*  
Invoked when the selected element changes.  

```csharp
delegate void SelectionChangedCallbackDelegate();
```

#### class `ResizableSeparator`

A draggable splitter for editor windows: draws a resize handle and tracks the current size, clamped between min/max and persisted per key in PlayerPrefs.  

```csharp
public ResizableSeparator(string prefsKey, int defaultSize, ResizeDirection direction = ResizeDirection.Horizontal, int minSize = 200, int maxSize = 500);
public int CurrentHeight { get; }
public int CurrentSize { get; }
public int CurrentWidth { get; }
public float TotalSize { get; }
public void DrawResizeSeparator();
public void ResetHeight();
public void ResetSize();
public void ResetWidth();
```

- `CurrentSize` — Current size (width for horizontal, height for vertical) in pixels.
- `CurrentWidth` — Alias of CurrentSize for horizontal separators.
- `CurrentHeight` — Alias of CurrentSize for vertical separators.
- `TotalSize` — Current size plus the drawn separator handle thickness.
- `DrawResizeSeparator` — Draw the separator handle (with a resize cursor) and process drag input to update the size.
- `ResetSize` — Reset the size back to the configured default and persist it.
- `ResetWidth` — Alias of ResetSize.
- `ResetHeight` — Alias of ResetSize.

#### enum `ResizeDirection`

Axis along which a ResizableSeparator can be dragged.  

```csharp
enum ResizeDirection : int
{
    Horizontal = 0,
    Vertical = 1,
}
```

