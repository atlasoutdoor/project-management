using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using DayPilot.Utils;
using DayPilot.Web.Ui.Enums;
using Util;

public partial class NewDialog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }



    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        string name = TextBoxName.Text;
        new DataManager().CreateProject(name);

        Hashtable ht = new Hashtable();
        ht["refresh"] = "yes";
        Modal.Close(this, ht);
    }

    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Modal.Close(this);
    }
}
