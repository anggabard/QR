using QR_Generator.Constants.Enums;
using QR_Generator.Constants.Enums.Matrix;
using QR_Generator.Extensions;
using System.Text;

namespace QR_Generator.Matrix;

public class BaseMatrix
{
    protected byte?[,] matrix;
    protected readonly int version;
    private readonly ErrorCorrectionLevel errorCorrectionLevel;
    private readonly int maskPattern;

    protected int MatrixSize { get { return version * 4 + 17; } }

    public BaseMatrix(int version, ErrorCorrectionLevel errorCorrectionLevel, int maskPattern)
    {
        this.version = version;
        this.errorCorrectionLevel = errorCorrectionLevel;
        this.maskPattern = maskPattern;
        matrix = new byte?[MatrixSize, MatrixSize];
    }

    public async Task DrawFixedPatterns()
    {
        await DrawFinderPattern(0, 0);
        await DrawLine(7, 0, 8, Color.White);
        await DrawLine(0, 7, 7, Color.White, LineType.Vertical);

        await DrawFinderPattern(MatrixSize - 7, 0);
        await DrawLine(MatrixSize - 8, 0, 8, Color.White);
        await DrawLine(MatrixSize - 7, 7, 7, Color.White, LineType.Vertical);

        await DrawFinderPattern(0, MatrixSize - 7);
        await DrawLine(7, MatrixSize - 8, 8, Color.White);
        await DrawLine(0, MatrixSize - 8, 7, Color.White, LineType.Vertical);

        await DrawAlignmentPatterns();

        await DrawDottedLine(6, 8, MatrixSize - 16);
        await DrawDottedLine(8, 6, MatrixSize - 16, LineType.Vertical);

        matrix[4 * version + 9, 8] = 1;

        if (version >= 7)
        {
            //await PlaceFormatInfo();
        }
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
        var patternTopLeftLocations = GetAlignmentPatternTopLeftLocations();

        foreach (var (top, left) in patternTopLeftLocations)
        {
            await DrawSquare(top, left, 5);
            await DrawSquare(top + 1, left + 1, 3, Color.White);
            matrix[top + 2, left + 2] = 1;
        }
    }

    private List<Tuple<int, int>> GetAlignmentPatternTopLeftLocations()
    {
        List<int> positions = GetAlignmentPositions(version);
        List<Tuple<int, int>> patternTopLeftLocations = [];

        foreach (int centerRow in positions)
        {
            foreach (int centerCol in positions)
            {
                // Skip Finder Pattern areas
                if (!((centerRow == 6 && centerCol == 6) ||
                      (centerRow == 6 && centerCol == positions[^1]) ||
                      (centerRow == positions[^1] && centerCol == 6)))
                {
                    patternTopLeftLocations.Add(Tuple.Create(centerRow + 2, centerCol + 2));
                }
            }
        }

        return patternTopLeftLocations;
    }

    private List<int> GetAlignmentPositions(int version)
    {
        if (version == 1) return []; // No alignment patterns in Version 1

        int numPatterns = (version / 7) + 2; // Calculate the number of alignment positions
        List<int> positions = [];

        // Calculate equally spaced positions
        int start = 6;
        int end = 4 * version + 6; // Max matrix size
        int step = (end - start) / (numPatterns - 1);

        for (int i = 0; i < numPatterns; i++)
        {
            positions.Add(start + (i * step));
        }

        return positions;
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
                    sb.Append(matrix[i, j] == Color.Black.ToNullableByte() ? "██" : "  ");
                }
                sb.Append(' ');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
