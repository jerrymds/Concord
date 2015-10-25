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
using System.Collections.Generic;
using System.Data;
using Company.Utilities.ORM.QueryProviders;
using Company.Utilities.SQL.Interfaces;
#endregion

namespace Company.Utilities.ORM
{
    /// <summary>
    /// Session class
    /// </summary>
    public class Session
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="QueryProvider">Query provider</param>
        public Session(Default QueryProvider)
        {
            this.QueryProvider = QueryProvider;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Query provider
        /// </summary>
        private Default QueryProvider { get; set; }

        #endregion

        #region Functions

        #region Any

        /// <summary>
        /// Returns a single item matching the criteria
        /// </summary>
        /// <typeparam name="ObjectType">Type of the object</typeparam>
        /// <param name="Columns">Columns to load</param>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>A single object matching the criteria</returns>
        public virtual ObjectType Any<ObjectType>(string Columns, params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.Any<ObjectType>(this, Columns, null, Parameters);
        }

        /// <summary>
        /// Returns a single item matching the criteria
        /// </summary>
        /// <typeparam name="ObjectType">Type of the object</typeparam>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>A single object matching the criteria</returns>
        public virtual ObjectType Any<ObjectType>(params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.Any<ObjectType>(this, null, Parameters);
        }

        /// <summary>
        /// Returns a single item matching the criteria
        /// </summary>
        /// <typeparam name="ObjectType">Type of the object</typeparam>
        /// <param name="Command">SQL Command to run</param>
        /// <param name="CommandType">Command type</param>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>A single object matching the criteria</returns>
        public virtual ObjectType Any<ObjectType>(string Command, CommandType CommandType, params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.Any<ObjectType>(this, Command, CommandType, null, Parameters);
        }

        #endregion

        #region All

        /// <summary>
        /// Returns all items that match the criteria
        /// </summary>
        /// <typeparam name="ObjectType">Type of the object</typeparam>
        /// <param name="Columns">Columns to load</param>
        /// <param name="Limit">Limit of the number of items to load</param>
        /// <param name="OrderBy">Order by clause (minus the ORDER BY part)</param>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>All items that match the criteria</returns>
        public virtual IEnumerable<ObjectType> All<ObjectType>(string Columns, int Limit, string OrderBy, params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.All<ObjectType>(this, Columns, Limit, OrderBy, Parameters);
        }

        /// <summary>
        /// Returns all items that match the criteria
        /// </summary>
        /// <typeparam name="ObjectType">Type of the object</typeparam>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>All items that match the criteria</returns>
        public virtual IEnumerable<ObjectType> All<ObjectType>(params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.All<ObjectType>(this, Parameters);
        }

        /// <summary>
        /// /// Returns all items that match the criteria
        /// </summary>
        /// <typeparam name="ObjectType"></typeparam>
        /// <param name="Command">SQL Command to run</param>
        /// <param name="CommandType">Command type</param>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>All items that match the criteria</returns>
        public virtual IEnumerable<ObjectType> All<ObjectType>(string Command, CommandType CommandType, params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.All<ObjectType>(this, Command, CommandType, Parameters);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes an object from the database
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <param name="Object">Object to delete</param>
        public virtual void Delete<ObjectType>(ObjectType Object) where ObjectType : class,new()
        {
            QueryProvider.Delete(Object);
        }

        #endregion

        #region LoadProperties

        /// <summary>
        /// Loads a property (primarily used internally for lazy loading)
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <typeparam name="DataType">Data type</typeparam>
        /// <param name="Object">Object</param>
        /// <param name="PropertyName">Property name</param>
        /// <param name="Parameters">Extra parameters (generally will be the ID of the object)</param>
        /// <returns>The appropriate property value</returns>
        public virtual IEnumerable<DataType> LoadProperties<ObjectType, DataType>(ObjectType Object, string PropertyName, params IParameter[] Parameters)
            where ObjectType : class,new()
            where DataType : class,new()
        {
            return QueryProvider.LoadProperties<ObjectType, DataType>(this, Object, PropertyName, Parameters);
        }

        #endregion

        #region LoadListProperties

        /// <summary>
        /// Loads a property (primarily used internally for lazy loading)
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <typeparam name="DataType">Data type</typeparam>
        /// <param name="Object">Object</param>
        /// <param name="PropertyName">Property name</param>
        /// <param name="Parameters">Extra parameters (generally will be the ID of the object)</param>
        /// <returns>The appropriate property value</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public virtual List<DataType> LoadListProperties<ObjectType, DataType>(ObjectType Object, string PropertyName, params IParameter[] Parameters)
            where ObjectType : class,new()
            where DataType : class,new()
        {
            return QueryProvider.LoadListProperties<ObjectType, DataType>(this, Object, PropertyName, Parameters);
        }

        #endregion

        #region LoadProperty

        /// <summary>
        /// Loads a property (primarily used internally for lazy loading)
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <typeparam name="DataType">Data type</typeparam>
        /// <param name="Object">Object</param>
        /// <param name="PropertyName">Property name</param>
        /// <param name="Parameters">Extra parameters (generally will be the ID of the object)</param>
        /// <returns>The appropriate property value</returns>
        public virtual DataType LoadProperty<ObjectType, DataType>(ObjectType Object, string PropertyName, params IParameter[] Parameters)
            where ObjectType : class,new()
            where DataType : class,new()
        {
            return QueryProvider.LoadProperty<ObjectType, DataType>(this, Object, PropertyName, Parameters);
        }

        #endregion

        #region Paged

        /// <summary>
        /// Returns a paged list of items
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <param name="Columns">Columns to load</param>
        /// <param name="OrderBy">Order by clause (minus the ORDER BY part)</param>
        /// <param name="PageSize">Page size</param>
        /// <param name="CurrentPage">Current page (starting with 0)</param>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>A paged list of items that match the criteria</returns>
        public virtual IEnumerable<ObjectType> Paged<ObjectType>(string Columns = "*", string OrderBy = "", int PageSize = 25, int CurrentPage = 0, params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.Paged<ObjectType>(this, Columns, OrderBy, PageSize, CurrentPage, Parameters);
        }

        #endregion

        #region PagedCommand

        /// <summary>
        /// Returns a paged list of items
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <param name="Command">Command to call</param>
        /// <param name="OrderBy">Order by clause (minus the ORDER BY part)</param>
        /// <param name="PageSize">Page size</param>
        /// <param name="CurrentPage">Current page (starting with 0)</param>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>A paged list of items that match the criteria</returns>
        public virtual IEnumerable<ObjectType> PagedCommand<ObjectType>(string Command, string OrderBy = "", int PageSize = 25, int CurrentPage = 0, params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.PagedCommand<ObjectType>(this, Command, OrderBy, PageSize, CurrentPage, Parameters);
        }

        #endregion

        #region PageCount

        /// <summary>
        /// Gets the number of pages based on the specified 
        /// </summary>
        /// <param name="PageSize">Page size</param>
        /// <param name="Parameters">Parameters to search by</param>
        /// <typeparam name="ObjectType">Object type to get the page count of</typeparam>
        /// <returns>The number of pages that the table contains for the specified page size</returns>
        public virtual int PageCount<ObjectType>(int PageSize = 25, params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.PageCount<ObjectType>(this, PageSize, Parameters);
        }

        /// <summary>
        /// Gets the number of pages based on the specified 
        /// </summary>
        /// <param name="PageSize">Page size</param>
        /// <param name="Parameters">Parameters to search by</param>
        /// <param name="Command">Command to get the page count of</param>
        /// <typeparam name="ObjectType">Object type to get the page count of</typeparam>
        /// <returns>The number of pages that the table contains for the specified page size</returns>
        public virtual int PageCount<ObjectType>(string Command, int PageSize = 25, params IParameter[] Parameters) where ObjectType : class,new()
        {
            return QueryProvider.PageCount<ObjectType>(this, Command, PageSize, Parameters);
        }

        #endregion

        #region Save

        /// <summary>
        /// Saves an object to the database
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <typeparam name="PrimaryKeyType">Primary key type</typeparam>
        /// <param name="Object">Object to save</param>
        /// <param name="Parameters">Extra parameters to save</param>
        public virtual void Save<ObjectType, PrimaryKeyType>(ObjectType Object, params IParameter[] Parameters) where ObjectType : class,new()
        {
            QueryProvider.Save<ObjectType, PrimaryKeyType>(Object, Parameters);
        }

        #endregion

        #region Scalar

        /// <summary>
        /// Runs a supplied scalar function and returns the result
        /// </summary>
        /// <param name="CommandType">Command type</param>
        /// <param name="Parameters">Parameters to search by</param>
        /// <param name="Command">Command to get the page count of</param>
        /// <typeparam name="DataType">Data type</typeparam>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <returns>The scalar value returned by the command</returns>
        public virtual DataType Scalar<ObjectType, DataType>(string Command, CommandType CommandType, params IParameter[] Parameters)
            where ObjectType : class,new()
        {
            return QueryProvider.Scalar<ObjectType, DataType>(this, Command, CommandType, Parameters);
        }

        /// <summary>
        /// Runs a scalar command using the specified aggregate function
        /// </summary>
        /// <typeparam name="DataType">Data type</typeparam>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <param name="AggregateFunction">Aggregate function</param>
        /// <param name="Parameters">Parameters</param>
        /// <returns>The scalar value returned by the command</returns>
        public virtual DataType Scalar<ObjectType, DataType>(string AggregateFunction, params IParameter[] Parameters)
            where ObjectType : class,new()
        {
            return QueryProvider.Scalar<ObjectType, DataType>(this, AggregateFunction, Parameters);
        }

        #endregion

        #endregion
    }
}