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

        const string k_Elipsis = "…";

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
                UnityEngine.Object targetu��f��u����Ƀ����  �|t	��f�9f;>uf��tf�yf;~u��f��u����Ƀ�����  �u	��f�9f;>uf��tf�yf;~u��f��u����Ƀ�����  ��s	��f�9f;>uf��tf�yf;~u��f��u����Ƀ����Z  �Lu	��f�9f;>uf��tf�yf;~u��f��u����Ƀ����"  �`t	��f�9f;>uf��tf�yf;~u��f��u����Ƀ�����  �Dt	��f�9f;>uf��tf�yf;~u��f��u����Ƀ�����  �<u	��f�9f;>uf��tf�yf;~u��f��u����Ƀ����z  ��t	��f�9f;>uf��tf�yf;~u��f��u����Ƀ����B  ��t	��f�9f;>uf��tf�yf;~u��f��u����Ƀ����
  �(u	��f�9f;>uf��tf�yf;~u��f��u����Ƀ�����   ��t	��f�9f;>uf��tf�yf;~u��f��u����Ƀ�����   ��t	��f�9f;>uf��tf�yf;~u��f��u����Ƀ���tf��t	��f�9f;>uf��tf�yf;~u��f��u����Ƀ���t2�t	f�2f;1uf��tf�rf;qu��f��u����Ƀ���u�_^[������f�U��Q��3ŉE��EV�u��W�}���t�M+}��b	���U��u�M�_3�^�  ��]� �����f�U��QQ�USVW��3��}��u�h|�R�Xb	��YY��tQ�C
h��P�Xb	��YY��t3����+���;wt�F�}��\�}��]���L���]�멸��)� ������3�����E���x��u� ����w_^[��]� �����f�U���SVh���u3ۉM��]��Xb	YY��u
� ���C  W��$h��P�Xb	��YY���!  !]��E�;X�  ��h��W�Xb	�E�YY����   +ǍM�����j Q�ȉE�� ���������   �E����   �����E�H;�C�����;���   h�3ɋ�jZ�������Q� ��YY��tz�M�E�SP��虄������xX�M�3��U��}�h��f�K��\��]�C�]�L�����E���}�P�Xb	��YY���	�����t#�E��X3ۅ�tS�� Y�