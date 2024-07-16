using System;
using System.Collections.Generic;
using System.Linq;
using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using AAA_NewSaveSystem.Scripts.SaveSystem.UnityComponentsData;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace AAA_NewSaveSystem.Scripts.SaveSystem
{
    public static class ComponentSerializer
    {
        public static ComponentData SerializeComponent(Component component)
        {
            ComponentData data = new ComponentData
            {
                typeName = component.GetType().AssemblyQualifiedName,
                instanceId = component.GetInstanceID(),
            };

            switch (component)
            {
                case Transform transform:
                    TransformData transformData = new TransformData()
                    {
                        LocalPosition = transform.localPosition,
                        LocalRotation = transform.localRotation,
                        LocalScale = transform.localScale,
                    };
                    data.jsonData = JsonUtility.ToJson(transformData);

                    break;
                default:
                    try
                    {
                        data.jsonData = JsonUtility.ToJson(component);
                    }
                    catch
                    {
                        // ignored
                    }

                    break;
            }

            return data;
        }

        public static void DeserializeComponent(GameObject go, ComponentData data)
        {
            Type type = Type.GetType(data.typeName);

            if (type == null) return;

            if (type == typeof(Transform))
            {
                TransformData transformData = new TransformData();
                transformData = (TransformData)JsonUtility.FromJson(data.jsonData, transformData.GetType());
                go.transform.localPosition = transformData.LocalPosition;
                go.transform.localRotation = transformData.LocalRotation;
                go.transform.localScale = transformData.LocalScale;
                RootSaver.AddObject(go.transform, data.instanceId);

                return;
            }

            Component component = go.AddComponent(type);
            RootSaver.AddObject(component, data.instanceId);

            RootSaver.ObjectsCreated += () =>
            {
                try
                {
                    JObject jsonObject = JObject.Parse(data.jsonData);
                    ReplaceInstanceIDWithZero(jsonObject);
                    JsonUtility.FromJsonOverwrite(jsonObject.ToString(), component);
                }
                catch
                {
                    // ignored
                }
            };
        }
        
        static void ReplaceInstanceIDWithZero(JToken token)
        {
            if (token is JProperty jProperty && jProperty.Name == "instanceID")
            {
                jProperty.Value = RootSaver.GetCurrentObjectIDByPreviousID(Convert.ToInt32(jProperty.Value.ToString()));
            }

            if (token is JProperty property)
            {
                foreach (var child in token.Children())
                {
                    ReplaceInstanceIDWithZero(child);
                }
            }
            else if (token is JObject)
            {
                foreach (var child in token.Children<JProperty>())
                {
                    ReplaceInstanceIDWithZero(child);
                }
            }
            else if (token is JArray)
            {
                foreach (var child in token.Children())
                {
                    ReplaceInstanceIDWithZero(child);
                }
            }
        }
    }
}