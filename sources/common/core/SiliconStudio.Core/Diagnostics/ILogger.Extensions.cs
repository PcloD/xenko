// Copyright (c) 2012-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
//
// File AUTO-GENERATED, do not edit!
using System;
using SiliconStudio.Core.Annotations;

namespace SiliconStudio.Core.Diagnostics
{
    /// <summary>
    /// Extensions for <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs the specified verbose message with an exception.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The verbose message.</param>
        /// <param name="exception">An exception to log with the message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Verbose([NotNull] this ILogger logger, string message, Exception exception, CallerInfo callerInfo = null)
        {
            logger.Log(new LogMessage(logger.Module, LogMessageType.Verbose, message, exception, callerInfo));
        }

        /// <summary>
        /// Logs the specified verbose message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The verbose message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Verbose([NotNull] this ILogger logger, string message, CallerInfo callerInfo = null)
        {
            Verbose(logger, message, null, callerInfo);
        }
        /// <summary>
        /// Logs the specified debug message with an exception.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The debug message.</param>
        /// <param name="exception">An exception to log with the message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Debug([NotNull] this ILogger logger, string message, Exception exception, CallerInfo callerInfo = null)
        {
            logger.Log(new LogMessage(logger.Module, LogMessageType.Debug, message, exception, callerInfo));
        }

        /// <summary>
        /// Logs the specified debug message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The debug message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Debug([NotNull] this ILogger logger, string message, CallerInfo callerInfo = null)
        {
            Debug(logger, message, null, callerInfo);
        }
        /// <summary>
        /// Logs the specified info message with an exception.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The info message.</param>
        /// <param name="exception">An exception to log with the message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Info([NotNull] this ILogger logger, string message, Exception exception, CallerInfo callerInfo = null)
        {
            logger.Log(new LogMessage(logger.Module, LogMessageType.Info, message, exception, callerInfo));
        }

        /// <summary>
        /// Logs the specified info message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The info message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Info([NotNull] this ILogger logger, string message, CallerInfo callerInfo = null)
        {
            Info(logger, message, null, callerInfo);
        }
        /// <summary>
        /// Logs the specified warning message with an exception.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The warning message.</param>
        /// <param name="exception">An exception to log with the message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Warning([NotNull] this ILogger logger, string message, Exception exception, CallerInfo callerInfo = null)
        {
            logger.Log(new LogMessage(logger.Module, LogMessageType.Warning, message, exception, callerInfo));
        }

        /// <summary>
        /// Logs the specified warning message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The warning message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Warning([NotNull] this ILogger logger, string message, CallerInfo callerInfo = null)
        {
            Warning(logger, message, null, callerInfo);
        }
        /// <summary>
        /// Logs the specified error message with an exception.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The error message.</param>
        /// <param name="exception">An exception to log with the message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Error([NotNull] this ILogger logger, string message, Exception exception, CallerInfo callerInfo = null)
        {
            logger.Log(new LogMessage(logger.Module, LogMessageType.Error, message, exception, callerInfo));
        }

        /// <summary>
        /// Logs the specified error message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The error message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Error([NotNull] this ILogger logger, string message, CallerInfo callerInfo = null)
        {
            Error(logger, message, null, callerInfo);
        }
        /// <summary>
        /// Logs the specified fatal message with an exception.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The fatal message.</param>
        /// <param name="exception">An exception to log with the message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Fatal([NotNull] this ILogger logger, string message, Exception exception, CallerInfo callerInfo = null)
        {
            logger.Log(new LogMessage(logger.Module, LogMessageType.Fatal, message, exception, callerInfo));
        }

        /// <summary>
        /// Logs the specified fatal message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The fatal message.</param>
        /// <param name="callerInfo">Information about the caller. Default is null, otherwise use <see cref="CallerInfo.Get"/>.</param>
        public static void Fatal([NotNull] this ILogger logger, string message, CallerInfo callerInfo = null)
        {
            Fatal(logger, message, null, callerInfo);
        }
    }
}
