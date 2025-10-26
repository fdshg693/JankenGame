using JankenGame.Models.Janken;

namespace JankenGame.Services.Janken
{
    /// <summary>
    /// ジャンケンチャレンジモードのビジネスロジックを処理するサービスクラス
    /// </summary>
    public class JankenChallengeService
    {
        private readonly Random _random = new();

        /// <summary>
        /// ランダムにコンピューターの手を生成する
        /// </summary>
        /// <returns>コンピューターの手</returns>
        public JankenHand GenerateComputerHand()
        {
            return (JankenHand)_random.Next(0, 3);
        }

        /// <summary>
        /// プレイヤーの選択がコンピューターに勝つ手かどうかを判定する
        /// </summary>
        /// <param name="playerHand">プレイヤーが選んだ手</param>
        /// <param name="computerHand">コンピューターの手</param>
        /// <returns>プレイヤーが勝つ手を選んだ場合はtrue</returns>
        public bool IsCorrectAnswer(JankenHand playerHand, JankenHand computerHand)
        {
            // プレイヤーの手がコンピューターに勝つパターンをチェック
            return (playerHand == JankenHand.Rock && computerHand == JankenHand.Scissors) ||
                   (playerHand == JankenHand.Paper && computerHand == JankenHand.Rock) ||
                   (playerHand == JankenHand.Scissors && computerHand == JankenHand.Paper);
        }

        /// <summary>
        /// コンピューターの手に対して勝つ手を取得する（答え）
        /// </summary>
        /// <param name="computerHand">コンピューターの手</param>
        /// <returns>勝つ手</returns>
        public JankenHand GetWinningHand(JankenHand computerHand)
        {
            return computerHand switch
            {
                JankenHand.Rock => JankenHand.Paper,
                JankenHand.Paper => JankenHand.Scissors,
                JankenHand.Scissors => JankenHand.Rock,
                _ => throw new ArgumentException("無効な手です", nameof(computerHand))
            };
        }
    }
}
