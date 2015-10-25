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
using System.Text;
using System.Xml;
using Company.Utilities.DataTypes.ExtensionMethods;
using System.Globalization;
#endregion

namespace Company.Utilities.FileFormats.BlogML
{
    /// <summary>
    /// Individual author
    /// </summary>
    public class Author
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Author()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Element">XML element containing the author info</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public Author(XmlElement Element)
        {
            Element.ThrowIfNull("Element");
            ID = Element.Attributes["id"] != null ? Element.Attributes["id"].Value : "";
            Email = Element.Attributes["email"] != null ? Element.Attributes["email"].Value : "";
            REF = Element.Attributes["ref"] != null ? Element.Attributes["ref"].Value : "";
            DateCreated = Element.Attributes["date-created"] != null ? DateTime.Parse(Element.Attributes["date-created"].Value, CultureInfo.InvariantCulture) : DateTime.Now;
            DateModified = Element.Attributes["date-modified"] != null ? DateTime.Parse(Element.Attributes["date-modified"].Value, CultureInfo.InvariantCulture) : DateTime.Now;
            foreach (XmlNode Children in Element.ChildNodes)
            {
                if (Children.Name.Equals("title", StringComparison.CurrentCultureIgnoreCase))
                    Title = Children.InnerText;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// ID of the author
        /// </summary>
        public virtual string ID { get; set; }

        /// <summary>
        /// Email address of the author
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// The person's title (most likely their name)
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Determines if this should be a reference to an author
        /// </summary>
        public virtual string REF { get; set; }

        /// <summary>
        /// Date created
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Date modified
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        /// Converts the author to a string
        /// </summary>
        /// <returns>The author as a string</returns>
        public override string ToString()
        {
            StringBuilder Builder = new StringBuilder();
            Builder.Append("<author ");
            if (REF.IsNullOrEmpty())
            {
                Builder.AppendFormat(CultureInfo.InvariantCulture, "id=\"{0}\" date-created=\"{1}\" date-modified=\"{2}\" approved=\"true\" email=\"{3}\">\n", ID, DateCreated.ToString("yyyy-MM-ddThh:mm:ss", CultureInfo.InvariantCulture), DateModified.ToString("yyyy-MM-ddThh:mm:ss", CultureInfo.InvariantCulture), Email);
                Builder.AppendFormat(CultureInfo.InvariantCulture, "<title type=\"text\"><![CDATA[{0}]]></title>\n", Title);
                Builder.AppendLine("</author>");
            }
            else
            {
                Builder.AppendFormat("ref=\"{0}\" />\n", REF);
            }
            return Builder.ToString();
        }

        #endregion
    }
}