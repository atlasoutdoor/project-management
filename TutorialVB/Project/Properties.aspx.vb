Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports DayPilot.Web.Ui
Imports DayPilot.Web.Ui.Events.Scheduler
Imports Util.Task
Imports Util.Ui
Imports Resource = DayPilot.Web.Ui.Resource

Partial Public Class Project_Properties
	Inherits ProjectPage

	Private _plan As Plan = Nothing

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then
			LoadProjectData()
		End If

	End Sub


	Protected Sub ButtonRefresh_Click(ByVal sender As Object, ByVal e As EventArgs)
		LoadProjectData()
	End Sub

	Private Sub LoadProjectData()
		LabelName.Text = Binder.Get(Project, "ProjectName").String
		DataBind()
	End Sub
End Class