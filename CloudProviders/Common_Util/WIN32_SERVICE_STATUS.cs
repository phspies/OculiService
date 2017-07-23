public struct WIN32_SERVICE_STATUS
{
    public int serviceType;
    public int currentState;
    public int controlsAccepted;
    public int win32ExitCode;
    public int serviceSpecificExitCode;
    public int checkPoint;
    public int waitHint;
}
