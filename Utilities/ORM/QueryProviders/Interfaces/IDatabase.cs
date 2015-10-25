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

#endregion

namespace Company.Utilities.ORM.QueryProviders.Interfaces
{
    /// <summary>
    /// Database configuration interface
    /// </summary>
    public interface IDatabase
    {
        #region Properties

        /// <summary>
        /// Name associated with the database
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Connection string
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// String used to start parameter names
        /// </summary>
        string ParameterStarter { get; }

        /// <summary>
        /// Determines if audit tables are generated
        /// </summary>
        bool Audit { get; }

        /// <summary>
        /// Should the structure of the database be updated?
        /// </summary>
        bool Update { get; }

        /// <summary>
        /// Should this database be used to write data?
        /// </summary>
        bool Writable { get; }

        /// <summary>
        /// Should this database be used to read data?
        /// </summary>
        bool Readable { get; }

        /// <summary>
        /// Order that this database should be in
        /// (if only one database is being used, it is ignored)
        /// </summary>
        int Order { get; }

        #endregion
    }
}