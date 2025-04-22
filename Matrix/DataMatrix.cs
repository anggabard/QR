using QR_Generator.Constants.Enums;

namespace QR_Generator.Matrix;

public class DataMatrix : BaseMatrix
{
    private readonly byte?[,] patternsMatrix;
    private readonly ErrorCorrectionLevel errorCorrectionLevel;

    internal DataMatrix(int version, byte?[,] patternsMatrix, ErrorCorrectionLevel errorCorrectionLevel) : base(version)
    {
        this.patternsMatrix = patternsMatrix;
        this.errorCorrectionLevel = errorCorrectionLevel;
    }

    public Task SetData(List<byte> data)
    {
        return Task.Run(() =>
        {
            int index = 0;
            for (var cursor = new Cursor(MatrixSize); !cursor.Done(); cursor.Next())
            {
                if (index > data.Count - 1) return;
                var bit = data.ElementAt(index);

                if (patternsMatrix[cursor.i, cursor.j] != null) continue;
                //TODO: Fix this

                matrix[cursor.i, cursor.j] = bit;
                index++;
            }
        });
    }
}
