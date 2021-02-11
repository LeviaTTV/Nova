using C5;
using EpPathFinding.Grid;

namespace EpPathFinding
{
    public class JumpPointParam : ParamBase
    {
        public JumpPointParam(BaseGrid iGrid, GridPos iStartPos, GridPos iEndPos, EndNodeUnWalkableTreatment iAllowEndNodeUnWalkable = EndNodeUnWalkableTreatment.Allow, DiagonalMovement iDiagonalMovement = DiagonalMovement.Always, HeuristicMode iMode = HeuristicMode.Euclidean)
            : base(iGrid, iStartPos, iEndPos, iDiagonalMovement, iMode)
        {

            CurEndNodeUnWalkableTreatment = iAllowEndNodeUnWalkable;
            openList = new IntervalHeap<Node>();

            CurIterationType = IterationType.Loop;
        }

        public JumpPointParam(BaseGrid iGrid, EndNodeUnWalkableTreatment iAllowEndNodeUnWalkable = EndNodeUnWalkableTreatment.Allow, DiagonalMovement iDiagonalMovement = DiagonalMovement.Always, HeuristicMode iMode = HeuristicMode.Euclidean)
            : base(iGrid, iDiagonalMovement, iMode)
        {
            CurEndNodeUnWalkableTreatment = iAllowEndNodeUnWalkable;

            openList = new IntervalHeap<Node>();
            CurIterationType = IterationType.Loop;
        }

        public JumpPointParam(JumpPointParam b) : base(b)
        {
            m_heuristic = b.m_heuristic;
            CurEndNodeUnWalkableTreatment = b.CurEndNodeUnWalkableTreatment;

            openList = new IntervalHeap<Node>();
            openList.AddAll(b.openList);

            CurIterationType = b.CurIterationType;
        }
        
        internal override void _reset(GridPos iStartPos, GridPos iEndPos, BaseGrid iSearchGrid = null)
        {
            openList = new IntervalHeap<Node>();
        }

        public EndNodeUnWalkableTreatment CurEndNodeUnWalkableTreatment
        {
            get;
            set;
        }
        public IterationType CurIterationType
        {
            get;
            set;
        }

        public IntervalHeap<Node> openList;

    }
}