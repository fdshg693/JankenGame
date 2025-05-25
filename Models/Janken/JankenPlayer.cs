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

        public JankenPlayer RecreateSelf()
        {
            return new JankenPlayer(Name)
            {
            };
        }

        public string ShowInfo()
        {
            return $"{Name}さんの手は{(Hand.HasValue ? Hand.Value.GetDescription() : "未設定")}です。";
        }
    }

    public class JankenPlayerList : List<JankenPlayer>
    {
        public JankenPlayerList players => this;
        public JankenPlayerList(List<string> playerNames)
        {
            foreach (var name in playerNames)
            {
                this.Add(new JankenPlayer(name));
            }
        }
        public JankenPlayerList RecreateSelf()
        {
            return new JankenPlayerList(this.Select(p => p.Name).ToList());
        }

        public void SetPlayerHand(string playerName, JankenHand hand)
        {
            var existingPlayer = this.FirstOrDefault(p => p.Name == playerName);
            if (existingPlayer != null)
            {
                existingPlayer.Hand = hand;
            }
            else
            {
                throw new InvalidOperationException($"Player {playerName} not found in the list.");
            }
        }
        public string ShowInfo()
        {
            var info = string.Empty;
            foreach (var player in this)
            {
                info += player.ShowInfo() + "\n";
            }
            return info;
        }
    }

}

