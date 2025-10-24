using JankenGame.Models.Janken;

namespace JankenGame.Services.Janken
{
    /// <summary>
    /// ジャンケンゲームのビジネスロジックを処理するサービスクラス
    /// </summary>
    public class JankenGameService
    {
        /// <summary>
        /// プレイヤーとコンピューターの手から勝敗を判定します
        /// </summary>
        /// <param name="playerHand">プレイヤーの手</param>
        /// <param name="computerHand">コンピューターの手</param>
        /// <returns>ゲーム結果</returns>
        public JankenResult DetermineWinner(JankenHand playerHand, JankenHand computerHand)
        {
            if (playerHand == computerHand)
            {
                return JankenResult.Draw;
            }

            // プレイヤーが勝つパターン
            if ((playerHand == JankenHand.Rock && computerHand == JankenHand.Scissors) ||
                (playerHand == JankenHand.Paper && computerHand == JankenHand.Rock) ||
                (playerHand == JankenHand.Scissors && computerHand == JankenHand.Paper))
            {
                return JankenResult.Win;
            }

            return JankenResult.Lose;
        }
    }
}
