#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace AssetBundle.Editor
{
    [CreateAssetMenu(fileName = "AssetBundleData", menuName = "Adler/Asset Bundle Data", order = 0)]
    public class SOAssetBundleData : ScriptableObject
    {
        public string bundleFolder = "aaa";
        public bool forceRebuild = false;
        public BuildTarget target = BuildTarget.StandaloneWindows;
        public bool createZipPackages = true;
        public string zipOutputDir = "";
        public string version = "";
    }
}

#endif