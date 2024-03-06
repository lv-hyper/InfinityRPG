using System.Collections.Generic;
using System.Xml.Serialization;
using System;

namespace Gpm.Manager.Constant
{

    [XmlRoot("noticeInfo")]
    public class NoticeInfo
    {
        private const int KOREA_STANDARD_TIME = 9;

        public class Day
        {
            [XmlElement("start")]
            public string start;
            [XmlElement("end")]
            public string end;
        }

        public class TimeInfo
        {
            [XmlElement("startTime")]
            public string startTime;
            [XmlElement("endTime")]
            public string endTime;
            [XmlElement("day")]
            public Day day;
        }

        [XmlRoot("noticeList")]
        public class NoticeList
        {
            [XmlRoot("notice")]
            public class Notice
            {
                [XmlElement("text")]
                public string text;
                [XmlElement("url")]
                public string url;
                [XmlElement("timeInfo")]
                public TimeInfo timeInfo;

                public bool IsActiveTime()
                {
                    DateTime dateTimeStart = DateTime.ParseExact(timeInfo.startTime, "yyyy-MM-dd HH:mm", null).AddHours(-KOREA_STANDARD_TIME);
                    DateTime dateTimeEnd = DateTime.ParseExact(timeInfo.endTime, "yyyy-MM-dd HH:mm", null).AddHours(-KOREA_STANDARD_TIME);

                    int utcNowHour = (DateTime.UtcNow.Hour + KOREA_STANDARD_TIME) % 24;

                    if (DateTime.UtcNow.Ticks >= dateTimeStart.Ticks
                        && DateTime.UtcNow.Ticks <= dateTimeEnd.Ticks)
                    {
                        if (utcNowHour >= int.Parse(timeInfo.day.start)
                            && utcNowHour <= int.Parse(timeInfo.day.end))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            [XmlElement("notice")]
            public List<Notice> list;
        }

        public NoticeList noticeList;
    }
}