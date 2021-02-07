using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace UnityEditor.Timeline
{
    partial class TimelineWindow
    {
        public TimelineTreeViewGUI treeView { get; private set; }

        void TracksGUI(Rect clientRect, WindowState state, TimelineModeGUIState trackState)
        {
            if (Event.current.type == EventType.Repaint && treeView != null)
            {
                state.spacePartitioner.Clear();
            }

            if (state.IsEditingASubTimeline() && !state.IsEditingAnEmptyTimeline())
            {
                var headerRect = clientRect;
                headerRect.width = state.sequencerHeaderWidth;
                Graphics.DrawBackgroundRect(state, headerRect);

                var clipRect = clientRect;
                clipRect.xMin = headerRect.xMax;
                Graphics.DrawBackgroundRect(state, clipRect, subSequenceMode: true);
            }
            else
            {
                Graphics.DrawBackgroundRect(state, clientRect);
            }

            if (!state.IsEditingAnEmptyTimeline())
                m_TimeArea.DrawMajorTicks(sequenceContentRect, state.referenceSequence.frameRate);

            GUILayout.BeginVertical();
            {
                GUILayout.Space(5.0f);
                GUILayout.BeginHorizontal();

                if (this.state.editSequence.asset == null)
                    DrawNoSequenceGUI(state);
                else
                    DrawTracksGUI(clientRect, trackState);

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            Graphics.DrawShadow(clientRect);
        }

        void DrawNoSequenceGUI(WindowState windowState)
        {
            bool showCreateButton = false;
            var currentlySelectedGo = UnityEditor.Selection.activeObject != null ? UnityEditor.Selection.activeObject as GameObject : null;
            var textContent = DirectorStyles.noTimelineAssetSelected;
            var existingDirector = currentlySelectedGo != null ? currentlySelectedGo.GetComponent<PlayableDirector>() : null;
            var existingAsset = existingDirector != null ? existingDirector.playableAsset : null;

            if (currentlySelectedGo != null && !TimelineUtility.IsPrefabOrAsset(currentlySelectedGo) && existingAsset == null)
            {
                showCreateButton = true;
                textContent = new GUIContent(String.Format(DirectorStyles.createTimelineOnSelection.text, currentlySelectedGo.name, "a Director component and a Timeline asset"));
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.Label(textContent);

            if (showCreateButton)
            {
                GUILayout.BeginHorizontal();
                var textSize = GUI.skin.label.CalcSize(textContent);
                GUILayout.Space((textSize.x / 2.0f) - (WindowConstants.createButtonWidth / 2.0f));
                if (GUILayout.Button("Create", GUILayout.Width(WindowConstants.createButtonWidth)))
                {
                    var message = DirectorStyles.createNewTimelineText.text + " '" + currentlySelectedGo.name + "'";
                    string newSequencePath = EditorUtility.SaveFilePanelInProject(DirectorStyles.createNewTimelineText.text, currentlySelectedGo.name + "Timeline", "playable", message, ProjectWindowUtil.GetActiveFolderPath());
                    if (!string.IsNullOrEmpty(newSequencePath))
                    {
                        var newAsset = CreateInstance<TimelineAsset>();
                        AssetDatabase.CreateAsset(newAsset, newSequencePath);

                        Undo.IncrementCurrentGroup();

                        if (existingDirector == null)
                        {
                            existingDirector = Undo.AddComponent<PlayableDirector>(currentlySelectedGo);
                        }

                        existingDirector.playableAsset = newAsset;
                        SetCurrentTimeline(existingDirector);
                        var newTrack = TimelineHelpers.CreateTrack<AnimationTrack>();

                        windowState.previewMode = false;
                        TimelineUtility.SetSceneGameObject(windowState.editSequence.director, newTrack, currentlySelectedGo);
                    }

                    // If we reach this point, the state of the pannel has changed; skip the rest of this GUI phase
                    // Fixes: case 955831 - [OSX] NullReferenceException when creating a timeline on a selected object
                    GUIUtility.ExitGUI();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        public enum OverlayDataTypes
        {
            None,
            BackgroundColor,
            BackgroundTexture,
            TextBox
        }

        public struct OverlayData
        {
            public OverlayDataTypes types { get; private set; }
            public Rect rect { get; internal set; }
            public string text { get; private set; }
            public Texture2D texture { get; private set; }
            public Color color { get; private set; }
            public GUIStyle backgroundTextStyle { get; private set; }
            public GUIStyle textStyle { get; private set; }

            public static OverlayData CreateColorOverlay(Rect rectangle, Color backgroundColor)
            {
                OverlayData data = new OverlayData();
                data.rect = rectangle;
                data.color = backgroundColor;
                data.types = OverlayDataTypes.BackgroundColor;
                return data;
            }

            public static OverlayData CreateTextureOverlay(Rect rectangle, Texture2D backTexture)
            {
                OverlayData data = new OverlayData();
                data.rect = rectangle;
                data.texture = backTexture;
                data.types = OverlayDataTypes.BackgroundTexture;
                return data;
            }

            public static OverlayData CreateTextBoxOverlay(Rect rectangle, string msg, GUIStyle textstyle, Color textcolor, Color bgTextColor, GUIStyle bgTextStyle)
            {
                OverlayData data = new OverlayData();
                data.rect = rectangle;
                data.text = msg;
                data.textStyle = textstyle;
                data.textStyle.normal.textColor = textcolor;
                data.backgroundTextStyle = bgTextStyle;
                data.backgroundTextStyle.normal.textColor = bgTextColor;
                data.types = OverlayDataTypes.TextBox;
                return data;
            }
        }

        internal List<OverlayData> OverlayDrawData = new List<OverlayData>();

        void DrawTracksGUI(Rect clientRect, TimelineModeGUIState trackState)
        {
            GUILayout.BeginVertical(GUILayout.Height(clientRect.height));
            if (treeView != null)
            {
                if (Event.current.type == EventType.Layout)
                {
                    OverlayDrawData.Clear();
                }

                treeView.OnGUI(clientRect);

                if (Event.current.type == EventType.Repaint)
                {
                    foreach (var overlayData in OverlayDrawData)
                    {
                        using (new GUIViewportScope(sequenceContentRect))
                            DrawOverlay(overlayData);
                    }
                }
            }
            GUILayout.EndVertical();
        }

        void DrawOverlay(OverlayData overlayData)
        {
            Rect overlayRect = GUIClip.Clip(overlayData.rect);
            if (overlayData.types == OverlayDataTypes.BackgroundColor)
            {
                EditorGUI.DrawRect(overlayRect, overlayData.color);
            }
            else if (overlayData.types == OverlayDataTypes.BackgroundTexture)
            {
                GraphSV��M +���0�����D�����,�����@���P��  ��@����F�����L���� -_ V��P������  ���vX � -_ �֋E� �p(�HtX �M��e  �M��]  �E�P�XtX �M��\L����T����QL����l����FL���M��'�������� � jp��]W �� ��]�e� �}̃e� 3�f�E��E�   �����E�P�\tX h�>\ �M��E��hL��jXf�E̍M��E��E�  W�PfE������E��E��e� �M�e� P�E�P�  h �A jj(�E��E�P��� W��M�fE���  j_W�u���E��  �e� �M��}�3���>\ f�E�W��K���E��E�P�E�P�E�PSQ�̍EP�  �����M��E����+K��VWh�>\ ��  �K����M��  �M���  �E�P�XtX �M���J���M��������~� � h�   �&^W ��� �M�]3�3��u��u��E�   f�E�W��E��M�fE���  W��E��M�fE���  3��u�f�E��}��E�   �����E�P�\tX h,>\ �M��E���J��jV�E�PP�TtX �E�P�M���  �E��E�P�XtX �M��2J��3��uؿ?\ �E�   W�M�f�E��J���E��E�P�E�P�E�PSQ�E��P�T  �N����M��E�����I��VWhX?\ ��  �������e  �E��M�� �p(�