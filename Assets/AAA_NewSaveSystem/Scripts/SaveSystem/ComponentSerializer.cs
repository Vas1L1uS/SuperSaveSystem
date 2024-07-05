using System;
using System.Collections.Generic;
using System.Linq;
using AAA_NewSaveSystem.Scripts.UnityComponentsData;
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
                // case Collider collider:
                //     ColliderData colliderData = new ColliderData()
                //     {
                //         Enabled = collider.enabled,
                //         IsTrigger = collider.isTrigger
                //     };
                //     data.jsonData = JsonUtility.ToJson(colliderData);
                //
                //     break;
                // case Rigidbody rigidbody:
                //     RigidbodyData rigidbodyData = new RigidbodyData()
                //     {
                //         Velocity = rigidbody.velocity,
                //         AngularVelocity = rigidbody.angularVelocity,
                //     };
                //     data.jsonData = JsonUtility.ToJson(rigidbodyData);
                //
                //     break;
                case MeshRenderer meshRenderer:
                    MeshRendererData meshRendererData = new MeshRendererData()
                    {
                        materialDataArray = GetMaterialDataArray(meshRenderer),
                    };
                    
                    data.jsonData = JsonUtility.ToJson(meshRendererData);
                
                    List<MaterialData> GetMaterialDataArray(MeshRenderer mr)
                    {
                        MaterialData[] materialDataArray = new MaterialData[mr.materials.Length];

                        for (int i = 0; i < mr.materials.Length; i++)
                        {
                            List<Color> colors = new();
                            var currentMat  = mr.materials[i];
                            materialDataArray[i].color = currentMat.color;
                            List<string> colorsNames = new();
                            
                            try
                            {
                                for (int x = 0; x < currentMat.enabledKeywords.Length; x++)
                                {
                                    var currentKeywordName = currentMat.enabledKeywords[i].name;
                                    string colorName = GetColorNameByKeyWordName(currentKeywordName);

                                    if (String.IsNullOrEmpty(colorName) == false)
                                    {
                                        colors.Add(currentMat.GetColor(colorName));
                                        colorsNames.Add(colorName);
                                    }
                                }
                            }
                            catch
                            {
                                // ignored
                            }

                            materialDataArray[i].colors = colors.ToArray();
                            materialDataArray[i].shaderName  = currentMat.shader.name;
                            materialDataArray[i].colorsNames = colorsNames.ToArray();
                            materialDataArray[i].enabledKeywords = currentMat.enabledKeywords;
                            materialDataArray[i].enableInstancing = currentMat.enableInstancing;
                            materialDataArray[i].mainTexture = currentMat.mainTexture;
                            materialDataArray[i].renderQueue = currentMat.renderQueue;
                            materialDataArray[i].shaderKeywords = currentMat.shaderKeywords;
                            materialDataArray[i].globalIlluminationFlags = currentMat.globalIlluminationFlags;
                            materialDataArray[i].mainTextureOffset = currentMat.mainTextureOffset;
                            materialDataArray[i].mainTextureScale = currentMat.mainTextureScale;
                            materialDataArray[i].doubleSidedGI = currentMat.doubleSidedGI;
                        }
                
                        return materialDataArray.ToList();
                    }
                
                    break;
                case MeshFilter meshFilter:
                    MeshFilterData meshFilterData = new MeshFilterData()
                    {
                        mesh = GetMeshDataByMesh(meshFilter.mesh),
                        sharedMesh = GetMeshDataByMesh(meshFilter.sharedMesh),
                    };
                    data.jsonData = JsonUtility.ToJson(meshFilterData);
                
                    MeshData GetMeshDataByMesh(Mesh m)
                    {
                        MeshData result = new MeshData()
                        {
                            bindposes = m.bindposes,
                            bounds = m.bounds,
                            colors = m.colors,
                            colors32 = m.colors32,
                            normals = m.normals,
                            tangents = m.tangents,
                            triangles = m.triangles,
                            uv = m.uv,
                            uv2 = m.uv2,
                            uv3 = m.uv3,
                            uv4 = m.uv4,
                            uv5 = m.uv5,
                            uv6 = m.uv6,
                            uv7 = m.uv7,
                            uv8 = m.uv8,
                            vertices = m.vertices,
                            boneWeights = m.boneWeights,
                            indexFormat = m.indexFormat,
                            indexBufferTarget = m.indexBufferTarget,
                            subMeshCount = m.subMeshCount,
                            vertexBufferTarget = m.vertexBufferTarget,
                            name = m.name,
                        };
                
                        return result;
                    }
                    break;
                default:
                    try
                    {
                        data.jsonData = JsonUtility.ToJson(component);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Component not serialized: {e}");
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

            switch (component)
            {
                // case Collider collider:
                //     ColliderData colliderData = new ColliderData();
                //     colliderData = (ColliderData)JsonUtility.FromJson(data.jsonData, colliderData.GetType());
                //     
                //         collider.enabled = colliderData.Enabled;
                //         collider.isTrigger = colliderData.IsTrigger;
                //     
                //     break;
                // case Rigidbody rigidbody:
                //     RigidbodyData rigidbodyData = new RigidbodyData();
                //     rigidbodyData = (RigidbodyData)JsonUtility.FromJson(data.jsonData, rigidbodyData.GetType());
                //     
                //         rigidbody.velocity = rigidbodyData.Velocity;
                //         rigidbody.angularVelocity = rigidbodyData.AngularVelocity;
                //     
                //     break;
                case MeshRenderer meshRenderer:
                    MeshRendererData meshRendererData = new MeshRendererData()
                    {
                        materialDataArray = new(),
                    };
                    meshRendererData = (MeshRendererData)JsonUtility.FromJson(data.jsonData, meshRendererData.GetType());
                    
                        Material[] materials = new Material[meshRendererData.materialDataArray.Count];
                
                        for (int i = 0; i < meshRendererData.materialDataArray.Count; i++)
                        {
                            var currentMat = meshRendererData.materialDataArray[i];
                            var shader = Shader.Find(meshRendererData.materialDataArray[i].shaderName);
                            
                            var newMat = new Material(shader)
                            {
                                name = "AAA",
                                color = currentMat.color,
                                enableInstancing = currentMat.enableInstancing,
                                mainTexture = currentMat.mainTexture,
                                renderQueue = currentMat.renderQueue,
                                shaderKeywords = currentMat.shaderKeywords,
                                globalIlluminationFlags = currentMat.globalIlluminationFlags,
                                mainTextureOffset = currentMat.mainTextureOffset,
                                mainTextureScale = currentMat.mainTextureScale,
                                doubleSidedGI = currentMat.doubleSidedGI,
                            };

                            for (int j = 0; j < currentMat.colorsNames.Length; j++)
                            {
                                string name = currentMat.colorsNames[i];
                                
                                try
                                {
                                    newMat.SetColor(GetColorNameByKeyWordName(name), currentMat.colors[j]);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                            
                            materials[i] = newMat;
                        }
                    
                    meshRenderer.materials = materials;
                    break;
                case MeshFilter meshFilter:
                    MeshFilterData meshFilterData = new MeshFilterData();
                    meshFilterData = (MeshFilterData)JsonUtility.FromJson(data.jsonData, meshFilterData.GetType());
                    
                        meshFilter.mesh = GetMeshByMeshData(meshFilterData.mesh);
                        meshFilter.sharedMesh = GetMeshByMeshData(meshFilterData.sharedMesh);
                
                        Mesh GetMeshByMeshData(MeshData m)
                        {
                            Mesh result = new Mesh
                            {
                                vertices = m.vertices,
                                bindposes = m.bindposes,
                                bounds = m.bounds,
                                colors = m.colors,
                                colors32 = m.colors32,
                                normals = m.normals,
                                tangents = m.tangents,
                                triangles = m.triangles,
                                uv = m.uv,
                                uv2 = m.uv2,
                                uv3 = m.uv3,
                                uv4 = m.uv4,
                                uv5 = m.uv5,
                                uv6 = m.uv6,
                                uv7 = m.uv7,
                                uv8 = m.uv8,
                                boneWeights = m.boneWeights,
                                indexFormat = m.indexFormat,
                                indexBufferTarget = m.indexBufferTarget,
                                subMeshCount = m.subMeshCount,
                                vertexBufferTarget = m.vertexBufferTarget,
                                name = m.name,
                            };
                
                            return result;
                        }
                    
                    break;
                default:
                    //JsonUtility.FromJsonOverwrite(data.ToString(), component);
                    RootSaver.ObjectsReady += () =>
                    {
                        try
                        {
                            JObject jsonObject = JObject.Parse(data.jsonData);
                            ReplaceInstanceIDWithZero(jsonObject);
                            JsonUtility.FromJsonOverwrite(jsonObject.ToString(), component);
                        }
                        catch
                        {
                            
                        }
                    };
                    break;
            }
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
        
        private static string GetColorNameByKeyWordName(string name)
        {
            return @name switch
            {
                "_EMISSION" => "_EmissionColor",
                _ => null
            };
        }
        
        private static string GetKeyWordNameByColorName(string name)
        {
            return @name switch
            {
                "_EmissionColor" => "_EMISSION",
                _ => null
            };
        }
    }
}