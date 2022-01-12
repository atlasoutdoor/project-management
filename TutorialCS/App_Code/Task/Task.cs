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
using System.Data;
using System.Linq;
using System.Web.UI;
using Util.Ui;

namespace Util.Task
{
    public class Task
    {
        public object Source { get; set; }
        public int Duration { get; set; }
        public string ResourceId { get; set; }
        public string Id { get; set; }
        public List<Link> Incoming { get; private set; }
        public List<Link> Outgoing { get; private set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Resource Resource { get; set; }

        public string Text { get { return Binder.Get(Source, "AssignmentNote").String; } }
        //public string Color { get { return Binder.Get(Source, "AssignmentColor").String; } }

        public bool IncomingAllFulfilled
        {
            get { return Incoming.All(link => link.Fulfilled); }
        }

        public Task()
        {
            Incoming = new List<Link>();
            Outgoing = new List<Link>();
        }

        /// <summary>
        /// Gets a property value of the original DataItem object.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object this[string property]
        {
            get
            {
                if (Source is DataRow)
                {
                    DataRow dr = (DataRow)Source;
                    return dr[property];
                }
                return DataBinder.GetPropertyValue(Source, property, null);
            }
        }
    }
}