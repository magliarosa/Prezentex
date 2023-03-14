namespace Prezentex.Domain.Entities
{
    public class Notification : Entity
    {
        public string Content { get; set; }
        public string Receiver { get; set; }
    }
}
