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
using System.Text;
using Company.Utilities.Reflection.Emit.Interfaces;
using Company.Utilities.DataTypes.ExtensionMethods;
#endregion

namespace Company.Utilities.Reflection.Emit
{
    /// <summary>
    /// Helper class for defining types
    /// </summary>
    public class TypeBuilder : IType
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Assembly">Assembly to generate the type within</param>
        /// <param name="Name">Name of the type</param>
        /// <param name="Interfaces">Interfaces that the type implements</param>
        /// <param name="Attributes">Attributes for the type (public, private, etc.)</param>
        /// <param name="BaseClass">Base class for the type</param>
        public TypeBuilder(Assembly Assembly, string Name, IEnumerable<Type> Interfaces,
            Type BaseClass, TypeAttributes Attributes)
        {
            if (Assembly == null)
                throw new ArgumentNullException("Assembly");
            if (string.IsNullOrEmpty(Name))
                throw new ArgumentNullException("Name");
            this.Assembly = Assembly;
            this.Name = Name;
            this.Interfaces = new List<Type>();
            if (Interfaces != null)
                this.Interfaces.Add(Interfaces);
            this.BaseClass = BaseClass;
            this.Attributes = Attributes;
            Methods = new List<IMethodBuilder>();
            Fields = new List<FieldBuilder>();
            Properties = new List<IPropertyBuilder>();
            Constructors = new List<IMethodBuilder>();
            Builder = Assembly.Module.DefineType(Assembly.Name + "." + Name, Attributes, BaseClass, this.Interfaces.ToArray(x => x));
        }

        #endregion

        #region Functions

        #region Create

        /// <summary>
        /// Creates the type
        /// </summary>
        /// <returns>The type defined by this TypeBuilder</returns>
        public virtual Type Create()
        {
            if (Builder == null)
                throw new InvalidOperationException("The builder object has not been defined. Ensure that Setup is called prior to Create");
            if (DefinedType != null)
                return DefinedType;
            DefinedType = Builder.CreateType();
            return DefinedType;
        }

        #endregion

        #region CreateMethod

        /// <summary>
        /// Creates a method
        /// </summary>
        /// <param name="Name">Method name</param>
        /// <param name="Attributes">Attributes for the method (public, virtual, etc.)</param>
        /// <param name="ReturnType">Return type</param>
        /// <param name="ParameterTypes">Parameter types</param>
        /// <returns>Method builder for the method</returns>
        public virtual IMethodBuilder CreateMethod(string Name,
            MethodAttributes Attributes = MethodAttributes.Public| MethodAttributes.Virtual,
            Type ReturnType = null, IEnumerable<Type> ParameterTypes = null)
        {
            MethodBuilder ReturnValue = new MethodBuilder(this, Name, Attributes, ParameterTypes, ReturnType);
            Methods.Add(ReturnValue);
            return ReturnValue;
        }

        #endregion

        #region CreateField

        /// <summary>
        /// Creates a field
        /// </summary>
        /// <param name="Name">Name of the field</param>
        /// <param name="FieldType">Type of the field</param>
        /// <param name="Attributes">Attributes for the field (public, private, etc.)</param>
        /// <returns>Field builder for the field</returns>
        public virtual FieldBuilder CreateField(string Name, Type FieldType, FieldAttributes Attributes = FieldAttributes.Public)
        {
            FieldBuilder ReturnValue = new FieldBuilder(this, Name, FieldType, Attributes);
            Fields.Add(ReturnValue);
            return ReturnValue;
        }

        #endregion

        #region CreateProperty

        /// <summary>
        /// Creates a property
        /// </summary>
        /// <param name="Name">Name of the property</param>
        /// <param name="PropertyType">Type of the property</param>
        /// <param name="Attributes">Attributes for the property (special name, etc.)</param>
        /// <param name="GetMethodAttributes">Get method's attributes (public, private, etc.)</param>
        /// <param name="SetMethodAttributes">Set method's attributes (public, private, etc.)</param>
        /// <param name="Parameters">Parameter types</param>
        /// <returns>Property builder for the property</returns>
        public virtual IPropertyBuilder CreateProperty(string Name, Type PropertyType,
            PropertyAttributes Attributes = PropertyAttributes.SpecialName,
            MethodAttributes GetMethodAttributes = MethodAttributes.Public | MethodAttributes.Virtual,
            MethodAttributes SetMethodAttributes = MethodAttributes.Public | MethodAttributes.Virtual,
            IEnumerable<Type> Parameters = null)
        {
            PropertyBuilder ReturnValue = new PropertyBuilder(this, Name, Attributes,
                GetMethodAttributes, SetMethodAttributes, PropertyType, Parameters);
            Properties.Add(ReturnValue);
            return ReturnValue;
        }

        #endregion

        #region CreateDefaultProperty

        /// <summary>
        /// Creates a default property (ex int Property{get;set;}
        /// </summary>
        /// <param name="Name">Name of the property</param>
        /// <param name="PropertyType">Type of the property</param>
        /// <param name="Attributes">Attributes for the property (special name, etc.)</param>
        /// <param name="GetMethodAttributes">Get method's attributes (public, private, etc.)</param>
        /// <param name="SetMethodAttributes">Set method's attributes (public, private, etc.)</param>
        /// <param name="Parameters">Parameter types</param>
        /// <returns>Property builder for the property</returns>
        public virtual IPropertyBuilder CreateDefaultProperty(string Name, Type PropertyType,
            PropertyAttributes Attributes = PropertyAttributes.SpecialName,
            MethodAttributes GetMethodAttributes = MethodAttributes.Public | MethodAttributes.Virtual,
            MethodAttributes SetMethodAttributes = MethodAttributes.Public | MethodAttributes.Virtual,
            IEnumerable<Type> Parameters = null)
        {
            DefaultPropertyBuilder ReturnValue = new DefaultPropertyBuilder(this, Name, Attributes,
                GetMethodAttributes, SetMethodAttributes, PropertyType, Parameters);
            Properties.Add(ReturnValue);
            return ReturnValue;
        }

        #endregion

        #region CreateConstructor

        /// <summary>
        /// Creates a constructor
        /// </summary>
        /// <param name="Attributes">Attributes for the constructor (public, private, etc.)</param>
        /// <param name="ParameterTypes">The types for the parameters</param>
        /// <param name="CallingConventions">The calling convention used</param>
        /// <returns>Constructor builder for the constructor</returns>
        public virtual IMethodBuilder CreateConstructor(MethodAttributes Attributes = MethodAttributes.Public,
            IEnumerable<Type> ParameterTypes = null, CallingConventions CallingConventions = CallingConventions.Standard)
        {
            ConstructorBuilder ReturnValue = new ConstructorBuilder(this, Attributes, ParameterTypes, CallingConventions);
            Constructors.Add(ReturnValue);
            return ReturnValue;
        }

        #endregion

        #region CreateDefaultConstructor

        /// <summary>
        /// Creates a default constructor
        /// </summary>
        /// <param name="Attributes">Attributes for the constructor (public, private, etc.)</param>
        /// <returns>Constructor builder for the constructor</returns>
        public virtual IMethodBuilder CreateDefaultConstructor(MethodAttributes Attributes = MethodAttributes.Public)
        {
            DefaultConstructorBuilder ReturnValue = new DefaultConstructorBuilder(this, Attributes);
            Constructors.Add(ReturnValue);
            return ReturnValue;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// The type defined by this TypeBuilder (filled once Create is called)
        /// </summary>
        public Type DefinedType { get; protected set; }

        /// <summary>
        /// List of methods defined by this TypeBuilder 
        /// (does not include methods defined in base classes unless overridden)
        /// </summary>
        public ICollection<IMethodBuilder> Methods { get; private set; }

        /// <summary>
        /// List of fields defined by the TypeBuilder
        /// (does not include fields defined in base classes)
        /// </summary>
        public ICollection<FieldBuilder> Fields { get; private set; }

        /// <summary>
        /// List of properties defined by the TypeBuilder
        /// (does not include properties defined in base classes)
        /// </summary>
        public ICollection<IPropertyBuilder> Properties { get; private set; }

        /// <summary>
        /// Constructors defined by the TypeBuilder
        /// </summary>
        public ICollection<IMethodBuilder> Constructors { get; private set; }

        /// <summary>
        /// List of interfaces used by this type
        /// </summary>
        public ICollection<Type> Interfaces { get; private set; }

        /// <summary>
        /// Base class used by this type
        /// </summary>
        public Type BaseClass { get; protected set; }

        /// <summary>
        /// Builder used by this type
        /// </summary>
        public System.Reflection.Emit.TypeBuilder Builder { get; protected set; }

        /// <summary>
        /// TypeAttributes for this type
        /// </summary>
        public TypeAttributes Attributes { get; protected set; }

        /// <summary>
        /// Name of this type
        /// </summary>
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Assembly builder
        /// </summary>
        protected Assembly Assembly { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        /// Writes out the type builder to a readable string
        /// </summary>
        /// <returns>Code version of the type builder</returns>
        public override string ToString()
        {
            string[] Splitter = { "." };
            string[] NameParts = Name.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder Output = new StringBuilder();
            Output.Append("namespace ").Append(Assembly.Name);
            for (int x = 0; x < NameParts.Length - 1; ++x)
                Output.Append(".").Append(NameParts[x]);
            Output.Append("\n{\n");
            Output.Append((Attributes & TypeAttributes.Public) > 0 ? "public " : "private ");
            Output.Append("class ");
            Output.Append(NameParts[NameParts.Length - 1]);
            string Seperator = " : ";
            if (BaseClass != null)
            {
                Output.Append(Seperator).Append(BaseClass.Name);
                Seperator = ", ";
            }
            foreach (Type Interface in Interfaces)
            {
                Output.Append(Seperator).Append(Interface.Name);
                Seperator = ", ";
            }
            Output.Append("\n{");
            Constructors.ForEach(x => Output.Append(x.ToString()));
            Methods.ForEach(x => Output.Append(x.ToString()));
            Properties.ForEach(x => Output.Append(x.GetDefinition()));
            Fields.ForEach(x => Output.Append(x.GetDefinition()));
            Output.Append("\n}\n}\n\n");
            return Output.ToString();
        }

        #endregion
    }
}