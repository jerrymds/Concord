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
using System.Reflection;
using Company.Utilities.ORM.Aspect;
using Company.Utilities.ORM.Database;
using Company.Utilities.ORM.Mapping;
using Company.Utilities.ORM.Mapping.Interfaces;
using Company.Utilities.ORM.QueryProviders;
#endregion

namespace Company.Utilities.ORM
{
    /// <summary>
    /// Main ORM class
    /// </summary>
    public class ORM
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Profile">Should calls be profiled?</param>
        /// <param name="Assemblies">Assemblies containing business object mappings</param>
        public ORM(bool Profile, params Assembly[] Assemblies)
        {
            Setup(Profile, Assemblies);
        }

        #endregion

        #region Functions

        private static void Setup(bool Profile, Assembly[] Assemblies)
        {
            MappingManager = new MappingManager(Assemblies);
            QueryProvider = new Default(Profile, Assemblies);
            foreach (Type Key in MappingManager.Mappings.Keys)
                foreach (IMapping Mapping in MappingManager.Mappings[Key])
                    QueryProvider.AddMapping(Mapping);
            Company.Utilities.Reflection.AOP.AOPManager Manager = new Reflection.AOP.AOPManager();
            Manager.AddAspect(new ORMAspect(MappingManager.Mappings));
            DatabaseManager = new DatabaseManager(QueryProvider.Mappings);
            DatabaseManager.Setup();
        }

        /// <summary>
        /// Can be used to setup various bits of data that are normally created on the fly as the system is used.
        /// Also calls initialization code found in mappings.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void Setup()
        {
            Company.Utilities.Reflection.AOP.AOPManager Manager = new Reflection.AOP.AOPManager();
            foreach (Type Key in MappingManager.Mappings.Keys)
                foreach (IMapping Mapping in MappingManager.Mappings[Key])
                    Manager.Setup(Mapping.ObjectType);
            Manager.Save();
            MappingManager.Initialize();
        }

        /// <summary>
        /// Creates a session to allow you to make queries to the system
        /// </summary>
        /// <returns>A session object</returns>
        public static Session CreateSession()
        {
            return new Session(QueryProvider);
        }

        /// <summary>
        /// Deletes all mappings, etc. Basically clears out everything so you can recreate items (only really useful for testing)
        /// </summary>
        public static void Destroy()
        {
            Company.Utilities.SQL.SQLHelper.ClearAllMappings();
            Reflection.AOP.AOPManager.Destroy();
            MappingManager = null;
            QueryProvider = null;
            DatabaseManager = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Query provider
        /// </summary>
        private static Default QueryProvider { get; set; }

        /// <summary>
        /// Mapping manager
        /// </summary>
        private static MappingManager MappingManager { get; set; }

        /// <summary>
        /// Database manager
        /// </summary>
        private static DatabaseManager DatabaseManager { get; set; }

        #endregion
    }
}