using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Algorithms.Apps.TexasHoldem
{
    public class GameTest
    {
        public static void Start()
        {
            var game = new Game();
            game.Initialize(4);
            game.MinimumBet = 24;

            game.IsStarted = true;

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("[NEW ROUND {0}]", i.ToString("D5"));
                game.PlayOneRound();
                Console.WriteLine("[ROUND END {0}]", i.ToString("D5"));
            }

            var str = game.Report();
            Console.WriteLine(str);
        }

        public static void TestFindHand()
        {
            var sb = new StringBuilder();
            var loop = 1000;
            Hand hand;
            var decks = new List<Cards>();
            for (int i = 0; i < loop; i++)
            {
                var cards = Deck.NewRandom().DrawCards(7);
                decks.Add(cards);
            }

            for (int i = 0; i < loop; i++)
            {
                var cards = decks[i];
                hand = CardCombinationHelper.Find(cards);
                sb.AppendLine(hand.ToString());
            }

            Clipboard.SetText(sb.ToString());
            Console.ReadLine();
        }
    }
}