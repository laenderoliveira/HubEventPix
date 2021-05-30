namespace WorkerPix.Models
{
    public class WorkerConfiguration
    {
        public string Queue { get; set; }

        public string Exchange { get; set; }

        public string RoutingKey { get; set; }
    }
}
