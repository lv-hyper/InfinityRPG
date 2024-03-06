namespace Gpm.CacheStorage.Sample
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Gpm.CacheStorage;
    using System.Collections;
    using UnityEngine.Networking;

    public class CacheStorageSample : MonoBehaviour
    {
        public InputField url;

        public RawImage testUnityWebRequestImage;
        public Text testUnityWebRequestTime;
        public Text testUnityWebRequestResponseSize;

        public RawImage testCacheStorageImage;
        public Text testCacheStorageTime;
        public Text testCacheStorageResponseSize;

        public RawImage testCacheStorage_LocalImage;
        public Text testCacheStorage_LocalTime;
        public Text testCacheStorage_LocalResponseSize;

        public Dropdown removeCacheIndex;

        public Text cacheSizeText;
        public Text cacheMaxSizeText;
        public Text cacheCountText;
        public Text cacheMaxCountText;

        public InputField cacheMaxSizeInputField;
        public InputField cacheMaxCountInputField;


        public void Start()
        {
            cacheSizeText.text = SizeString(GpmCacheStorage.GetCacheSize());
            cacheCountText.text = GpmCacheStorage.GetCacheCount().ToString();

            cacheMaxSizeInputField.text = GpmCacheStorage.GetMaxSize().ToString();
            cacheMaxCountInputField.text = GpmCacheStorage.GetMaxCount().ToString();

            cacheMaxSizeInputField.onEndEdit.RemoveAllListeners();
            cacheMaxSizeInputField.onEndEdit.AddListener((value) =>
            {
                long size = 0;
                if (long.TryParse(value, out size) == true)
                {
                    GpmCacheStorage.SetMaxSize(size);
                }
            });

            cacheMaxCountInputField.onEndEdit.RemoveAllListeners();
            cacheMaxCountInputField.onEndEdit.AddListener((value) =>
            {
                int count = 0;
                if (int.TryParse(value, out count) == true)
                {
                    GpmCacheStorage.SetMaxCount(count);
                }
            });

            SettingScroll();

            GpmCacheStorage.onChangeCache += () =>
            {
                cacheSizeText.text = SizeString(GpmCacheStorage.GetCacheSize());
                cacheCountText.text = GpmCacheStorage.GetCacheCount().ToString();

                SettingScroll();
            };
        }

        private void SettingScroll()
        {
            removeCacheIndex.ClearOptions();

            var add = new List<string>();
            for (int i = 0; i < GpmCacheStorage.GetCacheCount(); i++)
            {
                add.Add(i.ToString());
            }
            removeCacheIndex.AddOptions(add);

            removeCacheIndex.RefreshShownValue();
        }

        public void ReuqestUnityWebRequest()
        {
            StartCoroutine(TestUnityWebRequest(url.text));
        }

        private IEnumerator TestUnityWebRequest(string url)
        {
            testUnityWebRequestImage.texture = null;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            using (UnityWebRequest request = new UnityWebRequest())
            {
                request.method = UnityWebRequest.kHttpVerbGET;
                request.downloadHandler = new DownloadHandlerBuffer();
                request.url = url;
                request.useHttpContinue = false;

                request.SendWebRequest();

                yield return request;

                while (request.isDone == false)
                {
                    yield return null;
                }

                sw.Stop();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(request.downloadHandler.data);
                    texture.Apply();

                    testUnityWebRequestImage.texture = texture;

                    testUnityWebRequestTime.text = sw.ElapsedMilliseconds.ToString();
                    testUnityWebRequestResponseSize.text = SizeString((long)request.downloadedBytes);
                }
            }
        }

        public void ReuqestCacheInfo()
        {
            testCacheStorageImage.texture = null;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            CacheInfo cacheInfo = GpmCacheStorage.RequestHttpCache(url.text, (result) =>
            {
                sw.Stop();

                if (result.IsSuccess() == true)
                {
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(result.Data);
                    texture.Apply();

                    testCacheStorageImage.texture = texture;


                    testCacheStorageTime.text = sw.ElapsedMilliseconds.ToString();
                    testCacheStorageResponseSize.text = SizeString(result.Info.contentLength);
                }
            });
        }

        public void ReuqestCacheInfo_Local()
        {
            testCacheStorage_LocalImage.texture = null;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            CacheInfo cacheInfo = GpmCacheStorage.RequestLocalCache(url.text, (result_local) =>
            {
                if (result_local.IsSuccess() == true)
                {
                    sw.Stop();
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(result_local.Data);
                    texture.Apply();

                    testCacheStorage_LocalImage.texture = texture;

                    testCacheStorage_LocalTime.text = sw.ElapsedMilliseconds.ToString();
                    testCacheStorageResponseSize.text = SizeString(result_local.Info.contentLength);
                }
                else
                {
                    cacheInfo = GpmCacheStorage.RequestHttpCache(url.text, (result) =>
                    {
                        sw.Stop();

                        if (result.IsSuccess() == true)
                        {
                            Texture2D texture = new Texture2D(1, 1);
                            texture.LoadImage(result.Data);
                            texture.Apply();

                            testCacheStorage_LocalImage.texture = texture;

                            testCacheStorage_LocalTime.text = sw.ElapsedMilliseconds.ToString();
                            testCacheStorageResponseSize.text = SizeString(result.Info.contentLength);
                        }
                    });
                }
            });
        }

        private string SizeString(long bytes)
        {
            double kb = bytes / 1024.0;
            double mb = kb / 1024.0;
            if (mb > 1)
            {
                return string.Format("{0}\n({1:0.00}) mb", bytes, mb);
            }
            else if (kb > 1)
            {
                return string.Format("{0}\n({1:0.00}) kb", bytes, kb);
            }
            else
            {
                return string.Format("{0} byte", bytes);
            }
        }

        public void RemoveData()
        {
            int removeIIndex = removeCacheIndex.value;
            GpmCacheStorage.Package.RemoveCacheData(GpmCacheStorage.Package.cacheStorage[removeIIndex]);

            removeCacheIndex.options.RemoveAt(removeIIndex);
            removeCacheIndex.RefreshShownValue();
        }
        public void ClearCache()
        {
            GpmCacheStorage.ClearCache();

            removeCacheIndex.ClearOptions();
            removeCacheIndex.RefreshShownValue();
        }

        public void OpenCacheFolder()
        {
            Application.OpenURL(GpmCacheStorage.GetCachePath());
        }

        public void SecuringStorage()
        {
            long maxSize = GpmCacheStorage.GetMaxSize();
            if (maxSize > 0)
            {
                GpmCacheStorage.Package.SecuringStorage(maxSize, 0);
            }
        }
    }
}