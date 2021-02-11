/*! 
@file DynamicGridWPool.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief DynamicGrid with Pool Interface
@version 2.0

@section LICENSE

The MIT License (MIT)

Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

@section DESCRIPTION

An Interface for the DynamicGrid with Pool Class.

*/

using System.Collections.Generic;

namespace EpPathFinding.Grid
{
    public class DynamicGridWPool : BaseGrid
    {
         private bool m_notSet;
         private readonly NodePool m_nodePool;

        public override int width
        {
            get
            {
                if (m_notSet)
                    setBoundingBox();
                return m_gridRect.MaxX - m_gridRect.MinX + 1;
            }
            protected set
            {

            }
        }

        public override int height
        {
            get
            {
                if (m_notSet)
                    setBoundingBox();
                return m_gridRect.MaxY - m_gridRect.MinY + 1;
            }
            protected set
            {

            }
        }

        public DynamicGridWPool(NodePool iNodePool)
            : base()
        {
            m_gridRect = new GridRect();
            m_gridRect.MinX = 0;
            m_gridRect.MinY = 0;
            m_gridRect.MaxX = 0;
            m_gridRect.MaxY = 0;
            m_notSet = true;
            m_nodePool = iNodePool;
        }

        public DynamicGridWPool(DynamicGridWPool b)
            : base(b)
        {
            m_notSet = b.m_notSet;
            m_nodePool = b.m_nodePool;
        }

        public override Node GetNodeAt(int iX, int iY)
        {
            GridPos pos = new GridPos(iX, iY);
            return GetNodeAt(pos);
        }

        public override bool IsWalkableAt(int iX, int iY)
        {
            GridPos pos = new GridPos(iX, iY);
            return IsWalkableAt(pos);
        }

        private void setBoundingBox()
        {
            m_notSet = true;
            foreach (KeyValuePair<GridPos, Node> pair in m_nodePool.Nodes)
            {
                if (pair.Key.x < m_gridRect.MinX || m_notSet)
                    m_gridRect.MinX = pair.Key.x;
                if (pair.Key.x > m_gridRect.MaxX || m_notSet)
                    m_gridRect.MaxX = pair.Key.x;
                if (pair.Key.y < m_gridRect.MinY || m_notSet)
                    m_gridRect.MinY = pair.Key.y;
                if (pair.Key.y > m_gridRect.MaxY || m_notSet)
                    m_gridRect.MaxY = pair.Key.y;
                m_notSet = false;
            }
            m_notSet = false;
        }

        public override bool SetWalkableAt(int iX, int iY, bool iWalkable)
        {
            GridPos pos = new GridPos(iX, iY);
            m_nodePool.SetNode(pos, iWalkable);
            if (iWalkable)
            {
                if (iX < m_gridRect.MinX || m_notSet)
                    m_gridRect.MinX = iX;
                if (iX > m_gridRect.MaxX || m_notSet)
                    m_gridRect.MaxX = iX;
                if (iY < m_gridRect.MinY || m_notSet)
                    m_gridRect.MinY = iY;
                if (iY > m_gridRect.MaxY || m_notSet)
                    m_gridRect.MaxY = iY;
                //m_notSet = false;
            }
            else
            {
                if (iX == m_gridRect.MinX || iX == m_gridRect.MaxX || iY == m_gridRect.MinY || iY == m_gridRect.MaxY)
                    m_notSet = true;
                
            }
            return true;
        }

        public override Node GetNodeAt(GridPos iPos)
        {
            return m_nodePool.GetNode(iPos);
        }

        public override bool IsWalkableAt(GridPos iPos)
        {
            return  m_nodePool.Nodes.ContainsKey(iPos);
        }

        public override bool SetWalkableAt(GridPos iPos, bool iWalkable)
        {
            return SetWalkableAt(iPos.x, iPos.y, iWalkable);
        }


        public override void Reset()
        {
            foreach (KeyValuePair<GridPos, Node> keyValue in m_nodePool.Nodes)
            {
                keyValue.Value.Reset();
            }
        }

        public override BaseGrid Clone()
        {
            DynamicGridWPool tNewGrid = new DynamicGridWPool(m_nodePool);
            return tNewGrid;
        }
    }

}