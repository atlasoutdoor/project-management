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
using System.Web;
using Util.Task;
using Util.Ui;

/// <summary>
/// Summary description for TaskComparer
/// </summary>
public class TaskComparerOrdinal : IComparer<Task>
{
	public TaskComparerOrdinal()
	{
		
	}

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <returns>
    /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
    /// </returns>
    /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
    public int Compare(Task x, Task y)
    {
        int xo = Binder.Get(x, "AssignmentOrdinal").Int32.GetValueOrDefault(0);
        int yo = Binder.Get(y, "AssignmentOrdinal").Int32.GetValueOrDefault(0);
        
        return xo - yo; 
    }
}