Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls

Partial Public Class Account_Logout
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		FormsAuthentication.SignOut()
		HttpContext.Current.Session.Abandon()
		Response.Redirect("~/")
	End Sub
End Class