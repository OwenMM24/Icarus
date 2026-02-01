using UnityEngine;
using UnityEditor;

public class InitRefine : MonoBehaviour
{
    [InitializeOnLoad]
    public class InitOnLoad
    {
        static InitOnLoad()
        {
            if (!EditorPrefs.HasKey("RefineUI.Installed"))
            {
                EditorPrefs.SetInt("RefineUI.Installed", 1);
                EditorUtility.DisplayDialog("Hello there!", "Thank you for purchasing RefineUI.\r\rImport TextMesh Pro from Package Manager if you haven't already.\r\rYou can check Documentation file for help, or contact me at rayan@r4yan.com or yessou.rayan@gmail.com", "Okay :)");
            }
        }
    }
}