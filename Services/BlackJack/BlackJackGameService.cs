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
        private readonly BlackJackLogicService _logicService;

        // ゲーム状態管理
        public BlackJackGameState GameState { get; private set; }
        public List<BlackJackPlayer> Players { get; private set; }
        public BlackJackDealer Dealer { get; private set; }
        public int CurrentPlayerIndex { get; private set; }
        public BlackJackPlayer? CurrentPlayer => 
            CurrentPlayerIndex >= 0 && CurrentPlayerIndex < Players.Count 
                ? Players[CurrentPlayerIndex] 
                : null;

        public BlackJackGameService()
        {
            _deck = new Deck();
            _logicService = new BlackJackLogicService();
            Players = new List<BlackJackPlayer>();
            Dealer = new BlackJackDealer();
            GameState = BlackJackGameState.Waiting;
            CurrentPlayerIndex = -1;
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
        /// プレイヤーを追加
        /// </summary>
        public void AddPlayer(BlackJackPlayer player)
        {
            Players.Add(player);
        }

        /// <summary>
        /// ゲームを開始（初期配牌）
        /// </summary>
        public void StartGame()
        {
            if (Players.Count == 0)
                throw new InvalidOperationException("プレイヤーが登録されていません");

            GameState = BlackJackGameState.PlayersTurn;
            CurrentPlayerIndex = 0;

            // 各プレイヤーに2枚配る
            foreach (var player in Players)
            {
                player.ResetCards();
                player.Cards.Add(DrawCard());
                player.Cards.Add(DrawCard());
            }

            // ディーラーに2枚配る
            Dealer.ResetCards();
            Dealer.Cards.Add(DrawCard());
            Dealer.Cards.Add(DrawCard());

            // 最初のプレイヤーがブラックジャックなら次へ
            CheckCurrentPlayerBlackjackOrBust();
        }

        /// <summary>
        /// 現在のプレイヤーがヒット
        /// </summary>
        public void Hit()
        {
            if (GameState != BlackJackGameState.PlayersTurn || CurrentPlayer == null)
                throw new InvalidOperationException("現在はヒットできません");

            CurrentPlayer.Cards.Add(DrawCard());

            // バストチェック
            if (CurrentPlayer.IsBust)
            {
                MoveToNextPlayer();
            }
        }

        /// <summary>
        /// 現在のプレイヤーがスタンド
        /// </summary>
        public void Stand()
        {
            if (GameState != BlackJackGameState.PlayersTurn)
                throw new InvalidOperationException("現在はスタンドできません");

            MoveToNextPlayer();
        }

        /// <summary>
        /// 次のプレイヤーに移動
        /// </summary>
        private void MoveToNextPlayer()
        {
            CurrentPlayerIndex++;

            if (CurrentPlayerIndex >= Players.Count)
            {
                // 全プレイヤー終了 → ディーラーのターン
                StartDealerTurn();
            }
            else
            {
                // 次のプレイヤーのブラックジャック・バストチェック
                CheckCurrentPlayerBlackjackOrBust();
            }
        }

        /// <summary>
        /// 現在のプレイヤーがブラックジャックまたはバストなら自動的に次へ
        /// </summary>
        private void CheckCurrentPlayerBlackjackOrBust()
        {
            if (CurrentPlayer == null)
                return;

            if (CurrentPlayer.IsBlackjack || CurrentPlayer.IsBust)
            {
                MoveToNextPlayer();
            }
        }

        /// <summary>
        /// ディーラーのターンを開始
        /// </summary>
        private void StartDealerTurn()
        {
            GameState = BlackJackGameState.DealerTurn;

            // 全プレイヤーがバストしていたらディーラーは引かない
            bool allPlayersBust = Players.All(p => p.IsBust);
            if (allPlayersBust)
            {
                EndGame();
                return;
            }

            // ディーラーは17以上になるまで引く
            while (_logicService.ShouldDealerHit(Dealer.Score))
            {
                Dealer.Cards.Add(DrawCard());
            }

            EndGame();
        }

        /// <summary>
        /// ゲームを終了し、勝敗を判定
        /// </summary>
        private void EndGame()
        {
            GameState = BlackJackGameState.GameOver;

            // 各プレイヤーの勝敗を判定
            foreach (var player in Players)
            {
                string result = _logicService.DetermineWinner(
                    player.Score,
                    Dealer.Score,
                    player.IsBust,
                    Dealer.IsBust
                );

                RecordResult(player, result);
            }
        }

        /// <summary>
        /// 結果を記録
        /// </summary>
        private void RecordResult(BlackJackPlayer player, string result)
        {
            if (result.Contains("あなたの勝ち") || result.Contains("プレイヤーの勝ち"))
            {
                player.RecordWin();
                Dealer.RecordLoss();
            }
            else if (result.Contains("ディーラーの勝ち") || result.Contains("バースト"))
            {
                player.RecordLoss();
                Dealer.RecordWin();
            }
            else
            {
                player.RecordDraw();
                Dealer.RecordDraw();
            }
        }

        /// <summary>
        /// 各プレイヤーの結果メッセージを取得
        /// </summary>
        public string GetResultMessage(BlackJackPlayer player)
        {
            return _logicService.DetermineWinner(
                player.Score,
                Dealer.Score,
                player.IsBust,
                Dealer.IsBust
            );
        }

        /// <summary>
        /// ゲームをリセット（カードを場から回収）
        /// </summary>
        public void ResetGame()
        {
            // 場にあるカードをクリア
            _cardsInPlay.Clear();

            // 全プレイヤーのカードをリセット
            foreach (var player in Players)
            {
                player.ResetCards();
            }
            Dealer.ResetCards();

            // 状態をリセット
            GameState = BlackJackGameState.Waiting;
            CurrentPlayerIndex = -1;
        }

        /// <summary>
        /// ゲームをリセット（カードを場から回収）- 旧バージョン互換性のため残す
        /// </summary>
        public void ReturnCards(BlackJackPlayer player, BlackJackDealer dealer)
        {
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
