using Serilog.Core;
using Serilog.Events;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.CommonClasses
{
    public static class Logger
    {
        private static string _lastCallerModuleName;
        public static void Log(LogType logType, int code, string title, string message)
        {
            //Logger.Log("Type: " + logType + "{Environment.NewLine}Code: " + code + "{Environment.NewLine}Title:" + title + "{Environment.NewLine}Message: " + message);
            Serilog.Log.Information("Type: " + logType + $"{Environment.NewLine}Code: " + code + $"{Environment.NewLine}Title:" + title + $"{Environment.NewLine}Message: " + message);
        }

        public static void Log(string message, string title = default, LogType logType = LogType.Debug)
        {
            var baseCallingAssemblyName = Assembly.GetCallingAssembly().GetName().Name.Split('.').LastOrDefault()
                ?.Replace("ZK", "ZkTeco");
            Task.Run(() =>
            {
                lock (Serilog.Log.Logger)
                {
                    try
                    {
                        var callingAssemblyName = baseCallingAssemblyName;
                        title = title?.Trim();
                        message = message.Trim();

                        switch (logType)
                        {
                            case LogType.Information:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Information(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                break;
                            case LogType.Debug:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Debug(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                break;
                            case LogType.Error:
                                Console.WriteLine();
                                Serilog.Log.Error(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                Console.WriteLine();
                                break;
                            case LogType.Fatal:
                                Console.WriteLine();
                                Serilog.Log.Fatal(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                Console.WriteLine();
                                break;
                            case LogType.Warning:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Warning(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                Console.WriteLine();
                                break;
                            case LogType.Verbose:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Verbose(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                break;
                            default:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Verbose(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                break;
                        }

                        _lastCallerModuleName = callingAssemblyName;
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            });
        }

        public static void Log(object message, string title = default, LogType logType = LogType.Debug)
        {
            var baseCallingAssemblyName = Assembly.GetCallingAssembly().GetName().Name.Split('.').LastOrDefault()
                ?.Replace("ZK", "ZkTeco");
            Task.Run(() =>
            {
                lock (Serilog.Log.Logger)
                {
                    try
                    {
                        var callingAssemblyName = baseCallingAssemblyName;

                        title = title?.Trim();
                        message = message.ToString().Trim();

                        switch (logType)
                        {
                            case LogType.Information:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Information(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                break;
                            case LogType.Debug:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Debug(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                break;
                            case LogType.Error:
                                Console.WriteLine();
                                Serilog.Log.Error(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                Console.WriteLine();
                                break;
                            case LogType.Fatal:
                                Console.WriteLine();
                                Serilog.Log.Fatal(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                Console.WriteLine();
                                break;
                            case LogType.Warning:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Warning(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                Console.WriteLine();
                                break;
                            case LogType.Verbose:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Verbose(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                break;
                            default:
                                if (!string.Equals(_lastCallerModuleName, callingAssemblyName,
                                    StringComparison.InvariantCultureIgnoreCase)) Console.WriteLine();
                                Serilog.Log.Verbose(
                                    $"[{callingAssemblyName}] : {(title == default ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                //Console.WriteLine(
                                //    $"[{DateTime.Now:HH:mm:ss} VRB, {Thread.CurrentThread.ManagedThreadId}] [{callingAssemblyName}] : {(title == default(string) ? "" : $"Title:{title}{Environment.NewLine}Message: ")}{message}");
                                break;
                        }

                        _lastCallerModuleName = callingAssemblyName;
                    }
                    catch (Exception exception)
                    {
                        Log(exception);
                    }
                }
            });
        }

        public static void Log(Exception exception, string title = default, LogType logType = LogType.Warning)
        {
            var baseCallingAssemblyName = Assembly.GetCallingAssembly().GetName().Name.Split('.').LastOrDefault()
                ?.Replace("ZK", "ZkTeco");
            Task.Run(() =>
            {
                var callingAssemblyName = baseCallingAssemblyName;
                lock (Serilog.Log.Logger)
                {

                    switch (logType)
                    {
                        case LogType.Information:
                            Serilog.Log.Information(exception,
                                $"[{callingAssemblyName}] : {title} {(exception.InnerException != null ? $"{Environment.NewLine}Inner Exception: {exception.InnerException}" : default(string))}");
                            break;
                        case LogType.Debug:
                            Serilog.Log.Debug(exception,
                                $"[{callingAssemblyName}] : {title} {(exception.InnerException != null ? $"{Environment.NewLine}Inner Exception: {exception.InnerException}" : default(string))}");
                            break;
                        case LogType.Verbose:
                            Serilog.Log.Verbose(exception,
                                $"[{callingAssemblyName}] : {title} {(exception.InnerException != null ? $"{Environment.NewLine}Inner Exception: {exception.InnerException}" : default(string))}");
                            break;
                        case LogType.Error:
                            Console.WriteLine();
                            Serilog.Log.Error(exception,
                                $"[{callingAssemblyName}] : {title} {(exception.InnerException != null ? $"{Environment.NewLine}Inner Exception: {exception.InnerException}" : default(string))}");
                            Console.WriteLine();
                            break;
                        case LogType.Fatal:
                            Console.WriteLine();
                            Serilog.Log.Error(exception,
                                $"[{callingAssemblyName}] : {title} {(exception.InnerException != null ? $"{Environment.NewLine}Inner Exception: {exception.InnerException}" : default(string))}");
                            Console.WriteLine();
                            break;
                        case LogType.Warning:
                            Console.WriteLine();
                            Serilog.Log.Warning(exception,
                                $"[{callingAssemblyName}] : {title} {(exception.InnerException != null ? $"{Environment.NewLine}Inner Exception: {exception.InnerException}" : default(string))}");
                            Console.WriteLine();
                            break;
                        default:
                            Serilog.Log.Warning(exception,
                                $"[{callingAssemblyName}] : {title} {(exception.InnerException != null ? $"{Environment.NewLine}Inner Exception: {exception.InnerException}" : default(string))}");
                            Console.WriteLine();
                            break;
                    }
                }
            });
        }
    }

    /// <summary>
    /// ExceptionType is a enum which designed for diffrentiating between 2 kind of logs: Critical and Optional
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// The error which cause failing some oprations.
        /// </summary>
        Error,
        /// <summary>
        /// The Informational logs which can be neglected.
        /// </summary>
        Information,
        Debug,
        Warning,
        Verbose,
        Fatal
    }

    public class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ThreadId", Thread.CurrentThread.ManagedThreadId));
        }
    }
}
