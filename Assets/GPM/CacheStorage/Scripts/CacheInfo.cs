using UnityEngine;
using System;

namespace Gpm.CacheStorage
{
    [Serializable]
    public class CacheInfo : IComparable<CacheInfo>
    {
        private const long SECONDSPERDAY = 86400;
        public enum State
        {
            NONE = 0,
            REQUEST,
            CACHED,
            EXPIRED,
        }

        [NonSerialized]
        internal CachePackage storage;

        [SerializeField]
        public string url;

        [SerializeField]
        public string eTag;

        [SerializeField]
        public StringToValue<DateTime> lastModified;

        [SerializeField]
        public long lastAccess;

        [SerializeField]
        public StringToValue<DateTime> expires;

        [SerializeField]
        public StringToValue<DateTime> requestTime;

        [SerializeField]
        public StringToValue<DateTime> responseTime;

        [SerializeField]
        public StringToValue<int> age;

        [SerializeField]
        public StringToValue<DateTime> date;

        [SerializeField]
        public CacheControl cacheControl;

        [SerializeField]
        public StringToValue<int> contentLength;

        [SerializeField]
        private int initialAge = 0;

        [SerializeField]
        private int freshnessLifetime = 0;

        [SerializeField]
        internal State state;

        [SerializeField]
        internal int index;

        internal bool requestInPlay = false;

        private long lastCheckTime = 0;

        private bool lastCheckAccessWeek = false;
        private bool lastCheckAccessMonth = false;

        public CacheInfo()
        {

        }

        public CacheInfo(CachePackage storage, string url)
        {
            this.storage = storage;
            this.url = url;
        }

        public CacheInfo(int index)
        {
            this.index = index;
        }

        public int CompareTo(CacheInfo other)
        {
            bool lastAccessMonth = IsLastAccessMonth();
            if (lastAccessMonth != other.IsLastAccessMonth())
            {
                if (lastAccessMonth == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            bool lastAccessWeek = IsLastAccessWeek();
            if (lastAccessWeek != other.IsLastAccessWeek())
            {
                if (lastAccessWeek == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            bool expired = IsExpired();
            if (expired != other.IsExpired())
            {
                if (expired == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            return -other.index.CompareTo(index);
        }

        public bool IsCached()
        {
            return index > 0;
        }

        private int FromToSeconds(DateTime from, DateTime to)
        {
            return (int)(to - from).TotalSeconds;
        }

        private int GetCurrentAge()
        {
            int residentTime = FromToSeconds(date, DateTime.UtcNow);
            return initialAge + residentTime;
        }

        public bool NeedRequest(double reRequestTime = 0)
        {
            if (cacheControl != null)
            {
                if (cacheControl.noCache == true)
                {
                    return true;
                }
                else if (cacheControl.maxAge.IsValid() == true)
                {
                    if (cacheControl.maxAge == 0)
                    {
                        return true;
                    }
                }
            }

            if (reRequestTime == 0)
            {
                reRequestTime = GpmCacheStorage.GetReRequestTime();
            }
            if (GpmCacheStorage.CheckReRequest(reRequestTime, responseTime) == true)
            {
                return true;
            }

            return IsExpired();
        }

        public void CheckState()
        {
            if (lastCheckTime != GpmCacheStorage.updateTime)
            {
                lastCheckTime = GpmCacheStorage.updateTime;

                lastCheckAccessWeek = IsLastAccessPeriod(SECONDSPERDAY * 7);
                lastCheckAccessMonth = IsLastAccessPeriod(SECONDSPERDAY * 30);

                if (state != State.EXPIRED &&
                    IsFresh() == false)
                {
                    state = State.EXPIRED;
                }
            }
        }

        public bool IsExpired()
        {
            CheckState();

            return state == State.EXPIRED;
        }

        public bool IsLastAccessWeek()
        {
            CheckState();

            return lastCheckAccessWeek;
        }

        public bool IsLastAccessMonth()
        {
            CheckState();

            return lastCheckAccessMonth;
        }

        public bool IsLastAccessPeriod(double periodSecond)
        {
            return DateTime.UtcNow.Ticks - lastAccess > TimeSpan.TicksPerSecond * periodSecond;
        }

        public bool IsFresh()
        {
            return freshnessLifetime > GetCurrentAge();
        }

        public void CaculateCacheInfo()
        {
            CaculateInitialAge();
            CaculateFreshnessLifeTime();
        }

        public int CaculateInitialAge()
        {
            int apparentAge = Math.Max(0, FromToSeconds(date, responseTime));

            int responseDelay = FromToSeconds(requestTime, responseTime);
            int correctedAgeValue = age + responseDelay;

            initialAge = Math.Max(apparentAge, correctedAgeValue);
            return initialAge;
        }

        public int CaculateFreshnessLifeTime()
        {
            freshnessLifetime = 0;

            if (cacheControl != null &&
                cacheControl.maxAge.IsValid() == true)
            {
                freshnessLifetime = cacheControl.maxAge;
            }
            else if (expires != null)
            {
                if (date != null)
                {
                    freshnessLifetime = FromToSeconds(date, expires);
                }
                else
                {
                    freshnessLifetime = FromToSeconds(responseTime, expires);
                }
            }
            return freshnessLifetime;
        }
    }
}