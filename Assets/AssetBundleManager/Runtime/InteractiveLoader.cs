using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using Codice.Client.Commands.WkTree;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveLoader : MonoBehaviour
{
    [Tooltip("URL where to download zipped Bundles")]
    public string url = "";
    [Tooltip("Names of packages to download")]
    public string[] packages;
    [Tooltip("If false, packages will wait for call to load")]
    public bool loadOnStart = true;
//    private string zipFile = "";
    private readonly string versionTag = "version";

    public UnityEvent<string> onUpdated;

    private class PackageData
    {
        public string package;
        public string zipFile;
    }
    
    void loadDynamic(object o, AsyncCompletedEventArgs args )
    {
        PackageData data = (PackageData)args.UserState;
        string dest = Path.Combine(Application.streamingAssetsPath, data.package);
        // delete directory, if exists
        try { Directory.Delete(dest, true); } catch {}
        Directory.CreateDirectory(dest);
        System.IO.Compression.ZipFile.ExtractToDirectory(data.zipFile, dest);
        File.Delete(data.zipFile);
        onUpdated.Invoke(data.package);
    }

    private bool isUpdateRequired(string remoteVersion, string package)
    {
        if ( string.IsNullOrEmpty(remoteVersion) ) return true;
        
        // try to read local version
        string ljsonPath = Path.Combine(Application.streamingAssetsPath, $"{package}.json");
        try
        {
            
            Dictionary<string, string> map = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ljsonPath));
            string v = map.GetValueOrDefault(versionTag, "");
            if (string.IsNullOrEmpty(v)) return true;
            // if v (localVersion) succedes or equals the remote Version
            if (v.CompareTo(remoteVersion) >= 0) 
            {
                Debug.Log($"Versions: Remote {remoteVersion}, Local {v} - no update required");
                onUpdated.Invoke(package);
                return false;    // remove version is older or equals
            }   
            Debug.Log($"Versions: Remote {remoteVersion}, Local {v} - update required");
        }
        catch { }
        
        return true;    // no valid local version
    }

    public void Start()
    {
        if (loadOnStart) load();
    }

    /**
     * This method will start to download packages from given URL
     */
    public void load()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);
        
        using (WebClient client = new WebClient())
        {
            client.DownloadFileCompleted += loadDynamic;
            foreach (string package in packages)
            {
                string zipFile = Path.Combine(Application.streamingAssetsPath, $"{package}.zip");
                Uri zipUri = new Uri(Path.Combine(url, $"{package}.zip"));
                Uri jsonUri = new Uri(Path.Combine(url, $"{package}.json"));
                
                string jsonString = client.DownloadString(jsonUri);
                Dictionary<string, string> map = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonString);
                if (!isUpdateRequired(map.GetValueOrDefault(versionTag, ""), package)) return;
                File.WriteAllText(Path.Combine(Application.streamingAssetsPath, $"{package}.json"), jsonString);

                PackageData data = new PackageData { package = package, zipFile = zipFile };
                client.DownloadFileAsync(zipUri, zipFile, data);
            }
        }
        
    }
}
