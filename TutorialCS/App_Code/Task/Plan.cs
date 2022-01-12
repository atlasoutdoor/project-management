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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Util.Ui;

namespace Util.Task
{
    public class Plan
    {
        private Dictionary<string, Resource> Resources = new Dictionary<string, Resource>();
        private string durationField;
        private string resourceField;
        private  string idField;
        private string fromField;
        private string toField;

        private Dictionary<string, Task> all = new Dictionary<string, Task>();

        public List<Task> Processed { get; private set; }

        public DateTime Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).AddMinutes(1);

        public void LoadTasks(ICollection data, string idField, string durationField, string resourceField)
        {
            this.idField = idField;
            this.durationField = durationField;
            this.resourceField = resourceField;

            foreach (object dataItem in data)
            {
                Task task = CreateTask(dataItem);
                Resource r = FindResource(task.ResourceId);
                r.AddTask(task);
                all.Add(task.Id, task);
            }
        }

        public void LoadLinks(ICollection data, string fromField, string toField)
        {
            this.fromField = fromField;
            this.toField = toField;

            foreach (object dataItem in data)
            {
                Link link = CreateLink(dataItem);
                if (all.ContainsKey(link.FromId))
                {
                    Task task = all[link.FromId];
                    link.From = task;
                    task.Outgoing.Add(link);
                }

                if (all.ContainsKey(link.ToId))
                {
                    Task task = all[link.ToId];
                    link.To = task;
                    task.Incoming.Add(link);
                }
            }
        }

        private Link CreateLink(object dataItem)
        {
            string from = Binder.Get(dataItem, fromField).String;
            string to = Binder.Get(dataItem, toField).String;

            return new Link() {FromId = from, ToId = to};
        }

        public void Process()
        {
            foreach (Resource r in Resources.Values)
            {
                r.Prepare();
            }

            while (AnyHasNext)
            {
                ResourceWithSmallestPoint.Next();
            }

            Processed = new List<Task>();
            foreach (var t in all.Values)
            {
                Processed.Add(t);
            }

            Processed.Sort(new TaskComparerStartEnd());
        }

        public bool AnyHasNext
        {
            get { return Resources.Values.Any(r => r.HasNext); }
        }

        private Task CreateTask(object dataItem)
        {
            int? duration = Binder.Get(dataItem, durationField).Int32;
            string resource = Binder.Get(dataItem, resourceField).String;
            string id = Binder.Get(dataItem, idField).String;

            if (duration == null)
            {
                duration = 15;
            }
            return new Task() { Duration = (int) duration, ResourceId = resource, Id = id, Source = dataItem };
        }

        private Resource FindResource(string r)
        {
            if (r == null)
            {
                r = "NULL";
            }
            if (!Resources.ContainsKey(r))
            {
                Resources[r] = new Resource { Id = r, Plan = this, Start = Start};
            }
            return Resources[r];
        }

        public Resource ResourceWithSmallestPoint
        {
            get
            {
                DateTime point = DateTime.MaxValue;
                Resource smallest = null;
                foreach(var r in Resources.Values)
                {
                    if (r.Point < point && r.HasNext)
                    {
                        point = r.Point;
                        smallest = r;
                    }
                }
                return smallest;
            }
        }

        public DateTime? VeryStart
        {
            get
            {
                if (Processed.Count > 0)
                {
                    return Processed[0].Start.Date;
                }
                return null;
            }
        }

        public DateTime? VeryEnd
        {
            get
            {
                if (Processed.Count > 0)
                {
                    return Processed[Processed.Count - 1].End;
                }
                return null;

            }
        }

        public int? Days
        {
            get
            {
                if (VeryStart == null || VeryEnd == null)
                {
                    return null;
                }
                return (int) (VeryEnd.Value.Date.AddDays(1) - VeryStart.Value).TotalDays;
            }
        }
    }
}