using QR_Generator.Constants.Enums;
using System.Threading.Tasks;

namespace QR_Generator.Services;

public class MaskService(byte?[,] patternsMatrix, byte?[,] dataMatrix, int matrixSize)
{
    private readonly byte?[,] patternsMatrix = patternsMatrix;
    private readonly byte?[,] dataMatrix = dataMatrix;
    private readonly int MatrixSize = matrixSize;

    public readonly Dictionary<Masks, byte[,]> masks = [];
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

    public async Task GenerateMasks(ErrorCorrectionLevel ecl)
    {
        foreach (var key in formulas.Keys)
        {
            masks[key] = new byte[MatrixSize, MatrixSize];
            await GenerateMask(key, ecl);
        }
        //await Task.WhenAll(formulas.Keys.Select(mask => GenerateMask(mask, ecl)));
    }

    private Task GenerateMask(Masks mask, ErrorCorrectionLevel ecl)
    {
        return Task.Run(() =>
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
}
