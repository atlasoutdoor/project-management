using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using Util;
using Util.Ui;

public partial class Project_PropertiesEditName : ProjectPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();

        if (!IsPostBack)
        {
            TextBoxName.Text = Binder.Get(Project, "ProjectName").String;

        }
    }


    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        string name = TextBoxName.Text;

        new DataManager().UpdateProject(ProjectId, name);
        
        Hashtable ht = new Hashtable();
        ht["refresh"] = "yes";
        ht["message"] = "Event updated.";

        Modal.Close(this, ht);
    }

    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Modal.Close(this);
    }


}