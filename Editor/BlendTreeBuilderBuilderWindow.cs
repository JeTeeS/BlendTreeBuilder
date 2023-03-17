using DreadScripts.BlendTreeBulder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static BlendTreeBuilderData;
using static DreadScripts.BlendTreeBulder.BlendTreeBuilderHelper;
using static BlendTreeBuilderBuilder;


public static class BlendTreeBuilderBuilderWindow
{
    public static void DrawBuilderWindow(this BlendTreeBuilderWindow window)
    {
        
        if (BlendTreeBuilderWindow.builderData == null)
        {
            SeriBlendTreeBuilderData blendTreeBuilderData = AssetDatabase.LoadAssetAtPath<SeriBlendTreeBuilderData>(BlendTreeBuilderWindow.GENERATED_ASSETS_PATH + "/BlendTreeBuilderData.asset");
            if (blendTreeBuilderData == null)
            {
                ReadyAssetPath(BlendTreeBuilderWindow.GENERATED_ASSETS_PATH, "BlendTreeBuilderData.asset");
                blendTreeBuilderData = ScriptableObject.CreateInstance<SeriBlendTreeBuilderData>();
                AssetDatabase.CreateAsset(blendTreeBuilderData, BlendTreeBuilderWindow.GENERATED_ASSETS_PATH + "/BlendTreeBuilderData.asset");
            }
            BlendTreeBuilderWindow.builderData = blendTreeBuilderData.DeserializeBlendTreeData(BlendTreeBuilderWindow.avatar.gameObject);
        }

        BlendTreeBuilderData builderData = BlendTreeBuilderWindow.builderData;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.BeginHorizontal();
        int userArrayLength = EditorGUILayout.IntField(new GUIContent("Toggles"), builderData.togglesList.Length);
        if (GUILayout.Button("+")) { userArrayLength++; }
        if (GUILayout.Button("-")) { userArrayLength--; }
        if (userArrayLength != 0) { Array.Resize(ref builderData.togglesList, userArrayLength); }
        else { Array.Resize(ref builderData.togglesList, 1); }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < builderData.togglesList.Length; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Toggle " + (i + 1).ToString());
            if (builderData.togglesList[i] == null)
            {
                builderData.togglesList[i] = new ToggleList();
            }
            ToggleList toggleList = builderData.togglesList[i];

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            int userComponentArrayLength = EditorGUILayout.IntField(new GUIContent("Components"), toggleList.components.Length);
            if (GUILayout.Button("+")) { userComponentArrayLength++; }
            if (GUILayout.Button("-")) { userComponentArrayLength--; }
            if (userComponentArrayLength > -1) { Array.Resize(ref toggleList.components, userComponentArrayLength); }
            else { Array.Resize(ref toggleList.components, 0); }
            GUILayout.EndHorizontal();


            for (int j = 0; j < toggleList.components.Length; j++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                Component component = toggleList.components[j];

                toggleList.components[j] = (Component)EditorGUILayout.ObjectField(new GUIContent("Component") ,component, typeof(Component), true);
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            int userMeshArrayLength = EditorGUILayout.IntField(new GUIContent("Meshes"), toggleList.meshAndAttributes.Length);
            if (GUILayout.Button("+")) { userMeshArrayLength++; }
            if (GUILayout.Button("-")) { userMeshArrayLength--; }
            GUILayout.EndHorizontal();
            if (userMeshArrayLength > -1) { Array.Resize(ref toggleList.meshAndAttributes, userMeshArrayLength); }
            else { Array.Resize(ref toggleList.meshAndAttributes, 0); }

            for (int j = 0; j < toggleList.meshAndAttributes.Length; j++)
            {
                if (toggleList.meshAndAttributes[j] == null)
                {
                    toggleList.meshAndAttributes[j] = new MeshAndAttribute();
                }
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                toggleList.meshAndAttributes[j].skinnedMeshRenderer = (SkinnedMeshRenderer) EditorGUILayout.ObjectField(new GUIContent("Mesh " + (j+1)), toggleList.meshAndAttributes[j].skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true);
                toggleList.meshAndAttributes[j].customMeshAttribute = EditorGUILayout.TextField(new GUIContent("Mesh Attribute"), toggleList.meshAndAttributes[j].customMeshAttribute);
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("GENERATE"))
        {
            AssetDatabase.CreateAsset(builderData.SerializeBlendTreeData(BlendTreeBuilderWindow.avatar.gameObject), BlendTreeBuilderWindow.GENERATED_ASSETS_PATH + "/BlendTreeBuilderData.asset");
            BlendTreeBuilderBuild(builderData, BlendTreeBuilderWindow.avatar);
        }
        

            

    }
}
