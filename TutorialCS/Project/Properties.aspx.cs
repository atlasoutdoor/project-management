using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DayPilot.Web.Ui;
using DayPilot.Web.Ui.Events.Scheduler;
using Util.Task;
using Util.Ui;
using Resource = DayPilot.Web.Ui.Resource;

public partial class Project_Properties : ProjectPage
{
    private Plan _plan = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadProjectData();
        }

    }


    protected void ButtonRefresh_Click(object sender, EventArgs e)
    {
        LoadProjectData();
    }

    private void LoadProjectData()
    {
        LabelName.Text = Binder.Get(Project, "ProjectName").String;
        DataBind();
    }
}