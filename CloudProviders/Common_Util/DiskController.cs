using System;
using System.Runtime.Serialization;

[DataContract]
[Serializable]
public enum DiskController
{
    [EnumMember]
    IDE,
    [EnumMember]
    Scsi,
}
