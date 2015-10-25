﻿/*
Copyright (c) 2012 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

#region Usings
using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Company.Utilities.DataTypes.ExtensionMethods;
using Company.Utilities.SQL.DataClasses;
using Company.Utilities.SQL.DataClasses.Enums;
using Company.Utilities.SQL.DataClasses.Interfaces;
using Company.Utilities.SQL.MicroORM.Interfaces;
using System.Globalization;
#endregion

namespace Company.Utilities.SQL.SQLServer
{
    /// <summary>
    /// Functions helpful for SQL Server
    /// </summary>
    public static class SQLServer
    {
        #region Public Static Functions

        /// <summary>
        /// Checks if a database exists
        /// </summary>
        /// <param name="Database">Name of the database</param>
        /// <param name="ConnectionString">Connection string</param>
        /// <returns>True if it exists, false otherwise</returns>
        public static bool DoesDatabaseExist(string Database, string ConnectionString)
        {
            return CheckExists("SELECT * FROM Master.sys.Databases WHERE name=@Name", Database, ConnectionString);
        }

        /// <summary>
        /// Checks if a table exists
        /// </summary>
        /// <param name="Table">Table name</param>
        /// <param name="ConnectionString">Connection string</param>
        /// <returns>True if it exists, false otherwise</returns>
        public static bool DoesTableExist(string Table, string ConnectionString)
        {
            return CheckExists("SELECT * FROM sys.Tables WHERE name=@Name", Table, ConnectionString);
        }

        /// <summary>
        /// Checks if a view exists
        /// </summary>
        /// <param name="View">View name</param>
        /// <param name="ConnectionString">Connection string</param>
        /// <returns>True if it exists, false otherwise</returns>
        public static bool DoesViewExist(string View, string ConnectionString)
        {
            return CheckExists("SELECT * FROM sys.views WHERE name=@Name", View, ConnectionString);
        }

        /// <summary>
        /// Checks if stored procedure exists
        /// </summary>
        /// <param name="StoredProcedure">Stored procedure's name</param>
        /// <param name="ConnectionString">Connection string</param>
        /// <returns>True if it exists, false otherwise</returns>
        public static bool DoesStoredProcedureExist(string StoredProcedure, string ConnectionString)
        {
            return CheckExists("SELECT * FROM sys.Procedures WHERE name=@Name", StoredProcedure, ConnectionString);
        }

        /// <summary>
        /// Checks if trigger exists
        /// </summary>
        /// <param name="Trigger">Trigger's name</param>
        /// <param name="ConnectionString">Connection string</param>
        /// <returns>True if it exists, false otherwise</returns>
        public static bool DoesTriggerExist(string Trigger, string ConnectionString)
        {
            return CheckExists("SELECT * FROM sys.triggers WHERE name=@Name", Trigger, ConnectionString);
        }

        /// <summary>
        /// Creates a database out of the structure it is given
        /// </summary>
        /// <param name="Database">Database structure</param>
        /// <param name="ConnectionString">The connection string to the database's location</param>
        public static void CreateDatabase(Database Database, string ConnectionString)
        {
            string Command = BuildCommands(Database);
            string[] Splitter = { "\n" };
            string[] Commands = Command.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
            ConnectionString = Regex.Replace(ConnectionString, "Pooling=(.*?;)", "", RegexOptions.IgnoreCase) + ";Pooling=false;";
            string DatabaseConnectionString = Regex.Replace(ConnectionString, "Initial Catalog=(.*?;)", "", RegexOptions.IgnoreCase);
            using (SQLHelper Helper = new SQLHelper(Commands[0], DatabaseConnectionString, CommandType.Text))
            {
                Helper.ExecuteNonQuery();
            }
            using (SQLHelper Helper = new SQLHelper("", ConnectionString, CommandType.Text))
            {
                IBatchCommand Batcher = Helper.Batch();
                for (int x = 1; x < Commands.Length; ++x)
                {
                    if (Commands[x].Contains("CREATE TRIGGER") || Commands[x].Contains("CREATE FUNCTION"))
                    {
                        if (Batcher.CommandCount > 0)
                        {
                            Helper.ExecuteNonQuery();
                            Batcher = Helper.Batch();
                        }
                        Batcher.AddCommand(Commands[x], CommandType.Text);
                        if (x < Commands.Length - 1)
                        {
                            Helper.ExecuteNonQuery();
                            Batcher = Helper.Batch();
                        }
                    }
                    else
                    {
                        Batcher.AddCommand(Commands[x], CommandType.Text);
                    }
                }
                Helper.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates a database (only adds new fields, tables, etc. does not delete old fields)
        /// </summary>
        /// <param name="DesiredDatabase">The desired structure of the database</param>
        /// <param name="CurrentDatabase">The current database structure</param>
        /// <param name="ConnectionString">Connection string to the database</param>
        public static void UpdateDatabase(Database DesiredDatabase, Database CurrentDatabase, string ConnectionString)
        {
            if (CurrentDatabase == null)
            {
                CreateDatabase(DesiredDatabase, ConnectionString);
                return;
            }
            string Command = BuildCommands(DesiredDatabase, CurrentDatabase);
            string[] Splitter = { "\n" };
            string[] Commands = Command.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
            ConnectionString = Regex.Replace(ConnectionString, "Pooling=(.*?;)", "", RegexOptions.IgnoreCase) + ";Pooling=false;";
            using (SQLHelper Helper = new SQLHelper("", ConnectionString, CommandType.Text))
            {
                IBatchCommand Batcher = Helper.Batch();
                for (int x = 0; x < Commands.Length; ++x)
                {
                    if (Commands[x].Contains("CREATE TRIGGER") || Commands[x].Contains("CREATE FUNCTION"))
                    {
                        if (Batcher.CommandCount > 0)
                        {
                            Helper.ExecuteNonQuery();
                            Batcher = Helper.Batch();
                        }
                        Batcher.AddCommand(Commands[x], CommandType.Text);
                        if (x < Commands.Length - 1)
                        {
                            Helper.ExecuteNonQuery();
                            Batcher = Helper.Batch();
                        }
                    }
                    else
                    {
                        Batcher.AddCommand(Commands[x], CommandType.Text);
                    }
                }
                Helper.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets the structure of a database
        /// </summary>
        /// <param name="ConnectionString">Connection string</param>
        /// <returns>The database structure</returns>
        public static Database GetDatabaseStructure(string ConnectionString)
        {
            string DatabaseName = Regex.Match(ConnectionString, "Initial Catalog=(.*?;)").Value.Replace("Initial Catalog=", "").Replace(";", "");
            if (!DoesDatabaseExist(DatabaseName, ConnectionString))
                return null;
            Database Temp = new Database(DatabaseName);
            GetTables(ConnectionString, Temp);
            SetupTables(ConnectionString, Temp);
            SetupViews(ConnectionString, Temp);
            SetupStoredProcedures(ConnectionString, Temp);
            SetupFunctions(ConnectionString, Temp);
            return Temp;
        }

        #endregion

        #region Private Static Functions

        /// <summary>
        /// Builds a list of commands for a datatbase
        /// </summary>
        /// <param name="DesiredDatabase">Desired database structure</param>
        /// <param name="CurrentDatabase">Current database structure</param>
        /// <returns>A list of commands</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static string BuildCommands(Database DesiredDatabase, Database CurrentDatabase)
        {
            StringBuilder Builder = new StringBuilder();

            foreach (Table Table in DesiredDatabase.Tables)
            {
                Table CurrentTable = CurrentDatabase[Table.Name];
                if (CurrentTable == null)
                {
                    Builder.Append(GetTableCommand(Table));
                }
                else
                {
                    Builder.Append(GetAlterTableCommand(Table, CurrentTable));
                }
            }
            foreach (Table Table in DesiredDatabase.Tables)
            {
                Table CurrentTable = CurrentDatabase[Table.Name];
                if (CurrentTable == null)
                {
                    Builder.Append(GetForeignKeyCommand(Table));
                }
            }
            foreach (Table Table in DesiredDatabase.Tables)
            {
                Table CurrentTable = CurrentDatabase[Table.Name];
                if (CurrentTable == null)
                {
                    Builder.Append(GetTriggerCommand(Table));
                }
                else
                {
                    Builder.Append(GetAlterTriggerCommand(Table, CurrentTable));
                }
            }
            foreach (Function Function in DesiredDatabase.Functions)
            {
                bool Found = false;
                foreach (Function CurrentFunction in CurrentDatabase.Functions)
                {
                    if (CurrentFunction.Name == Function.Name)
                    {
                        Builder.Append(GetAlterFunctionCommand(Function, CurrentFunction));
                        Found = true;
                        break;
                    }
                }
                if (!Found)
                {
                    Builder.Append(GetFunctionCommand(Function));
                }
            }
            foreach (View View in DesiredDatabase.Views)
            {
                bool Found = false;
                foreach (View CurrentView in CurrentDatabase.Views)
                {
                    if (CurrentView.Name == View.Name)
                    {
                        Builder.Append(GetAlterViewCommand(View, CurrentView));
                        Found = true;
                        break;
                    }
                }
                if (!Found)
                {
                    Builder.Append(GetViewCommand(View));
                }
            }
            foreach (StoredProcedure StoredProcedure in DesiredDatabase.StoredProcedures)
            {
                bool Found = false;
                foreach (StoredProcedure CurrentStoredProcedure in CurrentDatabase.StoredProcedures)
                {
                    if (StoredProcedure.Name == CurrentStoredProcedure.Name)
                    {
                        Builder.Append(GetAlterStoredProcedure(StoredProcedure, CurrentStoredProcedure));
                        Found = true;
                        break;
                    }
                }
                if (!Found)
                {
                    Builder.Append(GetStoredProcedure(StoredProcedure));
                }
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Gets a list of alter commands for a stored procedure
        /// </summary>
        /// <param name="StoredProcedure">Desired stored procedure</param>
        /// <param name="CurrentStoredProcedure">Current stored procedure</param>
        /// <returns>A list of commands in a string</returns>
        private static string GetAlterStoredProcedure(StoredProcedure StoredProcedure, StoredProcedure CurrentStoredProcedure)
        {
            StringBuilder Builder = new StringBuilder();
            if (StoredProcedure.Definition != CurrentStoredProcedure.Definition)
            {
                Builder.Append("EXEC dbo.sp_executesql @statement = N'DROP PROCEDURE ").Append(StoredProcedure.Name).Append("'\n");
                Builder.Append(GetStoredProcedure(StoredProcedure));
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Gets a list of alter commands for a view
        /// </summary>
        /// <param name="View">Desired view structure</param>
        /// <param name="CurrentView">Current view structure</param>
        /// <returns>A list of commands in a string</returns>
        private static string GetAlterViewCommand(View View, View CurrentView)
        {
            StringBuilder Builder = new StringBuilder();
            if (View.Definition != CurrentView.Definition)
            {
                Builder.Append("EXEC dbo.sp_executesql @statement = N'DROP VIEW ").Append(View.Name).Append("'\n");
                Builder.Append(GetViewCommand(View));
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Gets a list of alter commands for a function
        /// </summary>
        /// <param name="Function">Desired function structure</param>
        /// <param name="CurrentFunction">Current function structure</param>
        /// <returns>A list of commands in a string</returns>
        private static string GetAlterFunctionCommand(Function Function, Function CurrentFunction)
        {
            StringBuilder Builder = new StringBuilder();
            if (Function.Definition != CurrentFunction.Definition)
            {
                Builder.Append("EXEC dbo.sp_executesql @statement = N'DROP FUNCTION ").Append(Function.Name).Append("'\n");
                Builder.Append(GetFunctionCommand(Function));
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Gets a list of commands altering triggers
        /// </summary>
        /// <param name="Table">Desired table containing the triggers</param>
        /// <param name="CurrentTable">Current table containing the current triggers</param>
        /// <returns>A string containing a list of commands</returns>
        private static string GetAlterTriggerCommand(Table Table, Table CurrentTable)
        {
            StringBuilder Builder = new StringBuilder();
            foreach (Trigger Trigger in Table.Triggers)
            {
                foreach (Trigger Trigger2 in CurrentTable.Triggers)
                {
                    if (Trigger.Name == Trigger2.Name && Trigger.Definition != Trigger2.Definition)
                    {
                        Builder.Append("EXEC dbo.sp_executesql @statement = N'DROP TRIGGER ").Append(Trigger.Name).Append("'\n");
                        string Definition = Regex.Replace(Trigger.Definition, "-- (.*)", "");
                        Builder.Append(Definition.Replace("\n", " ").Replace("\r", " ") + "\n");
                        break;
                    }
                }
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Gets alter commands for a table
        /// </summary>
        /// <param name="DesiredTable">Desired table structure</param>
        /// <param name="CurrentTable">Current table structure</param>
        /// <returns>A string containing a list of commands</returns>
        private static string GetAlterTableCommand(Table DesiredTable, Table CurrentTable)
        {
            StringBuilder Builder = new StringBuilder();
            foreach (IColumn Column in DesiredTable.Columns)
            {
                IColumn CurrentColumn = CurrentTable[Column.Name];
                if (CurrentColumn == null)
                {
                    Builder.Append("EXEC dbo.sp_executesql @statement = N'ALTER TABLE ").Append(DesiredTable.Name)
                        .Append(" ADD ").Append(Column.Name).Append(" ").Append(Column.DataType.ToSQLDbType().ToString());
                    if (Column.DataType == SqlDbType.VarChar.ToDbType() || Column.DataType == SqlDbType.NVarChar.ToDbType())
                    {
                        if (Column.Length < 0 || Column.Length >= 4000)
                        {
                            Builder.Append("(MAX)");
                        }
                        else
                        {
                            Builder.Append("(").Append(Column.Length.ToString(CultureInfo.InvariantCulture)).Append(")");
                        }
                    }
                    Builder.Append("'\n");
                    foreach (IColumn ForeignKey in Column.ForeignKey)
                    {
                        Builder.Append("EXEC dbo.sp_executesql @statement = N'ALTER TABLE ").Append(DesiredTable.Name)
                            .Append(" ADD FOREIGN KEY (").Append(Column.Name).Append(") REFERENCES ")
                            .Append(ForeignKey.ParentTable.Name).Append("(").Append(ForeignKey.Name).Append(")");
                        if (Column.OnDeleteCascade)
                            Builder.Append(" ON DELETE CASCADE");
                        if (Column.OnUpdateCascade)
                            Builder.Append(" ON UPDATE CASCADE");
                        if (Column.OnDeleteSetNull)
                            Builder.Append(" ON DELETE SET NULL");
                        Builder.Append("'\n");
                    }
                }
                else if (CurrentColumn.DataType != Column.DataType
                    || (CurrentColumn.DataType == Column.DataType
                        && CurrentColumn.DataType == SqlDbType.NVarChar.ToDbType()
                        && CurrentColumn.Length != Column.Length * 2
                        && (CurrentColumn.Length != -1 && Column.Length != -1)))
                {
                    Builder.Append("EXEC dbo.sp_executesql @statement = N'ALTER TABLE ").Append(DesiredTable.Name)
                        .Append(" ALTER COLUMN ").Append(Column.Name).Append(" ").Append(Column.DataType.ToSQLDbType().ToString());
                    if (Column.DataType == SqlDbType.VarChar.ToDbType() || Column.DataType == SqlDbType.NVarChar.ToDbType())
                    {
                        if (Column.Length < 0 || Column.Length >= 4000)
                        {
                            Builder.Append("(MAX)");
                        }
                        else
                        {
                            Builder.Append("(").Append(Column.Length.ToString(CultureInfo.InvariantCulture)).Append(")");
                        }
                    }
                    Builder.Append("'\n");
                }
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Builds the list of commands to build the database
        /// </summary>
        /// <param name="Database">Database object</param>
        /// <returns>The commands needed to  build the database</returns>
        private static string BuildCommands(Database Database)
        {
            StringBuilder Builder = new StringBuilder();
            Builder.Append("EXEC dbo.sp_executesql @statement = N'CREATE DATABASE ").Append(Database.Name).Append("'\n");
            foreach (Table Table in Database.Tables)
            {
                Builder.Append(GetTableCommand(Table));
            }
            foreach (Table Table in Database.Tables)
            {
                Builder.Append(GetForeignKeyCommand(Table));
            }
            foreach (Table Table in Database.Tables)
            {
                Builder.Append(GetTriggerCommand(Table));
            }
            foreach (Function Function in Database.Functions)
            {
                Builder.Append(GetFunctionCommand(Function));
            }
            foreach (View View in Database.Views)
            {
                Builder.Append(GetViewCommand(View));
            }
            foreach (StoredProcedure StoredProcedure in Database.StoredProcedures)
            {
                Builder.Append(GetStoredProcedure(StoredProcedure));
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Gets the list of triggers associated with the table
        /// </summary>
        /// <param name="Table">Table object</param>
        /// <returns>The string containing the various creation commands</returns>
        private static string GetTriggerCommand(Table Table)
        {
            StringBuilder Builder = new StringBuilder();
            foreach (Trigger Trigger in Table.Triggers)
            {
                string Definition = Regex.Replace(Trigger.Definition, "-- (.*)", "");
                Builder.Append(Definition.Replace("\n", " ").Replace("\r", " ") + "\n");
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Gets the foreign keys creation command
        /// </summary>
        /// <param name="Table">Table object</param>
        /// <returns>The string creating the foreign keys</returns>
        private static string GetForeignKeyCommand(Table Table)
        {
            StringBuilder Builder = new StringBuilder();
            foreach (IColumn Column in Table.Columns)
            {
                if (Column.ForeignKey.Count > 0)
                {
                    foreach (IColumn ForeignKey in Column.ForeignKey)
                    {
                        Builder.Append("EXEC dbo.sp_executesql @statement = N'ALTER TABLE ");
                        Builder.Append(Column.ParentTable.Name).Append(" ADD FOREIGN KEY (");
                        Builder.Append(Column.Name).Append(") REFERENCES ").Append(ForeignKey.ParentTable.Name);
                        Builder.Append("(").Append(ForeignKey.Name).Append(")");
                        if (Column.OnDeleteCascade)
                            Builder.Append(" ON DELETE CASCADE");
                        if (Column.OnUpdateCascade)
                            Builder.Append(" ON UPDATE CASCADE");
                        if (Column.OnDeleteSetNull)
                            Builder.Append(" ON DELETE SET NULL");
                        Builder.Append("'\n");
                    }
                }
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Gets the stored procedure creation command
        /// </summary>
        /// <param name="StoredProcedure">The stored procedure object</param>
        /// <returns>The string creating the stored procedure</returns>
        private static string GetStoredProcedure(StoredProcedure StoredProcedure)
        {
            string Definition = Regex.Replace(StoredProcedure.Definition, "-- (.*)", "");
            return Definition.Replace("\n", " ").Replace("\r", " ") + "\n";
        }

        /// <summary>
        /// Gets the view creation command
        /// </summary>
        /// <param name="View">The view object</param>
        /// <returns>The string creating the view</returns>
        private static string GetViewCommand(View View)
        {
            string Definition = Regex.Replace(View.Definition, "-- (.*)", "");
            return Definition.Replace("\n", " ").Replace("\r", " ") + "\n";
        }

        /// <summary>
        /// Gets the function command
        /// </summary>
        /// <param name="Function">The function object</param>
        /// <returns>The string creating the function</returns>
        private static string GetFunctionCommand(Function Function)
        {
            string Definition = Regex.Replace(Function.Definition, "-- (.*)", "");
            return Definition.Replace("\n", " ").Replace("\r", " ") + "\n";
        }

        /// <summary>
        /// Gets the table creation commands
        /// </summary>
        /// <param name="Table">Table object</param>
        /// <returns>The string containing the creation commands</returns>
        private static string GetTableCommand(Table Table)
        {
            StringBuilder Builder = new StringBuilder();
            Builder.Append("EXEC dbo.sp_executesql @statement = N'CREATE TABLE ").Append(Table.Name).Append("(");
            string Splitter = "";
            foreach (IColumn Column in Table.Columns)
            {
                Builder.Append(Splitter).Append(Column.Name).Append(" ").Append(Column.DataType.ToSQLDbType().ToString());
                if (Column.DataType == SqlDbType.VarChar.ToDbType() || Column.DataType == SqlDbType.NVarChar.ToDbType())
                {
                    if (Column.Length < 0 || Column.Length >= 4000)
                    {
                        Builder.Append("(MAX)");
                    }
                    else
                    {
                        Builder.Append("(").Append(Column.Length.ToString(CultureInfo.InvariantCulture)).Append(")");
                    }
                }
                if (!Column.Nullable)
                {
                    Builder.Append(" NOT NULL");
                }
                if (Column.Unique)
                {
                    Builder.Append(" UNIQUE");
                }
                if (Column.PrimaryKey)
                {
                    Builder.Append(" PRIMARY KEY");
                }
                if (!string.IsNullOrEmpty(Column.Default))
                {
                    Builder.Append(" DEFAULT ").Append(Column.Default.Replace("(", "").Replace(")", "").Replace("'", "''"));
                }
                if (Column.AutoIncrement)
                {
                    Builder.Append(" IDENTITY");
                }
                Splitter = ",";
            }
            Builder.Append(")'\n");
            int Counter = 0;
            foreach (IColumn Column in Table.Columns)
            {
                if (Column.Index && Column.Unique)
                {
                    Builder.Append("EXEC dbo.sp_executesql @statement = N'CREATE UNIQUE INDEX ");
                    Builder.Append("Index_").Append(Column.Name).Append(Counter.ToString(CultureInfo.InvariantCulture)).Append(" ON ");
                    Builder.Append(Column.ParentTable.Name).Append("(").Append(Column.Name).Append(")");
                    Builder.Append("'\n");
                }
                else if (Column.Index)
                {
                    Builder.Append("EXEC dbo.sp_executesql @statement = N'CREATE INDEX ");
                    Builder.Append("Index_").Append(Column.Name).Append(Counter.ToString(CultureInfo.InvariantCulture)).Append(" ON ");
                    Builder.Append(Column.ParentTable.Name).Append("(").Append(Column.Name).Append(")");
                    Builder.Append("'\n");
                }
                ++Counter;
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Sets up the functions
        /// </summary>
        /// <param name="ConnectionString">Connection string</param>
        /// <param name="Temp">Database object</param>
        private static void SetupFunctions(string ConnectionString, Database Temp)
        {
            string Command = "SELECT SPECIFIC_NAME as NAME,ROUTINE_DEFINITION as DEFINITION FROM INFORMATION_SCHEMA.ROUTINES WHERE INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE='FUNCTION'";
            using (SQLHelper Helper = new SQLHelper(Command, ConnectionString, CommandType.Text))
            {
                Helper.ExecuteReader();
                while (Helper.Read())
                {
                    string Name = (string)Helper.GetParameter("NAME", "");
                    string Definition = (string)Helper.GetParameter("DEFINITION", "");
                    Temp.AddFunction(Name, Definition);
                }
            }
        }

        /// <summary>
        /// Sets up stored procedures
        /// </summary>
        /// <param name="ConnectionString">Connection string</param>
        /// <param name="Temp">Database object</param>
        private static void SetupStoredProcedures(string ConnectionString, Database Temp)
        {
            string Command = "SELECT sys.procedures.name as NAME,OBJECT_DEFINITION(sys.procedures.object_id) as DEFINITION FROM sys.procedures";
            using (SQLHelper Helper = new SQLHelper(Command, ConnectionString, CommandType.Text))
            {
                Helper.ExecuteReader();
                while (Helper.Read())
                {
                    string ProcedureName = Helper.GetParameter("NAME", "");
                    string Definition = Helper.GetParameter("DEFINITION", "");
                    Temp.AddStoredProcedure(ProcedureName, Definition);
                }
            }
            foreach (StoredProcedure Procedure in Temp.StoredProcedures)
            {
                Command = "SELECT sys.systypes.name as TYPE,sys.parameters.name as NAME,sys.parameters.max_length as LENGTH,sys.parameters.default_value as [DEFAULT VALUE] FROM sys.procedures INNER JOIN sys.parameters on sys.procedures.object_id=sys.parameters.object_id INNER JOIN sys.systypes on sys.systypes.xusertype=sys.parameters.system_type_id WHERE sys.procedures.name=@ProcedureName AND (sys.systypes.xusertype <> 256)";
                using (SQLHelper Helper = new SQLHelper(Command, ConnectionString, CommandType.Text))
                {
                    Helper.AddParameter("@ProcedureName", Procedure.Name)
                          .ExecuteReader();
                    while (Helper.Read())
                    {
                        string Type = Helper.GetParameter("TYPE", "");
                        string Name = Helper.GetParameter("NAME", "");
                        int Length = Helper.GetParameter("LENGTH", 0);
                        if (Type == "nvarchar")
                            Length /= 2;
                        string Default = Helper.GetParameter("DEFAULT VALUE", "");
                        Procedure.AddColumn<string>(Name, Type.TryTo<string, SqlDbType>().ToDbType(), Length, Default);
                    }
                }
            }
        }

        /// <summary>
        /// Sets up the views
        /// </summary>
        /// <param name="ConnectionString">Connection string</param>
        /// <param name="Temp">Database object</param>
        private static void SetupViews(string ConnectionString, Database Temp)
        {
            foreach (View View in Temp.Views)
            {
                string Command = "SELECT OBJECT_DEFINITION(sys.views.object_id) as Definition FROM sys.views WHERE sys.views.name=@ViewName";
                using (SQLHelper Helper = new SQLHelper(Command, ConnectionString, CommandType.Text))
                {
                    Helper.AddParameter("@ViewName", View.Name)
                    .ExecuteReader();
                    if (Helper.Read())
                    {
                        View.Definition = Helper.GetParameter("Definition", "");
                    }
                }
                Command = "SELECT sys.columns.name AS [Column], sys.systypes.name AS [COLUMN TYPE], sys.columns.max_length as [MAX LENGTH], sys.columns.is_nullable as [IS NULLABLE] FROM sys.views INNER JOIN sys.columns on sys.columns.object_id=sys.views.object_id INNER JOIN sys.systypes ON sys.systypes.xtype = sys.columns.system_type_id WHERE (sys.views.name = @ViewName) AND (sys.systypes.xusertype <> 256)";
                using (SQLHelper Helper = new SQLHelper(Command, ConnectionString, CommandType.Text))
                {
                    Helper.AddParameter("@ViewName", View.Name)
                          .ExecuteReader();
                    while (Helper.Read())
                    {
                        string ColumnName = Helper.GetParameter("Column", "");
                        string ColumnType = Helper.GetParameter("COLUMN TYPE", "");
                        int MaxLength = Helper.GetParameter("MAX LENGTH", 0);
                        if (ColumnType == "nvarchar")
                            MaxLength /= 2;
                        bool Nullable = Helper.GetParameter("IS NULLABLE", false);
                        View.AddColumn<string>(ColumnName, ColumnType.TryTo<string, SqlDbType>().ToDbType(), MaxLength, Nullable);
                    }
                }
            }
        }

        /// <summary>
        /// Sets up the tables (pulls columns, etc.)
        /// </summary>
        /// <param name="ConnectionString">Connection string</param>
        /// <param name="Temp">Database object</param>
        private static void SetupTables(string ConnectionString, Database Temp)
        {
            foreach (Table Table in Temp.Tables)
            {
                string Command = "SELECT sys.columns.name AS [Column], sys.systypes.name AS [COLUMN TYPE], sys.columns.max_length as [MAX LENGTH], sys.columns.is_nullable as [IS NULLABLE], sys.columns.is_identity as [IS IDENTITY], sys.index_columns.index_id as [IS INDEX], key_constraints.name as [PRIMARY KEY], key_constraints_1.name as [UNIQUE], tables_1.name as [FOREIGN KEY TABLE], columns_1.name as [FOREIGN KEY COLUMN], sys.default_constraints.definition as [DEFAULT VALUE] FROM sys.tables INNER JOIN sys.columns on sys.columns.object_id=sys.tables.object_id INNER JOIN sys.systypes ON sys.systypes.xtype = sys.columns.system_type_id LEFT OUTER JOIN sys.index_columns on sys.index_columns.object_id=sys.tables.object_id and sys.index_columns.column_id=sys.columns.column_id LEFT OUTER JOIN sys.key_constraints on sys.key_constraints.parent_object_id=sys.tables.object_id and sys.key_constraints.parent_object_id=sys.index_columns.object_id and sys.index_columns.index_id=sys.key_constraints.unique_index_id and sys.key_constraints.type='PK' LEFT OUTER JOIN sys.foreign_key_columns on sys.foreign_key_columns.parent_object_id=sys.tables.object_id and sys.foreign_key_columns.parent_column_id=sys.columns.column_id LEFT OUTER JOIN sys.tables as tables_1 on tables_1.object_id=sys.foreign_key_columns.referenced_object_id LEFT OUTER JOIN sys.columns as columns_1 on columns_1.column_id=sys.foreign_key_columns.referenced_column_id and columns_1.object_id=tables_1.object_id LEFT OUTER JOIN sys.key_constraints as key_constraints_1 on key_constraints_1.parent_object_id=sys.tables.object_id and key_constraints_1.parent_object_id=sys.index_columns.object_id and sys.index_columns.index_id=key_constraints_1.unique_index_id and key_constraints_1.type='UQ' LEFT OUTER JOIN sys.default_constraints on sys.default_constraints.object_id=sys.columns.default_object_id WHERE (sys.tables.name = @TableName) AND (sys.systypes.xusertype <> 256)";
                using (SQLHelper Helper = new SQLHelper(Command, ConnectionString, CommandType.Text))
                {
                    Helper.AddParameter("@TableName", Table.Name)
                    .ExecuteReader();
                    while (Helper.Read())
                    {
                        string ColumnName = Helper.GetParameter("Column", "");
                        string ColumnType = Helper.GetParameter("COLUMN TYPE", "");
                        int MaxLength = Helper.GetParameter("MAX LENGTH", 0);
                        if (ColumnType == "nvarchar")
                            MaxLength /= 2;
                        bool Nullable = Helper.GetParameter("IS NULLABLE", false);
                        bool Identity = Helper.GetParameter("IS IDENTITY", false);
                        bool Index = Helper.GetParameter("IS INDEX", 0) != 0;
                        bool PrimaryKey = Helper.GetParameter("PRIMARY KEY", "").IsNullOrEmpty() ? false : true;
                        bool Unique = Helper.GetParameter("UNIQUE", "").IsNullOrEmpty() ? false : true;
                        string ForeignKeyTable = Helper.GetParameter("FOREIGN KEY TABLE", "");
                        string ForeignKeyColumn = Helper.GetParameter("FOREIGN KEY COLUMN", "");
                        string DefaultValue = Helper.GetParameter("DEFAULT VALUE", "");
                        if (Table.ContainsColumn(ColumnName))
                        {
                            Table.AddForeignKey(ColumnName, ForeignKeyTable, ForeignKeyColumn);
                        }
                        else
                        {
                            Table.AddColumn(ColumnName, ColumnType.TryTo<string, SqlDbType>().ToDbType(), MaxLength, Nullable, Identity, Index, PrimaryKey, Unique, ForeignKeyTable, ForeignKeyColumn, DefaultValue);
                        }
                    }
                }
                Command = "SELECT sys.triggers.name as Name,sys.trigger_events.type as Type,OBJECT_DEFINITION(sys.triggers.object_id) as Definition FROM sys.triggers INNER JOIN sys.trigger_events ON sys.triggers.object_id=sys.trigger_events.object_id INNER JOIN sys.tables on sys.triggers.parent_id=sys.tables.object_id where sys.tables.name=@TableName";
                using (SQLHelper Helper = new SQLHelper(Command, ConnectionString, CommandType.Text))
                {
                    Helper.AddParameter("@TableName", Table.Name)
                        .ExecuteReader();
                    while (Helper.Read())
                    {
                        string Name = Helper.GetParameter("Name", "");
                        int Type = Helper.GetParameter("Type", 0);
                        string Definition = Helper.GetParameter("Definition", "");
                        Table.AddTrigger(Name, Definition, Type.ToString(CultureInfo.InvariantCulture).TryTo<string, TriggerType>());
                    }
                }
            }
            foreach (Table Table in Temp.Tables)
            {
                Table.SetupForeignKeys();
            }
        }

        /// <summary>
        /// Gets the tables for a database
        /// </summary>
        /// <param name="ConnectionString">Connection string</param>
        /// <param name="Temp">The database object</param>
        private static void GetTables(string ConnectionString, Database Temp)
        {
            string Command = "SELECT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES";
            using (SQLHelper Helper = new SQLHelper(Command, ConnectionString, CommandType.Text))
            {
                Helper.ExecuteReader();
                while (Helper.Read())
                {
                    string TableName = Helper.GetParameter("TABLE_NAME", "");
                    string TableType = Helper.GetParameter("TABLE_TYPE", "");
                    if (TableType == "BASE TABLE")
                    {
                        Temp.AddTable(TableName);
                    }
                    else if (TableType == "VIEW")
                    {
                        Temp.AddView(TableName);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if something exists
        /// </summary>
        /// <param name="Command">Command to run</param>
        /// <param name="Name">Name of the item</param>
        /// <param name="ConnectionString">Connection string</param>
        /// <returns>True if it exists, false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static bool CheckExists(string Command, string Name, string ConnectionString)
        {
            using (SQLHelper Helper = new SQLHelper(Command, ConnectionString, CommandType.Text))
            {
                try
                {
                    return Helper.AddParameter("@Name", Name)
                        .ExecuteReader()
                        .Read();
                }
                catch { }
            }
            return false;
        }

        #endregion
    }
}