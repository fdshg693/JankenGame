namespace JankenGame.Models.Janken
{
    public class JankenRecord
    {
        public JankenHand PlayerHand { get; set; }
        public JankenHand ComputerHand { get; set; }
        public JankenResult Outcome { get; set; }
        public DateTime Timestamp { get; set; }

        public JankenRecord(JankenHand playerHand, JankenHand computerHand, JankenResult outcome)
        {
            PlayerHand = playerHand;
            ComputerHand = computerHand;
            Outcome = outcome;
            Timestamp = DateTime.Now;
        }
    }

    public class JankenRecordList : List<JankenRecord>
    {
        public void AddRecord(JankenHand? playerHand, JankenHand? computerHand, JankenResult outcome)
        {
            if (playerHand == null)
            {
                throw new ArgumentNullException(nameof(playerHand), "Player hand cannot be null.");
            }
            if (computerHand == null)
            {
                throw new ArgumentNullException(nameof(computerHand), "Computer hand cannot be null.");
            }
            var record = new JankenRecord(playerHand.Value, computerHand.Value, outcome);
            this.Add(record);
        }
        public int TotalWins => this.Count(r => r.Outcome == JankenResult.Win);
        public int TotalLosses => this.Count(r => r.Outcome == JankenResult.Lose);
        public int TotalDraws => this.Count(r => r.Outcome == JankenResult.Draw);
    }
}
