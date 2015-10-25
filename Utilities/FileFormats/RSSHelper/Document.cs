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
using System.Diagnostics.CodeAnalysis;
using Company.Utilities.DataTypes.ExtensionMethods;
using System.Linq;
#endregion

namespace Company.Utilities.FileFormats.RSSHelper
{
    /// <summary>
    /// RSS document class
    /// </summary>
    public class Document
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Document()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Location">Location of the RSS feed to load</param>
        public Document(string Location)
        {
            if (string.IsNullOrEmpty(Location))
                throw new ArgumentNullException("Location");
            XmlDocument Document = new XmlDocument();
            Document.Load(Location);
            Load(Document);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Document">XML document containing an RSS feed</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public Document(XmlDocument Document)
        {
            if (Document == null)
                throw new ArgumentNullException("Document");
            Load(Document);
        }

        #endregion

        #region Private Variables

        private ICollection<Channel> _Channels = null;

        #endregion

        #region Properties

        /// <summary>
        /// Channels for the RSS feed
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Channel> Channels
        {
            get
            {
                if (_Channels == null)
                {
                    _Channels = new List<Channel>();
                }
                return _Channels;
            }
            set { _Channels = value; }
        }

        #endregion

        #region Public Overridden Functions

        /// <summary>
        /// string representation of the RSS feed.
        /// </summary>
        /// <returns>An rss formatted string</returns>
        public override string ToString()
        {
            StringBuilder DocumentString = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<rss xmlns:itunes=\"http://www.itunes.com/dtds/podcast-1.0.dtd\" xmlns:media=\"http://search.yahoo.com/mrss/\" version=\"2.0\">\r\n");
            foreach (Channel CurrentChannel in Channels)
            {
                DocumentString.Append(CurrentChannel.ToString());
            }
            DocumentString.Append("</rss>");
            return DocumentString.ToString();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Copies one document's channels to another
        /// </summary>
        /// <param name="CopyFrom">RSS document to copy from</param>
        public virtual void Copy(Document CopyFrom)
        {
            if (CopyFrom == null)
                throw new ArgumentNullException("CopyFrom");
            foreach (Channel CurrentChannel in CopyFrom.Channels)
            {
                Channels.Add(CurrentChannel);
            }
        }

        #endregion

        #region Private Functions

        private void Load(XmlDocument Document)
        {
            if (Document == null)
                throw new ArgumentNullException("Document");
            XmlNamespaceManager NamespaceManager = new XmlNamespaceManager(Document.NameTable);
            XmlNodeList Nodes = Document.DocumentElement.SelectNodes("./channel", NamespaceManager);
            foreach (XmlNode Element in Nodes)
            {
                Channels.Add(new Channel((XmlElement)Element));
            }
            if (Channels.Count == 0)
            {
                Nodes = Document.DocumentElement.SelectNodes(".//channel", NamespaceManager);
                foreach (XmlNode Element in Nodes)
                {
                    Channels.Add(new Channel((XmlElement)Element));
                }
                List<Item> Items = new List<Item>();
                Nodes = Document.DocumentElement.SelectNodes(".//item", NamespaceManager);
                foreach (XmlNode Element in Nodes)
                {
                    Items.Add(new Item((XmlElement)Element));
                }
                if (Channels.Count > 0)
                {
                    Channels.FirstOrDefault().Items = Items;
                }
            }
        }

        #endregion
    }
}