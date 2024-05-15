namespace OrderRestaurant.Model
{
    public class NotificationModel
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public bool IsCheck { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
