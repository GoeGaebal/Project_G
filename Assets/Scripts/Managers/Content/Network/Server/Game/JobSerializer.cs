using System.Collections.Generic;
using Unity.Jobs;

public class JobSerializer
{
    public struct EnterGameJob : IJob
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
    
    Queue<IJob> _jobQueue = new Queue<IJob>();
    
}