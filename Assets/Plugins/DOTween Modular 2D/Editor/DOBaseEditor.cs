#if UNITY_EDITOR

namespace DOTweenModular.Editor
{

    using DOTweenModular.Enums;
    using DG.Tweening;
    using UnityEngine;
    using UnityEditor;

    public class DOBaseEditor : Editor
    {

        #region Serialized Properties

        protected SerializedProperty beginProp;
        protected SerializedProperty tweenObjectProp;

        protected SerializedProperty delayProp;
        protected SerializedProperty tweenTypeProp;
        protected SerializedProperty loopTypeProp;
        private SerializedProperty easeTypeProp;
        private SerializedProperty curveProp;
        protected SerializedProperty loopsProp;
        protected SerializedProperty durationProp;

        private SerializedProperty onTweenCreatedProp;
        private SerializedProperty onTweenStartedProp;
        private SerializedProperty onTweenCompletedProp;
        private SerializedProperty onTweenKilledProp;

        #endregion

        private DOBase doBase;
        protected string componentName; 
        protected int instanceId;

        public virtual void OnEnable()
        {
            doBase = (DOBase)target;
            componentName = doBase.GetType().ToString();
            instanceId = doBase.GetInstanceID();

            beginProp = serializedObject.FindProperty("begin");
            tweenObjectProp = serializedObject.FindProperty("tweenObject");

            delayProp = serializedObject.FindProperty("delay");
            tweenTypeProp = serializedObject.FindProperty("tweenType");
            loopTypeProp = serializedObject.FindProperty("loopType");
            easeTypeProp = serializedObject.FindProperty("easeType");
            curveProp = serializedObject.FindProperty("curve");
            loopsProp = serializedObject.FindProperty("loops");
            durationProp = serializedObject.FindProperty("duration");

            onTweenCreatedProp = serializedObject.FindProperty("onTweenCreated");
            onTweenStartedProp = serializedObject.FindProperty("onTweenStarted");
            onTweenCompletedProp = serializedObject.FindProperty("onTweenCompleted");
            onTweenKilledProp = serializedObject.FindProperty("onTweenKilled");
        }

        private void OnDisable()
        { 
            // this means the GameObject/Component was deleted
            // TODO - Confirm it
            if (target == null)
            {
            
            }
        }

        #region Draw Properties Functions

        protected void DrawBeginProperty()
        {
            EditorGUILayout.PropertyField(beginProp);
        }
        protected void DrawTweenObjectProperty()
        {
            EditorGUILayout.PropertyField(tweenObjectProp);
        }
        protected void DrawDelayProperty()
        {
            EditorGUILayout.PropertyField(delayProp);
        }
        protected void DrawTweenTypeProperty()
        {
            EditorGUILayout.PropertyField(tweenTypeProp);
        }
        protected void DrawLoopTypeProperty()
        {
            EditorGUILayout.PropertyField(loopTypeProp);
        }
        protected void DrawEaseTypeProperty()
        {
            EditorGUILayout.PropertyField(easeTypeProp);
        }
        protected void DrawCurveProperty()
        {
            EditorGUILayout.PropertyField(curveProp);
        }
        protected void DrawLoopsProperty()
        {
            EditorGUILayout.PropertyField(loopsProp);
        }
        protected void DrawDurationProperty()
        {
            EditorGUILayout.PropertyField(durationProp);
        }
        protected void DrawOnTweenCreatedProperty()
        {
            EditorGUILayout.PropertyField(onTweenCreatedProp);
        }
        protected void DrawOnTweenStartedProperty()
        {
            EditorGUILayout.PropertyField(onTweenStartedProp);
        }
        protected void DrawOnTweenCompletedProp()
        {
            EditorGUILayout.PropertyField(onTweenCompletedProp);
        }
        protected void DrawOnTweenKilledProp()
        {
            EditorGUILayout.PropertyField(onTweenKilledProp);
        }

        #endregion

        #region GUI Element Handling

        protected void BeginBackgroundBox()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        }

        protected bool BeginFoldout(string foldoutName, bool isOpen)
        {
            string key = componentName + "_" + instanceId + "_" + foldoutName;

            if (!EditorPrefs.HasKey(key))
            {
                bool open = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, foldoutName);
                EditorPrefs.SetBool(key, open);
                return open;
            }
            else
            {
                bool open = EditorPrefs.GetBool(key);
                open = EditorGUILayout.BeginFoldoutHeaderGroup(open, foldoutName);
                EditorPrefs.SetBool(key, open);
                return open;
            }
        }

        protected void EndBackgroundBox()
        {
            EditorGUILayout.EndVertical();
        }

        protected void EndFoldout()
        {
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        #endregion

        #region Inspector Draw Functions

        /// <summary>
        /// Draws begin, tweenObjectProp(if Begin = After or With), kill <br/>
        /// destroy component, destroy gameObject
        /// </summary>
        protected void DrawLifeTimeSettings()
        {
            DrawBeginProperty();

            if ((Begin)beginProp.enumValueIndex == Begin.With ||
                (Begin)beginProp.enumValueIndex == Begin.After)
            {
                DrawTweenObjectProperty();
            }
        }

        /// <summary>
        /// Draws Helpbox for Inspector messages regarding tweenObject and Begin property
        /// </summary>
        protected void DrawTweenObjectHelpBox()
        {
            if ((Begin)beginProp.enumValueIndex == Begin.After ||
                (Begin)beginProp.enumValueIndex == Begin.With)
            {
                if (tweenObjectProp.objectReferenceValue == null)
                    EditorGUILayout.HelpBox("Tween Object is not assigned", MessageType.Error);
            }
            else
            {
                if (tweenObjectProp.objectReferenceValue != null)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.HelpBox("Tween Object is assigned, it should be removed", MessageType.Warning);

                    GUIContent trashButton = EditorGUIUtility.IconContent("TreeEditor.Trash");
                    trashButton.tooltip = "Remove Tween Object";

                    if (GUILayout.Button(trashButton, GUILayout.Height(40), GUILayout.Width(40 * 2f)))
                    {
                        tweenObjectProp.objectReferenceValue = null;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        /// <summary>
        /// Draws tweenType loopType (if tweenType = Looped), <br/> 
        /// easeType, curve(if easeType = INTERNAL_Custom)
        /// </summary>
        protected virtual void DrawTypeSettings()
        {
            DrawTweenTypeProperty();

            if ((Enums.TweenType)tweenTypeProp.enumValueIndex == Enums.TweenType.Looped)
            {
                DrawLoopTypeProperty();
            }

            EditorGUILayout.PropertyField(easeTypeProp);

            if ((Ease)easeTypeProp.enumValueIndex == Ease.INTERNAL_Custom)
            {
                DrawCurveProperty();
            }
        }

        /// <summary>
        /// Draws loops(if loopType = Looped), delay, duration Property
        /// </summary>    
        protected virtual void DrawValues()
        {
            if ((Enums.TweenType)tweenTypeProp.enumValueIndex == Enums.TweenType.Looped)
            {
                EditorGUILayout.PropertyField(loopsProp);
                DrawLoopsProperty();
            }

            DrawDelayProperty();
            DrawDurationProperty();
        }

        /// <summary>
        /// Draws onTweenCreated, onTweenStartedProp, onTweenCompleted, onTweenKilledProp events
        /// </summary>
        protected void DrawEvents()
        {
            DrawOnTweenCreatedProperty();
            DrawOnTweenStartedProperty();
            DrawOnTweenCompletedProp();
            DrawOnTweenKilledProp();
        }

        #endregion

        #region Preview Functions

        /// <summary>
        /// Draws complete lines to backward Tween Objects, also displays arrow head and Begin Property
        /// </summary>
        protected void DrawTweenObjectInfo()
        {
            Handles.color = Color.cyan;

            DOBase tweenObj = (DOBase)tweenObjectProp.objectReferenceValue;

            Vector2 lineStart = doBase.transform.position;
            Vector2 lineEnd = tweenObj.transform.position;

            Handles.DrawLine(lineStart, lineEnd);

            Vector2 midPoint = (lineStart + lineEnd) * 0.5f;
            string text = doBase.begin.ToString();
            Handles.Label(midPoint, text);

            Vector2 arrowPosition = Vector2.Lerp(lineStart, lineEnd, 0.1f);

            Vector2 arrowDirection = lineStart - midPoint;

            Handles.ConeHandleCap(10, arrowPosition, Quaternion.LookRotation(arrowDirection), 0.5f, EventType.Repaint);

            while ((tweenObj.begin == Begin.After || tweenObj.begin == Begin.With)
                     && tweenObj.tweenObject != null)
            {
                text = tweenObj.begin.ToString();

                lineStart = tweenObj.transform.position;
                tweenObj = tweenObj.tweenObject;

                lineEnd = tweenObj.transform.position;

                Handles.DrawLine(lineStart, lineEnd);

                midPoint = (lineStart + lineEnd) * 0.5f;                
                Handles.Label(midPoint, text);

                arrowPosition = Vector2.Lerp(lineStart, lineEnd, 0.1f);
                arrowDirection = lineStart - midPoint;
                Handles.ConeHandleCap(10, arrowPosition, Quaternion.LookRotation(arrowDirection), 0.5f, EventType.Repaint);
            }
        }

        #endregion

    }

    public class RelativeFlags : ScriptableObject
    {
        public bool firstTimeRelative;
        public bool firstTimeNonRelative;
    }

}

#endif