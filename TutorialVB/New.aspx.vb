Imports System
Imports System.Collections
Imports System.Data
Imports System.Web.UI.WebControls
Imports DayPilot.Utils
Imports DayPilot.Web.Ui.Enums
Imports Util

Partial Public Class NewDialog
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then
		End If
	End Sub



	Protected Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs)
		Dim name As String = TextBoxName.Text
		CType(New DataManager(), DataManager).CreateProject(name)

		Dim ht As New Hashtable()
		ht("refresh") = "yes"
		Modal.Close(Me, ht)
	End Sub

	Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
		Modal.Close(Me)
	End Sub
End Class
