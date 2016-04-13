using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PerfectParallel.CourseForge.UI;

namespace PerfectParallel.CourseForge.UI
{
    public class AndroidWindow : EditorWindow
    {
        OptimizationTool optimize = new OptimizationTool();

        bool build = false;
        bool removeUnused = false;
        bool revertRU = false;
        bool optimizeTextures = false;
        bool optimizeMaterials = false;
        bool revertOM = false;
        bool removeTextures = false;
        bool revertRT = false;
        bool buildComplete = false;
        string scene = EditorApplication.currentScene;
        bool isMenu = false;
        int progress = 0;
        int total = 0;

        bool buildLogExists = false;

        List<int> output = new List<int>();
        List<string> message = new List<string>();
        GUIStyle centeredWrapped = new GUIStyle(EditorStyles.label);
        GUIStyle centeredBold = new GUIStyle(EditorStyles.boldLabel);
        Rect window = new Rect(0, 0, 0, 0);

        public void getRect(float x, float y, int width, int height)
        {
            window = new Rect(x - width / 2, y - height / 2, width, height);
        }
        public void getResults(List<int> getOutput, List<string> getMessage)
        {
            output = getOutput;
            message = getMessage;
        }
        public void getMenu(bool getMenu)
        {
            isMenu = getMenu;
        }
        public void getBuildLog(bool getLog)
        {
            buildLogExists = getLog;
        }
        public void getProgress(int getProgress)
        {
            progress = getProgress;
        }
        public void getTotal(int getTotal)
        {
            total = getTotal;
        }
        void OnGUI()
        {
            this.position = window;

            while (MainToolUI.EnableValidate() && scene == EditorApplication.currentScene)
            {
                MainToolUI.EnablePerform();
                if (revertRU && buildComplete)
                {
                    optimize.revertOptimizations("Removed Assets/removedAssets.txt");
                    revertRU = false;
                }
                if (revertOM && buildComplete)
                {
                    optimize.revertOptimizations("Removed Assets/materials.txt");
                    revertOM = false;
                }
                if (revertRT && buildComplete)
                {
                    optimize.revertOptimizations("Removed Assets/duplicateTextures.txt");
                    revertRT = false;
                }
                if (output.Count != 0)
                {
                    this.getRect(this.position.x + this.position.width / 2, this.position.y + this.position.height / 2, 400, 30 * (output.Count + 4));
                    this.getResults(output, message);
                    this.isMenu = false;
                    this.Focus();
                }
            }

            centeredWrapped.alignment = TextAnchor.MiddleCenter;
            centeredWrapped.wordWrap = true;
            centeredBold.alignment = TextAnchor.MiddleCenter;

            if (isMenu)
            {
                if (GUI.Button(new Rect(25, 200, 60, 18), "Close"))
                {
                    this.Close();
                }
                if (buildLogExists)
                {
                    EditorGUI.LabelField(new Rect(0, 10, this.position.width, 18), "Optimize for Android", centeredBold);
                    build = EditorGUI.ToggleLeft(new Rect(10, 30, this.position.width, 18), "Build After Optimization", build, EditorStyles.boldLabel);
                    removeUnused = EditorGUI.ToggleLeft(new Rect(10, 50, this.position.width, 18), "Remove Unused Assets", removeUnused, EditorStyles.boldLabel);
                    EditorGUI.BeginDisabledGroup(removeUnused == false || build == false);
                    revertRU = EditorGUI.ToggleLeft(new Rect(30, 70, this.position.width, 18), "Revert After Build", revertRU);
                    EditorGUI.EndDisabledGroup();
                    optimizeTextures = EditorGUI.ToggleLeft(new Rect(10, 90, this.position.width, 18), "Optimize Textures", optimizeTextures, EditorStyles.boldLabel);
                    optimizeMaterials = EditorGUI.ToggleLeft(new Rect(10, 110, this.position.width, 18), "Optimize Materials", optimizeMaterials, EditorStyles.boldLabel);
                    EditorGUI.BeginDisabledGroup(optimizeMaterials == false || build == false);
                    revertOM = EditorGUI.ToggleLeft(new Rect(30, 130, this.position.width, 18), "Revert After Build", revertOM);
                    EditorGUI.EndDisabledGroup();
                    removeTextures = EditorGUI.ToggleLeft(new Rect(10, 150, this.position.width, 18), "Remove Redundant Textures", removeTextures, EditorStyles.boldLabel);
                    EditorGUI.BeginDisabledGroup(removeTextures == false || build == false);
                    revertRT = EditorGUI.ToggleLeft(new Rect(30, 170, this.position.width, 18), "Revert After Build", revertRT);
                    EditorGUI.EndDisabledGroup();
                    if (GUI.Button(new Rect(125, 200, 60, 18), "Run"))
                    {
                        if (removeUnused)
                        {
                            output.Add(optimize.RemoveUnusedAssets());
                            message.Add(" unused assets removed.");
                        }
                        if (optimizeTextures)
                        {
                            output.Add(optimize.optimizeTextures());
                            message.Add(" max texture sizes overridden for Android.");
                        }
                        if (optimizeMaterials)
                        {
                            output.Add(optimize.optimizeMaterials());
                            message.Add(" materials optimized.");
                        }
                        if (removeTextures)
                        {
                            output.Add(optimize.removeTextures());
                            message.Add(" redundant textures removed.");
                        }
                        if (build)
                        {
                            buildComplete = true;
                            optimize.build();
                        }
                        if (output.Count != 0)
                        {
                            this.getRect(this.position.x + this.position.width / 2, this.position.y + this.position.height / 2, 400, 30 * (output.Count + 4));
                            this.getResults(output, message);
                            this.isMenu = false;
                            this.Focus();
                        }
                    }
                }
                else
                {
                    EditorGUI.LabelField(new Rect(0, 10, this.position.width, 18), "Optimize for Android", centeredBold);
                    EditorGUI.LabelField(new Rect(0, 55, this.position.width, 18), "Please build an output log.", centeredBold);
                    EditorGUI.LabelField(new Rect(0, 100, this.position.width, 36), "If prompted to save scene, select cancel to continue build.", centeredWrapped);
                    if (GUI.Button(new Rect(125, 200, 100, 18), "Build Log"))
                    {
                        buildLogExists = true;
                        optimize.build();
                    }
                }
            }

            else
            {
                for (int i = 0; i < output.Count; i++)
                {
                    int labelSize = (output[i] + message[i]).Length * 8;
                    GUI.Label(new Rect(0, 30 * (i + 1), this.position.width, 30),
                        output[i] + message[i], centeredWrapped);
                }
                if (output.Count > 0)
                {
                    GUI.Label(new Rect(5, 30 * (output.Count + 1), this.position.width - 10, 30),
                            "Altered or removed files have been moved to the Removed Assets folder in your project.", centeredWrapped);
                    if (GUI.Button(new Rect(this.position.width / 4 - 20, 30 * (output.Count + 3), 40, 18), "Ok"))
                    {
                        this.Close();
                    }
                    if (GUI.Button(new Rect(this.position.width * 3 / 4 - 100, 30 * (output.Count + 3), 130, 18), "Open Folder Location"))
                    {
                        Application.OpenURL(Directory.GetCurrentDirectory() + "/Removed Assets");
                    }
                }
                else
                {
                    GUI.Label(new Rect(5, 30, this.position.width - 10, 30),
                            "Please wait...", centeredWrapped);
                    if (GUI.Button(new Rect(this.position.width / 4 - 20, 30 * (output.Count + 3), 40, 18), "Ok"))
                    {
                        this.Close();
                    }
                }
            }
        }
    }
}

