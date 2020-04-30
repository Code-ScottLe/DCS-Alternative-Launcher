using System;
using System.Collections.Generic;
using System.Text;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.General
{
    public class LaunchOptionsSettingsViewModel : SettingsCategoryViewModelBase
    {
        public ReactiveCollection<object> PreLaunchOptions
        {
            get;
        } = new ReactiveCollection<object>();

        public ReactiveCollection<object> PostLaunchOptions
        {
            get;
        } = new ReactiveCollection<object>();

        public LaunchOptionsSettingsViewModel(SettingsController controller) 
            : base("LAUNCH OPTIONS", controller)
        {
        }
    }
}
