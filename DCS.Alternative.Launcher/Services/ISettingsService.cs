﻿using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Modules;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Services
{
    public interface ISettingsService
    {
        InstallLocation SelectedInstall { get; set; }

        void RemoveInstalls(params string[] directory);

        void AddInstalls(params string[] directory);

        ModuleViewportTemplate[] GetViewportTemplates();

        void RemoveViewportTemplate(string moduleId);

        void RemoveViewport(string moduleId, Viewport viewport);

        void UpsertViewport(string name, string moduleId, Screen screen, Viewport viewport);

        InstallLocation[] GetInstallations();

        T GetValue<T>(string category, string key, T defaultValue = default(T));

        void SetValue(string category, string key, object value);

        ModuleViewportTemplate[] GetDefaultViewportTemplates();
    }
}