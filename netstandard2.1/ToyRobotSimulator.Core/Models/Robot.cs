using ToyRobotSimulator.Core.Enums;

namespace ToyRobotSimulator.Core.Models
{
    internal class Robot
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public DirectionEnum Direction { get; private set; }

        internal void Place(int x, int y, DirectionEnum direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }

        internal DirectionEnum Left(DirectionEnum currentDirection)
        {
            return currentDirection switch
            {
                DirectionEnum.NORTH => Direction = DirectionEnum.WEST,
                DirectionEnum.EAST => Direction = DirectionEnum.NORTH,
                DirectionEnum.SOUTH => Direction = DirectionEnum.EAST,
                DirectionEnum.WEST => Direction = DirectionEnum.SOUTH,
                _ => 0
            };
        }

        internal DirectionEnum Right(DirectionEnum currentDirection)
        {
            return currentDirection switch
            {
                DirectionEnum.NORTH => Direction = DirectionEnum.EAST,
                DirectionEnum.EAST => Direction = DirectionEnum.SOUTH,
                DirectionEnum.SOUTH => Direction = DirectionEnum.WEST,
                DirectionEnum.WEST => Direction = DirectionEnum.NORTH,
                _ => 0
            };
        }

        internal void Move(DirectionEnum currentDirection, int x, int y)
        {
            switch (currentDirection)
            {
                case DirectionEnum.NORTH:
                    if (y < AppConstants.MaxY)
                        Y = y + 1;
                    break;
                case DirectionEnum.EAST:
                    if (x < AppConstants.MaxX)
                        X = x + 1;
                    break;
                case DirectionEnum.SOUTH:
                    if (y > 0)
                        Y = y - 1;
                    break;
                case DirectionEnum.WEST:
                    if (x > 0)
                        X = x - 1;
                    break;
                default:
                    break;
            }
        }
    }
}
