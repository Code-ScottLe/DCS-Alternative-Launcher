﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.Analytics;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Lua;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Threading;
using DCS.Alternative.Launcher.Windows.FirstUse;
using DCS.Alternative.Launcher.Wizards;
using DCS.Alternative.Launcher.Wizards.Steps;
using DCS.Alternative.Launcher.Wizards.Steps.FirstUse;
using DCS.Alternative.Launcher.Wizards.Steps.Settings.SelectViewport;
using WpfScreenHelper;
using IContainer = DCS.Alternative.Launcher.ServiceModel.IContainer;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public class SettingsController
    {
        private readonly IContainer _container;
        private readonly IDcsWorldService _dcsWorldService;
        private readonly ISettingsService _settingsService;
        private readonly IProfileSettingsService _profileSettingsService;

        public SettingsController(IContainer container)
        {
            _container = container;
            _dcsWorldService = container.Resolve<IDcsWorldService>();
            _settingsService = container.Resolve<ISettingsService>();
            _profileSettingsService = container.Resolve<IProfileSettingsService>();
        }

        public async Task<Viewport[]> EditViewportsAsync(string templateName, string exampleImageUrl, Module module, Viewport[] viewports)
        {
            var deviceViewportMonitorIds = GetDeviceViewportMonitorIds();
            var screens = Screen.AllScreens.Where(s => deviceViewportMonitorIds.Contains(s.DeviceName)).ToArray();

            Array.ForEach(viewports, v => v.MonitorId = screens[0].DeviceName);

            var model =
                new ModuleViewportModel(
                    templateName,
                    exampleImageUrl,
                    module,
                    viewports);

            return await EditViewportsAsync(model);
        }

        public async Task<Viewport[]> EditViewportsAsync(ModuleViewportModel value)
        {
            if (!IsValidViewports(value.Viewports.ToArray()))
            {
                MessageBoxEx.Show($"One or more viewports were defined on a monitor that is no longer configured, they have been removed.{Environment.NewLine}Make sure you add any missing viewports before saving.", "Invalid Viewport");
            }

            var tasks = new List<Task>();
            var screenIds = GetDeviceViewportMonitorIds();
            var devices = _profileSettingsService.GetViewportDevices(value.Module.Value.ModuleId).Where(d=>value.Viewports.Any(v=>v.SeatIndex == d.SeatIndex)).ToArray();
            var windows = new List<Window>();
            var viewModels = new List<ViewportEditorWindowViewModel>();

            var editorScreens = Screen.AllScreens.Where(s => screenIds.Contains(s.DeviceName));
            var overlayScreens = Screen.AllScreens.Where(s => !screenIds.Contains(s.DeviceName) && !value.Viewports.Select(v => v.MonitorId).Contains(s.DeviceName));

            foreach (var screen in editorScreens)
            {
                var viewportModels = new List<ViewportModel>();

                foreach (var viewport in value.Viewports.Where(v => v.MonitorId == screen.DeviceName))
                {
                    if (!screenIds.Contains(viewport.MonitorId))
                    {
                        continue;
                    }

                    var model = new ViewportModel();

                    model.Height.Value = viewport.Height;
                    model.InitFile.Value = viewport.RelativeInitFilePath;
                    model.ImageUrl.Value = Path.Combine(ApplicationPaths.ViewportPath, $"Images/{value.Module.Value.ModuleId}/{viewport.ViewportName}.jpg");
                    model.Name.Value = viewport.ViewportName;
                    model.Width.Value = viewport.Width;
                    model.X.Value = viewport.X;
                    model.Y.Value = viewport.Y;

                    viewportModels.Add(model);
                }

                var window = new ViewportEditorWindow();
                var vm = new ViewportEditorWindowViewModel(_container, false, screen.DeviceName, value.Module.Value, devices, viewportModels.ToArray());

                vm.Viewports.CollectionChanged += (sender, args) => 
                {
                    OnViewportsChanged(vm, viewModels);
                };
                
                window.Screen = screen;
                window.DataContext = vm;
                window.Show();
                window.BringIntoView();

                windows.Add(window);
                viewModels.Add(vm);

                tasks.Add(EventAsync.FromEvent(
                    handler => window.Closed += handler,
                    handler => window.Closed -= handler));
            }

            foreach (var screen in overlayScreens)
            {
                var window = new MonitorOverlay
                {
                    Screen = screen
                };

                window.Show();
                window.BringIntoView();
                windows.Add(window);

                tasks.Add(EventAsync.FromEvent(
                    handler => window.Closed += handler,
                    handler => window.Closed -= handler));
            }

            await Task.WhenAny(tasks);

            foreach (var window in windows)
            {
                try
                {
                    window.Close();
                }
                catch
                {
                }
            }

            var save = viewModels.Any(vm => vm.DialogResult.Value == true);
            var viewports = new List<Viewport>();

            if (save)
            {
                foreach (var viewModel in viewModels)
                {
                    var screen = Screen.AllScreens.First(s => s.DeviceName == viewModel.MonitorId);

                    foreach (var viewportModel in viewModel.Viewports)
                    {
                        viewports.Add(
                            new Viewport
                            {
                                Height = (int) viewportModel.Height.Value,
                                MonitorId = screen.DeviceName,
                                SeatIndex = viewportModel.SeatIndex.Value,
                                OriginalDisplayHeight = (int) screen.Bounds.Height,
                                OriginalDisplayWidth = (int) screen.Bounds.Width,
                                RelativeInitFilePath = viewportModel.InitFile.Value,
                                ViewportName = viewportModel.Name.Value,
                                Width = (int) viewportModel.Width.Value,
                                X = (int) viewportModel.X.Value,
                                Y = (int) viewportModel.Y.Value
                            });
                    }
                }
                return viewports.ToArray();
            }
            else
            {
                return value.Viewports.ToArray();
            }
        }

        private void OnViewportsChanged(ViewportEditorWindowViewModel sender, List<ViewportEditorWindowViewModel> allViewModels)
        {
            var viewports = allViewModels.SelectMany(vm => vm.Viewports.Select(v => v.Name.Value)).ToArray();

            foreach (var vm in allViewModels)
            {
                vm.ConsumedViewports.Clear();
                vm.ConsumedViewports.AddRange(viewports);
            }
        }

        public IEnumerable<InstallLocation> GetInstallations()
        {
            return _settingsService.GetInstallations();
        }

        public void AddInstalls(string directory)
        {
            _settingsService.AddInstalls(directory);
        }

        public void RemoveInstalls(string installationDirectory)
        {
            _settingsService.RemoveInstalls(installationDirectory);
        }

        public async Task GenerateMonitorConfigAsync()
        {
            var sb = new StringBuilder();

            sb.AppendLine("_  = function(p) return p; end;");
            sb.AppendLine("name = _(\"monitor_config_DAL\");");
            sb.AppendLine("Description = _(\"Monitor-Config created by DCS Alternate Launcher\")");
            sb.AppendLine();
            sb.AppendLine("-- *************** Displays ***************");

            var screens = Screen.AllScreens.OrderBy(s => s.DeviceName).ToArray();
            var screensIndexByName = new Dictionary<string, int>();
            var usedScreens = new List<Screen>();

            sb.AppendLine("displays =");
            sb.AppendLine("{");

            var realBounds = new Rect();

            for (var i = 0; i < screens.Length; i++)
            {
                var screen = screens[i];
                realBounds.Union(screen.Bounds);
            }

            var xOffset = Math.Abs(Screen.PrimaryScreen.Bounds.X - realBounds.X);
            var yOffset = Math.Abs(Screen.PrimaryScreen.Bounds.Y - realBounds.Y);

            for (var i = 0; i < screens.Length; i++)
            {
                var screen = screens[i];

                screensIndexByName.Add(screen.DeviceName, i + 1);

                sb.AppendLine($"    [{i + 1}] = ");
                sb.AppendLine("    {");
                sb.AppendLine($"        -- {screen.DeviceName},");
                sb.AppendLine($"        x = {xOffset + screen.Bounds.X},");
                sb.AppendLine($"        y = {yOffset + screen.Bounds.Y},");
                sb.AppendLine($"        width = {screen.Bounds.Width},");
                sb.AppendLine($"        height = {screen.Bounds.Height}");
                sb.AppendLine("    },");
            }

            sb.AppendLine("}");
            sb.AppendLine();

            sb.AppendLine("Viewports =");
            sb.AppendLine("{");
            sb.AppendLine("    Center =");
            sb.AppendLine("    {");
            sb.AppendLine($"        x = {xOffset},");
            sb.AppendLine($"        y = {yOffset},");
            sb.AppendLine($"        width = {Screen.PrimaryScreen.Bounds.Width},");
            sb.AppendLine($"        height = {Screen.PrimaryScreen.Bounds.Height},");
            sb.AppendLine("        viewDx = 0,");
            sb.AppendLine("        viewDy = 0,");
            sb.AppendLine($"        aspect = {Screen.PrimaryScreen.Bounds.Width} / {Screen.PrimaryScreen.Bounds.Height},");
            sb.AppendLine("    },");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("UIMainView = Viewports.Center");
            sb.AppendLine();

            usedScreens.Add(Screen.PrimaryScreen);

            var viewportTemplates = _profileSettingsService.GetViewportTemplates();
            var installedModules = await _dcsWorldService.GetInstalledAircraftModulesAsync();

            realBounds = Screen.PrimaryScreen.Bounds;

            foreach (var template in viewportTemplates)
            {
                if (!IsValidViewports(template.Viewports.ToArray()))
                {
                    continue;
                }

                var module = installedModules.FirstOrDefault(m => m.ModuleId == template.ModuleId);

                if (module == null)
                {
                    Tracer.Warn($"Could not patch viewport for module {template.ModuleId} because the module is not installed.");
                    continue;
                }

                Tracker.Instance.SendEvent(AnalyticsCategories.Viewports, "generating_viewports", module.ModuleId);

                sb.AppendLine($"-- *************** {module.DisplayName} ***************");
                sb.AppendLine();
                sb.AppendLine();

                foreach (var viewport in template.Viewports)
                {
                    var screen = screens.Single(s => s.DeviceName == viewport.MonitorId);
                    var displayIndex = screensIndexByName[screen.DeviceName];
                    var originalWidth = (double) viewport.OriginalDisplayWidth;
                    var originalHeight = (double) viewport.OriginalDisplayHeight;
                    var ratioX = viewport.X / originalWidth;
                    var ratioY = viewport.Y / originalHeight;
                    var ratioW = viewport.Width / originalWidth;
                    var ratioH = viewport.Height / originalHeight;

                    var x = (int) Math.Round(screen.Bounds.Width * ratioX, 0, MidpointRounding.AwayFromZero);
                    var y = (int) Math.Round(screen.Bounds.Height * ratioY, 0, MidpointRounding.AwayFromZero);
                    var w = (int) Math.Round(screen.Bounds.Width * ratioW, 0, MidpointRounding.AwayFromZero);
                    var h = (int) Math.Round(screen.Bounds.Height * ratioH, 0, MidpointRounding.AwayFromZero);

                    realBounds.Union(screen.Bounds);

                    sb.AppendLine($"--{viewport.RelativeInitFilePath}");
                    sb.AppendLine($"{template.ViewportPrefix}_{viewport.ViewportName} =");
                    sb.AppendLine("{");
                    sb.AppendLine($"    x = displays[{displayIndex}].x + {x},");
                    sb.AppendLine($"    y = displays[{displayIndex}].y + {y},");
                    sb.AppendLine($"    width = {w},");
                    sb.AppendLine($"    height = {h},");
                    sb.AppendLine("}");
                    sb.AppendLine();

                    if (!usedScreens.Contains(screen))
                    {
                        usedScreens.Add(screen);
                    }
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            var contents = sb.ToString();
            var install = _settingsService.SelectedInstall;
            var monitorConfigDirectory = Path.Combine(install.SavedGamesPath, "Config\\MonitorSetup");
            var monitorConfigPath = Path.Combine(monitorConfigDirectory, "monitor_config_DAL.lua");

            if (!Directory.Exists(monitorConfigDirectory))
            {
                Directory.CreateDirectory(monitorConfigDirectory);
            }

            if (File.Exists(monitorConfigPath))
            {
                if (MessageBoxEx.Show("Monitor config already exists.  Are you sure you want to override it?", "Overwrite?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.WriteAllText(monitorConfigPath, contents);
                }
            }
            else
            {
                File.WriteAllText(monitorConfigPath, contents);
            }

            if (MessageBoxEx.Show("Do you want DCS Alternative Launcher to adjust your Game Resolution and Monitor Config?", "Update DCS", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var context = new OptionLuaContext(install))
                {
                    context.SetValue("graphics", "multiMonitorSetup", "monitor_config_DAL");
                    context.SetValue("graphics", "width", realBounds.Width);
                    context.SetValue("graphics", "height", realBounds.Height);
                    context.Save();
                }
            }
            else
            {
                MessageBoxEx.Show(@"Make sure you setup the proper Monitor config in DCS once it is started." + Environment.NewLine +
                                  @"To do this go to Options -> System -> Monitors and change the drop down to ""monitor_config_DAL""" + Environment.NewLine +
                                  $"Then make sure you change your monitor resolution to at least {realBounds.Width}x{realBounds.Height}.");
            }
        }

        public string[] GetDeviceViewportMonitorIds()
        {
            return _profileSettingsService.GetValue(ProfileSettingsCategories.Viewports, SettingsKeys.DeviceViewportsDisplays, new string[0]);
        }

        public IContainer GetChildContainer()
        {
            return _container.GetChildContainer();
        }

        public ModuleViewportTemplate[] GetViewportTemplates()
        {
            return _profileSettingsService.GetViewportTemplates();
        }

        public ModuleViewportTemplate[] GetDefaultViewportTemplates()
        {
            return _profileSettingsService.GetDefaultViewportTemplates();
        }

        public Task<Module[]> GetInstalledAircraftModulesAsync()
        {
            return _dcsWorldService.GetInstalledAircraftModulesAsync();
        }
        public ModuleViewportTemplate[] GetDefaultViewportTemplatesForModule(string moduleId)
        {
            return _profileSettingsService.GetDefaultViewportTemplates().Where(t => t.ModuleId == moduleId).ToArray();
        }

        public void SaveViewports(string name, string moduleId, Viewport[] viewports)
        {
            _profileSettingsService.ClearViewports(name, moduleId);

            foreach (var viewport in viewports)
            {
                var screen = Screen.AllScreens.First(s => s.DeviceName == viewport.MonitorId);
                _profileSettingsService.UpsertViewport(name, moduleId, screen, viewport);
            }
        }

        public void RemoveViewportTemplate(string moduleId)
        {
            _profileSettingsService.RemoveViewportTemplate(moduleId);
        }

        public Option[] GetAdvancedOptions(string optionsCategory)
        {
            var options = _profileSettingsService.GetAdvancedOptions(optionsCategory);

            foreach (var option in options)
            {
                if (_profileSettingsService.TryGetValue<object>(ProfileSettingsCategories.AdvancedOptions, option.Id, out var value))
                {
                    option.Value = value;
                }
            }

            return options;
        }

        public DcsOptionsCategory[] GetDcsOptionCategories()
        {
            return _profileSettingsService.GetDcsOptions();
        }

        public DcsOptionsCategory[] GetDcsCategoryOptionForInstall(InstallLocation install, bool isVr)
        {
            var categories = _profileSettingsService.GetDcsOptions();

            using (var context = new OptionLuaContext(install))
            {
                foreach (var category in categories)
                {
                    foreach (var option in category.Options)
                    {
                        if (!_profileSettingsService.TryGetValue<object>(ProfileSettingsCategories.GameOptions, option.Id, out var value))
                        {
                            value = context.GetValue(category.Id, option.Id);
                        }

                        if (value != null)
                        {
                            var valueStr = value.ToString();
                            var valueType = option.Value.GetType();
                            var converter = TypeDescriptor.GetConverter(valueType);

                            try
                            {
                                option.Value = converter.ConvertFromString(valueStr);
                            }
                            catch (Exception e)
                            {
                                Tracer.Error(e, $"An error occured while trying to convert the value {valueStr} to type {valueType} for option id {option.Id}.");
                            }
                        }
                        else
                        {
                            Tracer.Warn($"Unable to find option value for {category.DisplayName} {option.Id}.  Using default value {option.Value}");
                        }
                    }
                }
            }

            return categories;
        }

        public void UpsertAdvancedOption(string id, object value)
        {
            _profileSettingsService.SetValue(ProfileSettingsCategories.AdvancedOptions, id, value);
        }

        public void UpsertViewportOption(string moduleId, string optionId, object value)
        {
            _profileSettingsService.SetValue(string.Format(ProfileSettingsCategories.ViewportOptionsFormat, moduleId), optionId, value);
        }

        public ViewportOption[] GetViewportOptions(string moduleId)
        {
            var options = _profileSettingsService.GetViewportOptionsByModuleId(moduleId);

            foreach (var option in options)
            {
                if (_profileSettingsService.TryGetValue<object>(string.Format(ProfileSettingsCategories.ViewportOptionsFormat, moduleId), option.Id, out var value))
                {
                    option.Value = value;
                }
            }

            return options;
        }

        public InstallLocation GetCurrentInstall()
        {
            return _settingsService.SelectedInstall;
        }

        public void UpsertDcsOption(string categoryId, string id, object value, bool isVr)
        {
            _profileSettingsService.SetValue(ProfileSettingsCategories.GameOptions, id, value);
        }

        public object ResetAdvancedOptionValue(string categoryId, string optionId)
        {
            var defaultValue = _profileSettingsService.GetAdvancedOptionDefaultValue(categoryId, optionId);

            _profileSettingsService.DeleteValue(ProfileSettingsCategories.AdvancedOptions, optionId);

            return defaultValue;
        }

        public object ResetDcsOption(string categoryId, string optionId)
        {
            var categories = _profileSettingsService.GetDcsOptions();
            var category = categories.FirstOrDefault(c => c.Id == categoryId);
            var option = category.Options.FirstOrDefault(o => o.Id == optionId);

            _profileSettingsService.DeleteValue(ProfileSettingsCategories.GameOptions, optionId);

            return option?.Value;
        }

        public void SaveDcsOptions()
        {
            _dcsWorldService.WriteOptionsAsync(false);
        }

        public void ShowMonitorSetupWizard()
        {
            using (var container = _container.GetChildContainer())
            {
                var firstUseWizard = new Wizard();
                var viewModel = new WizardViewModel(container,
                    new SelectGameViewportScreensStepViewModel(container),
                    new SelectUIViewportScreensStepViewModel(container),
                    new SelectDeviceViewportScreensStepViewModel(container));

                firstUseWizard.DataContext = viewModel;
                firstUseWizard.ShowDialog();
            }
        }

        public bool IsValidViewports(Viewport[] viewports)
        {
            var screenIds = GetDeviceViewportMonitorIds();
            var isValid = true;

            for (var i = 0; i < viewports.Length && isValid; i++)
            {
                isValid = screenIds.Contains(viewports[i].MonitorId);
            }

            return isValid;
        }

        public ViewportDevice[] GetViewportDevices(string moduleId)
        {
            return _profileSettingsService.GetViewportDevices(moduleId);
        }

        public ModuleViewportTemplate ShowTemplateSelection(ModuleViewportTemplate[] templates)
        {
            using (var container = _container.GetChildContainer())
            {
                container.Register<SelectViewportWizardController>().AsSingleton();

                var controller = container.Resolve<SelectViewportWizardController>();
                var wizard = new Wizard();

                var steps = new WizardStepBase<SelectViewportWizardController>[]
                {
                    new SelectViewportsWizardStepViewModel(container, templates),
                };

                var viewModel = new WizardViewModel(container, steps);

                wizard.DataContext = viewModel;
                wizard.ShowDialog();

                return controller.SelectedTemplate;
            }
        }

        public void RemoveProfile(string profileName)
        {
            _profileSettingsService.RemoveProfile(profileName);
        }
    }
}