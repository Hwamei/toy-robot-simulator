using System;
using System.Collections.Generic;
using System.Linq;
using ToyRobotSimulator.Core.Enums;
using ToyRobotSimulator.Core.Interfaces;
using ToyRobotSimulator.Core.Models;

namespace ToyRobotSimulator.Core.Services
{
    public class CommandExecutor : ICommandExecutor
    {
        public List<string> Execute(List<RobotCommand> commands)
        {
            var messages = new List<string>();

            if (!commands.Any())
                return messages;

            var robot = new Robot();

            try
            {
                commands.ForEach(command =>
                {
                    switch (command.Action)
                    {
                        case RobotActionEnum.Place:
                            var direction = command.Direction > 0 ? command.Direction : robot.Direction;
                            robot.Place(command.X, command.Y, direction);
                            break;
                        case RobotActionEnum.Report:
                            messages.Add($"{robot.X},{robot.Y},{robot.Direction}");
                            break;
                        case RobotActionEnum.Move:
                            robot.Move(robot.Direction, robot.X, robot.Y);
                            break;
                        case RobotActionEnum.Left:
                            robot.Left(robot.Direction);
                            break;
                        case RobotActionEnum.Right:
                            robot.Right(robot.Direction);
                            break;
                        default:
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return messages;
        }
    }
}
