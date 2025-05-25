namespace JankenGame.Models.Janken
{
    public static class JankenLogic
    {
        public static JankenResult DetermineWinner(JankenPlayer player, JankenPlayer computer)
        {
            if (player.Hand == null)
            {
                return JankenResult.Undecided; // プレイヤーの手が未決定の場合
            }
            if (computer.Hand == null)
            {
                return JankenResult.Undecided; // コンピューターの手が未決定の場合
            }
            if (player.Hand == computer.Hand)
            {
                return JankenResult.Draw;
            }
            else if (
                (player.Hand == JankenHand.Rock && computer.Hand == JankenHand.Scissors) ||
                (player.Hand == JankenHand.Paper && computer.Hand == JankenHand.Rock) ||
                (player.Hand == JankenHand.Scissors && computer.Hand == JankenHand.Paper)
            )
            {
                return JankenResult.Win;
            }
            else
            {
                return JankenResult.Lose;
            }
        }
    }
}
