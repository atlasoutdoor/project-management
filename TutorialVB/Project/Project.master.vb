Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Util.Ui

Partial Public Class MasterProject
	Inherits MasterPage

	'public string ProjectId;

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		Response.Cache.SetNoStore()
	End Sub

	Public ReadOnly Property Project() As DataRow
		Get
			Dim info = Request.PathInfo
			'ProjectId = Request.PathInfo;
'INSTANT VB NOTE: The variable id was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim id_Renamed = info.Substring(1)

			Return (New DataManager()).GetProject(id_Renamed)
		End Get
	End Property

	Public ReadOnly Property ProjectId() As Integer
		Get
			Return Binder.Get(Project, "ProjectId").Int32.Value
		End Get

	End Property

	Public ReadOnly Property ProjectName() As String
		Get
			Return Binder.Get(Project, "ProjectName").String
		End Get
	End Property

	Public ReadOnly Property ActiveUser() As DataRow
		Get
			Return (New DataManager()).GetUserById(Page.User.Identity.Name)
		End Get
	End Property

End Class
