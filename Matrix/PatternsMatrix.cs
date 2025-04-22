using QR_Generator.Constants.Enums;
using QR_Generator.Constants.Enums.Matrix;

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
            //TODO: Add version information
        }
    }

    public DataMatrix ToDataMatrix(ErrorCorrectionLevel errorCorrectionLevel)
    {
        return new DataMatrix(version, matrix, errorCorrectionLevel);
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
}
