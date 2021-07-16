namespace YoonFactory
{
    public enum eYoonDir2DMode : int
    {
        Fixed = 0,
        Clock4,
        AntiClock4,
        Clock8,
        AntiClock8,
        Increase,
        Decrease,
        Whirlpool,
        AntiWhirlpool,
        AxisX,
        AxisY,
    }

    public enum eYoonStatus : int
    {
        Normal = 0,
        Conform,
        Send,
        Receive,
        User,
        Inspect,
        Info,
        OK,
        NG,
        Error,
    }

    // Init position is the quadrant of 1 (At Top-Right)
    public enum eYoonDir2D : int
    {
        None = -1,
        Center = 0,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
    }

    public static class YoonDirFactory
    {
        public static eYoonDir2D Go(this eYoonDir2D e, eYoonDir2DMode nMode)
        {
            return nMode switch
            {
                eYoonDir2DMode.Fixed => e,
                eYoonDir2DMode.Clock4 => e.PreviousQuadrant(),
                eYoonDir2DMode.AntiClock4 => e.NextQuadrant(),
                eYoonDir2DMode.Clock8 => e.PreviousOctant(),
                eYoonDir2DMode.AntiClock8 => e.NextOctant(),
                eYoonDir2DMode.Increase => e.NextOrder(),
                eYoonDir2DMode.Decrease => e.PreviousOrder(),
                eYoonDir2DMode.Whirlpool => e.NextWhirlpool(),
                eYoonDir2DMode.AntiWhirlpool => e.PreviousWhirlpool(),
                eYoonDir2DMode.AxisX => e.ReverseY(),
                eYoonDir2DMode.AxisY => e.ReverseX(),
                _ => eYoonDir2D.None
            };
        }

        public static eYoonDir2D NextQuadrant(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.Top => eYoonDir2D.Left,
                eYoonDir2D.Left => eYoonDir2D.Bottom,
                eYoonDir2D.Bottom => eYoonDir2D.Right,
                eYoonDir2D.Right => eYoonDir2D.Top,
                eYoonDir2D.TopRight => eYoonDir2D.TopLeft,
                eYoonDir2D.TopLeft => eYoonDir2D.BottomLeft,
                eYoonDir2D.BottomLeft => eYoonDir2D.BottomRight,
                eYoonDir2D.BottomRight => eYoonDir2D.TopRight,
                _ => e
            };
        }

        public static eYoonDir2D PreviousQuadrant(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.Top => eYoonDir2D.Right,
                eYoonDir2D.Right => eYoonDir2D.Bottom,
                eYoonDir2D.Bottom => eYoonDir2D.Left,
                eYoonDir2D.Left => eYoonDir2D.Top,
                eYoonDir2D.TopRight => eYoonDir2D.BottomRight,
                eYoonDir2D.BottomRight => eYoonDir2D.BottomLeft,
                eYoonDir2D.BottomLeft => eYoonDir2D.TopLeft,
                eYoonDir2D.TopLeft => eYoonDir2D.TopRight,
                _ => e
            };
        }

        public static eYoonDir2D NextOctant(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.Right => eYoonDir2D.TopRight,
                eYoonDir2D.TopRight => eYoonDir2D.Top,
                eYoonDir2D.Top => eYoonDir2D.TopLeft,
                eYoonDir2D.TopLeft => eYoonDir2D.Left,
                eYoonDir2D.Left => eYoonDir2D.BottomLeft,
                eYoonDir2D.BottomLeft => eYoonDir2D.Bottom,
                eYoonDir2D.Bottom => eYoonDir2D.BottomRight,
                eYoonDir2D.BottomRight => eYoonDir2D.Right,
                _ => e
            };
        }

        public static eYoonDir2D PreviousOctant(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.Right => eYoonDir2D.BottomRight,
                eYoonDir2D.BottomRight => eYoonDir2D.Bottom,
                eYoonDir2D.Bottom => eYoonDir2D.BottomLeft,
                eYoonDir2D.BottomLeft => eYoonDir2D.Left,
                eYoonDir2D.Left => eYoonDir2D.TopLeft,
                eYoonDir2D.TopLeft => eYoonDir2D.Top,
                eYoonDir2D.Top => eYoonDir2D.TopRight,
                eYoonDir2D.TopRight => eYoonDir2D.Right,
                _ => e
            };
        }

        public static eYoonDir2D ReverseX(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.TopLeft => eYoonDir2D.BottomLeft,
                eYoonDir2D.Top => eYoonDir2D.Bottom,
                eYoonDir2D.TopRight => eYoonDir2D.BottomRight,
                eYoonDir2D.BottomLeft => eYoonDir2D.TopLeft,
                eYoonDir2D.Bottom => eYoonDir2D.Top,
                eYoonDir2D.BottomRight => eYoonDir2D.TopRight,
                _ => e
            };
        }

        public static eYoonDir2D ReverseY(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.TopLeft => eYoonDir2D.TopRight,
                eYoonDir2D.Left => eYoonDir2D.Right,
                eYoonDir2D.BottomLeft => eYoonDir2D.BottomRight,
                eYoonDir2D.TopRight => eYoonDir2D.TopLeft,
                eYoonDir2D.Right => eYoonDir2D.Left,
                eYoonDir2D.BottomRight => eYoonDir2D.BottomLeft,
                _ => e
            };
        }

        public static eYoonDir2D NextOrder(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.TopLeft => // 1
                    eYoonDir2D.Top,
                eYoonDir2D.Top => // 2
                    eYoonDir2D.TopRight,
                eYoonDir2D.TopRight => // 3
                    eYoonDir2D.Left,
                eYoonDir2D.Left => // 4
                    eYoonDir2D.Center,
                eYoonDir2D.Center => // 5
                    eYoonDir2D.Right,
                eYoonDir2D.Right => // 6
                    eYoonDir2D.BottomLeft,
                eYoonDir2D.BottomLeft => // 7
                    eYoonDir2D.Bottom,
                eYoonDir2D.Bottom => // 8
                    eYoonDir2D.BottomRight,
                eYoonDir2D.BottomRight => // 9
                    eYoonDir2D.TopLeft,
                _ => eYoonDir2D.None
            };
        }

        public static eYoonDir2D PreviousOrder(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.BottomRight => // 9
                    eYoonDir2D.Bottom,
                eYoonDir2D.Bottom => // 8
                    eYoonDir2D.BottomLeft,
                eYoonDir2D.BottomLeft => // 7
                    eYoonDir2D.Right,
                eYoonDir2D.Right => // 6
                    eYoonDir2D.Center,
                eYoonDir2D.Center => // 5
                    eYoonDir2D.Left,
                eYoonDir2D.Left => // 4
                    eYoonDir2D.TopRight,
                eYoonDir2D.TopRight => // 3
                    eYoonDir2D.Top,
                eYoonDir2D.Top => // 2
                    eYoonDir2D.TopLeft,
                eYoonDir2D.TopLeft => // 1
                    eYoonDir2D.BottomRight,
                _ => eYoonDir2D.None
            };
        }

        public static eYoonDir2D NextWhirlpool(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.TopLeft => eYoonDir2D.Top,
                eYoonDir2D.Top => eYoonDir2D.TopRight,
                eYoonDir2D.TopRight => eYoonDir2D.Right,
                eYoonDir2D.Right => eYoonDir2D.BottomRight,
                eYoonDir2D.BottomRight => eYoonDir2D.Bottom,
                eYoonDir2D.Bottom => eYoonDir2D.BottomLeft,
                eYoonDir2D.BottomLeft => eYoonDir2D.Left,
                eYoonDir2D.Left => eYoonDir2D.Center,
                eYoonDir2D.Center => eYoonDir2D.TopLeft,
                _ => eYoonDir2D.None
            };
        }

        public static eYoonDir2D PreviousWhirlpool(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.Center => eYoonDir2D.Left,
                eYoonDir2D.Left => eYoonDir2D.BottomLeft,
                eYoonDir2D.BottomLeft => eYoonDir2D.Bottom,
                eYoonDir2D.Bottom => eYoonDir2D.BottomRight,
                eYoonDir2D.BottomRight => eYoonDir2D.Right,
                eYoonDir2D.Right => eYoonDir2D.TopRight,
                eYoonDir2D.TopRight => eYoonDir2D.Top,
                eYoonDir2D.Top => eYoonDir2D.TopLeft,
                eYoonDir2D.TopLeft => eYoonDir2D.Center,
                _ => eYoonDir2D.None
            };
        }

        public static int ToOrder(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.TopLeft => 0,
                eYoonDir2D.Top => 1,
                eYoonDir2D.TopRight => 2,
                eYoonDir2D.Left => 3,
                eYoonDir2D.Center => 4,
                eYoonDir2D.Right => 5,
                eYoonDir2D.BottomLeft => 6,
                eYoonDir2D.Bottom => 7,
                eYoonDir2D.BottomRight => 8,
                _ => -1
            };
        }

        public static eYoonDir2D FromOrder(int nNum)
        {
            return nNum switch
            {
                0 => eYoonDir2D.TopLeft,
                1 => eYoonDir2D.Top,
                2 => eYoonDir2D.TopRight,
                3 => eYoonDir2D.Left,
                4 => eYoonDir2D.Center,
                5 => eYoonDir2D.Right,
                6 => eYoonDir2D.BottomLeft,
                7 => eYoonDir2D.Bottom,
                8 => eYoonDir2D.BottomRight,
                _ => eYoonDir2D.None
            };
        }

        public static int ToClock(this eYoonDir2D e)
        {
            return e switch
            {
                eYoonDir2D.Top => 0,
                eYoonDir2D.TopRight => 1,
                eYoonDir2D.Right => 3,
                eYoonDir2D.BottomRight => 5,
                eYoonDir2D.Bottom => 6,
                eYoonDir2D.BottomLeft => 7,
                eYoonDir2D.Left => 9,
                eYoonDir2D.TopLeft => 11,
                _ => -1
            };
        }

        public static eYoonDir2D FromClock(int nClock)
        {
            switch (nClock)
            {
                case 0:
                case 12:
                    return eYoonDir2D.Top;
                case 1:
                case 2:
                    return eYoonDir2D.TopRight;
                case 3:
                    return eYoonDir2D.Right;
                case 4:
                case 5:
                    return eYoonDir2D.BottomRight;
                case 6:
                    return eYoonDir2D.Bottom;
                case 7:
                case 8:
                    return eYoonDir2D.BottomLeft;
                case 9:
                    return eYoonDir2D.Left;
                case 10:
                case 11:
                    return eYoonDir2D.TopLeft;
                default:
                    return eYoonDir2D.None;
            }
        }

        public static int ToQuadrant(this eYoonDir2D e)
        {
            switch (e)
            {
                case eYoonDir2D.Center:
                    return 0;
                case eYoonDir2D.Right:
                case eYoonDir2D.TopRight:
                    return 1;
                case eYoonDir2D.Top:
                case eYoonDir2D.TopLeft:
                    return 2;
                case eYoonDir2D.Left:
                case eYoonDir2D.BottomLeft:
                    return 3;
                case eYoonDir2D.Bottom:
                case eYoonDir2D.BottomRight:
                    return 4;
                default:
                    return -1;
            }
        }

        public static eYoonDir2D FromQuadrant(int nNum)
        {
            return nNum switch
            {
                0 => eYoonDir2D.Center,
                1 => eYoonDir2D.TopRight,
                2 => eYoonDir2D.TopLeft,
                3 => eYoonDir2D.BottomLeft,
                4 => eYoonDir2D.BottomRight,
                _ => eYoonDir2D.None
            };
        }

        public static eYoonDir2D[] GetClockDirections()
        {
            return new eYoonDir2D[8]
            {
                eYoonDir2D.Right, eYoonDir2D.TopRight, eYoonDir2D.Top, eYoonDir2D.TopLeft, eYoonDir2D.Left,
                eYoonDir2D.BottomLeft, eYoonDir2D.Bottom, eYoonDir2D.BottomRight
            };
        }

        public static eYoonDir2D[] GetSquareDirections()
        {
            return new eYoonDir2D[4]
                {eYoonDir2D.TopRight, eYoonDir2D.TopLeft, eYoonDir2D.BottomLeft, eYoonDir2D.BottomRight};
        }

        public static eYoonDir2D[] GetRhombusDirections()
        {
            return new eYoonDir2D[4] {eYoonDir2D.Top, eYoonDir2D.Left, eYoonDir2D.Bottom, eYoonDir2D.Right};
        }

        public static eYoonDir2D[] GetHorizonDirections()
        {
            return new eYoonDir2D[2] {eYoonDir2D.Left, eYoonDir2D.Right};
        }

        public static eYoonDir2D[] GetVerticalDirections()
        {
            return new eYoonDir2D[2] {eYoonDir2D.Top, eYoonDir2D.Bottom};
        }
    }
}