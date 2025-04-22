using System.Text;

namespace QR_Generator.Matrix;

public class BaseMatrix
{
    protected readonly int version;
    protected byte?[,] matrix;
    protected int MatrixSize { get { return version * 4 + 17; } }

    public BaseMatrix(int version)
    {
        this.version = version;
        matrix = new byte?[MatrixSize, MatrixSize];
    }


    public override string ToString()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < MatrixSize; i++)
        {
            for (int j = 0; j < MatrixSize; j++)
            {

                if (matrix[i, j] == null)
                {
                    sb.Append("XX");
                }
                else
                {
                    sb.Append(matrix[i, j] == 1 ? "██" : "  ");
                }
                sb.Append(' ');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
