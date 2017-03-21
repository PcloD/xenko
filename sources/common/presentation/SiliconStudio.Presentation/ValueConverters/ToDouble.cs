﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Globalization;

using SiliconStudio.Core.Reflection;

namespace SiliconStudio.Presentation.ValueConverters
{
    /// <summary>
    /// This value converter will convert any numeric value to double. <see cref="ConvertBack"/> is supported and
    /// will convert the value to the target if it is numeric, otherwise it returns the value as-is.
    /// </summary>
    public class ToDouble : ValueConverterBase<ToDouble>
    {
        /// <inheritdoc/>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If we're targeting a nullable double, convert only if the input is a numeric type.
            if (targetType == typeof(double?))
                return value != null && value.GetType().IsNumeric() ? System.Convert.ChangeType(value, typeof(double)) : null;

            // Otherwise just rely on the TypeConverter.
            return System.Convert.ChangeType(value, typeof(double));
        }

        /// <inheritdoc/>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If we're targeting a nullable, check that it's a nullable numeric and convert only if the input is not null.
            var nullableType = Nullable.GetUnderlyingType(targetType);
            if (nullableType != null)
            {
                return value != null && value.GetType().IsNumeric() ? System.Convert.ChangeType(value, nullableType) : value;
            }

            return targetType.IsNumeric() ? System.Convert.ChangeType(value, targetType) : value;
        }
    }
}
