using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Account_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
    }

    protected void LoginUser_LoggedIn(object sender, EventArgs e)
    {
        DataRow dr = new DataManager().GetUserByEmail(LoginUser.UserName);
        int id = (int)dr["UserId"];

        FormsAuthentication.SetAuthCookie(id.ToString(), LoginUser.RememberMeSet);

    }
}
