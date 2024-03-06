namespace Gpm.Ui
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using Gpm.CacheStorage;

    [RequireComponent(typeof(RawImage))]
    public class WebCacheImage : MonoBehaviour
    {
        [SerializeField]
        private string url;

        [SerializeField]
        private bool preLoad = true;

        [SerializeField]
        private LoadTextureEvent onLoadTexture = new LoadTextureEvent();

        private RawImage image;

        private CacheInfo cacheInfo;

        private bool isInitilize = false;

        public RawImage Image
        {
            get
            {
                if (image == null)
                {
                    image = GetComponent<RawImage>();
                }

                return image;
            }

            private set
            {
                image = value;
            }
        }

        public CacheInfo CacheInfo
        {
            get
            {
                return cacheInfo;
            }
        }

        private void Awake()
        {
            if (image == null)
            {
                image = GetComponent<RawImage>();
            }
        }
        private void OnEnable()
        {
            if(isInitilize == false)
            {
                if(preLoad == true)
                {
                    Preload();
                }

                isInitilize = true;
            }
        }

        public void Preload()
        {
            if (image != null)
            {
                cacheInfo = GpmCacheStorage.GetCachedTexture(url, (cachedTexture) =>
                {
                    if (cachedTexture != null)
                    {
                        Image.texture = cachedTexture.texture;
                    }
                });
            }
        }

        public void LoadImage()
        {
            if (image != null)
            {
                Image.texture = null;

                if (preLoad == true)
                {
                    Preload();
                }

                if (string.IsNullOrEmpty(this.url) == false)
                {
                    cacheInfo = GpmCacheStorage.RequestTexture(url, (cachedTexture) =>
                    {
                        if (cachedTexture != null)
                        {
                            Image.texture = cachedTexture.texture;
                        }
                    });
                }
            }
        }

        public void SetUrl(string url)
        {
            if(this.url != url)
            {
                this.url = url;
                LoadImage();
            }
        }

        public void SetLoadTextureEvent(UnityAction<Texture> onListener)
        {
            CleatLoadTextureEvent();
            AddLoadTextureEvent(onListener);
        }

        public void AddLoadTextureEvent(UnityAction<Texture> onListener)
        {
            onLoadTexture.AddListener(onListener);
        }

        public void CleatLoadTextureEvent()
        {
            onLoadTexture = new LoadTextureEvent();
        }

        [Serializable]
        public class LoadTextureEvent : UnityEvent<Texture>
        {
            public LoadTextureEvent()
            {
            }
        }
    }
}