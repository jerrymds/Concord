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
using System.Text;
using Company.Utilities.Reflection.Emit.BaseClasses;
using Company.Utilities.Reflection.Emit.Interfaces;
#endregion

namespace Company.Utilities.Reflection.Emit.Commands
{
    /// <summary>
    /// Return command
    /// </summary>
    public class Return : CommandBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ReturnType">Return type</param>
        /// <param name="ReturnValue">Return value</param>
        public Return(Type ReturnType, object ReturnValue)
            : base()
        {
            this.ReturnType = ReturnType;
            VariableBase TempReturnValue = ReturnValue as VariableBase;
            this.ReturnValue = ReturnValue == null || TempReturnValue == null ? new ConstantBuilder(ReturnValue) : TempReturnValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Return type
        /// </summary>
        protected virtual Type ReturnType { get; set; }

        /// <summary>
        /// Return value
        /// </summary>
        protected virtual VariableBase ReturnValue { get; set; }

        #endregion

        #region Function

        /// <summary>
        /// Sets up the command
        /// </summary>
        public override void Setup()
        {
            ILGenerator Generator = Company.Utilities.Reflection.Emit.BaseClasses.MethodBase.CurrentMethod.Generator;
            if (ReturnType == typeof(void) || ReturnType == null)
            {
                Generator.Emit(OpCodes.Ret);
                return;
            }
            if (ReturnValue is FieldBuilder || ReturnValue is IPropertyBuilder)
                Generator.Emit(OpCodes.Ldarg_0);
            ReturnValue.Load(Generator);
            Generator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            StringBuilder Output = new StringBuilder();
            if (ReturnType != null && ReturnType != typeof(void))
            {
                Output.Append("return ").Append(ReturnValue).Append(";\n");
            }
            return Output.ToString();
        }

        #endregion
    }
}