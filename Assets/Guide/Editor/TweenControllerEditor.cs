/*
 * Author Leon kim
 * 
*/

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TweenController), true)]
public class TweenControllerEditor : Editor
{
    SerializedProperty delayTime;
    SerializedProperty time;

    SerializedProperty isUseAddtionalParam;
    SerializedProperty playOnStart;
    SerializedProperty animGameObj;

    SerializedProperty xCurve;
    SerializedProperty yCurve;
    SerializedProperty fromPos;
    SerializedProperty toPos;

    SerializedProperty offsetCurve;
    SerializedProperty offset;

    SerializedProperty scaleCurve;
    SerializedProperty fromScale;
    SerializedProperty toScale;

    SerializedProperty rotateCurve;
    SerializedProperty fromRotate;
    SerializedProperty toRotate;

    SerializedProperty alphaCurve;
    SerializedProperty fromAlpha;
    SerializedProperty toAlpha;

    protected void OnEnable()
    {
        delayTime = serializedObject.FindProperty("delayTime");
        time = serializedObject.FindProperty("time");

        xCurve = serializedObject.FindProperty("xCurve");
        yCurve = serializedObject.FindProperty("yCurve");
        fromPos = serializedObject.FindProperty("fromPos");
        toPos = serializedObject.FindProperty("toPos");

        offsetCurve = serializedObject.FindProperty("offsetCurve");
        offset = serializedObject.FindProperty("offset");

        scaleCurve = serializedObject.FindProperty("scaleCurve");
        fromScale = serializedObject.FindProperty("fromScale");
        toScale = serializedObject.FindProperty("toScale");

        rotateCurve = serializedObject.FindProperty("rotateCurve");
        fromRotate = serializedObject.FindProperty("fromRotate");
        toRotate = serializedObject.FindProperty("toRotate");

        alphaCurve = serializedObject.FindProperty("alphaCurve");
        fromAlpha = serializedObject.FindProperty("fromAlpha");
        toAlpha = serializedObject.FindProperty("toAlpha");

        playOnStart = serializedObject.FindProperty("playOnStart");
        animGameObj = serializedObject.FindProperty("animGameObj");
    }

    public override void OnInspectorGUI()
    {
        TweenController tw = target as TweenController;

        //serializedObject.Update();
        EditorGUILayout.PropertyField(delayTime);
        EditorGUILayout.PropertyField(time);

        EditorGUI.indentLevel = 0;
        tw.isUseAdditionalParam = EditorGUILayout.ToggleLeft("自定义物体", tw.isUseAdditionalParam);
        if (tw.isUseAdditionalParam)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(playOnStart);
            EditorGUILayout.PropertyField(animGameObj);
        }

        EditorGUI.indentLevel = 0;
        tw.moveType = (TweenType)EditorGUILayout.EnumPopup("移动 (Move)", (System.Enum)tw.moveType);
        if (tw.moveType != TweenType.None)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(xCurve);
            EditorGUILayout.PropertyField(yCurve);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(fromPos);
            if (GUILayout.Button("reset"))
            {
                try
                {
                    tw.fromPos = tw.animGameObj.transform.GetComponent<RectTransform>().anchoredPosition;
                }
                catch
                {
                    throw new System.Exception("此按钮仅 自定义物体时 可用, 若需自定义请指定物体到 Anim Game Obj");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(toPos);
            if (GUILayout.Button("reset"))
            {
                try
                {
                    tw.toPos = tw.animGameObj.transform.GetComponent<RectTransform>().anchoredPosition;
                }
                catch
                {
                    throw new System.Exception("此按钮仅 自定义物体时 可用, 若需自定义请指定物体到 Anim Game Obj");
                }
            }
            EditorGUILayout.EndHorizontal();

            tw.offsetType = (TweenType)EditorGUILayout.EnumPopup("偏移 (offset)", (System.Enum)tw.offsetType);
            if (tw.offsetType != TweenType.None)
            {
                EditorGUILayout.PropertyField(offsetCurve);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(offset);
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUI.indentLevel = 0;
        tw.scaleType = (TweenType)EditorGUILayout.EnumPopup("缩放 (Scale)", (System.Enum)tw.scaleType);
        if (tw.scaleType != TweenType.None)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(scaleCurve);
            EditorGUILayout.PropertyField(fromScale);
            EditorGUILayout.PropertyField(toScale);

        }
        EditorGUI.indentLevel = 0;
        tw.rotateType = (TweenType)EditorGUILayout.EnumPopup("旋转 (Rotate)", (System.Enum)tw.rotateType);
        if (tw.rotateType != TweenType.None)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(rotateCurve);
            EditorGUILayout.PropertyField(fromRotate);
            EditorGUILayout.PropertyField(toRotate);
        }
        EditorGUI.indentLevel = 0;
        tw.alphaType = (TweenType)EditorGUILayout.EnumPopup("透明 (Alpha)", (System.Enum)tw.alphaType);
        if (tw.alphaType != TweenType.None)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(alphaCurve);
            EditorGUILayout.PropertyField(fromAlpha);
            EditorGUILayout.PropertyField(toAlpha);
        }

        serializedObject.ApplyModifiedProperties();
    }

}
