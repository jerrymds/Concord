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
using System.Text;
using Company.Utilities.FileFormats.Cisco.Interfaces;
#endregion

namespace Company.Utilities.FileFormats.Cisco
{
    /// <summary>
    /// Graphic menu item
    /// </summary>
    public class GraphicMenuItem : IMenuItem
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public GraphicMenuItem()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the Graphic menu
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Invoked when area touched
        /// </summary>
        public virtual string URL { get; set; }

        /// <summary>
        /// Left
        /// </summary>
        public virtual int X1 { get; set; }

        /// <summary>
        /// Right
        /// </summary>
        public virtual int X2 { get; set; }

        /// <summary>
        /// Top
        /// </summary>
        public virtual int Y1 { get; set; }

        /// <summary>
        /// Bottom
        /// </summary>
        public virtual int Y2 { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        /// Exports the item as a properly formatted string
        /// </summary>
        /// <returns>The properly formatted string</returns>
        public override string ToString()
        {
            StringBuilder Builder = new StringBuilder();
            Builder.Append("<MenuItem><Name>").Append(Name).Append("</Name><URL>").Append(URL).Append("</URL<TouchArea X1=\"")
                .Append(X1).Append("\" Y1=\"").Append(Y1).Append("\" X2=\"").Append(X2).Append("\" Y2=\"").Append(Y2)
                .Append("\" /></MenuItem>");
            return Builder.ToString();
        }

        #endregion
    }
}