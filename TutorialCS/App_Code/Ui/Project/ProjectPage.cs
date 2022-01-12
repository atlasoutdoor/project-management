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
using System.Web;
using System.Web.UI;
using Util.Ui;

/// <summary>
/// Summary description for ProjectPage
/// </summary>
public class ProjectPage : Page
{
    public DataRow ActiveUser { get; private set; }

	public ProjectPage()
	{
		this.Load += new EventHandler(ProjectPage_Load);
	}

    private void ProjectPage_Load(object sender, EventArgs e)
    {
        ActiveUser = new DataManager().GetUserById(User.Identity.Name);
    }

    public DataRow Project
    {
        get
        {
            var info = Request.PathInfo;
            var id = info.Substring(1);

            return new DataManager().GetProject(id);
        }
    }

    public int ProjectId
    {
        get
        {
            return (int)Binder.Get(Project, "ProjectId").Int32;
        }

    }

}