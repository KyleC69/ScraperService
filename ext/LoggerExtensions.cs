#pragma warning disable IDE0021


namespace ScraperService
{


      public static class LoggerExtensions
      {
            // CONSTANT REGISTRATION
            private const int UNKNOWN_GENERAL_SERVICE_FAULT = 100;
            private const int PAGE_SCRAPER_SITE_CAPTURE_FAULT = 200;
            private const int PAGE_SCRAPER_NAVIGATION_FAULT = 250;
            private const int PAGE_SCRAPER_LOGIN_FAULT = 275;
            private const int PAGE_SCRAPER_UNKNOWN_FAULT = 299;
            private const int WEB_DRIVER_INITIALIZATION_FAULT = 300;
            private const int DATABASE_GENERAL_ERROR = 1400;
            private const int DATABASE_CONTROLLER_ERROR = 1405;
            private const int DATABASE_QUERY_FAULT = 1425;
            private const int DATABASE_UPDATE_FAULT = 1450;
            private const int TASK_CANCELLED_EXCEPTION = 325;
            private const int TASK_OPERATION_FAULTED = 350;






            //PRIVATE LOGGER MESSAGE METHODS DEFINITIONS
            private static readonly Action<ILogger, Exception?> _UnknownGeneralError;
            private static readonly Action<ILogger, Exception?> _PageScraperSiteCaptureFault;
            private static readonly Action<ILogger, Exception?> _PageScraperUnknownFault;
            private static readonly Action<ILogger, Exception?> _PageScraperNavigationFault;
            private static readonly Action<ILogger, Exception?> _PageScraperSiteLoginFault;
            private static readonly Action<ILogger, Exception?> _WebDriverInitializationFault;
            private static readonly Action<ILogger, Exception?> _DatabaseQueryFault;
            private static readonly Action<ILogger, Exception?> _DatabaseUpdateFault;
            private static readonly Action<ILogger, Exception?> _GeneralDatabaseError;
            private static readonly Action<ILogger, string, Exception?> _DatabaseControllerError;
            private static readonly Action<ILogger, Exception?> _TaskCancelledException;

            private static readonly Func<ILogger, DateTime, IDisposable> _processingWorkScope;




            // LOGGER METHODS DEFINED
            static LoggerExtensions()
            {
                  LogDefineOptions ldi = new() { SkipEnabledCheck = true };

                  _processingWorkScope = LoggerMessage.DefineScope<DateTime>("Processing work, started at: {DateTime}");





                  _PageScraperSiteCaptureFault = LoggerMessage.Define(
                        LogLevel.Error,
                        new EventId(PAGE_SCRAPER_SITE_CAPTURE_FAULT, nameof(PageScraperSiteCaptureFault)),
                  "An error occured during site capture", ldi);

                  //
                  //
                  _PageScraperNavigationFault = LoggerMessage.Define(
                        LogLevel.Error,
                        new EventId(PAGE_SCRAPER_NAVIGATION_FAULT, nameof(PageScraperNavigationFault)),
                        "A Web driver navigation error occured", ldi);
                  //
                  //
                  _DatabaseUpdateFault = LoggerMessage.Define(
                        LogLevel.Error,
                        new EventId(DATABASE_UPDATE_FAULT, nameof(DatabaseUpdateFault)),
                        "A database update fault occured", ldi);
                  //
                  //

                  _UnknownGeneralError = LoggerMessage.Define(
                       LogLevel.Error,
                       new EventId(UNKNOWN_GENERAL_SERVICE_FAULT, nameof(UnknownGeneralError)),
                        "General Unknown Error...", ldi);

                  //
                  // 
                  _DatabaseQueryFault = LoggerMessage.Define(
                        LogLevel.Error,
                        new EventId(DATABASE_QUERY_FAULT, nameof(DatabaseQueryFault)),
                        "A database query fault occured", ldi);
                  //
                  //

                  _GeneralDatabaseError = LoggerMessage.Define(
                              LogLevel.Error,
                              new EventId(DATABASE_GENERAL_ERROR, nameof(GeneralDatabaseError)),
                              "A General database error");


                  _DatabaseControllerError = LoggerMessage.Define<string>(
                        LogLevel.Trace,
                        new EventId(DATABASE_CONTROLLER_ERROR, nameof(DatabaseControllerError)),
                         "General database controller error {message}");

                  _PageScraperSiteLoginFault = LoggerMessage.Define(
                        LogLevel.Error,
                        new EventId(PAGE_SCRAPER_LOGIN_FAULT, nameof(PageScraperSiteLoginFault)),
                        "An error occured during site login...");

                  _PageScraperUnknownFault = LoggerMessage.Define(
                        LogLevel.Error,
                        new EventId(PAGE_SCRAPER_UNKNOWN_FAULT, nameof(PageScraperUnknownFault)),
                         "Unknown page scraper error");

                  _TaskCancelledException = LoggerMessage.Define(
                        LogLevel.Error,
                        new EventId(TASK_CANCELLED_EXCEPTION, nameof(TaskCanceledException)),
                        "A Task was cancelled during execution");


                  _WebDriverInitializationFault = LoggerMessage.Define(
                        LogLevel.Error,
                        new EventId(WEB_DRIVER_INITIALIZATION_FAULT, nameof(WebDriverInitializationFault)),
                        "Edge Webdriver failed to initialize");



            }

            //      LOGGER MESSAGE METHOD IMPLEMENTATIONS
            public static void DatabaseQueryFault(this ILogger logger) => _DatabaseQueryFault(logger, null);
            public static void DatabaseUpdateFault(this ILogger logger) => _DatabaseUpdateFault(logger, null);
            public static void GeneralDatabaseError(this ILogger logger) => _GeneralDatabaseError(logger, null);
            public static void UnknownGeneralError(this ILogger logger) => _UnknownGeneralError(logger, null);
            public static void DatabaseControllerError(this ILogger logger, string message, Exception? ex) => _DatabaseControllerError(logger, message, ex);
            public static void PageScraperNavigationFault(this ILogger logger) => _PageScraperNavigationFault(logger, null);
            public static void PageScraperNavigationFault(this ILogger logger, Exception? ex) => _PageScraperNavigationFault(logger, ex);
            public static void PageScraperSiteLoginFault(this ILogger logger, Exception? ex) => _PageScraperSiteLoginFault(logger, ex);
            public static void PageScraperSiteCaptureFault(this ILogger logger) => _PageScraperSiteCaptureFault(logger, null);
            public static void WebDriverInitializationFault(this ILogger logger) => _WebDriverInitializationFault(logger, null);
            /// <summary>
            /// Register an unknown Page Scaper Fault
            /// </summary>
            /// <param name="logger"></param>
            /// <param name="ex"></param>
            public static void PageScraperUnknownFault(this ILogger logger, Exception? ex) => _PageScraperUnknownFault(logger, ex);
            public static void TaskCanceledException(this ILogger logger) => _TaskCancelledException(logger, null);

            public static IDisposable ProcessingWorkScope(this ILogger logger, DateTime time) => _processingWorkScope(logger, time);



      }
}