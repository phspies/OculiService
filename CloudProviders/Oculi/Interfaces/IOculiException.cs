using Newtonsoft.Json.Serialization;

namespace OculiService.CloudProviders.Oculi.Interfaces
{
    public interface IOculiException
    {
        void DeserializationExpection(object sender, ErrorEventArgs errorArgs);
    }
}
