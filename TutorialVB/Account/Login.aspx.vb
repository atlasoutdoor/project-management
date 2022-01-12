Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls

Partial Public Class Account_Login
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" & HttpUtility.UrlEncode(Request.QueryString("ReturnUrl"))
	End Sub

	Protected Sub LoginUser_LoggedIn(ByVal sender As Object, ByVal e As EventArgs)
		Dim dr As DataRow = (New DataManager()).GetUserByEmail(LoginUser.UserName)
'INSTANT VB NOTE: The variable id was renamed since Visual Basic does not handle local variables named the same as class members well:
		Dim id_Renamed As Integer = CInt(Fix(dr("UserId")))

		FormsAuthentication.SetAuthCookie(id_Renamed.ToString(), LoginUser.RememberMeSet)

	End Sub
End Class
