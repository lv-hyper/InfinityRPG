using System.Collections;
using System.Collections.Generic;
using System.IO;
using InGame.Data.Item.Group;
using UnityEngine;
using UnityEngine.Networking;

public class ItemBundleLoader : MonoBehaviour
{
    [SerializeField] List<string> paths;
    [SerializeField] List<string> purpose;
    [SerializeField] GameObject loadingLayer;

    private void Awake() {
        //Set framerate temporarily on this script. need to be changed.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 1f / 60f;
        Time.maximumDeltaTime = 1f / 3f;
        Shader.WarmupAllShaders();

        string recentVersion = PlayerPrefs.GetString("recentVersion", "none");
        if(recentVersion != Application.version)
        {
        }
        PlayerPrefs.SetString("recentVersion", Application.version);

        for(int i=0; i<paths.Count; i++)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                paths[i] = Path.Combine(Application.streamingAssetsPath, paths[i]);

            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                paths[i] = Path.Combine(Application.streamingAssetsPath, paths[i]);

            }
            else
            {
                paths[i] = Path.Combine(Application.streamingAssetsPath, paths[i]);

            }
        }
        
        StartCoroutine(LoadFromMemoryAsync(
            paths
        ));
    }

    IEnumerator LoadFromMemoryAsync(List<string> paths) {
        byte[] bytes = null;
        List<AssetBundle> bundles = new List<AssetBundle>();

        loadingLayer.SetActive(true);
        int i = 0;
        foreach (string path in paths)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                UnityWebRequest www = UnityWebRequest.Get(path);

                Debug.Log(path);

                yield return www.SendWebRequest();

                if (www.isDone)
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

            switch (purpose[i])
            {
                case "Item":
                    SendItems(bundle);
                    break;
                case "Skill":
                    SendSkills(bundle);
                    break;
                case "Ability":
                    SendAbilities(bundle);
                    break;
                case "ItemGroup":
                    SendItemGroups(bundle);
                    break;
                case "Mob":
                    SendMobs(bundle);
                    break;
                case "BoardSection":
                    SendBoardSections(bundle);
                    break;
                case "ElementalTree":
                    SendElementalTrees(bundle);
                    break;
                default:
                    break;

            }

            bundles.Add(bundle);

            ++i;
        }

        foreach(var bundle in bundles)
        {
            var asyncOperation = bundle.UnloadAsync(false);
            yield return asyncOperation;
        }

        loadingLayer.SetActive(false);
    }

    public void SendItems(AssetBundle bundle)
    {
        var items = bundle.LoadAllAssets<InGame.Data.Item.Item>();

        foreach (var item in items)
        {
            InGame.Data.Item.ItemCollection.GetInstance().AddItem(item);
        }
    }

    public void SendSkills(AssetBundle bundle)
    {
        var skills = bundle.LoadAllAssets<InGame.Data.Skill.CharacterSkill>();

        foreach (var skill in skills)
        {
            InGame.Data.Skill.SkillCollection.GetInstance().AddSkill(skill);
        }
    }

    public void SendAbilities(AssetBundle bundle)
    {
        var abilities = bundle.LoadAllAssets<InGame.Data.Ability.Ability>();

        InGame.Data.Ability.AbilityCollection.GetInstance().ResetAbility();

        foreach (var ability in abilities)
        {
            InGame.Data.Ability.AbilityCollection.GetInstance().AddAbility(ability);
        }
    }

    public void SendMobs(AssetBundle bundle)
    {
        var mobs = bundle.LoadAllAssets<InGame.Data.Mob.AbstractMob>();

        InGame.Data.Mob.MobCollection.GetInstance().SetList(
            new List<InGame.Data.Mob.AbstractMob>(mobs)
        );
    }

    public void SendItemGroups(AssetBundle bundle)
    {
        if (!ItemSetCollection.GetInstance().Loaded())
        { 
            var itemGroups = bundle.LoadAllAssets<InGame.Data.Item.Group.ItemGroup>();

            foreach (var itemGroup in itemGroups)
            {
                itemGroup.EnrollToCollection();
            }

            ItemSetCollection.GetInstance().SetLoadFinished();
        }
    }

    public void SendBoardSections(AssetBundle bundle)
    {
        var boardSections = bundle.LoadAllAssets<InGame.UI.BoardSection>();

        InGame.UI.BoardSectionCollection.GetInstance().ClearCollection();

        foreach (var boardSection in boardSections)
        {
            InGame.UI.BoardSectionCollection.GetInstance().AddBoardSection(boardSection);
        }
    }

    public void SendElementalTrees(AssetBundle bundle)
    {
        var elementalTrees = bundle.LoadAllAssets<InGame.Data.ElementalTree>();

        InGame.Data.ElementalTreeCollection.GetInstance().ResetCollection();

        foreach(var elementalTree in elementalTrees)
        {
            InGame.Data.ElementalTreeCollection.GetInstance().AddElementalTree(elementalTree);
        }

        Debug.Log("Wahoo");
        InGame.Data.ElementalTreeCollection.GetInstance().SetElementalTreeElementRequirement();
        Debug.Log("asdf");
    }

}
