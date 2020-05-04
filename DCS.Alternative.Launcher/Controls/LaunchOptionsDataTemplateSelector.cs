using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DCS.Alternative.Launcher.Models.LaunchOptions;

namespace DCS.Alternative.Launcher.Controls
{
    public class LaunchOptionsDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GenericWin32ApplicationTemplate
        {
            get; set;
        }

        public LaunchOptionsDataTemplateSelector()
        {
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Win32ApplicationLaunchOption launchOption)
            {
                return GenericWin32ApplicationTemplate;
            }
            else
            {
                return base.SelectTemplate(item, container);
            }
        }
    }
}
