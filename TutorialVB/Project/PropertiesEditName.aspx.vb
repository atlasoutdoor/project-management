Imports System
Imports System.Collections
Imports System.Data
Imports System.Web
Imports System.Web.UI
Imports Util
Imports Util.Ui

Partial Public Class Project_PropertiesEditName
	Inherits ProjectPage

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		Response.Cache.SetCacheability(HttpCacheability.NoCache)
		Response.Cache.SetNoStore()

		If Not IsPostBack Then
			TextBoxName.Text = Binder.Get(Project, "ProjectName").String

		End If
	End Sub


	Protected Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs)
		Dim name As String = TextBoxName.Text

		CType(New DataManager(), DataManager).UpdateProject(ProjectId, name)

		Dim ht As New Hashtable()
		ht("refresh") = "yes"
		ht("message") = "Event updated."

		Modal.Close(Me, ht)
	End Sub

	Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
		Modal.Close(Me)
	End Sub


End Class