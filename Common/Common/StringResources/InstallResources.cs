using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace OculiService.Common.StringResources
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class InstallResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (InstallResources.resourceMan == null)
          InstallResources.resourceMan = new ResourceManager("Oculi.Common.StringResources.InstallResources", typeof (InstallResources).Assembly);
        return InstallResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get
      {
        return InstallResources.resourceCulture;
      }
      set
      {
        InstallResources.resourceCulture = value;
      }
    }

    public static string AboutToInstall
    {
      get
      {
        return InstallResources.ResourceManager.GetString("AboutToInstall", InstallResources.resourceCulture);
      }
    }

    public static string AutomaticRebootFailure
    {
      get
      {
        return InstallResources.ResourceManager.GetString("AutomaticRebootFailure", InstallResources.resourceCulture);
      }
    }

    public static string CannotAccessWMI
    {
      get
      {
        return InstallResources.ResourceManager.GetString("CannotAccessWMI", InstallResources.resourceCulture);
      }
    }

    public static string CannotCopyInstallFiles
    {
      get
      {
        return InstallResources.ResourceManager.GetString("CannotCopyInstallFiles", InstallResources.resourceCulture);
      }
    }

    public static string CannotInstall
    {
      get
      {
        return InstallResources.ResourceManager.GetString("CannotInstall", InstallResources.resourceCulture);
      }
    }

    public static string CouldNotFindExistingInstall
    {
      get
      {
        return InstallResources.ResourceManager.GetString("CouldNotFindExistingInstall", InstallResources.resourceCulture);
      }
    }

    public static string OculiAlreadyInstalled
    {
      get
      {
        return InstallResources.ResourceManager.GetString("OculiAlreadyInstalled", InstallResources.resourceCulture);
      }
    }

    public static string ErrorExecutingInstall
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ErrorExecutingInstall", InstallResources.resourceCulture);
      }
    }

    public static string ErrorSchedulingInstall
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ErrorSchedulingInstall", InstallResources.resourceCulture);
      }
    }

    public static string ErrorSchedulingInstallAtProtocol
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ErrorSchedulingInstallAtProtocol", InstallResources.resourceCulture);
      }
    }

    public static string InstallCredentialsRequired
    {
      get
      {
        return InstallResources.ResourceManager.GetString("InstallCredentialsRequired", InstallResources.resourceCulture);
      }
    }

    public static string InstallErrorOccurred
    {
      get
      {
        return InstallResources.ResourceManager.GetString("InstallErrorOccurred", InstallResources.resourceCulture);
      }
    }

    public static string InstallMachineValidationIssue
    {
      get
      {
        return InstallResources.ResourceManager.GetString("InstallMachineValidationIssue", InstallResources.resourceCulture);
      }
    }

    public static string InstallTimedOut
    {
      get
      {
        return InstallResources.ResourceManager.GetString("InstallTimedOut", InstallResources.resourceCulture);
      }
    }

    public static string ManualRebootRequired
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ManualRebootRequired", InstallResources.resourceCulture);
      }
    }

    public static string NoActivationCodes
    {
      get
      {
        return InstallResources.ResourceManager.GetString("NoActivationCodes", InstallResources.resourceCulture);
      }
    }

    public static string PreparingForScheduledInstall
    {
      get
      {
        return InstallResources.ResourceManager.GetString("PreparingForScheduledInstall", InstallResources.resourceCulture);
      }
    }

    public static string ProgressCanAccessWMI
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressCanAccessWMI", InstallResources.resourceCulture);
      }
    }

    public static string ProgressCanCopyFiles
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressCanCopyFiles", InstallResources.resourceCulture);
      }
    }

    public static string ProgressCanInstall
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressCanInstall", InstallResources.resourceCulture);
      }
    }

    public static string ProgressCheckingLogFile
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressCheckingLogFile", InstallResources.resourceCulture);
      }
    }

    public static string ProgressCopyingFiles
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressCopyingFiles", InstallResources.resourceCulture);
      }
    }

    public static string ProgressCouldNotAccessLogFile
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressCouldNotAccessLogFile", InstallResources.resourceCulture);
      }
    }

    public static string ProgressCouldNotSaveLogFileLocally
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressCouldNotSaveLogFileLocally", InstallResources.resourceCulture);
      }
    }

    public static string ProgressOculiNotInstalled
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressOculiNotInstalled", InstallResources.resourceCulture);
      }
    }

    public static string ProgressFilesCopiedSuccessfully
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressFilesCopiedSuccessfully", InstallResources.resourceCulture);
      }
    }

    public static string ProgressHasLicenses
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressHasLicenses", InstallResources.resourceCulture);
      }
    }

    public static string ProgressInstallScheduled
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressInstallScheduled", InstallResources.resourceCulture);
      }
    }

    public static string ProgressInstallSuccess
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressInstallSuccess", InstallResources.resourceCulture);
      }
    }

    public static string ProgressInvalidNumberOfPackageFiles
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressInvalidNumberOfPackageFiles", InstallResources.resourceCulture);
      }
    }

    public static string ProgressLookingForExistingInstall
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressLookingForExistingInstall", InstallResources.resourceCulture);
      }
    }

    public static string ProgressMonitoringInstall
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressMonitoringInstall", InstallResources.resourceCulture);
      }
    }

    public static string ProgressReclaimingActivationCode
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressReclaimingActivationCode", InstallResources.resourceCulture);
      }
    }

    public static string ProgressRemoteExecutionSuccess
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressRemoteExecutionSuccess", InstallResources.resourceCulture);
      }
    }

    public static string ProgressRemoteExecutionSuccessful
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressRemoteExecutionSuccessful", InstallResources.resourceCulture);
      }
    }

    public static string ProgressRestartingMachine
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressRestartingMachine", InstallResources.resourceCulture);
      }
    }

    public static string ProgressRunningInstaller
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressRunningInstaller", InstallResources.resourceCulture);
      }
    }

    public static string ProgressUninstalling
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressUninstalling", InstallResources.resourceCulture);
      }
    }

    public static string ProgressUpgradeMessage
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ProgressUpgradeMessage", InstallResources.resourceCulture);
      }
    }

    public static string PushingUnexpectedError
    {
      get
      {
        return InstallResources.ResourceManager.GetString("PushingUnexpectedError", InstallResources.resourceCulture);
      }
    }

    public static string ScheduledInstallTime
    {
      get
      {
        return InstallResources.ResourceManager.GetString("ScheduledInstallTime", InstallResources.resourceCulture);
      }
    }

    public static string UninstallCredentialsRequired
    {
      get
      {
        return InstallResources.ResourceManager.GetString("UninstallCredentialsRequired", InstallResources.resourceCulture);
      }
    }

    public static string WrongPlatformPackage
    {
      get
      {
        return InstallResources.ResourceManager.GetString("WrongPlatformPackage", InstallResources.resourceCulture);
      }
    }

    internal InstallResources()
    {
    }
  }
}
