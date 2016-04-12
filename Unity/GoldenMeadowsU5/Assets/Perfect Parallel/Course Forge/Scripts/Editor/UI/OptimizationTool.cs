using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace PerfectParallel.CourseForge.UI
{
    /// <summary>
    /// Course Forge Android tool, controls optimization and building for mobile
    /// </summary>
    public class OptimizationTool
    {
        #region Methods
        //Builds scene; only needed to build Used Assets list in Editor.log
        public void build()
        {
            EditorPrefs.SetString("Course Forge/ExportBuildTarget", "Android");
            Course.BuildCourse((int)Utility.FromName<UnityEditor.BuildTarget>("Android"));
        }
        //Compares list of used assets with the asset library and moves any assets that are not being used to a path outside the library
        public int RemoveUnusedAssets()
        {
            List<String> unusedAssets = new List<String>();
            List<String> used = AnalyzeLogFile();
            used.AddRange(meshMaterials(used));
            string currentAsset;
            string newPath = "Removed Assets";
            string[] assetLibrary = AssetDatabase.FindAssets("t:Object");
            bool unused = false;
            int numRemoved = 0;

            GameObject prefab;
            bool prefabInHierarchy = false;
            List<String> hierarchyPrefabs = new List<String>();
            //Create folder for removed assets
            if (!Directory.Exists(newPath))
            {
                CreateFileStructure();
            }
            //Compile list of non-spline prefabs in hierarchy
            foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.transform.parent == null)
                {
                    if (!obj.name.Contains("spline"))
                    {
                        hierarchyPrefabs.Add(obj.name);
                    }
                }
            }
            //Check objects in asset library against list of used assets from build log to compile list of assets that are safe to remove
            for (int i = 0; i < assetLibrary.Length; i++)
            {
                currentAsset = AssetDatabase.GUIDToAssetPath(assetLibrary[i]);
                for (int index = 0; index < used.Count; index++)
                {
                    if (used.ElementAt(index).Contains(currentAsset))
                    {
                        unused = false;
                        index = used.Count;
                    }
                    else
                    {
                        unused = true;
                    }
                }
                if (unused && currentAsset.Contains(".") && !currentAsset.Contains(".dll") && !currentAsset.Contains("Camera") && !currentAsset.Contains(".js") && !currentAsset.Contains(".cs") && !currentAsset.Contains("Perfect Parallel"))
                {
                    unusedAssets.Add(currentAsset);
                }
            }

            //Moves unused assets to Removed Assets directory, generates list of all removed assets
            for (int i = 0; i < unusedAssets.Count; i++)
            {
                currentAsset = unusedAssets.ElementAt(i);
                if (File.Exists(currentAsset) && !File.Exists(newPath + "/" + currentAsset))
                {
                    if (!currentAsset.Contains("prefab") && !currentAsset.Contains(".unity"))
                    {
                        File.Move(AssetDatabase.GetTextMetaFilePathFromAssetPath(currentAsset), newPath + "/" + AssetDatabase.GetTextMetaFilePathFromAssetPath(currentAsset));
                        File.Move(currentAsset, newPath + "/" + currentAsset);
                        File.AppendAllText(newPath + "/" + "removedAssets.txt", currentAsset + "\r\n");
                        AssetDatabase.Refresh();
                        numRemoved++;
                    }
                    else if (currentAsset.Contains("prefab"))
                    {
                        prefab = AssetDatabase.LoadAssetAtPath(currentAsset, typeof(GameObject)) as GameObject;
                        for (int index = 0; index < hierarchyPrefabs.Count; index++)
                        {
                            if (hierarchyPrefabs.ElementAt(index).Contains(prefab.name))
                            {
                                index = hierarchyPrefabs.Count;
                                prefabInHierarchy = true;
                            }
                            else
                            {
                                prefabInHierarchy = false;
                            }
                        }
                        if (!prefabInHierarchy && !currentAsset.Contains("skybox") && !currentAsset.Contains("Skybox"))
                        {
                            File.Move(AssetDatabase.GetTextMetaFilePathFromAssetPath(currentAsset), newPath + "/" + AssetDatabase.GetTextMetaFilePathFromAssetPath(currentAsset));
                            File.Move(currentAsset, newPath + "/" + currentAsset);
                            File.AppendAllText(newPath + "/" + "removedAssets.txt", currentAsset + "\r\n");
                            AssetDatabase.Refresh();
                            numRemoved++;
                        }
                    }
                    else if (currentAsset.Contains(".unity"))
                    {
                        if (currentAsset != EditorApplication.currentScene)
                        {
                            File.Move(AssetDatabase.GetTextMetaFilePathFromAssetPath(currentAsset), newPath + "/" + AssetDatabase.GetTextMetaFilePathFromAssetPath(currentAsset));
                            File.Move(currentAsset, newPath + "/" + currentAsset);
                            File.AppendAllText(newPath + "/" + "removedAssets.txt", currentAsset + "\r\n");
                            AssetDatabase.Refresh();
                            numRemoved++;
                        }
                    }
                }
            }
            return numRemoved;
        }
        //Set max texture size override for each texture in library to 512
        public int optimizeTextures() 
        {
            string[] texLib = AssetDatabase.FindAssets("t:texture2D");
            Texture sizeCheck;
            int numResized = 0;
            for (int i = 0; i < texLib.Length; i++)
            {
                if (!AssetDatabase.GUIDToAssetPath(texLib[i]).Contains(".cubemap") && !AssetDatabase.GUIDToAssetPath(texLib[i]).Contains("Perfect Parallel/"))
                {
                    string path = AssetDatabase.GUIDToAssetPath(texLib[i]);
                    sizeCheck = AssetDatabase.LoadAssetAtPath(path, typeof(Texture)) as Texture;
                    TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(path);
                    if (sizeCheck.height > 512 || sizeCheck.width > 512)
                    {
                        texImporter.SetPlatformTextureSettings("Android", 512, TextureImporterFormat.AutomaticCompressed);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        numResized++;
                    }
                    else
                    {
                        texImporter.SetPlatformTextureSettings("Android", 128, TextureImporterFormat.AutomaticCompressed);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        numResized++;
                    }
                }
            }
            return numResized;
        }
        //Set material shader for every non-transparent texture in library to a mobile version of that shader
        public int optimizeMaterials()
        {
            string[] androidMaterialsString = AssetDatabase.FindAssets("t:material");
            Shader mobileDiffuse = Shader.Find("Mobile/Diffuse");
            Shader mobileDiffuseB = Shader.Find("Mobile/Bumped Diffuse");
            Shader mobileDiffuseSB = Shader.Find("Mobile/Bumped Specular");
            Shader mobileSkybox = Shader.Find("Mobile/Skybox");
            String legacyDiffuse = Shader.Find("Legacy Shaders/Diffuse").name;
            String legacyDiffuseB = Shader.Find("Legacy Shaders/Bumped Diffuse").name;
            String legacyDiffuseSB = Shader.Find("Legacy Shaders/Bumped Specular").name;
            String legacySkybox = Shader.Find("Skybox/6 Sided").name;
            String standard = Shader.Find("Standard").name;
            String speedTree = Shader.Find("Nature/SpeedTree").name;
            String natureBark = Shader.Find("Nature/Tree Soft Occlusion Bark").name;
            Shader transparent = Shader.Find("Mobile/Particles/VertexLit Blended");
            Material androidMaterial;
            Texture2D transparentCheck = null;
            int copiedMats = 0;

            string newPath = "Removed Assets";
            if (!Directory.Exists(newPath))
            {
                CreateFileStructure();
            }

            for (int i = 0; i < androidMaterialsString.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(androidMaterialsString[i]);

                androidMaterial = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
                if (androidMaterial.HasProperty("_MainTex"))
                {
                    if (androidMaterial.mainTexture != null)
                    {
                        transparentCheck = androidMaterial.mainTexture as Texture2D;
                    }
                }
                else
                {
                    transparentCheck = null;
                }

                if (androidMaterial.shader.name == legacyDiffuse || androidMaterial.shader.name == natureBark)
                {
                    copiedMats = copyOriginalMaterial(path, copiedMats);
                    (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material).shader = mobileDiffuse;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
                else if (androidMaterial.shader.name == legacyDiffuseB || androidMaterial.shader.name == legacyDiffuseSB || androidMaterial.shader.name == standard)
                {
                    copiedMats = copyOriginalMaterial(path, copiedMats);
                    (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material).shader = mobileDiffuseB;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
                else if (androidMaterial.shader.name.Contains("Water"))
                {
                    copiedMats = copyOriginalMaterial(path, copiedMats);
                    (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material).shader = mobileDiffuseSB;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
                else if (androidMaterial.shader.name == legacySkybox)
                {
                    copiedMats = copyOriginalMaterial(path, copiedMats);
                    (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material).shader = mobileSkybox;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
                //Replaces transparent textures
                 else if (transparentCheck != null && androidMaterial.shader.name == speedTree)
                {
                    copiedMats = copyOriginalMaterial(path, copiedMats);
                    if (transparentCheck.format.ToString().Contains("RGBA") || transparentCheck.format.ToString().Contains("ARGB") || transparentCheck.format.ToString().Contains("DXT5"))
                    {
                        (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material).shader = transparent;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                    else
                    {
                        (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material).shader = mobileDiffuseB;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }

                }
                else if (transparentCheck != null)
                {
                    copiedMats = copyOriginalMaterial(path, copiedMats);
                    if ((transparentCheck.format.ToString().Contains("RGBA") || transparentCheck.format.ToString().Contains("ARGB") || transparentCheck.format.ToString().Contains("DXT5")))
                    {
                        (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material).shader = transparent;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                }
                /*Alternate method that does not replace transparent materials
                 * else if (transparentCheck != null && androidMaterial.shader.name == speedTree)
                {
                    if (!transparentCheck.format.ToString().Contains("RGBA") && !transparentCheck.format.ToString().Contains("ARGB") && !transparentCheck.format.ToString().Contains("DXT5"))
                    {
                        (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material).shader = mobileDiffuseB;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                }*/
            }
            return copiedMats;
        }
        //Analyzes textures in library, finds identical textures, removes duplicates
        public int removeTextures()
        {
            string[] assetLib = AssetDatabase.FindAssets("t:texture2D");
            List<string> textureLib = new List<string>();
            List<Texture2D> duplicates = new List<Texture2D>();
            bool remapped;
            List<Texture2D> remove = new List<Texture2D>();
            for (int i = 0; i < assetLib.Length; i++)
            {
                if (!AssetDatabase.GUIDToAssetPath(assetLib[i]).Contains(".cubemap"))
                {
                    textureLib.Add(assetLib[i]);
                }
            }
            string[] path = new string[textureLib.Count];
            byte[] texData;
            int index;
            int[] texSize = new int[textureLib.Count];
            Texture2D[] compareTextures = new Texture2D[textureLib.Count];
            Color32[] comparePixels1;
            Color32[] comparePixels2;

            bool dup = false;
            bool orig = false;
            TextureImporterType[] texTypes = new TextureImporterType[textureLib.Count];
          
            string newPath = "Removed Assets";
            if (!Directory.Exists(newPath))
            {
                CreateFileStructure();
            }

            //Imports all textures as "advanced" so the pixel information is readable
            for (int i = 0; i < textureLib.Count; i++)
            {
                path[i] = AssetDatabase.GUIDToAssetPath(textureLib[i]);
                texData = File.ReadAllBytes(path[i]);
                texSize[i] = texData.Length;
                TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(path[i]);
                texTypes[i] = texImporter.textureType;
                texImporter.textureType = TextureImporterType.Advanced;
                texImporter.isReadable = true;
                compareTextures[i] = AssetDatabase.LoadAssetAtPath(path[i], typeof(Texture2D)) as Texture2D;
                AssetDatabase.ImportAsset(path[i], ImportAssetOptions.ForceUpdate);
            }

            for (int a = 0; a < textureLib.Count; a++)
            {
                for (int b = 1; b < textureLib.Count - a; b++)
                {
                    index = a + b;
                    //is the size of texture[a] equal to the size of each subsesquent texture?
                    if (texSize[a] == texSize[index])
                    {
                        comparePixels1 = compareTextures[a].GetPixels32();
                        comparePixels2 = compareTextures[index].GetPixels32();
                        //does texture[a] have the same number of pixels?
                        if (comparePixels1.Length == comparePixels2.Length)
                        {
                            //is each pixel in texture[a] the same color?
                            for (int c = 0; c < comparePixels1.Length; c++)
                            {
                                if (!comparePixels1[c].Equals(comparePixels2[c]))
                                {
                                    c = comparePixels1.Length;
                                    dup = false;
                                }
                                else
                                {
                                    dup = true;
                                }
                            }
                            //add the texture that matches texture[a] to the list of duplicates
                            if (dup)
                            {
                                duplicates.Add(compareTextures[index]);
                                //if texture[a] doesn't already exist in the duplicates, it is the original texture
                                for (int d = 0; d < duplicates.Count; d++)
                                {
                                    if (compareTextures[a] == duplicates[d])
                                    {
                                        orig = false;
                                        d = duplicates.Count;
                                    }
                                    else
                                    {
                                        orig = true;
                                    }
                                }
                                //if texture[a] is the original texture, remap the duplicate texture to it. if able to remap, add the duplicate texture to the list of textures to remove
                                if (orig)
                                {
                                    remapped = remapMaterials(compareTextures[a], compareTextures[index], texTypes[a]);
                                    if (remapped)
                                    {
                                        remove.Add(compareTextures[index]);
                                    }
                                    if (path[index].Contains("Perfect Parallel Libraries"))
                                    {
                                        remove.Add(compareTextures[index]);
                                        remove.Add(compareTextures[a]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Reformats textures to be their original type
            for (int i = 0; i < textureLib.Count; i++)
            {
                path[i] = AssetDatabase.GUIDToAssetPath(textureLib[i]);
                if (File.Exists(path[i]))
                {
                    TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(path[i]);
                    texImporter.textureType = texTypes[i];
                    AssetDatabase.ImportAsset(path[i], ImportAssetOptions.ForceUpdate);
                }
            }
            //Moves remapped duplicate textures out of library; excludes if they are in the library associated with the course
            for (int i = 0; i < remove.Count; i++)
            {
                string currentAsset = AssetDatabase.GetAssetPath(remove.ElementAt(i).GetInstanceID());
                if (File.Exists(currentAsset) && !currentAsset.Contains(Course.LibraryName))
                {
                    File.Move(currentAsset, "Removed Assets/" + currentAsset);
                    if (File.Exists(currentAsset + ".meta"))
                    {
                        File.Move(currentAsset + ".meta", "Removed Assets/" + currentAsset + ".meta");
                    }
                    File.AppendAllText("Removed Assets/duplicateTextures.txt", currentAsset + "\r\n");
                    AssetDatabase.Refresh();
                }
            }
            return remove.Count;
        }
        //Moves duplicate textures back into asset library
        public void revertOptimizations(string file)
        {
            if (File.Exists(file))
            {
                Debug.Log("Reverting changes from " + file);
                List<String> revertFiles = new List<String>();
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);

                fs.Position = 0;
                sr.DiscardBufferedData();
                while (!sr.EndOfStream)
                {
                    revertFiles.Add(sr.ReadLine());
                }

                for (int i = 0; i < revertFiles.Count; i++)
                {
                    if (File.Exists(revertFiles[i]))
                    {
                        File.Delete(revertFiles[i]);
                        if (File.Exists(revertFiles[i] + ".meta"))
                        {
                            File.Delete(revertFiles[i] + ".meta");
                        }
                    }
                    AssetDatabase.Refresh();
                    File.Move("Removed Assets/" + revertFiles[i], revertFiles[i]);
                    if (File.Exists("Removed Assets/" + revertFiles[i] + ".meta"))
                    {
                        File.Move("Removed Assets/" + revertFiles[i] + ".meta", revertFiles[i] + ".meta");
                    }
                    AssetDatabase.Refresh();
                }
                sr.Close();
                fs.Close();
                File.Delete(file);

            }
        }
        #endregion
        #region Support Methods
        //Copies material at given path to Removed Assets directory and adds to a list of changed materials
        int copyOriginalMaterial(string path, int copied)
        {
            if (!File.Exists("Removed Assets/" + path))
            {
                File.Copy(path, "Removed Assets/" + path);
                if (File.Exists(path + ".meta"))
                {
                    File.Copy(path + ".meta", "Removed Assets/" + path + ".meta");
                }
                File.AppendAllText("Removed Assets/materials.txt", path + "\r\n");
                return copied + 1;
            }
            else
            {
                return copied;
            }

        }
        //Remaps materials using duplicate textures to the same texture, returns true if the textures were successfully remapped
        bool remapMaterials(Texture2D newTexture, Texture2D oldTexture, TextureImporterType texType)
        {
            string[] materialList = AssetDatabase.FindAssets("t:material");
            List<Material> materialLibrary = new List<Material>();
            bool remapped = false;
            for (int i = 0; i < materialList.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(materialList[i]);
                materialLibrary.Add(AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material);
            }
            //search the material library for materials containing the duplicate texture
            if (texType.Equals(TextureImporterType.Image))
            {
                for (int i = 0; i < materialLibrary.Count; i++)
                {
                    if (materialLibrary[i].HasProperty("_MainTex"))
                    {
                        if (materialLibrary[i].GetTexture("_MainTex") != null)
                        {
                            if (materialLibrary[i].GetTexture("_MainTex").GetInstanceID() == oldTexture.GetInstanceID())
                            {
                                materialLibrary[i].SetTexture("_MainTex", newTexture);
                                if (materialLibrary[i].GetTexture("_MainTex").GetInstanceID() == newTexture.GetInstanceID())
                                {
                                    remapped = true;
                                }
                            }
                        }
                    }
                }
            }
            else if (texType.Equals(TextureImporterType.Bump))
            {
                for (int i = 0; i < materialLibrary.Count; i++)
                {
                    if (materialLibrary[i].HasProperty("_BumpMap"))
                    {
                        if (materialLibrary[i].GetTexture("_BumpMap") != null)
                        {
                            if (materialLibrary[i].GetTexture("_BumpMap").GetInstanceID() == oldTexture.GetInstanceID())
                            {
                                materialLibrary[i].SetTexture("_BumpMap", newTexture);
                                if (materialLibrary[i].GetTexture("_BumpMap").GetInstanceID() == newTexture.GetInstanceID())
                                {
                                    remapped = true;
                                }
                            }
                        }
                    }
                }
            }
            return remapped;
        }
        //Generates file structure used when assets are being removed
        void CreateFileStructure()
        {
            string newPath = "Removed Assets";
            string[] assetLibrary = AssetDatabase.FindAssets("t:Object");
            string currentAsset;
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            for (int i = 0; i < assetLibrary.Length; i++)
            {
                currentAsset = AssetDatabase.GUIDToAssetPath(assetLibrary[i]);
                if (!currentAsset.Contains("."))
                {
                    if (!Directory.Exists(newPath + "/" + currentAsset))
                    {
                        Directory.CreateDirectory(newPath + "/" + currentAsset);
                    }
                }
            }
        }
        //Generates file structure used when assets are being removed
        void RemoveEmptyFolders()
        {
            string newPath = "Removed Assets";
            string[] assetLibrary = AssetDatabase.FindAssets("t:Object");
            string currentAsset;
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            for (int i = 0; i < assetLibrary.Length; i++)
            {
                currentAsset = AssetDatabase.GUIDToAssetPath(assetLibrary[i]);
                if (!currentAsset.Contains("."))
                {
                    if (!Directory.Exists(newPath + "/" + currentAsset))
                    {
                        Directory.CreateDirectory(newPath + "/" + currentAsset);
                    }
                }
            }
        }
        //Generates a list of strings representing the assets actively being used by the scene
        List<String> AnalyzeLogFile()
        {
            String UnityLogFolderPath = GetLogFolderPath();
            List<String> usedAssets = new List<String>();
            try
            {
                FileStream fs = new FileStream(UnityLogFolderPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);

                string line;

                fs.Position = 0;
                sr.DiscardBufferedData();
                while (!sr.EndOfStream && !(line = sr.ReadLine()).Contains("sorted by uncompressed size")) ;
                while (!sr.EndOfStream && !(line = sr.ReadLine()).Contains("Build Successful"))
                {
                    usedAssets.Add(line);
                }
                return usedAssets;
            }
            catch (Exception e)
            {
                Debug.Log("Please generate a build log before attempting to remove unused assets.");
                Debug.LogError(e.Message);
                return usedAssets;
            }
        }
        public bool buildLog()
        {
            List<String> usedAssets = AnalyzeLogFile();
            if (usedAssets.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //Returns materials associated with .fbx files
        List<String> meshMaterials(List<String> usedAssets)
        {
            string[] meshGUIDs = AssetDatabase.FindAssets("t:mesh");
            List<String> meshList = new List<String>();
            List<String> used = new List<String>();
            GameObject mesh = new GameObject();
            for (int i = 0; i < usedAssets.Count; i++)
            {
                if (usedAssets[i].Contains(".fbx"))
                {
                    meshList.Add(usedAssets[i]);
                }
            }
            for (int i = 0; i < meshGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(meshGUIDs[i]);
                for (int index = 0; index < meshList.Count; index++)
                {
                    if (meshList[index].Contains(path))
                    {
                        mesh = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                        if(mesh.GetComponent<MeshRenderer>() != null)
                        {
                            int instID = mesh.GetComponent<MeshRenderer>().sharedMaterial.GetInstanceID();
                            used.Add(AssetDatabase.GetAssetPath(instID));
                        }
                    }
                }
            }
            return used;
        }
        //Find Editor.log for either Windows or OSX
        static string GetLogFolderPath()
        {
            string LocalAppData;
            string UnityLogFolderPath = string.Empty;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                LocalAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                UnityLogFolderPath = LocalAppData + "\\Unity\\Editor\\Editor.log";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                LocalAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                UnityLogFolderPath = LocalAppData + "/Library/Logs/Unity/Editor.log";
            }
            else
                Debug.LogError("RuntimePlatform not known");
            return UnityLogFolderPath;
        }   
        #endregion
     }
}
