using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
public class BuildAssetBundles : MonoBehaviour
{
    [MenuItem("Tools/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {

    #if (UNITY_ANDROID)
      BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);
    #elif (UNITY_IOS)
      BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXUniversal);
    #else
      BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    #endif
    }
}
#endif