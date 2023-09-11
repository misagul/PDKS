namespace PDKS.Models
{
	public class ChartDataModel
	{
		public List<string> labels { get; set; }
		public List<Datasets> datasets { get; set; }

		public class Datasets
		{
			public string label { get; set; }
			public List<int> data { get; set; }
			public string backgroundColor { get; set; }

        }
	}
}
