using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SkillBundleLoader : MonoBehaviour
{
    [SerializeField] string path;
    [SerializeField] GameObject loadingLayer;

    private void Awake() {
        string recentVersion = PlayerPrefs.GetString("recentVersion", "none");
        if(recentVersion != Application.version)
        {
        }
        PlayerPrefs.SetString("recentVersion", Application.version);

        if(Application.platform == RuntimePlatform.Android)
        {
            path = Path.Combine(Application.streamingAssetsPath, path);

        }
        else if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = Path.Combine(Application.streamingAssetsPath, path);

        }
        else 
        {
            path = Path.Combine(Application.streamingAssetsPath, path);

        }
        StartCoroutine(LoadFromMemoryAsync(path));
    }

    IEnumerator LoadFromMemoryAsync(string path) { 
        byte[] bytes = null;

        loadingLayer.SetActive(true);

        if(Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(path);

            Debug.Log(path);
            
            yield return www.SendWebRequest();

            if(www.isDone)
            {
                bytes = www.downloadHandler.data;
                Debug.Log(bytes.LongLength);
            }
            else
            {
                Debug.Log(www.error);
            }
        }
        else 
        {
            bytes = File.ReadAllBytes(path);

        }


        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(bytes); 
        
        yield return request; 
        
        AssetBundle bundle = request.assetBundle; 

        var skills = bundle.LoadAllAssets<InGame.Data.Skill.CharacterSkill>();

        foreach(var skill in skills)
        {
            InGame.Data.Skill.SkillCollection.GetInstance().AddSkill(skill);
        }

        
        var asyncOperation = bundle.UnloadAsync(false);

        loadingLayer.SetActive(false);

        yield return asyncOperation; 
    }

}
