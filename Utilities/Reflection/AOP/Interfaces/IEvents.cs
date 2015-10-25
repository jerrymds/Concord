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
using Company.Utilities.Reflection.AOP.EventArgs;
#endregion

namespace Company.Utilities.Reflection.AOP.Interfaces
{
    /// <summary>
    /// Events interface (injected into all objects)
    /// </summary>
    public interface IEvents
    {
        #region Events

        /// <summary>
        /// Called when property/function is ending
        /// </summary>
        EventHandler<Ending> Aspectus_Ending { get; set; }

        /// <summary>
        /// Called when property/function is starting
        /// </summary>
        EventHandler<Starting> Aspectus_Starting { get; set; }

        /// <summary>
        /// Called when an error is caught
        /// </summary>
        EventHandler<Company.Utilities.Reflection.AOP.EventArgs.Exception> Aspectus_Exception { get; set; }

        #endregion
    }
}