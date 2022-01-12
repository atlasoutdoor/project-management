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
Imports System.Web.UI
Imports Util.Ui

Namespace Util.Task
	Public Class Task
		Public Property Source() As Object
		Public Property Duration() As Integer
		Public Property ResourceId() As String
		Public Property Id() As String
		Private privateIncoming As List(Of Link)
		Public Property Incoming() As List(Of Link)
			Get
				Return privateIncoming
			End Get
			Private Set(ByVal value As List(Of Link))
				privateIncoming = value
			End Set
		End Property
		Private privateOutgoing As List(Of Link)
		Public Property Outgoing() As List(Of Link)
			Get
				Return privateOutgoing
			End Get
			Private Set(ByVal value As List(Of Link))
				privateOutgoing = value
			End Set
		End Property

		Public Property Start() As Date
		Public Property [End]() As Date
		Public Property Resource() As Resource

		Public ReadOnly Property Text() As String
			Get
				Return Binder.Get(Source, "AssignmentNote").String
			End Get
		End Property
		'public string Color { get { return Binder.Get(Source, "AssignmentColor").String; } }

		Public ReadOnly Property IncomingAllFulfilled() As Boolean
			Get
				Return Incoming.All(Function(link) link.Fulfilled)
			End Get
		End Property

		Public Sub New()
			Incoming = New List(Of Link)()
			Outgoing = New List(Of Link)()
		End Sub

		''' <summary>
		''' Gets a property value of the original DataItem object.
		''' </summary>
		''' <param name="property"></param>
		''' <returns></returns>
		Default Public ReadOnly Property Item(ByVal [property] As String) As Object
			Get
				If TypeOf Source Is DataRow Then
					Dim dr As DataRow = CType(Source, DataRow)
					Return dr([property])
				End If
				Return DataBinder.GetPropertyValue(Source, [property], Nothing)
			End Get
		End Property
	End Class
End Namespace