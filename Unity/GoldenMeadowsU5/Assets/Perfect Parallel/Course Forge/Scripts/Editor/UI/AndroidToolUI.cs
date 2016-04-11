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
    public class AndroidToolUI : ToolUI
    {
        #region Properties
        OptimizationTool optimize = new OptimizationTool();
        #endregion
        #region Methods
        public override void OnUI(bool selected)
        {
            BeginUI();

            MoveToTop();
            Move(1, 1);
            MovePixels(1, 1);
            Background(6, 4.05f, 0.5f);
            #region Android Optimization Menu
            #endregion 
            #region Asset Removal
            Label("| Unused Asset Removal", 3.5f);

            Move(0, 0.5f);
            MovePixels(0, 1);
            Background(6, 1.0f, 0.75f);
            {
                Label("  Build Output Log", 2.5f);
                Move(2.5f, 0);
                if (Button("Build Log", 1.6f, 0.6f))
                {
                    optimize.build();
                }
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Remove Unused", 2.5f);
                Move(2.5f, 0);
                if (Button("Remove", 1.6f, 0.6f))
                {
                    MyWindow window = EditorWindow.CreateInstance<MyWindow>();
                    window.getRect(EditorWindow.focusedWindow.position.x + EditorWindow.focusedWindow.position.width / 2, EditorWindow.focusedWindow.position.y + EditorWindow.focusedWindow.position.height / 2, 400, 150);
                    window.ShowPopup();
                    List<int> output = new List<int>();
                    List<string> message = new List<string>();
                    output.Add(optimize.RemoveUnusedAssets());
                    message.Add(" unused assets removed.");
                    window.getResults(output, message);
                }
                Move(1.7f, 0);
                if (Button("Revert", 1.6f, 0.6f))
                {
                    optimize.revertOptimizations("Removed Assets/removedAssets.txt");
                }
                Move(-4.2f, 0);
            }
            #endregion

            MoveToTop();
            Move(0, 3.5f);
            MovePixels(0, 1);
            Move(0, -1.0f);

            #region Optimize
            Label("| Optimize Assets", 3.5f);
            Move(0, 0.5f);
            MovePixels(0, 1);
            Background(6, 1.0f, 0.75f);
            {
                Label("  Textures", 2.5f);
                Move(2.5f, 0);
                if (Button("Optimize", 1.6f, 0.6f))
                {
                    MyWindow window = EditorWindow.CreateInstance<MyWindow>();
                    window.getRect(EditorWindow.focusedWindow.position.x + EditorWindow.focusedWindow.position.width / 2, EditorWindow.focusedWindow.position.y + EditorWindow.focusedWindow.position.height / 2, 400, 150);
                    window.ShowPopup();
                    List<int> output = new List<int>();
                    List<string> message = new List<string>();
                    output.Add(optimize.optimizeTextures());
                    message.Add(" max texture sizes overridden for Android.");
                    window.getResults(output, message);
                }
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Materials", 2.5f);
                Move(2.5f, 0);
                if (Button("Optimize", 1.6f, 0.6f))
                {
                    MyWindow window = EditorWindow.CreateInstance<MyWindow>();
                    window.getRect(EditorWindow.focusedWindow.position.x + EditorWindow.focusedWindow.position.width / 2, EditorWindow.focusedWindow.position.y + EditorWindow.focusedWindow.position.height / 2, 400, 150);
                    window.ShowPopup();
                    List<int> output = new List<int>();
                    List<string> message = new List<string>();
                    output.Add(optimize.optimizeMaterials());
                    message.Add(" materials optimized.");
                    window.getResults(output, message);
                }
                Move(1.7f, 0);
                if (Button("Revert", 1.6f, 0.6f))
                {
                    optimize.revertOptimizations("Removed Assets/materials.txt");
                }
                Move(-4.2f, 0);
            }
            Move(0, 0.5f);
            #endregion

            #region Redundant Textures
            Label("| Remove Redundant Textures", 4.0f);
            Move(0, 0.5f);
            MovePixels(0, 1);
            Background(6, 0.9f, 0.75f);
            {
                Label("  Textures", 2.5f);
                Move(2.5f, 0);
                if (Button("Remove", 1.6f, 0.6f))
                {
                    MyWindow window = EditorWindow.CreateInstance<MyWindow>();
                    window.getRect(EditorWindow.focusedWindow.position.x + EditorWindow.focusedWindow.position.width / 2, EditorWindow.focusedWindow.position.y + EditorWindow.focusedWindow.position.height / 2, 400, 150);
                    window.ShowPopup();
                    List<int> output = new List<int>();
                    List<string> message = new List<string>();
                    output.Add(optimize.removeTextures());
                    message.Add(" redundant textures removed.");
                    window.getResults(output, message);
                }
                Move(1.7f, 0);
                if (Button("Revert", 1.6f, 0.6f))
                {
                    optimize.revertOptimizations("Removed Assets/duplicateTextures.txt");
                }
                Move(-4.2f, 0);
                Move(2.5f, 0.4f);
                if (Button("test", 1.6f, 0.6f))
                {
                    MyWindow window = EditorWindow.CreateInstance<MyWindow>();
                    window.getRect(EditorWindow.focusedWindow.position.x + EditorWindow.focusedWindow.position.width / 2, EditorWindow.focusedWindow.position.y + EditorWindow.focusedWindow.position.height / 2, 240, 240);
                    window.getBuildLog(optimize.buildLog());
                    window.getMenu(true);
                    window.ShowPopup();
                }
                Move(-2.5f, 0);
            }
            MovePixels(-1, 0);
            #endregion
            EndUI();
        }
        #endregion
     }
}
