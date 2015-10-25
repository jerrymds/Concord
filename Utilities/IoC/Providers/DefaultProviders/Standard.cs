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
using Company.Utilities.IoC.Mappings;
using Company.Utilities.IoC.Providers.Implementations;
using Company.Utilities.IoC.Providers.Interfaces;
using Company.Utilities.IoC.Providers.Scope;
#endregion

namespace Company.Utilities.IoC.Providers.DefaultProviders
{
    /// <summary>
    /// Standard provider
    /// </summary>
    public class Standard : IProvider
    {
        /// <summary>
        /// Creates an implementation based off of a type
        /// </summary>
        /// <param name="ImplementationType">Implementation type</param>
        /// <param name="MappingManager">Mapping manager</param>
        /// <returns>an implementation class</returns>
        public IImplementation CreateImplementation(Type ImplementationType, MappingManager MappingManager)
        {
            return new Implementations.Standard(ImplementationType, MappingManager);
        }

        /// <summary>
        /// Provider scope
        /// </summary>
        public BaseScope ProviderScope
        {
            get { return new StandardScope(); }
        }

        /// <summary>
        /// Creates an implementation based off an existing implementation
        /// </summary>
        /// <param name="Implementation">Implementation class</param>
        /// <param name="MappingManager">Mapping manager</param>
        /// <returns>Potentially a new implementation class (if appropriate)</returns>
        public IImplementation CreateImplementation(IImplementation Implementation, MappingManager MappingManager)
        {
            return CreateImplementation(Implementation.ReturnType, MappingManager);
        }

        /// <summary>
        /// Creates an implementation that uses a specified function
        /// </summary>
        /// <param name="Implementation">Implementation delegate</param>
        /// <returns>An implementation class</returns>
        public IImplementation CreateImplementation<ImplementationType>(Func<ImplementationType> Implementation)
        {
            return new Delegate<ImplementationType>(Implementation);
        }
    }
}
