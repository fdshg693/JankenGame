namespace JankenGame.Models.Janken
{
    /// <summary>
    /// 複数プレイヤーのジャンケンゲームの結果を表すクラス
    /// </summary>
    public class JankenGameResult
    {
        /// <summary>
        /// 勝者が存在するかどうか（全員引き分けの場合はfalse）
        /// </summary>
        public bool ExistsWinner { get; }

        /// <summary>
        /// 勝った手（引き分けの場合はnull）
        /// </summary>
        public JankenHand? WinningHand { get; }

        /// <summary>
        /// JankenGameResultのコンストラクタ
        /// </summary>
        /// <param name="existsWinner">勝者が存在するか</param>
        /// <param name="winningHand">勝った手（引き分けの場合はnull）</param>
        public JankenGameResult(bool existsWinner, JankenHand? winningHand)
        {
            ExistsWinner = existsWinner;
            WinningHand = winningHand;
        }

        /// <summary>
        /// 引き分けの結果を作成します
        /// </summary>
        /// <returns>引き分けを表すJankenGameResult</returns>
        public static JankenGameResult CreateDraw()
        {
            return new JankenGameResult(false, null);
        }

        /// <summary>
        /// 勝者ありの結果を作成します
        /// </summary>
        /// <param name="winningHand">勝った手</param>
        /// <returns>勝者ありを表すJankenGameResult</returns>
        public static JankenGameResult CreateWinner(JankenHand winningHand)
        {
            return new JankenGameResult(true, winningHand);
        }
    }
}
