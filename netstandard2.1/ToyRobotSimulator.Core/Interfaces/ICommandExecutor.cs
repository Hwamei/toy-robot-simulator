using System.Collections.Generic;
using ToyRobotSimulator.Core.Models;

namespace ToyRobotSimulator.Core.Interfaces
{
    public interface ICommandExecutor
    {
        List<string> Execute(List<RobotCommand> commands);
    }
}
