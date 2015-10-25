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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
#endregion

namespace Company.Utilities.FileFormats.Delimited
{
    /// <summary>
    /// Individual row within a delimited file
    /// </summary>
    public class Row
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Row()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Delimiter">Delimiter to parse the individual cells</param>
        public Row(string Delimiter)
        {
            this.Cells = new List<Cell>();
            this.Delimiter = Delimiter;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Content">Content of the row</param>
        /// <param name="Delimiter">Delimiter to parse the individual cells</param>
        public Row(string Content, string Delimiter)
        {
            this.Cells = new List<Cell>();
            this.Delimiter = Delimiter;
            Regex TempSplitter = new Regex(string.Format(CultureInfo.InvariantCulture, "(?<Value>\"(?:[^\"]|\"\")*\"|[^{0}\r\n]*?)(?<Delimiter>{0}|\r\n|\n|$)", Regex.Escape(Delimiter)));
            MatchCollection Matches = TempSplitter.Matches(Content);
            bool Finished = false;
            foreach (Match Match in Matches)
            {
                if (!Finished)
                {
                    Cells.Add(new Cell(Match.Groups["Value"].Value));
                }
                Finished = string.IsNullOrEmpty(Match.Groups["Delimiter"].Value) || Match.Groups["Delimiter"].Value == "\r\n" || Match.Groups["Delimiter"].Value == "\n";
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Cells within the row
        /// </summary>
        public ICollection<Cell> Cells { get; private set; }

        /// <summary>
        /// Returns a cell within the row
        /// </summary>
        /// <param name="Position">The position of the cell</param>
        /// <returns>The specified cell</returns>
        public virtual Cell this[int Position]
        {
            get { return Cells.ElementAt(Position); }
        }

        /// <summary>
        /// Number of cells within the row
        /// </summary>
        public virtual int NumberOfCells
        {
            get { return Cells.Count; }
        }

        /// <summary>
        /// Delimiter used
        /// </summary>
        protected virtual string Delimiter { get; set; }

        #endregion

        #region Public Overridden Functions

        /// <summary>
        /// To string function
        /// </summary>
        /// <returns>The content of the row in string form</returns>
        public override string ToString()
        {
            StringBuilder Builder = new StringBuilder();
            string Seperator = "";
            foreach (Cell CurrentCell in Cells)
            {
                Builder.Append(Seperator).Append(CurrentCell);
                Seperator = Delimiter;
            }
            return Builder.Append(System.Environment.NewLine).ToString();
        }

        #endregion
    }
}