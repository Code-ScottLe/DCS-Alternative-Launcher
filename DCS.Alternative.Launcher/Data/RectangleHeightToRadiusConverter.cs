﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DCS.Alternative.Launcher.Data
{
    [ValueConversion(typeof(double?), typeof(double))]
    [MarkupExtensionReturnType(typeof(RectangleHeightToRadiusConverter))]
    public class RectangleHeightToRadiusConverter : MarkupConverter
    {
        private static RectangleHeightToRadiusConverter _instance;

        static RectangleHeightToRadiusConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new RectangleHeightToRadiusConverter());
        }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var height = value as double?;
            return height.GetValueOrDefault(0) / 2d;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}