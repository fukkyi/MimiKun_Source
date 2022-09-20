using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif
public class SoundButton : Button
{
    public string selectSoundName = "Select";
    public string submitSoundName = "Submit";

    public override void OnSelect(BaseEventData eventData)
    {
        if (AppManager.Instance.EventSystem.firstSelectedGameObject == null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySE(selectSoundName);
            }
        }
        else
        {
            AppManager.Instance.EventSystem.firstSelectedGameObject = null;
        }

        base.OnSelect(eventData);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(submitSoundName);
        }
        base.OnSubmit(eventData);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SoundButton))]
public class CustomButtonEditor : ButtonEditor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SoundButton component = (SoundButton)target;

        string selectSoundPropertyName = "Select Sound Name";
        string submitSoundPropertyName = "Submit Sound Name";

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(component.selectSoundName)), new GUIContent(selectSoundPropertyName));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(component.submitSoundName)), new GUIContent(submitSoundPropertyName));

        serializedObject.ApplyModifiedProperties();

    }
}
#endif
