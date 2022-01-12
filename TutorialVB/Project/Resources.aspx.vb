Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports DayPilot.Utils
Imports DayPilot.Web.Ui.Events.Scheduler
Imports Util.Task
Imports Util.Ui
Imports Resource = DayPilot.Web.Ui.Resource

Partial Public Class Project_Resources
	Inherits ProjectPage

	Private _plan As Plan = Nothing
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)

		If Not IsPostBack Then
			DayPilotScheduler1.StartDate = If(Plan.VeryStart, Date.Today)
			DayPilotScheduler1.Days = If(Plan.Days, 1)
			LoadEvents()
			UpdateZoomLevel()
			CreateResources()
		End If
	End Sub

	Private Sub LoadEvents()
		DayPilotScheduler1.DataSource = Data
		DataBind()
	End Sub


	Private Sub CreateResources()
		DayPilotScheduler1.Resources.Clear()

		AddDefaultGroup()

		For Each dr As DataRow In Groups.Rows
			Dim name As String = CStr(dr("GroupName"))
'INSTANT VB NOTE: The variable id was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim id_Renamed As Integer = Convert.ToInt32(dr("GroupId"))

			Dim html As String = table(name, 0)
			DayPilotScheduler1.Resources.Add(New Resource(html, "GROUP"))

			AddChildren(id_Renamed)
		Next dr
	End Sub

	Private Sub AddDefaultGroup()
		Dim name As String = "(default)"

		Dim html As String = table(name, 0)
		DayPilotScheduler1.Resources.Add(New Resource(html, "DEFAULT"))

		AddChildren(Nothing)
	End Sub

	Private Sub AddChildren(ByVal parent? As Integer)
		For Each dr As DataRow In Resources(parent).Rows
			Dim name As String = CStr(dr("ResourceName"))
'INSTANT VB NOTE: The variable id was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim id_Renamed As Integer = Convert.ToInt32(dr("ResourceId"))

			Dim html As String = table(name, 1)
			DayPilotScheduler1.Resources.Add(New Resource(html, id_Renamed.ToString()))
		Next dr
	End Sub

	Private ReadOnly Property Groups() As DataTable
		Get
			Return (New DataManager()).GetGroups(ProjectId)
		End Get
	End Property

	Private Function Resources(ByVal group? As Integer) As DataTable
		Return (New DataManager()).GetResources(ProjectId, group)
	End Function

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

	Private ReadOnly Property Finished() As List(Of Task)
		Get
			Dim tasks As New List(Of Task)()
			Dim table As DataTable = (New DataManager()).GetAssignmentsFinished(ProjectId)
			For Each row In table.Rows

				Dim task = New Task() With {.Start = CDate(Binder.Get(row, "AssignmentStart").DateTime), .End = CDate(Binder.Get(row, "AssignmentEnd").DateTime), .Id = Binder.Get(row, "AssignmentId").String, .ResourceId = Binder.Get(row, "ResourceId").String, .Source = row}
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

				'throw new Exception("task: " + row["AssignmentNote"]);

				Dim task = New Task() With {.Start = CDate(Binder.Get(row, "AssignmentStart").DateTime), .End = Date.Now, .Id = Binder.Get(row, "AssignmentId").String, .ResourceId = Binder.Get(row, "ResourceId").String, .Source = row}

				tasks.Add(task)
			Next row
			Return tasks
		End Get
	End Property


	Private Function table(ByVal name As String, ByVal indent As Integer) As String
		Dim sb As New StringBuilder()
		sb.Append("<div>")

		For i As Integer = 0 To indent - 1
			sb.Append("<div style='display:inline-block; width: 20px; height: 1px;'>")
			sb.Append("</div>")
		Next i

		sb.Append("<div style='display:inline-block;'>")
		sb.Append(name)
		sb.Append("</div>")


		sb.Append("</div>")

		Return sb.ToString()
	End Function

	Protected Sub UpdatePanelScheduler_Load(ByVal sender As Object, ByVal e As EventArgs)
	End Sub

	Protected Sub DayPilotScheduler1_BeforeEventRender(ByVal sender As Object, ByVal e As BeforeEventRenderEventArgs)
		Dim t As Task = CType(e.DataItem.Source, Task)
		e.DurationBarColor = Helper.StatusToColor(t("AssignmentStatus"))
	End Sub

	Protected Sub ButtonRefresh_Click(ByVal sender As Object, ByVal e As EventArgs)
		LoadEvents()
		CreateResources()
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

	Protected Sub RadioButtonListZoom_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
		UpdateZoomLevel()
		LoadEvents()
	End Sub

	Protected Sub CheckBoxHideFinished_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
		LoadEvents()
	End Sub

End Class
