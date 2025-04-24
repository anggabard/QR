using QR_Generator.Constants.Enums.Matrix;
using QR_Generator.Services;

namespace QR_Generator.Matrix;

public class PatternsMatrix(int version) : BaseMatrix(version)
{
    public async Task DrawFixedPatterns()
    {
        await DrawFinderPattern(0, 0);
        await DrawFinderPattern(MatrixSize - 7, 0);
        await DrawFinderPattern(0, MatrixSize - 7);

        //Separators
        await DrawLine(7, 0, 8, Color.White);
        await DrawLine(0, 7, 7, Color.White, LineType.Vertical);
        await DrawLine(MatrixSize - 8, 0, 8, Color.White);
        await DrawLine(MatrixSize - 7, 7, 7, Color.White, LineType.Vertical);
        await DrawLine(7, MatrixSize - 8, 8, Color.White);
        await DrawLine(0, MatrixSize - 8, 7, Color.White, LineType.Vertical);

        await DrawAlignmentPatterns();

        //Dark Module
        matrix[4 * version + 9, 8] = 1;

        //Reserve Format Information
        await DrawLine(8, 0, 9, Color.White);
        await DrawLine(0, 8, 8, Color.White, LineType.Vertical);
        await DrawLine(8, MatrixSize - 8, 8, Color.White);
        await DrawLine(4 * version + 10, 8, 7, Color.White, LineType.Vertical);

        //Timing 
        await DrawDottedLine(6, 8, MatrixSize - 16);
        await DrawDottedLine(8, 6, MatrixSize - 16, LineType.Vertical);

        if (version >= 7)
        {
            await DrawVersionInformation();
        }
    }

    public DataMatrix ToDataMatrix()
    {
        return new DataMatrix(version, matrix);
    }

    private async Task DrawFinderPattern(int x, int y)
    {
        await DrawSquare(x++, y++, 7);
        await DrawSquare(x++, y++, 5, Color.White);
        await DrawSquare(x++, y++, 3);
        matrix[x, y] = 1;
    }

    private async Task DrawAlignmentPatterns()
    {
        var patternCenterCoordotantes = VersionAlignmentPatternService.Get(version);
        ArgumentNullException.ThrowIfNull(patternCenterCoordotantes);

        foreach (var (x, y) in patternCenterCoordotantes)
        {
            await DrawSquare(x - 2, y - 2, 5);
            await DrawSquare(x - 1, y - 1, 3, Color.White);
            matrix[x, y] = 1;
        }
    }

    private async Task DrawVersionInformation()
    {
        var versionInformationCode = VersionInformationPatternService.Get(version);

        ArgumentNullException.ThrowIfNull(versionInformationCode);
        if (versionInformationCode.Count != 6 * 3) throw new ArgumentOutOfRangeException(nameof(versionInformationCode));

        //Bottom-Left
        await Task.Run(() =>
        {
            int index = 0;
            for (int j = 5; j >= 0; j--)
            {
                for (int i = MatrixSize - 9; i >= MatrixSize - 11; i--)
                {
                    matrix[i, j] = versionInformationCode[index];
                    index++;
                }
            }
        });

        //Top-Right
        await Task.Run(() =>
        {
            int index = 0;
            for (int i = 5; i >= 0; i--)
            {
                for (int j = MatrixSize - 9; j >= MatrixSize - 11; j--)
                {
                    matrix[i, j] = versionInformationCode[index];
                    index++;
                }
            }
        });
    }

    private async Task DrawSquare(int x, int y, int lenght, Color color = Color.Black)
    {
        await DrawLine(x, y, lenght, color);
        await DrawLine(x + lenght - 1, y, lenght, color);
        await DrawLine(x + 1, y, lenght - 2, color, LineType.Vertical);
        await DrawLine(x + 1, y + lenght - 1, lenght - 2, color, LineType.Vertical);
    }

    private async Task DrawLine(int x, int y, int lenght, Color color = Color.Black, LineType lineType = LineType.Horizontal)
    {
        await Task.Run(() =>
        {
            switch (lineType)
            {
                case LineType.Horizontal:
                    {
                        for (int j = y; j < y + lenght; j++)
                        {
                            matrix[x, j] = (byte)color;
                        }
                        break;
                    }

                case LineType.Vertical:
                    {
                        for (int i = x; i < x + lenght; i++)
                        {
                            matrix[i, y] = (byte)color;
                        }
                        break;
                    }

                default:
                    break;
            }
        });
    }

    private async Task DrawDottedLine(int x, int y, int lenght, LineType lineType = LineType.Horizontal)
    {
        await Task.Run(() =>
        {
            byte value = 1;

            switch (lineType)
            {
                case LineType.Horizontal:
                    {
                        for (int j = y; j < y + lenght; j++)
                        {
                            matrix[x, j] = value;
                            value = (byte)(1 - value);
                        }
                        break;
                    }

                case LineType.Vertical:
                    {
                        for (int i = x; i < x + lenght; i++)
                        {
                            matrix[i, y] = value;
                            value = (byte)(1 - value);
                        }
                        break;
                    }

                default:
                    break;
            }
        });
    }
}
