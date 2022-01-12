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
Imports System.Web.Security

Namespace Util.Membership
	Public Class MyRoleProvider
		Inherits RoleProvider

		Public Overrides Function IsUserInRole(ByVal username As String, ByVal roleName As String) As Boolean
			Throw New NotImplementedException()
		End Function

		Public Overrides Function GetRolesForUser(ByVal username As String) As String()
			Throw New NotImplementedException()
		End Function

		Public Overrides Sub CreateRole(ByVal roleName As String)
			Throw New NotImplementedException()
		End Sub

		Public Overrides Function DeleteRole(ByVal roleName As String, ByVal throwOnPopulatedRole As Boolean) As Boolean
			Throw New NotImplementedException()
		End Function

		Public Overrides Function RoleExists(ByVal roleName As String) As Boolean
			Throw New NotImplementedException()
		End Function

		Public Overrides Sub AddUsersToRoles(ByVal usernames() As String, ByVal roleNames() As String)
			Throw New NotImplementedException()
		End Sub

		Public Overrides Sub RemoveUsersFromRoles(ByVal usernames() As String, ByVal roleNames() As String)
			Throw New NotImplementedException()
		End Sub

		Public Overrides Function GetUsersInRole(ByVal roleName As String) As String()
			Throw New NotImplementedException()
		End Function

		Public Overrides Function GetAllRoles() As String()
			Throw New NotImplementedException()
		End Function

		Public Overrides Function FindUsersInRole(ByVal roleName As String, ByVal usernameToMatch As String) As String()
			Throw New NotImplementedException()
		End Function

		Public Overrides Property ApplicationName() As String
			Get
				Throw New NotImplementedException()
			End Get
			Set(ByVal value As String)
				Throw New NotImplementedException()
			End Set
		End Property
	End Class
End Namespace