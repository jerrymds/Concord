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
using System.Linq;
using Company.Utilities.DataTypes;
#endregion

namespace Company.Utilities.Math.ExtensionMethods
{
    /// <summary>
    /// Permutation extensions
    /// </summary>
    public static class PermutationExtensions
    {
        #region Functions

        #region Permute

        /// <summary>
        /// Finds all permutations of the items within the list
        /// </summary>
        /// <typeparam name="T">Object type in the list</typeparam>
        /// <param name="Input">Input list</param>
        /// <returns>The list of permutations</returns>
        public static ListMapping<int, T> Permute<T>(this IEnumerable<T> Input)
        {
            if (Input == null)
                throw new ArgumentNullException("Input");
            System.Collections.Generic.List<T> Current = new System.Collections.Generic.List<T>();
            Current.AddRange(Input);
            ListMapping<int, T> ReturnValue = new ListMapping<int, T>();
            int Max = (Input.Count() - 1).Factorial();
            int CurrentValue = 0;
            for (int x = 0; x < Input.Count(); ++x)
            {
                int z = 0;
                while (z < Max)
                {
                    int y = Input.Count() - 1;
                    while (y > 1)
                    {
                        T TempHolder = Current[y - 1];
                        Current[y - 1] = Current[y];
                        Current[y] = TempHolder;
                        --y;
                        foreach (T Item in Current)
                            ReturnValue.Add(CurrentValue, Item);
                        ++z;
                        ++CurrentValue;
                        if (z == Max)
                            break;
                    }
                }
                if (x + 1 != Input.Count())
                {
                    Current.Clear();
                    Current.AddRange(Input);
                    T TempHolder2 = Current[0];
                    Current[0] = Current[x + 1];
                    Current[x + 1] = TempHolder2;
                }
            }
            return ReturnValue;
        }

        #endregion

        #endregion
    }
}
