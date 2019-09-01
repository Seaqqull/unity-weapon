using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TagSelectorAttribute))]
public class TagSelectorPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Work only with string values
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // If activated default drawing option
        if ((this.attribute as TagSelectorAttribute).IsDefaultTagDrawer)
        {
            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        }
        else
        {
            // Generate list of tags
            List<string> tagValues = new List<string>();
            tagValues.Add("<NoTag>");
            tagValues.AddRange(UnityEditorInternal.InternalEditorUtility.tags);

            string propertyValue = property.stringValue;
            int index = 0;  //default <NoTag> value

            if (propertyValue != "")
            {
                // Check if there is selected tag and then assign its index
                for (int i = 1; i < tagValues.Count; i++)
                {
                    if (tagValues[i] == propertyValue)
                    {
                        index = i;
                        break;
                    }
                }
            }

            // Draw popup box with selected index
            index = EditorGUI.Popup(position, label.text, index, tagValues.ToArray());

            // Select active tag value based on the selection
            if (index >= 1)
                property.stringValue = tagValues[index];
            else
                property.stringValue = "";
        }

        EditorGUI.EndProperty();
    }
}
