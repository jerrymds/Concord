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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Company.Utilities.IO.ExtensionMethods;


#endregion

namespace Company.Utilities.FileFormats.INI
{
    /// <summary>
    /// Class for helping with INI files
    /// </summary>
    public class INI
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public INI()
        {
            LoadFile();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileName">Name of the file</param>
        public INI(string FileName)
        {
            this.FileName = FileName;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Writes a change to an INI file
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <param name="Value">Value</param>
        public virtual void WriteToINI(string Section, string Key, string Value)
        {
            if (FileContents.Keys.Contains(Section))
            {
                if (FileContents[Section].Keys.Contains(Key))
                {
                    FileContents[Section][Key] = Value;
                }
                else
                {
                    FileContents[Section].Add(Key, Value);
                }
            }
            else
            {
                Dictionary<string, string> TempDictionary = new Dictionary<string, string>();
                TempDictionary.Add(Key, Value);
                FileContents.Add(Section, TempDictionary);
            }
            WriteFile();
        }

        /// <summary>
        /// Reads a value from an INI file
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <param name="DefaultValue">Default value if it does not exist</param>
        public virtual string ReadFromINI(string Section, string Key, string DefaultValue="")
        {
            if (FileContents.Keys.Contains(Section) && FileContents[Section].Keys.Contains(Key))
                return FileContents[Section][Key];
            return DefaultValue;
        }

        /// <summary>
        /// Returns an XML representation of the INI file
        /// </summary>
        /// <returns>An XML representation of the INI file</returns>
        public virtual string ToXML()
        {
            if (string.IsNullOrEmpty(this.FileName))
                return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<INI>\r\n</INI>";
            StringBuilder Builder = new StringBuilder();
            Builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
            Builder.Append("<INI>\r\n");
            foreach (string Header in FileContents.Keys)
            {
                Builder.Append("<section name=\"" + Header + "\">\r\n");
                foreach (string Key in FileContents[Header].Keys)
                {
                    Builder.Append("<key name=\"" + Key + "\">" + FileContents[Header][Key] + "</key>\r\n");
                }
                Builder.Append("</section>\r\n");
            }
            Builder.Append("</INI>");
            return Builder.ToString();
        }

        /// <summary>
        /// Deletes a section from the INI file
        /// </summary>
        /// <param name="Section">Section to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public virtual bool DeleteFromINI(string Section)
        {
            bool ReturnValue = false;
            if (FileContents.ContainsKey(Section))
            {
                ReturnValue = FileContents.Remove(Section);
                WriteFile();
            }
            return ReturnValue;
        }

        /// <summary>
        /// Deletes a key from the INI file
        /// </summary>
        /// <param name="Section">Section the key is under</param>
        /// <param name="Key">Key to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public virtual bool DeleteFromINI(string Section, string Key)
        {
            bool ReturnValue = false;
            if (FileContents.ContainsKey(Section) && FileContents[Section].ContainsKey(Key))
            {
                ReturnValue = FileContents[Section].Remove(Key);
                WriteFile();
            }
            return ReturnValue;
        }

        /// <summary>
        /// Convert the INI to a string
        /// </summary>
        /// <returns>The INI file as a string</returns>
        public override string ToString()
        {
            StringBuilder Builder = new StringBuilder();
            foreach (string Header in FileContents.Keys)
            {
                Builder.Append("[" + Header + "]\r\n");
                foreach (string Key in FileContents[Header].Keys)
                    Builder.Append(Key + "=" + FileContents[Header][Key] + "\r\n");
            }
            return Builder.ToString();
        }

        #endregion

        #region Private Functions
        /// <summary>
        /// Writes the INI information to a file
        /// </summary>
        private void WriteFile()
        {
            if (string.IsNullOrEmpty(this.FileName))
                return;
            new FileInfo(FileName).Save(this.ToString());
        }

        /// <summary>
        /// Loads an INI file
        /// </summary>
        private void LoadFile()
        {
            FileContents = new Dictionary<string, Dictionary<string, string>>();
            if (string.IsNullOrEmpty(this.FileName))
                return;

            string Contents = new FileInfo(FileName).Read();
            Regex Section = new Regex("[" + Regex.Escape(" ") + "\t]*" + Regex.Escape("[") + ".*" + Regex.Escape("]\r\n"));
            string[] Sections = Section.Split(Contents);
            MatchCollection SectionHeaders = Section.Matches(Contents);
            int Counter = 1;
            foreach (Match SectionHeader in SectionHeaders)
            {
                string[] Splitter = { "\r\n" };
                string[] Splitter2 = { "=" };
                string[] Items = Sections[Counter].Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string> SectionValues = new Dictionary<string, string>();
                foreach (string Item in Items)
                {
                    SectionValues.Add(Item.Split(Splitter2, StringSplitOptions.None)[0], Item.Split(Splitter2, StringSplitOptions.None)[1]);
                }
                FileContents.Add(SectionHeader.Value.Replace("[", "").Replace("]\r\n", ""), SectionValues);
                ++Counter;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Name of the file
        /// </summary>
        public virtual string FileName
        {
            get { return _FileName; }
            set { _FileName = value; LoadFile(); }
        }

        private Dictionary<string, Dictionary<string, string>> FileContents { get; set; }
        private string _FileName = string.Empty;

        #endregion
    }
}