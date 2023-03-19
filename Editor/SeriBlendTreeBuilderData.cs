using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace DreadScripts.BlendTreeBuilder
{
    public class SeriBlendTreeBuilderData : ScriptableObject
    {
        [Serializable]
        public class MeshAndAttribute
        {
            public string skinnedMeshRendererPath;
            public string customMeshAttributes;

            public SkinnedMeshRenderer Lookup(GameObject avatar)
            {
                if (skinnedMeshRendererPath == "" || skinnedMeshRendererPath == null)
                {
                    return null;
                }
                string avatarPath = avatar.transform.GetPathToTransform();
                string absolutePath = avatarPath + "/" + skinnedMeshRendererPath;
                GameObject componentObject = GameObject.Find(absolutePath);
                if (componentObject == null)
                {
                    //Debug.LogError("Object not found at " + absolutePath);
                    return null;
                }
                return componentObject.GetComponent<SkinnedMeshRenderer>();

            }
            

        }

        [Serializable]
        public class PathAndType
        {
            public string componentPath;
            public string componentType;
            public object Lookup(GameObject avatar)
            {
                if (componentType == "" || componentType == null)
                {
                    return null;
                }
                string avatarPath = avatar.transform.GetPathToTransform();
                string absolutePath = avatarPath + "/" + componentPath;
                GameObject componentObject = GameObject.Find(absolutePath);
                if (componentObject == null)
                {
                    //Debug.LogError("Object not found at " + absolutePath);
                    return null;
                }
                return componentObject.GetComponent(Type.GetType(componentType));

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
    }

}

