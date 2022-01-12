using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DayPilot.Utils;
using DayPilot.Web.Ui;
using DayPilot.Web.Ui.Events.Scheduler;
using Util.Task;
using Util.Ui;
using Resource = DayPilot.Web.Ui.Resource;

public partial class Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadProjects();
        }
    }

    private void LoadProjects()
    {
        RepeaterProjects.DataSource = new DataManager().GetProjects();
        DataBind();
    }

    protected void ButtonRefresh_Click(object sender, EventArgs e)
    {
        LoadProjects();
    }

    protected void UpdatePanelProjects_Load(object sender, EventArgs e)
    {
        
    }
}
