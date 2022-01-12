Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports DayPilot.Utils
Imports DayPilot.Web.Ui
Imports DayPilot.Web.Ui.Events.Scheduler
Imports Util.Task
Imports Util.Ui
Imports Resource = DayPilot.Web.Ui.Resource

Partial Public Class [Default]
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then
			LoadProjects()
		End If
	End Sub

	Private Sub LoadProjects()
		RepeaterProjects.DataSource = (New DataManager()).GetProjects()
		DataBind()
	End Sub

	Protected Sub ButtonRefresh_Click(ByVal sender As Object, ByVal e As EventArgs)
		LoadProjects()
	End Sub

	Protected Sub UpdatePanelProjects_Load(ByVal sender As Object, ByVal e As EventArgs)

	End Sub
End Class
