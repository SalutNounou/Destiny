using Destiny.Model.Evaluator;
using NUnit.Framework;

namespace Destiny.Model.Tests
{
    public class HandEvaluatorTests
    {
        [TestCase(true, 34, 46, 47, 30, 28, 33, 19,13,39)]
        [TestCase(false, 4, 21, 40, 9, 24, 49, 42, 10, 6)]
        [TestCase(false, 26, 16, 37, 1, 17, 32, 45, 35, 11)]
        [TestCase(true, 51, 12, 47, 8, 38, 27, 31, 29, 34)]
        [TestCase(true, 37, 36, 47, 8, 38, 27, 31, 29, 34)]
        public void Should_Evaluate_Hand_Correctly(bool expected, int player1Card1, int player1Card2, int flop1, int flop2, int flop3, int turn, int river,int player2Card1, int player2Card2)
        {
            var hand1 = new Hand(new Card(player1Card1), new Card(player1Card2),
                new Flop(new Card(flop1), new Card(flop2), new Card(flop3)), new Card(turn), new Card(river));
            var evaluator = new HandEvaluator();
            var handValue1 = evaluator.EvaluateHand(hand1.Mask,7);
          //  var description1 = evaluator.DescriptionFromHandValueInternal(handValue1);

            var hand2 = new Hand(new Card(player2Card1), new Card(player2Card2),
                new Flop(new Card(flop1), new Card(flop2), new Card(flop3)), new Card(turn), new Card(river));
            var handValue2 = evaluator.EvaluateHand(hand2.Mask, 7);
         //   var description2 = evaluator.DescriptionFromHandValueInternal(handValue2);


            Assert.AreEqual(expected, handValue1 > handValue2);
        }

    }
}