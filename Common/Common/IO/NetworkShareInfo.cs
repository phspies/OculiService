using System.Collections.Generic;
using System.Management;

namespace OculiService.Common.IO
{
  public class NetworkShareInfo
  {
    private string _name;
    private uint _type;
    private string _path;
    private IEnumerable<Ace> _aces;
    private uint? _maxUsers;
    private string _server;

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public NetworkShareType ShareType
    {
      get
      {
        return (NetworkShareType) ((int) ushort.MaxValue & (int) this._type);
      }
    }

    public bool IsAdmin
    {
      get
      {
        return (2147483648U & this._type) > 0U;
      }
    }

    public string Path
    {
      get
      {
        return this._path;
      }
    }

    public IEnumerable<Ace> Aces
    {
      get
      {
        return this._aces;
      }
    }

    public int? MaxUsers
    {
      get
      {
        uint? maxUsers = this._maxUsers;
        if (!maxUsers.HasValue)
          return new int?();
        return new int?((int) maxUsers.GetValueOrDefault());
      }
    }

    public int TypeMask
    {
      get
      {
        return (int) this._type;
      }
    }

    public string Server
    {
      get
      {
        return this._server;
      }
    }

    private NetworkShareInfo(ManagementObject share)
    {
      this._name = (string) share.Properties["Name"].Value;
      this._type = (uint) share.Properties["Type"].Value;
      this._path = (string) share.Properties["Path"].Value;
      this._maxUsers = (uint?) share.Properties["MaximumAllowed"].Value;
    }

    public NetworkShareInfo(string name, int type, string path, IEnumerable<Ace> aces, int? maxUsers, string server = null)
    {
      this._name = name;
      this._type = (uint) type;
      this._path = path;
      this._aces = aces;
      int? nullable = maxUsers;
      this._maxUsers = nullable.HasValue ? new uint?((uint) nullable.GetValueOrDefault()) : new uint?();
      this._server = server;
    }

    public static explicit operator NetworkShareInfo(ManagementObject share)
    {
      return new NetworkShareInfo(share);
    }
  }
}
