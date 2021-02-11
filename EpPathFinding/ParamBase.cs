using EpPathFinding.Grid;

namespace EpPathFinding
{
    public delegate float HeuristicDelegate(int iDx, int iDy);

    public abstract class ParamBase
    {
        protected ParamBase(BaseGrid iGrid, GridPos iStartPos, GridPos iEndPos, DiagonalMovement iDiagonalMovement, HeuristicMode iMode) : this(iGrid, iDiagonalMovement, iMode)
        {
            m_startNode = m_searchGrid.GetNodeAt(iStartPos.x, iStartPos.y);
            m_endNode = m_searchGrid.GetNodeAt(iEndPos.x, iEndPos.y);
            if (m_startNode == null)
                m_startNode = new Node(iStartPos.x, iStartPos.y, true);
            if (m_endNode == null)
                m_endNode = new Node(iEndPos.x, iEndPos.y, true);
        }

        protected ParamBase(BaseGrid iGrid, DiagonalMovement iDiagonalMovement, HeuristicMode iMode)
        {
            SetHeuristic(iMode);

            m_searchGrid = iGrid;
            DiagonalMovement = iDiagonalMovement;
            m_startNode = null;
            m_endNode = null;
        }

        protected ParamBase(ParamBase param)
        {
            m_searchGrid = param.m_searchGrid;
            DiagonalMovement = param.DiagonalMovement;
            m_startNode = param.m_startNode;
            m_endNode = param.m_endNode;
            
        }

        internal abstract void _reset(GridPos iStartPos, GridPos iEndPos, BaseGrid iSearchGrid = null);

        public void Reset(GridPos iStartPos, GridPos iEndPos, BaseGrid iSearchGrid = null)
        {
            _reset(iStartPos, iEndPos, iSearchGrid);
            m_startNode = null;
            m_endNode = null;

            if (iSearchGrid != null)
                m_searchGrid = iSearchGrid;
            m_searchGrid.Reset();
            m_startNode = m_searchGrid.GetNodeAt(iStartPos.x, iStartPos.y);
            m_endNode = m_searchGrid.GetNodeAt(iEndPos.x, iEndPos.y);
            if (m_startNode == null)
                m_startNode = new Node(iStartPos.x, iStartPos.y, true);
            if (m_endNode == null)
                m_endNode = new Node(iEndPos.x, iEndPos.y, true);
        }

        public DiagonalMovement DiagonalMovement;
        public HeuristicDelegate HeuristicFunc
        {
            get
            {
                return m_heuristic;
            }
        }

        public BaseGrid SearchGrid
        {
            get
            {
                return m_searchGrid;
            }
        }

        public Node StartNode
        {
            get
            {
                return m_startNode;
            }
        }
        public Node EndNode
        {
            get
            {
                return m_endNode;
            }
        }

        public void SetHeuristic(HeuristicMode iMode)
        {
            m_heuristic = null;
            switch (iMode)
            {
                case HeuristicMode.Manhattan:
                    m_heuristic = new HeuristicDelegate(Heuristic.Manhattan);
                    break;
                case HeuristicMode.Euclidean:
                    m_heuristic = new HeuristicDelegate(Heuristic.Euclidean);
                    break;
                case HeuristicMode.Chebyshev:
                    m_heuristic = new HeuristicDelegate(Heuristic.Chebyshev);
                    break;
                default:
                    m_heuristic = new HeuristicDelegate(Heuristic.Euclidean);
                    break;
            }
        }

        protected BaseGrid m_searchGrid;
        protected Node m_startNode;
        protected Node m_endNode;
        protected HeuristicDelegate m_heuristic;
    }
}
