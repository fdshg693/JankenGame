namespace JankenGame.Models.BlackJack
{
    public class Deck
    {
        private Stack<Card> _cards;
        private static readonly Random _rnd = new();

        public Deck()
        {
            var cards = new List<Card>();
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
                foreach (Rank r in Enum.GetValues(typeof(Rank)))
                    cards.Add(new Card { Suit = s, Rank = r });
            // シャッフル
            _cards = new Stack<Card>(cards.OrderBy(_ => _rnd.Next()));
        }

        public Card Draw() => _cards.Pop();
    }
}
