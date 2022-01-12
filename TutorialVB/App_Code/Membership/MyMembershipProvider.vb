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
Imports System.Web.Security

Namespace Util.Membership

	Public Class MyMembershipProvider
		Inherits MembershipProvider

		Public Overrides Function CreateUser(ByVal username As String, ByVal password As String, ByVal email As String, ByVal passwordQuestion As String, ByVal passwordAnswer As String, ByVal isApproved As Boolean, ByVal providerUserKey As Object, <System.Runtime.InteropServices.Out()> ByRef status As MembershipCreateStatus) As MembershipUser
			CType(New DataManager(), DataManager).CreateUser(username, password, email)
			status = MembershipCreateStatus.Success

			Return New MembershipUser(providerName:= "MyMembershipProvider", name:= username, providerUserKey:= Nothing, email:= email, passwordQuestion:= "", comment:= "", isApproved:= True, isLockedOut:= False, creationDate:= Date.Now, lastLoginDate:= Date.Now, lastActivityDate:= Date.Now, lastPasswordChangedDate:= Date.Now, lastLockoutDate:= Date.MinValue)
		End Function

		Public Overrides Function ChangePasswordQuestionAndAnswer(ByVal username As String, ByVal password As String, ByVal newPasswordQuestion As String, ByVal newPasswordAnswer As String) As Boolean
			Throw New NotImplementedException()
		End Function

		Public Overrides Function GetPassword(ByVal username As String, ByVal answer As String) As String
			Throw New NotImplementedException()
		End Function

		Public Overrides Function ChangePassword(ByVal username As String, ByVal oldPassword As String, ByVal newPassword As String) As Boolean
			Throw New NotImplementedException()
		End Function

		Public Overrides Function ResetPassword(ByVal username As String, ByVal answer As String) As String
			Throw New NotImplementedException()
		End Function

		Public Overrides Sub UpdateUser(ByVal user As MembershipUser)
			Throw New NotImplementedException()
		End Sub

		Public Overrides Function ValidateUser(ByVal username As String, ByVal password As String) As Boolean
			Return (New DataManager()).ValidateUser(username, password)
		End Function

		Public Overrides Function UnlockUser(ByVal userName As String) As Boolean
			Throw New NotImplementedException()
		End Function

		Public Overrides Function GetUser(ByVal providerUserKey As Object, ByVal userIsOnline As Boolean) As MembershipUser
			Throw New NotImplementedException()
		End Function

		Public Overrides Function GetUser(ByVal username As String, ByVal userIsOnline As Boolean) As MembershipUser
			Throw New NotImplementedException()
		End Function

		Public Overrides Function GetUserNameByEmail(ByVal email As String) As String
			Dim dr As DataRow = (New DataManager()).GetUserByEmail(email)
			If dr Is Nothing Then
				Return Nothing
			End If
			Return CStr(dr("UserId"))
		End Function

		Public Overrides Function DeleteUser(ByVal username As String, ByVal deleteAllRelatedData As Boolean) As Boolean
			Throw New NotImplementedException()
		End Function

		Public Overrides Function GetAllUsers(ByVal pageIndex As Integer, ByVal pageSize As Integer, <System.Runtime.InteropServices.Out()> ByRef totalRecords As Integer) As MembershipUserCollection
			Throw New NotImplementedException()
		End Function

		Public Overrides Function GetNumberOfUsersOnline() As Integer
			Throw New NotImplementedException()
		End Function

		Public Overrides Function FindUsersByName(ByVal usernameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, <System.Runtime.InteropServices.Out()> ByRef totalRecords As Integer) As MembershipUserCollection
			Throw New NotImplementedException()
		End Function

		Public Overrides Function FindUsersByEmail(ByVal emailToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, <System.Runtime.InteropServices.Out()> ByRef totalRecords As Integer) As MembershipUserCollection
			Throw New NotImplementedException()
		End Function

		Public Overrides ReadOnly Property EnablePasswordRetrieval() As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property EnablePasswordReset() As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property RequiresQuestionAndAnswer() As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Property ApplicationName() As String
			Get
				Throw New NotImplementedException()
			End Get
			Set(ByVal value As String)
				Throw New NotImplementedException()
			End Set
		End Property

		Public Overrides ReadOnly Property MaxInvalidPasswordAttempts() As Integer
			Get
				Throw New NotImplementedException()
			End Get
		End Property

		Public Overrides ReadOnly Property PasswordAttemptWindow() As Integer
			Get
				Throw New NotImplementedException()
			End Get
		End Property

		Public Overrides ReadOnly Property RequiresUniqueEmail() As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property PasswordFormat() As MembershipPasswordFormat
			Get
				Throw New NotImplementedException()
			End Get
		End Property

		Public Overrides ReadOnly Property MinRequiredPasswordLength() As Integer
			Get
				Return 6
			End Get
		End Property

		Public Overrides ReadOnly Property MinRequiredNonAlphanumericCharacters() As Integer
			Get
				Throw New NotImplementedException()
			End Get
		End Property

		Public Overrides ReadOnly Property PasswordStrengthRegularExpression() As String
			Get
				Throw New NotImplementedException()
			End Get
		End Property
	End Class
End Namespace