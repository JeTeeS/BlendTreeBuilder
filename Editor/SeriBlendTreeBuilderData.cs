using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static SeriBlendTreeBuilderData;

public class BlendTreeBuilderData
{
    public class MeshAndAttribute
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public string customMeshAttribute;
    }

    public class ToggleList
    {
        public Component[] components;
        public MeshAndAttribute[] meshAndAttributes;

        public ToggleList()
        {
            this.components = new Component[0];
            this.meshAndAttributes = new MeshAndAttribute[0];
        }
    }
    public ToggleList[] togglesList;

    public BlendTreeBuilderData()
    {
        this.togglesList = new ToggleList[0];
    }

    public SeriBlendTreeBuilderData SerializeBlendTreeData(GameObject avatar)
    {
        SeriBlendTreeBuilderData scriptableObject = ScriptableObject.CreateInstance<SeriBlendTreeBuilderData>();

        scriptableObject.togglesList = new SeriBlendTreeBuilderData.ToggleList[togglesList.Length];
        for (int i = 0; i < togglesList.Length; i++)
        {
            ToggleList oldToggleList = togglesList[i];
            SeriBlendTreeBuilderData.ToggleList newToggleList = new SeriBlendTreeBuilderData.ToggleList();
            newToggleList.componentPathAndTypes = new PathAndType[oldToggleList.components.Length];
            for(int j = 0; j < oldToggleList.components.Length; j++)
            {
                if (oldToggleList.components[j] == null)
                {
                    newToggleList.componentPathAndTypes[j] = new PathAndType()
                    {
                        componentPath = null,
                        componentType = null,
                    };
                    continue;
                }
                string path = AnimationUtility.CalculateTransformPath(oldToggleList.components[j].transform, avatar.transform);
                if (path == null)
                {
                    Debug.LogError("Invalid Path");
                }
                else
                {
                    newToggleList.componentPathAndTypes[j] = new PathAndType()
                    {
                        componentPath = path,
                        componentType = oldToggleList.components[j].GetType().AssemblyQualifiedName,
                    };
                }

            }

            newToggleList.meshAndAttributes = new SeriBlendTreeBuilderData.MeshAndAttribute[oldToggleList.meshAndAttributes.Length];

            for (int j = 0; j < oldToggleList.meshAndAttributes.Length; j++)
            {

                if (oldToggleList.meshAndAttributes[j] == null || oldToggleList.meshAndAttributes[j].skinnedMeshRenderer == null)
                {
                    newToggleList.meshAndAttributes[j] = new SeriBlendTreeBuilderData.MeshAndAttribute()
                    {
                        skinnedMeshRendererPath = null,
                        customMeshAttributes = null,
                    };
                    continue;
                }


                string path = AnimationUtility.CalculateTransformPath(oldToggleList.meshAndAttributes[j].skinnedMeshRenderer.transform, avatar.transform);
                if (path == null)
                {
                    Debug.LogError("Invalid Path");
                }
                else
                {
                    newToggleList.meshAndAttributes[j] = new SeriBlendTreeBuilderData.MeshAndAttribute()
                    {
                        skinnedMeshRendererPath = path,
                        customMeshAttributes = oldToggleList.meshAndAttributes[j].customMeshAttribute,
                    };
                }
            }
            scriptableObject.togglesList[i] = newToggleList;

        }
        return scriptableObject;
    }
}


public class SeriBlendTreeBuilderData : ScriptableObject
{
    [Serializable]
    public class MeshAndAttribute
    {
        public string skinnedMeshRendererPath;
        public string customMeshAttributes;

        public SkinnedMeshRenderer Lookup(GameObject avatar)
        {
            string avatarPath = GetGameObjectPath(avatar.transform);
            string absolutePath = avatarPath + "/" + skinnedMeshRendererPath;
            GameObject componentObject = GameObject.Find(absolutePath);
            if (componentObject == null)
            {
                Debug.LogError("Object not found at " + absolutePath);
                return null;
            }
            return componentObject.GetComponent<SkinnedMeshRenderer>();

        }
        private static string GetGameObjectPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }

    }
    [Serializable]
    public class PathAndType
    {
        public string componentPath;
        public string componentType;
        public object Lookup(GameObject avatar) 
        {
            if(componentType == "")
            {
                return null;
            }
            string avatarPath = GetGameObjectPath(avatar.transform);
            string absolutePath = avatarPath + "/" + componentPath;
            GameObject componentObject = GameObject.Find(absolutePath);
            if (componentObject == null)
            {
                Debug.LogError("Object not found at " + absolutePath);
                return null;
            }
            return componentObject.GetComponent(Type.GetType(componentType));

        }
        private static string GetGameObjectPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }

    }

    [Serializable]
    public class ToggleList
    {
        public PathAndType[] componentPathAndTypes;
        public MeshAndAttribute[] meshAndAttributes;

        public ToggleList()
        {
            this.componentPathAndTypes = new PathAndType[0];
            this.meshAndAttributes = new MeshAndAttribute[0];
        }
    }
    public ToggleList[] togglesList;

    public SeriBlendTreeBuilderData()
    {
        this.togglesList = new ToggleList[0];
    }

    public BlendTreeBuilderData DeserializeBlendTreeData(GameObject avatar)
    {
        BlendTreeBuilderData dataObject = new BlendTreeBuilderData();

        dataObject.togglesList = new BlendTreeBuilderData.ToggleList[togglesList.Length];
        for (int i = 0; i < togglesList.Length; i++)
        {
            ToggleList oldToggleList = togglesList[i];
            BlendTreeBuilderData.ToggleList newToggleList = new BlendTreeBuilderData.ToggleList();
            newToggleList.components = new Component[oldToggleList.componentPathAndTypes.Length];
            for (int j = 0; j < oldToggleList.componentPathAndTypes.Length; j++)
            {
                newToggleList.components[j] = oldToggleList.componentPathAndTypes[j].Lookup(avatar) as Component;

            }

            newToggleList.meshAndAttributes = new BlendTreeBuilderData.MeshAndAttribute[oldToggleList.meshAndAttributes.Length];

            for (int j = 0; j < newToggleList.meshAndAttributes.Length; j++)
            {
                newToggleList.meshAndAttributes[j] = new BlendTreeBuilderData.MeshAndAttribute()
                {
                    skinnedMeshRenderer = oldToggleList.meshAndAttributes[j].Lookup(avatar),
                    customMeshAttribute = oldToggleList.meshAndAttributes[j].customMeshAttributes,
                };
            }
            dataObject.togglesList[i] = newToggleList;

        }
        return dataObject;
    }
}

