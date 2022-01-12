Imports System
Imports System.Collections
Imports System.Data
Imports System.Web.UI.WebControls
Imports DayPilot.Utils
Imports DayPilot.Web.Ui.Enums
Imports Util

Partial Public Class Project_New
	Inherits ProjectPage

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then
			Dim start As Date = Convert.ToDateTime(Request.QueryString("start"))
			Helper.FillDurations(DropDownListDuration)
		End If
	End Sub

	Protected Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs)

		'DateTime end = Convert.ToDateTime(TextBoxEnd.Text);
		Dim duration As Integer = Convert.ToInt32(DropDownListDuration.SelectedValue)
		Dim note As String = TextBoxNote.Text

		CType(New DataManager(), DataManager).CreateAssignment(ProjectId, duration, note)

		' passed to the modal dialog close handler, see Scripts/DayPilot/event_handling.js
		Dim ht As New Hashtable()
		ht("refresh") = "yes"
		ht("message") = "Event created."

		Modal.Close(Me, ht)
	End Sub

	Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
		Modal.Close(Me)
	End Sub
End Class
