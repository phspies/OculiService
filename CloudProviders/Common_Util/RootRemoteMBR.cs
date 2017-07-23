public abstract class RootRemoteMBR : LongLiveMBR, IRemoteRoot
{
    public abstract string RemotingId { get; }

    public abstract void Shutdown();

    public virtual void Heartbeat()
    {
    }
}
