﻿using DCS.Alternative.Launcher.Analytics;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Wizards.Steps
{
    public class QueryViewportSetupWizardStepViewModel : WizardStepBase
    {
        private readonly ISettingsService _settingsService;

        public QueryViewportSetupWizardStepViewModel(IContainer container)
            : base(container)
        {
            _settingsService = container.Resolve<ISettingsService>();
        }

        public ReactiveProperty<bool> IsSingleDisplaySetup
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsMultipleDisplaySetup
        {
            get;
        } = new ReactiveProperty<bool>();

        public override bool Commit()
        {
            Controller.ClearAfter(GetType());

            if (IsMultipleDisplaySetup.Value)
            {
                Tracker.Instance.SendEvent(AnalyticsCategories.Configuration, AnalyticsEvents.ViewportSetup, "multi");

                Controller.Steps.Add(new SelectGameViewportScreensStepViewModel(Container));
                Controller.Steps.Add(new SelectUIViewportScreensStepViewModel(Container));
                Controller.Steps.Add(new SelectDeviceViewportScreensStepViewModel(Container));
            }
            else
            {
                Tracker.Instance.SendEvent(AnalyticsCategories.Configuration, AnalyticsEvents.ViewportSetup, "single");
            }

            return base.Commit();
        }

        public override bool Validate()
        {
            if (!IsSingleDisplaySetup.Value && !IsMultipleDisplaySetup.Value)
            {
                MessageBoxEx.Show("Please select either single or multiple screen setup to continue.", "Screen Setup");
                return false;
            }

            return base.Validate();
        }
    }
}