using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DayPilot.Web.Ui;
using DayPilot.Web.Ui.Data;
using DayPilot.Web.Ui.Events;
using DayPilot.Web.Ui.Events.Scheduler;
using Util.Task;
using Util.Ui;

public partial class Project_Default : ProjectPage
{
    private Plan _plan = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DayPilotScheduler1.StartDate = Plan.VeryStart ?? DateTime.Today;
            DayPilotScheduler1.Days = Plan.Days ?? 1;
            UpdateZoomLevel();
            LoadEvents();
        }

        string cols = new DataManager().GetUserConfig(User.Identity.Name, "project.cols");
        if (cols != null)
        {
            DayPilotScheduler1.RowHeaderColumnWidths = cols;
        }

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


    private string TaskLink(string name, int id, string status)
    {
        if (status == String.Empty)
        {
            status = null;
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class='task_status ");
        sb.Append(status ?? "planned");
        sb.Append("' data-taskid='" + id + "'></div>");

        sb.Append("<a title='");
        sb.Append(name);
        sb.Append("' ");
        sb.Append("href='javascript:edit(\"");
        sb.Append(id);
        sb.Append("\")'>");
        sb.Append(name);
        sb.Append("</a>");

        return sb.ToString();
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

                var task = new Task()
                {
                    Start = (DateTime)Binder.Get(row, "AssignmentStart").DateTime,
                    End = DateTime.Now,
                    Id = Binder.Get(row, "AssignmentId").String,
                    Source = row
                };
                tasks.Add(task);
            }

            return tasks;
        }
    }


    protected void UpdatePanelScheduler_Load(object sender, EventArgs e)
    {
    }

    protected void DayPilotScheduler1_BeforeEventRender(object sender, BeforeEventRenderEventArgs e)
    {
        Task t = (Task)e.DataItem.Source;
        e.DurationBarColor = Helper.StatusToColor(t["AssignmentStatus"]);
    }


    protected void DayPilotScheduler1_HeaderColumnWidthChanged(object sender, HeaderColumnWidthChangedEventArgs e)
    {
        new DataManager().SetUserConfig(User.Identity.Name, "project.cols", DayPilotScheduler1.RowHeaderColumnWidths);
        LoadEvents();
    }

    protected void RadioButtonListZoom_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateZoomLevel();

        LoadEvents();
    }


    protected void ButtonReorder_Click(object sender, EventArgs e)
    {
        string order = HiddenOrder.Value;
        new DataManager().UpdateOrder(ProjectId, order);

        LoadEvents();
    }

    private void LoadEvents()
    {
        DayPilotScheduler1.DataSource = Data;
        LabelSummary.Text = String.Format("Estimated finish time: {0}", Plan.VeryEnd);
        DataBind();
    }

    protected void ButtonRefresh_Click(object sender, EventArgs e)
    {
        LoadEvents();
    }

    protected void CheckBoxHideFinished_CheckedChanged(object sender, EventArgs e)
    {
        LoadEvents();
    }

    protected void DayPilotScheduler1_BeforeResHeaderRender(object sender, BeforeHeaderRenderEventArgs e)
    {
        DataItemWrapper task = e.DataItem;

        string name = (string)task["AssignmentNote"];
        int id = Convert.ToInt32(task["AssignmentId"]);
        string resource = Convert.ToString(task["ResourceName"]);
        string status = Convert.ToString(task["AssignmentStatus"]);

        TimeSpan duration = TimeSpan.FromMinutes(Convert.ToInt32(task["AssignmentDuration"]));

        string durationString = duration.ToHourMinuteString();
        string spentString = Binder.Get(task, "AssignmentDurationReal").IsNotNull
                                 ? Binder.Get(task, "AssignmentDurationReal").TimeSpanFromMinutes.ToHourMinuteString() : String.Empty;

        e.InnerHTML = TaskLink(name, id, status);
        e.Columns[0].InnerHTML = "<div style='text-align:right'>" + durationString + "</div>";
        e.Columns[1].InnerHTML = "<div style='text-align:right'>" + spentString + "</div>";
        e.Columns[2].InnerHTML = resource;

    }
}