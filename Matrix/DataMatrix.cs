namespace QR_Generator.Matrix;

public class DataMatrix : BaseMatrix
{
    private readonly byte?[,] patternsMatrix;

    internal DataMatrix(int version, byte?[,] patternsMatrix) : base(version)
    {
        this.patternsMatrix = patternsMatrix;
        //matrix = patternsMatrix;
    }

    public Task SetData(List<byte> data)
    {
        return Task.Run(() =>
        {
            try
            {
                for (var cursor = new Cursor(MatrixSize); !cursor.Done(); cursor.Next())
                {
                    var bit = data.ElementAtOrDefault(cursor.byteIndex);

                    if (cursor.i == 8 && cursor.j == 5) cursor.j--; //Skip the timing line
                    if (patternsMatrix[cursor.i, cursor.j] != null) continue;

                    matrix[cursor.i, cursor.j] = bit;
                    cursor.byteIndex++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        });
    }
}
