using InGame.Data;
using InGame.Data.Item;
using InGame.Data.Mob;
using InGame.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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


    [MenuItem("Utilities/test")]

    public static void TestMethod()
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

    [CanEditMultipleObjects]
    [MenuItem("Assets/Utilities/Convert To Item")]
    static void ImageToItem()
    {
        foreach (var _obj in Selection.objects)
        {
            Sprite sprite = _obj as Sprite;
            if (sprite == null) continue;

            Debug.Log("Selected Image : " + sprite);

            string suffix = sprite.name.Split(' ').Last();

            

            switch (suffix)
            {
                case "Sword":
                    InGame.Data.Item.Weapon sword = ScriptableObject.CreateInstance<Weapon>();
                    sword.itemSprite = sprite;
                    sword.name = sprite.name;
                    sword.weaponType = Weapon.WeaponType.Melee;
                    sword.zone = InGame.Data.Item.ItemCategory.Zone.FrozenShores;
                    AssetDatabase.CreateAsset(sword, "Assets/ScriptableObject/Item/Weapon/Frozen Shores/"+sword.name+".asset");
                    EditorUtility.SetDirty(sword);
                    break;
                case "Bow":
                    InGame.Data.Item.Weapon bow = ScriptableObject.CreateInstance<Weapon>();
                    bow.itemSprite = sprite;
                    bow.name = sprite.name;
                    bow.weaponType = Weapon.WeaponType.Ranged;
                    bow.zone = InGame.Data.Item.ItemCategory.Zone.FrozenShores;
                    AssetDatabase.CreateAsset(bow, "Assets/ScriptableObject/Item/Weapon/Frozen Shores/"+bow.name + ".asset");
                    EditorUtility.SetDirty(bow);
                    break;
                case "Staff":
                    InGame.Data.Item.Weapon staff = ScriptableObject.CreateInstance<Weapon>();
                    staff.itemSprite = sprite;
                    staff.name = sprite.name;
                    staff.weaponType = Weapon.WeaponType.Staff;
                    staff.zone = InGame.Data.Item.ItemCategory.Zone.FrozenShores;
                    AssetDatabase.CreateAsset(staff, "Assets/ScriptableObject/Item/Weapon/Frozen Shores/"+staff.name + ".asset");
                    EditorUtility.SetDirty(staff);
                    break;
                case "Helmet":
                    InGame.Data.Item.Armor.Helmet helmet = ScriptableObject.CreateInstance<InGame.Data.Item.Armor.Helmet>();
                    helmet.itemSprite = sprite;
                    helmet.name = sprite.name;
                    helmet.zone = InGame.Data.Item.ItemCategory.Zone.FrozenShores;
                    AssetDatabase.CreateAsset(helmet, "Assets/ScriptableObject/Item/Armor/Frozen Shores/"+helmet.name + ".asset");
                    EditorUtility.SetDirty(helmet);
                    break;
                case "Armor":
                    InGame.Data.Item.Armor.Robe armor = ScriptableObject.CreateInstance<InGame.Data.Item.Armor.Robe>();
                    armor.itemSprite = sprite;
                    armor.name = sprite.name;
                    armor.zone = InGame.Data.Item.ItemCategory.Zone.FrozenShores;
                    AssetDatabase.CreateAsset(armor, "Assets/ScriptableObject/Item/Armor/Frozen Shores/"+armor.name + ".asset");
                    EditorUtility.SetDirty(armor);
                    break;
                case "Boots":
                    InGame.Data.Item.Armor.Greave boots = ScriptableObject.CreateInstance<InGame.Data.Item.Armor.Greave>();
                    boots.itemSprite = sprite;
                    boots.name = sprite.name;
                    boots.zone = InGame.Data.Item.ItemCategory.Zone.FrozenShores;
                    AssetDatabase.CreateAsset(boots, "Assets/ScriptableObject/Item/Armor/Frozen Shores/"+boots.name + ".asset");
                    EditorUtility.SetDirty(boots);
                    break;
                case "Gloves":
                    InGame.Data.Item.Armor.Glove glove = ScriptableObject.CreateInstance<InGame.Data.Item.Armor.Glove>();
                    glove.itemSprite = sprite;
                    glove.name = sprite.name;
                    glove.zone = InGame.Data.Item.ItemCategory.Zone.FrozenShores;
                    AssetDatabase.CreateAsset(glove, "Assets/ScriptableObject/Item/Armor/Frozen Shores/"+glove.name + ".asset");
                    EditorUtility.SetDirty(glove);
                    break;
                default:
                    InGame.Data.Item.Item acc = ScriptableObject.CreateInstance<InGame.Data.Item.Item>();
                    acc.itemSprite = sprite;
                    acc.name = sprite.name;
                    acc.zone = InGame.Data.Item.ItemCategory.Zone.FrozenShores;
                    AssetDatabase.CreateAsset(acc, "Assets/ScriptableObject/Item/Material/Frozen Shores/" + acc.name + ".asset");
                    EditorUtility.SetDirty(acc);
                    break;

            }
        }
        AssetDatabase.SaveAssets();


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

    [CanEditMultipleObjects]
    [MenuItem("Assets/Utilities/Reduce Boss Level")]
    static void ReduceBossLevel()
    {
        foreach (var _obj in Selection.objects)
        {
            AbstractMob _boss = _obj as AbstractMob;
            if (_boss == null) continue;
            if (_boss as Boss != null) continue;

            if(_boss.GetDifficulty() == "hard")
                _boss.SetLV((long)_boss.GetLV()*100);

            EditorUtility.SetDirty(_boss);
        }
        AssetDatabase.SaveAssets();
    }
}
