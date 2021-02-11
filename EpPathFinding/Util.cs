namespace EpPathFinding
{
    public class Util
    {
        public static DiagonalMovement GetDiagonalMovement(bool iCrossCorners, bool iCrossAdjacentPoint)
        {
            switch (iCrossCorners)
            {
                case true when iCrossAdjacentPoint:
                    return DiagonalMovement.Always;
                case true:
                    return DiagonalMovement.IfAtLeastOneWalkable;
                default:
                    return DiagonalMovement.OnlyWhenNoObstacles;
            }
        }
    }
}
