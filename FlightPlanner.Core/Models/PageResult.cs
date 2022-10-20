using System.Collections.Generic;

namespace FlightPlanner
{
    public class PageResult
    {
        public List<Flight> Items { get; set; }
        public int Page { get; set; }
        public int TotalItems { get; set; }

        public PageResult(List<Flight> itemList)
        {
            Items = itemList;
            Page = 0;
            TotalItems = itemList.Count;
        }
    }
}
