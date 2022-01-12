using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using DayPilot.Utils;
using DayPilot.Web.Ui.Enums;
using Util;

public partial class Project_New : ProjectPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DateTime start = Convert.ToDateTime(Request.QueryString["start"]);
            Helper.FillDurations(DropDownListDuration);
        }
    }

    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        
        //DateTime end = Convert.ToDateTime(TextBoxEnd.Text);
        int duration = Convert.ToInt32(DropDownListDuration.SelectedValue);
        string note = TextBoxNote.Text;

        new DataManager().CreateAssignment(ProjectId, duration, note);

        // passed to the modal dialog close handler, see Scripts/DayPilot/event_handling.js
        Hashtable ht = new Hashtable();
        ht["refresh"] = "yes";
        ht["message"] = "Event created.";

        Modal.Close(this, ht);
    }

    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Modal.Close(this);
    }
}
