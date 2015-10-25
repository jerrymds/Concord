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
using System.Reflection.Emit;
using Company.Utilities.Reflection.Emit.BaseClasses;
using Company.Utilities.Reflection.Emit.Interfaces;
#endregion

namespace Company.Utilities.Reflection.Emit
{
    /// <summary>
    /// Helper class for defining a local variable
    /// </summary>
    public class LocalBuilder : VariableBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MethodBuilder">Method builder</param>
        /// <param name="Name">Name of the local</param>
        /// <param name="LocalType">Type of the local</param>
        public LocalBuilder(IMethodBuilder MethodBuilder, string Name, Type LocalType)
            : base()
        {
            if (MethodBuilder == null)
                throw new ArgumentNullException("MethodBuilder");
            if (string.IsNullOrEmpty(Name))
                throw new ArgumentNullException("Name");
            this.Name = Name;
            this.MethodBuilder = MethodBuilder;
            this.DataType = LocalType;
            Builder = MethodBuilder.Generator.DeclareLocal(LocalType);
        }

        #endregion

        #region Functions

        /// <summary>
        /// Loads the local object
        /// </summary>
        /// <param name="Generator">IL Generator</param>
        public override void Load(ILGenerator Generator)
        {
            Generator.Emit(OpCodes.Ldloc, Builder);
        }

        /// <summary>
        /// Saves the local object
        /// </summary>
        /// <param name="Generator">IL Generator</param>
        public override void Save(ILGenerator Generator)
        {
            Generator.Emit(OpCodes.Stloc, Builder);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Local builder
        /// </summary>
        public virtual System.Reflection.Emit.LocalBuilder Builder { get; protected set; }

        /// <summary>
        /// Method builder
        /// </summary>
        protected virtual IMethodBuilder MethodBuilder { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        /// The local item as a string
        /// </summary>
        /// <returns>The local item as a string</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Operator Functions

        /// <summary>
        /// Increments the local object by one
        /// </summary>
        /// <param name="Left">Local object to increment</param>
        /// <returns>The local object</returns>
        public static LocalBuilder operator ++(LocalBuilder Left)
        {
            if (Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            Left.Assign(Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod.Add(Left, 1));
            return Left;
        }

        /// <summary>
        /// Decrements the local object by one
        /// </summary>
        /// <param name="Left">Local object to decrement</param>
        /// <returns>The local object</returns>
        public static LocalBuilder operator --(LocalBuilder Left)
        {
            if (Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            Left.Assign(Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod.Subtract(Left, 1));
            return Left;
        }

        #endregion
    }
}