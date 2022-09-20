using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// ��莞�Ԍ�ɃA�N�V���������s����^�C�}�[
/// </summary>
[Serializable]
public class ActionTimer
{
    public float activateTime = 1.0f;
    public float elapsedTime = 0;
    public bool repeatAction = false;
    public bool IsExecutedAction { get; protected set; } = false;
    public Action action = null;

    /// <summary>
    /// �^�C�}�[���X�V����
    /// </summary>
    public void UpdateTimer(bool fixedDeltaTime = false)
    {
        // ���s���鎞�Ԃ��ق�0��菬�����ꍇ�̓^�C�}�[���X�V���Ȃ�
        if (activateTime <= float.Epsilon) return;

        elapsedTime += fixedDeltaTime ? Time.fixedDeltaTime : Time.deltaTime;
        // �ڕW�̎��Ԃ��o�߂�����A�N�V���������s����
        if (repeatAction)
        {
            while (elapsedTime >= activateTime)
            {
                action.Invoke();
                elapsedTime -= activateTime;
            }
        }
        else if(!IsExecutedAction)
        {
            if (elapsedTime >= activateTime)
            {
                action.Invoke();
                elapsedTime = activateTime;
                IsExecutedAction = true;
            }
        }
    }

    /// <summary>
    /// �^�C�}�[��������x���s�o����悤�Ƀ��Z�b�g����
    /// </summary>
    public void ResetTimer()
    {
        elapsedTime = 0;
        IsExecutedAction = false;
    }

    /// <summary>
    /// �ڕW���Ԃ܂ł̌o�ߗ����擾����
    /// </summary>
    /// <returns></returns>
    public float GetElapsedRate()
    {
        if (activateTime <= float.Epsilon) return 0;

        return elapsedTime / activateTime;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ActionTimer))]
public class ActionDrawer : PropertyDrawer
{
    private static readonly float indentWidth = 15.0f;

    private bool showAdvanceProperty = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            SerializedProperty activateTimeProperty = property.FindPropertyRelative("activateTime");
            SerializedProperty elapsedTimeProperty = property.FindPropertyRelative("elapsedTime");
            SerializedProperty repeatActionProperty = property.FindPropertyRelative("repeatAction");

            float activateTime = activateTimeProperty.floatValue;
            float elapsedTime = elapsedTimeProperty.floatValue;
            bool repeatAction = repeatActionProperty.boolValue;

            Rect labelRect = new Rect(position)
            {
                width = EditorGUIUtility.labelWidth,
                height = EditorGUIUtility.singleLineHeight
            };
            Rect activateTimeRect = new Rect(labelRect)
            {
                x = labelRect.xMax,
                width = position.width - EditorGUIUtility.labelWidth
            };
            Rect advancePropertyRect = new Rect(position)
            {
                x = position.x + indentWidth,
                y = position.y + EditorGUIUtility.singleLineHeight,
                height = EditorGUIUtility.singleLineHeight
            };

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.BeginChangeCheck();
            {
                activateTime = EditorGUI.DelayedFloatField(activateTimeRect, activateTime);
            }
            if (EditorGUI.EndChangeCheck())
            {
                activateTimeProperty.floatValue = activateTime;
            }

            showAdvanceProperty = EditorGUI.BeginFoldoutHeaderGroup(advancePropertyRect, showAdvanceProperty, "AdvanceProperty");
            if (showAdvanceProperty)
            {
                float advancePropertyLeftPos = position.x + indentWidth * 2;
                Rect repeatActionLabelRect = new Rect(advancePropertyRect)
                {
                    x = advancePropertyLeftPos,
                    y = advancePropertyRect.y + EditorGUIUtility.singleLineHeight,
                    width = EditorGUIUtility.labelWidth - advancePropertyLeftPos
                };
                Rect repeatActionValueRect = new Rect(repeatActionLabelRect)
                {
                    x = repeatActionLabelRect.xMax,
                    width = EditorGUIUtility.fieldWidth
                };
                Rect elapsedTimeSliderRect = new Rect(repeatActionLabelRect)
                {
                    y = repeatActionValueRect.y + EditorGUIUtility.singleLineHeight,
                    width = position.width - advancePropertyLeftPos
                };

                EditorGUI.LabelField(repeatActionLabelRect, "Repeat Action");
                repeatAction = EditorGUI.Toggle(repeatActionValueRect, repeatAction);

                elapsedTime = EditorGUI.Slider(elapsedTimeSliderRect, "Elapsed Time", elapsedTime, 0, activateTime);

                repeatActionProperty.boolValue = repeatAction;
                elapsedTimeProperty.floatValue = elapsedTime;
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float propertyHeight = EditorGUIUtility.singleLineHeight * 2;
        if (showAdvanceProperty)
        {
            propertyHeight = EditorGUIUtility.singleLineHeight * 4;
        }

        return propertyHeight;
    }
}
#endif
