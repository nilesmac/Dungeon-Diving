using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    enum TitleMode
    {
        None,
        DisabledComponent,
        Prefab,
        PrefabOutOfContext,
        Asset,
        GameObject
    }

    struct BreadCrumbTitle
    {
        public string name;
        public TitleMode mode;
    }

    class BreadcrumbDrawer
    {
        static readonly GUIContent s_TextContent = new GUIContent();
        static readonly string k_DisabledComponentText = L10n.Tr("The PlayableDirector is disabled");
        static readonly string k_PrefabOutOfContext = L10n.Tr("Prefab Isolation not enabled. Click to Enable.");

        static readonly GUIStyle k_BreadCrumbLeft;
        static readonly GUIStyle k_BreadCrumbMid;
        static readonly GUIStyle k_BreadCrumbLeftBg;
        static readonly GUIStyle k_BreadCrumbMidBg;
        static readonly GUIStyle k_BreadCrumbMidSelected;
        static readonly GUIStyle k_BreadCrumbMidBgSelected;

        static readonly Texture k_TimelineIcon;

        const string k_Elipsis = "â€¦";

        static BreadcrumbDrawer()
        {
            k_BreadCrumbLeft = new GUIStyle("GUIEditor.BreadcrumbLeft");
            k_BreadCrumbMid = new GUIStyle("GUIEditor.BreadcrumbMid");
            k_BreadCrumbLeftBg = new GUIStyle("GUIEditor.BreadcrumbLeftBackground");
            k_BreadCrumbMidBg = new GUIStyle("GUIEditor.BreadcrumbMidBackground");

            k_BreadCrumbMidSelected = new GUIStyle(k_BreadCrumbMid);
            k_BreadCrumbMidSelected.normal = k_BreadCrumbMidSelected.onNormal;

            k_BreadCrumbMidBgSelected = new GUIStyle(k_BreadCrumbMidBg);
            k_BreadCrumbMidBgSelected.normal = k_BreadCrumbMidBgSelected.onNormal;
            k_TimelineIcon = EditorGUIUtility.IconContent("TimelineAsset Icon").image;
        }

        static string FitTextInArea(float areaWidth, string text, GUIStyle style)
        {
            var borderWidth = style.border.left + style.border.right;
            var textWidth = style.CalcSize(EditorGUIUtility.TextContent(text)).x;

            if (borderWidth + textWidth < areaWidth)
                return text;

            // Need to truncate the text to fit in the areaWidth
            var textAreaWidth = areaWidth - borderWidth;
            var pixByChar = textWidth / text.Length;
            var charNeeded = (int)Mathf.Floor(textAreaWidth / pixByChar);
            charNeeded -= k_Elipsis.Length;

            if (charNeeded <= 0)
                return k_Elipsis;

            if (charNeeded <= text.Length)
                return k_Elipsis + " " + text.Substring(text.Length - charNeeded);

            return k_Elipsis;
        }

        public static void Draw(float breadcrumbAreaWidth, List<BreadCrumbTitle> labels, Action<int> navigateToBreadcrumbIndex)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(breadcrumbAreaWidth));
            {
                var labelWidth = (int)(breadcrumbAreaWidth / labels.Count);

                for (var i = 0; i < labels.Count; i++)
                {
                    var label = labels[i];

                    var style = i == 0 ? k_BreadCrumbLeft : k_BreadCrumbMid;
                    var backgroundStyle = i == 0 ? k_BreadCrumbLeftBg : k_BreadCrumbMidBg;

                    if (i == labels.Count - 1)
                    {
                        if (i > 0)
                        {
                            // Only tint last breadcrumb if we are dug-in
                            DrawBreadcrumbAsSelectedSubSequence(labelWidth, label, k_BreadCrumbMidSelected, k_BreadCrumbMidBgSelected);
                        }
                        else
                        {
                            DrawActiveBreadcrumb(labelWidth, label, style, backgroundStyle);
                        }
                    }
                    else
                    {
                        var previousContentColor = GUI.contentColor;

                        GUI.contentColor = new Color(previousContentColor.r,
                            previousContentColor.g,
                            previousContentColor.b,
                            previousContentColor.a * 0.6f);
                        var content = GetTextContent(labelWidth, label, style);
                        var rect = GetBreadcrumbLayoutRect(content, style);

                        if (Event.current.type == EventType.Repaint)
                        {
                            backgroundStyle.Draw(rect, GUIContent.none, 0);
                        }

                        if (GUI.Button(rect, content, style))
                        {
                            navigateToBreadcrumbIndex.Invoke(i);
                        }
                        GUI.contentColor = previousContentColor;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        static GUIContent GetTextContent(int width, BreadCrumbTitle text, GUIStyle style)
        {
            s_TextContent.tooltip = string.Empty;
            s_TextContent.image = null;
            if (text.mode == TitleMode.DisabledComponent)
            {
                s_TextContent.tooltip = k_DisabledComponentText;
                s_TextContent.image = EditorGUIUtility.GetHelpIcon(MessageType.Warning);
            }
            else if (text.mode == TitleMode.Prefab)
                s_TextContent.image = PrefabUtility.GameObjectStyles.prefabIcon;
            else if (text.mode == TitleMode.GameObject)
                s_TextContent.image = PrefabUtility.GameObjectStyles.gameObjectIcon;
            else if (text.mode == TitleMode.Asset)
                s_TextContent.image = k_TimelineIcon;
            else if (text.mode == TitleMode.PrefabOutOfContext)
            {
                s_TextContent.image = PrefabUtility.GameObjectStyles.prefabIcon;
                if (!TimelineWindow.instance.locked)
                    s_TextContent.tooltip = k_PrefabOutOfContext;
            }

            if (s_TextContent.image != null)
                width = Math.Max(0, width - s_TextContent.image.width);
            s_TextContent.text = FitTextInArea(width, text.name, style);

            return s_TextContent;
        }

        static void DrawBreadcrumbAsSelectedSubSequence(int width, BreadCrumbTitle label, GUIStyle style, GUIStyle backgroundStyle)
        {
            var rect = DrawActiveBreadcrumb(width, label, style, backgroundStyle);
            const float underlineThickness = 2.0f;
            const float underlineVerticalOffset = 0.0f;
            var underlineHorizontalOffset = backgroundStyle.border.right * 0.333f;
            var underlineRect = Rect.MinMaxRect(
                rect.xMin - underlineHorizontalOffset,
                rect.yMax - underlineThickness - underlineVerticalOffset,
                rect.xMax - underlineHorizontalOffset,
                rect.yMax - underlineVerticalOffset);

            EditorGUI.DrawRect(underlineRect, DirectorStyles.Instance.customSkin.colorSubSequenceDurationLine);
        }

        static Rect GetBreadcrumbLayoutRect(GUIContent content, GUIStyle style)
        {
            // the image makes the button far too big compared to non-image versions
            var image = content.image;
            content.image = null;
            var size = style.CalcSizeWithConstraints(content, Vector2.zero);
            content.image = image;
            if (image != null)
                size.x += size.y; // assumes square image, constrained by height

            return GUILayoutUtility.GetRect(content, style, GUILayout.MaxWidth(size.x));
        }

        static Rect DrawActiveBreadcrumb(int width, BreadCrumbTitle label, GUIStyle style, GUIStyle backgroundStyle)
        {
            var content = GetTextContent(width, label, style);
            var rect = GetBreadcrumbLayoutRect(content, style);

            if (Event.current.type == EventType.Repaint)
            {
                backgroundStyle.Draw(rect, GUIContent.none, 0);
            }

            if (GUI.Button(rect, content, style))
            {
                UnityEngine.Object targetuËóf…ÿuà‹ÈëÉƒÉ…É„  ¾|t	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„Ê  ¾u	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„’  ¾üs	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„Z  ¾Lu	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„"  ¾`t	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„ê  ¾Dt	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„²  ¾<u	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„z  ¾ˆt	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„B  ¾Ìt	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„
  ¾(u	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„Ò   ¾Üt	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…É„š   ¾˜t	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…Étf¾øt	‹Êf‹9f;>uf…ÿtf‹yf;~uËóf…ÿuà‹ÈëÉƒÉ…Ét2¹t	f‹2f;1uf…ötf‹rf;quÓËf…öuà‹ÈëÉƒÉ…Éu°_^[ÃÌÌÌÌÌfU‹ìQ¡€3Å‰Eü‹EV‹u¯ÆW‹}ø…öt‹M+}ÿìb	‹ÏÿUƒîuê‹Mü_3Í^è  ‹å]Â ÌÌÌÌÌfU‹ìQQ‹USVW‹ù3ö‰}ø‰uüh|ÑRÿXb	‹ØYY…ÛtQC
hˆÑPÿXb	‹ĞYY…Òt3ƒÂ‹Ê+ËÑù;wt‹F‹}ü‰\‹}ø‹]ü‹‰LƒÃ‰]üë©¸€ë)¹ ğ€ë¹ğ€3Àùğ€EÁ…Àx…öu¸ ğ€ë‰w_^[‹å]Â ÌÌÌÌÌfU‹ìƒìSVh˜Ñÿu3Û‰Mü‰]ôÿXb	YY…Àu
¾ ğ€éC  WƒÀ$hÀÑPÿXb	‹øYY…ÿ„!  !]ğ‹Eü;X„  ƒÇhÜÑWÿXb	‰EäYY…À„ï   +ÇMøÑø‹×j Q‹È‰Eìè …ÿÿ‹ğ…öˆÛ   ‹Eø¨…Ä   ÑèƒÊÿ‰EèH;ÈCÑöæ€;È‚°   hğ3É‹ÂjZ÷âÁ÷ÙÈQè« ‹ØYY…Ûtz‹MìEøSP‹×è™„ÿÿ‹ğ…öxX‹Mè3À‹Uü‹}ğhÀÑf‰K‹‰\‹‹]ôC‰]ô‰L‹ÆƒÇ‹EäƒÀ‰}ğPÿXb	‹øYY…ÿ…	ÿÿÿ…Ût#‹Eü‰X3Û…ÛtSè’ş Yë