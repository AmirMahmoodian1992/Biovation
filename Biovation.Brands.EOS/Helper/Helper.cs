using System;
using System.Threading;
using EosClocks;
using Serilog;

namespace Biovation.Brands.EOS.Helper
{
    public static class Helper
    {
        public static T Attempt<T>(this int attempts, ILogger logger, Func<T> callFunction, Clock clock = null)
        {
            if (clock is null)
            {
                for (var i = 0; i < attempts; i++)
                {
                    try
                    {
                        return callFunction();
                    }
                    catch (Exception exception)
                    {
                        logger.Debug(exception, exception.Message);
                        Thread.Sleep(++i * 100);
                    }
                }

                logger.Debug($"Failed to call {callFunction.Method.Name}");
                return default;
            }
            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    lock (clock)
                        return callFunction();
                }
                catch (Exception exception)
                {
                    logger.Debug(exception, exception.Message);
                    Thread.Sleep(++i * 100);
                }
            }

            logger.Debug($"Failed to call {callFunction.Method.Name}");
            return default;

        }
    }
}


//for (var i = 0; i < 5;)
//{
//    try
//    {
//        fingerTemplates = _clock.Sensor.GetUserTemplates((int)userId);
//        break;
//    }
//    catch (Exception exception)
//    {
//        _logger.Debug(exception, exception.Message);
//        Thread.Sleep(++i * 100);
//    }
//}