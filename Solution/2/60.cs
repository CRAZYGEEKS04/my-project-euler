using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// A game is played with three piles of stones and two players.
    /// At her turn, a player removes one or more stones from the piles. However, if
    /// she takes stones from more than one pile, she must remove the same number of
    /// stones from each of the selected piles.
    ///
    /// In other words, the player chooses some N>0 and removes:
    ///
    /// N stones from any single pile; or
    /// N stones from each of any two piles (2N total); or
    /// N stones from each of the three piles (3N total).
    /// The player taking the last stone(s) wins the game.
    /// A winning configuration is one where the first player can force a win.
    /// For example, (0,0,13), (0,11,11) and (5,5,5) are winning configurations because
    /// the first player can immediately remove all stones.
    ///
    /// A losing configuration is one where the second player can force a win, no
    /// matter what the first player does.
    /// For example, (0,1,2) and (1,3,3) are losing configurations: any legal move
    /// leaves a winning configuration for the second player.
    ///
    /// Consider all losing configurations (xi,yi,zi) where xi <= yi <= zi <= 100.
    /// We can verify that sum(xi+yi+zi) = 173895 for these.
    ///
    /// Find sum(xi+yi+zi) where (xi,yi,zi) ranges over the losing configurations with
    /// xi <= yi <= zi <= 1000.
    /// </summary>
    internal class Problem260 : Problem
    {
        private const int upper = 1000;

        public Problem260() : base(260) { }

        private int GetKey(int x, int y, int z)
        {
            return (x << 20) + (y << 10) + z;
        }

        private bool GetResult(BitVector wins, BitVector loses, int x, int y, int z)
        {
            int key = GetKey(x, y, z);
            bool ret = true;

            if (!wins[key] && !loses[key])
            {
                for (int n = 1; n <= z; n++)
                {
                    if (n <= x)
                    {
                        // Take n from x
                        ret = GetResult(wins, loses, x - n, y, z);
                        if (!ret)
                            break;
                        // Take n from x, y
                        ret = GetResult(wins, loses, x - n, y - n, z);
                        if (!ret)
                            break;
                        // Take n from x, z
                        if (z - n < y)
                            ret = GetResult(wins, loses, x - n, z - n, y);
                        else
                            ret = GetResult(wins, loses, x - n, y, z - n);
                        if (!ret)
                            break;
                        // Take n from x, y, z
                        ret = GetResult(wins, loses, x - n, y - n, z - n);
                        if (!ret)
                            break;
                    }

                    if (n <= y)
                    {
                        // Take n from y
                        if (y - n < x)
                            ret = GetResult(wins, loses, y - n, x, z);
                        else
                            ret = GetResult(wins, loses, x, y - n, z);
                        if (!ret)
                            break;
                        // Take n from y, z
                        if (z - n < x)
                            ret = GetResult(wins, loses, y - n, z - n, x);
                        else if (y - n < x)
                            ret = GetResult(wins, loses, y - n, x, z - n);
                        else
                            ret = GetResult(wins, loses, x, y - n, z - n);
                        if (!ret)
                            break;
                    }

                    // Take n from z
                    if (z - n < x)
                        ret = GetResult(wins, loses, z - n, x, y);
                    else if (z - n < y)
                        ret = GetResult(wins, loses, x, z - n, y);
                    else
                        ret = GetResult(wins, loses, x, y, z - n);
                    if (!ret)
                        break;
                }

                if (ret)
                    loses.Set(key);
                else
                    wins.Set(key);

                // if this is a losing configuration, add winning configuration
                if (ret)
                {
                    for (int n = 1; n <= upper - x; n++)
                    {
                        if (z + n <= upper)
                        {
                            // Add n for z
                            wins.Set(GetKey(x, y, z + n));
                            // Add n for y, z
                            wins.Set(GetKey(x, y + n, z + n));
                            // Add n for x, y, z
                            wins.Set(GetKey(x + n, y + n, z + n));
                        }
                        if (y + n <= upper)
                        {
                            // Add n for y
                            if (y + n > z)
                                wins.Set(GetKey(x, z, y + n));
                            else
                                wins.Set(GetKey(x, y + n, z));
                            // Add n for x, y
                            if (x + n > z)
                                wins.Set(GetKey(z, x + n, y + n));
                            else if (y + n > z)
                                wins.Set(GetKey(x + n, z, y + n));
                            else
                                wins.Set(GetKey(x + n, y + n, z));
                        }
                        // Add n for x
                        if (x + n > z)
                            wins.Set(GetKey(y, z, x + n));
                        else if (x + n > y)
                            wins.Set(GetKey(y, x + n, z));
                        else
                            wins.Set(GetKey(x + n, y, z));
                    }
                }
            }

            if (wins[key])
                return true;
            else
                return false;
        }

        protected override string Action()
        {
            BitVector wins = new BitVector(1 << 30), loses = new BitVector(1 << 30);
            long sum = 0;

            for (int z = 0; z <= upper; z++)
            {
                for (int y = 0; y <= z; y++)
                {
                    for (int x = 0; x <= y; x++)
                    {
                        if (!GetResult(wins, loses, x, y, z))
                            sum += (x + y + z);
                    }
                }
            }

            return sum.ToString();
        }
    }
}