using JankenGame.Models.BlackJack;

namespace JankenGame.Services.BlackJack
{
    /// <summary>
    /// ブラックジャックのゲーム状態管理とデッキ管理を行うサービス
    /// </summary>
    public class BlackJackGameService
    {
        private Deck _deck;
        private readonly List<Card> _cardsInPlay = new();

        public BlackJackGameService()
        {
            _deck = new Deck();
        }

        /// <summary>
        /// カードを引く（デッキが空になったら再構築）
        /// </summary>
        public Card DrawCard()
        {
            Card card;
            try
            {
                card = _deck.Draw();
            }
            catch (InvalidOperationException)
            {
                // デッキが空になったら、場にあるカードを除いて再構築
                RebuildDeck();
                card = _deck.Draw();
            }
            
            _cardsInPlay.Add(card);
            return card;
        }

        /// <summary>
        /// ゲームをリセット（カードを場から回収）
        /// </summary>
        public void ReturnCards(BlackJackPlayer player, BlackJackDealer dealer)
        {
            // 場にあるカードをクリア（実際は回収されたものとして扱う）
            _cardsInPlay.Clear();
        }

        /// <summary>
        /// ゲームをリセット（カードを場から回収）- BlackJackHand版（互換性のため残す）
        /// </summary>
        public void ReturnCards(BlackJackHand player, BlackJackHand dealer)
        {
            // 場にあるカードをクリア（実際は回収されたものとして扱う）
            _cardsInPlay.Clear();
        }

        /// <summary>
        /// デッキを再構築（場にあるカードを除く）
        /// </summary>
        private void RebuildDeck()
        {
            // 全カードを生成
            var allCards = Deck.CreateAllCards();

            // 場にあるカードを除外
            var availableCards = allCards
                .Where(card => !_cardsInPlay.Any(inPlay => 
                    inPlay.Suit == card.Suit && inPlay.Rank == card.Rank))
                .ToList();

            _deck = new Deck(availableCards);
        }
    }
}
