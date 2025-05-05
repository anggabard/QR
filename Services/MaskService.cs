using QR_Generator.Constants.Enums;
using System.Collections.Concurrent;

namespace QR_Generator.Services;

public class MaskService(byte?[,] patternsMatrix, byte?[,] dataMatrix, int matrixSize)
{
    private readonly byte?[,] patternsMatrix = patternsMatrix;
    private readonly byte?[,] dataMatrix = dataMatrix;
    private readonly int MatrixSize = matrixSize;

    public readonly ConcurrentDictionary<Masks, byte[,]> masks = [];
    public readonly ConcurrentDictionary<(Masks, Penalty), int> penalityTable = [];
    private readonly Dictionary<Masks, Func<int, int, bool>> formulas = new()
    {
        {Masks.Zero, (i, j) =>  (i + j) % 2 == 0},
        {Masks.One, (i, j) => i % 2 == 0},
        {Masks.Two, (i, j) => j % 3 == 0},
        {Masks.Three, (i, j) => (i + j) % 3 == 0},
        {Masks.Four, (i, j) => (i/2 + j/3) % 2 == 0},
        {Masks.Five, (i, j) => i*j % 2 + i*j % 3 == 0},
        {Masks.Six, (i, j) => (i*j % 3 + i*j) % 2 == 0},
        {Masks.Seven, (i, j) => (i*j % 3 + i + j) % 2 == 0}
    };
    private static readonly byte[] RULE_3_PATTERN = [1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 0];
    private static readonly byte[] RULE_3_REVERSED_PATTERN = RULE_3_PATTERN.Reverse().ToArray();

    public async Task GenerateMasks(ErrorCorrectionLevel ecl)
    {
        foreach (var mask in formulas.Keys)
        {
            masks[mask] = new byte[MatrixSize, MatrixSize];
            await GenerateMask(mask, ecl);
        }
    }

    public byte[,] GetFinalQR()
    {
        var maxPenality = int.MaxValue;
        byte[,] result = new byte[MatrixSize, MatrixSize];
        foreach (var mask in formulas.Keys)
        {
            if (penalityTable[(mask, Penalty.Total)] < maxPenality)
            {
                result = masks[mask];
                maxPenality = penalityTable[(mask, Penalty.Total)];
            }
        }

        return result;
    }

    private async Task GenerateMask(Masks mask, ErrorCorrectionLevel ecl)
    {
        await Task.Run(() =>
        {
            var patternsMatrixCopy = AddFormatInformation(patternsMatrix, mask, ecl);

            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    if (dataMatrix[i, j] == null)
                    {
                        masks[mask][i, j] = patternsMatrixCopy[i, j].GetValueOrDefault();
                        continue;
                    }

                    var maskValue = Convert.ToByte(formulas[mask].Invoke(i, j));
                    masks[mask][i, j] = (byte)((maskValue + dataMatrix[i, j].GetValueOrDefault()) % 2);
                }
            }
        });

        await Task.WhenAll([Rule1(mask), Rule2(mask), Rule3(mask), Rule4(mask)]);
        await ComputeTotalPenallity(mask);
    }

    private byte?[,] AddFormatInformation(byte?[,] patternsMatrixCopy, Masks mask, ErrorCorrectionLevel ecl)
    {
        var formatInformationCode = FormatInformationPatternService.Get(ecl, mask);
        if (formatInformationCode == null || formatInformationCode.Count != 15) throw new Exception("Invalid length of formatInformationCode");

        var informationPatternPosition = VersionInformationPatternService.GetPatternPositions(MatrixSize);

        for (int i = 0; i <= 14; i++)
        {
            foreach (var position in informationPatternPosition[i])
            {
                patternsMatrixCopy[position.Item1, position.Item2] = formatInformationCode[i];
            }
        }

        return patternsMatrixCopy;
    }

    private Task ComputeTotalPenallity(Masks mask)
    {
        return Task.Run(() =>
        {
            penalityTable[(mask, Penalty.Total)] = penalityTable[(mask, Penalty.LinearHorizontal)] +
                                                    penalityTable[(mask, Penalty.LinearVertical)] +
                                                    penalityTable[(mask, Penalty.Box)] +
                                                    penalityTable[(mask, Penalty.FinderHorizontal)] +
                                                    penalityTable[(mask, Penalty.FinderVertical)] +
                                                    penalityTable[(mask, Penalty.Balance)];
        });
    }

    private Task Rule1(Masks mask)
    {
        return Task.WhenAll([CountLinearPenalityVertical(mask), CountLinearPenalityHorizontal(mask)]);
    }

    private Task Rule2(Masks mask)
    {
        return Task.Run(() =>
        {
            int blocks = 0;
            for (int row = 0; row < MatrixSize - 1; row++)
            {
                for (int col = 0; col < MatrixSize - 1; col++)
                {
                    var module = masks[mask][row, col];
                    if (
                        masks[mask][row, col + 1].Equals(module) &&
                        masks[mask][row + 1, col].Equals(module) &&
                        masks[mask][row + 1, col + 1].Equals(module)
                    )
                    {
                        blocks++;
                    }
                }
            }

            penalityTable[(mask, Penalty.Box)] = blocks * 3;
        });
    }

    private Task Rule3(Masks mask)
    {
        return Task.WhenAll([CountFinderPenalityVertical(mask), CountFinderPenalityHorizontal(mask)]);
    }

    private Task Rule4(Masks mask)
    {
        return Task.Run(() =>
        {
            float darkModules = 0;
            for (int row = 0; row < matrixSize; row++)
            {
                for (int col = 0; col < matrixSize; col++)
                {
                    darkModules += masks[mask][row, col];
                }

            }

            var totalModules = MatrixSize * MatrixSize;
            float percentage = darkModules * 100 / totalModules;
            var roundedPercentage = percentage > 50
                ? (int)Math.Floor(percentage / 5.0) * 5
                : (int)Math.Ceiling(percentage / 5.0) * 5;
            penalityTable[(mask, Penalty.Balance)] = Math.Abs(roundedPercentage - 50) * 2; ;
        });
    }

    private Task CountLinearPenalityHorizontal(Masks mask)
    {
        return Task.Run(() =>
        {
            int penality = 0;
            for (int row = 0; row < matrixSize; row++)
            {
                byte bit = masks[mask][row, 1];
                int runLength = 1;
                for (int col = 1; col < matrixSize; col++)
                {
                    if (masks[mask][row, col].Equals(masks[mask][row, col - 1]))
                    {
                        runLength++;
                    }
                    else
                    {
                        if (runLength >= 5) penality += runLength - 2;

                        runLength = 1;
                    }
                }

                if (runLength >= 5) penality += runLength - 2;
            }

            penalityTable[(mask, Penalty.LinearHorizontal)] = penality;
        });
     }

    private Task CountLinearPenalityVertical(Masks mask)
    {
        return Task.Run(() =>
        {
            int penality = 0;
            for (int col = 0; col < matrixSize; col++)
            {
                int runLength = 1;
                for (int row = 1; row < matrixSize; row++)
                {
                    if (masks[mask][row, col].Equals(masks[mask][row - 1, col]))
                    {
                        runLength++;
                    }
                    else
                    {
                        if (runLength >= 5) penality += runLength - 2;
                        runLength = 1;
                    }
                }
                if (runLength >= 5) penality += runLength - 2;
            }

            penalityTable[(mask, Penalty.LinearVertical)] = penality;
        });
    }

    private Task CountFinderPenalityHorizontal(Masks mask)
    {
        return Task.Run(() => 
        {
            int patterns = 0;
            for (int row = 0; row < MatrixSize; row++)
            {
                for (int col = 0; col <= MatrixSize - RULE_3_PATTERN.Length; col++)
                {
                    bool matches = MatchesPattern(masks[mask], row, col, RULE_3_PATTERN, isRow: true) ||
                                   MatchesPattern(masks[mask], row, col, RULE_3_REVERSED_PATTERN, isRow: true);
                    if (matches)
                        patterns++;
                }
            }

            penalityTable[(mask, Penalty.FinderHorizontal)] = patterns * 40;
        });
    }

    private Task CountFinderPenalityVertical(Masks mask)
    {
        return Task.Run(() =>
        {
            int patterns = 0;
            for (int col = 0; col < MatrixSize; col++)
            {
                for (int row = 0; row <= MatrixSize - RULE_3_PATTERN.Length; row++)
                {
                    bool matches = MatchesPattern(masks[mask], row, col, RULE_3_PATTERN, isRow: false) ||
                                   MatchesPattern(masks[mask], row, col, RULE_3_REVERSED_PATTERN, isRow: false);
                    if (matches)
                        patterns++;
                }
            }

            penalityTable[(mask, Penalty.FinderVertical)] = patterns * 40;
        });
    }

    private bool MatchesPattern(byte[,] matrix, int startRow, int startCol, byte[] pattern, bool isRow)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            int value = isRow ? matrix[startRow, startCol + i] : matrix[startRow + i, startCol];
            if (value != pattern[i])
                return false;
        }
        return true;
    }
}
