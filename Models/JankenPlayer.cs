namespace JankenGame.Models
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

