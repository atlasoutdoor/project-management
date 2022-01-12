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

Imports System.Collections.Generic

Namespace Util.Task
	Public Class TaskComparerStartEnd
		Implements IComparer(Of Task)

		''' <summary>
		''' Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		''' </summary>
		''' <returns>
		''' A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
		''' </returns>
		''' <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
		Public Function Compare(ByVal x As Task, ByVal y As Task) As Integer Implements IComparer(Of Task).Compare
			Dim result As Long = 0
			result = x.Start.Ticks - y.Start.Ticks

			If result <> 0 Then
				Return If(result > 0, 1, -1)
			End If

			result = x.End.Ticks - y.End.Ticks

			If result = 0 Then
				Return 0
			End If

			Return If(result > 0, 1, -1)

		End Function
	End Class
End Namespace