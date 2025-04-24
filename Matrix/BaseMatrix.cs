using System.Text;

namespace QR_Generator.Matrix;

public class BaseMatrix
{
    protected readonly int version;
    protected byte?[,] matrix;
    public int MatrixSize { get { return version * 4 + 17; } }

    public BaseMatrix(int version)
    {
        this.version = version;
        matrix = new byte?[MatrixSize, MatrixSize];
    }

    public byte?[,] GetMatrix() { return matrix; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < MatrixSize; i++)
        {
            for (int j = 0; j < MatrixSize; j++)
            {
                if (matrix[i, j] == null || matrix[i, j] < 0) sb.Append("XX");
                else if (matrix[i, j] == 1) sb.Append("██");
                else if (matrix[i, j] == 0) sb.Append("  ");
                else if (matrix[i, j] < 9) sb.Append('0' + matrix[i, j].ToString());
                else if (matrix[i, j] > 9 && matrix[i, j] < 100) sb.Append(matrix[i, j]);
                else  sb.Append(matrix[i, j] % 100);

                sb.Append(' ');
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
