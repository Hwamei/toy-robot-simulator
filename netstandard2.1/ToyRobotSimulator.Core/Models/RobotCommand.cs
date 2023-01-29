using ToyRobotSimulator.Core.Enums;

namespace ToyRobotSimulator.Core.Models
{
    public class RobotCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public DirectionEnum Direction { get; set; }
        public RobotActionEnum Action { get; set; }

        public RobotCommand()
        {
        }

        public RobotCommand(RobotActionEnum action)
        {
            this.Action = action;
        }
    }
}
