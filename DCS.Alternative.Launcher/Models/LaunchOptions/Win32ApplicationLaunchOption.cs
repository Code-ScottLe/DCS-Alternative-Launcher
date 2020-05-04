using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Models.LaunchOptions
{
    public class Win32ApplicationLaunchOption : LaunchOptionBase
    {
        public ReactiveProperty<string> ApplicationPath
        {
            get; protected set;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<string> LaunchParameter
        {
            get; protected set;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<bool> IsSingleInstance
        {
            get; protected set;
        } = new ReactiveProperty<bool>(true);

        public Win32ApplicationLaunchOption()
            : base()
        {
        }
    }
}
