using DreadScripts.BlendTreeBulder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using static DreadScripts.BlendTreeBulder.BlendTreeBuilderMain;
using static DreadScripts.BlendTreeBulder.BlendTreeBuilderHelper;
using System;
using UnityEditor;
using System.Linq;
using Codice.Client.Common;
using Unity.Plastic.Antlr3.Runtime.Tree;
using NUnit.Framework;
using System.Drawing;

public class BlendTreeBuilderBuilder
{
    public static string getPathOfComponent(Component target, GameObject avatar)
    {
        string path = "";
        Transform transform = target.transform;
        if (avatar == transform) { return ""; }
        while (transform.parent != null && transform.parent != avatar)
        {
            path = transform.name + "/" + path;
            transform = transform.parent;
        }
        return path.Substring(0, path.Length - 1);
    }
    public static void BlendTreeBuilderBuild(BlendTreeBuilderData blendTreeBuilderData, VRCAvatarDescriptor avi)
    {
        var mainBlendTree = GetOrGenerateMasterBlendTree(avi);

        for (int i = 0; i < blendTreeBuilderData.togglesList.Length; i++)
        {
            var fx = avi.GetPlayableLayer(VRCAvatarDescriptor.AnimLayerType.FX);


            AnimationClip toggleAnim = new AnimationClip() { wrapMode = WrapMode.Clamp, name = null, };

            for (int j = 0; j < blendTreeBuilderData.togglesList[i].meshAndAttributes.Length; j++)
            {
                


                if (String.IsNullOrWhiteSpace(blendTreeBuilderData.togglesList[i].meshAndAttributes[j].customMeshAttribute))
                {
                    if (String.IsNullOrWhiteSpace(toggleAnim.name))
                    {
                        toggleAnim.name = blendTreeBuilderData.togglesList[i].meshAndAttributes[j].skinnedMeshRenderer.name + " BTBToggle";
                    }
                    AnimationCurve newAnimationCurve = AnimationCurve.Linear(0, 0, 1 / 60f, 1);
                    toggleAnim.SetCurve(getPathOfComponent(blendTreeBuilderData.togglesList[i].meshAndAttributes[j].skinnedMeshRenderer, avi.gameObject), typeof(SkinnedMeshRenderer), "m_Enabled", newAnimationCurve);
                }
                else if (blendTreeBuilderData.togglesList[i].meshAndAttributes[j].customMeshAttribute.Contains("blendShape"))
                {
                    if (String.IsNullOrWhiteSpace(toggleAnim.name))
                    {
                        toggleAnim.name = blendTreeBuilderData.togglesList[i].meshAndAttributes[j].skinnedMeshRenderer.name + blendTreeBuilderData.togglesList[i].meshAndAttributes[j].customMeshAttribute + " BTBToggle";
                    }
                    AnimationCurve newAnimationCurve = AnimationCurve.Linear(0, 0, 1 / 60f, 100);
                    toggleAnim.SetCurve(getPathOfComponent(blendTreeBuilderData.togglesList[i].meshAndAttributes[j].skinnedMeshRenderer, avi.gameObject), typeof(SkinnedMeshRenderer), blendTreeBuilderData.togglesList[i].meshAndAttributes[j].customMeshAttribute, newAnimationCurve);
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(toggleAnim.name))
                    {
                        toggleAnim.name = blendTreeBuilderData.togglesList[i].meshAndAttributes[j].skinnedMeshRenderer.name + blendTreeBuilderData.togglesList[i].meshAndAttributes[j].customMeshAttribute + " BTBToggle";
                    }
                    AnimationCurve newAnimationCurve = AnimationCurve.Linear(0, 0, 1 / 60f, 1);
                    toggleAnim.SetCurve(getPathOfComponent(blendTreeBuilderData.togglesList[i].meshAndAttributes[j].skinnedMeshRenderer, avi.gameObject), typeof(SkinnedMeshRenderer), blendTreeBuilderData.togglesList[i].meshAndAttributes[j].customMeshAttribute, newAnimationCurve);

                }
            }

            for (int j = 0; j < blendTreeBuilderData.togglesList[i].components.Length; j++)
            {
                if (String.IsNullOrWhiteSpace(toggleAnim.name))
                {
                    toggleAnim.name = blendTreeBuilderData.togglesList[i].components[j].name + " BTBToggle";
                }
                AnimationCurve newAnimationCurve = AnimationCurve.Linear(0, 0, 1 / 60f, 1);

                if (blendTreeBuilderData.togglesList[i].components[j].GetType() == typeof(Transform))
                {
                    toggleAnim.SetCurve(getPathOfComponent(blendTreeBuilderData.togglesList[i].components[j], avi.gameObject), blendTreeBuilderData.togglesList[i].components[j].GetType(), "m_IsActive", newAnimationCurve);
                }
                else
                {
                    toggleAnim.SetCurve(getPathOfComponent(blendTreeBuilderData.togglesList[i].components[j], avi.gameObject), blendTreeBuilderData.togglesList[i].components[j].GetType(), "m_Enabled", newAnimationCurve);
                }
            }

            if (String.IsNullOrWhiteSpace(toggleAnim.name))
            {
                Debug.LogError("Toggle " + (i + 1).ToString() + " has no components to toggle!");
                continue;
            }

            AnimatorControllerParameter newParam = new AnimatorControllerParameter()
            {
                name = "BTB/" + toggleAnim.name,
                type = AnimatorControllerParameterType.Float,
            };

            bool paramAlreadyExists = false;
            foreach (AnimatorControllerParameter existingParam in fx.parameters)
            {
                if (newParam.name == existingParam.name)
                {
                    paramAlreadyExists = true;
                    break;
                }
            }
            if (!paramAlreadyExists) 
            {
                fx.AddParameter(newParam);
            }

            string fxFilePath = AssetDatabase.GetAssetPath(fx);
            UnityEngine.Object[] subBlendTrees = AssetDatabase.LoadAllAssetsAtPath(fxFilePath);
            for (int j = 0; j < subBlendTrees.Length; j++)
            {
                if (subBlendTrees[j].GetType() == typeof(BlendTree))
                {
                    Debug.Log("It's a blendtree!");
                    if (subBlendTrees[j].name == toggleAnim.name)
                    {
                        AssetDatabase.RemoveObjectFromAsset(subBlendTrees[j]);
                    }
                }
            }
            
            AssetDatabase.SaveAssets();

            List<ChildMotion> tempChildMotions = mainBlendTree.children.ToList();
            for (int j = 0; j < mainBlendTree.children.Length; j++)
            {
                if (mainBlendTree.children[j].motion == null)
                {
                    tempChildMotions.Remove(tempChildMotions[j]);
                }
                else
                {
                    if (mainBlendTree.children[j].motion.name == toggleAnim.name)
                    {
                        tempChildMotions.Remove(tempChildMotions[j]);
                    }
                }
            }
            mainBlendTree.children = tempChildMotions.ToArray();

            
            BlendTree newToggleBlendTree = new BlendTree()
            {
                hideFlags = HideFlags.HideInHierarchy,
                blendType = BlendTreeType.Simple1D,
                blendParameter = newParam.name,
                name = toggleAnim.name,
            };
            newToggleBlendTree.AddChild(toggleAnim);
            newToggleBlendTree.AddChild(toggleAnim);
            ChildMotion[] newToggleChildMotions = newToggleBlendTree.children;
            newToggleChildMotions[0].timeScale = -1;
            newToggleBlendTree.children = newToggleChildMotions;
            mainBlendTree.AddChild(newToggleBlendTree);
            AssetDatabase.AddObjectToAsset(newToggleBlendTree, AssetDatabase.GetAssetPath(fx));
            
            var tempMainBlendTreeChildren = mainBlendTree.children;
            tempMainBlendTreeChildren[(tempMainBlendTreeChildren.Length - 1)].directBlendParameter = BlendTreeBuilderMain.WEIGHTONE_PARAMETER_NAME;
            mainBlendTree.children = tempMainBlendTreeChildren;

            ReadyAssetPath(BlendTreeBuilderWindow.GENERATED_ASSETS_PATH + "/anims");
            if (AssetDatabase.LoadAssetAtPath<Animation>(BlendTreeBuilderWindow.GENERATED_ASSETS_PATH + "/anims/" + toggleAnim.name + ".anim") != null)
            {
                AssetDatabase.DeleteAsset(BlendTreeBuilderWindow.GENERATED_ASSETS_PATH + "/anims/" + toggleAnim.name + ".anim");
            }
            AssetDatabase.CreateAsset(toggleAnim, BlendTreeBuilderWindow.GENERATED_ASSETS_PATH + "/anims/" + toggleAnim.name + ".anim");
        }
    }
}
