namespace Bonyan.AspNetCore.Job;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CronJobAttribute : Attribute
{
  public string CronExpression { get; }

  public CronJobAttribute(string cronExpression)
  {
    CronExpression = cronExpression;
  }
}