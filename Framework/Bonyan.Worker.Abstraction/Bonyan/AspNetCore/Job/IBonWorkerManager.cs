namespace Bonyan.AspNetCore.Job
{
    public interface IBonWorkerManager
    {
        void Enqueue<TJob>(string? jobId = null) where TJob : IBonWorker;
        void Enqueue(Type jobType, string? jobId = null);

        void ScheduleRecurring<TJob>(string scheduleExpression, string? jobId = null) where TJob : IBonWorker;
        void ScheduleRecurring(Type jobType, string scheduleExpression, string? jobId = null);

        void CancelRecurring(string jobId);
    }
}