using OculiService.Common;
using Oculi.Core;
using OculiService.CloudProviders.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oculi.Jobs.Context
{
    public static class ErrorInfoExtensions
    {
        public static IEnumerable<LowLevelStateData> ToLowLevelStates(this List<ErrorInfo> details)
        {
            Invariant.ArgumentNotNull((object)details, "details");
            return details.Select<ErrorInfo, LowLevelStateData>((Func<ErrorInfo, LowLevelStateData>)(errInfo => new LowLevelStateData() { Health = errInfo.Health, MessageId = errInfo.Message, HighLevelState = HighLevelState.Protecting }));
        }
    }
}
