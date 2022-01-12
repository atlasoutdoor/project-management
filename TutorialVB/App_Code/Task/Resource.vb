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

Namespace Util.Task
	Public Class Resource
		Public Property Plan() As Plan

		Public Property Id() As String
		Public Property Start() As Date

		Private privateTasks As List(Of Task)
		Public Property Tasks() As List(Of Task)
			Get
				Return privateTasks
			End Get
			Private Set(ByVal value As List(Of Task))
				privateTasks = value
			End Set
		End Property

		Private privateDone As List(Of Task)
		Public Property Done() As List(Of Task)
			Get
				Return privateDone
			End Get
			Private Set(ByVal value As List(Of Task))
				privateDone = value
			End Set
		End Property
		Private privateReady As List(Of Task)
		Public Property Ready() As List(Of Task)
			Get
				Return privateReady
			End Get
			Private Set(ByVal value As List(Of Task))
				privateReady = value
			End Set
		End Property
		Private privateBlocked As List(Of Task)
		Public Property Blocked() As List(Of Task)
			Get
				Return privateBlocked
			End Get
			Private Set(ByVal value As List(Of Task))
				privateBlocked = value
			End Set
		End Property

		Private privatePoint As Date
		Public Property Point() As Date
			Get
				Return privatePoint
			End Get
			Private Set(ByVal value As Date)
				privatePoint = value
			End Set
		End Property

		Public Sub New()
			Start = (New Date(Date.Now.Year, Date.Now.Month, Date.Now.Day, Date.Now.Hour, Date.Now.Minute, 0)).AddMinutes(1)
			Tasks = New List(Of Task)()

			Blocked = New List(Of Task)()
			Ready = New List(Of Task)()
			Done = New List(Of Task)()
		End Sub

		Public Sub Prepare()
			'Tasks.Sort(new TaskComparerLinkCount());
			Tasks.Sort(New TaskComparerOrdinal())

			For Each task In Tasks
				If task.IncomingAllFulfilled Then
					Ready.Add(task)
				End If
			Next task
			Point = Start
		End Sub


		Private Sub ScheduleTask(ByVal task As Task)
			Dim minutesToBeAdded As Integer = task.Duration
			Dim test As Date = task.Start
			Dim startFixed As Boolean = False

			Do While minutesToBeAdded > 0
				If IsBusinessMinute(test) Then
					minutesToBeAdded -= 1
					If Not startFixed Then
						task.Start = test
					End If
					startFixed = True
				End If

				test = test.AddMinutes(1)
			Loop

			task.End = test
		End Sub


		Private Function IsBusinessMinute(ByVal start As Date) As Boolean
			Const businessStartHour As Integer = 9
			Const businessEndHour As Integer = 17

			Dim freeDays = { DayOfWeek.Saturday, DayOfWeek.Sunday }

			If freeDays.Contains(start.DayOfWeek) Then
				Return False
			End If

			If start.Hour < businessStartHour Then
				Return False
			End If

			If start.Hour >= businessEndHour Then
				Return False
			End If
			Return True
		End Function


		Public Sub [Next]()
			If Ready.Count = 0 Then
				Return
			End If

			Dim task = Ready(0)

			task.Start = Point

			ScheduleTask(task)

			Point = task.End

			For Each link As Link In task.Outgoing
				link.Fulfilled = True

				If link.To.IncomingAllFulfilled Then

					link.To.Resource.Blocked.Remove(link.To)
					link.To.Resource.Ready.Add(link.To)
				End If
			Next link

			Done.Add(task)
			Ready.Remove(task)
		End Sub

		Public ReadOnly Property HasNext() As Boolean
			Get
				Return Ready.Count > 0
			End Get
		End Property


		Public Sub AddTask(ByVal task As Task)
			Tasks.Add(task)
			task.Resource = Me
		End Sub
	End Class
End Namespace