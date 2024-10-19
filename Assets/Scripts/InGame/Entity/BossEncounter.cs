using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InGame.Data;
using System.Numerics;

namespace InGame.Entity
{

    public class BossEncounter : MonoBehaviour
    {
        [SerializeField] Data.Mob.Boss boss;
        [SerializeField] bool scaleToZoneLevel;
        [SerializeField] float scaleRatio = 1.0f;

        [SerializeField] int additionalEnergy;

        [SerializeField] bool isUnextractable = false;

        [SerializeField] string bossID;

        [SerializeField] Transform returnPoint;

        [SerializeField] bool isTeleport = false;
        [SerializeField] string teleportDestMap, teleportTargetID;

        [SerializeField] List<Action.BossKillCallbackable> bossKillCallbacks;

        bool tmpIsKilledNow = false; // TODO : refactor this

        public int GetAdditionalEnergy()
        {
            return additionalEnergy;
        }

        public bool IsUnextractable()
        {
            return isUnextractable;
        }

        private void Awake()
        {
            var _character = Entity.Character.GetInstance();
            if (_character != null && _character.isExtracted(bossID))
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
            }

            if (BossKillData.GetInstance().IsBossKilled(bossID))
            {
                Destroy(gameObject);
            }
        }

        public BigInteger GetHP()
        {
            return boss.GetHP();
        }

        public BigInteger GetATK()
        {
            return boss.GetATK();
        }

        public BigInteger GetLV()
        {
            if(scaleToZoneLevel)
            {
                return (long)(Entity.Character.GetInstance().GetCurrentGameZone().GetMaxLv() * scaleRatio);
            }
            return boss.GetLV();
        }

        public System.Numerics.BigInteger GetEXP()
        {
            return boss.GetEXP();
        }


        public BigInteger GetGold()
        {
            return boss.GetGold();
        }

        public Sprite GetSprite()
        {
            return boss.GetSprite();
        }

        public UnityEngine.Vector2 GetImageOffset() {
            return boss.GetImageOffset();
        }

        public Data.Mob.AbstractMob GenerateBattleInstance()
        {
            return boss.SummonNewMobInstance(GetLV());
        }

        public void Kill()
        {
            tmpIsKilledNow = true;
            BossKillData.GetInstance().SetBossKilled(bossID);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Data.BattleInstance.IMobAction mobAction;
            mobAction = GetComponent<Data.BattleInstance.IMobAction>();

            if(BossKillData.GetInstance().IsBossKilled(bossID))
            {
                if (mobAction != null)
                {
                    if(mobAction.GetType() == typeof(Data.BattleInstance.InstantConvertItemAction))
                    {
                        if(tmpIsKilledNow)
                            mobAction.Action(bossID);
                    }
                    else
                        mobAction.Action(bossID);
                }

                foreach(var _callback in bossKillCallbacks)
                {
                    _callback._Callback(boss);
                }
            }




        }
        public Transform GetReturnPoint() { return returnPoint; }
        public string GetBossID() { return bossID; }

        public Data.Mob.Boss GetMob() { return boss; }
        public void SetMob(Data.Mob.Boss boss) { this.boss = boss; }

        public bool GetTeleportTarget(out string teleportDestMap, out string teleportTargetID)
        {
            if(isTeleport)
            {
                teleportDestMap = this.teleportDestMap;
                teleportTargetID = this.teleportTargetID;
                return true;
            }
            else
            {
                teleportDestMap = null;
                teleportTargetID = null;
                return false;
            }
        }
    }
}