using JankenGame.Models.Janken;

namespace JankenGame.Services.Janken
{
    /// <summary>
    /// ジャンケンゲームのビジネスロジックを処理するサービスクラス
    /// </summary>
    public class JankenGameService
    {
        private readonly JankenLogicService _logicService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        public JankenGameService()
        {
            _logicService = new JankenLogicService();
        }

        /// <summary>
        /// プレイヤーとコンピューターの手から勝敗を判定します
        /// </summary>
        /// <param name="playerHand">プレイヤーの手</param>
        /// <param name="computerHand">コンピューターの手</param>
        /// <returns>ゲーム結果</returns>
        public JankenResult DetermineWinner(JankenHand playerHand, JankenHand computerHand)
        {
            return _logicService.DetermineResult(playerHand, computerHand);
        }
    }
}
