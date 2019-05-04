using System;
using System.Collections.Generic;
using System.Text;

namespace UserDetailsClient.Core.Features.Logging
{
    public interface ILoggingService
    {
        void Debug(string message);

        void Warning(string message);

        void Error(Exception exception);
    }
}