using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using DayPilot.Utils;
using DayPilot.Web.Ui.Enums;
using Util;

public partial class Project_NewGroup : ProjectPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }

    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        
        //DateTime end = Convert.ToDateTime(TextBoxEnd.Text);
        string name = TextBoxName.Text;

        new DataManager().CreateGroup(ProjectId, name);

        // passed to the modal dialog close handler, see Scripts/DayPilot/event_handling.js
        Hashtable ht = new Hashtable();
        ht["refresh"] = "yes";
        ht["message"] = "Resource created.";

        Modal.Close(this, ht);
    }

    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Modal.Close(this);
    }
}
