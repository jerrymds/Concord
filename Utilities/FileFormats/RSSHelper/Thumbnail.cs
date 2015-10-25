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
using System.Xml;
using System.Globalization;
#endregion

namespace Company.Utilities.FileFormats.RSSHelper
{
    /// <summary>
    /// Thumbnail info holder
    /// </summary>
    public class Thumbnail
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Thumbnail()
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Element">XML element holding info for the enclosure</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public Thumbnail(XmlElement Element)
        {
            if (Element == null)
                throw new ArgumentNullException("Element");
            if (!Element.Name.Equals("media:thumbnail", StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("Element is not a thumbnail");
            if (Element.Attributes["url"] != null)
            {
                Url = Element.Attributes["url"].Value;
            }
            if (Element.Attributes["width"] != null)
            {
                Width = int.Parse(Element.Attributes["width"].Value, CultureInfo.InvariantCulture);
            }
            if (Element.Attributes["height"] != null)
            {
                Height = int.Parse(Element.Attributes["height"].Value, CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Location of the item
        /// </summary>
        public virtual string Url { get; set; }

        /// <summary>
        /// Image width
        /// </summary>
        public virtual int Width { get; set; }

        /// <summary>
        /// Image height
        /// </summary>
        public virtual int Height { get; set; }

        #endregion

        #region Public Overridden Functions

        /// <summary>
        /// to string item. Used for outputting the item to RSS.
        /// </summary>
        /// <returns>A string formatted for RSS output</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                return "<media:thumbnail url=\"" + Url + "\" width=\"" + Width.ToString(CultureInfo.InvariantCulture) + "\" height=\"" + Height + "\" />\r\n";
            }
            return string.Empty;
        }

        #endregion
    }
}