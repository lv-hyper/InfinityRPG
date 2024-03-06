using System.Net;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace Gpm.CacheStorage
{
    using Common;
    using Common.Util;

    public enum CacheRequestType
    { 
        ALWAYS,
        FIRSTPLAY,
        ONCE,
        LOCAL,
    }

    [Serializable]
    public class CachePackage
    {
        public const string PACKAGE_NAME = "CacheStoragePackage";

        [SerializeField]
        public List<CacheInfo> cacheStorage = new List<CacheInfo>();

        [SerializeField]
        internal int lastIndex = 0;

        [SerializeField]
        internal long cachedSize = 0;

        [SerializeField]
        private List<int> spaceIdx = new List<int>();

        [SerializeField]
        public List<CacheInfo> removeCache = new List<CacheInfo>();

        private List<CacheInfo> requestCache = new List<CacheInfo>();

        private bool dirty = false;

        public DateTime lastRemoveTime;

        public void OnAfterDeserialize()
        {
            foreach (var info in cacheStorage)
            {
                info.storage = this;
            }

            foreach (var info in removeCache)
            {
                info.storage = this;
            }
        }

        internal CacheInfo GetCacheInfo(string url)
        {
            foreach (CacheInfo cachInfo in cacheStorage)
            {
                if (cachInfo.url.Equals(url) == true)
                {
                    return cachInfo;           
                }
            }

            return null;
        }

        public CacheInfo RequestLocal(string url, Action<GpmCacheResult> onResult)
        {
            CacheInfo info = GetCacheInfo(url);

            byte[] datas = null;
            if (info != null)
            {
                datas = GetCacheData(info);
            }
            onResult(new GpmCacheResult(info, datas));

            return info;
        }

        public CacheInfo Request(string url, CacheRequestType requestType, double reRequestTime, Action<GpmCacheResult> onResult)
        {
            foreach (var rq in requestCache)
            {
                if (rq.url.Equals(url) == true)
                {
                    return rq;
                }
            }

            CacheInfo info = GetCacheInfo(url);
            bool useCache = true;
            if (info == null)
            {
                info = new CacheInfo(this, url);
                useCache = false;
            }

            System.Action<byte[]> OnData = (datas) =>
            {
                info.lastAccess = DateTime.UtcNow.Ticks;
                GpmCacheStorage.UpdatePackage();

                onResult(new GpmCacheResult(info, datas));
            };

            if (requestType == CacheRequestType.LOCAL ||
                Application.internetReachability == NetworkReachability.NotReachable)
            {
                byte[] datas = GetCacheData(info);
                OnData(datas);
                return info;
            }

            if (useCache == true &&
                requestType != CacheRequestType.ALWAYS &&
                info.NeedRequest() == false)
            {
                bool useLocalCache = false;
                if (requestType == CacheRequestType.FIRSTPLAY)
                {
                    if (info.requestInPlay == true)
                    {
                        useLocalCache = true;
                    }
                }
                else if (requestType == CacheRequestType.ONCE)
                {
                    useLocalCache = true;
                }

                if (reRequestTime == 0)
                {
                    reRequestTime = GpmCacheStorage.GetReRequestTime();
                }
                if (GpmCacheStorage.CheckReRequest(reRequestTime, info.responseTime) == true)
                {
                    useLocalCache = true;
                }

                if (useLocalCache == true)
                {
                    if (IsValidCacheData(info) == true)
                    {
                        byte[] datas = GetCacheData(info);
                        if (datas != null)
                        {
                            OnData(datas);
                            return info;
                        }
                    }

                    useCache = false;
                }
            }

            info.state = CacheInfo.State.REQUEST;
            requestCache.Add(info);

            GpmWebRequest request = new GpmWebRequest();
            if (useCache == true)
            {
                if (string.IsNullOrEmpty(info.eTag) == false)
                {
                    request.SetRequestHeader("If-None-Match", info.eTag);
                }
                if (string.IsNullOrEmpty(info.lastModified) == false)
                {
                    request.SetRequestHeader("If-Modified-Since", info.lastModified.GetValue().ToUniversalTime().ToString("r"));
                }
            }

            info.requestTime = DateTime.UtcNow;
            info.requestInPlay = true;
            request.Get(url, (requestResult) =>
            {
                info.state = CacheInfo.State.NONE;
                requestCache.Remove(info);

                if (requestResult.isSuccess == true)
                {
                    if (requestResult.responseCode == (long)HttpStatusCode.NotModified)
                    {
                        byte[] datas = GetCacheData(info);
                        if (datas != null)
                        {
                            info.state = CacheInfo.State.CACHED;
                            OnData(datas);
                        }
                        else
                        {
                            // Request again if no data
                            info.eTag = string.Empty;
                            Request(url, CacheRequestType.ALWAYS, 0, onResult);
                        }
                    }
                    else if (requestResult.responseCode == (long)HttpStatusCode.OK)
                    {
                        CacheControl cacheControl = null;

                        Dictionary<string, string> cacheControlElements = CacheControl.GetElements(requestResult.request.GetResponseHeader("cache-control"));

                        bool noStore = false;
                        if (cacheControlElements.Count > 0)
                        {
                            noStore = cacheControlElements.ContainsKey("no-store") == true;
                            cacheControl = new CacheControl(cacheControlElements);
                        }
                        info.cacheControl = cacheControl;
                        
                        info.eTag = requestResult.request.GetResponseHeader("ETag");
                        info.expires = requestResult.request.GetResponseHeader("Expires");
                        info.expires = info.expires.GetValue().ToUniversalTime();
                        info.lastModified = requestResult.request.GetResponseHeader("Last-Modified");

                        info.age = requestResult.request.GetResponseHeader("Age");
                        info.date = requestResult.request.GetResponseHeader("Date");
                        info.date = info.date.GetValue().ToUniversalTime();

                        info.contentLength = requestResult.request.GetResponseHeader("Content-Length");

                        info.cacheControl = cacheControl;

                        info.responseTime = DateTime.UtcNow;

                        info.CaculateCacheInfo();

                        byte[] datas = requestResult.request.downloadHandler.data;

                        if (datas != null)
                        {
                            if (noStore == true)
                            {
                                if(info.IsCached() == true)
                                {
                                    RemoveCacheData(info);
                                }
                            }
                            else
                            {
                                AddCacheData(info, datas);
                            }
                            
                        }

                        OnData(datas);
                    }
                    else
                    {
                        OnData(null);
                    }
                }
                else
                {
                    OnData(null);
                }
            });
            return info;
        }

        public bool IsCached(CacheInfo info)
        {
            return info.index > 0;
        }

        internal string GetCacheDataPath(CacheInfo info)
        {
            if (IsCached(info) == true)
            {
                return Path.Combine(GpmCacheStorage.GetCachePath(), info.index.ToString());
            }

            return "";
        }

        public void SaveCacheData(CacheInfo info, byte[] data)
        {
            if (Directory.Exists(GpmCacheStorage.GetCachePath()) == false)
            {
                Directory.CreateDirectory(GpmCacheStorage.GetCachePath());
            }
            string filePath = GetCacheDataPath(info);

            File.WriteAllBytes(filePath, data);
        }

        public bool IsValidCacheData(CacheInfo info)
        {
            string filePath = GetCacheDataPath(info);

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists == true &&
                fileInfo.Length == info.contentLength)
            {
                return true;
            }

            return false;
        }

        public byte[] GetCacheData(CacheInfo info)
        {
            string filePath = GetCacheDataPath(info);

            return File.ReadAllBytes(filePath);
        }

        public string GetCacheData(CacheInfo info, System.Text.Encoding encoding = null)
        {
            byte[] data = GetCacheData(info);

            if (encoding == null)
            {
                encoding = System.Text.Encoding.Default;
            }

            return encoding.GetString(data);
        }

        public void AddCacheData(CacheInfo info, byte[] datas)
        {
            long maxCount = GpmCacheStorage.GetMaxCount();
            if (maxCount > 0)
            {
                SecuringStorageCount(1);
            }

            long maxSize = GpmCacheStorage.GetMaxSize();
            if(maxSize > 0)
            {
                SecuringStorage(maxSize, datas.LongLength);
            }

            if (spaceIdx.Count > 0)
            {
                info.index = spaceIdx[0];
                spaceIdx.RemoveAt(0);
            }
            else
            {
                info.index = ++lastIndex;
            }

            cachedSize += info.contentLength;
            info.state = CacheInfo.State.CACHED;

            SaveCacheData(info, datas);

            cacheStorage.Add(info);

            GpmCacheStorage.UpdatePackage();
        }

        public void CacheSort()
        {
            cacheStorage.Sort();
        }

        public bool IsDirty()
        {
            return dirty;
        }

        public void SetDirty(bool dirty = true)
        {
            this.dirty = dirty;
        }

        public void SecuringStorageLastAccess(double unusedTime)
        {
            List<CacheInfo> newExpired = new List<CacheInfo>();
            for (int i = 0; i < cacheStorage.Count; i++)
            {
                if (cacheStorage[i].IsLastAccessPeriod(unusedTime) == true)
                {
                    newExpired.Add(cacheStorage[i]);
                }
            } 

            for (int i = 0; i < newExpired.Count; i++)
            {
                if (RemoveCacheData(newExpired[i]) == false)
                {
                    break;
                }
            }
        }

        public void SecuringStorageCount(int addCount = 0)
        {
            long maxCount = GpmCacheStorage.GetMaxCount();
            if (maxCount <= 0)
            {
                return;
            }

            if (cacheStorage.Count + addCount > maxCount)
            {
                CacheSort();
            }

            while (cacheStorage.Count + addCount > maxCount)
            {
                if (RemoveCacheData(cacheStorage.Last<CacheInfo>(), true) == false)
                {
                    break;
                }
            }
        }


        public void SecuringStorage(long maxSize, long addSize = 0)
        {
            if(maxSize == 0)
            {
                return;
            }

            if (addSize > maxSize)
            {
                return;
            }

            if (cacheStorage.Count > 0 &&
                cachedSize + addSize > maxSize)
            {
                CacheSort();

                while ( cacheStorage.Count > 0 &&
                        cachedSize + addSize > maxSize)
                {
                    if (RemoveCacheData(cacheStorage.Last<CacheInfo>(), true) == false)
                    {
                        break;
                    }
                }
            }
        }

        public bool RemoveCacheData(CacheInfo info, bool immediately = false)
        {
            if (info.IsCached() == false)
            {
                return true;
            }

            if (cacheStorage.Remove(info) == true)
            {
                if (immediately == true)
                {
                    DeleteCacheData(info);
                }
                else
                {
                    removeCache.Add(info);
                }

                cachedSize -= info.contentLength;

                GpmCacheStorage.UpdatePackage();

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool DeleteCacheData(CacheInfo info)
        {
            try
            {
                if (info.IsCached() == true)
                {
                    string filePath = GetCacheDataPath(info);
                    File.Delete(filePath);
                    spaceIdx.Add(info.index);
                    spaceIdx.Sort();

                    while (spaceIdx.Count > 0 &&
                            spaceIdx[spaceIdx.Count - 1] >= lastIndex)
                    {
                        spaceIdx.RemoveAt(spaceIdx.Count - 1);
                        lastIndex--;
                    }

                    GpmCacheStorage.UpdatePackage();
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }

            return false;
        }


        public void Remove()
        {
            Directory.Delete(GpmCacheStorage.GetCachePath());

            lastIndex = 0;
            cachedSize = 0;
            cacheStorage.Clear();
            spaceIdx.Clear();
        }

        public void RemoveAll()
        {
            foreach (CacheInfo info in cacheStorage)
            {
                try
                {
                    string filePath = GetCacheDataPath(info);
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }

            lastIndex = 0;
            cachedSize = 0;
            cacheStorage.Clear();
            spaceIdx.Clear();

            GpmCacheStorage.UpdatePackage();
        }

        public void Update()
        {
            if (CanRemove() == true)
            {
                int removeIndex = 0;
                for (int idx = 0; idx < removeCache.Count; idx++)
                {
                    if (removeCache[removeIndex].index < removeCache[idx].index)
                    {
                        removeIndex = idx;
                    }
                }

                DeleteCacheData(removeCache[removeIndex]);

                removeCache.RemoveAt(removeIndex);
                lastRemoveTime = DateTime.UtcNow;
            }
        }

        public bool CanRemove()
        {
            if (removeCache.Count > 0)
            {
                double removePeriodTime = GpmCacheStorage.GetRemoveCycle();
                if (removePeriodTime > 0)
                {
                    if ((DateTime.UtcNow - lastRemoveTime).TotalSeconds > removePeriodTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string PackagePath()
        {
            return Path.Combine(GpmCacheStorage.GetCachePath(), PACKAGE_NAME);
        }        

        public static CachePackage Load()
        {
            CachePackage cachePackage = null;

            string path = PackagePath();
            if (File.Exists(path) == true)
            {
                try
                {
                    cachePackage = GpmJsonMapper.ToObject<CachePackage>(File.ReadAllText(path));

                    cachePackage.OnAfterDeserialize();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }

            return cachePackage;
        }

        public void Save()
        {
            if(Directory.Exists(GpmCacheStorage.GetCachePath()) == false)
            {
                Directory.CreateDirectory(GpmCacheStorage.GetCachePath());
            }
            
            File.WriteAllText(PackagePath(), GpmJsonMapper.ToJson(this));
        }
    }
}