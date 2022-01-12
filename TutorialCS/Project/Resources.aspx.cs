using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DayPilot.Utils;
using DayPilot.Web.Ui.Events.Scheduler;
using Util.Task;
using Util.Ui;
using Resource = DayPilot.Web.Ui.Resource;

public partial class Project_Resources : ProjectPage
{
    private Plan _plan = null;
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            DayPilotScheduler1.StartDate = Plan.VeryStart ?? DateTime.Today;
            DayPilotScheduler1.Days = Plan.Days ?? 1;
            LoadEvents();
            UpdateZoomLevel();
            CreateResources();
        }
    }

    private void LoadEvents()
    {
        DayPilotScheduler1.DataSource = Data;
        DataBind();
    }


    private void CreateResources()
    {
        DayPilotScheduler1.Resources.Clear();

        AddDefaultGroup();

        foreach (DataRow dr in Groups.Rows)
        {
            string name = (string) dr["GroupName"];
            int id = Convert.ToInt32(dr["GroupId"]);

            string html = table(name, 0);
            DayPilotScheduler1.Resources.Add(new Resource(html, "GROUP"));

            AddChildren(id);
        }
    }

    private void AddDefaultGroup()
    {
        string name = "(default)";

        string html = table(name, 0);
        DayPilotScheduler1.Resources.Add(new Resource(html, "DEFAULT"));

        AddChildren(null);
    }

    private void AddChildren(int? parent)
    {
        foreach (DataRow dr in Resources(parent).Rows)
        {
            string name = (string)dr["ResourceName"];
            int id = Convert.ToInt32(dr["ResourceId"]);

            string html = table(name, 1);
            DayPilotScheduler1.Resources.Add(new Resource(html, id.ToString()));
        }
    }

    private DataTable Groups
    {
        get
        {
            return new DataManager().GetGroups(ProjectId);
        }
    }

    private DataTable Resources(int? group)
    {
        return new DataManager().GetResources(ProjectId, group);
    }

    private List<Task> Data
    {
        get
        {
            List<Task> all = new List<Task>();
            if (!CheckBoxHideFinished.Checked)
            {
                all.AddRange(Finished);
            }
            all.AddRange(Started);
            all.AddRange(Plan.Processed);
            return all;
        }
    }

    private Plan Plan
    {
        get
        {
            if (_plan == null)
            {
                DataTable table = new DataManager().GetAssignmentsPlanned(ProjectId);

                _plan = new Plan();
                _plan.LoadTasks(table.Rows, "AssignmentId", "AssignmentDuration", "ResourceId");
                _plan.Process();
            }
            return _plan;
        }
    }

    private List<Task> Finished
    {
        get
        {
            List<Task> tasks = new List<Task>();
            DataTable table = new DataManager().GetAssignmentsFinished(ProjectId);
            foreach (var row in table.Rows)
            {

                var task = new Task()
                {
                    Start = (DateTime)Binder.Get(row, "AssignmentStart").DateTime,
                    End = (DateTime)Binder.Get(row, "AssignmentEnd").DateTime,
                    Id = Binder.Get(row, "AssignmentId").String,
                    ResourceId = Binder.Get(row, "ResourceId").String,
                    Source = row
                };
                tasks.Add(task);
            }
            return tasks;
        }
    }

    private List<Task> Started
    {
        get
        {
            List<Task> tasks = new List<Task>();
            DataTable table = new DataManager().GetAssignmentsStarted(ProjectId);
            foreach (DataRow row in table.Rows)
            {

                //throw new Exception("task: " + row["AssignmentNote"]);

                var task = new Task()
                {
                    Start = (DateTime)Binder.Get(row, "AssignmentStart").DateTime,
                    End = DateTime.Now,
                    Id = Binder.Get(row, "AssignmentId").String,
                    ResourceId = Binder.Get(row, "ResourceId").String,
                    Source = row
                };

                tasks.Add(task);
            }
            return tasks;
        }
    }


    private string table(string name, int indent)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<div>");

        for (int i = 0; i < indent; i++)
        {
            sb.Append("<div style='display:inline-block; width: 20px; height: 1px;'>");
            sb.Append("</div>");
        }

        sb.Append("<div style='display:inline-block;'>");
        sb.Append(name);
        sb.Append("</div>");


        sb.Append("</div>");

        return sb.ToString();
    }

    protected void UpdatePanelScheduler_Load(object sender, EventArgs e)
    {
    }

    protected void DayPilotScheduler1_BeforeEventRender(object sender, BeforeEventRenderEventArgs e)
    {
        Task t = (Task)e.DataItem.Source;
        e.DurationBarColor = Helper.StatusToColor(t["AssignmentStatus"]);
    }

    protected void ButtonRefresh_Click(object sender, EventArgs e)
    {
        LoadEvents();
        CreateResources();
    }


    private void UpdateZoomLevel()
    {
        string level = RadioButtonListZoom.SelectedValue;
        switch (level)
        {
            case "Hours":
                DayPilotScheduler1.CellDuration = 60;
                break;
            case "Days":
                DayPilotScheduler1.CellDuration = 1440;
                break;
        }
    }

    protected void RadioButtonListZoom_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateZoomLevel();
        LoadEvents();
    }

    protected void CheckBoxHideFinished_CheckedChanged(object sender, EventArgs e)
    {
        LoadEvents();
    }

}
