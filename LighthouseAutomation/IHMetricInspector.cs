namespace LighthouseAutomation
{
	public interface IHMetricInspector
	{
		ValueTask<TimeSpan?> GetMetricAsync(HMetrics metric);
	}
}
