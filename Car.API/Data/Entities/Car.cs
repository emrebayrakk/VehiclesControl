namespace Car.API.Data.Entities
{
    public class Car : Entity
    {
        public long CarId { get; set; }
        public bool HeadlightsOn { get; set; }
        public int Wheels { get; set; }
        public string Color { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
