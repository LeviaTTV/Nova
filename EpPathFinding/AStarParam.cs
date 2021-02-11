using EpPathFinding.Grid;

namespace EpPathFinding
{
    public class AStarParam : ParamBase
    {
        public delegate float HeuristicDelegate(int iDx, int iDy);


        public float Weight;

        public AStarParam(BaseGrid iGrid, GridPos iStartPos, GridPos iEndPos, float iweight, DiagonalMovement iDiagonalMovement = DiagonalMovement.Always, HeuristicMode iMode = HeuristicMode.Euclidean)
            : base(iGrid,iStartPos,iEndPos, iDiagonalMovement,iMode)
        {
            Weight = iweight;
        }

        public AStarParam(BaseGrid iGrid, float iweight, DiagonalMovement iDiagonalMovement = DiagonalMovement.Always, HeuristicMode iMode = HeuristicMode.Euclidean)
            : base(iGrid, iDiagonalMovement, iMode)
        {
            Weight = iweight;
        }

        internal override void _reset(GridPos iStartPos, GridPos iEndPos, BaseGrid iSearchGrid = null)
        {
        }
    }
}