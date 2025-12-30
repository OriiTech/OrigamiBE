using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Utils
{
    public static class TimeFormatHelper
    {
        public static string FormatTimeRemaining(DateTime endTime)
        {
            var now = DateTime.UtcNow;

            if (endTime <= now)
                return "Expired";

            var span = endTime - now;

            if (span.TotalDays >= 1)
                return $"{(int)span.TotalDays} days {span.Hours} hours";

            if (span.TotalHours >= 1)
                return $"{(int)span.TotalHours} hours {span.Minutes} minutes";

            return $"{span.Minutes} minutes";
        }
    }

}
