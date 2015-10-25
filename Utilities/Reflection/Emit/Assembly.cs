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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Company.Utilities.Reflection.Emit.Enums;
using Company.Utilities.Reflection.Emit.Interfaces;
using Company.Utilities.DataTypes.ExtensionMethods;
#endregion

namespace Company.Utilities.Reflection.Emit
{
    /// <summary>
    /// Assembly class
    /// </summary>
    public class Assembly
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Name">Assembly name</param>
        /// <param name="Directory">Directory to save the assembly (if left blank, the assembly is run only and will not be saved)</param>
        /// <param name="Type">Assembly type (exe or dll)</param>
        public Assembly(string Name, string Directory = "", AssemblyType Type = AssemblyType.DLL)
        {
            if (string.IsNullOrEmpty(Name))
                throw new ArgumentNullException("Name");
            Setup(Name, Directory, Type);
        }

        #endregion

        #region Functions

        #region Setup

        /// <summary>
        /// Sets up the assembly
        /// </summary>
        /// <param name="Name">Assembly name</param>
        /// <param name="Directory">Directory to save the assembly (if left blank, the assembly is run only and will not be saved)</param>
        /// <param name="Type">Assembly type (dll or exe)</param>
        private void Setup(string Name, string Directory = "", AssemblyType Type = AssemblyType.DLL)
        {
            this.Name = Name;
            this.Directory = Directory;
            this.Type = Type;
            AssemblyName AssemblyName = new AssemblyName(Name);
            AppDomain Domain = Thread.GetDomain();
            if (!string.IsNullOrEmpty(Directory))
            {
                Builder = Domain.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndSave, Directory);
                Module = Builder.DefineDynamicModule(Name, Name + (Type == AssemblyType.DLL ? ".dll" : ".exe"), true);
            }
            else
            {
                Builder = Domain.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);
                Module = Builder.DefineDynamicModule(Name);
            }
            Classes = new List<TypeBuilder>();
            Enums = new List<EnumBuilder>();
        }

        #endregion

        #region CreateType

        /// <summary>
        /// Creates a type builder
        /// </summary>
        /// <param name="Name">Name of the type</param>
        /// <param name="Attributes">Attributes associated with the type</param>
        /// <param name="BaseClass">Base class for this type</param>
        /// <param name="Interfaces">Interfaces used by this type</param>
        /// <returns>A TypeBuilder class</returns>
        public virtual TypeBuilder CreateType(string Name, TypeAttributes Attributes = TypeAttributes.Public,
            Type BaseClass = null, IEnumerable<Type> Interfaces = null)
        {
            TypeBuilder ReturnValue = new TypeBuilder(this, Name, Interfaces, BaseClass, Attributes);
            Classes.Add(ReturnValue);
            return ReturnValue;
        }

        #endregion

        #region CreateEnum

        /// <summary>
        /// Creates an enum builder
        /// </summary>
        /// <param name="Name">Name of the enum</param>
        /// <param name="EnumBaseType">Base type of the enum (defaults to int)</param>
        /// <param name="Attributes">Attributes associated with the type</param>
        /// <returns>An EnumBuilder class</returns>
        public virtual EnumBuilder CreateEnum(string Name, Type EnumBaseType = null,
            TypeAttributes Attributes = TypeAttributes.Public)
        {
            if (EnumBaseType == null)
                EnumBaseType = typeof(int);
            EnumBuilder ReturnValue = new EnumBuilder(this, Name, EnumBaseType, Attributes);
            Enums.Add(ReturnValue);
            return ReturnValue;
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates all types associated with the assembly and saves the assembly to disk
        /// if a directory is specified.
        /// </summary>
        public virtual void Create()
        {
            foreach (IType Class in Classes)
                Class.Create();
            foreach (IType Enum in Enums)
                Enum.Create();
            if(!string.IsNullOrEmpty(this.Directory))
                Builder.Save(Name + (Type == AssemblyType.DLL ? ".dll" : ".exe"));
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// ModuleBuilder object
        /// </summary>
        public ModuleBuilder Module { get; protected set; }

        /// <summary>
        /// Name of the assembly
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Directory of the assembly
        /// </summary>
        public string Directory { get; protected set; }

        /// <summary>
        /// List of classes associated with this assembly
        /// </summary>
        public ICollection<TypeBuilder> Classes { get; private set; }

        /// <summary>
        /// List of enums associated with this assembly
        /// </summary>
        public ICollection<EnumBuilder> Enums { get; private set; }

        /// <summary>
        /// Assembly type (exe or dll)
        /// </summary>
        public AssemblyType Type { get; protected set; }

        /// <summary>
        /// Assembly builder
        /// </summary>
        protected AssemblyBuilder Builder { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        /// Converts the assembly to a string
        /// </summary>
        /// <returns>The string version of the assembly</returns>
        public override string ToString()
        {
            StringBuilder Output = new StringBuilder();
            Enums.ForEach(x => Output.Append(x.ToString()));
            Classes.ForEach(x => Output.Append(x.ToString()));
            return Output.ToString();
        }

        #endregion
    }
}