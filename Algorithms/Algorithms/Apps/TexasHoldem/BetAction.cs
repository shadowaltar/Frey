namespace Algorithms.Apps.TexasHoldem
{
    public enum BetAction
    {
        Check, // don't raise and pass
        Raise, // raise the bet from 0 or previous player's bet
        Call,  // follow previous player's bet.
        Fold,  // stop playing for this round.
        SmallBlind, // blindly bet, for small blind player
        BigBlind, // bindly bet, for big blind player
    }
}