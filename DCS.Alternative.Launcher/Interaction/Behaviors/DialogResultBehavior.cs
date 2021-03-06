﻿using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace DCS.Alternative.Launcher.Interaction.Behaviors
{
    public class DialogResultBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool?), typeof(DialogResultBehavior), new PropertyMetadata(null, OnValuePropertyChanged));

        public static readonly DependencyProperty IsDialogProperty =
            DependencyProperty.Register("IsDialog", typeof(bool), typeof(DialogResultBehavior), new PropertyMetadata(true));

        public bool IsDialog
        {
            get { return (bool) GetValue(IsDialogProperty); }
            set { SetValue(IsDialogProperty, value); }
        }
        public bool? Value
        {
            get { return (bool?) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as DialogResultBehavior;

            behavior?.OnValueChanged((bool?) e.OldValue, (bool?) e.NewValue);
        }

        private void OnValueChanged(bool? oldValue, bool? newValue)
        {
            if (IsDialog)
            {
                AssociatedObject.DialogResult = newValue;
            }
            else
            {
                AssociatedObject.Close();
            }
        }
    }
}