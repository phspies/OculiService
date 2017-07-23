using OculiService.Jobs.Contract;

namespace OculiService.Jobs.Commands
{
    public class CommandInvoker : ICommandInvoker
    {
        private ICommandFactory _Factory;

        public CommandInvoker(ICommandFactory factory)
        {
            this._Factory = factory;
        }

        public void CreateJobCmd()
        {
            this._Factory.Create<IJobCommandCommon>("CreateJobCmd").Invoke();
        }
        public void CreateJob()
        {
            this._Factory.Create<IJobCommandCommon>("CreateJobCmd").Invoke();
        }
    }
}
