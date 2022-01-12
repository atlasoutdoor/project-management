/*
Copyright © 2013 Annpoint, s.r.o.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

-------------------------------------------------------------------------

NOTE: Reuse requires the following acknowledgement (see also NOTICE):
This product includes DayPilot (http://www.daypilot.org) developed by Annpoint, s.r.o.
*/

using System;

namespace Util.Ui
{
    public static class TimeSpanFormattingExtension
    {
        public static string ToHourMinuteString(this TimeSpan span)
        {
            if (span == TimeSpan.Zero)
            {
                return String.Empty;
            }
            return string.Format("{0}:{1:00}", Math.Floor(span.TotalHours), span.Minutes);
        }

        public static string ToIsoString(this DateTime? date)
        {
            if (date == null)
            {
                return String.Empty;
            }
            return date.Value.ToString("s");
        }
    }
}