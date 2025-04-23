using QR_Generator.Constants.Enums.Matrix;

namespace QR_Generator.Matrix;

internal class Cursor(int matrixLenght)
{
    private readonly int MatrixLenght = matrixLenght;

    private Direction Direction = Direction.Up;
    private bool shouldMoveLeft = true;
    public int i = matrixLenght - 1, j = matrixLenght - 1, byteIndex = 0;

    public void Next()
    {
        if (shouldMoveLeft)
        {
            j--; 
        }
        else
        {
            if (i - 1 < 0 && Direction == Direction.Up)
            {
                j--;
                ToggleDirection();
            } 
            else if (i + 1 > MatrixLenght - 1 && Direction == Direction.Down)
            {
                j--;
                ToggleDirection();
            }
            else
            {
                Move(Direction);
            }
        }

        shouldMoveLeft = !shouldMoveLeft;
    }

    public bool Done() => i == MatrixLenght - 1 && j == 0;

    public void ToggleDirection() => Direction = (Direction)((int)Direction ^ 1);

    private void Move(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                i--;
                break;
            case Direction.Down:
                i++;
                break;
        }
        j++;
    }
}
