using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ArrayElementTitleAttribute))]
public class ArrayElementTitleDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                    GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    protected virtual ArrayElementTitleAttribute Atribute => (ArrayElementTitleAttribute)attribute;

    SerializedProperty TitleNameProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = Atribute;
        var newLabel = "";

        switch (attr.Varnames.Length)
        {
            case 0:
            {
                TitleNameProp = property.serializedObject.FindProperty(property.propertyPath);
                newLabel = GetTitle();
                if (string.IsNullOrEmpty(newLabel))
                    newLabel = label.text.Split(' ')[1];
                break;
            }
            case 1:
            {
                var fullPathName = $"{property.propertyPath}.{attr.Varnames[0]}";
                TitleNameProp = property.serializedObject.FindProperty(fullPathName);
                newLabel = GetTitle();
                break;
            }
            default:
            {
                foreach (var varName in attr.Varnames)
                {
                    var fullPathName = $"{property.propertyPath}.{varName}";
                    TitleNameProp = property.serializedObject.FindProperty(fullPathName);

                    string title = GetTitle();
                    if (string.IsNullOrEmpty(title))
                        continue;

                    if (attr.switchmode)
                    {
                        newLabel = title;
                        break;
                    }
                    newLabel += $"{title}|";
                }

                break;
            }
        }
        if (string.IsNullOrEmpty(newLabel))
            newLabel = label.text;

        if (attr.baseHeader != "")
            newLabel = $"{attr.baseHeader} {newLabel}";

        EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true);
    }

    private string GetTitle()
    {
        switch (TitleNameProp.propertyType)
        {
            case SerializedPropertyType.Generic:
                break;
            case SerializedPropertyType.Integer:
                return TitleNameProp.intValue.ToString();
            case SerializedPropertyType.Boolean:
                return TitleNameProp.boolValue.ToString();
            case SerializedPropertyType.Float:
                return TitleNameProp.floatValue.ToString();
            case SerializedPropertyType.String:
                return TitleNameProp.stringValue;
            case SerializedPropertyType.Color:
                return TitleNameProp.colorValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return TitleNameProp.objectReferenceValue.ToString();
            case SerializedPropertyType.LayerMask:
                break;
            case SerializedPropertyType.Enum:
                {
                    if (Atribute.lockEnumNull && TitleNameProp.intValue == 0)
                        return "";

                    return TitleNameProp.enumNames[TitleNameProp.enumValueIndex];
                }
            case SerializedPropertyType.Vector2:
                return TitleNameProp.vector2Value.ToString();
            case SerializedPropertyType.Vector3:
                return TitleNameProp.vector3Value.ToString();
            case SerializedPropertyType.Vector4:
                return TitleNameProp.vector4Value.ToString();
            case SerializedPropertyType.Rect:
                break;
            case SerializedPropertyType.ArraySize:
                break;
            case SerializedPropertyType.Character:
                break;
            case SerializedPropertyType.AnimationCurve:
                break;
            case SerializedPropertyType.Bounds:
                break;
            case SerializedPropertyType.Gradient:
                break;
            case SerializedPropertyType.Quaternion:
                break;
            default:
                break;
        }
        return "";
    }

}
