using System.Collections.Generic;
using UnityEngine;

namespace Gpm.CacheStorage
{
    public class CachedTexture
    {
        public CacheInfo info;
        public bool requested;
        public Texture2D texture;
    }

    public static class CachedTextureManager
    {
        private static Dictionary<CacheInfo, CachedTexture> cachedTextureList = new Dictionary<CacheInfo, CachedTexture>();

        public static CachedTexture Get(CacheInfo info)
        {
            CachedTexture cachedTexture = null;
            if (cachedTextureList.TryGetValue(info, out cachedTexture) == true)
            {
            }

            return cachedTexture;
        }

        public static CachedTexture Cache(CacheInfo info, bool requested, byte[] data)
        {
            CachedTexture cachedTexture = null;

            if (cachedTextureList.TryGetValue(info, out cachedTexture) == false)
            {
                cachedTexture = new CachedTexture();

                cachedTexture.info = info;
                cachedTexture.requested = requested;
                cachedTexture.texture = new Texture2D(1, 1);

                cachedTextureList.Add(info, cachedTexture);
            }

            cachedTexture.texture.LoadImage(data);
            cachedTexture.texture.Apply();

            return cachedTexture;
        }

        public static void Release(CacheInfo info)
        {
            cachedTextureList.Remove(info);
        }
        public static void Clear()
        {
            cachedTextureList.Clear();
        }
    }
}
