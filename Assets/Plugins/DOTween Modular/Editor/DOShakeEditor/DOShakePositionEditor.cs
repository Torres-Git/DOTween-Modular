#if UNITY_EDITOR

using UnityEditor;

namespace DOTweenModular.Editor
{
    [CustomEditor(typeof(DOShakePosition)), CanEditMultipleObjects]
    public sealed class DOShakePositionEditor : DOShakeBaseEditor
    {
        private SerializedProperty snappingProp;

        public override void OnEnable()
        {
            base.OnEnable();
            
            snappingProp = serializedObject.FindProperty("snapping");
        }

        protected override void DrawValues()
        {
            DrawProperty(strengthProp);
            DrawProperty(snappingProp);

            base.DrawValues();
        }
    }
}

#endif
