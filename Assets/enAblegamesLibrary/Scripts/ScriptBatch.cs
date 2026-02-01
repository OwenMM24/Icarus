#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Threading;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

public class ScriptBatch 
{
    [MenuItem("EnableGames/Build/Build and Generate FileList")]
    public static void TryBuildAndGenerateFileList ()
    {
        // Get filename.
        BuildPlayerOptions buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions());

        BuildPipeline.BuildPlayer(buildPlayerOptions);

        Debug.Log(Path.GetDirectoryName(buildPlayerOptions.locationPathName));
        GenerateFileList(Path.GetDirectoryName(buildPlayerOptions.locationPathName));
    }


    [MenuItem("EnableGames/Build/Generate FileList")]
    public static void TryGenerateFileList()
    {
        // Get filename.
        BuildPlayerOptions buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions());

        Debug.Log(Path.GetDirectoryName(buildPlayerOptions.locationPathName));
        GenerateFileList(Path.GetDirectoryName(buildPlayerOptions.locationPathName));
    }

    [MenuItem("EnableGames/SceneSetup")]
    public static void SceneSetup()
    {
        if(LayerMask.NameToLayer("BodyTrack") == -1)
            IgnoreLayerForMainCamera();

        GameObject debugPrefab = Resources.Load<GameObject>("eagIngameDebugConsole");
        GameObject parameterPrefab = Resources.Load<GameObject>("_ParameterHandler");
        GameObject companionPrefab = Resources.Load<GameObject>("CompanionHandler");
        GameObject DDAPrefab = Resources.Load<GameObject>("DDAManager");
        GameObject canvasPrefab = Resources.Load<GameObject>("eg_Canvas");
        GameObject objectPrefab = Resources.Load<GameObject>("egObjects");
        GameObject serialPrefab = Resources.Load<GameObject>("SerialHandler");

        PrefabUtility.InstantiatePrefab(debugPrefab);
        PrefabUtility.InstantiatePrefab(parameterPrefab);
        PrefabUtility.InstantiatePrefab(companionPrefab);
        PrefabUtility.InstantiatePrefab(DDAPrefab);
        PrefabUtility.InstantiatePrefab(canvasPrefab);
        PrefabUtility.InstantiatePrefab(objectPrefab);
        PrefabUtility.InstantiatePrefab(serialPrefab);
    }
    
    [MenuItem("EnableGames/AddDebug")]
    public static void AddDebugPrefab()
    {
        GameObject debugPrefab = Resources.Load<GameObject>("eagIngameDebugConsole");
        PrefabUtility.InstantiatePrefab(debugPrefab);
    }
    
    [MenuItem("EnableGames/CopyStreamingAssets")]
    public static void CopyStreamingAssets()
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
    }
    
    static void Copy(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach(var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

        foreach(var directory in Directory.GetDirectories(sourceDir))
            Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
    }



    static void GenerateFileList(string path)
    {
        Debug.Log("start generating fileList");

        //Everything is duplicated to generate both the fileList.txt and the games_fileList.txt at the same time.



        Debug.Log("workingGameBuildPath: " + path);


        string localFileListPath = Path.Combine(path, "fileList.txt");

        string[] _AllFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

        TextWriter tw = new StreamWriter(localFileListPath, false);

        string _exePath = "";
        
        _exePath = Path.Combine(path, PlayerSettings.productName + ".exe");

        using (var md5 = MD5.Create())
        {
            Debug.Log("Exepath is " + _exePath);
            using (var stream = File.OpenRead(_exePath))
            {
                string _md5 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                tw.WriteLine(_md5);
            }
        }

        foreach (string s in _AllFiles)
        {
            string t = s.Replace(path , null);
            t = t.Substring(1);

            //Add Exceptions if you have items in build output folder that you do not want in final. Uncomment if statement and add your exceptions.
            //Example !t.StartsWith(@"Logs\") && !t.EndsWith("Thumbs.db")
            if (!t.Contains("fileList.txt") && !t.Contains("fileList_server.txt") && !t.Contains("fileList_games.txt") && !t.Contains("GamesList.txt") && !t.Contains("fileList_games_server.txt") && !t.Contains("output_log.txt")&& !t.Contains(".DS_Store"))
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(s))
                    {
                        string _md5 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                        string newline = Path.Combine("Games", PlayerSettings.productName,t);
                        newline += "\t" + _md5;
                        tw.WriteLine(newline);
                        
                    }
                }
            }
        }

        tw.Close();

        Debug.Log("File List Created in Build Output Folder");
    }



    public static void IgnoreLayerForMainCamera()
    {
        EditorPrefs.SetInt("BodyTrack", 20);

        int layerToIgnore = EditorPrefs.GetInt("BodyTrack");

        Camera mainCamera = Camera.main;

        if (LayerMask.LayerToName(layerToIgnore) != "BodyTrack")
        {
            while (LayerMask.LayerToName(layerToIgnore) != "" && LayerMask.LayerToName(layerToIgnore) != null)
            {
                layerToIgnore += 1;
                EditorPrefs.SetInt("BodyTrack", layerToIgnore);
            }
            //Debug.LogError($"RenderLayer 20 came with a different name: '{LayerMask.LayerToName(layerToIgnore)}'. This layer is used by enAbleGames body tracker and must be set to 'BodyTrack'. Please set a different RenderLayer to use in your game.");

            SetLayerName(layerToIgnore, "BodyTrack");
        }

        if (mainCamera != null)
        {
            // Get the current culling mask
            int cullingMask = mainCamera.cullingMask;

            // Calculate the mask to ignore the specified layer
            int layerMask = 1 << layerToIgnore;
            int invertedMask = ~layerMask;

            // Set the new culling mask for the main camera
            mainCamera.cullingMask = cullingMask & invertedMask;
        }
    }



    static void SetLayerName(int index, string name)
    {
        // Access the UnityEditorInternal.TagManager class
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layersProperty = tagManager.FindProperty("layers");

        // Check if the index is valid
        if (layersProperty == null || index >= layersProperty.arraySize)
        {
            Debug.LogError("Layer index out of range.");
            return;
        }

        // Set the new name
        var layerProperty = layersProperty.GetArrayElementAtIndex(index);
        layerProperty.stringValue = name;
        tagManager.ApplyModifiedProperties();

        Debug.Log($"RenderLayer {index} renamed to '{name}'");
    }
}

#endif