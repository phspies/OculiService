namespace OculiService.Common.Contract
{
  public interface IProtectableItem
  {
    string Name { get; }

    string Path { get; }

    bool IsContainer { get; }

    bool IsReadOnly { get; set; }

    string ItemType { get; }

    SaturationLevel Saturation { get; set; }
  }
}
