using OculiService.Common.Contract;
using OculiService.Jobs.Commands;

namespace OculiService.Interfaces
{
    public interface IEditJobCommand : ITaskCommandBase
    {
        void Invoke(TaskOptions jobOptions);
    }
}