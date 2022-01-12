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
Imports System.Data
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports Util.Ui

''' <summary>
''' Summary description for ProjectPage
''' </summary>
Public Class ProjectPage
	Inherits Page

	Private privateActiveUser As DataRow
	Public Property ActiveUser() As DataRow
		Get
			Return privateActiveUser
		End Get
		Private Set(ByVal value As DataRow)
			privateActiveUser = value
		End Set
	End Property

	Public Sub New()
		AddHandler Load, AddressOf ProjectPage_Load
	End Sub

	Private Sub ProjectPage_Load(ByVal sender As Object, ByVal e As EventArgs)
		ActiveUser = (New DataManager()).GetUserById(User.Identity.Name)
	End Sub

	Public ReadOnly Property Project() As DataRow
		Get
			Dim info = Request.PathInfo
'INSTANT VB NOTE: The variable id was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim id_Renamed = info.Substring(1)

			Return (New DataManager()).GetProject(id_Renamed)
		End Get
	End Property

	Public ReadOnly Property ProjectId() As Integer
		Get
			Return CInt(Fix(Binder.Get(Project, "ProjectId").Int32))
		End Get

	End Property

End Class