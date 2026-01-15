#if UNITY_EDITOR


using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using AssetBundle.Editor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetBundleManager : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [SerializeField] 
    private SOAssetBundleData data;

    private static readonly string versionTag = "version";
    private static readonly string ScriptAssemblies = "Library/ScriptAssemblies";
    

    [MenuItem("Assets/Build Asset Bundles")]
    public static void ShowDialog()
    {
        AssetBundleManager wnd = GetWindow<AssetBundleManager>();
        wnd.titleContent = new GUIContent("Build Asset Bundles");
    }

    private void createZip(string bundle)
    {
        // bundle is the bundle name which may include a path
        // the last part is the file prefix
        // The bundle "dynamic/star" is the bundle "star" with the files
        // "star" and "star.manifest" in the folder star
        // Additionally it might be that someone had created an assembly definition with
        // same name which should be packaged to
        string projectDir = System.Environment.CurrentDirectory;    // Folder including Assets
        string path = Path.GetDirectoryName(bundle);        // e.g. "dynamic"
        string bundleFile = Path.GetFileName(bundle);       // e.g. "star"
        string dllFile = $"{bundleFile}.dll";               // e.g. "star.dll"
        string bundleManifest = $"{bundleFile}.manifest";   // e.g. "star.manifest"
        string jsonFile = Path.Combine(projectDir, data.zipOutputDir, $"{bundleFile}.json");
        

        string zipFolder = Path.Combine(projectDir, data.zipOutputDir);
        string zipFile = Path.Combine(zipFolder, $"{bundleFile}.zip");
        if (!Directory.Exists(zipFolder)) Directory.CreateDirectory(zipFolder);

        if (File.Exists(zipFile)) File.Delete(zipFile);
        if (File.Exists(jsonFile)) File.Delete(jsonFile);
       
        using (ZipArchive zip = ZipFile.Open(zipFile, ZipArchiveMode.Create))
        {
            zip.CreateEntryFromFile(Path.Combine(data.bundleFolder, path, bundleFile), bundle);
            zip.CreateEntryFromFile(Path.Combine(data.bundleFolder, path, bundleManifest), Path.Combine(path, bundleManifest));

            string dll = Path.Combine(projectDir, ScriptAssemblies, dllFile);
            if (File.Exists(dll))
            {
                zip.CreateEntryFromFile(dll, dllFile);
            }
        }

        if (!string.IsNullOrEmpty(data?.version))
        {
            Dictionary<string, string> v = new Dictionary<string, string>();
            v.Add(versionTag, data.version);
            string json = JsonConvert.SerializeObject(v);
            File.WriteAllText(jsonFile, json);
        }
    }

    private void buildBundles()
    {
        // Ensure the AssetBundles directory exists, and if it doesn't, create it.
        string assetBundleDirectory = data.bundleFolder;
        if (!Directory.Exists(assetBundleDirectory))
            Directory.CreateDirectory(assetBundleDirectory);

        BuildAssetBundleOptions opts = BuildAssetBundleOptions.None;
        if (data.forceRebuild) opts = BuildAssetBundleOptions.ForceRebuildAssetBundle;
        
        // Build all AssetBundles and place them in the specified directory.
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(assetBundleDirectory, opts, data.target);
        string[] bundles = manifest.GetAllAssetBundles();
        foreach (string bundle in bundles)
        {
            Debug.Log(bundle);
            createZip(bundle);
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
        
        Button btn = root.Q<Button>("btnBuild");
        btn.clicked += buildBundles;

    }
}
#endif
