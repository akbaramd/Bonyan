namespace Bonyan.AspNetCore.Job
{
  public interface IBonWorkerManager
  {
    void Enqueue<TJob>(string? jobId = null) where TJob : IBonWorker;
    void ScheduleRecurring<TJob>(string scheduleExpression, string? jobId = null) where TJob : IBonWorker;
    void CancelRecurring(string jobId);
  }
}
