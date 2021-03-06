﻿using System.Threading.Tasks;
using DCS.Alternative.Launcher.ServiceModel;

namespace DCS.Alternative.Launcher.Controls
{
    public abstract class WizardStepBase<TController> : IWizardStep
        where TController : WizardController
    {
        private bool _isInitialized;

        protected WizardStepBase(IContainer container)
        {
            Container = container;
            Controller = container.Resolve<TController>();
        }

        public IContainer Container
        {
            get;
        }

        public TController Controller
        {
            get;
            internal set;
        }

        public virtual bool Commit()
        {
            return true;
        }

        public virtual void Dispose()
        {
        }

        public virtual bool Validate()
        {
            return true;
        }

        public virtual bool OnNext()
        {
            return true;
        }

        public virtual bool OnBack()
        {
            return true;
        }

        public virtual bool OnComplete()
        {
            return true;
        }

        public virtual async Task ActivateAsync()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
                _isInitialized = true;
            }

            IsActivated = true;
        }

        public virtual Task DeactivateAsync()
        {
            return Task.FromResult(true);
        }

        public bool IsActivated
        {
            get;
            set;
        }

        protected virtual Task InitializeAsync()
        {
            return Task.FromResult(true);
        }
    }

    public abstract class WizardStepBase : WizardStepBase<WizardController>
    {
        protected WizardStepBase(IContainer container)
            : base(container)
        {
        }
    }
}