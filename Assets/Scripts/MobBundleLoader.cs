using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MobBundleLoader : MonoBehaviour
{
    [SerializeField] string path;
    [SerializeField] GameObject loadingLayer;
    [SerializeField] Main.UI.Statistics.CollectionDisplay collectionDisplay;

    bool itemLoad = false;

    private void Awake() {
        string recentVersion = PlayerPrefs.GetString("recentVersion", "none");
        if(recentVersion != Application.version)
        {
            Directory.Delete(Application.persistentDataPath, true);
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

        while (!itemLoad)
        {
            yield return null;
        }

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

        /*
        var skills = bundle.LoadAllAssets<InGame.Data.Skill.Skill>();

        foreach(var skill in skills)
        {
            InGame.Data.Skill.SkillCollection.GetInstance().AddSkill(skill);
        }
        */

        var mobs = bundle.LoadAllAssets<InGame.Data.Mob.AbstractMob>();

        InGame.Data.Mob.MobCollection.GetInstance().SetList(
            new List<InGame.Data.Mob.AbstractMob>(mobs)
        );

        var asyncOperation = bundle.UnloadAsync(false);

        loadingLayer.SetActive(false);
        collectionDisplay.Refresh();


        yield return asyncOperation; 
    }

    public void ItemLoadFinished()
    {
        itemLoad = true;
    }
}
