using System.Text;

namespace QR_Generator.Extensions;

public static class ByteMatrixExtension
{
    public static string ToQR(this byte[,] matrix, bool asBinary = false)
    {
        var matrixSize = matrix.GetLength(0);
        var sb = new StringBuilder();

        for (int i = 0; i < matrixSize; i++)
        {
            for (int j = 0; j < matrixSize; j++)
            {
                if (asBinary)
                {
                    sb.Append(matrix[i, j]);
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
