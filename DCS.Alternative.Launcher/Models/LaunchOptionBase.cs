using System;
using System.Collections.Generic;
using System.Text;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Models
{
    public abstract class LaunchOptionBase
    {
        public ReactiveProperty<int> Index
        {
            get; protected set;
        } = new ReactiveProperty<int>();

        public ReactiveProperty<string> Name
        {
            get; protected set;
        } = new ReactiveProperty<string>();


        public LaunchOptionBase()
        {
        }
    }
}
