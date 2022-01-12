Imports System
Imports System.Collections
Imports System.Data
Imports System.Web.UI.WebControls
Imports DayPilot.Utils
Imports DayPilot.Web.Ui.Enums
Imports Util

Partial Public Class Project_NewResource
	Inherits ProjectPage

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then
			DropDownListGroup.DataTextField = "GroupName"
			DropDownListGroup.DataValueField = "GroupId"

			DropDownListGroup.DataSource = (New DataManager()).GetGroups(ProjectId)
			DropDownListGroup.DataBind()
		End If
	End Sub

	Protected Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs)

		'DateTime end = Convert.ToDateTime(TextBoxEnd.Text);
		Dim name As String = TextBoxName.Text
		Dim group As String = DropDownListGroup.SelectedValue

		CType(New DataManager(), DataManager).CreateResource(ProjectId, name, group)

		' passed to the modal dialog close handler, see Scripts/DayPilot/event_handling.js
		Dim ht As New Hashtable()
		ht("refresh") = "yes"
		ht("message") = "Resource created."

		Modal.Close(Me, ht)
	End Sub

	Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
		Modal.Close(Me)
	End Sub
End Class
