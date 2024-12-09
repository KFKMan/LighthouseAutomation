namespace LighthouseAutomation
{
	public interface IMetricInspector
	{
		ValueTask<ushort?> GetMetricAsync(Metrics metric);
	}
}
