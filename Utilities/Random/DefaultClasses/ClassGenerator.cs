﻿using System;
using System.Reflection;
using Company.Utilities.Random.BaseClasses;
using Company.Utilities.Random.Interfaces;
using Company.Utilities.Reflection.ExtensionMethods;

namespace Company.Utilities.Random.DefaultClasses
{
    /// <summary>
    /// Randomly generates a class
    /// </summary>
    /// <typeparam name="T">Class type to generate</typeparam>
    public class ClassGenerator<T> : IGenerator<T>
        where T : class,new()
    {
        /// <summary>
        /// Generates a random version of the class
        /// </summary>
        /// <param name="Rand">Random generator to use</param>
        /// <returns>The randomly generated class</returns>
        public T Next(System.Random Rand)
        {
            T ReturnItem = new T();
            System.Type ObjectType = typeof(T);
            foreach (PropertyInfo Property in ObjectType.GetProperties())
            {
                GeneratorAttributeBase Attribute = Property.GetAttribute<GeneratorAttributeBase>();
                if(Attribute!=null)
                    ReturnItem.SetProperty(Property, Attribute.NextObj(Rand));
            }
            return ReturnItem;
        }

        /// <summary>
        /// Generates a random version of the class
        /// </summary>
        /// <param name="Rand">Random generator to use</param>
        /// <param name="Min">Min value (not used)</param>
        /// <param name="Max">Max value (not used)</param>
        /// <returns>The randomly generated class</returns>
        public T Next(System.Random Rand, T Min, T Max)
        {
            return new T();
        }

        /// <summary>
        /// Gets a random version of the class
        /// </summary>
        /// <param name="Rand">Random generator used</param>
        /// <returns>The randonly generated class</returns>
        public object NextObj(System.Random Rand)
        {
            return new T();
        }
    }
}