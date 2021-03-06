using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member",
        Target = "DCS.Alternative.Launcher.ConsoleManager.#AllocConsole()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member",
        Target = "DCS.Alternative.Launcher.ConsoleManager.#FreeConsole()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member",
        Target = "DCS.Alternative.Launcher.ConsoleManager.#GetConsoleWindow()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "type",
        Target = "DCS.Alternative.Launcher.Collections.SafeDictionary`2")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member",
        Target = "DCS.Alternative.Launcher.Collections.SafeDictionary`2.#Dispose()")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "m_writer",
        Scope = "member",
        Target = "DCS.Alternative.Launcher.Diagnostics.Trace.Listeners.StreamOuputEventListener.#Dispose()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member",
        Target = "DCS.Alternative.Launcher.Diagnostics.Trace.Listeners.StreamOuputEventListener.#Dispose()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type",
        Target = "DCS.Alternative.Launcher.Models.NewsArticleModel")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type",
        Target = "DCS.Alternative.Launcher.Plugins.Game.Views.GameViewModel")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type",
        Target = "DCS.Alternative.Launcher.Plugins.Settings.Models.ModuleViewportModel")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type",
        Target = "DCS.Alternative.Launcher.Plugins.Settings.Views.SettingsViewModel")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Scope = "type",
        Target = "DCS.Alternative.Launcher.ServiceModel.IoCAutoRegistrationException")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Scope = "type",
        Target = "DCS.Alternative.Launcher.ServiceModel.IoCConstructorResolutionException")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Scope = "type",
        Target = "DCS.Alternative.Launcher.ServiceModel.IoCRegistrationException")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Scope = "type",
        Target = "DCS.Alternative.Launcher.ServiceModel.IoCRegistrationTypeException")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Scope = "type",
        Target = "DCS.Alternative.Launcher.ServiceModel.IoCResolutionException")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Scope = "type",
        Target = "DCS.Alternative.Launcher.ServiceModel.IoCWeakReferenceException")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Scope = "type",
        Target = "DCS.Alternative.Launcher.ServiceModel.NamedParameterOverloads")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Scope = "member",
        Target = "DCS.Alternative.Launcher.Services.IPluginNavigationSite.#PluginRegistered")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type",
        Target = "DCS.Alternative.Launcher.Views.GameModuleModel")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type",
        Target = "DCS.Alternative.Launcher.Windows.MainWindowViewModel")]
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.