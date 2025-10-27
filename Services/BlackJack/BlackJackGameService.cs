using JankenGame.Models.BlackJack;

namespace JankenGame.Services.BlackJack
{
    /// <summary>
    /// ブラックジャックのゲーム進行を統合管理するサービス
    /// DeckManager、StateManager、LogicServiceを利用してゲーム全体を制御
    /// </summary>
    public class BlackJackGameService
    {
        private readonly BlackJackDeckManager _deckManager;
        private readonly BlackJackGameStateManager _stateManager;
        private readonly BlackJackLogicService _logicService;

        // ゲーム状態への公開プロパティ（StateManagerへの委譲）
        public BlackJackGameState GameState => _stateManager.GameState;
        public List<BlackJackPlayer> Players => _stateManager.Players;
        public BlackJackDealer Dealer => _stateManager.Dealer;
        public int CurrentPlayerIndex => _stateManager.CurrentPlayerIndex;
        public BlackJackPlayer? CurrentPlayer => _stateManager.CurrentPlayer;

        public BlackJackGameService()
        {
            _deckManager = new BlackJackDeckManager();
            _stateManager = new BlackJackGameStateManager();
            _logicService = new BlackJackLogicService();
        }


        /// <summary>
        /// プレイヤーを追加
        /// </summary>
        public void AddPlayer(BlackJackPlayer player)
        {
            _stateManager.AddPlayer(player);
        }

        /// <summary>
        /// ゲームを開始（初期配牌）
        /// </summary>
        public void StartGame()
        {
            if (Players.Count == 0)
                throw new InvalidOperationException("プレイヤーが登録されていません");

            _stateManager.StartPlayersTurn();

            // 各プレイヤーに2枚配る
            foreach (var player in Players)
            {
                player.ResetCards();
                player.Cards.Add(_deckManager.DrawCard());
                player.Cards.Add(_deckManager.DrawCard());
            }

            // ディーラーに2枚配る
            Dealer.ResetCards();
            Dealer.Cards.Add(_deckManager.DrawCard());
            Dealer.Cards.Add(_deckManager.DrawCard());

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

            CurrentPlayer.Cards.Add(_deckManager.DrawCard());

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
            bool allPlayersFinished = _stateManager.MoveToNextPlayer();

            if (allPlayersFinished)
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
            _stateManager.StartDealerTurn();

            // 全プレイヤーがバストしていたらディーラーは引かない
            if (_stateManager.AreAllPlayersBust())
            {
                EndGame();
                return;
            }

            // ディーラーは17以上になるまで引く
            while (_logicService.ShouldDealerHit(Dealer.Score))
            {
                Dealer.Cards.Add(_deckManager.DrawCard());
            }

            EndGame();
        }

        /// <summary>
        /// ゲームを終了し、勝敗を判定
        /// </summary>
        private void EndGame()
        {
            _stateManager.EndGame();

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
            // デッキマネージャーでカードをクリア
            _deckManager.ClearCardsInPlay();

            // 状態マネージャーでゲームをリセット
            _stateManager.ResetGame();
        }

        /// <summary>
        /// ゲームをリセット（カードを場から回収）- 旧バージョン互換性のため残す
        /// </summary>
        public void ReturnCards(BlackJackPlayer player, BlackJackDealer dealer)
        {
            _deckManager.ClearCardsInPlay();
        }

        /// <summary>
        /// ゲームをリセット（カードを場から回収）- BlackJackHand版（互換性のため残す）
        /// </summary>
        public void ReturnCards(BlackJackHand player, BlackJackHand dealer)
        {
            _deckManager.ClearCardsInPlay();
        }
    }
}
