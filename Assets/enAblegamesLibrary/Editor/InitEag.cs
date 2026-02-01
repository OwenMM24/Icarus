using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class InitEag : MonoBehaviour
{
    [InitializeOnLoad]
    public class InitOnLoad
    {
        static InitOnLoad()
        {
            if (!EditorPrefs.HasKey("eagInitialized"))
            {
                try
                {
                    Copy(Path.Combine(Application.dataPath, "enAblegamesLibrary", "egConfig"),
                        Application.streamingAssetsPath);
                }
                catch (Exception e) 
                { 
                    Console.WriteLine(e);
                    return;
                }
                
                EditorPrefs.SetInt("eagInitialized",1); 
                EditorUtility.DisplayDialog("Hello there!", "You have successfully imported enAbleGames Library. Please refer to the Readme to finish the set up", "Okay :)");
            }
        }
    }
    
    
    static void Copy(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach(var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

        foreach(var directory in Directory.GetDirectories(sourceDir))
            Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
    }
    
    
}
