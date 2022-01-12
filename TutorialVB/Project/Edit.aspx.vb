Imports System
Imports System.Collections
Imports System.Data
Imports System.Web
Imports System.Web.UI
Imports Util
Imports Util.Ui

Partial Public Class Project_Edit
	Inherits ProjectPage

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		Response.Cache.SetCacheability(HttpCacheability.NoCache)
		Response.Cache.SetNoStore()

		If Not IsPostBack Then
			Helper.FillDurations(DropDownListDuration)
			Helper.FillDurationsWithNull(DropDownListSpent, "(automatic)")

			Dim dr As DataRow = (New DataManager()).GetAssignment(Convert.ToInt32(Request.QueryString("id")))

			'DateTime start = (DateTime) dr["AssignmentStart"];
			'DateTime end = (DateTime) dr["AssignmentEnd"];

			DropDownListDuration.SelectedValue = Convert.ToString(dr("AssignmentDuration"))
			DropDownListSpent.SelectedValue = Convert.ToString(dr("AssignmentDurationReal"))

			TextBoxNote.Text = Convert.ToString(dr("AssignmentNote"))
			RadioButtonListStatus.SelectedValue = Convert.ToString(dr("AssignmentStatus"))

			DropDownListResource.DataSource = (New DataManager()).GetResourcesAllWithNull(ProjectId)
			DropDownListResource.DataTextField = "ResourceName"
			DropDownListResource.DataValueField = "ResourceId"
			DropDownListResource.DataBind()

			DropDownListResource.SelectedValue = Convert.ToString(dr("ResourceId"))

			UpdateStartEnd(Binder.Get(dr, "AssignmentStart").DateTime, Binder.Get(dr, "AssignmentEnd").DateTime, Binder.Get(dr, "AssignmentDurationReal").Int32)

		End If
	End Sub

	Private Sub UpdateStartEnd(ByVal start? As Date, ByVal [end]? As Date, ByVal spent? As Integer)
		'string status = Convert.ToString(dr["AssignmentStatus"]);
		'DateTime start = Convert.ToDateTime(dr["AssignmentStart"]);
		'DateTime end = Convert.ToDateTime(dr["AssignmentEnd"]);
		Dim took As String = Nothing
		If spent IsNot Nothing Then
			took = TimeSpan.FromMinutes(CDbl(spent)).ToString()
		End If

		Select Case RadioButtonListStatus.SelectedValue
			Case "planned"
				PanelStartEnd.Visible = False

				HiddenStart.Value = Nothing
				HiddenEnd.Value = Nothing
				HiddenSpent.Value = "" & spent
			Case "started"
				PanelStartEnd.Visible = True

				LabelStart.Text = start.ToString()
				HiddenStart.Value = start.ToIsoString()

				LabelEnd.Text = String.Empty
				HiddenEnd.Value = String.Empty

				LabelSpent.Text = took
				HiddenSpent.Value = "" & If(spent, String.Empty)
			Case "finished"
				PanelStartEnd.Visible = True

				LabelStart.Text = start.ToString()
				HiddenStart.Value = start.ToIsoString()

				LabelEnd.Text = [end].ToString()
				HiddenEnd.Value = [end].ToIsoString()

				LabelSpent.Text = took
				HiddenSpent.Value = spent.ToString()
		End Select
	End Sub

	Protected Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs)
		Dim note As String = TextBoxNote.Text
		'string color = DropDownListColor.SelectedValue;
'INSTANT VB NOTE: The variable id was renamed since Visual Basic does not handle local variables named the same as class members well:
		Dim id_Renamed As Integer = Convert.ToInt32(Request.QueryString("id"))
		Dim resource As String = DropDownListResource.SelectedValue
		Dim duration As Integer = Convert.ToInt32(DropDownListDuration.SelectedValue)
		'string spent = Convert.ToString(DropDownListSpent.SelectedValue);
		Dim status As String = RadioButtonListStatus.SelectedValue

		Dim start? As Date = Binder.Get(HiddenStart.Value).DateTime
		Dim [end]? As Date = Binder.Get(HiddenEnd.Value).DateTime
		Dim spent? As Integer = Binder.Get(HiddenSpent.Value).Int32

		CType(New DataManager(), DataManager).UpdateAssignment(id_Renamed, note, resource, duration, spent, start, [end], status)

		If status = "started" Then
			CType(New DataManager(), DataManager).UpdateAssignmentStartManual(id_Renamed, start)
		ElseIf status = "planned" Then
			CType(New DataManager(), DataManager).UpdateAssignmentStartManual(id_Renamed, Nothing)
		End If

		Dim ht As New Hashtable()
		ht("refresh") = "yes"
		ht("message") = "Event updated."

		Modal.Close(Me, ht)
	End Sub

	Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
		Modal.Close(Me)
	End Sub

	Protected Sub ButtonDelete_Click(ByVal sender As Object, ByVal e As EventArgs)
		CType(New DataManager(), DataManager).DeleteAssignment(Convert.ToInt32(Request.QueryString("id")))
		Dim ht As New Hashtable()
		ht("refresh") = "yes"
		ht("message") = "Event deleted."
		Modal.Close(Me, ht)
		'ScriptManager.RegisterStartupScript(this, this.GetType(), "modal", "<script type='text/javascript'>setTimeout(function() { modal.close({refresh:'yes',message:'Event deleted'); }, 0);</script>", false);
	End Sub

	Protected Sub UpdatePanel_Load(ByVal sender As Object, ByVal e As EventArgs)
		'ScriptManager.RegisterStartupScript(this, this.GetType(), "modal", "<script type='text/javascript'>setTimeout(function() { modal.stretch(); }, 0);</script>", false);
	End Sub

	Protected Sub RadioButtonListStatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
		CalculatePreview()
	End Sub

	Private Sub CalculatePreview()

		Dim dr As DataRow = (New DataManager()).GetAssignment(Convert.ToInt32(Request.QueryString("id")))

		Dim start? As Date = Nothing
		Dim [end]? As Date = Nothing

		'int? spent = Binder.Get(dr, "AssignmentDurationReal").Int32;
		'int? duration = Binder.Get(dr, "AssignmentDuration").Int32;
		Dim spent? As Integer = Binder.Get(DropDownListSpent.SelectedValue).Int32
		Dim duration? As Integer = Binder.Get(DropDownListDuration.SelectedValue).Int32

		Select Case RadioButtonListStatus.SelectedValue
			Case "planned"
				start = Nothing
				[end] = Nothing
			Case "started"
				If Binder.Get(dr, "AssignmentStartManual").IsNotNull Then
					start = Binder.Get(dr, "AssignmentStartManual").DateTime
				Else
					start = Date.Now
				End If
				[end] = Nothing
			Case "finished"
				[end] = If(Binder.Get(dr, "AssignmentEnd").DateTime, Date.Now)

				start = Binder.Get(dr, "AssignmentStartManual").DateTime

				If start Is Nothing Then ' closing directly
					spent = If(spent, duration)
					start = [end].Value.AddMinutes(CDbl(-spent))
				Else ' started previously
					If spent Is Nothing Then ' automatic
						spent = CType(([end] - start).Value.TotalMinutes, Integer?)
					Else ' override start
						start = [end].Value.AddMinutes(CDbl(-spent))
					End If
				End If


		End Select
		UpdateStartEnd(start, [end], spent)
	End Sub

	Protected Sub DropDownListDuration_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
		CalculatePreview()
	End Sub

	Protected Sub DropDownListSpent_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
		CalculatePreview()
	End Sub
End Class