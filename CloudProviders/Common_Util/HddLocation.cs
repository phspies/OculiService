public class HddLocation
{
    public uint Bus;
    public uint Lun;
    public uint Port;
    public uint Target;

    public override string ToString()
    {
        return string.Format("bus={0}, lun={1}, port={2}, target={3}", (object)this.Bus, (object)this.Lun, (object)this.Port, (object)this.Target);
    }
}
