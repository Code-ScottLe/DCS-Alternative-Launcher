using System;
using System.Collections.Generic;
using System.Text;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Models.LaunchOptions;
using Microsoft.Win32;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.General
{
    public class LaunchOptionsSettingsViewModel : SettingsCategoryViewModelBase
    {
        public ReactiveCollection<LaunchOptionBase> PreLaunchOptions
        {
            get;
        } = new ReactiveCollection<LaunchOptionBase>();

        public ReactiveCollection<LaunchOptionBase> PostLaunchOptions
        {
            get;
        } = new ReactiveCollection<LaunchOptionBase>();

        public ReactiveCommand BrowseApplicationCommand
        {
            get; protected set;
        } = new ReactiveCommand();

        public LaunchOptionsSettingsViewModel(SettingsController controller) 
            : base("LAUNCH OPTIONS", controller)
        {
            BrowseApplicationCommand.Subscribe(OnBrowseApplication);
        }


        private void OnBrowseApplication()
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".exe";
            dialog.Filter = "Application Executable (.exe)|*.exe";
            dialog.Multiselect = false;
            bool? selected = dialog.ShowDialog();
            if (selected == true)
            {
                Win32ApplicationLaunchOption op = new Win32ApplicationLaunchOption();
                op.ApplicationPath.Value = dialog.FileName;

                PreLaunchOptions.Add(op);
            }
        }
    }
}
