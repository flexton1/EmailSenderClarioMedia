namespace EmailSender.Model
{
    public class EmailRequest
    {
        public string TeamName { get; set; }
        public string Country { get; set; }
        public string Representative { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Players { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public string Notes { get; set; }
    }
    
    public class ContactEmailRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}