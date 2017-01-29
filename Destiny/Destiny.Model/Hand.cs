namespace Destiny.Model
{
    public class Hand
    {
        public Hand(Card firstSecret, Card secondSecret, Flop flop, Card turn, Card river)
        {
            FirstSecretCard = firstSecret;
            SecondSecretCard = secondSecret;
            Flop = flop;
            Turn = turn;
            River = river;
        }
        public Card FirstSecretCard { get; }
        public Card SecondSecretCard { get; }
        public Flop Flop { get; }
        public Card Turn { get; }
        public Card River { get; }

        public ulong Mask
        {
            get
            {
                return FirstSecretCard.Mask | SecondSecretCard.Mask | Flop.Mask | Turn.Mask | River.Mask;
            }
        }


        public bool IsValid
        {
            get
            {
                return Flop.IsValid
                       && (Flop.Mask | FirstSecretCard.Mask) != Flop.Mask
                       && (Flop.Mask | SecondSecretCard.Mask) != Flop.Mask
                       && (Flop.Mask | Turn.Mask) != Flop.Mask
                       && (Flop.Mask | River.Mask) != Flop.Mask
                       && FirstSecretCard.Mask != SecondSecretCard.Mask
                       && FirstSecretCard.Mask != Turn.Mask
                       && FirstSecretCard.Mask != River.Mask
                       && SecondSecretCard.Mask != Turn.Mask
                       && FirstSecretCard.Mask != River.Mask
                       && Turn.Mask != River.Mask;
            }
        }

    }
}