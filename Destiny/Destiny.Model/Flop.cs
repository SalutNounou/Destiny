namespace Destiny.Model
{
    public class Flop
    {
        public Flop(Card first, Card second, Card third)
        {
            FirstCard = first;
            SecondCard = second;
            ThirdCard = third;
        }
        public Card FirstCard { get; }
        public Card SecondCard { get; }
        public Card ThirdCard { get; }

        public ulong Mask { get { return FirstCard.Mask | SecondCard.Mask | ThirdCard.Mask; } }

        public bool IsValid
        {
            get
            {
                return FirstCard.Mask != SecondCard.Mask && SecondCard.Mask != ThirdCard.Mask &&
                       FirstCard.Mask != ThirdCard.Mask;
            }
        }

    }
}