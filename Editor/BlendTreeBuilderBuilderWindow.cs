using DreadScripts.BlendTreeBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static DreadScripts.BlendTreeBuilder.BlendTreeBuilderHelper;
using static DreadScripts.BlendTreeBuilder.SeriBlendTreeBuilderData;

namespace DreadScripts.BlendTreeBuilder
{
    public static class BlendTreeBuilderBuilderWindow
    {
        public static void DrawBuilderWindow(this BlendTreeBuilderWindow window)
        {
            GameObject avi = BlendTreeBuilderWindow.avatar.gameObject;

            if (BlendTreeBuilderWindow.builderData == null)
            {
                SeriBlendTreeBuilderData blendTreeBuilderData = AssetDatabase.LoadAssetAtPath<SeriBlendTreeBuilderData>(BlendTreeBuilderWindow.GENERATED_ASSETS_PATH + "/BlendTreeBuilderData.asset");
                if (blendTreeBuilderData == null)
                {
                    ReadyAssetPath(BlendTreeBuilderWindow.GENERATED_ASSETS_PATH, "BlendTreeBuilderData.asset");
                    blendTreeBuilderData = ScriptableObject.CreateInstance<SeriBlendTreeBuilderData>();
                    AssetDatabase.CreateAsset(blendTreeBuilderData, BlendTreeBuilderWindow.GENERATED_ASSETS_PATH + "/BlendTreeBuilderData.asset");
                }
                BlendTreeBuilderWindow.builderData = blendTreeBuilderData;
            }

            SeriBlendTreeBuilderData builderData = BlendTreeBuilderWindow.builderData;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            int oldToggleLength = builderData.togglesList.Length;
            int userArrayLength = EditorGUILayout.IntField(new GUIContent("Toggles"), builderData.togglesList.Length);
            if (GUILayout.Button("+")) { userArrayLength++; }
            if (GUILayout.Button("-")) { userArrayLength--; }
            if (userArrayLength != 0) { Array.Resize(ref builderData.togglesList, userArrayLength); }
            else { Array.Resize(ref builderData.togglesList, 1); }
            if (oldToggleLength != userArrayLength)
            {
                EditorUtility.SetDirty(builderData);
            }
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
                int oldComponentsLength = toggleList.componentPathAndTypes.Length;
                int userComponentArrayLength = EditorGUILayout.IntField(new GUIContent("Components"), toggleList.componentPathAndTypes.Length);
                if (GUILayout.Button("+")) { userComponentArrayLength++; }
                if (GUILayout.Button("-")) { userComponentArrayLength--; }
                if (userComponentArrayLength > -1) { Array.Resize(ref toggleList.componentPathAndTypes, userComponentArrayLength); }
                else { Array.Resize(ref toggleList.componentPathAndTypes, 0); }
                if (oldComponentsLength != userComponentArrayLength)
                {
                    EditorUtility.SetDirty(builderData);
                }
                GUILayout.EndHorizontal();


                for (int j = 0; j < toggleList.componentPathAndTypes.Length; j++)
                {
                    
                    if (toggleList.componentPathAndTypes[j] == null)
                    {
                        toggleList.componentPathAndTypes[j] = new PathAndType();
                    }
                    Component component = (Component)toggleList.componentPathAndTypes[j].Lookup(avi);

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.BeginChangeCheck();
                    component = (Component)EditorGUILayout.ObjectField(new GUIContent("Component"), component, typeof(Component), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (component == null)
                        {
                            toggleList.componentPathAndTypes[j].componentPath = null;
                            toggleList.componentPathAndTypes[j].componentType = null;
                        }
                        else
                        {
                            toggleList.componentPathAndTypes[j].componentPath = AnimationUtility.CalculateTransformPath(component.transform, avi.transform);
                            toggleList.componentPathAndTypes[j].componentType = component.GetType().AssemblyQualifiedName;
                        }
                        EditorUtility.SetDirty(builderData);
                    }

                    EditorGUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.BeginHorizontal();
                int oldMeshLength = toggleList.meshAndAttributes.Length;
                int userMeshArrayLength = EditorGUILayout.IntField(new GUIContent("Meshes"), toggleList.meshAndAttributes.Length);
                if (GUILayout.Button("+")) { userMeshArrayLength++; }
                if (GUILayout.Button("-")) { userMeshArrayLength--; }
                GUILayout.EndHorizontal();
                if (userMeshArrayLength > -1) { Array.Resize(ref toggleList.meshAndAttributes, userMeshArrayLength); }
                else { Array.Resize(ref toggleList.meshAndAttributes, 0); }
                if (oldMeshLength != userComponentArrayLength)
                {
                    EditorUtility.SetDirty(builderData);
                }

                for (int j = 0; j < toggleList.meshAndAttributes.Length; j++)
                {
                    if (toggleList.meshAndAttributes[j] == null)
                    {
                        toggleList.meshAndAttributes[j] = new MeshAndAttribute();
                    }
                    SkinnedMeshRenderer skinnedMesh = (SkinnedMeshRenderer)toggleList.meshAndAttributes[j].Lookup(avi);


                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUI.BeginChangeCheck();
                    skinnedMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(new GUIContent("Mesh " + (j + 1)), skinnedMesh, typeof(SkinnedMeshRenderer), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if(skinnedMesh == null)
                        {
                            toggleList.meshAndAttributes[j].skinnedMeshRendererPath = null;
                        }
                        else
                        {
                            toggleList.meshAndAttributes[j].skinnedMeshRendererPath = AnimationUtility.CalculateTransformPath(skinnedMesh.transform, avi.transform);
                        }
                        EditorUtility.SetDirty(builderData);
                    }
                    EditorGUI.BeginChangeCheck();
                    toggleList.meshAndAttributes[j].customMeshAttributes = EditorGUILayout.TextField(new GUIContent("Mesh Attribute"), toggleList.meshAndAttributes[j].customMeshAttributes);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(builderData);
                    }

                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("GENERATE"))
            {
                BlendTreeBuilderBuilder.BlendTreeBuilderBuild(builderData, BlendTreeBuilderWindow.avatar);
            }
        }
    }
}
