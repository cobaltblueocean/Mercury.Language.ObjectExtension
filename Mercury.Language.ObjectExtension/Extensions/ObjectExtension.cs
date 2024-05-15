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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mercury.Language;

namespace System
{
    /// <summary>
    /// ObjectExtension class to supply multiple utility methods including very complex object comparing method.
    /// </summary>
    /// <see href="https://www.cyotek.com/blog/comparing-the-properties-of-two-objects-via-reflection"></see>
    public static class MercuryObjectExtension
    {
        static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        public static bool IsList<T>(this Object o)
        {
            if (o == null) return false;
            return o is IList<T> &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsDictionary<TKey, TValue>(this Object o)
        {
            if (o == null) return false;
            return o is IDictionary<TKey, TValue> &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        /// <summary>
        /// Identify if this object is KeyValuePair or not
        /// </summary>
        /// <see href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/9ad76a19-ed9c-4a02-be6b-95870af0e10b/how-to-determine-if-object-is-keyvaluepairlttkey-tvaluegt?forum=csharpgeneral"/>
        /// <param name="o">target object</param>
        /// <returns>True if the target object is KeyValuePair, otherwise returns false</returns>
        public static bool IsKeyValuePair(this object o)
        {
            Type type = o.GetType();
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() != null ? type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>) : false;
            }
            return false;
        }

        /// <summary>
        /// Get Property value by Property's name
        /// </summary>
        /// <param name="src">target object</param>
        /// <param name="propName">Property name</param>
        /// <returns>The property's value</returns>
        public static object GetPropertyValue(this object src, string propName)
        {
            var properties = src.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            var propInfo = properties.FirstOrDefault(x => x.Name.ToLower() == propName.ToLower());

            if (propInfo != null)
                return propInfo.GetValue(src, null);

            throw new KeyNotFoundException(String.Format("The property {0} is not found in the source object.", propName));
        }


        /// <summary>
        /// Get Property value by Property's name
        /// </summary>
        /// <param name="target">target object</param>
        /// <param name="propName">Property name</param>
        /// <returns>The property's value</returns>
        public static void SetPropertyValue<T, V>(this T target, String propName, V value)
        {
            var properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            var propInfo = properties.FirstOrDefault(x => x.Name.ToLower() == propName.ToLower());

            if (propInfo != null)
                propInfo.SetValue(target, value);

            throw new KeyNotFoundException(String.Format("The property {0} is not found in the source object.", propName));
        }

        public static String ValueOf<T>(this T target)
        {
            if (IsNullable(typeof(T)))
            {
                if (target == null)
                    return "null";
                else
                    return target.ToString();
            }
            else
            {
                return target.ToString();
            }
        }

        /// <summary>
        /// Compares the properties of two objects of the same type and returns if all properties are equal ignoring Obsoleted property.
        /// </summary>
        /// <param name="objectA">The first object to compare.</param>
        /// <param name="objectB">The second object to compre.</param>
        /// <param name="ignoreList">A list of property names to ignore from the comparison.</param>
        /// <returns><c>true</c> if all property values are equal, otherwise <c>false</c>.</returns>
        public static bool AreObjectsEqual(this object objectA, object objectB, params string[] ignoreList)
        {

            return AreObjectsEqual(objectA, objectB, false, ignoreList);
        }

        /// <summary>
        /// Compares the properties of two objects of the same type and returns if all properties are equal.
        /// </summary>
        /// <param name="objectA">The first object to compare.</param>
        /// <param name="objectB">The second object to compre.</param>
        /// <param name="isIncludeObsolete">Compare including Obsoleted property or not</param>
        /// <param name="ignoreList">A list of property names to ignore from the comparison.</param>
        /// <returns><c>true</c> if all property values are equal, otherwise <c>false</c>.</returns>
        public static bool AreObjectsEqual(this object objectA, object objectB, Boolean isIncludeObsolete, params string[] ignoreList)
        {
            List<long> objectHashList = new List<long>();
            return EvaluateObjectsComparison(objectA, objectB, objectHashList, isIncludeObsolete, ignoreList);
        }

        /// <summary>
        /// Compares the properties of two objects of the same type and returns if all properties are equal.
        /// Check evaluatedObject's hash to prevent cercular evaluation
        /// </summary>
        /// <param name="objectA">The first object to compare.</param>
        /// <param name="objectB">The second object to compre.</param>
        /// <param name="objectHashList">A list of object's hash to check that already evaluated.</param>
        /// <param name="isIncludeObsolete">Compare including Obsoleted property or not</param>
        /// <param name="ignoreList">A list of property names to ignore from the comparison.</param>
        /// <returns><c>true</c> if all property values are equal, otherwise <c>false</c>.</returns>
        private static bool EvaluateObjectsComparison(object objectA, object objectB, List<long> objectHashList, Boolean isIncludeObsolete, params string[] ignoreList)
        {
            Boolean result = true;

            try
            {
                if (objectA != null && objectB != null)
                {
                    if (objectA.Equals(objectB))
                    {
                        return true;
                    }
                    else
                    {
                        Type objectTypeA;
                        Type objectTypeB;

                        objectTypeA = objectA.GetType();
                        objectTypeB = objectB.GetType();

                        if (!objectHashList.Contains(objectA.GetHashCode()))
                        {
                            objectHashList = MarkAsPrevioiuslyAppreared(objectHashList, objectA);

                            if (IsTypeSame(objectA.GetType(), objectB.GetType()))
                            {
                                if (!IsKeyValuePair(objectA))
                                {
                                    if (CanDirectlyCompare(objectTypeA) && !IsList(objectA))
                                    {
                                        result = objectA.Equals(objectB);
                                        if (!result)
                                            Console.WriteLine(String.Format(LocalizedResources.Instance().MismatchWithPropertyFound, objectA.GetType().Name, objectB.GetType().Name));

                                        return result;
                                    }
                                    // if it implements IEnumerable, then scan any items
                                    else if (typeof(IEnumerable).IsAssignableFrom(objectTypeA))
                                    {
                                        IEnumerable<object> collectionItems1;
                                        IEnumerable<object> collectionItems2;
                                        int collectionItemsCount1;
                                        int collectionItemsCount2;

                                        // null check
                                        if (objectA == null && objectB != null || objectA != null && objectB == null)
                                        {
                                            result = false;
                                        }
                                        else if (objectA != null && objectB != null)
                                        {
                                            collectionItems1 = ((IEnumerable)objectA).Cast<object>();
                                            collectionItems2 = ((IEnumerable)objectB).Cast<object>();
                                            collectionItemsCount1 = collectionItems1.Count();
                                            collectionItemsCount2 = collectionItems2.Count();

                                            // check the counts to ensure they match
                                            if (collectionItemsCount1 != collectionItemsCount2)
                                            {
                                                result = false;
                                            }
                                            // and if they do, compare each item... this assumes both collections have the same order
                                            else
                                            {
                                                for (int i = 0; i < collectionItemsCount1; i++)
                                                {
                                                    object collectionItem1;
                                                    object collectionItem2;
                                                    Type collectionItemType;

                                                    collectionItem1 = collectionItems1.ElementAt(i);
                                                    collectionItem2 = collectionItems2.ElementAt(i);
                                                    collectionItemType = collectionItem1.GetType();
                                                    if (!IsKeyValuePair(collectionItem1))
                                                    {
                                                        if (CanDirectlyCompare(collectionItemType))
                                                        {
                                                            if (!AreValuesEqual(collectionItem1, collectionItem2))
                                                            {
                                                                result = false;
                                                            }
                                                        }
                                                        else if (!EvaluateObjectsComparison(collectionItem1, collectionItem2, objectHashList, isIncludeObsolete, ignoreList))
                                                        {
                                                            result = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var collectionItem1Key = GetPropertyValue(collectionItem1, "Key");
                                                        var collectionItem2Key = GetPropertyValue(collectionItem2, "Key");
                                                        var collectionItemKeyType = collectionItem1Key.GetType();
                                                        if (CanDirectlyCompare(collectionItemKeyType))
                                                        {
                                                            if (!AreValuesEqual(collectionItem1Key, collectionItem2Key))
                                                            {
                                                                result = false;
                                                            }
                                                        }
                                                        else if (!EvaluateObjectsComparison(collectionItem1Key, collectionItem2Key, objectHashList, isIncludeObsolete, ignoreList))
                                                        {
                                                            result = false;
                                                        }

                                                        var collectionItem1Value = GetPropertyValue(collectionItem1, "Value");
                                                        var collectionItem2Value = GetPropertyValue(collectionItem2, "Value");
                                                        var collectionItemValueType = collectionItem1Value.GetType();
                                                        if (CanDirectlyCompare(collectionItemValueType))
                                                        {
                                                            if (!AreValuesEqual(collectionItem1Value, collectionItem2Value))
                                                            {
                                                                result = false;
                                                            }
                                                        }
                                                        else if (!EvaluateObjectsComparison(collectionItem1Value, collectionItem2Value, objectHashList, isIncludeObsolete, ignoreList))
                                                        {
                                                            result = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (IsList(objectA))
                                    {
                                        int listItemsCount1;
                                        int listItemsCount2;

                                        // null check
                                        if (objectA == null && objectB != null || objectA != null && objectB == null)
                                        {
                                            result = false;
                                        }
                                        else if (objectA != null && objectB != null)
                                        {
                                            var ToArrayMethodA = objectA.GetType().GetMethod("ToArray");
                                            var ToArrayMethodB = objectB.GetType().GetMethod("ToArray");
                                            var listItemsArrayA = (System.Array)ToArrayMethodA.Invoke(objectA, null);
                                            var listItemsArrayB = (System.Array)ToArrayMethodB.Invoke(objectB, null);

                                            listItemsCount1 = listItemsArrayA.Length;
                                            listItemsCount2 = listItemsArrayA.Length;

                                            // check the counts to ensure they match
                                            if (listItemsCount1 != listItemsCount2)
                                            {
                                                result = false;
                                            }
                                            // and if they do, compare each item... this assumes both collections have the same order
                                            else
                                            {
                                                for (int i = 0; i < listItemsCount1; i++)
                                                {
                                                    object listItem1;
                                                    object listItem2;
                                                    Type listItemType;

                                                    listItem1 = listItemsArrayA.GetValue(i);
                                                    listItem2 = listItemsArrayB.GetValue(i);
                                                    listItemType = listItem1.GetType();
                                                    if (!IsKeyValuePair(listItem1))
                                                    {
                                                        if (CanDirectlyCompare(listItemType))
                                                        {
                                                            if (!AreValuesEqual(listItem1, listItem2))
                                                            {
                                                                result = false;
                                                            }
                                                        }
                                                        else if (!EvaluateObjectsComparison(listItem1, listItem2, objectHashList, isIncludeObsolete, ignoreList))
                                                        {
                                                            result = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var listItem1Key = GetPropertyValue(listItem1, "Key");
                                                        var listItem2Key = GetPropertyValue(listItem2, "Key");
                                                        var listItemKeyType = listItem1Key.GetType();
                                                        if (CanDirectlyCompare(listItemKeyType))
                                                        {
                                                            if (!AreValuesEqual(listItem1Key, listItem2Key))
                                                            {
                                                                result = false;
                                                            }
                                                        }
                                                        else if (!EvaluateObjectsComparison(listItem1Key, listItem2Key, objectHashList, isIncludeObsolete, ignoreList))
                                                        {
                                                            result = false;
                                                        }

                                                        var listItem1Value = GetPropertyValue(listItem1, "Value");
                                                        var listItem2Value = GetPropertyValue(listItem2, "Value");
                                                        var listItemValueType = listItem1Value.GetType();
                                                        if (CanDirectlyCompare(listItemValueType))
                                                        {
                                                            if (!AreValuesEqual(listItem1Value, listItem2Value))
                                                            {
                                                                result = false;
                                                            }
                                                        }
                                                        else if (!EvaluateObjectsComparison(listItem1Value, listItem2Value, objectHashList, isIncludeObsolete, ignoreList))
                                                        {
                                                            result = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (objectA.GetType().IsClass || objectA.GetType().IsInterface)
                                    {
                                        PropertyInfo[] properties;
                                        if (isIncludeObsolete)
                                            properties = objectTypeA.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && !ignoreList.Contains(p.Name) && p.GetCustomAttributes(typeof(IgnoreObjectCompareAttribute), true).Length == 0).ToArray();
                                        else
                                            properties = objectTypeA.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && !ignoreList.Contains(p.Name) && p.GetCustomAttributes(typeof(IgnoreObjectCompareAttribute), true).Length == 0 && p.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0).ToArray();

                                        foreach (PropertyInfo propertyInfo in properties)
                                        {
                                            Object valueTypeA;
                                            try
                                            {
                                                valueTypeA = propertyInfo.GetValue(objectA, null);
                                                if (objectA != valueTypeA)
                                                {
                                                    if (!EvaluateObjectsComparison(propertyInfo.GetValue(objectA, null), propertyInfo.GetValue(objectB, null), objectHashList, isIncludeObsolete, ignoreList))
                                                    {
                                                        Console.WriteLine(String.Format(LocalizedResources.Instance().CannotCompareProperty, valueTypeA.GetType().FullName, propertyInfo.Name));
                                                        result = false;
                                                    }
                                                }
                                            }
                                            catch (TargetParameterCountException ex)
                                            {
                                                // If "this[]" property, ignore it.
                                                if (propertyInfo.Name != "Item")
                                                {
                                                    Console.WriteLine(String.Format(LocalizedResources.Instance().CannotCompareProperty, objectA.GetType().FullName, propertyInfo.Name));
                                                }
                                            }
                                            catch (TargetInvocationException te)
                                            {
                                                Console.WriteLine(String.Format(LocalizedResources.Instance().PropertyOrMethodNotImplemented, propertyInfo.Name));
                                            }
                                            catch (NotImplementedException ne)
                                            {
                                                Console.WriteLine(String.Format(LocalizedResources.Instance().PropertyOrMethodNotImplemented, propertyInfo.Name));
                                            }
                                        }
                                    }
                                    else if (objectTypeA != objectTypeB)
                                    {
                                        result = false;
                                        return result;
                                    }
                                    else
                                    {
                                        result = false;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(String.Format(LocalizedResources.Instance().MismatchWithPropertyFound, objectA.GetType().Name, objectB.GetType().Name));
                                result = false;
                            }
                        }
                    }
                }
                else if (objectA == null && objectB == null)
                {
                    // Do nothing
                    // result = true;
                }
                else
                {
                    Console.WriteLine(String.Format(LocalizedResources.Instance().MismatchWithPropertyFound, objectA.GetType().Name, objectB.GetType().Name));
                    result = object.Equals(objectA, objectB);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(String.Format(LocalizedResources.Instance().CannotCompareValues, ex.Message));
                result = false;
            }

            return result;
        }

        private static List<long> MarkAsPrevioiuslyAppreared(List<long> objectHashList, Object objectA)
        {
            if ((!objectA.GetType().IsPrimitive) && (!objectA.GetType().IsValueType))
            {
                objectHashList.Add(objectA.GetHashCode());
            }
            return objectHashList;
        }

        private static Boolean IsTypeSame(Type objectTypeA, Type objectTypeB)
        {
            Boolean result = false;

            if (objectTypeA.Equals(objectTypeB))
            {
                result = true;
            }
            else if (objectTypeB.GetBaseTypes().Contains(objectTypeA))
            {
                result = true;
            }
            else if (objectTypeA.GetBaseTypes().Contains(objectTypeB))
            {
                result = true;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(objectTypeA))
            {
                Type elementTypeA = objectTypeA.GetElementType();
                Type elementTypeB = objectTypeB.GetElementType();

                result = IsTypeSame(elementTypeA, elementTypeB);
            }
            else if (objectTypeA.IsGenericType && objectTypeA.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementTypeA = objectTypeA.GetGenericArguments()[0];
                Type elementTypeB = objectTypeB.GetGenericArguments()[0];

                result = IsTypeSame(elementTypeA, elementTypeB);
            }

            return result;
        }

        private static Boolean IsLeftObjectBasedOnTypeRight(Object objectA, Object objectB)
        {
            Type objectTypeA = objectA.GetType();
            Type objectTypeB = objectB.GetType();

            if (objectTypeA.GetType().GetBaseTypes().Contains(objectTypeB))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        private static Boolean IsTypeSame(Object objectA, Object objectB)
        {
            Boolean result = false;
            Type objectTypeA = objectA.GetType();
            Type objectTypeB = objectB.GetType();

            if (objectTypeA.Equals(objectTypeB))
            {
                result = true;
            }
            else if (objectTypeB.GetBaseTypes().Contains(objectTypeA))
            {
                result = true;
            }
            else if (objectTypeA.GetBaseTypes().Contains(objectTypeB))
            {
                result = true;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(objectTypeA))
            {
                Type elementTypeA = objectTypeA.GetElementType();
                Type elementTypeB = objectTypeB.GetElementType();

                result = IsTypeSame(elementTypeA, elementTypeB);
            }
            else if (objectTypeA.IsGenericType && objectTypeA.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementTypeA = objectTypeA.GetGenericArguments()[0];
                Type elementTypeB = objectTypeB.GetGenericArguments()[0];

                result = IsTypeSame(elementTypeA, elementTypeB);
            }

            return result;
        }

        /// <summary>
        /// Determines whether value instances of the specified type can be directly compared.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this value instances of the specified type can be directly compared; otherwise, <c>false</c>.
        /// </returns>
        /// <see cref="https://www.cyotek.com/blog/comparing-the-properties-of-two-objects-via-reflection"/>
        public static bool CanDirectlyCompare(this Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType || type == typeof(string);
        }

        /// <summary>
        /// Compares two values and returns if they are the same.
        /// </summary>
        /// <param name="valueA">The first value to compare.</param>
        /// <param name="valueB">The second value to compare.</param>
        /// <returns><c>true</c> if both values match, otherwise <c>false</c>.</returns>
        /// <see cref="https://www.cyotek.com/blog/comparing-the-properties-of-two-objects-via-reflection"/>
        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (!object.Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }

        public static bool IsNullable<T>(this T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public static bool HasValue(this Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                Type type = obj.GetType();

                // ValueType always has value, not null
                if (type.IsValueType)
                {
                    return true;
                }
                // Other types
                else //if (Nullable.GetUnderlyingType(type) != null)
                {
                    return obj != null;
                }
            }
        }

        public static T CastType<T>(this object input)
        {
            T result = default(T);
            try
            {
                result = (T)Convert.ChangeType(input, typeof(T));
            }
            catch
            {
                result = (T)(object)input;
            }

            return result;
        }

        public static T[] ConvertToArray<T>(this T input)
        {
            return ConvertToList<T>(input).ToArray();
        }

        public static List<T> ConvertToList<T>(this T input)
        {
            return new List<T>() { input };
        }

        public static bool IsImplementType(this Object baseObj, Type type)
        {
            var class0 = baseObj.GetType();
            return class0.GetInterfaces().Contains(type);
        }

        public static bool IsGenericParameterImplementType(this Object objBase, Type type)
        {
            return objBase.GetType().IsGenericParameterImplementType(type);
        }

        public static bool IsGenericParameterImplementType(this Type typeBase, Type type)
        {
            List<Type> result = new List<Type>();

            if (typeBase.IsGenericType)
            {
                result = typeBase.GetGenericTypeParameter().SelectMany(x => x.GetInterfaces()).ToList();
            }
            return result.Contains(type);
        }

        public static bool IsGenericParameterImplementBaseClass(this Object objBase, Type type)
        {
            return objBase.GetType().IsGenericParameterImplementBaseClass(type);
        }

        public static bool IsGenericParameterImplementBaseClass(this Type typeBase, Type type)
        {
            List<Type> result = new List<Type>();

            if (typeBase.IsGenericType)
            {
                result = typeBase.GetGenericTypeParameter().Select(x => x.BaseType).ToList();
            }
            return result.Contains(type);
        }

        public static List<Type> GetGenericTypeParameter(this Type type)
        {
            List<Type> result = null;

            if (type.IsGenericType)
            {
                result = type.GetGenericArguments().ToList();
            }
            return result;
        }


        public static T NewInstance<T>(this Type type)
        {
            return (T)NewInstance(type);
        }

        public static dynamic NewInstance(this Type type)
        {
            return Core.CreateInstanceFromType(type);
        }

        public static T NewInstance<T>(this Type type, Object[] paramValues)
        {
            return (T)NewInstance(type, paramValues);
        }

        public static dynamic NewInstance(this Type type, Object[] paramValues)
        {
            return Core.CreateInstanceFromType(type, paramValues);
        }

        public static void SetFieldValue<T, V>(this T target, String property, V value)
        {
            var properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.Name.ToLower() == property.ToLower())
                {
                    field.SetValue(target, value);
                    return;
                }
            }
        }
    }
}
