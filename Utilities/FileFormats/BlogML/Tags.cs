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
using System.Text;
using System.Xml;
using Company.Utilities.DataTypes.ExtensionMethods;
using System.Linq;
#endregion

namespace Company.Utilities.FileFormats.BlogML
{
    /// <summary>
    /// Tags information
    /// </summary>
    public class Tags
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Tags()
        {
            TagList = new List<Tag>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Element">Element containing tags info</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public Tags(XmlElement Element)
        {
            Element.ThrowIfNull("Element");
            TagList = new List<Tag>();
            foreach (XmlNode Children in Element.ChildNodes)
            {
                if (Children.Name.Equals("tag", StringComparison.CurrentCultureIgnoreCase))
                    TagList.Add(new Tag((XmlElement)Children));
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Tags list
        /// </summary>
        public ICollection<Tag> TagList { get; private set; }

        /// <summary>
        /// gets a specific tag
        /// </summary>
        /// <param name="index">Index of the tag</param>
        /// <returns>A specific tag</returns>
        public Tag this[int index]
        {
            get { return TagList.ElementAt(index); }
        }

        #endregion

        #region Overridden Functions

        /// <summary>
        /// Converts the object to a string
        /// </summary>
        /// <returns>The object as a string</returns>
        public override string ToString()
        {
            StringBuilder Builder = new StringBuilder();
            Builder.AppendLine("<tags>");
            foreach (Tag Tag in TagList)
            {
                Builder.AppendLine(Tag.ToString());
            }
            Builder.AppendLine("</tags>");
            return Builder.ToString();
        }

        #endregion
    }
}