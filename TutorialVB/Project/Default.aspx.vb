Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports DayPilot.Web.Ui
Imports DayPilot.Web.Ui.Data
Imports DayPilot.Web.Ui.Events
Imports DayPilot.Web.Ui.Events.Scheduler
Imports Util.Task
Imports Util.Ui

Partial Public Class Project_Default
	Inherits ProjectPage

	Private _plan As Plan = Nothing

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then
			DayPilotScheduler1.StartDate = If(Plan.VeryStart, Date.Today)
			DayPilotScheduler1.Days = If(Plan.Days, 1)
			UpdateZoomLevel()
			LoadEvents()
		End If

		Dim cols As String = (New DataManager()).GetUserConfig(User.Identity.Name, "project.cols")
		If cols IsNot Nothing Then
			DayPilotScheduler1.RowHeaderColumnWidths = cols
		End If

	End Sub

	Private Sub UpdateZoomLevel()
		Dim level As String = RadioButtonListZoom.SelectedValue
		Select Case level
			Case "Hours"
				DayPilotScheduler1.CellDuration = 60
			Case "Days"
				DayPilotScheduler1.CellDuration = 1440
		End Select
	End Sub


	Private Function TaskLink(ByVal name As String, ByVal id As Integer, ByVal status As String) As String
		If status = String.Empty Then
			status = Nothing
		End If
		Dim sb As New StringBuilder()
		sb.Append("<div class='task_status ")
		sb.Append(If(status, "planned"))
		sb.Append("' data-taskid='" & id & "'></div>")

		sb.Append("<a title='")
		sb.Append(name)
		sb.Append("' ")
		sb.Append("href='javascript:edit(""")
		sb.Append(id)
		sb.Append(""")'>")
		sb.Append(name)
		sb.Append("</a>")

		Return sb.ToString()
	End Function

	Private ReadOnly Property Plan() As Plan
		Get
			If _plan Is Nothing Then
				Dim table As DataTable = (New DataManager()).GetAssignmentsPlanned(ProjectId)

				_plan = New Plan()
				_plan.LoadTasks(table.Rows, "AssignmentId", "AssignmentDuration", "ResourceId")
				_plan.Process()
			End If
			Return _plan
		End Get
	End Property

	Private ReadOnly Property Data() As List(Of Task)
		Get
			Dim all As New List(Of Task)()
			If Not CheckBoxHideFinished.Checked Then
				all.AddRange(Finished)
			End If
			all.AddRange(Started)
			all.AddRange(Plan.Processed)
			Return all
		End Get
	End Property

	Private ReadOnly Property Finished() As List(Of Task)
		Get
			Dim tasks As New List(Of Task)()
			Dim table As DataTable = (New DataManager()).GetAssignmentsFinished(ProjectId)
			For Each row In table.Rows

				Dim task = New Task() With {.Start = CDate(Binder.Get(row, "AssignmentStart").DateTime), .End = CDate(Binder.Get(row, "AssignmentEnd").DateTime), .Id = Binder.Get(row, "AssignmentId").String, .Source = row}
				tasks.Add(task)
			Next row
			Return tasks
		End Get
	End Property

	Private ReadOnly Property Started() As List(Of Task)
		Get
			Dim tasks As New List(Of Task)()
			Dim table As DataTable = (New DataManager()).GetAssignmentsStarted(ProjectId)
			For Each row As DataRow In table.Rows

				Dim task = New Task() With {.Start = CDate(Binder.Get(row, "AssignmentStart").DateTime), .End = Date.Now, .Id = Binder.Get(row, "AssignmentId").String, .Source = row}
				tasks.Add(task)
			Next row

			Return tasks
		End Get
	End Property


	Protected Sub UpdatePanelScheduler_Load(ByVal sender As Object, ByVal e As EventArgs)
	End Sub

	Protected Sub DayPilotScheduler1_BeforeEventRender(ByVal sender As Object, ByVal e As BeforeEventRenderEventArgs)
		Dim t As Task = CType(e.DataItem.Source, Task)
		e.DurationBarColor = Helper.StatusToColor(t("AssignmentStatus"))
	End Sub


	Protected Sub DayPilotScheduler1_HeaderColumnWidthChanged(ByVal sender As Object, ByVal e As HeaderColumnWidthChangedEventArgs)
		CType(New DataManager(), DataManager).SetUserConfig(User.Identity.Name, "project.cols", DayPilotScheduler1.RowHeaderColumnWidths)
		LoadEvents()
	End Sub

	Protected Sub RadioButtonListZoom_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
		UpdateZoomLevel()

		LoadEvents()
	End Sub


	Protected Sub ButtonReorder_Click(ByVal sender As Object, ByVal e As EventArgs)
		Dim order As String = HiddenOrder.Value
		CType(New DataManager(), DataManager).UpdateOrder(ProjectId, order)

		LoadEvents()
	End Sub

	Private Sub LoadEvents()
		DayPilotScheduler1.DataSource = Data
		LabelSummary.Text = String.Format("Estimated finish time: {0}", Plan.VeryEnd)
		DataBind()
	End Sub

	Protected Sub ButtonRefresh_Click(ByVal sender As Object, ByVal e As EventArgs)
		LoadEvents()
	End Sub

	Protected Sub CheckBoxHideFinished_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
		LoadEvents()
	End Sub

	Protected Sub DayPilotScheduler1_BeforeResHeaderRender(ByVal sender As Object, ByVal e As BeforeHeaderRenderEventArgs)
		Dim task As DataItemWrapper = e.DataItem

		Dim name As String = CStr(task("AssignmentNote"))
'INSTANT VB NOTE: The variable id was renamed since Visual Basic does not handle local variables named the same as class members well:
		Dim id_Renamed As Integer = Convert.ToInt32(task("AssignmentId"))
		Dim resource As String = Convert.ToString(task("ResourceName"))
		Dim status As String = Convert.ToString(task("AssignmentStatus"))

		Dim duration As TimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(task("AssignmentDuration")))

		Dim durationString As String = duration.ToHourMinuteString()
		Dim spentString As String = If(Binder.Get(task, "AssignmentDurationReal").IsNotNull, Binder.Get(task, "AssignmentDurationReal").TimeSpanFromMinutes.ToHourMinuteString(), String.Empty)

		e.InnerHTML = TaskLink(name, id_Renamed, status)
		e.Columns(0).InnerHTML = "<div style='text-align:right'>" & durationString & "</div>"
		e.Columns(1).InnerHTML = "<div style='text-align:right'>" & spentString & "</div>"
		e.Columns(2).InnerHTML = resource

	End Sub
End Class