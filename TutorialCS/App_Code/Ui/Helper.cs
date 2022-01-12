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
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for Helper
/// </summary>
public class Helper
{

    public static void FillDurationsWithNull(DropDownList list)
    {
        FillDurationsWithNull(list, "(not set)");
    }

    public static void FillDurations(DropDownList DropDownListDuration)
    {
        DropDownListDuration.Items.Add(new ListItem("15 minutes", "15"));
        DropDownListDuration.Items.Add(new ListItem("30 minutes", "30"));
        DropDownListDuration.Items.Add(new ListItem("45 minutes", "45"));
        DropDownListDuration.Items.Add(new ListItem("1 hour", "60"));
        DropDownListDuration.Items.Add(new ListItem("1.5 hours", "90"));
        DropDownListDuration.Items.Add(new ListItem("2 hours", "120"));
        DropDownListDuration.Items.Add(new ListItem("3 hours", "180"));
        DropDownListDuration.Items.Add(new ListItem("4 hours", "240"));
        DropDownListDuration.Items.Add(new ListItem("5 hours", "300"));
        DropDownListDuration.Items.Add(new ListItem("6 hours", "360"));
        DropDownListDuration.Items.Add(new ListItem("7 hours", "420"));
        DropDownListDuration.Items.Add(new ListItem("8 hours", "480"));
    }

    public static string StatusToColor(object o)
    {
        string status = Convert.ToString(o);
        if (String.IsNullOrEmpty(status))
        {
            status = "planned";
        }
        switch (status)
        {
            case "planned":
                return "#004dc3";
            case "started":
                return "#008e00";
            case "finished":
                return "#eab71e";
        }
        throw new ArgumentException("Unrecognized status");
    }

    public static void FillDurationsWithNull(DropDownList list, string nullText)
    {
        list.Items.Add(new ListItem(nullText, String.Empty));
        FillDurations(list);

    }
}