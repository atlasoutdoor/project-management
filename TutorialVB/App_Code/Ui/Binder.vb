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
Imports System.Data
Imports System.Reflection
Imports System.Web.UI
Imports PropertyAttributes = System.Data.PropertyAttributes

Namespace Util.Ui
	Public Class Binder
		Public Shared Function [Get](ByVal o As Object, ByVal [property] As String) As DataItem
			If TypeOf o Is DataRow Then
				Dim dr As DataRow = CType(o, DataRow)
				Return New DataItem() With {.Source = dr([property])}
			End If
			Return New DataItem() With {.Source = ReadPropertyValue(o, [property])}

		End Function

		Private Shared Function ReadPropertyValue(ByVal o As Object, ByVal [property] As String) As Object
			Dim type As Type = o.GetType()
			Dim pi As PropertyInfo = type.GetProperty([property])
			If pi IsNot Nothing Then
				Return pi.GetValue(o, Nothing)
			Else ' try to read using indexed property
				Dim mi As MethodInfo = type.GetMethod("get_Item", New Type() {GetType(String)})
				If mi IsNot Nothing Then
					Return mi.Invoke(o, New Object() {[property]})
				End If
			End If

			Throw New ArgumentException("Property or index not found.")
		End Function

		Public Shared Function [Get](ByVal value As Object) As DataItem
			Return New DataItem() With {.Source = value}
		End Function

		Public Class DataItem
			Friend Source As Object

			Public ReadOnly Property [Object]() As Object
				Get
					If Source Is DBNull.Value Then
						Return Nothing
					End If
					Return Source
				End Get
			End Property
			Public ReadOnly Property [String]() As String
				Get
					If Source Is DBNull.Value OrElse Source Is Nothing Then
						Return Nothing
					End If
					Return Convert.ToString(Source)
				End Get
			End Property

            Public ReadOnly Property [DateTime]() As Date?
                Get
                    If Source Is DBNull.Value OrElse Source Is Nothing Then
                        Return Nothing
                    End If
                    If TypeOf Source Is String AndAlso CStr(Source) = [String].Empty Then
                        Return Nothing
                    End If
                    Return Convert.ToDateTime(Source)
                End Get
            End Property

			Public ReadOnly Property Int32() As Integer?
				Get
					If Source Is DBNull.Value OrElse Source Is Nothing Then
						Return Nothing
					End If
					If TypeOf Source Is String AndAlso CStr(Source) = [String].Empty Then
						Return Nothing
					End If
					Return Convert.ToInt32(Source)
				End Get
			End Property

			Public ReadOnly Property [Double]() As Double?
				Get
					If Source Is DBNull.Value OrElse Source Is Nothing Then
						Return Nothing
					End If
					If TypeOf Source Is String AndAlso CStr(Source) = [String].Empty Then
						Return Nothing
					End If
					Return Convert.ToDouble(Source)
				End Get
			End Property

			Public ReadOnly Property IsNull() As Boolean
				Get
					Return Source Is Nothing OrElse Source Is DBNull.Value
				End Get
			End Property

			Public ReadOnly Property IsNotNull() As Boolean
				Get
					Return Not IsNull
				End Get
			End Property

			Public ReadOnly Property TimeSpanFromMinutes() As TimeSpan
				Get
					If IsNull Then
						Return TimeSpan.Zero
					End If
					Return TimeSpan.FromMinutes(CDbl([Double]))
				End Get
			End Property
		End Class

	End Class


End Namespace