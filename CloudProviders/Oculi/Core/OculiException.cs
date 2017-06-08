using Newtonsoft.Json.Serialization;
using OculiService.CloudProviders.Oculi.Interfaces;

namespace OculiService.CloudProviders.Oculi
{
    public class OculiException : IOculiException
    {
        public void DeserializationExpection(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
