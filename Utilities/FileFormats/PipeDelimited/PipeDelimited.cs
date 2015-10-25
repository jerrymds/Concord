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

namespace Company.Utilities.FileFormats.PipeDelimited
{
    /// <summary>
    /// Pipe delimited loader
    /// </summary>
    public class PipeDelimited : Delimited.Delimited
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PipeDelimited() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileContent">File content</param>
        public PipeDelimited(string FileContent) : base(FileContent) { }

        #endregion

        #region Properties

        /// <summary>
        /// Delimiter used
        /// </summary>
        protected override string Delimiter { get { return "|"; } }

        #endregion
    }
}