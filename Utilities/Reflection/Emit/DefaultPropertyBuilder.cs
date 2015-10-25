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
using Company.Utilities.Reflection.Emit.BaseClasses;
using Company.Utilities.Reflection.Emit.Interfaces;
using Company.Utilities.Reflection.ExtensionMethods;
using System.Linq;
#endregion

namespace Company.Utilities.Reflection.Emit
{
    /// <summary>
    /// Helper class for defining default properties
    /// </summary>
    public class DefaultPropertyBuilder : VariableBase, IPropertyBuilder
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TypeBuilder">Type builder</param>
        /// <param name="Name">Name of the property</param>
        /// <param name="Attributes">Attributes for the property (public, private, etc.)</param>
        /// <param name="GetMethodAttributes">Get method attributes</param>
        /// <param name="SetMethodAttributes">Set method attributes</param>
        /// <param name="PropertyType">Property type for the property</param>
        /// <param name="Parameters">Parameter types for the property</param>
        public DefaultPropertyBuilder(TypeBuilder TypeBuilder, string Name,
            PropertyAttributes Attributes, MethodAttributes GetMethodAttributes,
            MethodAttributes SetMethodAttributes,
            Type PropertyType, IEnumerable<Type> Parameters)
            : base()
        {
            if (TypeBuilder == null)
                throw new ArgumentNullException("TypeBuilder");
            if (string.IsNullOrEmpty(Name))
                throw new ArgumentNullException("Name");
            this.Name = Name;
            this.Type = TypeBuilder;
            this.Attributes = Attributes;
            this.GetMethodAttributes = GetMethodAttributes;
            this.SetMethodAttributes = SetMethodAttributes;
            this.DataType = PropertyType;
            this.Parameters = new List<ParameterBuilder>();
            if (Parameters != null)
            {
                int x = 1;
                foreach (Type Parameter in Parameters)
                {
                    this.Parameters.Add(new ParameterBuilder(Parameter, x));
                    ++x;
                }
            }
            Field = new FieldBuilder(Type, "_" + Name + "field", PropertyType, FieldAttributes.Private);
            Builder = Type.Builder.DefineProperty(Name, Attributes, PropertyType,
                (Parameters != null && Parameters.Count() > 0) ? Parameters.ToArray() : System.Type.EmptyTypes);
            GetMethod = new MethodBuilder(Type, "get_" + Name, GetMethodAttributes, Parameters, PropertyType);
            GetMethod.Generator.Emit(OpCodes.Ldarg_0);
            GetMethod.Generator.Emit(OpCodes.Ldfld, Field.Builder);
            GetMethod.Generator.Emit(OpCodes.Ret);
            List<Type> SetParameters = new List<System.Type>();
            if (Parameters != null)
            {
                SetParameters.AddRange(Parameters);
            }
            SetParameters.Add(PropertyType);
            SetMethod = new MethodBuilder(Type, "set_" + Name, SetMethodAttributes, SetParameters, typeof(void));
            SetMethod.Generator.Emit(OpCodes.Ldarg_0);
            SetMethod.Generator.Emit(OpCodes.Ldarg_1);
            SetMethod.Generator.Emit(OpCodes.Stfld, Field.Builder);
            SetMethod.Generator.Emit(OpCodes.Ret);
            Builder.SetGetMethod(GetMethod.Builder);
            Builder.SetSetMethod(SetMethod.Builder);
        }

        #endregion

        #region Functions

        /// <summary>
        /// Loads the property
        /// </summary>
        /// <param name="Generator">IL Generator</param>
        public override void Load(ILGenerator Generator)
        {
            if (GetMethod.Builder.IsVirtual)
                Generator.EmitCall(OpCodes.Callvirt, GetMethod.Builder, null);
            else
                Generator.EmitCall(OpCodes.Call, GetMethod.Builder, null);
        }

        /// <summary>
        /// Saves the property
        /// </summary>
        /// <param name="Generator">IL Generator</param>
        public override void Save(ILGenerator Generator)
        {
            if (SetMethod.Builder.IsVirtual)
                Generator.EmitCall(OpCodes.Callvirt, SetMethod.Builder, null);
            else
                Generator.EmitCall(OpCodes.Call, SetMethod.Builder, null);
        }

        /// <summary>
        /// Gets the definition of the property
        /// </summary>
        /// <returns>The definition of the property</returns>
        public override string GetDefinition()
        {
            StringBuilder Output = new StringBuilder();

            Output.Append("\n");
            if ((GetMethodAttributes & MethodAttributes.Public) > 0)
                Output.Append("public ");
            else if ((GetMethodAttributes & MethodAttributes.Private) > 0)
                Output.Append("private ");
            if ((GetMethodAttributes & MethodAttributes.Static) > 0)
                Output.Append("static ");
            if ((GetMethodAttributes & MethodAttributes.Virtual) > 0)
                Output.Append("virtual ");
            else if ((GetMethodAttributes & MethodAttributes.Abstract) > 0)
                Output.Append("abstract ");
            else if ((GetMethodAttributes & MethodAttributes.HideBySig) > 0)
                Output.Append("override ");
            Output.Append(DataType.GetName());
            Output.Append(" ").Append(Name);

            string Splitter = "";
            if (Parameters != null && Parameters.Count > 0)
            {
                Output.Append("[");
                foreach (ParameterBuilder Parameter in Parameters)
                {
                    Output.Append(Splitter).Append(Parameter.GetDefinition());
                    Splitter = ",";
                }
                Output.Append("]");
            }
            Output.Append(" { get; ");
            if ((SetMethodAttributes & GetMethodAttributes) != SetMethodAttributes)
            {
                if ((SetMethodAttributes & MethodAttributes.Public) > 0)
                    Output.Append("public ");
                else if ((SetMethodAttributes & MethodAttributes.Private) > 0)
                    Output.Append("private ");
            }
            Output.Append("set; }\n");

            return Output.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Parameter types
        /// </summary>
        public ICollection<ParameterBuilder> Parameters { get; private set; }

        /// <summary>
        /// Method builder
        /// </summary>
        public System.Reflection.Emit.PropertyBuilder Builder { get; protected set; }

        /// <summary>
        /// Attributes for the property
        /// </summary>
        public System.Reflection.PropertyAttributes Attributes { get; protected set; }

        /// <summary>
        /// Attributes for the get method
        /// </summary>
        public System.Reflection.MethodAttributes GetMethodAttributes { get; protected set; }

        /// <summary>
        /// Attributes for the set method
        /// </summary>
        public System.Reflection.MethodAttributes SetMethodAttributes { get; protected set; }

        /// <summary>
        /// Method builder for the get method
        /// </summary>
        public MethodBuilder GetMethod { get; protected set; }

        /// <summary>
        /// Method builder for the set method
        /// </summary>
        public MethodBuilder SetMethod { get; protected set; }

        /// <summary>
        /// Field builder
        /// </summary>
        public FieldBuilder Field { get; protected set; }

        /// <summary>
        /// Type builder
        /// </summary>
        protected TypeBuilder Type { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        /// property as a string
        /// </summary>
        /// <returns>Property as a string</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Operator Functions

        /// <summary>
        /// Increments the property by one
        /// </summary>
        /// <param name="Left">The property to increment</param>
        /// <returns>The property</returns>
        public static DefaultPropertyBuilder operator ++(DefaultPropertyBuilder Left)
        {
            if (Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            Left.Assign(Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod.Add(Left, 1));
            return Left;
        }

        /// <summary>
        /// Decrements the property by one
        /// </summary>
        /// <param name="Left">The property to decrement</param>
        /// <returns>The property</returns>
        public static DefaultPropertyBuilder operator --(DefaultPropertyBuilder Left)
        {
            if (Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            Left.Assign(Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod.Subtract(Left, 1));
            return Left;
        }

        #endregion
    }
}