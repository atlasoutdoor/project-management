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
Imports System.Data.Common
Imports System.Linq
Imports System.Web
Imports Util.Ui

''' <summary>
''' Summary description for DataManager
''' </summary>
Public Class DataManager

	#Region "Helper methods"
	Private ReadOnly Property ConnectionString() As String
		Get
			Return Db.ConnectionString()
		End Get
	End Property

	Private ReadOnly Property Factory() As DbProviderFactory
		Get
			Return Db.Factory()
		End Get
	End Property

	Private Function CreateConnection() As DbConnection
		Dim connection As DbConnection = Factory.CreateConnection()
		connection.ConnectionString = ConnectionString
		Return connection
	End Function

	Private Function CreateCommand(ByVal text As String) As DbCommand
		Dim command As DbCommand = Factory.CreateCommand()
		command.CommandText = text
		command.Connection = CreateConnection()

		Return command
	End Function

	Private Function CreateCommand(ByVal text As String, ByVal connection As DbConnection) As DbCommand
		Dim command As DbCommand = Factory.CreateCommand()
		command.CommandText = text
		command.Connection = connection

		Return command
	End Function

	Private Sub AddParameterWithValue(ByVal cmd As DbCommand, ByVal name As String, ByVal value As Object)
		Dim parameter = Factory.CreateParameter()
		parameter.Direction = ParameterDirection.Input
		parameter.ParameterName = name
		parameter.Value = value
		cmd.Parameters.Add(parameter)
	End Sub

	Private Function GetIdentity(ByVal c As DbConnection) As Integer
		Dim cmd = CreateCommand(Db.IdentityCommand(), c)
		Return Convert.ToInt32(cmd.ExecuteScalar())
	End Function

	Private Function CreateDataAdapter(ByVal [select] As String) As DbDataAdapter
		Dim da As DbDataAdapter = Factory.CreateDataAdapter()
		da.SelectCommand = CreateCommand([select])
		Return da
	End Function

	#End Region


	Public Function GetAssignment(ByVal id As Integer) As DataRow
		Dim da = CreateDataAdapter("select * from [Assignment] where [Assignment].[AssignmentId] = @id")
		AddParameterWithValue(da.SelectCommand, "id", id)
		Dim dt As New DataTable()
		da.Fill(dt)
		If dt.Rows.Count = 1 Then
			Return dt.Rows(0)
		End If
		Return Nothing
	End Function


	Public Function GetAssignments(ByVal projectId As Integer) As DataTable
		Dim dt As New DataTable()
		Dim da = CreateDataAdapter("select * from [Assignment] left outer join [Resource] on [Assignment].[ResourceId] = [Resource].[ResourceId] where [Assignment].[ProjectId] = @project")
		AddParameterWithValue(da.SelectCommand, "project", projectId)
		da.Fill(dt)
		Return dt
	End Function

	Public Function GetAssignmentsPlanned(ByVal projectId As Integer) As DataTable
		Dim dt As New DataTable()
		Dim da = CreateDataAdapter("select * from [Assignment] left outer join [Resource] on [Assignment].[ResourceId] = [Resource].[ResourceId] where [AssignmentStatus] = 'planned' and [Assignment].[ProjectId] = @project order by [AssignmentOrdinal]")
		AddParameterWithValue(da.SelectCommand, "project", projectId)
		da.Fill(dt)
		Return dt
	End Function


	Public Function GetAssignmentsStarted(ByVal projectId As Integer) As DataTable
		Dim dt As New DataTable()
		Dim da = CreateDataAdapter("select * from [Assignment] left outer join [Resource] on [Assignment].[ResourceId] = [Resource].[ResourceId] where [AssignmentStatus] = 'started' and [Assignment].[ProjectId] = @project")
		AddParameterWithValue(da.SelectCommand, "project", projectId)
		da.Fill(dt)
		Return dt
	End Function

	Public Function GetAssignmentsFinished(ByVal projectId As Integer) As DataTable
		Dim dt As New DataTable()
		Dim da = CreateDataAdapter("select * from [Assignment] left outer join [Resource] on [Assignment].[ResourceId] = [Resource].[ResourceId] where [AssignmentStatus] = 'finished' and [Assignment].[ProjectId] = @project order by [AssignmentStart], [AssignmentEnd]")
		AddParameterWithValue(da.SelectCommand, "project", projectId)
		da.Fill(dt)
		Return dt
	End Function

	Public Function GetResources(ByVal projectId As Integer, ByVal group? As Integer) As DataTable
		If group Is Nothing Then
			Dim dt As New DataTable()
			Dim da = CreateDataAdapter("select * from [Resource] where [GroupId] is null and [ProjectId] = @project order by [ResourceName]")
			AddParameterWithValue(da.SelectCommand, "project", projectId)
			da.Fill(dt)
			Return dt
		Else
			Dim dt As New DataTable()
			Dim da = CreateDataAdapter("select * from [Resource] where [GroupId] = @group and [ProjectId] = @project order by [ResourceName]")
			AddParameterWithValue(da.SelectCommand, "project", projectId)
			AddParameterWithValue(da.SelectCommand, "group", group)
			da.Fill(dt)
			Return dt
		End If

	End Function

	Public Function GetResourcesAllWithNull(ByVal projectId As Integer) As DataTable
		Dim dt As New DataTable()
		Dim da = CreateDataAdapter("select * from [Resource] where [ProjectId] = @project order by [ResourceName]")
		AddParameterWithValue(da.SelectCommand, "project", projectId)
		da.Fill(dt)

		Dim dr As DataRow = dt.NewRow()
		dr("ResourceName") = "(none)"

		dt.Rows.InsertAt(dr, 0)

		Return dt
	End Function

	Public Function GetGroups(ByVal projectId As Integer) As DataTable
		Dim dt As New DataTable()
		Dim da = CreateDataAdapter("select * from [Group] where [ProjectId] = @project order by [GroupName]")
		AddParameterWithValue(da.SelectCommand, "project", projectId)
		da.Fill(dt)
		Return dt
	End Function

	Public Sub UpdateAssignment(ByVal id As Integer, ByVal note As String, ByVal resource As String, ByVal duration As Integer, ByVal spent? As Integer, ByVal start? As Date, ByVal [end]? As Date, ByVal status As String)
		Dim resolvedResource As Object = resource
		If String.IsNullOrEmpty(resource) Then
			resolvedResource = DBNull.Value
		End If

		Using con = CreateConnection()
			con.Open()

			Dim cmd = CreateCommand("update [Assignment] set [AssignmentNote] = @note, [AssignmentStart] = @start, [AssignmentEnd] = @end, [AssignmentDurationReal] = @spent, [ResourceId] = @resource, [AssignmentDuration] = @duration, [AssignmentStatus] = @status where [AssignmentId] = @id", con)
			AddParameterWithValue(cmd, "id", id)
			AddParameterWithValue(cmd, "note", note)
			AddParameterWithValue(cmd, "resource", resolvedResource)
			AddParameterWithValue(cmd, "spent", If(CObj(spent), DBNull.Value))
			AddParameterWithValue(cmd, "start", If(CObj(start), DBNull.Value))
			AddParameterWithValue(cmd, "end", If(CObj([end]), DBNull.Value))
			AddParameterWithValue(cmd, "duration", duration)
			AddParameterWithValue(cmd, "status", status)
			cmd.ExecuteNonQuery()
		End Using
	End Sub

	Public Sub DeleteAssignment(ByVal id As Integer)
		Using con = CreateConnection()
			con.Open()

			Dim cmd = CreateCommand("delete from [Assignment] where [AssignmentId] = @id", con)
			AddParameterWithValue(cmd, "id", id)
			cmd.ExecuteNonQuery()
		End Using
	End Sub

	Public Sub CreateResource(ByVal projectId As Integer, ByVal name As String, ByVal group As String)
		Dim resolvedGroup As Object = group
		If String.IsNullOrEmpty(group) Then
			resolvedGroup = DBNull.Value
		End If

		Using con As DbConnection = CreateConnection()
			con.Open()

			Dim cmd = CreateCommand("insert into [Resource] ([ResourceName], [GroupId], [ProjectId]) values (@name, @group, @project)", con)
			AddParameterWithValue(cmd, "name", name)
			AddParameterWithValue(cmd, "group", resolvedGroup)
			AddParameterWithValue(cmd, "project", projectId)
			cmd.ExecuteNonQuery()

		End Using

	End Sub

	Public Sub CreateGroup(ByVal projectId As Integer, ByVal name As String)
		Using con As DbConnection = CreateConnection()
			con.Open()

			Dim cmd = CreateCommand("insert into [Group] ([GroupName], [ProjectId]) values (@name, @project)", con)
			AddParameterWithValue(cmd, "name", name)
			AddParameterWithValue(cmd, "project", projectId)
			cmd.ExecuteNonQuery()

		End Using
	End Sub

	Public Sub CreateAssignment(ByVal projectId As Integer, ByVal duration As Integer, ByVal note As String)
		Dim zero As New Date(2000, 1, 1)
		Using con As DbConnection = CreateConnection()
			con.Open()

			Dim cmd = CreateCommand("select max([AssignmentOrdinal]) from [Assignment] where [ProjectId] = @project", con)
			AddParameterWithValue(cmd, "project", projectId)
			Dim ordinal As Integer = Binder.Get(cmd.ExecuteScalar()).Int32.GetValueOrDefault(0)
			ordinal += 1

			cmd = CreateCommand("insert into [Assignment] ([AssignmentDuration], [AssignmentNote], [AssignmentStart], [AssignmentEnd], [AssignmentStatus], [AssignmentOrdinal], [ProjectId]) values (@duration, @note, @start, @end, @status, @ordinal, @project)", con)
			AddParameterWithValue(cmd, "duration", duration)
			AddParameterWithValue(cmd, "note", note)
			AddParameterWithValue(cmd, "start", DBNull.Value)
			AddParameterWithValue(cmd, "end", DBNull.Value)
			AddParameterWithValue(cmd, "status", "planned")
			AddParameterWithValue(cmd, "ordinal", ordinal)
			AddParameterWithValue(cmd, "project", projectId)

			cmd.ExecuteNonQuery()

		End Using
	End Sub

	Public Sub UpdateAssignmentStartManual(ByVal id As Integer, ByVal start? As Date)
		Using con = CreateConnection()
			con.Open()

			Dim cmd = CreateCommand("update [Assignment] set [AssignmentStartManual] = @start where [AssignmentId] = @id", con)
			AddParameterWithValue(cmd, "id", id)
			AddParameterWithValue(cmd, "start", If(CObj(start), DBNull.Value))
			cmd.ExecuteNonQuery()
		End Using
	End Sub

	Private Sub UpdateOrdinal(ByVal con As DbConnection, ByVal projectId As Integer, ByVal id As String, ByVal ordinal As Integer)
		Dim cmd = CreateCommand("update [Assignment] set [AssignmentOrdinal] = @ordinal where [AssignmentId] = @id and [ProjectId] = @project", con)
		AddParameterWithValue(cmd, "id", id)
		AddParameterWithValue(cmd, "ordinal", ordinal)
		AddParameterWithValue(cmd, "project", projectId)
		cmd.ExecuteNonQuery()
	End Sub

	Public Sub UpdateOrder(ByVal projectId As Integer, ByVal order As String)
		Dim ids() As String = order.Split( {","c})
		Dim ordinal As Integer = 0

		Using con = CreateConnection()
			con.Open()
			For Each id In ids
				UpdateOrdinal(con, projectId, id, ordinal)
				ordinal += 1
			Next id
		End Using
	End Sub

	Public Sub CreateProject(ByVal name As String)
		Using con As DbConnection = CreateConnection()
			con.Open()

			Dim cmd = CreateCommand("insert into [Project] ([ProjectName]) values (@name)", con)
			AddParameterWithValue(cmd, "name", name)

			cmd.ExecuteNonQuery()

		End Using
	End Sub

	Public Function GetProjects() As DataTable
		Dim dt As New DataTable()
		Dim da = CreateDataAdapter("select * from [Project]")
		da.Fill(dt)
		Return dt
	End Function

	Public Function GetProject(ByVal id As String) As DataRow
		Dim da = CreateDataAdapter("select * from [Project] where [Project].[ProjectId] = @id")
		AddParameterWithValue(da.SelectCommand, "id", id)
		Dim dt As New DataTable()
		da.Fill(dt)
		If dt.Rows.Count = 1 Then
			Return dt.Rows(0)
		End If
		Return Nothing

	End Function

	Public Sub UpdateProject(ByVal projectId As Integer, ByVal name As String)
		Using con As DbConnection = CreateConnection()
			con.Open()
			Dim cmd = CreateCommand("update [Project] set [ProjectName] = @name where [ProjectId] = @id", con)
			AddParameterWithValue(cmd, "id", projectId)
			AddParameterWithValue(cmd, "name", name)
			cmd.ExecuteNonQuery()
		End Using
	End Sub

	Public Sub CreateUser(ByVal username As String, ByVal password As String, ByVal email As String)
		Using con As DbConnection = CreateConnection()
			con.Open()
			Dim cmd = CreateCommand("insert into [User] ([UserLogin], [UserEmail], [UserPassword]) values (@login, @email, @password)", con)
			AddParameterWithValue(cmd, "login", username)
			AddParameterWithValue(cmd, "password", password)
			AddParameterWithValue(cmd, "email", email)
			cmd.ExecuteNonQuery()
		End Using
	End Sub

	Public Function ValidateUser(ByVal username As String, ByVal password As String) As Boolean
		Dim da = CreateDataAdapter("select * from [User] where [UserEmail] = @username and [UserPassword] = @password")
		AddParameterWithValue(da.SelectCommand, "username", username)
		AddParameterWithValue(da.SelectCommand, "password", password)
		Dim dt As New DataTable()
		da.Fill(dt)
		If dt.Rows.Count = 1 Then
			Return True
		End If
		Return False
	End Function

	Public Function GetUserByEmail(ByVal email As String) As DataRow
		Dim da = CreateDataAdapter("select * from [User] where [UserEmail] = @username")
		AddParameterWithValue(da.SelectCommand, "username", email)
		Dim dt As New DataTable()
		da.Fill(dt)
		If dt.Rows.Count = 1 Then
			Return dt.Rows(0)
		End If
		Return Nothing
	End Function

	Public Function GetUserById(ByVal id As String) As DataRow
		Dim da = CreateDataAdapter("select * from [User] where [UserId] = @id")
		AddParameterWithValue(da.SelectCommand, "id", id)
		Dim dt As New DataTable()
		da.Fill(dt)
		If dt.Rows.Count = 1 Then
			Return dt.Rows(0)
		End If
		Return Nothing
	End Function

	Public Function GetUserConfig(ByVal userId As String, ByVal key As String) As String
		Dim da = CreateDataAdapter("select * from [UserConfig] where [UserId] = @id and [UserConfigKey] = @key order by [UserConfigId]")
		AddParameterWithValue(da.SelectCommand, "id", userId)
		AddParameterWithValue(da.SelectCommand, "key", key)
		Dim dt As New DataTable()
		da.Fill(dt)
		If dt.Rows.Count > 0 Then
			Dim result As Object = dt.Rows(dt.Rows.Count - 1)("UserConfigValue")
			If result Is DBNull.Value Then
				Return Nothing
			End If
			Return CStr(result)
		End If
		Return Nothing
	End Function

	Public Sub SetUserConfig(ByVal userId As String, ByVal key As String, ByVal value As String)
		' TODO transaction
		Dim old As String = GetUserConfig(userId, key)

		If old Is Nothing Then
			Using con As DbConnection = CreateConnection()
				con.Open()
				Dim cmd = CreateCommand("insert into [UserConfig] ([UserId], [UserConfigKey], [UserConfigValue]) values (@id, @key, @value)", con)
				AddParameterWithValue(cmd, "id", userId)
				AddParameterWithValue(cmd, "key", key)
				AddParameterWithValue(cmd, "value", If(CObj(value), DBNull.Value))
				cmd.ExecuteNonQuery()
			End Using

		Else
			Using con As DbConnection = CreateConnection()
				con.Open()
				Dim cmd = CreateCommand("update [UserConfig] set [UserId] = @id, [UserConfigKey] = @key, [UserConfigValue] = @value", con)
				AddParameterWithValue(cmd, "id", userId)
				AddParameterWithValue(cmd, "key", key)
				AddParameterWithValue(cmd, "value", value)
				cmd.ExecuteNonQuery()
			End Using
		End If

	End Sub
End Class