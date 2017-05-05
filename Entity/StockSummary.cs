namespace StockWebApi.Entity
{
    public class StockSummary
    {
        public AgregatorSummary PeYear{get; set;}
        public AgregatorSummary NetProfitYear{get; set;}
        public AgregatorSummary GrowthYear{get; set;}
        public AgregatorSummary PegYear{get; set;}
        public AgregatorSummary PeDiffYear{get; set;}
    }
}