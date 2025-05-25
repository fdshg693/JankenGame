namespace JankenGame.Models.Janken
{
    public class JankenPlayer
    {
        public JankenPlayer()
        {
            Hand = null;
        }

        public JankenHand? Hand { get; set; }
    }
}

