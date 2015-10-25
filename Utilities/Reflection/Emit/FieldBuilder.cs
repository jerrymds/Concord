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
using System.Reflection.Emit;
using System.Text;
using Company.Utilities.Reflection.Emit.BaseClasses;
using Company.Utilities.Reflection.ExtensionMethods;
#endregion

namespace Company.Utilities.Reflection.Emit
{
    /// <summary>
    /// Helper class for defining a field within a type
    /// </summary>
    public class FieldBuilder : VariableBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TypeBuilder">Type builder</param>
        /// <param name="Name">Name of the method</param>
        /// <param name="Attributes">Attributes for the field (public, private, etc.)</param>
        /// <param name="FieldType">Type for the field</param>
        public FieldBuilder(TypeBuilder TypeBuilder, string Name, Type FieldType, FieldAttributes Attributes)
            : base()
        {
            if (TypeBuilder == null)
                throw new ArgumentNullException("TypeBuilder");
            if (string.IsNullOrEmpty(Name))
                throw new ArgumentNullException("Name");
            this.Name = Name;
            this.Type = TypeBuilder;
            this.DataType = FieldType;
            this.Attributes = Attributes;
            Builder = Type.Builder.DefineField(Name, FieldType, Attributes);
        }

        #endregion

        #region Functions

        /// <summary>
        /// Loads the field
        /// </summary>
        /// <param name="Generator">IL Generator</param>
        public override void Load(System.Reflection.Emit.ILGenerator Generator)
        {
            Generator.Emit(OpCodes.Ldfld, Builder);
        }

        /// <summary>
        /// Saves the field
        /// </summary>
        /// <param name="Generator">IL Generator</param>
        public override void Save(System.Reflection.Emit.ILGenerator Generator)
        {
            Generator.Emit(OpCodes.Stfld, Builder);
        }

        /// <summary>
        /// Gets the definition of the field
        /// </summary>
        /// <returns>The field's definition</returns>
        public override string GetDefinition()
        {
            StringBuilder Output = new StringBuilder();

            Output.Append("\n");
            if ((Attributes & FieldAttributes.Public) > 0)
                Output.Append("public ");
            else if ((Attributes & FieldAttributes.Private) > 0)
                Output.Append("private ");
            if ((Attributes & FieldAttributes.Static) > 0)
                Output.Append("static ");
            Output.Append(DataType.GetName());
            Output.Append(" ").Append(Name).Append(";");

            return Output.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field builder
        /// </summary>
        public virtual System.Reflection.Emit.FieldBuilder Builder { get; protected set; }

        /// <summary>
        /// Attributes for the field (private, public, etc.)
        /// </summary>
        public virtual FieldAttributes Attributes { get; protected set; }

        /// <summary>
        /// Type builder
        /// </summary>
        protected virtual TypeBuilder Type { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        /// The field as a string
        /// </summary>
        /// <returns>The field as a string</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Operator Functions

        /// <summary>
        /// Increments the field by one
        /// </summary>
        /// <param name="Left">Field to increment</param>
        /// <returns>The field</returns>
        public static FieldBuilder operator ++(FieldBuilder Left)
        {
            if (Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            Left.Assign(Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod.Add(Left, 1));
            return Left;
        }

        /// <summary>
        /// Decrements the field by one
        /// </summary>
        /// <param name="Left">Field to decrement</param>
        /// <returns>The field</returns>
        public static FieldBuilder operator --(FieldBuilder Left)
        {
            if (Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            Left.Assign(Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod.Subtract(Left, 1));
            return Left;
        }

        #endregion
    }
}