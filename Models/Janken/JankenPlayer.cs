namespace JankenGame.Models.Janken
{
    public class JankenPlayer
    {
        public JankenPlayer(string name)
        {
            Hand = null;
            Name = name;
        }

        public JankenHand? Hand { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

