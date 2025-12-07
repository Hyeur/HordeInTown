using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor script to help rename audio files from .sfx/.bgm to proper extensions
/// </summary>
public class AudioFileRenamer : EditorWindow
{
    [MenuItem("Tools/Rename Audio Files")]
    public static void ShowWindow()
    {
        GetWindow<AudioFileRenamer>("Audio File Renamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Audio File Renamer", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("This tool helps rename .sfx and .bgm files to standard audio formats.");
        GUILayout.Space(10);
        
        GUILayout.Label("Instructions:");
        GUILayout.Label("1. Check the actual format of your audio files");
        GUILayout.Label("2. Choose the target format below");
        GUILayout.Label("3. Click 'Rename Files'");
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Note: Unity supports .wav, .mp3, .ogg, .aiff formats.\n" +
            "If your files are already in one of these formats but have .sfx/.bgm extensions,\n" +
            "you can rename them. Otherwise, you may need to convert them first.",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Rename .sfx files to .wav", GUILayout.Height(30)))
        {
            RenameAudioFiles("Assets/Sound/Sound Effect", "*.sfx", ".wav");
        }
        
        if (GUILayout.Button("Rename .sfx files to .mp3", GUILayout.Height(30)))
        {
            RenameAudioFiles("Assets/Sound/Sound Effect", "*.sfx", ".mp3");
        }
        
        if (GUILayout.Button("Rename .bgm files to .wav", GUILayout.Height(30)))
        {
            RenameAudioFiles("Assets/Sound/Background Music", "*.bgm", ".wav");
        }
        
        if (GUILayout.Button("Rename .bgm files to .mp3", GUILayout.Height(30)))
        {
            RenameAudioFiles("Assets/Sound/Background Music", "*.bgm", ".mp3");
        }
        
        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "After renaming, Unity will automatically reimport the files as audio.\n" +
            "Then assign them to AudioManager in the Inspector.",
            MessageType.Info
        );
    }
    
    private void RenameAudioFiles(string folderPath, string searchPattern, string newExtension)
    {
        if (!Directory.Exists(folderPath))
        {
            EditorUtility.DisplayDialog("Error", $"Folder not found: {folderPath}", "OK");
            return;
        }
        
        string[] files = Directory.GetFiles(folderPath, searchPattern);
        
        if (files.Length == 0)
        {
            EditorUtility.DisplayDialog("Info", "No files found to rename.", "OK");
            return;
        }
        
        if (EditorUtility.DisplayDialog(
            "Confirm Rename",
            $"This will rename {files.Length} file(s) from {searchPattern} to {newExtension}.\n\n" +
            "Make sure your files are actually in this format!\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            foreach (string file in files)
            {
                string newPath = Path.ChangeExtension(file, newExtension);
                
                // Delete old .meta file
                string oldMeta = file + ".meta";
                if (File.Exists(oldMeta))
                {
                    File.Delete(oldMeta);
                }
                
                // Rename file
                File.Move(file, newPath);
                
                Debug.Log($"Renamed: {Path.GetFileName(file)} -> {Path.GetFileName(newPath)}");
            }
            
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", $"Renamed {files.Length} file(s). Unity will now reimport them as audio.", "OK");
        }
    }
}

