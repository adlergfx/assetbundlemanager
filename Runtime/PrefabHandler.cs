using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PrefabHandler : MonoBehaviour
{
    [Tooltip("Package this Handler is waiting for")]
    public string package = "";
    [Tooltip("Name of bundle to load. May include path prefix additional to package")]
    public string bundle = "";
    [Tooltip("Prefab in Bundle to load")]
    public string prefab = "";
    
    public void OnAssetBundleLoaded(string package)
    {
       if (package != this.package) return;
       // check if bundle is already loaded
       IEnumerable<AssetBundle> bundles = AssetBundle.GetAllLoadedAssetBundles();
       AssetBundle bndl = bundles.Where((b) => Path.GetFileName(b.name) == package).FirstOrDefault();

       if (bndl == null)
       {

           string dll = Path.Combine(Application.streamingAssetsPath, package, $"{package}.dll");
           Assembly ass = Assembly.LoadFile(dll); // classes now knows

           List<GameObject> child = new List<GameObject>();
           for (int i = 0; i < transform.childCount; i++) child.Add(transform.GetChild(i).gameObject);
           child.ForEach((c) => Destroy(c));

           string path = Path.Combine(Application.streamingAssetsPath, package, bundle);
           bndl = AssetBundle.LoadFromFile(path);
       }

       GameObject go = bndl.LoadAsset<GameObject>(prefab);
       Instantiate(go, transform);
    }
    
}