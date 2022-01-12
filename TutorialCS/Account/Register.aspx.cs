using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Account_Register : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterUser.ContinueDestinationPageUrl = Request.QueryString["ReturnUrl"];
        RegisterUser.CreatingUser += new LoginCancelEventHandler(RegisterUser_CreatingUser);
    }

    protected void RegisterUser_CreatedUser(object sender, EventArgs e)
    {
        DataRow dr = new DataManager().GetUserByEmail(RegisterUser.Email);
        string id = (string) dr["UserId"];

        FormsAuthentication.SetAuthCookie(id, true);

        string continueUrl = RegisterUser.ContinueDestinationPageUrl;
        if (String.IsNullOrEmpty(continueUrl))
        {
            continueUrl = "~/";
        }
        Response.Redirect(continueUrl);
    }

    void RegisterUser_CreatingUser(object sender, LoginCancelEventArgs e)
    {
        RegisterUser.UserName = Guid.NewGuid().ToString();
    }

}
