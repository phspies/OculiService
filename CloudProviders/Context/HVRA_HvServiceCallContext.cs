using HvWrapper;

namespace Oculi.Jobs.Context
{
    public class HOculi_HvServiceCallContext : IHvServiceCallContext
    {
        public bool Stop
        {
            get
            {
                return false;
            }
        }

        public long Timeout
        {
            get
            {
                return 0;
            }
        }
    }
}
