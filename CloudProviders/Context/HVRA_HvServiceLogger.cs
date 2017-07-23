using OculiService.Common.Logging;
using HvWrapper;
using System;
using System.Diagnostics;

namespace Oculi.Jobs.Context
{
    public class HOculi_HvServiceLogger : IHvServiceLogger
    {
        private ILogger _Logger;

        public HOculi_HvServiceLogger()
        {
        }

        public HOculi_HvServiceLogger(ILogger logger)
        {
            this._Logger = logger;
        }

        public void WriteLine(string msg)
        {
            Console.WriteLine(msg);
            Trace.WriteLine(msg);
            if (this._Logger == null)
                return;
            this._Logger.Verbose(msg);
        }

        public void LogException(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Trace.WriteLine(ex.Message);
            if (this._Logger == null)
                return;
            this._Logger.Verbose(ex);
        }
    }
}
