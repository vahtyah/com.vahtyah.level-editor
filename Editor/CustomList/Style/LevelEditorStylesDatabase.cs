using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VahTyah.Core;

namespace VahTyah.LevelEditor
{
    // [CreateAssetMenu(fileName = "StylesDatabase", menuName = "Custom List/List Styles Database", order = 1)]
    public class LevelEditorStylesDatabase : ScriptableObject
    {
        // [SerializeField] private int defaultStyleIndex = 0;
        [SerializeField] private List<LevelEditorStyleData> styles = new List<LevelEditorStyleData>();

        public void AddDefaultStyle()
        {
            styles.Add(GetDefaultDarkStyle());
            styles.Add(GetDefaultLightStyle());
        }

        public LevelEditorStyleData GetStyle()
        {
            if (styles.Count < 2)
            {
                styles.Clear();
                AddDefaultStyle();
            }

            var isDarkMode = EditorGUIUtility.isProSkin;
            return isDarkMode ? styles[0] : styles[1];
        }

        public static LevelEditorStyleData GetDefaultStyle()
        {
            return EditorGUIUtility.isProSkin ? GetDefaultDarkStyle() : GetDefaultLightStyle();
        }

        public static LevelEditorStyleData GetDefaultDarkStyle()
        {
            LevelEditorStyleData defaultStyleData = new LevelEditorStyleData
            {
                globalBackground = LevelEditorStyleData.GlobalBackground.CreateDefaultStyles(true),
                customListStyle = LevelEditorStyleData.CustomListStyle.CreateDefaultStyles(true),
                panelNavigatorStyles = LevelEditorStyleData.PanelNavigatorStyles.CreateDefaultStyles(true)
            };
            return defaultStyleData;
        }

        public static LevelEditorStyleData GetDefaultLightStyle()
        {
            LevelEditorStyleData defaultStyleData = new LevelEditorStyleData
            {
                globalBackground = LevelEditorStyleData.GlobalBackground.CreateDefaultStyles(false),
                customListStyle = LevelEditorStyleData.CustomListStyle.CreateDefaultStyles(false),
                panelNavigatorStyles = LevelEditorStyleData.PanelNavigatorStyles.CreateDefaultStyles(false)
            };
            return defaultStyleData;
        }
    }

    [Serializable]
    public class LevelEditorStyleData
    {
        public GlobalBackground globalBackground;
        public CustomListStyle customListStyle;
        public PanelNavigatorStyles panelNavigatorStyles;

        public static LevelEditorStyleData CreateDefaultStyleData()
        {
            bool isDarkMode = IsDarkMode();
            
            LevelEditorStyleData styleData = new LevelEditorStyleData
            {
                globalBackground = GlobalBackground.CreateDefaultStyles(isDarkMode),
                customListStyle = CustomListStyle.CreateDefaultStyles(isDarkMode),
                panelNavigatorStyles = PanelNavigatorStyles.CreateDefaultStyles(isDarkMode)
            };
            return styleData;
        }
        
        private static bool IsDarkMode()
        {
            return EditorGUIUtility.isProSkin;
        }


        [Serializable]
        public class GlobalBackground
        {
            public LayerConfiguration backgroundConfig;
            
            public static GlobalBackground CreateDefaultStyles(bool isDarkMode)
            {
                GlobalBackground globalBackground = new GlobalBackground
                {
                    backgroundConfig = new LayerConfiguration(2)
                };

                if (isDarkMode)
                {
                    // Dark mode styles
                    globalBackground.backgroundConfig.layers[0] = new Layer
                    {
                        type = LayerType.RoundedRect,
                        color = new Color(0.302f, 0.302f, 0.302f, 1f),
                        borderWidth = Vector4.one * 100,
                        borderRadius = Vector4.one * 4,
                        padding = new Padding(1, 1, 1, 1)
                    };

                    globalBackground.backgroundConfig.layers[1] = new Layer
                    {
                        type = LayerType.Border,
                        color = new Color(0.141f, 0.141f, 0.141f, 1f),
                        borderWidth = Vector4.one * 1,
                        borderRadius = Vector4.one * 4,
                        padding = new Padding(1, 1, 1, 1)
                    };
                }
                else
                {
                    // Light mode styles
                    globalBackground.backgroundConfig.layers[0] = new Layer
                    {
                        type = LayerType.RoundedRect,
                        color = new Color(0.76f, 0.76f, 0.76f, 1f),
                        borderWidth = Vector4.one * 100,
                        borderRadius = Vector4.one * 4,
                        padding = new Padding(1, 1, 1, 1)
                    };

                    globalBackground.backgroundConfig.layers[1] = new Layer
                    {
                        type = LayerType.Border,
                        color = new Color(0.6f, 0.6f, 0.6f, 1f),
                        borderWidth = Vector4.one * 1,
                        borderRadius = Vector4.one * 4,
                        padding = new Padding(1, 1, 1, 1)
                    };
                }

                return globalBackground;
            }
        }

        [Serializable]
        public class CustomListStyle
        {
            [Header("Features")] public bool enableHeader = true;
            public bool enableSearch = true;
            public bool enableFooterAddButton = true;
            public bool enableFooterRemoveButton = true;
            public bool enableFooterReloadButton = false;
            public bool enableKeyboardNavigation = true;
            public bool enableScrollWheelNavigation = true;
            public bool enableElementRemoveButton = false;
            public bool enableAddDropdown = false;
            public bool ignoreDragEvents = false;

            [Header("Dimensions")] public float minHeight = 200f;
            public float minWidth = 150f;
            public bool stretchHeight = true;
            public bool stretchWidth = true;

            [Header("Components")] public Header header;
            public SearchField searchField;
            public ElementList list;
            public Element element;
            public DragHandle dragHandle;
            public RemoveElementButton removeElementButton;
            public Pagination pagination;
            public FooterButtons footerButtons;

            [Header("Messages")] public string emptyListMessage = "List is empty";
            public string noResultsMessage = "No results found";

            public static CustomListStyle CreateDefaultStyles(bool isDarkMode)
            {
                CustomListStyle styles = new CustomListStyle
                {
                    enableHeader = false,
                    enableSearch = false,
                    enableFooterAddButton = true,
                    enableFooterRemoveButton = true,
                    enableElementRemoveButton = false,
                    ignoreDragEvents = false,

                    minHeight = 200f,
                    minWidth = 150f,
                    stretchHeight = true,
                    stretchWidth = true,

                    emptyListMessage = "List is empty",
                    noResultsMessage = "No results found",

                    header = CreateHeaderStyle(isDarkMode),
                    searchField = CreateSearchFieldStyle(isDarkMode),
                    list = CreateElementListStyle(),
                    element = CreateElementStyle(isDarkMode),
                    dragHandle = CreateDragHandleStyle(),
                    removeElementButton = CreateRemoveElementButtonStyle(isDarkMode),
                    pagination = CreatePaginationStyle(isDarkMode),
                    footerButtons = CreateFooterButtonsStyle(isDarkMode),
                };

                return styles;
            }

            private static Header CreateHeaderStyle(bool isDarkMode)
            {
                Header header = new Header
                {
                    height = 20f,
                    contentPaddingLeft = 6f,
                    contentPaddingRight = 6f,
                    contentPaddingTop = 2f,
                    contentPaddingBottom = 2f,
                    textColor = isDarkMode ? Color.white : Color.black,
                    backgroundConfig = new LayerConfiguration(1)
                };

                header.backgroundConfig.layers[0] = new Layer
                {
                    type = LayerType.Border,
                    color = isDarkMode ? new Color(0.22f, 0.22f, 0.22f, 1f) : new Color(0.65f, 0.65f, 0.65f, 1f),
                    borderWidth = new Vector4(0, 0, 0, 1),
                    borderRadius = Vector4.zero,
                    padding = new Padding()
                };

                return header;
            }

            private static SearchField CreateSearchFieldStyle(bool isDarkMode)
            {
                SearchField searchField = new SearchField
                {
                    height = 22f,
                    contentPaddingLeft = 6f,
                    contentPaddingRight = 6f,
                    contentPaddingTop = 2f,
                    contentPaddingBottom = 2f,
                    clearButtonWidth = 16f,
                    clearButtonText = "×",
                    clearButtonTextColor = isDarkMode ? new Color(0.5f, 0.5f, 0.5f, 0.8f) : new Color(0.4f, 0.4f, 0.4f, 0.8f),
                    clearButtonHoverColor = new Color(1f, 0.3f, 0.3f, 1f),
                    backgroundConfig = new LayerConfiguration(1)
                };

                searchField.backgroundConfig.layers[0] = new Layer
                {
                    type = LayerType.Border,
                    color = isDarkMode ? new Color(0.22f, 0.22f, 0.22f, 1f) : new Color(0.65f, 0.65f, 0.65f, 1f),
                    borderWidth = new Vector4(0, 0, 0, 1),
                    borderRadius = Vector4.zero,
                    padding = new Padding()
                };

                return searchField;
            }

            private static ElementList CreateElementListStyle()
            {
                return new ElementList
                {
                    contentPaddingLeft = 6f,
                    contentPaddingRight = 6f,
                    contentPaddingTop = 2f,
                    contentPaddingBottom = 2f,
                    backgroundConfig = new LayerConfiguration(0)
                };
            }

            private static Element CreateElementStyle(bool isDarkMode)
            {
                Element element = new Element
                {
                    collapsedElementHeight = 20f,
                    headerPaddingLeft = 0f,
                    headerPaddingRight = 6f,
                    headerPaddingTop = 0f,
                    headerPaddingBottom = 0f,
                    textColor = isDarkMode ? Color.white : Color.black,
                    selectedBackgroundConfig = new LayerConfiguration(1),
                    unselectedBackgroundConfig = new LayerConfiguration(0),
                    hoverBackgroundConfig = new LayerConfiguration(1)
                };

                if (isDarkMode)
                {
                    // Dark mode selection colors
                    element.selectedBackgroundConfig.layers[0] = Layer.CreateSolidColor(
                        new Color(0.172549f, 0.3647059f, 0.5294118f, 1f)
                    );

                    element.hoverBackgroundConfig.layers[0] = Layer.CreateSolidColor(
                        new Color(0.3f, 0.3f, 0.3f, 0.5f)
                    );
                }
                else
                {
                    // Light mode selection colors
                    element.selectedBackgroundConfig.layers[0] = Layer.CreateSolidColor(
                        new Color(0.22f, 0.45f, 0.69f, 1f)
                    );

                    element.hoverBackgroundConfig.layers[0] = Layer.CreateSolidColor(
                        new Color(0.7f, 0.7f, 0.7f, 0.5f)
                    );
                }

                return element;
            }

            private static DragHandle CreateDragHandleStyle()
            {
                return new DragHandle
                {
                    paddingLeft = 5f,
                    paddingBottom = 6f,
                    width = 10f,
                    height = 6f,
                    allocatedHorizontalSpace = 20f
                };
            }

            private static RemoveElementButton CreateRemoveElementButtonStyle(bool isDarkMode)
            {
                return new RemoveElementButton
                {
                    width = 20f,
                    height = 20f,
                    paddingRight = 0f,
                    paddingLeft = 6f,
                    allocatedHorizontalSpace = 26f,
                    text = "X",
                    fontSize = 16,
                    textColor = isDarkMode ? Color.white : Color.black
                };
            }

            private static Pagination CreatePaginationStyle(bool isDarkMode)
            {
                Pagination pagination = new Pagination
                {
                    height = 20f,
                    contentPaddingLeft = 6f,
                    contentPaddingRight = 6f,
                    contentPaddingTop = 2f,
                    contentPaddingBottom = 2f,
                    buttonsWidth = 25f,
                    buttonsHeight = 16f,
                    backgroundConfig = new LayerConfiguration(1)
                };

                pagination.backgroundConfig.layers[0] = new Layer
                {
                    type = LayerType.Border,
                    color = isDarkMode ? new Color(0.22f, 0.22f, 0.22f, 1f) : new Color(0.65f, 0.65f, 0.65f, 1f),
                    borderWidth = new Vector4(0, 1, 0, 0),
                    borderRadius = Vector4.zero,
                    padding = new Padding()
                };

                return pagination;
            }

            private static FooterButtons CreateFooterButtonsStyle(bool isDarkMode)
            {
                FooterButtons footerButtons = new FooterButtons
                {
                    height = 20f,
                    marginRight = 10f,
                    spaceBetweenButtons = 0f,
                    paddingTop = 4f,
                    paddingLeft = 4f,
                    paddingRight = 4f,
                    buttonsWidth = 25f,
                    buttonsHeight = 16f,
                    backgroundConfig = new LayerConfiguration(2)
                };

                if (isDarkMode)
                {
                    // Dark mode styles
                    footerButtons.backgroundConfig.layers[0] = new Layer
                    {
                        type = LayerType.RoundedRect,
                        color = new Color(0.302f, 0.302f, 0.302f, 1f),
                        borderWidth = Vector4.one * 100,
                        borderRadius = new Vector4(0, 0, 4, 4),
                        padding = new Padding(0, 0, 0, 0)
                    };

                    footerButtons.backgroundConfig.layers[1] = new Layer
                    {
                        type = LayerType.Border,
                        color = new Color(0.141f, 0.141f, 0.141f, 1f),
                        borderWidth = new Vector4(1, 0, 1, 1),
                        borderRadius = new Vector4(0, 0, 4, 4),
                        padding = new Padding(0, 0, 0, 0)
                    };
                }
                else
                {
                    // Light mode styles
                    footerButtons.backgroundConfig.layers[0] = new Layer
                    {
                        type = LayerType.RoundedRect,
                        color = new Color(0.76f, 0.76f, 0.76f, 1f),
                        borderWidth = Vector4.one * 100,
                        borderRadius = new Vector4(0, 0, 4, 4),
                        padding = new Padding(0, 0, 0, 0)
                    };

                    footerButtons.backgroundConfig.layers[1] = new Layer
                    {
                        type = LayerType.Border,
                        color = new Color(0.6f, 0.6f, 0.6f, 1f),
                        borderWidth = new Vector4(1, 0, 1, 1),
                        borderRadius = new Vector4(0, 0, 4, 4),
                        padding = new Padding(0, 0, 0, 0)
                    };
                }

                return footerButtons;
            }

            [Serializable]
            public class Header
            {
                public float height;
                public float contentPaddingLeft;
                public float contentPaddingRight;
                public float contentPaddingTop;
                public float contentPaddingBottom;
                public Color textColor = Color.white;
                public LayerConfiguration backgroundConfig;
            }

            [Serializable]
            public class SearchField
            {
                public float height;
                public float contentPaddingLeft;
                public float contentPaddingRight;
                public float contentPaddingTop;
                public float contentPaddingBottom;
                public float clearButtonWidth;
                public string clearButtonText;
                public Color clearButtonTextColor;
                public Color clearButtonHoverColor;
                public LayerConfiguration backgroundConfig;
            }

            [Serializable]
            public class ElementList
            {
                public float contentPaddingLeft;
                public float contentPaddingRight;
                public float contentPaddingTop;
                public float contentPaddingBottom;
                public LayerConfiguration backgroundConfig;
            }

            [Serializable]
            public class Element
            {
                public float collapsedElementHeight;
                public float headerPaddingLeft;
                public float headerPaddingRight;
                public float headerPaddingTop;
                public float headerPaddingBottom;
                public Color textColor = Color.white;
                public LayerConfiguration selectedBackgroundConfig;
                public LayerConfiguration unselectedBackgroundConfig;
                public LayerConfiguration hoverBackgroundConfig;
            }

            [Serializable]
            public class DragHandle
            {
                public float paddingLeft;
                public float paddingBottom;
                public float width;
                public float height;
                public float allocatedHorizontalSpace;
            }

            [Serializable]
            public class RemoveElementButton
            {
                public float width;
                public float height;
                public float paddingRight;
                public float paddingLeft;
                public float allocatedHorizontalSpace;
                public string text;
                public int fontSize;
                public Color textColor = Color.white;
            }

            [Serializable]
            public class Pagination
            {
                public float height;
                public float contentPaddingLeft;
                public float contentPaddingRight;
                public float contentPaddingTop;
                public float contentPaddingBottom;
                public float buttonsWidth;
                public float buttonsHeight;
                public LayerConfiguration backgroundConfig;
            }

            [Serializable]
            public class FooterButtons
            {
                public float height;
                public float marginRight;
                public float spaceBetweenButtons;
                public float paddingTop;
                public float paddingLeft;
                public float paddingRight;
                public float buttonsWidth;
                public float buttonsHeight;
                public LayerConfiguration backgroundConfig;
            }
        }

        [Serializable]
        public class PanelNavigatorStyles
        {
            public float menuBarHeight;
            public LayerConfiguration activeTabStyle;
            public LayerConfiguration inactiveTabStyle;

            public static PanelNavigatorStyles CreateDefaultStyles(bool isDarkMode)
            {
                PanelNavigatorStyles styles = new PanelNavigatorStyles
                {
                    menuBarHeight = 28f,
                    activeTabStyle = CreateActiveTabStyle(isDarkMode),
                    inactiveTabStyle = CreateInactiveTabStyle(isDarkMode)
                };
                return styles;
            }

            private static LayerConfiguration CreateActiveTabStyle(bool isDarkMode)
            {
                LayerConfiguration config = new LayerConfiguration(2);
                
                if (isDarkMode)
                {
                    // Dark mode styles
                    config.layers[0] = new Layer();
                    config.layers[0].type = LayerType.RoundedRect;
                    config.layers[0].color = new Color(0.302f, 0.302f, 0.302f, 1f);
                    config.layers[0].borderWidth = Vector4.one * 100;
                    config.layers[0].borderRadius = new Vector4(4, 4, 0, 0);
                    config.layers[0].padding = new Padding(2, 2, 2, 0);

                    config.layers[1] = new Layer();
                    config.layers[1].type = LayerType.Border;
                    config.layers[1].color = new Color(0.141f, 0.141f, 0.141f, 1f);
                    config.layers[1].borderWidth = new Vector4(1, 1, 1, 0);
                    config.layers[1].borderRadius = new Vector4(4, 4, 0, 0);
                    config.layers[1].padding = new Padding(2, 2, 2, 0);
                }
                else
                {
                    // Light mode styles
                    config.layers[0] = new Layer();
                    config.layers[0].type = LayerType.RoundedRect;
                    config.layers[0].color = new Color(0.76f, 0.76f, 0.76f, 1f);
                    config.layers[0].borderWidth = Vector4.one * 100;
                    config.layers[0].borderRadius = new Vector4(4, 4, 0, 0);
                    config.layers[0].padding = new Padding(2, 2, 2, 0);

                    config.layers[1] = new Layer();
                    config.layers[1].type = LayerType.Border;
                    config.layers[1].color = new Color(0.6f, 0.6f, 0.6f, 1f);
                    config.layers[1].borderWidth = new Vector4(1, 1, 1, 0);
                    config.layers[1].borderRadius = new Vector4(4, 4, 0, 0);
                    config.layers[1].padding = new Padding(2, 2, 2, 0);
                }
                
                return config;
            }

            private static LayerConfiguration CreateInactiveTabStyle(bool isDarkMode)
            {
                LayerConfiguration config = new LayerConfiguration(1);
                config.layers[0] = new Layer();
                config.layers[0].type = LayerType.RoundedRect;
                config.layers[0].color = isDarkMode 
                    ? new Color(0.302f, 0.302f, 0.302f, .5f) 
                    : new Color(0.76f, 0.76f, 0.76f, .5f);
                config.layers[0].borderWidth = Vector4.one * 100;
                config.layers[0].borderRadius = new Vector4(4, 4, 0, 0);
                config.layers[0].padding = new Padding(2, 2, 2, 0);
                return config;
            }
        }
    }
}