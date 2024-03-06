using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace InGame.Action
{
    [Serializable]
    public abstract class BossKillCallbackable : MonoBehaviour
    {
        public abstract bool ActivateCondition();
        public abstract void Activate();
        public abstract void _Callback(UnityEngine.Object _object);
    }

    public class FairyFieldCustomTeleport : BossKillCallbackable
    {
        [SerializeField] int count = 4;

        [SerializeField] string triggerID;

        [SerializeField] string firstBossName = "";

        [SerializeField] string teleportDestMap;

        [SerializeField] List<Data.GameZone> gameZones;

        Dictionary<string, string> teleportTargetDict = new Dictionary<string, string>
        {
            {"Air Fairy",       "Air Portal"},
            {"Fire Fairy",      "Fire Portal"},
            {"Nature Fairy",    "Nature Portal"},
            {"Water Fairy",     "Water Portal"},
            {"Air Fairy [Hard]",       "Air Portal"},
            {"Fire Fairy [Hard]",      "Fire Portal"},
            {"Nature Fairy [Hard]",    "Nature Portal"},
            {"Water Fairy [Hard]",     "Water Portal"}
        };

        int originalMinLevel, originalMaxLevel;

        private void Awake()
        {
            var countKey = string.Format("{0}_count", triggerID);
            var firstBossNameKey = string.Format("{0}_firstBossName", triggerID);

            count = PlayerPrefs.GetInt(countKey, 4);
            firstBossName = PlayerPrefs.GetString(firstBossNameKey, "");

            if (ActivateCondition())
            {
                Activate();
            }
        }

        public override bool ActivateCondition()
        {
            return count == 0 && firstBossName != "";
        }

        public override void Activate()
        {
            var _character = Entity.Character.GetInstance();

            PlayerPrefs.SetString("SceneMovementTriggerFallback", teleportTargetDict[firstBossName]);

            _character.SaveCharacterData(teleportDestMap, teleportTargetDict[firstBossName]);
            _character.Teleport         (teleportDestMap, teleportTargetDict[firstBossName]);

        }

        public override void _Callback(UnityEngine.Object _object)
        {
            var boss = _object as InGame.Data.Mob.Boss;

            if (count == 4)
            {
                firstBossName = boss.name;
            }

            --count;

            if (count > 0)
            {
                foreach (var gameZone in gameZones)
                {
                    long minLv = gameZone.GetMinLv();
                    long maxLv = gameZone.GetMaxLv();

                    gameZone.SetMinLv(minLv * 57 / 50);
                    gameZone.SetMaxLv(maxLv * 57 / 50);
                }
            }

            if (ActivateCondition())
            {
                Activate();
            }
        }

        public void SetTriggerID(string v)
        {
            triggerID = v;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(FairyFieldCustomTeleport))]
    [CanEditMultipleObjects]
    public class FairyFieldCustomTeleportEditor : Editor
    {
        FairyFieldCustomTeleport _object;

        void OnEnable()
        {
            _object = target as FairyFieldCustomTeleport;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Generate Random ID", GUILayout.Width(200), GUILayout.Height(20)))
            {
                _object.SetTriggerID(System.Guid.NewGuid().ToString());
                EditorUtility.SetDirty(_object);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
#endif

}
