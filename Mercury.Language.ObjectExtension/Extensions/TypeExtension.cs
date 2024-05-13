// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by OpenGamma Inc.
//
// Copyright (C) 2012 - present by OpenGamma Inc. and the OpenGamma group of companies
//
// Please see distribution for license.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//     
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// TypeExtension will supply the extension methods to enhance the language support
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Get all base type of the Type that given by this parameter
        /// </summary>
        /// <param name="self">Target Type</param>
        /// <returns>All Type</returns>
        public static IEnumerable<Type> GetBaseTypes(this Type self)
        {
            for (var baseType = self.BaseType; null != baseType; baseType = baseType.BaseType)
            {
                yield return baseType;
            }
        }

        /// <summary>
        /// Check if the type is Nullable type or not
        /// </summary>
        /// <param name="type">Target Type</param>
        /// <returns>True if the type is Nullable, otherwise return false</returns>
        public static Boolean IsNullable(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if the type is Comparable type or not
        /// </summary>
        /// <param name="type">Target Type</param>
        /// <returns>True if the type is Nullable, otherwise return false</returns>
        public static Boolean IsComparable(this Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IComparable));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Target Type</param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Boolean HasGenericTypeArguments(this Type type, Type[] types)
        {
            if (!type.IsGenericType)
                return true;

            var result = true;

            Type[] genericTypes = type.GetGenericArguments();

            var _find = new Boolean[types.Length];
            Fill(_find, false);
            int _count = 0;

            foreach (var g in genericTypes)
            {
                foreach (var t in types)
                {
                    if (IsInheritType(g, t))
                    {
                        _find[_count] = true;
                    }
                }
                _count++;
            }
            if (_find.Any(f => !f))
                result = false;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Target Type</param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static Boolean IsInheritType(this Type? type, Type targetType)
        {
            if ((type == null) || (type == typeof(Object)))
                return false;

            if (type == targetType)
            {
                return true;
            }
            else
            {
                return IsInheritType(type.BaseType, targetType);
            }
        }

        /// <summary>
        /// Private method, to fill the array with supplied value
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="originalArray">Original array</param>
        /// <param name="with">Filling value</param>
        private static void Fill<T>(T[] originalArray, T with)
        {
            for (int i = 0; i < originalArray.Length; i++)
            {
                originalArray[i] = with;
            }
        }
    }
}
