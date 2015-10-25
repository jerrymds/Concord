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
using System.Linq;
using Company.Utilities.DataMapper.Interfaces;
#endregion

namespace Company.Utilities.DataMapper
{
    /// <summary>
    /// Mapping manager
    /// </summary>
    public class MappingManager
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MappingManager() { Mappings = new List<ITypeMapping>(); }

        #endregion

        #region Properties

        /// <summary>
        /// Mappings
        /// </summary>
        protected virtual ICollection<ITypeMapping> Mappings { get;private set; }

        #endregion

        #region Functions

        /// <summary>
        /// Adds or returns a mapping between two types
        /// </summary>
        /// <typeparam name="Left">Left type</typeparam>
        /// <typeparam name="Right">Right type</typeparam>
        /// <returns>A mapping object for the two types specified</returns>
        public virtual TypeMapping<Left, Right> Map<Left, Right>()
        {
            IEnumerable<TypeMapping<Left, Right>> MappingsList = Mappings.OfType<TypeMapping<Left, Right>>();
            if (MappingsList.Count() > 0)
                return MappingsList.ElementAt(0);
            TypeMapping<Left, Right> Mapping = new TypeMapping<Left, Right>();
            Mappings.Add(Mapping);
            return Mapping;
        }

        #endregion
    }
}
