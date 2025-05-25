namespace JankenGame.Models.Janken
{
    public class JankenRecord
    {
        public List<JankenPlayer> Winners { get; set; }
        public DateTime Timestamp { get; set; }

        public JankenRecord(List<JankenPlayer> winners)
        {
            Winners = winners ?? throw new ArgumentNullException(nameof(winners), "Winners cannot be null.");
            Timestamp = DateTime.Now;
        }
    }

    public class JankenRecordList : List<JankenRecord>
    {
        public void AddRecord(List<JankenPlayer> winners)
        {
            var record = new JankenRecord(winners);
            this.Add(record);
        }
    }
}
