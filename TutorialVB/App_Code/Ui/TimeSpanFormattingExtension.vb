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

Namespace Util.Ui
	Public Module TimeSpanFormattingExtension
		<System.Runtime.CompilerServices.Extension> _
		Public Function ToHourMinuteString(ByVal span As TimeSpan) As String
			If span = TimeSpan.Zero Then
				Return String.Empty
			End If
			Return String.Format("{0}:{1:00}", Math.Floor(span.TotalHours), span.Minutes)
		End Function

		<System.Runtime.CompilerServices.Extension> _
		Public Function ToIsoString(ByVal [date]? As Date) As String
			If [date] Is Nothing Then
				Return String.Empty
			End If
			Return [date].Value.ToString("s")
		End Function
	End Module
End Namespace