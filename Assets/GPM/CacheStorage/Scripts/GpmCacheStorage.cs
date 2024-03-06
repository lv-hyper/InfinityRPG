using System;
using System.Collections;
using Gpm.Common;
using Gpm.Common.Util;

namespace Gpm.CacheStorage
{
    public static class GpmCacheStorage
    {
        public const string NAME = "GpmCacheStorage";
        public const string VERSION = "1.0.1";

        private static CacheStorageConfig cacheConfig;
        private static CachePackage cachePackage = new CachePackage();

        public static long updateTime = 0;

        public static CacheStorageConfig Config
        {
            get
            {
                if (cacheConfig == null)
                {
                    LoadConfig();
                }

                return cacheConfig;
            }
        }

        public static CachePackage Package
        {
            get
            {
                if (cachePackage == null)
                {
                    LoadPackage();
                }

                return cachePackage;
            }
        }


        public static event Action onChangeCache;

        public static int GetCacheCount()
        {
            return Package.cacheStorage.Count;
        }

        public static long GetCacheSize()
        {
            return Package.cachedSize;
        }

        public static void ClearCache()
        {
            Package.RemoveAll();
        }

        public static void SetMaxCount(int count = 0, bool applyStorage = true)
        {
            Config.SetMaxCount(count);

            if (applyStorage == true &&
                count > 0)
            {
                Package.SecuringStorageCount();
            }
        }

        public static int GetMaxCount()
        {
            return Config.GetMaxCount();
        }

        public static void SetMaxSize(long size = 0, bool applyStorage = true)
        {
            Config.SetMaxSize(size);

            if (applyStorage == true &&
                size > 0)
            {
                Package.SecuringStorage(size);
            }
        }

        public static long GetMaxSize()
        {
            return Config.GetMaxSize();
        }

        public static bool CheckReRequest(StringToValue<DateTime> responseTime)
        {
            return CheckReRequest(GetReRequestTime(), responseTime);
        }

        public static bool CheckReRequest(double reRequestTime, StringToValue<DateTime> responseTime)
        {
            if (reRequestTime > 0 &&
                responseTime.IsValid() == true)
            {
                if ((DateTime.UtcNow - responseTime).Seconds > reRequestTime)
                {
                    return true;
                }
            }

            return false;
        }

        public static void SetReRequestTime(double value)
        {
            Config.SetReRequestTime(value);
        }

        public static double GetReRequestTime()
        {
            return Config.GetReRequestTime();
        }

        public static void SetUnusedPeriodTime(double value)
        {
            Config.SetUnusedPeriodTime(value);
        }

        public static double GetUnusedPeriodTime()
        {
            return Config.GetUnusedPeriodTime();
        }

        public static void SetRemoveCycle(double value)
        {
            Config.SetRemoveCycle(value);
        }

        public static double GetRemoveCycle()
        {
            return Config.GetRemoveCycle();
        }

        public static CacheRequestType GetCacheRequestType()
        {
            return Config.GetCacheRequestType();
        }

        public static void SetCacheRequestType(CacheRequestType value)
        {
            Config.SetCacheRequestType(value);
        }

        static GpmCacheStorage()
        {
            initialize();
        }

        public static void SetCachePath(string path)
        {
            Config.SetCachePath(path);
        }

        public static string GetCachePath()
        {
            return Config.GetCachePath();
        }

        public static CacheInfo Request(string url, Action<GpmCacheResult> onResult)
        {
            return Request(url, Config.GetCacheRequestType(), onResult);
        }

        public static CacheInfo Request(string url, CacheRequestType requestType, Action<GpmCacheResult> onResult)
        {
            return Request(url, requestType, 0, onResult);
        }

        public static CacheInfo Request(string url, double reRequestTime, Action<GpmCacheResult> onResult)
        {
            return Request(url, Config.GetCacheRequestType(), reRequestTime, onResult);
        }

        public static CacheInfo Request(string url, CacheRequestType requestType, double reRequestTime, Action<GpmCacheResult> onResult)
        {
            return Package.Request(url, requestType, reRequestTime, onResult);
        }

        public static CacheInfo RequestHttpCache(string url, Action<GpmCacheResult> onResult)
        {
            return Request(url, CacheRequestType.ALWAYS, onResult);
        }

        public static CacheInfo RequestLocalCache(string url, Action<GpmCacheResult> onResult)
        {
            return Package.RequestLocal(url, onResult);
        }

        public static CacheInfo GetCachedTexture(string url, Action<CachedTexture> onResult)
        {
            CacheInfo info = Package.GetCacheInfo(url);
            if (info != null)
            {
                CachedTexture cachedTexture = CachedTextureManager.Get(info);
                if (cachedTexture != null)
                {
                    onResult(cachedTexture);
                    return info;
                }
            }

            return RequestLocalCache(url, (result) =>
            {
                if (result.IsSuccess() == true)
                {
                    onResult(CachedTextureManager.Cache(result.Info, false, result.Data));
                }
                else
                {
                    onResult(null);
                }
            });
        }

        public static CacheInfo RequestTexture(string url, Action<CachedTexture> onResult)
        {
            return RequestTexture(url, 0, onResult);
        }

        public static CacheInfo RequestTexture(string url, double reRequestTime, Action<CachedTexture> onResult)
        {
            CacheInfo info = Package.GetCacheInfo(url);
            if (info != null)
            {
                if (info.NeedRequest(reRequestTime) == false)
                {
                    CachedTexture cachedTexture = CachedTextureManager.Get(info);
                    if (cachedTexture != null)
                    {
                        if (cachedTexture.requested == true)
                        {
                            onResult(cachedTexture);
                            return info;
                        }
                    }
                }   
            }

            info = RequestHttpCache(url, (result) =>
            {
                if (result.IsSuccess() == true)
                {
                    onResult(CachedTextureManager.Cache(result.Info, true, result.Data));
                }
                else
                {
                    onResult(null);
                }
            });

            return info;
        }

        internal static void initialize()
        {
            GpmJsonMapper.RegisterExporter<StringToValue<int>>((sv, w) => w.Write(sv.GetText()));
            GpmJsonMapper.RegisterImporter<string, StringToValue<int>>(value => new StringToValue<int>(value));
            GpmJsonMapper.RegisterImporter<int, StringToValue<int>>(value => new StringToValue<int>(value));

            GpmJsonMapper.RegisterExporter<StringToValue<DateTime>>((sv, w) => w.Write(sv.GetValue().Ticks));
            GpmJsonMapper.RegisterImporter<long, StringToValue<DateTime>>(value => new StringToValue<DateTime>(new DateTime(value)));
            GpmJsonMapper.RegisterImporter<string, StringToValue<DateTime>>(value => new StringToValue<DateTime>(value));

            updateTime = DateTime.UtcNow.Ticks;
            
            LoadConfig();
            LoadPackage();

            ManagedCoroutine.Start(UpdateRoutine());
        }

        internal static CacheStorageConfig LoadConfig()
        {
            cacheConfig = CacheStorageConfig.Load();
            if (cacheConfig == null)
            {
                cacheConfig = new CacheStorageConfig();
            }
            return cacheConfig;
        }

        internal static CachePackage LoadPackage()
        {
            cachePackage = CachePackage.Load();
            if (cachePackage == null)
            {
                cachePackage = new CachePackage();
            }

            return cachePackage;
        }

        internal static void UpdatePackage(bool immediately = false)
        {
            if (immediately == true)
            {
                SavePackage();
            }
            else
            {
                if (Package.IsDirty() == false)
                {
                    Package.SetDirty(true);
                }
            }
        }

        private static IEnumerator UpdateRoutine()
        {
            while (cachePackage != null)
            {
                updateTime = DateTime.UtcNow.Ticks;

                Package.Update();

                AutoDeleteUnusedCache();

                if (Package.IsDirty() == true)
                {
                    SavePackage();

                    Package.SetDirty(false);
                }

                yield return null;
            }
            
        }

        internal static void SavePackage()
        {
            Package.Save();

            if (onChangeCache != null)
            {
                onChangeCache();
            }
        }

        internal static void AutoDeleteUnusedCache()
        {
            if (Config.GetUnusedPeriodTime() > 0)
            {
                Package.SecuringStorageLastAccess(Config.GetUnusedPeriodTime());
            }
        }
    }
}