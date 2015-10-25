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
using System.Data.Common;
using Company.Utilities.DataTypes.Comparison;
using Company.Utilities.DataTypes.ExtensionMethods;
using System.Globalization;

#endregion

namespace Company.Utilities.SQL.ExtensionMethods
{
    /// <summary>
    /// Extension methods for DbCommand objects
    /// </summary>
    public static class DbCommandExtensions
    {
        #region Functions

        #region AddParameter

        /// <summary>
        /// Adds a parameter to the call (for strings only)
        /// </summary>
        /// <param name="Command">Command object</param>
        /// <param name="ID">Name of the parameter</param>
        /// <param name="Value">Value to add</param>
        /// <param name="Direction">Direction that the parameter goes (in or out)</param>
        /// <returns>The DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand Command, string ID, string Value = "",
            ParameterDirection Direction = ParameterDirection.Input)
        {
            Command.ThrowIfNull("Command");
            ID.ThrowIfNullOrEmpty("ID");
            int Length = Value.IsNullOrEmpty() ? 1 : Value.Length;
            if (Direction == ParameterDirection.Output
                || Direction == ParameterDirection.InputOutput
                || Length > 4000
                || Length < -1)
                Length = -1;
            DbParameter Parameter = Command.GetOrCreateParameter(ID);
            Parameter.Value = Value.IsNullOrEmpty() ? System.DBNull.Value : (object)Value;
            Parameter.IsNullable = Value.IsNullOrEmpty();
            Parameter.DbType = typeof(string).ToDbType();
            Parameter.Direction = Direction;
            Parameter.Size = Length;
            return Command;
        }

        /// <summary>
        /// Adds a parameter to the call (for all types other than strings)
        /// </summary>
        /// <param name="ID">Name of the parameter</param>
        /// <param name="Value">Value to add</param>
        /// <param name="Direction">Direction that the parameter goes (in or out)</param>
        /// <param name="Command">Command object</param>
        /// <param name="Type">SQL type of the parameter</param>
        /// <returns>The DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand Command, string ID, SqlDbType Type,
            object Value = null, ParameterDirection Direction = ParameterDirection.Input)
        {
            Command.ThrowIfNull("Command");
            ID.ThrowIfNullOrEmpty("ID");
            return Command.AddParameter(ID, Type.ToDbType(), Value, Direction);
        }

        /// <summary>
        /// Adds a parameter to the call (for all types other than strings)
        /// </summary>
        /// <typeparam name="DataType">Data type of the parameter</typeparam>
        /// <param name="ID">Name of the parameter</param>
        /// <param name="Direction">Direction that the parameter goes (in or out)</param>
        /// <param name="Command">Command object</param>
        /// <param name="Value">Value to add</param>
        /// <returns>The DbCommand object</returns>
        public static DbCommand AddParameter<DataType>(this DbCommand Command, string ID, DataType Value = default(DataType),
            ParameterDirection Direction = ParameterDirection.Input)
        {
            Command.ThrowIfNull("Command");
            ID.ThrowIfNullOrEmpty("ID");
            return Command.AddParameter(ID,
                new GenericEqualityComparer<DataType>().Equals(Value, default(DataType)) ? typeof(DataType).ToDbType() : Value.GetType().ToDbType(),
                Value, Direction);
        }

        /// <summary>
        /// Adds a parameter to the call (for all types other than strings)
        /// </summary>
        /// <param name="ID">Name of the parameter</param>
        /// <param name="Direction">Direction that the parameter goes (in or out)</param>
        /// <param name="Command">Command object</param>
        /// <param name="Value">Value to add</param>
        /// <param name="Type">SQL type of the parameter</param>
        /// <returns>The DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand Command, string ID, DbType Type, object Value = null,
            ParameterDirection Direction = ParameterDirection.Input)
        {
            Command.ThrowIfNull("Command");
            ID.ThrowIfNullOrEmpty("ID");
            DbParameter Parameter = Command.GetOrCreateParameter(ID);
            Parameter.Value = Value.IsNull() ? System.DBNull.Value : Value;
            Parameter.IsNullable = Value.IsNull();
            if (Type != default(DbType))
                Parameter.DbType = Type;
            Parameter.Direction = Direction;
            if (Type.ToString() == "String" && Parameter.Direction == ParameterDirection.Output)
                Parameter.Size = 4000;
            return Command;
        }

        #endregion

        #region BeginTransaction

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <param name="Command">Command object</param>
        /// <returns>A transaction object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static DbTransaction BeginTransaction(this DbCommand Command)
        {
            if (Command.IsNull() && Command.Connection.IsNull())
                return null;
            Command.Open();
            Command.Transaction=Command.Connection.BeginTransaction();
            return Command.Transaction;
        }

        #endregion

        #region ClearParameters

        /// <summary>
        /// Clears the parameters
        /// </summary>
        /// <param name="Command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand ClearParameters(this DbCommand Command)
        {
            if (Command.IsNotNull() && Command.Parameters.IsNotNull())
                Command.Parameters.Clear();
            return Command;
        }

        #endregion

        #region Close

        /// <summary>
        /// Closes the connection
        /// </summary>
        /// <param name="Command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand Close(this DbCommand Command)
        {
            if (Command.IsNotNull()
                && Command.Connection.IsNotNull()
                && Command.Connection.State != ConnectionState.Closed)
                Command.Connection.Close();
            return Command;
        }

        #endregion

        #region Commit

        /// <summary>
        /// Commits a transaction
        /// </summary>
        /// <param name="Command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand Commit(this DbCommand Command)
        {
            if (Command.IsNotNull() && Command.Transaction.IsNotNull())
                Command.Transaction.Commit();
            return Command;
        }

        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// Executes the query and returns a data set
        /// </summary>
        /// <param name="Command">Command object</param>
        /// <param name="Factory">DbProviderFactory being used</param>
        /// <returns>A dataset filled with the results of the query</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static DataSet ExecuteDataSet(this DbCommand Command, DbProviderFactory Factory)
        {
            if (Command.IsNull())
                return null;
            Command.Open();
            using (DbDataAdapter Adapter = Factory.CreateDataAdapter())
            {
                Adapter.SelectCommand = Command;
                DataSet ReturnSet = new DataSet();
                ReturnSet.Locale = CultureInfo.CurrentCulture;
                Adapter.Fill(ReturnSet);
                return ReturnSet;
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes the stored procedure as a scalar query
        /// </summary>
        /// <param name="Command">Command object</param>
        /// <param name="Default">Default value if there is an issue</param>
        /// <returns>The object of the first row and first column</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static DataType ExecuteScalar<DataType>(this DbCommand Command, DataType Default = default(DataType))
        {
            if (Command.IsNull())
                return Default;
            Command.Open();
            return Command.ExecuteScalar().TryTo<object, DataType>(Default);
        }

        #endregion

        #region GetOrCreateParameter

        /// <summary>
        /// Gets a parameter or creates it, if it is not found
        /// </summary>
        /// <param name="ID">Name of the parameter</param>
        /// <param name="Command">Command object</param>
        /// <returns>The DbParameter associated with the ID</returns>
        public static DbParameter GetOrCreateParameter(this DbCommand Command, string ID)
        {
            if (Command.Parameters.Contains(ID))
                return Command.Parameters[ID];
            else
            {
                DbParameter Parameter = Command.CreateParameter();
                Parameter.ParameterName = ID;
                Command.Parameters.Add(Parameter);
                return Parameter;
            }
        }

        #endregion

        #region GetOutputParameter

        /// <summary>
        /// Returns an output parameter's value
        /// </summary>
        /// <typeparam name="DataType">Data type of the object</typeparam>
        /// <param name="ID">Parameter name</param>
        /// <param name="Command">Command object</param>
        /// <param name="Default">Default value for the parameter</param>
        /// <returns>if the parameter exists (and isn't null or empty), it returns the parameter's value. Otherwise the default value is returned.</returns>
        public static DataType GetOutputParameter<DataType>(this DbCommand Command, string ID, DataType Default = default(DataType))
        {
            return Command.IsNotNull() && Command.Parameters[ID].IsNotNull() ?
                Command.Parameters[ID].Value.TryTo<object, DataType>(Default) :
                Default;
        }

        #endregion

        #region Open

        /// <summary>
        /// Opens the connection
        /// </summary>
        /// <param name="Command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand Open(this DbCommand Command)
        {
            if (Command.IsNotNull()
                && Command.Connection.IsNotNull()
                && Command.Connection.State != ConnectionState.Open)
                Command.Connection.Open();
            return Command;
        }

        #endregion

        #region Rollback

        /// <summary>
        /// Rolls back a transaction
        /// </summary>
        /// <param name="Command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand Rollback(this DbCommand Command)
        {
            if (Command.IsNotNull() && Command.Transaction.IsNotNull())
                Command.Transaction.Rollback();
            return Command;
        }

        #endregion

        #endregion
    }
}