namespace JankenGame.Models.Janken
{
    public static class JankenLogic
    {
        public static List<JankenPlayer> DetermineWinner(JankenPlayerList players)
        {
            var totalHands = players.Select(p => p.Hand).Distinct().ToList();
            if (totalHands.Count == 3)
            {
                // グー、チョキ、パーが全て出ている場合
                return new List<JankenPlayer> { }; // 引き分け
            }
            else if (totalHands.Count == 2)
            {
                // 2種類の手が出ている場合
                if (totalHands.Contains(JankenHand.Rock) && totalHands.Contains(JankenHand.Scissors))
                {
                    return players
                        .Where(p => p.Hand == JankenHand.Rock)
                        .ToList(); // グーが勝ち
                }
                else if (totalHands.Contains(JankenHand.Scissors) && totalHands.Contains(JankenHand.Paper))
                {
                    return players
                        .Where(p => p.Hand == JankenHand.Scissors)
                        .ToList(); // チョキが勝ち
                }
                else if (totalHands.Contains(JankenHand.Paper) && totalHands.Contains(JankenHand.Rock))
                {
                    return players
                        .Where(p => p.Hand == JankenHand.Paper)
                        .ToList(); // パーが勝ち
                }
                else
                {
                    // ここに到達することはないはずですが、念のための例外処理
                    throw new InvalidOperationException("Unexpected hand combination: " + string.Join(", ", totalHands));

                }
            }
            else if (totalHands.Count == 1)
            {
                // 全員同じ手を出している場合
                return new List<JankenPlayer> { }; // 引き分け
            }
            else
            {
                // ここに到達することはないはずですが、念のための例外処理
                throw new InvalidOperationException("Unexpected number of distinct hands: " + totalHands.Count);
            }
        }
    }
}
