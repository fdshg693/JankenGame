using JankenGame.Models.BlackJack;

namespace JankenGame.Services.BlackJack
{
    /// <summary>
    /// ブラックジャックのゲーム進行を統合管理するサービス
    /// DeckManager、StateManager、LogicService、BettingServiceを利用してゲーム全体を制御
    /// </summary>
    public class BlackJackGameService
    {
        private readonly BlackJackDeckManager _deckManager;
        private readonly BlackJackGameStateManager _stateManager;
        private readonly BlackJackLogicService _logicService;
        private readonly BlackJackBettingService _bettingService;

        // ゲーム状態への公開プロパティ（StateManagerへの委譲）
        public BlackJackGameState GameState => _stateManager.GameState;
        public List<BlackJackPlayer> Players => _stateManager.Players;
        public BlackJackDealer Dealer => _stateManager.Dealer;
        public int CurrentPlayerIndex => _stateManager.CurrentPlayerIndex;
        public BlackJackPlayer? CurrentPlayer => _stateManager.CurrentPlayer;

        // ベッティング関連の公開プロパティ
        public BlackJackBettingService BettingService => _bettingService;
        public int CurrentBet => _bettingService.CurrentBet;
        public int Pot => _bettingService.Pot;

        public BlackJackGameService()
        {
            _deckManager = new BlackJackDeckManager();
            _stateManager = new BlackJackGameStateManager();
            _logicService = new BlackJackLogicService();
            _bettingService = new BlackJackBettingService();
        }


        /// <summary>
        /// プレイヤーを追加
        /// </summary>
        public void AddPlayer(BlackJackPlayer player)
        {
            _stateManager.AddPlayer(player);
        }

        /// <summary>
        /// ゲームを開始（アンティ徴収と初期配牌）
        /// </summary>
        public void StartGame()
        {
            if (Players.Count == 0)
                throw new InvalidOperationException("プレイヤーが登録されていません");

            // ベッティングラウンドを初期化
            _bettingService.InitializeBettingRound(Players);

            // アンティを徴収
            _stateManager.StartAnte();
            if (!_bettingService.StartAnte())
            {
                throw new InvalidOperationException("全プレイヤーのチップが不足しています");
            }

            // 初期配牌
            _stateManager.StartInitialDeal();

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

            // ベッティングラウンドへ
            _stateManager.StartBettingRound();
        }


        /// <summary>
        /// プレイヤーのベット処理
        /// </summary>
        public bool PlaceBet(int amount, BettingAction action)
        {
            if (GameState != BlackJackGameState.BettingRound || CurrentPlayer == null)
                return false;

            bool success = _bettingService.PlaceBet(CurrentPlayer, amount, action);
            
            if (success)
            {
                // ベッティングラウンドが完了したかチェック
                if (_bettingService.IsBettingRoundComplete())
                {
                    // プレイヤーターンへ移行
                    _stateManager.StartPlayersTurn();
                    CheckCurrentPlayerBlackjackOrBust();
                }
                else
                {
                    // 次のプレイヤーへ
                    _bettingService.MoveToNextPlayer();
                }
            }

            return success;
        }

        /// <summary>
        /// 現在のプレイヤーが行動可能か
        /// </summary>
        public bool CanCurrentPlayerAct()
        {
            if (CurrentPlayer == null)
                return false;

            if (GameState == BlackJackGameState.BettingRound)
            {
                var state = _bettingService.GetPlayerState(CurrentPlayer);
                return state != null && !state.HasFolded && !state.IsAllIn;
            }

            return GameState == BlackJackGameState.PlayersTurn;
        }

        /// <summary>
        /// 最小ベット額（コール額）を取得
        /// </summary>
        public int GetMinimumBet()
        {
            if (CurrentPlayer == null)
                return 0;

            return _bettingService.GetMinimumBet(CurrentPlayer);
        }

        /// <summary>
        /// 精算処理を実行
        /// </summary>
        public void ProcessShowdown()
        {
            _stateManager.StartShowdown();

            var activePlayers = _bettingService.GetActivePlayers()
                .Select(s => s.Player)
                .ToList();

            if (activePlayers.Count == 0)
            {
                // 全員フォールド（通常あり得ない）
                EndGame();
                return;
            }

            if (activePlayers.Count == 1)
            {
                // 1人だけ残った場合は自動的に勝利
                var winner = activePlayers[0];
                winner.RecordWin();
                Dealer.RecordDraw();
                _bettingService.DistributePot(new List<BlackJackPlayer> { winner });
                EndGame();
                return;
            }

            // 通常の勝敗判定
            var winners = DetermineWinners(activePlayers);
            _bettingService.DistributePot(winners);
            EndGame();
        }

        /// <summary>
        /// 勝者を判定
        /// </summary>
        private List<BlackJackPlayer> DetermineWinners(List<BlackJackPlayer> activePlayers)
        {
            var winners = new List<BlackJackPlayer>();

            foreach (var player in activePlayers)
            {
                string result = _logicService.DetermineWinner(
                    player.Score,
                    Dealer.Score,
                    player.IsBust,
                    Dealer.IsBust
                );

                if (result.Contains("あなたの勝ち") || result.Contains("プレイヤーの勝ち"))
                {
                    winners.Add(player);
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
                    // 引き分けの場合もベットを返す（勝者リストに追加）
                    winners.Add(player);
                    player.RecordDraw();
                    Dealer.RecordDraw();
                }
            }

            return winners;
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

            // フォールドしていないプレイヤーを確認
            var activePlayers = _bettingService.GetActivePlayers();
            
            // 全プレイヤーがフォールドまたはバストしていたらディーラーは引かない
            if (activePlayers.Count == 0 || _stateManager.AreAllPlayersBust())
            {
                ProcessShowdown();
                return;
            }

            // ディーラーは17以上になるまで引く
            while (_logicService.ShouldDealerHit(Dealer.Score))
            {
                Dealer.Cards.Add(_deckManager.DrawCard());
            }

            ProcessShowdown();
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

            // ベッティングサービスをリセット
            _bettingService.ResetBettingRound();
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
