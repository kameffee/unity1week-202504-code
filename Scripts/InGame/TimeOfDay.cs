using System;
using System.Text;

namespace Unity1week202504.InGame
{
    [Flags]
    public enum TimeOfDay
    {
        None = 0,
        Morning = 1 << 0,
        Afternoon = 1 << 1,
        Night = 1 << 2,
    }

    public static class TimeOfDayExtensions
    {
        public static string ToLocalizedString(this TimeOfDay timeOfDay)
        {
            if (timeOfDay == TimeOfDay.None)
                return "無し";

            var stringBuilder = new StringBuilder();
            if (timeOfDay.HasFlag(TimeOfDay.Morning))
            {
                stringBuilder.Append("朝");
            }

            if (timeOfDay.HasFlag(TimeOfDay.Afternoon))
            {
                stringBuilder.Append("昼");
            }

            if (timeOfDay.HasFlag(TimeOfDay.Night))
            {
                stringBuilder.Append("夜");
            }

            return stringBuilder.ToString();
        }
    }
}