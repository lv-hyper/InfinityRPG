using InGame.Data;
using InGame.Data.Item;
using InGame.Data.Mob;
using InGame.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InGame.Data.Skill;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using EnemyActiveSkill = InGame.Data.Skill.EnemyActiveSkill;
using Object = UnityEngine.Object;

public class Test : MonoBehaviour
{

    [MenuItem("Assets/Bind Image")]
    public static void BindImage()
    {
        var _asset = Selection.activeObject as UnityEngine.Tilemaps.Tile;
        var _image = AssetDatabase.LoadAssetAtPath("/Assets/Sprites/Seasonal maps/" + _asset.name, typeof(Sprite)) as Sprite;

        Debug.Log(_image);
        _asset.sprite = _image;
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Hard Mobs")]
    public static void GenerateHardMobs()
    {
        var currentScene = EditorSceneManager.GetActiveScene();

        GameZone[] gameZones = FindObjectsOfType(typeof(GameZone)) as GameZone[];

        foreach (var e in gameZones)
        {
            e.SetMinLv(e.GetMinLv() * 100);
            e.SetMaxLv(e.GetMaxLv() * 100);
            Debug.Log(e.GetBaseLv());

            var _mobList = new List<InGame.Data.Mob.AbstractMob>(e.GetMobList());

            for (int i = 0; i < _mobList.Count; i++)
            {
                var mobName = _mobList[i].name;

                var test = AssetDatabase.FindAssets(mobName + " [Hard] t:Mob");

                _mobList[i] = (AbstractMob)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(test[0]), typeof(AbstractMob));
            }

            e.PromoteMob(_mobList);


            //e.PromoteMob();

        }

        EditorSceneManager.SaveScene(currentScene, currentScene.path);
    }

    [MenuItem("Utilities/Generate Hard Boss")]
    public static void GenerateHardBoss()
    {
        var currentScene = EditorSceneManager.GetActiveScene();

        BossEncounter[] bosses = FindObjectsOfType(typeof(BossEncounter)) as BossEncounter[];

        foreach (var e in bosses)
        {
            var _boss = e.GetMob();
            var test = AssetDatabase.FindAssets(_boss.name + " [Hard] t:Boss");
            e.SetMob((Boss)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(test[0]), typeof(Boss)));

        }
        EditorSceneManager.SaveScene(currentScene, currentScene.path);
    }

   
    [CanEditMultipleObjects]
    [MenuItem("Assets/Utilities/Convert To ItemCraft")]
    static void ItemToItemCraft()
    {
        foreach (var _obj in Selection.objects)
        {
            Item item = _obj as Item;
            if (item == null) continue;

            Craft craft = ScriptableObject.CreateInstance<Craft>();

            craft.name = item.name;
            craft.SetResult(item);

            AssetDatabase.CreateAsset(craft, "Assets/ScriptableObject/ItemCraft/Elemental/" + craft.name + ".asset");
            EditorUtility.SetDirty(craft);
        }
        AssetDatabase.SaveAssets();


    }
    
    /*
     * public static void TestMethod()
       {
           var items = AssetDatabase.LoadAllAssetsAtPath("Assets/StreamingAssets/AssetBundles/");
           foreach (var guid in AssetDatabase.FindAssets("", new string[] { "Assets/ScriptableObject/Item" }))
           {
               var item = (Item)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Item));

               if (item == null) continue;

               item.legacyID = item.itemID;

               var itemClone = ScriptableObject.Instantiate(item);
               AssetDatabase.CreateAsset(itemClone, AssetDatabase.GUIDToAssetPath(guid));
           }

           AssetDatabase.SaveAssets();
       }
     */
    
    private static AssetReferenceT<Sprite> ConvertSpriteSheetReference(Sprite sprite)
    {
        if (sprite == null) return null;

        string assetPath = AssetDatabase.GetAssetPath(sprite);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer != null)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AssetReferenceT<Sprite> spriteRef = new AssetReferenceT<Sprite>(guid);

            // For multiple mode, we need to set the sub-asset name after creation
            if (importer.spriteImportMode == SpriteImportMode.Multiple)
            {
                spriteRef.SetEditorSubObject(sprite);
            }
            return spriteRef;
        }

        return null;
    }
}
