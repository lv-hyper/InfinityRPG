using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InGame.Data.Mob
{
    public class MobCollection
    {

        private static MobCollection instance;
        public List<AbstractMob> mobList;

        public static MobCollection GetInstance()
        {
            if (instance == null)
            {
                instance = new MobCollection();
            }

            return instance;
        }

        MobCollection()
        {
            mobList = new List<AbstractMob>();
        }

        public void AddMob(AbstractMob mob)
        {
            mobList.Add(mob);
        }

        public void SetList(List<AbstractMob> mobs)
        {
            mobList = mobs;
            SortList();
        }

        public void SortList()
        {
            mobList.Sort(
                (mob1, mob2) =>
                {
                    var _mobLv1 = mob1.GetLV();
                    var _mobLv2 = mob2.GetLV();

                    if (_mobLv1 != _mobLv2)
                        return _mobLv1.CompareTo(_mobLv2);

                    if (mob1.IsBoss() != mob2.IsBoss())
                        return mob1.IsBoss() ? 1 : -1;

                    if (mob1.name != mob2.name)
                        return mob1.name.CompareTo(mob2.name);

                    return 0;
                }
            );
        }

        public List<AbstractMob> GetList()
        {
            return mobList;
        }

        public List<AbstractMob> FindMobsWithItem(Data.Item.Item item)
        {
            List<AbstractMob> abstractMobs = mobList.FindAll(e => {
                if (e.GetDropTable().Count > 0)
                    return e.GetDropTable().Select(e => { return e.item.itemID; }).Contains(item.itemID);
                else
                    return false;
            });

            return abstractMobs;
        }
    }

}