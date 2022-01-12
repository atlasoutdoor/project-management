using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Util.Ui;

public partial class MasterProject : MasterPage
{
    //public string ProjectId;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetNoStore();
    }

    public DataRow Project
    {
        get
        {
            var info = Request.PathInfo;
            //ProjectId = Request.PathInfo;
            var id = info.Substring(1);

            return new DataManager().GetProject(id);
        }
    }

    public int ProjectId
    {
        get
        {
            return Binder.Get(Project, "ProjectId").Int32.Value;
        }

    }

    public string ProjectName
    {
        get { return Binder.Get(Project, "ProjectName").String; }
    }

    public DataRow ActiveUser
    {
        get { return new DataManager().GetUserById(Page.User.Identity.Name); }
    }

}
