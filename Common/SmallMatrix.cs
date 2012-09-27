using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common
{
    public class SmallMatrix
    {
        public long this[int row, int column]
        {
            get { return cells[row, column]; }
            internal set { cells[row, column] = value; }
        }

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        private long[,] cells;

        private SmallMatrix(int rows, int columns)
        {
            cells = new long[rows, columns];
            Rows = rows;
            Columns = columns;
        }

        public SmallMatrix(IEnumerable<long> data, int rows, int columns)
            : this(rows, columns)
        {
            int rid = 0, cid = 0;

            foreach (var cell in data)
            {
                cells[rid, cid] = cell;

                cid++;
                if (cid == columns)
                {
                    cid = 0;
                    rid++;
                }
            }
            while (rid != rows)
            {
                cells[rid, cid] = 0;

                cid++;
                if (cid == columns)
                {
                    cid = 0;
                    rid++;
                }
            }
        }

        public SmallMatrix(SmallMatrix other)
            : this(other.Rows, other.Columns)
        {
            for (int rid = 0; rid < other.Rows; rid++)
            {
                for (int cid = 0; cid < other.Columns; cid++)
                    cells[rid, cid] = other.cells[rid, cid];
            }
        }

        public static SmallMatrix operator +(SmallMatrix lhs, SmallMatrix rhs)
        {
            if (lhs.Rows != rhs.Rows || lhs.Columns != rhs.Columns)
                throw new InvalidOperationException("invalid size");

            var matrix = new SmallMatrix(lhs.Rows, lhs.Columns);

            for (int r = 0; r < matrix.Rows; r++)
                for (int c = 0; c < matrix.Columns; c++)
                    matrix.cells[r, c] = lhs[r, c] + rhs[r, c];

            return matrix;
        }

        public static SmallMatrix operator -(SmallMatrix lhs, SmallMatrix rhs)
        {
            if (lhs.Rows != rhs.Rows || lhs.Columns != rhs.Columns)
                throw new InvalidOperationException("invalid size");

            var matrix = new SmallMatrix(lhs.Rows, lhs.Columns);

            for (int r = 0; r < matrix.Rows; r++)
                for (int c = 0; c < matrix.Columns; c++)
                    matrix.cells[r, c] = lhs[r, c] - rhs[r, c];

            return matrix;
        }

        public static SmallMatrix operator *(SmallMatrix lhs, SmallMatrix rhs)
        {
            if (lhs.Columns != rhs.Rows)
                throw new InvalidOperationException("invalid size");

            var matrix = new SmallMatrix(lhs.Rows, rhs.Columns);

            for (int r = 0; r < matrix.Rows; r++)
            {
                for (int c = 0; c < matrix.Columns; c++)
                {
                    matrix.cells[r, c] = 0;
                    for (int i = 0; i < lhs.Columns; i++)
                        matrix.cells[r, c] += lhs[r, i] * rhs[i, c];
                }
            }

            return matrix;
        }

        public static SmallMatrix operator %(SmallMatrix other, long modulo)
        {
            SmallMatrix ret = new SmallMatrix(other);

            for (int rid = 0; rid < ret.Rows; rid++)
            {
                for (int cid = 0; cid < ret.Columns; cid++)
                    ret.cells[rid, cid] %= modulo;
            }

            return ret;
        }

        public static SmallMatrix ModPow(SmallMatrix matrix, long e, long modulo)
        {
            SmallMatrix ret = new SmallMatrix(matrix.Rows, matrix.Columns), factor = new SmallMatrix(matrix);

            if (matrix.Rows != matrix.Columns)
                throw new ArgumentException("invalid matrix!");
            for (int i = 0; i < matrix.Rows; i++)
                ret[i, i] = 1;
            while (e != 0)
            {
                if ((e & 1) != 0)
                    ret = ret * factor % modulo;
                factor = factor * factor % modulo;
                e >>= 1;
            }

            return ret;
        }
    }
}