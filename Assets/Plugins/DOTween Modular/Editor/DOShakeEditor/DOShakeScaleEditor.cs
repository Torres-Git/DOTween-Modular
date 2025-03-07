#if UNITY_EDITOR

using UnityEditor;

namespace DOTweenModular.Editor
{
    [CustomEditor(typeof(DOShakeScale)), CanEditMultipleObjects]
    public sealed class DOShakeScaleEditor : DOShakeBaseEditor
    {
        protected override void DrawValues()
        {
            DrawProperty(strengthProp);

            base.DrawValues();
        }
    }
}

#endif
