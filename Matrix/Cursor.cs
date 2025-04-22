using QR_Generator.Constants.Enums.Matrix;

namespace QR_Generator.Matrix;

internal class Cursor
{
    private Direction Direction = Direction.Up;
    private bool shouldMoveLeft = true;
    private readonly int MatrixLenght;
    public int i, j;

    public Cursor(int matrixLenght)
    {
        MatrixLenght = matrixLenght;
        i = matrixLenght - 1;
        j = matrixLenght - 1;
    }

    public void Next()
    {
        if (shouldMoveLeft)
        {
            i--;
        }
        else
        {
            if (j - 1 < 0 || j + 1 > MatrixLenght)
            {
                i--;
                ToggleDirection();
            }
            else
            {
                Move(Direction);
                i++;
            }

        }

        shouldMoveLeft = !shouldMoveLeft;
    }

    public bool Done()
    {
        return i == 0 && j == MatrixLenght - 1;
    }

    public void ToggleDirection() => Direction = (Direction)((int)Direction ^ 1);

    private void Move(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                j--;
                break;
            case Direction.Down:
                j++;
                break;
        }
    }
}
