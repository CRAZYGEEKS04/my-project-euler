using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// The prime 41, can be written as the sum of six consecutive primes:
    ///
    /// 41 = 2 + 3 + 5 + 7 + 11 + 13
    /// This is the longest sum of consecutive primes that adds to a prime below
    /// one-hundred.
    ///
    /// The longest sum of consecutive primes below one-thousand that adds to a prime,
    /// contains 21 terms, and is equal to 953.
    ///
    /// Which prime, below one-million, can be written as the sum of the most
    /// consecutive primes?
    /// </summary>
    internal sealed class Problem50 : Problem
    {
        private const int upper = 1000000;

        public Problem50() : base(50) { }

        protected override string Action()
        {
            var p = new Prime(upper);
            int maxlength = 0;
            int sum = 0;

            p.GenerateAll();
            foreach (var n in p)
            {
                sum += n;
                if (sum > upper)
                    break;
                maxlength++;
            }

            for (int l = maxlength; l > 0; l--)
            {
                for (int i = 0; i <= p.Nums.Count - l; i++)
                {
                    sum = p.Nums.Skip(i).Take(l).Sum();
                    if (sum > upper)
                        break;
                    if (p.Contains(sum))
                        return sum.ToString();
                }
            }

            return null;
        }
    }

    /// <summary>
    /// By replacing the 1st digit of *3, it turns out that six of the nine possible
    /// values: 13, 23, 43, 53, 73, and 83, are all prime.
    ///
    /// By replacing the 3rd and 4th digits of 56**3 with the same digit, this 5-digit
    /// number is the first example having seven primes among the ten generated
    /// numbers, yielding the family: 56003, 56113, 56333, 56443, 56663, 56773, and
    /// 56993. Consequently 56003, being the first member of this family, is the
    /// smallest prime with this property.
    ///
    /// Find the smallest prime which, by replacing part of the number (not necessarily
    /// adjacent digits) with the same digit, is part of an eight prime value family.
    /// </summary>
    internal sealed class Problem51 : Problem
    {
        private const int upper = 1000000;

        public Problem51() : base(51) { }

        private List<string> GetPattern(string number, List<int> idx, int l)
        {
            var ret = new List<string>();

            foreach (var p in Itertools.Combinations(idx, l))
            {
                var pattern = "";

                for (int i = 0; i < number.Length; i++)
                {
                    if (p.Contains(i))
                        pattern += "*";
                    else
                        pattern += number[i];
                }

                ret.Add(pattern);
            }

            return ret;
        }

        private List<string> GetIDX(string number)
        {
            var ret = new List<string>();

            for (var c = '0'; c <= '9'; c++)
            {
                var idx = new List<int>();
                var id = -1;

                while ((id = number.IndexOf(c, id + 1)) != -1)
                    idx.Add(id);

                for (int length = 1; length <= idx.Count; length++)
                    ret.AddRange(GetPattern(number, idx, length));
            }

            return ret;
        }

        protected override string Action()
        {
            var p = new Prime(upper);
            var r = new Dictionary<string, int[]>();

            p.GenerateAll();
            foreach (var n in p)
            {
                foreach (var id in GetIDX(n.ToString()))
                {
                    if (r.ContainsKey(id))
                    {
                        r[id][1]++;
                        if (r[id][1] == 8)
                            return r[id][0].ToString();
                    }
                    else
                    {
                        r.Add(id, new int[] { n, 1 });
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// It can be seen that the number, 125874, and its double, 251748, contain exactly
    /// the same digits, but in a different order.
    ///
    /// Find the smallest positive integer, x, such that 2x, 3x, 4x, 5x, and 6x,
    /// contain the same digits.
    /// </summary>
    internal sealed class Problem52 : Problem
    {
        public Problem52() : base(52) { }

        private bool Check(int number)
        {
            var digits = (from c in number.ToString()
                          select c - '0').ToList();

            digits.Sort();

            for (int i = 2; i <= 6; i++)
            {
                var tmp = (from c in (number * i).ToString()
                           select c - '0').ToList();
                var flags = true;
                tmp.Sort();

                for (int id = 0; id < tmp.Count; id++)
                    if (tmp[id] != digits[id])
                    {
                        flags = false;
                        break;
                    }
                if (!flags)
                    return false;
            }

            return true;
        }

        protected override string Action()
        {
            for (int i = 123456; i <= 987654 / 6; i++)
            {
                if (Check(i))
                    return i.ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// There are exactly ten ways of selecting three from five, 12345:
    ///
    /// 123, 124, 125, 134, 135, 145, 234, 235, 245, and 345
    ///
    /// In combinatorics, we use the notation, 5C3 = 10.
    ///
    /// In general,
    ///
    /// nCr = n!/r!(nr)! , where r <= n, n != n*(n1)*...*3*2*1, and 0! = 1.
    ///
    /// It is not until n = 23, that a value exceeds one-million: 23C10 = 1144066.
    ///
    /// How many, not necessarily distinct, values of nCr, for 1 <= n <= 100, are
    /// greater than one-million?
    /// </summary>
    internal sealed class Problem53 : Problem
    {
        private const int upper = 100;

        public Problem53() : base(53) { }

        protected override string Action()
        {
            var ret = 0;

            for (int n = 1; n <= upper; n++)
            {
                for (int c = 1; c < n / 2 + 2; c++)
                {
                    if (Probability.CountCombinations(new BigInteger(n), new BigInteger(c)) > 1000000)
                    {
                        ret += n - 2 * c + 1;
                        break;
                    }
                }
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// In the card game poker, a hand consists of five cards and are ranked, from
    /// lowest to highest, in the following way:
    ///
    /// High Card: Highest value card.
    /// One Pair: Two cards of the same value.
    /// Two Pairs: Two different pairs.
    /// Three of a Kind: Three cards of the same value.
    /// Straight: All cards are consecutive values.
    /// Flush: All cards of the same suit.
    /// Full House: Three of a kind and a pair.
    /// Four of a Kind: Four cards of the same value.
    /// Straight Flush: All cards are consecutive values of same suit.
    /// Royal Flush: Ten, Jack, Queen, King, Ace, in same suit.
    /// The cards are valued in the order:
    /// 2, 3, 4, 5, 6, 7, 8, 9, 10, Jack, Queen, King, Ace.
    ///
    /// If two players have the same ranked hands then the rank made up of the highest
    /// value wins; for example, a pair of eights beats a pair of fives (see example 1
    /// below). But if two ranks tie, for example, both players have a pair of queens,
    /// then highest cards in each hand are compared (see example 4 below); if the
    /// highest cards tie then the next highest cards are compared, and so on.
    ///
    /// Consider the following five hands dealt to two players:
    ///
    /// Hand
    /// Player 1
    /// Player 2
    /// Winner
    /// 1
    /// 5H 5C 6S 7S KD      Pair of Fives
    /// 2C 3S 8S 8D TD      Pair of Eights
    /// Player 2
    ///
    /// 2
    /// 5D 8C 9S JS AC      Highest card Ace
    /// 2C 5C 7D 8S QH      Highest card Queen
    /// Player 1
    ///
    /// 3
    /// 2D 9C AS AH AC      Three Aces
    /// 3D 6D 7D TD QD      Flush with Diamonds
    /// Player 2
    ///
    /// 4
    /// 4D 6S 9H QH QC      Pair of Queens  Highest card Nine
    /// 3D 6D 7H QD QS      Pair of Queens  Highest card Seven
    /// Player 1
    ///
    /// 5
    /// 2H 2D 4C 4D 4S      Full House With Three Fours
    /// 3C 3D 3S 9S 9D      Full House With Three Threes
    /// Player 1
    ///
    /// [file D0054.txt], contains one-thousand random hands dealt to two
    /// players. Each line of the file contains ten cards (separated by a single
    /// space): the first five are Player 1's cards and the last five are Player 2's
    /// cards. You can assume that all hands are valid (no invalid characters or
    /// repeated cards), each player's hand is in no specific order, and in each hand
    /// there is a clear winner.
    ///
    /// How many hands does Player 1 win?
    /// </summary>
    internal sealed class Problem54 : Problem
    {
        public Problem54() : base(54) { }

        private List<string[]> hands;

        protected override void PreAction(string data)
        {
            hands = (from line in data.Split('\n')
                     select line.Trim().Split(' ').ToArray()).ToList();
        }

        private static Dictionary<char, int> valueDict = new Dictionary<char, int>()
        {
            {'2', 1}, {'3', 2}, {'4', 3}, {'5', 4}, {'6', 5}, {'7', 6}, {'8', 7},
            {'9', 8}, {'T', 9}, {'J', 10}, {'Q', 11}, {'K', 12}, {'A', 13},
        };

        private Tuple<List<KeyValuePair<int, int>>, bool> GetCardNumbers(IEnumerable<string> hand)
        {
            var nums = new Dictionary<int, int>();
            var colors = new HashSet<char>();

            foreach (var card in hand)
            {
                if (nums.ContainsKey(valueDict[card[0]]))
                    nums[valueDict[card[0]]]++;
                else
                    nums.Add(valueDict[card[0]], 1);

                colors.Add(card[1]);
            }

            var ret = nums.ToList();
            ret.Sort((x, y) =>
            {
                var tmp = x.Value.CompareTo(y.Value);
                if (tmp != 0)
                    return -tmp;
                else
                    return y.Key.CompareTo(x.Key);
            });

            return new Tuple<List<KeyValuePair<int, int>>, bool>(ret, colors.Count == 1);
        }

        private Tuple<int, int> GetScore(List<KeyValuePair<int, int>> nums, bool isFlush)
        {
            // Royal Flush/Straight Flush
            if (isFlush && nums.Count == 5 && nums[0].Key == nums[4].Key + 4)
                return new Tuple<int, int>(10, nums[0].Key);

            // Four of a Kind
            foreach (var num in nums)
                if (num.Value == 4)
                    return new Tuple<int, int>(9, num.Key);

            // Full House
            if (nums[0].Value == 3 && nums[1].Value == 2)
                return new Tuple<int, int>(8, nums[0].Key);
            if (nums[0].Value == 2 && nums[1].Value == 3)
                return new Tuple<int, int>(8, nums[1].Key);

            // Flush
            if (isFlush)
                return new Tuple<int, int>(7, nums[0].Key);

            // Straight
            if (nums.Count == 5 && nums[0].Key == nums[4].Key + 4)
                return new Tuple<int, int>(6, nums[0].Key);

            // Three of a Kind
            foreach (var num in nums)
                if (num.Value == 3)
                    return new Tuple<int, int>(5, num.Key);

            // Two Pairs
            if (nums.Count == 3)
                if (nums[0].Value == 2)
                    return new Tuple<int, int>(4, nums[0].Key);
                else
                    return new Tuple<int, int>(4, nums[1].Key);

            // One Pair
            foreach (var num in nums)
                if (num.Value == 2)
                    return new Tuple<int, int>(3, num.Key);

            // High Card
            return new Tuple<int, int>(2, nums[0].Key);
        }

        protected override string Action()
        {
            var ret = 0;

            foreach (var hand in hands)
            {
                var tmp = GetCardNumbers(hand.Take(5));
                var score1 = GetScore(tmp.Item1, tmp.Item2);

                tmp = GetCardNumbers(hand.Skip(5));
                var score2 = GetScore(tmp.Item1, tmp.Item2);

                if (score1.Item1 > score2.Item1 || (score1.Item1 == score2.Item1 && score1.Item2 > score2.Item2))
                    ret++;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// If we take 47, reverse and add, 47 + 74 = 121, which is palindromic.
    ///
    /// Not all numbers produce palindromes so quickly. For example,
    ///
    /// 349 + 943 = 1292,
    /// 1292 + 2921 = 4213
    /// 4213 + 3124 = 7337
    ///
    /// That is, 349 took three iterations to arrive at a palindrome.
    ///
    /// Although no one has proved it yet, it is thought that some numbers, like 196,
    /// never produce a palindrome. A number that never forms a palindrome through the
    /// reverse and add process is called a Lychrel number. Due to the theoretical
    /// nature of these numbers, and for the purpose of this problem, we shall assume
    /// that a number is Lychrel until proven otherwise. In addition you are given that
    /// for every number below ten-thousand, it will either (i) become a palindrome in
    /// less than fifty iterations, or, (ii) no one, with all the computing power that
    /// exists, has managed so far to map it to a palindrome. In fact, 10677 is the
    /// first number to be shown to require over fifty iterations before producing a
    /// palindrome: 4668731596684224866951378664 (53 iterations, 28-digits).
    ///
    /// Surprisingly, there are palindromic numbers that are themselves Lychrel
    /// numbers; the first example is 4994.
    ///
    /// How many Lychrel numbers are there below ten-thousand?
    ///
    /// NOTE: Wording was modified slightly on 24 April 2007 to emphasise the
    /// theoretical nature of Lychrel numbers.
    /// </summary>
    internal sealed class Problem55 : Problem
    {
        private const int upper = 10000;

        public Problem55() : base(55) { }

        protected override string Action()
        {
            var counter = upper - 1;

            for (int i = 1; i < upper; i++)
            {
                var tmp = new BigInteger(i);
                var tmps = tmp.ToString();

                for (int t = 0; t < 50; t++)
                {
                    var rev = string.Join("", tmps.Reverse());

                    tmp += BigInteger.Parse(rev);
                    tmps = tmp.ToString();
                    if (Misc.IsPalindromic(tmps))
                    {
                        counter--;
                        break;
                    }
                }
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// A googol (10^100) is a massive number: one followed by one-hundred zeros;
    /// 100^100 is almost unimaginably large: one followed by two-hundred zeros. Despite
    /// their size, the sum of the digits in each number is only 1.
    ///
    /// Considering natural numbers of the form, a^b, where a, b < 100, what is the
    /// maximum digital sum?
    /// </summary>
    internal sealed class Problem56 : Problem
    {
        public Problem56() : base(56) { }

        protected override string Action()
        {
            int max = 0;

            foreach (var n in Itertools.Permutations(Itertools.Range(1, 99), 2))
            {
                var number = BigInteger.Pow(n[0], n[1]);
                var tmp = 0;

                foreach (var c in number.ToString())
                    tmp += c - '0';

                if (tmp > max)
                    max = tmp;
            }

            return max.ToString();
        }
    }

    /// <summary>
    /// It is possible to show that the square root of two can be expressed as an
    /// infinite continued fraction.
    ///
    /// sqrt(2) = 1 + 1/(2 + 1/(2 + 1/(2 + ... ))) = 1.414213...
    ///
    /// By expanding this for the first four iterations, we get:
    ///
    /// 1 + 1/2 = 3/2 = 1.5
    /// 1 + 1/(2 + 1/2) = 7/5 = 1.4
    /// 1 + 1/(2 + 1/(2 + 1/2)) = 17/12 = 1.41666...
    /// 1 + 1/(2 + 1/(2 + 1/(2 + 1/2))) = 41/29 = 1.41379...
    ///
    /// The next three expansions are 99/70, 239/169, and 577/408, but the eighth
    /// expansion, 1393/985, is the first example where the number of digits in the
    /// numerator exceeds the number of digits in the denominator.
    ///
    /// In the first one-thousand expansions, how many fractions contain a numerator
    /// with more digits than denominator?
    /// </summary>
    internal sealed class Problem57 : Problem
    {
        private const int upper = 1000;

        public Problem57() : base(57) { }

        protected override string Action()
        {
            var tmp = new Fraction(3, 2);
            var counter = 0;

            for (int i = 1; i < upper; i++)
            {
                tmp = 1 + 1 / (1 + tmp);
                if (tmp.Numerator.ToString().Length > tmp.Denominator.ToString().Length)
                    counter++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Starting with 1 and spiralling anticlockwise in the following way, a square
    /// spiral with side length 7 is formed.
    ///
    /// 37 36 35 34 33 32 31
    /// 38 17 16 15 14 13 30
    /// 39 18  5  4  3 12 29
    /// 40 19  6  1  2 11 28
    /// 41 20  7  8  9 10 27
    /// 42 21 22 23 24 25 26
    /// 43 44 45 46 47 48 49
    ///
    /// It is interesting to note that the odd squares lie along the bottom right
    /// diagonal, but what is more interesting is that 8 out of the 13 numbers lying
    /// along both diagonals are prime; that is, a ratio of 8/13  62%.
    ///
    /// If one complete new layer is wrapped around the spiral above, a square spiral
    /// with side length 9 will be formed. If this process is continued, what is the
    /// side length of the square spiral for which the ratio of primes along both
    /// diagonals first falls below 10%?
    /// </summary>
    internal sealed class Problem58 : Problem
    {
        private const int upper = 1000000;

        public Problem58() : base(58) { }

        protected override string Action()
        {
            var p = new Prime(upper);
            int nprime = 3, len = 3, current = 9;

            p.GenerateAll();
            while (len < upper)
            {
                len += 2;
                for (int i = 0; i < 4; i++)
                {
                    current += len - 1;
                    if (p.IsPrime(current))
                        nprime++;
                }
                if (10 * nprime < (len - 1) * 2 + 1)
                    return len.ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// Each character on a computer is assigned a unique code and the preferred
    /// standard is ASCII (American Standard Code for Information Interchange). For
    /// example, uppercase A = 65, asterisk (*) = 42, and lowercase k = 107.
    ///
    /// A modern encryption method is to take a text file, convert the bytes to ASCII,
    /// then XOR each byte with a given value, taken from a secret key. The advantage
    /// with the XOR function is that using the same encryption key on the cipher text,
    /// restores the plain text; for example, 65 XOR 42 = 107, then 107 XOR 42 = 65.
    ///
    /// For unbreakable encryption, the key is the same length as the plain text
    /// message, and the key is made up of random bytes. The user would keep the
    /// encrypted message and the encryption key in different locations, and without
    /// both "halves", it is impossible to decrypt the message.
    ///
    /// Unfortunately, this method is impractical for most users, so the modified
    /// method is to use a password as a key. If the password is shorter than the
    /// message, which is likely, the key is repeated cyclically throughout the
    /// message. The balance for this method is using a sufficiently long password key
    /// for security, but short enough to be memorable.
    ///
    /// Your task has been made easy, as the encryption key consists of three lower
    /// case characters. Using [file D0059.txt], a file containing the encrypted ASCII
    /// codes, and the knowledge that the plain text must contain common English words,
    /// decrypt the message and find the sum of the ASCII values in the original text.
    /// </summary>
    internal sealed class Problem59 : Problem
    {
        public Problem59() : base(59) { }

        private List<byte> codes;

        protected override void PreAction(string data)
        {
            codes = (from d in data.Split(',')
                     select Convert.ToByte(int.Parse(d))).ToList();
        }

        protected override string Action()
        {
            var tmp = new byte[codes.Count];

            foreach (var key in Itertools.PermutationsWithReplacement(Itertools.Range('a', 'z'), 3))
            {
                for (int i = 0; i < tmp.Length; i++)
                    tmp[i] = Convert.ToByte(codes[i] ^ key[i % 3]);

                var text = Encoding.ASCII.GetString(tmp);

                if (text.Contains("the") && text.Contains("with"))
                {
                    var sum = 0;
                    foreach (var t in tmp)
                        sum += t;
                    return sum.ToString();
                }
            }

            return null;
        }
    }
}