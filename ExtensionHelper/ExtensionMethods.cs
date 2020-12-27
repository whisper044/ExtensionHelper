﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtensionHelper
{
    public static class ExtensionMethods
    {
        public static bool In<T>(this T source, params T[] list)
        {
            if (source.IsNull()) /*throw new ArgumentNullException("source");*/
                return false;
            else
                return list.Contains(source);
        }
        public static bool NotIn<T>(this T source, params T[] list)
        {
            if (source.IsNull()) /*throw new ArgumentNullException("source");*/
                return true;
            else
                return !list.Contains(source);
        }
        public static T ToEnumType<T>(this string source) where T : struct
        {
            Type type = typeof(T);
            if (!Enum.GetNames(type).Contains(source, StringComparer.InvariantCultureIgnoreCase))
                return default(T);

            object result = Enum.Parse(type, source, true);

            if (result.IsNull())
                return default(T);
            else
                return (T)result;
        }
        public static T ToEnumType<T,K>(this string source) where T : struct
                                                            where K : Attribute
        {   
            Type type = typeof(T);
            if (!Enum.GetNames(type).Contains(source, StringComparer.InvariantCultureIgnoreCase))
            {
                source = type.GetAttributeOfType<K>(source);
                if (!Enum.GetNames(type).Contains(source, StringComparer.InvariantCultureIgnoreCase))
                    return default(T);
            }
            object result = Enum.Parse(type, source, true);

            if (result.IsNull())
                return default(T);
            else
                return (T)result;
        }
        private static string GetAttributeOfType<T>(this Type type, string source) where T : Attribute
        {
            var memInfo = type.GetMembers();
            foreach (var item in memInfo)
            {
                var attributes = item.GetCustomAttributes(typeof(T), false);
                if (attributes.Length > 0)
                {
                    var attribute = (T)attributes[0];
                    PropertyInfo[] props = typeof(T).GetProperties();
                    foreach (var prop in props)
                    {
                        object attributeValue = prop.GetValue(attribute);
                        if (attributeValue.ToString().Equals(source))
                            return item.Name;
                    }
                }
            }

            return source;
        }
        public static bool IsNull(this object obj)
        {
            return (obj is null);
        }
        public static bool IsNotNull(this object obj)
        {
            return !obj.IsNull();
        }
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        public static string Fill(this string name, params string[] parameters)
        {
            if (parameters.Length.Equals(0))
                return name;
            else
                return string.Format(name, parameters);
        }
        public static string RemoveLastCharacter(this String instr)
        {
            return instr.Substring(0, instr.Length - 1);
        }
        public static string RemoveLast(this String instr, int number)
        {
            return instr.Substring(0, instr.Length - number);
        }
        public static string RemoveFirstCharacter(this String instr)
        {
            return instr.Substring(1);
        }
        public static string RemoveFirst(this String instr, int number)
        {
            return instr.Substring(number);
        }
        public static bool Between(this DateTime dt, DateTime rangeBeg, DateTime rangeEnd)
        {
            return dt.Ticks >= rangeBeg.Ticks && dt.Ticks <= rangeEnd.Ticks;
        }
        public static int CalculateAge(this DateTime dateTime)
        {
            var age = DateTime.Now.Year - dateTime.Year;
            if (DateTime.Now < dateTime.AddYears(age))
                age--;
            return age;
        }
        public static string ToReadableTime(this DateTime value)
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - value.Ticks);
            double delta = ts.TotalSeconds;
            if (delta < 60)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 120)
            {
                return "a minute ago";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour ago";
            }
            if (delta < 86400) // 24 * 60 * 60
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 172800) // 48 * 60 * 60
            {
                return "yesterday";
            }
            if (delta < 2592000) // 30 * 24 * 60 * 60
            {
                return ts.Days + " days ago";
            }
            if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }

        /// <summary>
        ///Determine the Next date by passing in a DayOfWeek (i.e. From this date, when is the next Tuesday?)
        /// </summary>
        /// <param name="current"></param>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public static DateTime Next(this DateTime current, DayOfWeek dayOfWeek)
        {
            int offsetDays = dayOfWeek - current.DayOfWeek;
            if (offsetDays <= 0)
            {
                offsetDays += 7;
            }
            DateTime result = current.AddDays(offsetDays);
            return result;
        }
        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }
        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }
        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }
        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }
        public static string ToFileSize(this long size)
        {
            if (size < 1024) { return (size).ToString("F0") + " bytes"; }
            if (size < Math.Pow(1024, 2)) { return (size / 1024).ToString("F0") + "KB"; }
            if (size < Math.Pow(1024, 3)) { return (size / Math.Pow(1024, 2)).ToString("F0") + "MB"; }
            if (size < Math.Pow(1024, 4)) { return (size / Math.Pow(1024, 3)).ToString("F0") + "GB"; }
            if (size < Math.Pow(1024, 5)) { return (size / Math.Pow(1024, 4)).ToString("F0") + "TB"; }
            if (size < Math.Pow(1024, 6)) { return (size / Math.Pow(1024, 5)).ToString("F0") + "PB"; }
            return (size / Math.Pow(1024, 6)).ToString("F0") + "EB";
        }
        public static IEnumerable<TSource> FromHierarchy<TSource>(this TSource source,Func<TSource, TSource> nextItem, Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }
        public static IEnumerable<TSource> FromHierarchy<TSource>(this TSource source,Func<TSource, TSource> nextItem) where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }
        public static string GetaAllMessages(this Exception exception)
        {
            var messages = exception.FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message);
            return String.Join(Environment.NewLine, messages);
        }
    }
}
