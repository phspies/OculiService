namespace OculiService.CloudProviders.VMware
{
  public interface ICredential
  {
    string Username { get; }

    string Password { get; }
  }
}
