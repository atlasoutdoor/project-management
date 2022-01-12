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
using System.Collections.Generic;
using System.Linq;

namespace Util.Task
{
    public class Resource
    {
        public Plan Plan { get; set; }

        public string Id { get; set; }
        public DateTime Start { get; set; }

        public List<Task> Tasks { get; private set; }

        public List<Task> Done { get; private set; }
        public List<Task> Ready { get; private set; }
        public List<Task> Blocked { get; private set; }

        public DateTime Point { get; private set; }

        public Resource()
        {
            Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).AddMinutes(1);
            Tasks = new List<Task>();

            Blocked = new List<Task>();
            Ready = new List<Task>();
            Done = new List<Task>();
        }

        public void Prepare()
        {
            //Tasks.Sort(new TaskComparerLinkCount());
            Tasks.Sort(new TaskComparerOrdinal());

            foreach (var task in Tasks)
            {
                if (task.IncomingAllFulfilled)
                {
                    Ready.Add(task);
                }
            }
            Point = Start;
        }


        private void ScheduleTask(Task task)
        {
            int minutesToBeAdded = task.Duration;
            DateTime test = task.Start;
            bool startFixed = false;

            while (minutesToBeAdded > 0)
            {
                if (IsBusinessMinute(test))
                {
                    minutesToBeAdded -= 1;
                    if (!startFixed)
                    {
                        task.Start = test;
                    }
                    startFixed = true;
                }

                test = test.AddMinutes(1);
            }

            task.End = test;
        }


        private bool IsBusinessMinute(DateTime start)
        {
            const int businessStartHour = 9;
            const int businessEndHour = 17;

            var freeDays = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };

            if (freeDays.Contains(start.DayOfWeek))
            {
                return false;
            }

            if (start.Hour < businessStartHour)
            {
                return false;
            }

            if (start.Hour >= businessEndHour)
            {
                return false;
            }
            return true;
        }


        public void Next()
        {
            if (Ready.Count == 0)
            {
                return;
            }

            var task = Ready[0];

            task.Start = Point;

            ScheduleTask(task);

            Point = task.End;

            foreach(Link link in task.Outgoing)
            {
                link.Fulfilled = true;

                if (link.To.IncomingAllFulfilled)
                {
                    
                    link.To.Resource.Blocked.Remove(link.To);
                    link.To.Resource.Ready.Add(link.To);
                }
            }

            Done.Add(task);
            Ready.Remove(task);
        }

        public bool HasNext
        {
            get { return Ready.Count > 0; }
        }


        public void AddTask(Task task)
        {
            Tasks.Add(task);
            task.Resource = this;
        }
    }
}