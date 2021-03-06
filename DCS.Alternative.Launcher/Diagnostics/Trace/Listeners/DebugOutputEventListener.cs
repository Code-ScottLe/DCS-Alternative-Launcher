﻿using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace DCS.Alternative.Launcher.Diagnostics.Trace.Listeners
{
    public class DebugOutputEventListener : EventListener
    {
        private const string Format = "{0} {1:yyyy-MM-dd HH\\:mm\\:ss\\:ffff} {2}";

        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            var output = string.Format(Format, e.Level, DateTime.Now, e.Payload[0]);
            Debug.WriteLine(output);
        }
    }
}