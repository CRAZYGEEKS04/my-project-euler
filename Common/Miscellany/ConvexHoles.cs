using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public class ConvexHoles
    {
        private int N;
        private int[][] p;
        private List<int> pn;
        private double[] lean;
        private long answer;
        private int li;

        private class LeanComparer : IComparer<int>
        {
            private double[] lean;
            private int[][] p;

            public LeanComparer(int[][] p, double[] lean)
            {
                this.p = p;
                this.lean = lean;
            }

            public int Compare(int a, int b)
            {
                if (lean[a] == lean[b])
                    if (p[a][0] > p[b][0])
                        return -1;
                    else if (p[a][0] < p[b][0])
                        return 1;
                    else
                        return 0;
                else if (lean[a] > lean[b])
                    return 1;
                else
                    return -1;
            }
        }

        private void SortPoints()
        {
            int min, tmpx, tmpy;

            for (int i = 0; i < N - 1; i++)
            {
                min = i;
                for (int j = i + 1; j < N; j++)
                {
                    if ((p[min][0] > p[j][0]) || ((p[min][0] == p[j][0]) && (p[min][1] > p[j][1])))
                        min = j;
                }

                if (min != i)
                {
                    tmpx = p[min][0];
                    tmpy = p[min][1];
                    p[min][0] = p[i][0];
                    p[min][1] = p[i][1];
                    p[i][0] = tmpx;
                    p[i][1] = tmpy;
                }
            }
        }

        private void SortPP()
        {
            int j;

            for (j = li + 1; j < N; j++)
            {
                pn[j] = j;
                if (p[li][0] == p[j][0])
                {
                    if (p[li][1] > p[j][1])
                        lean[j] = -5000000.0 + (p[li][1] - p[j][1]);
                    else
                        lean[j] = 5000000.0 - (p[j][1] - p[li][1]);
                }
                else
                    lean[j] = (float)(p[li][1] - p[j][1]) / (float)(p[li][0] - p[j][0]);
            }

            pn.Sort(li + 1, N - li - 1, new LeanComparer(p, lean));
        }

        private int SameSide(int a0, int a1, int a2, int a3)
        {
            int x_diff, y_diff, v0, v1;

            y_diff = p[a1][1] - p[a0][1];
            x_diff = p[a1][0] - p[a0][0];
            v0 = (p[a3][1] - p[a0][1]) * x_diff - y_diff * (p[a3][0] - p[a0][0]);
            v1 = (p[a2][1] - p[a0][1]) * x_diff - y_diff * (p[a2][0] - p[a0][0]);

            if ((v0 == 0) || (v1 == 0))
                return 0;
            else if (((v0 < 0) && (v1 > 0)) || ((v0 > 0) && (v1 < 0)))
                return -1;
            else
                return 1;
        }

        private void Recur(int t1, int t2, int min, long s)
        {
            int i;
            long s_orig = s;
            long s_tmp;
            int t3;
            int tx, ty;

            tx = t1; ty = t2;

            for (i = min; i < N; i++)
            {
                t3 = pn[i];

                if (SameSide(tx, ty, li, t3) >= 0)
                {
                    s_tmp = -(p[t3][0] - p[li][0]) * (p[t2][1] - p[li][1]) + (p[t3][1] - p[li][1]) * (p[t2][0] - p[li][0]);
                    s = s_orig + s_tmp;
                    if (s > answer)
                        answer = s;

                    Recur(t2, t3, i + 1, s);
                    tx = t2;
                    ty = t3;
                }
            }
        }

        public long Solve(List<int> nums)
        {
            N = nums.Count / 2;
            p = new int[N][];
            pn = new int[N].ToList();
            lean = new double[N];
            answer = 0;

            for (int i = 0; i < nums.Count; i += 2)
                p[i / 2] = new int[] { nums[i], nums[i + 1] };
            SortPoints();

            for (int i = 0; i < N - 2; i++)
            {
                li = i;
                SortPP();
                for (int j = i + 1; j < N - 1; j++)
                    Recur(i, pn[j], j + 1, 0);
            }

            return answer;
        }
    }
}