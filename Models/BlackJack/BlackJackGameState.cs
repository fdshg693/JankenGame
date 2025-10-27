namespace JankenGame.Models.BlackJack
{
    /// <summary>
    /// ブラックジャックのゲーム進行状態
    /// </summary>
    public enum BlackJackGameState
    {
        /// <summary>
        /// ゲーム開始前
        /// </summary>
        Waiting,

        /// <summary>
        /// プレイヤーターン
        /// </summary>
        PlayersTurn,

        /// <summary>
        /// ディーラーターン
        /// </summary>
        DealerTurn,

        /// <summary>
        /// ゲーム終了
        /// </summary>
        GameOver
    }
}
