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
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.UI
Imports Util.Ui

Namespace Util.Task
	Public Class Plan
		Private Resources As New Dictionary(Of String, Resource)()
		Private durationField As String
		Private resourceField As String
		Private idField As String
		Private fromField As String
        Private toField As String

        Public Start As DateTime

		Private all As New Dictionary(Of String, Task)()

        Private privateProcessed As List(Of Task)

		Public Property Processed() As List(Of Task)
			Get
				Return privateProcessed
			End Get
			Private Set(ByVal value As List(Of Task))
				privateProcessed = value
			End Set
		End Property

        Public Sub New()
            Start = (New Date(Date.Now.Year, Date.Now.Month, Date.Now.Day, Date.Now.Hour, Date.Now.Minute, 0)).AddMinutes(1)
        End Sub


		Public Sub LoadTasks(ByVal data As ICollection, ByVal idField As String, ByVal durationField As String, ByVal resourceField As String)
			Me.idField = idField
			Me.durationField = durationField
			Me.resourceField = resourceField

			For Each dataItem As Object In data
				Dim task As Task = CreateTask(dataItem)
				Dim r As Resource = FindResource(task.ResourceId)
				r.AddTask(task)
				all.Add(task.Id, task)
			Next dataItem
		End Sub

		Public Sub LoadLinks(ByVal data As ICollection, ByVal fromField As String, ByVal toField As String)
			Me.fromField = fromField
			Me.toField = toField

			For Each dataItem As Object In data
				Dim link As Link = CreateLink(dataItem)
				If all.ContainsKey(link.FromId) Then
					Dim task As Task = all(link.FromId)
					link.From = task
					task.Outgoing.Add(link)
				End If

				If all.ContainsKey(link.ToId) Then
					Dim task As Task = all(link.ToId)
					link.To = task
					task.Incoming.Add(link)
				End If
			Next dataItem
		End Sub

		Private Function CreateLink(ByVal dataItem As Object) As Link
			Dim [from] As String = Binder.Get(dataItem, fromField).String
			Dim [to] As String = Binder.Get(dataItem, toField).String

			Return New Link() With {.FromId = [from], .ToId = [to]}
		End Function

		Public Sub Process()
			For Each r As Resource In Resources.Values
				r.Prepare()
			Next r

			Do While AnyHasNext
				ResourceWithSmallestPoint.Next()
			Loop

			Processed = New List(Of Task)()
			For Each t In all.Values
				Processed.Add(t)
			Next t

			Processed.Sort(New TaskComparerStartEnd())
		End Sub

		Public ReadOnly Property AnyHasNext() As Boolean
			Get
				Return Resources.Values.Any(Function(r) r.HasNext)
			End Get
		End Property

		Private Function CreateTask(ByVal dataItem As Object) As Task
			Dim duration? As Integer = Binder.Get(dataItem, durationField).Int32
			Dim resource As String = Binder.Get(dataItem, resourceField).String
			Dim id As String = Binder.Get(dataItem, idField).String

			If duration Is Nothing Then
				duration = 15
			End If
			Return New Task() With {.Duration = CInt(Fix(duration)), .ResourceId = resource, .Id = id, .Source = dataItem}
		End Function

		Private Function FindResource(ByVal r As String) As Resource
			If r Is Nothing Then
				r = "NULL"
			End If
			If Not Resources.ContainsKey(r) Then
				Resources(r) = New Resource With {.Id = r, .Plan = Me, .Start = Start}
			End If
			Return Resources(r)
		End Function

		Public ReadOnly Property ResourceWithSmallestPoint() As Resource
			Get
				Dim point As Date = Date.MaxValue
				Dim smallest As Resource = Nothing
				For Each r In Resources.Values
					If r.Point < point AndAlso r.HasNext Then
						point = r.Point
						smallest = r
					End If
				Next r
				Return smallest
			End Get
		End Property

		Public ReadOnly Property VeryStart() As Date?
			Get
				If Processed.Count > 0 Then
					Return Processed(0).Start.Date
				End If
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property VeryEnd() As Date?
			Get
				If Processed.Count > 0 Then
					Return Processed(Processed.Count - 1).End
				End If
				Return Nothing

			End Get
		End Property

		Public ReadOnly Property Days() As Integer?
			Get
				If VeryStart Is Nothing OrElse VeryEnd Is Nothing Then
					Return Nothing
				End If
				Return CInt(Fix((VeryEnd.Value.Date.AddDays(1) - VeryStart.Value).TotalDays))
			End Get
		End Property
	End Class
End Namespace