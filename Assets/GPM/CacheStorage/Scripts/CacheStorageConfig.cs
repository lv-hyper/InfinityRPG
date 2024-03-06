using UnityEngine;
using System;
using System.IO;

namespace Gpm.CacheStorage
{
    using Common.Util;

    [Serializable]
    public class CacheStorageConfig
    {
        public const string CONFIG_NAME = "CacheStorageConfig";

        [SerializeField]
        private string cachePath;

        [SerializeField]
        private int maxCount;

        [SerializeField]
        private long maxSize;

        [SerializeField]
        private double reRequestTime;

        [SerializeField]
        private double unusedPeriodTime;

        [SerializeField]
        private double removeCycle = 1;

        [SerializeField]
        private CacheRequestType defaultRequestType = CacheRequestType.FIRSTPLAY;

        public string GetCachePath()
        {
            if (string.IsNullOrEmpty(cachePath) == true)
            {
                SetCachePath(Application.temporaryCachePath);
            }

            return cachePath;
        }

        public void SetCachePath(string path)
        {
            cachePath = Path.Combine(path, GpmCacheStorage.NAME);
            Save();
        }

        public int GetMaxCount()
        {
            return maxCount;
        }

        public void SetMaxCount(int value)
        {
            maxCount = value;
            Save();
        }

        public long GetMaxSize()
        {
            return maxSize;
        }

        public void SetMaxSize(long value)
        {
            maxSize = value;
            Save();
        }
        
        public double GetReRequestTime()
        {
            return reRequestTime;
        }

        public void SetReRequestTime(double value)
        {
            reRequestTime = value;
            Save();
        }

        public double GetUnusedPeriodTime()
        {
            return unusedPeriodTime;
        }

        public void SetUnusedPeriodTime(double value)
        {
            unusedPeriodTime = value;
            Save();
        }

        public double GetRemoveCycle()
        {
            return removeCycle;
        }

        public void SetRemoveCycle(double value)
        {
            removeCycle = value;
            Save();
        }

        public CacheRequestType GetCacheRequestType()
        {
            return defaultRequestType;
        }

        public void SetCacheRequestType(CacheRequestType value)
        {
            defaultRequestType = value;
            Save();
        }

        private static string ConfigPath()
        {
            return Path.Combine(Application.persistentDataPath, CONFIG_NAME);
        }

        public static CacheStorageConfig Load()
        {
            CacheStorageConfig config = null;
            try
            {
                string path = ConfigPath();
                if (File.Exists(path) == true)
                {
                    config = GpmJsonMapper.ToObject<CacheStorageConfig>(File.ReadAllText(path));
                }
                
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }

            return config;
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(ConfigPath(), GpmJsonMapper.ToJson(this));
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
    }
} 