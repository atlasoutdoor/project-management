'
'Copyright © 2013 Annpoint, s.r.o.
'
'Licensed under the Apache License, Version 2.0 (the "License");
'you may not use this file except in compliance with the License.
'You may obtain a copy of the License at
'
'http://www.apache.org/licenses/LICENSE-2.0
'
'Unless required by applicable law or agreed to in writing, software
'distributed under the License is distributed on an "AS IS" BASIS,
'WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'See the License for the specific language governing permissions and
'limitations under the License.
'
'-------------------------------------------------------------------------
'
'NOTE: Reuse requires the following acknowledgement (see also NOTICE):
'This product includes DayPilot (http://www.daypilot.org) developed by Annpoint, s.r.o.
'

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI.WebControls

''' <summary>
''' Summary description for Helper
''' </summary>
Public Class Helper

	Public Shared Sub FillDurationsWithNull(ByVal list As DropDownList)
		FillDurationsWithNull(list, "(not set)")
	End Sub

	Public Shared Sub FillDurations(ByVal DropDownListDuration As DropDownList)
		DropDownListDuration.Items.Add(New ListItem("15 minutes", "15"))
		DropDownListDuration.Items.Add(New ListItem("30 minutes", "30"))
		DropDownListDuration.Items.Add(New ListItem("45 minutes", "45"))
		DropDownListDuration.Items.Add(New ListItem("1 hour", "60"))
		DropDownListDuration.Items.Add(New ListItem("1.5 hours", "90"))
		DropDownListDuration.Items.Add(New ListItem("2 hours", "120"))
		DropDownListDuration.Items.Add(New ListItem("3 hours", "180"))
		DropDownListDuration.Items.Add(New ListItem("4 hours", "240"))
		DropDownListDuration.Items.Add(New ListItem("5 hours", "300"))
		DropDownListDuration.Items.Add(New ListItem("6 hours", "360"))
		DropDownListDuration.Items.Add(New ListItem("7 hours", "420"))
		DropDownListDuration.Items.Add(New ListItem("8 hours", "480"))
	End Sub

	Public Shared Function StatusToColor(ByVal o As Object) As String
		Dim status As String = Convert.ToString(o)
		If String.IsNullOrEmpty(status) Then
			status = "planned"
		End If
		Select Case status
			Case "planned"
				Return "#004dc3"
			Case "started"
				Return "#008e00"
			Case "finished"
				Return "#eab71e"
		End Select
		Throw New ArgumentException("Unrecognized status")
	End Function

	Public Shared Sub FillDurationsWithNull(ByVal list As DropDownList, ByVal nullText As String)
		list.Items.Add(New ListItem(nullText, String.Empty))
		FillDurations(list)

	End Sub
End Class