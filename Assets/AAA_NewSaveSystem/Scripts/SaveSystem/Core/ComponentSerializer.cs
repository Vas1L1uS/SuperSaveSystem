using System;
using AAA_NewSaveSystem.Scripts.SaveSystem.UnityComponentsData;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.SaveSystem.Core
{
    public static class ComponentSerializer
    {
        public static ComponentData SerializeComponent(Component component)
        {
            ComponentData data = new()
            {
                typeName = component.GetType().AssemblyQualifiedName,
                instanceId = component.GetInstanceID(),
            };

            switch (component)
            {
                case Transform transform:
                    int parentInstanceId = 0;

                    if (transform.parent != null) parentInstanceId = transform.parent.gameObject.GetInstanceID();

                    TransformData transformData = new()
                    {
                        LocalPosition = transform.localPosition,
                        LocalRotation = transform.localRotation,
                        LocalScale = transform.localScale,
                        ParentInstanceId = parentInstanceId,
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

        public static void DeserializeComponent(ComponentData data, Component component)
        {
            JObject jsonObject = JObject.Parse(data.jsonData);
            ReplaceInstanceIDWithZero(jsonObject);
            JsonUtility.FromJsonOverwrite(jsonObject.ToString(), component);
        }
        
        static void ReplaceInstanceIDWithZero(JToken token)
        {
            if (token is JProperty jProperty && jProperty.Name == "instanceID")
            {
                jProperty.Value = RootSaver.GetCurrentInstanceIDByPreviousInstanceId(Convert.ToInt32(jProperty.Value.ToString()));
            }

            if (token is JProperty)
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