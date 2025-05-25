namespace JankenGame.Models.Janken
{
    public class JankenPlayer
    {
        public JankenPlayer(string name)
        {
            Hand = null;
            Name = name;
            UniqueId = UniqueIdManager.NextUniqueId();
        }

        public JankenHand? Hand { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UniqueId { get; private set; }

        public void RecreateSelf()
        {
            Hand = null;
        }

        public string ShowInfo()
        {
            return $"{Name}さんの手は{(Hand.HasValue ? Hand.Value.GetDescription() : "未設定")}です。";
        }

        public void ChangePlayerName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Player name cannot be null or empty.", nameof(newName));
            }
            Name = newName;
        }
    }

    public static class UniqueIdManager
    {
        public static int CurrentUniqueId { get; private set; } = 0;
        public static int NextUniqueId()
        {
            return CurrentUniqueId++;
        }
    }

    public class JankenPlayerList : List<JankenPlayer>
    {
        public JankenPlayerList(List<string> playerNames)
        {
            foreach (var name in playerNames)
            {
                this.Add(new JankenPlayer(name));
            }
        }
        public void RecreateSelf()
        {
            foreach (var player in this)
            {
                player.RecreateSelf();
            }
        }

        public void SetPlayerHand(int Id, JankenHand hand)
        {
            var existingPlayer = this.FirstOrDefault(p => p.UniqueId == Id);
            if (existingPlayer != null)
            {
                existingPlayer.Hand = hand;
            }
            else
            {
                throw new InvalidOperationException($"Player with ID {Id} not found in the list.");
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

        public int GetPlayerUniqueId(string name)
        {
            var player = this.FirstOrDefault(p => p.Name == name);
            if (player != null)
            {
                return player.UniqueId;
            }
            else
            {
                throw new InvalidOperationException($"Player with name {name} not found in the list.");
            }
        }

        public void ChangePlayerName(int Id, string newName)
        {
            var player = this.FirstOrDefault(p => p.UniqueId == Id);
            var sameNamePlayer = this.FirstOrDefault(p => p.Name == newName);
            if (sameNamePlayer != null && sameNamePlayer.UniqueId != Id)
            {
                throw new InvalidOperationException($"Player with name {newName} already exists in the list.");
            }
            if (player != null)
            {
                player.ChangePlayerName(newName);
            }
            else
            {
                throw new InvalidOperationException($"Player with ID {Id} not found in the list.");
            }
        }
    }

}

