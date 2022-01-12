/*
Copyright © 2013 Annpoint, s.r.o.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

-------------------------------------------------------------------------

NOTE: Reuse requires the following acknowledgement (see also NOTICE):
This product includes DayPilot (http://www.daypilot.org) developed by Annpoint, s.r.o.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using Util.Ui;

/// <summary>
/// Summary description for DataManager
/// </summary>
public class DataManager
{

    #region Helper methods
    private string ConnectionString
    {
        get { return Db.ConnectionString(); }
    }

    private DbProviderFactory Factory
    {
        get { return Db.Factory(); }
    }

    private DbConnection CreateConnection()
    {
        DbConnection connection = Factory.CreateConnection();
        connection.ConnectionString = ConnectionString;
        return connection;
    }

    private DbCommand CreateCommand(string text)
    {
        DbCommand command = Factory.CreateCommand();
        command.CommandText = text;
        command.Connection = CreateConnection();

        return command;
    }

    private DbCommand CreateCommand(string text, DbConnection connection)
    {
        DbCommand command = Factory.CreateCommand();
        command.CommandText = text;
        command.Connection = connection;

        return command;
    }

    private void AddParameterWithValue(DbCommand cmd, string name, object value)
    {
        var parameter = Factory.CreateParameter();
        parameter.Direction = ParameterDirection.Input;
        parameter.ParameterName = name;
        parameter.Value = value;
        cmd.Parameters.Add(parameter);
    }

    private int GetIdentity(DbConnection c)
    {
        var cmd = CreateCommand(Db.IdentityCommand(), c);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private DbDataAdapter CreateDataAdapter(string select)
    {
        DbDataAdapter da = Factory.CreateDataAdapter();
        da.SelectCommand = CreateCommand(select);
        return da;
    }

    #endregion


    public DataRow GetAssignment(int id)
    {
        var da = CreateDataAdapter("select * from [Assignment] where [Assignment].[AssignmentId] = @id");
        AddParameterWithValue(da.SelectCommand, "id", id);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count == 1)
        {
            return dt.Rows[0];
        }
        return null;
    }


    public DataTable GetAssignments(int projectId)
    {
        DataTable dt = new DataTable();
        var da = CreateDataAdapter("select * from [Assignment] left outer join [Resource] on [Assignment].[ResourceId] = [Resource].[ResourceId] where [Assignment].[ProjectId] = @project");
        AddParameterWithValue(da.SelectCommand, "project", projectId);
        da.Fill(dt);
        return dt;
    }

    public DataTable GetAssignmentsPlanned(int projectId)
    {
        DataTable dt = new DataTable();
        var da = CreateDataAdapter("select * from [Assignment] left outer join [Resource] on [Assignment].[ResourceId] = [Resource].[ResourceId] where [AssignmentStatus] = 'planned' and [Assignment].[ProjectId] = @project order by [AssignmentOrdinal]");
        AddParameterWithValue(da.SelectCommand, "project", projectId);
        da.Fill(dt);
        return dt;
    }


    public DataTable GetAssignmentsStarted(int projectId)
    {
        DataTable dt = new DataTable();
        var da = CreateDataAdapter("select * from [Assignment] left outer join [Resource] on [Assignment].[ResourceId] = [Resource].[ResourceId] where [AssignmentStatus] = 'started' and [Assignment].[ProjectId] = @project");
        AddParameterWithValue(da.SelectCommand, "project", projectId);
        da.Fill(dt);
        return dt;
    }

    public DataTable GetAssignmentsFinished(int projectId)
    {
        DataTable dt = new DataTable();
        var da = CreateDataAdapter("select * from [Assignment] left outer join [Resource] on [Assignment].[ResourceId] = [Resource].[ResourceId] where [AssignmentStatus] = 'finished' and [Assignment].[ProjectId] = @project order by [AssignmentStart], [AssignmentEnd]");
        AddParameterWithValue(da.SelectCommand, "project", projectId);
        da.Fill(dt);
        return dt;
    }

    public DataTable GetResources(int projectId, int? group)
    {
        if (group == null)
        {
            DataTable dt = new DataTable();
            var da = CreateDataAdapter("select * from [Resource] where [GroupId] is null and [ProjectId] = @project order by [ResourceName]");
            AddParameterWithValue(da.SelectCommand, "project", projectId);
            da.Fill(dt);
            return dt;
        }
        else
        {
            DataTable dt = new DataTable();
            var da = CreateDataAdapter("select * from [Resource] where [GroupId] = @group and [ProjectId] = @project order by [ResourceName]");
            AddParameterWithValue(da.SelectCommand, "project", projectId);
            AddParameterWithValue(da.SelectCommand, "group", group);
            da.Fill(dt);
            return dt;
        }

    }

    public DataTable GetResourcesAllWithNull(int projectId)
    {
        DataTable dt = new DataTable();
        var da = CreateDataAdapter("select * from [Resource] where [ProjectId] = @project order by [ResourceName]");
        AddParameterWithValue(da.SelectCommand, "project", projectId);
        da.Fill(dt);

        DataRow dr = dt.NewRow();
        dr["ResourceName"] = "(none)";

        dt.Rows.InsertAt(dr, 0);

        return dt;
    }

    public DataTable GetGroups(int projectId)
    {
        DataTable dt = new DataTable();
        var da = CreateDataAdapter("select * from [Group] where [ProjectId] = @project order by [GroupName]");
        AddParameterWithValue(da.SelectCommand, "project", projectId);
        da.Fill(dt);
        return dt;
    }

    public void UpdateAssignment(int id, string note, string resource, int duration, int? spent, DateTime? start, DateTime? end, string status)
    {
        object resolvedResource = resource;
        if (String.IsNullOrEmpty(resource))
        {
            resolvedResource = DBNull.Value;
        }

        using (var con = CreateConnection())
        {
            con.Open();

            var cmd = CreateCommand("update [Assignment] set [AssignmentNote] = @note, [AssignmentStart] = @start, [AssignmentEnd] = @end, [AssignmentDurationReal] = @spent, [ResourceId] = @resource, [AssignmentDuration] = @duration, [AssignmentStatus] = @status where [AssignmentId] = @id", con);
            AddParameterWithValue(cmd, "id", id);
            AddParameterWithValue(cmd, "note", note);
            AddParameterWithValue(cmd, "resource", resolvedResource);
            AddParameterWithValue(cmd, "spent", (object) spent ?? DBNull.Value);
            AddParameterWithValue(cmd, "start", (object) start ?? DBNull.Value);
            AddParameterWithValue(cmd, "end", (object) end ?? DBNull.Value);
            AddParameterWithValue(cmd, "duration", duration);
            AddParameterWithValue(cmd, "status", status);
            cmd.ExecuteNonQuery();
        }
    }

    public void DeleteAssignment(int id)
    {
        using (var con = CreateConnection())
        {
            con.Open();

            var cmd = CreateCommand("delete from [Assignment] where [AssignmentId] = @id", con);
            AddParameterWithValue(cmd, "id", id);
            cmd.ExecuteNonQuery();
        }
    }

    public void CreateResource(int projectId, string name, string group)
    {
        object resolvedGroup = group;
        if (String.IsNullOrEmpty(group))
        {
            resolvedGroup = DBNull.Value;
        }

        using (DbConnection con = CreateConnection())
        {
            con.Open();

            var cmd = CreateCommand("insert into [Resource] ([ResourceName], [GroupId], [ProjectId]) values (@name, @group, @project)", con);
            AddParameterWithValue(cmd, "name", name);
            AddParameterWithValue(cmd, "group", resolvedGroup);
            AddParameterWithValue(cmd, "project", projectId);
            cmd.ExecuteNonQuery();

        }

    }

    public void CreateGroup(int projectId, string name)
    {
        using (DbConnection con = CreateConnection())
        {
            con.Open();

            var cmd = CreateCommand("insert into [Group] ([GroupName], [ProjectId]) values (@name, @project)", con);
            AddParameterWithValue(cmd, "name", name);
            AddParameterWithValue(cmd, "project", projectId);
            cmd.ExecuteNonQuery();

        }
    }

    public void CreateAssignment(int projectId, int duration, string note)
    {
        DateTime zero = new DateTime(2000, 1, 1);
        using (DbConnection con = CreateConnection())
        {
            con.Open();

            var cmd = CreateCommand("select max([AssignmentOrdinal]) from [Assignment] where [ProjectId] = @project", con);
            AddParameterWithValue(cmd, "project", projectId);
            int ordinal = Binder.Get(cmd.ExecuteScalar()).Int32.GetValueOrDefault(0);
            ordinal += 1;

            cmd = CreateCommand("insert into [Assignment] ([AssignmentDuration], [AssignmentNote], [AssignmentStart], [AssignmentEnd], [AssignmentStatus], [AssignmentOrdinal], [ProjectId]) values (@duration, @note, @start, @end, @status, @ordinal, @project)", con);
            AddParameterWithValue(cmd, "duration", duration);
            AddParameterWithValue(cmd, "note", note);
            AddParameterWithValue(cmd, "start", DBNull.Value);
            AddParameterWithValue(cmd, "end", DBNull.Value);
            AddParameterWithValue(cmd, "status", "planned");
            AddParameterWithValue(cmd, "ordinal", ordinal);
            AddParameterWithValue(cmd, "project", projectId);

            cmd.ExecuteNonQuery();

        }
    }

    public void UpdateAssignmentStartManual(int id, DateTime? start)
    {
        using (var con = CreateConnection())
        {
            con.Open();

            var cmd = CreateCommand("update [Assignment] set [AssignmentStartManual] = @start where [AssignmentId] = @id", con);
            AddParameterWithValue(cmd, "id", id);
            AddParameterWithValue(cmd, "start", (object)start ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }
    }

    private void UpdateOrdinal(DbConnection con, int projectId, string id, int ordinal)
    {
        var cmd = CreateCommand("update [Assignment] set [AssignmentOrdinal] = @ordinal where [AssignmentId] = @id and [ProjectId] = @project", con);
        AddParameterWithValue(cmd, "id", id);
        AddParameterWithValue(cmd, "ordinal", ordinal);
        AddParameterWithValue(cmd, "project", projectId);
        cmd.ExecuteNonQuery();
    }

    public void UpdateOrder(int projectId, string order)
    {
        string[] ids = order.Split(new[] {','});
        int ordinal = 0;

        using (var con = CreateConnection())
        {
            con.Open();
            foreach (var id in ids)
            {
                UpdateOrdinal(con, projectId, id, ordinal);
                ordinal += 1;
            }
        }
    }

    public void CreateProject(string name)
    {
        using (DbConnection con = CreateConnection())
        {
            con.Open();

            var cmd = CreateCommand("insert into [Project] ([ProjectName]) values (@name)", con);
            AddParameterWithValue(cmd, "name", name);

            cmd.ExecuteNonQuery();

        }
    }

    public DataTable GetProjects()
    {
        DataTable dt = new DataTable();
        var da = CreateDataAdapter("select * from [Project]");
        da.Fill(dt);
        return dt;       
    }

    public DataRow GetProject(string id)
    {
        var da = CreateDataAdapter("select * from [Project] where [Project].[ProjectId] = @id");
        AddParameterWithValue(da.SelectCommand, "id", id);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count == 1)
        {
            return dt.Rows[0];
        }
        return null;
       
    }

    public void UpdateProject(int projectId, string name)
    {
        using (DbConnection con = CreateConnection())
        {
            con.Open();
            var cmd = CreateCommand("update [Project] set [ProjectName] = @name where [ProjectId] = @id", con);
            AddParameterWithValue(cmd, "id", projectId);
            AddParameterWithValue(cmd, "name", name);
            cmd.ExecuteNonQuery();
        }
    }

    public void CreateUser(string username, string password, string email)
    {
        using (DbConnection con = CreateConnection())
        {
            con.Open();
            var cmd = CreateCommand("insert into [User] ([UserLogin], [UserEmail], [UserPassword]) values (@login, @email, @password)", con);
            AddParameterWithValue(cmd, "login", username);
            AddParameterWithValue(cmd, "password", password);
            AddParameterWithValue(cmd, "email", email);
            cmd.ExecuteNonQuery();
        }
    }

    public bool ValidateUser(string username, string password)
    {
        var da = CreateDataAdapter("select * from [User] where [UserEmail] = @username and [UserPassword] = @password");
        AddParameterWithValue(da.SelectCommand, "username", username);
        AddParameterWithValue(da.SelectCommand, "password", password);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count == 1)
        {
            return true;
        }
        return false;
    }

    public DataRow GetUserByEmail(string email)
    {
        var da = CreateDataAdapter("select * from [User] where [UserEmail] = @username");
        AddParameterWithValue(da.SelectCommand, "username", email);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count == 1)
        {
            return dt.Rows[0];
        }
        return null;
    }

    public DataRow GetUserById(string id)
    {
        var da = CreateDataAdapter("select * from [User] where [UserId] = @id");
        AddParameterWithValue(da.SelectCommand, "id", id);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count == 1)
        {
            return dt.Rows[0];
        }
        return null;
    }

    public string GetUserConfig(string userId, string key)
    {
        var da = CreateDataAdapter("select * from [UserConfig] where [UserId] = @id and [UserConfigKey] = @key order by [UserConfigId]");
        AddParameterWithValue(da.SelectCommand, "id", userId);
        AddParameterWithValue(da.SelectCommand, "key", key);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            object result = dt.Rows[dt.Rows.Count - 1]["UserConfigValue"];
            if (result == DBNull.Value)
            {
                return null;
            }
            return (string) result;
        }
        return null;
    }

    public void SetUserConfig(string userId, string key, string value)
    {
        // TODO transaction
        string old = GetUserConfig(userId, key);

        if (old == null)
        {
            using (DbConnection con = CreateConnection())
            {
                con.Open();
                var cmd = CreateCommand("insert into [UserConfig] ([UserId], [UserConfigKey], [UserConfigValue]) values (@id, @key, @value)", con);
                AddParameterWithValue(cmd, "id", userId);
                AddParameterWithValue(cmd, "key", key);
                AddParameterWithValue(cmd, "value", (object) value ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
            
        }
        else
        {
            using (DbConnection con = CreateConnection())
            {
                con.Open();
                var cmd = CreateCommand("update [UserConfig] set [UserId] = @id, [UserConfigKey] = @key, [UserConfigValue] = @value", con);
                AddParameterWithValue(cmd, "id", userId);
                AddParameterWithValue(cmd, "key", key);
                AddParameterWithValue(cmd, "value", value);
                cmd.ExecuteNonQuery();
            }
        }

    }
}