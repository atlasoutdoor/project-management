Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls

Partial Public Class Account_Register
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		RegisterUser.ContinueDestinationPageUrl = Request.QueryString("ReturnUrl")
		AddHandler RegisterUser.CreatingUser, AddressOf RegisterUser_CreatingUser
	End Sub

	Protected Sub RegisterUser_CreatedUser(ByVal sender As Object, ByVal e As EventArgs)
		Dim dr As DataRow = (New DataManager()).GetUserByEmail(RegisterUser.Email)
'INSTANT VB NOTE: The variable id was renamed since Visual Basic does not handle local variables named the same as class members well:
		Dim id_Renamed As String = CStr(dr("UserId"))

		FormsAuthentication.SetAuthCookie(id_Renamed, True)

		Dim continueUrl As String = RegisterUser.ContinueDestinationPageUrl
		If String.IsNullOrEmpty(continueUrl) Then
			continueUrl = "~/"
		End If
		Response.Redirect(continueUrl)
	End Sub

	Private Sub RegisterUser_CreatingUser(ByVal sender As Object, ByVal e As LoginCancelEventArgs)
		RegisterUser.UserName = Guid.NewGuid().ToString()
	End Sub

End Class
