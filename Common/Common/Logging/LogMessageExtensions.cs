using System;using System.Collections.Generic;
using System.Text;

namespace OculiService.Common.Logging
{
  public static class LogMessageExtensions
  {
    public static void WriteAttemptedToAddCredentialWithEmptyUserName(this ILogger logger, Guid requestId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Attempted to add a credential with an empty UserName (Request Id - {0}).", new object[1]
      {
        (object) requestId
      });
    }

    public static void WriteAttemptingToSetNullCredentialsOnRequest(this ILogger logger, Guid requestId, Guid workflowId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Attempting to set null credentials on the request (Id - {0}, Workflow - {1}).", new object[2]
      {
        (object) requestId,
        (object) workflowId
      });
    }

    public static void WriteBothServicesNull(this ILogger logger, string state, Guid workflowId, string serverName)
    {
      logger.LogVerbose(LoggerCategory.Debug, "{0} - Both Management Service and Core Service are null (Workflow ID: {1}, Server Name: {2}).", (object) state, (object) workflowId, (object) serverName);
    }

    public static void WriteCallbackDidNotEndProperly(this ILogger logger, string callbackName)
    {
      logger.LogVerbose(LoggerCategory.Debug, "{0} did not end properly.", new object[1]
      {
        (object) callbackName
      });
    }

    public static void WriteCallbackWasCalledUnexpectedWorkflowId(this ILogger logger)
    {
      logger.LogWarning(LoggerCategory.Debug, "A callback was called with an unexpected workflow ID.", new object[0]);
    }

    public static void WriteCallbackWasCalledWithoutIAsyncResult(this ILogger logger)
    {
      logger.LogWarning(LoggerCategory.Debug, "A callback was called without an IAsyncResult.", new object[0]);
    }

    public static void WriteClientProxyCreatedNotHandled(this ILogger logger)
    {
      logger.LogWarning(LoggerCategory.Debug, "ClientProxyCreated was not handled, possibly because it contained an unexpected or null client.", new object[0]);
    }

    public static void WriteClusterInterfaceNotFoundOnServer(this ILogger logger, string serverName)
    {
      logger.LogWarning(LoggerCategory.Debug, "The cluster interface was expected but not found on server {0}.", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteClusterServiceReturnedUnknownAddressOwnershipModelOrUnexpectedData(this ILogger logger, string serverName)
    {
      logger.LogWarning(LoggerCategory.Debug, "Cluster service on '{0}' returned unknown address ownership model or unexpected data.", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteConnectionError(this ILogger logger, Exception exception, string stateType, string requestId, string serviceConnectorId)
    {
      logger.LogWarning(LoggerCategory.Debug, "(Request - {2}, WorkflowId - {3}) A connection error has occurred in the ({1}) state: ({0}).", (object) (exception == null ? "the exception was null" : exception.ToString()), (object) stateType, (object) requestId, (object) serviceConnectorId);
    }

    public static void WriteControlledServerInfoNonExistentRequest(this ILogger logger, Guid serviceConnectorId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Controlled Server Info returned for non-existent request, id {0}.", new object[1]
      {
        (object) serviceConnectorId
      });
    }

    public static void WriteCoreEngineNotFoundOnServer(this ILogger logger, string serverName)
    {
      logger.LogWarning(LoggerCategory.Debug, "The core engine interface was expected but not found on server {0}.", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteCouldNotGetFailbackOptions(this ILogger logger, string jobName, string owningServerName)
    {
      logger.LogWarning(LoggerCategory.Troubleshooting, "Could not get recommended failback options for job {0}, using job server {1}.", new object[2]
      {
        (object) jobName,
        (object) owningServerName
      });
    }

    public static void WriteCouldNotGetFailbackOptionsNullJobManager(this ILogger logger, string jobName, string owningServerName)
    {
      logger.LogWarning(LoggerCategory.Troubleshooting, "JobManager is null. Could not get recommended failback options for job {0}, using job server {1}.", new object[2]
      {
        (object) jobName,
        (object) owningServerName
      });
    }

    public static void WriteCouldNotGetRecommendedRestoreOptions(this ILogger logger, string jobName, string owningServerName)
    {
      logger.LogWarning(LoggerCategory.Troubleshooting, "Could not get recommended restore options for job {0}, using job server {1}.", new object[2]
      {
        (object) jobName,
        (object) owningServerName
      });
    }

    public static void WriteCouldNotGetRecommendedRestoreOptions(this ILogger logger, Exception exception, string jobName, string owningServerName, string restoreServerName)
    {
      logger.LogWarning(LoggerCategory.Troubleshooting, exception, "Could not get recommended restore options for job {0}, using job server {1} and restore server {2}.", (object) jobName, (object) owningServerName, (object) restoreServerName);
    }

    public static void WriteCouldNotParseNatAddress(this ILogger logger, string natEndpoint, string serverName)
    {
      logger.LogWarning(LoggerCategory.Debug, "Could not parse the NAT address '{0}' reported by server '{1}'", new object[2]
      {
        (object) natEndpoint,
        (object) serverName
      });
    }

    public static void WriteCurrentProxiedRequestNull(this ILogger logger, string serverName, Guid workflowId)
    {
      logger.LogVerbose(LoggerCategory.Debug, "Current Proxied Request is null but shouldn't be (Proxy: {0}, Workflow: {1}).", new object[2]
      {
        (object) serverName,
        (object) workflowId
      });
    }

    public static void WriteDashboardInvalidCombinationOfPollingOptions(this ILogger logger)
    {
      logger.LogWarning(LoggerCategory.Debug, "Dashboard internal state is possibly inconsistent.  Invalid combination of polling options.", new object[0]);
    }

    public static void WriteDashboardStartedSuccessfully(this ILogger logger)
    {
      logger.LogInformation(LoggerCategory.Debug, "DashboardEngine started successfully.", new object[0]);
    }

    public static void WriteDataCollectorDetectedMultipleConnectionInfoSameId(this ILogger logger, string serverNetworkId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Data Collector detected multiple ConnectionInfo with the same ID from '{0}'.", new object[1]
      {
        (object) serverNetworkId
      });
    }

    public static void WriteDataCollectorDetectedMultipleMonitorInfoSameId(this ILogger logger, string serverNetworkId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Data Collector detected multiple MonitorInfo with the same ID from '{0}'.", new object[1]
      {
        (object) serverNetworkId
      });
    }

    public static void WriteDataCollectorDetectedMultipleTargetStateInfoSameId(this ILogger logger, string serverNetworkId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Data Collector detected multiple TargetStateInfo with the same ID from '{0}'.", new object[1]
      {
        (object) serverNetworkId
      });
    }

    public static void WriteErrorInDiscoveryService(this ILogger logger, Exception exception, string name)
    {
      logger.LogError(LoggerCategory.Extensibility, "Error in discovery service {0}: {1}.", new object[2]
      {
        (object) name,
        (object) exception.Message
      });
    }

    public static void WriteException(this ILogger logger, Exception exception)
    {
      logger.LogError(LoggerCategory.Debug, exception);
    }

    public static void WriteExpectedDataAccessSolutionWasNotFound(this ILogger logger, Guid recognizedId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Expected DataAccessSolution was not found, RecognizedId - {0}.", new object[1]
      {
        (object) recognizedId
      });
    }

    public static void WriteFailedToConnectServer(this ILogger logger, string serverName, Guid workflowId, Exception exception)
    {
      logger.LogInformation(exception, "Failed to connect to the server '{0}' (WorkflowId - {1}).", new object[2]
      {
        (object) serverName,
        (object) workflowId
      });
    }

    public static void WriteFaultDuringTestMessageLegacySend(this ILogger logger, Exception exception)
    {
      logger.LogError(exception, "fault during test-message legacy send attempt.", new object[0]);
    }

    public static void WriteFaultDuringTestMessageSend(this ILogger logger, Exception exception)
    {
      logger.LogError(exception, "Fault during test-message send attempt.", new object[0]);
    }

    public static void WriteGettingControlledServerInfoStateExpectsInfo(this ILogger logger)
    {
      logger.LogWarning(LoggerCategory.Debug, "GettingControlledServerInfoState state expects a ControlledServerInfo.", new object[0]);
    }

    public static void WriteGettingProductInfoExpectsPartialRawData(this ILogger logger)
    {
      logger.LogWarning(LoggerCategory.Debug, "GettingProductInfo state expects partial raw data.", new object[0]);
    }

    public static void WriteGettingProxiedProductInfoStateExpectsPartialRawData(this ILogger logger)
    {
      logger.LogWarning(LoggerCategory.Debug, "GettingProxiedProductInfoState state expects partial raw data.", new object[0]);
    }

    public static void WriteInsufficientCredentials(this ILogger logger, string userName, string domain, string connectionString)
    {
      logger.LogWarning(LoggerCategory.Debug, "Insufficient Credentials (UserName - '{0}', Domain - '{1}', Connection - '{2}')", (object) userName, (object) domain, (object) connectionString);
    }

    public static void WriteInvalidMatchOfARequest(this ILogger logger, Guid requestId, Guid matchingRequestId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Invalid match of a request (Request Id - '{0}', Matching Request Id - '{1}')", new object[2]
      {
        (object) requestId,
        (object) matchingRequestId
      });
    }

    public static void WriteJobDeletePromptAlreadyRegistered(this ILogger logger, string jobType)
    {
      logger.LogWarning(LoggerCategory.Extensibility, "A job delete prompt handler for job type '{0}' is already registered.", new object[1]
      {
        (object) jobType
      });
    }

    public static void WriteJobInfoNonExistentRequest(this ILogger logger, Guid serviceConnectorId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Job info returned for non-existent request, id {0}.", new object[1]
      {
        (object) serviceConnectorId
      });
    }

    public static void WriteJobManagerNotFoundOnServer(this ILogger logger, string serverName)
    {
      logger.LogWarning(LoggerCategory.Debug, "The job manager interface was expected but not found on server {0}.", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteJobPropertiesHandlerAlreadyRegistered(this ILogger logger, string jobType)
    {
      logger.LogWarning(LoggerCategory.Extensibility, "A job properties handler for job type '{0}' is already registered.", new object[1]
      {
        (object) jobType
      });
    }

    public static void WriteJobWithTargetMatchedMultipleServers(this ILogger logger, string server)
    {
      logger.LogWarning(LoggerCategory.Debug, "A job with target '{0}' matched multiple servers.", new object[1]
      {
        (object) server
      });
    }

    public static void WriteJobWithTargetMatchedMultipleServers(this ILogger logger, string server, string uniqueId)
    {
      logger.LogWarning(LoggerCategory.Debug, "A job with target '{0} - {1}' matched multiple servers.", new object[2]
      {
        (object) server,
        (object) (uniqueId ?? "null")
      });
    }

    public static void WriteModelInconsistencyFound(this ILogger logger, string methodName, List<string> errorList)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(string.Format((IFormatProvider) logger.Culture, "Model inconsistency found in method( {0} )", new object[1]
      {
        (object) methodName
      }));
      errorList.ForEach((Action<string>) (x => sb.AppendLine(x)));
      logger.LogWarning(LoggerCategory.Debug, sb.ToString(), new object[0]);
    }

    public static void WriteModifyingJob(this ILogger logger, string optionsXml)
    {
      logger.LogVerbose("Modifying job with options:\n{0}", new object[1]
      {
        (object) optionsXml
      });
    }

    public static void WriteNoAccessForServer(this ILogger logger, Exception exception, string serverName)
    {
      logger.LogWarning(exception, "NoAccess for server '{0}'.", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteNoJobDeletePromptRegistered(this ILogger logger, string jobType)
    {
      logger.LogWarning(LoggerCategory.Extensibility, "No job delete prompt handler for job type '{0}' is registered.", new object[1]
      {
        (object) jobType
      });
    }

    public static void WriteNonExistentJob(this ILogger logger, Guid jobId)
    {
      logger.LogVerbose(LoggerCategory.Debug, "Job info returned for non-existent job, id {0}.", new object[1]
      {
        (object) jobId
      });
    }

    public static void WriteNonExistentServer(this ILogger logger, Guid serverId)
    {
      logger.LogVerbose(LoggerCategory.Debug, "Server data returned for non-existent server, id {0}.", new object[1]
      {
        (object) serverId
      });
    }

    public static void WriteEventNotHandled(this ILogger logger, string eventName, Guid requestId)
    {
      logger.LogInformation(LoggerCategory.Debug, "Event ({0}) was not handled by the RequestStateMachine for request {1}.", new object[2]
      {
        (object) eventName,
        (object) requestId
      });
    }

    public static void WriteReplicationServiceViewStoppingMonitoring(this ILogger logger, string serverName)
    {
      logger.LogInformation("Replication service view stopping monitoring of {0}", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteSchedulerAddedNewRequest(this ILogger logger, Guid requestId)
    {
      logger.LogInformation(LoggerCategory.Debug, "Scheduler added new request {0}", new object[1]
      {
        (object) requestId
      });
    }

    public static void WriteServerMatchedMultipleServers(this ILogger logger, string serverName)
    {
      logger.LogWarning(LoggerCategory.Debug, "A server '{0}' matched multiple servers.", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteServerRefreshFailed(this ILogger logger, Exception exception)
    {
      logger.LogError(exception, "Server refresh failed.", new object[0]);
    }

    public static void WriteServerResponseUnrecognizedObject(this ILogger logger)
    {
      logger.LogWarning(LoggerCategory.Debug, "A server response with an unrecognized object in AsyncState was encountered.", new object[0]);
    }

    public static void WriteServerResponseWithEmptyWorkflowId(this ILogger logger)
    {
      logger.LogWarning(LoggerCategory.Debug, "A server response with an empty workflow ID was encountered.", new object[0]);
    }

    public static void WriteStartUsingDeterministicServerForWorkflowId(this ILogger logger, Guid workflowId)
    {
      logger.LogInformation(LoggerCategory.Debug, "StartUsingDeterministicServer created new server extension for workflow ID {0}", new object[1]
      {
        (object) workflowId
      });
    }

    public static void WriteStartUsingServerCreatedNewExtension(this ILogger logger, Guid workflowId)
    {
      logger.LogInformation(LoggerCategory.Debug, "StartUsingServer created new server extension for workflow ID {0}", new object[1]
      {
        (object) workflowId
      });
    }

    public static void WriteStartUsingServerOnExistingWorkflowId(this ILogger logger, Guid workflowId)
    {
      logger.LogInformation(LoggerCategory.Debug, "StartUsingServer called on existing workflow ID {0}", new object[1]
      {
        (object) workflowId
      });
    }

    public static void WriteTestMessageLegacySendAttempt(this ILogger logger, Exception exception)
    {
      logger.LogError(exception, "Test-message legacy send attempt.", new object[0]);
    }

    public static void WriteTestMessageSendAttempt(this ILogger logger, Exception exception)
    {
      logger.LogError(exception, "Test-message send attempt.", new object[0]);
    }

    public static void WriteUnableToConnectClockAccuracy(this ILogger logger, Exception exception, string serverName)
    {
      logger.LogWarning(exception, "Unable to connect to the server '{0}', verify that date and clock time are accurate.", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteUnableToObtainOrConfigureLegacyEmailSettings(this ILogger logger, Exception exception)
    {
      logger.LogError(exception, "Unable to obtain or configure legacy e-mail settings.", new object[0]);
    }

    public static void WriteUniqueIdsDidNotMatch(this ILogger logger, string hardwareId, string instanceHardwareId)
    {
      logger.LogWarning(LoggerCategory.Debug, "Unique Ids did not match when updating the request's SC ({0}) - ({1})", new object[2]
      {
        (object) hardwareId,
        (object) instanceHardwareId
      });
    }

    public static void WriteUnrecognizedExceptionWhileConnecting(this ILogger logger, Exception exception, string serverName)
    {
      logger.LogWarning(exception, "Unrecognized exception while connecting to server '{0}'.", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteViewingEngineDiagnostics(this ILogger logger, string serverName)
    {
      logger.LogInformation("Viewing Engine diagnostics for server '{0}'", new object[1]
      {
        (object) serverName
      });
    }

    public static void WriteNullSystemStateOptions(this ILogger logger, string message, string source, string target)
    {
      logger.LogWarning(LoggerCategory.Debug, string.Format("{0} (Source - {1}, Target - {2})", (object) message, (object) (source ?? "null"), (object) (target ?? "null")), new object[0]);
    }
  }
}
