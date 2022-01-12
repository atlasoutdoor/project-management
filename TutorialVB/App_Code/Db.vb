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
Imports System.Configuration
Imports System.Data.Common
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Web
Imports System.Web.UI

	Public Class Db

		Public Shared Function ConnectionString() As String
			Dim mssql As Boolean = Not SqLiteFound()
			If mssql Then
				Return ConfigurationManager.ConnectionStrings("daypilot").ConnectionString
			End If

			If TryCast(HttpContext.Current.Session("cs"), String) Is Nothing Then
				HttpContext.Current.Session("cs") = GetNew()
			End If

			Return CStr(HttpContext.Current.Session("cs"))

		End Function

		Public Shared Function Factory() As DbProviderFactory
			Return DbProviderFactories.GetFactory(FactoryName())
		End Function

		Public Shared Function FactoryName() As String
			If SqLiteFound() Then
				Return "System.Data.SQLite"
			End If
			Return "System.Data.SqlClient"
		End Function

		Public Shared Function IdentityCommand() As String
			Select Case FactoryName()
				Case "System.Data.SQLite"
					Return "select last_insert_rowid();"
				Case "System.Data.SqlClient"
					Return "select @@identity;"
				Case Else
					Throw New NotSupportedException("Unsupported DB factory.")
			End Select
		End Function


		Private Shared Function GetNew() As String
			Dim today As String = Date.Today.ToString("yyyy-MM-dd")
			Dim guid As String = System.Guid.NewGuid().ToString()
			Dim dir As String = HttpContext.Current.Server.MapPath("~/App_Data/session/" & today & "/")
			Dim master As String = HttpContext.Current.Server.MapPath("~/App_Data/daypilot.sqlite")
			Dim path As String = dir & guid

			Directory.CreateDirectory(dir)
			File.Copy(master, path)

			Return String.Format("Data Source={0}", path)
		End Function

		Private Shared Function SqLiteFound() As Boolean
			Dim path As String = HttpContext.Current.Server.MapPath("~/bin/System.Data.SQLite.dll")
			Return File.Exists(path)
		End Function

	End Class
