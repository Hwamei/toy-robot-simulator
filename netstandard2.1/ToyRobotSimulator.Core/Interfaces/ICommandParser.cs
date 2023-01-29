using System.Collections.Generic;
using ToyRobotSimulator.Core.Models;

namespace ToyRobotSimulator.Core.Interfaces
{
    public interface ICommandParser
    {
        List<RobotCommand> Parse(string command);
    }
}
