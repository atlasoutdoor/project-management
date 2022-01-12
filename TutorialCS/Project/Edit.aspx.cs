using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using Util;
using Util.Ui;

public partial class Project_Edit : ProjectPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();

        if (!IsPostBack)
        {
            Helper.FillDurations(DropDownListDuration);
            Helper.FillDurationsWithNull(DropDownListSpent, "(automatic)");

            DataRow dr = new DataManager().GetAssignment(Convert.ToInt32(Request.QueryString["id"]));

            //DateTime start = (DateTime) dr["AssignmentStart"];
            //DateTime end = (DateTime) dr["AssignmentEnd"];

            DropDownListDuration.SelectedValue = Convert.ToString(dr["AssignmentDuration"]);
            DropDownListSpent.SelectedValue = Convert.ToString(dr["AssignmentDurationReal"]);

            TextBoxNote.Text = Convert.ToString(dr["AssignmentNote"]);
            RadioButtonListStatus.SelectedValue = Convert.ToString(dr["AssignmentStatus"]);

            DropDownListResource.DataSource = new DataManager().GetResourcesAllWithNull(ProjectId);
            DropDownListResource.DataTextField = "ResourceName";
            DropDownListResource.DataValueField = "ResourceId";
            DropDownListResource.DataBind();

            DropDownListResource.SelectedValue = Convert.ToString(dr["ResourceId"]);

            UpdateStartEnd(Binder.Get(dr, "AssignmentStart").DateTime, Binder.Get(dr, "AssignmentEnd").DateTime, Binder.Get(dr, "AssignmentDurationReal").Int32);

        }
    }

    private void UpdateStartEnd(DateTime? start, DateTime? end, int? spent)
    {
        //string status = Convert.ToString(dr["AssignmentStatus"]);
        //DateTime start = Convert.ToDateTime(dr["AssignmentStart"]);
        //DateTime end = Convert.ToDateTime(dr["AssignmentEnd"]);
        string took = null;
        if (spent != null)
        {
            took = TimeSpan.FromMinutes((double) spent).ToString();
        }

        switch (RadioButtonListStatus.SelectedValue)
        {
            case "planned":
                PanelStartEnd.Visible = false;

                HiddenStart.Value = null;
                HiddenEnd.Value = null;
                HiddenSpent.Value = "" + spent;
                break;
            case "started":
                PanelStartEnd.Visible = true;
                
                LabelStart.Text = start.ToString();
                HiddenStart.Value = start.ToIsoString();
                
                LabelEnd.Text = String.Empty;
                HiddenEnd.Value = String.Empty;
                
                LabelSpent.Text = took;
                HiddenSpent.Value = "" + spent ?? String.Empty;
                break;
            case "finished":
                PanelStartEnd.Visible = true;
                
                LabelStart.Text = start.ToString();
                HiddenStart.Value = start.ToIsoString();

                LabelEnd.Text = end.ToString();
                HiddenEnd.Value = end.ToIsoString();
                
                LabelSpent.Text = took;
                HiddenSpent.Value = spent.ToString();
                break;
        }
    }

    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        string note = TextBoxNote.Text;
        //string color = DropDownListColor.SelectedValue;
        int id = Convert.ToInt32(Request.QueryString["id"]);
        string resource = DropDownListResource.SelectedValue;
        int duration = Convert.ToInt32(DropDownListDuration.SelectedValue);
        //string spent = Convert.ToString(DropDownListSpent.SelectedValue);
        string status = RadioButtonListStatus.SelectedValue;

        DateTime? start = Binder.Get(HiddenStart.Value).DateTime;
        DateTime? end = Binder.Get(HiddenEnd.Value).DateTime;
        int? spent = Binder.Get(HiddenSpent.Value).Int32;

        new DataManager().UpdateAssignment(id, note, resource, duration, spent, start, end, status);
        
        if (status == "started")
        {
            new DataManager().UpdateAssignmentStartManual(id, start);
        }
        else if (status == "planned")
        {
            new DataManager().UpdateAssignmentStartManual(id, null);
        }

        Hashtable ht = new Hashtable();
        ht["refresh"] = "yes";
        ht["message"] = "Event updated.";

        Modal.Close(this, ht);
    }

    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Modal.Close(this);
    }

    protected void ButtonDelete_Click(object sender, EventArgs e)
    {
        new DataManager().DeleteAssignment(Convert.ToInt32(Request.QueryString["id"]));
        Hashtable ht = new Hashtable();
        ht["refresh"] = "yes";
        ht["message"] = "Event deleted.";
        Modal.Close(this, ht);
        //ScriptManager.RegisterStartupScript(this, this.GetType(), "modal", "<script type='text/javascript'>setTimeout(function() { modal.close({refresh:'yes',message:'Event deleted'); }, 0);</script>", false);
    }

    protected void UpdatePanel_Load(object sender, EventArgs e)
    {
        //ScriptManager.RegisterStartupScript(this, this.GetType(), "modal", "<script type='text/javascript'>setTimeout(function() { modal.stretch(); }, 0);</script>", false);
    }

    protected void RadioButtonListStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        CalculatePreview();
    }

    private void CalculatePreview()
    {

        DataRow dr = new DataManager().GetAssignment(Convert.ToInt32(Request.QueryString["id"]));

        DateTime? start = null;
        DateTime? end = null;

        //int? spent = Binder.Get(dr, "AssignmentDurationReal").Int32;
        //int? duration = Binder.Get(dr, "AssignmentDuration").Int32;
        int? spent = Binder.Get(DropDownListSpent.SelectedValue).Int32;
        int? duration = Binder.Get(DropDownListDuration.SelectedValue).Int32;

        switch (RadioButtonListStatus.SelectedValue)
        {
            case "planned":
                start = null;
                end = null;
                break;
            case "started":
                if (Binder.Get(dr, "AssignmentStartManual").IsNotNull)
                {
                    start = Binder.Get(dr, "AssignmentStartManual").DateTime;
                }
                else
                {
                    start = DateTime.Now;
                }
                end = null;
                break;
            case "finished":
                end = Binder.Get(dr, "AssignmentEnd").DateTime ?? DateTime.Now;

                start = Binder.Get(dr, "AssignmentStartManual").DateTime;

                if (start == null)  // closing directly
                {
                    spent = spent ?? duration;
                    start = end.Value.AddMinutes((double)(-spent));
                }
                else // started previously
                {
                    if (spent == null)  // automatic
                    {
                        spent = (int?)(end - start).Value.TotalMinutes;
                    }
                    else // override start
                    {
                        start = end.Value.AddMinutes((double) (-spent));
                    }
                }

                break;

        }
        UpdateStartEnd(start, end, spent);
    }

    protected void DropDownListDuration_SelectedIndexChanged(object sender, EventArgs e)
    {
        CalculatePreview();
    }

    protected void DropDownListSpent_SelectedIndexChanged(object sender, EventArgs e)
    {
        CalculatePreview();
    }
}