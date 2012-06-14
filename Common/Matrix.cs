using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common
{
    public class Matrix
    {
        public Fraction this[int row, int column]
        {
            get { return cells[row, column]; }
            internal set { cells[row, column] = value; }
        }

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        private Fraction[,] cells;

        private Matrix(int rows, int columns)
        {
            cells = new Fraction[rows, columns];
            Rows = rows;
            Columns = columns;
        }

        public Matrix(IEnumerable<Fraction> data, int rows, int columns)
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

        public static Matrix operator +(Matrix lhs, Matrix rhs)
        {
            if (lhs.Rows != rhs.Rows || lhs.Columns != rhs.Columns)
                throw new InvalidOperationException("invalid size");

            var matrix = new Matrix(lhs.Rows, lhs.Columns);

            for (int r = 0; r < matrix.Rows; r++)
                for (int c = 0; c < matrix.Columns; c++)
                    matrix.cells[r, c] = lhs[r, c] + rhs[r, c];

            return matrix;
        }

        public static Matrix operator -(Matrix lhs, Matrix rhs)
        {
            if (lhs.Rows != rhs.Rows || lhs.Columns != rhs.Columns)
                throw new InvalidOperationException("invalid size");

            var matrix = new Matrix(lhs.Rows, lhs.Columns);

            for (int r = 0; r < matrix.Rows; r++)
                for (int c = 0; c < matrix.Columns; c++)
                    matrix.cells[r, c] = lhs[r, c] - rhs[r, c];

            return matrix;
        }

        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            if (lhs.Columns != rhs.Rows)
                throw new InvalidOperationException("invalid size");

            var matrix = new Matrix(lhs.Rows, rhs.Columns);

            for (int r = 0; r < matrix.Rows; r++)
                for (int c = 0; c < matrix.Columns; c++)
                {
                    matrix.cells[r, c] = 0;
                    for (int i = 0; i < lhs.Columns; i++)
                        matrix.cells[r, c] += lhs[r, i] * rhs[i, c];
                }

            return matrix;
        }
    }
}