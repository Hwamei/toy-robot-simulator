using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ToyRobotSimulator.Core.Enums;
using ToyRobotSimulator.Core.Interfaces;
using ToyRobotSimulator.Core.Models;

namespace ToyRobotSimulator.Core.Services
{
    public class CommandParser : ICommandParser
    {
        public List<RobotCommand> Parse(string command)
        {
            var robotCommands = new List<RobotCommand>();

            if (string.IsNullOrWhiteSpace(command))
            {
                Console.WriteLine("Invalid command");
                return robotCommands;
            }

            try
            {
                command = command.Trim();
                command = command.Replace("\r", " ").Replace("\\r", " ").Replace("\n", " ").Replace("\\n", " ").ToUpper();
                command = RemoveSpacesInPlaceParameters(command, @"\d\s*,\s*\d");
                command = RemoveSpacesInPlaceParameters(command, @$"(\d\s*,\s*\b{DirectionEnum.NORTH}\b)|(\d\s*,\s*\b{DirectionEnum.EAST}\b)|(\d\s*,\s*\b{DirectionEnum.WEST}\b)|(\d\s*,\s*\b{DirectionEnum.SOUTH}\b)");

                var rawCommands = command.Split(" \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

                var isPlaced = false;
                for (int i = 0; i < rawCommands.Count; i++)
                {
                    var rawCommand = rawCommands[i];
                    if (isPlaced)
                    {
                        switch (rawCommand)
                        {
                            case AppConstants.Report:
                                robotCommands.Add(new RobotCommand(RobotActionEnum.Report));
                                break;
                            case AppConstants.Move:
                                robotCommands.Add(new RobotCommand(RobotActionEnum.Move));
                                break;
                            case AppConstants.Left:
                                robotCommands.Add(new RobotCommand(RobotActionEnum.Left));
                                break;
                            case AppConstants.Right:
                                robotCommands.Add(new RobotCommand(RobotActionEnum.Right));
                                break;
                            default:
                                break;
                        }
                    }

                    if (rawCommand != AppConstants.Place)
                        continue;

                    var isValid = ParsePlaceCommand(rawCommands, robotCommands, i, isPlaced);
                    if (!isValid)
                        continue;

                    isPlaced = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return robotCommands;
        }

        private static bool ParsePlaceCommand(List<string> rawCommands, List<RobotCommand> commands, int currentIndex, bool isPlaced)
        {
            var isValid = false;

            try
            {
                var nextIndex = currentIndex + 1;
                if (nextIndex >= rawCommands.Count)
                    return isValid;

                var data = rawCommands[nextIndex].Replace(" ", string.Empty);
                var dataList = data.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!isPlaced && dataList.Count != 3)
                    return isValid;

                bool xParseResult = int.TryParse(dataList[0], out int x);
                bool yParseResult = int.TryParse(dataList[1], out int y);
                DirectionEnum direction = 0;
                bool dParseResult = dataList.Count == 3 && Enum.TryParse(dataList[2], out direction);

                if (!xParseResult || !yParseResult || (!dParseResult && !isPlaced))
                    return isValid;
                if (x < 0 || x > AppConstants.MaxX || y < 0 || y > AppConstants.MaxY)
                    return isValid;

                commands.Add(new RobotCommand
                {
                    X = x,
                    Y = y,
                    Direction = direction,
                    Action = RobotActionEnum.Place
                });
                isValid = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isValid;
        }

        private static string RemoveSpacesInPlaceParameters(string command, string patternWithBoundaries)
        {
            var pattern = @"\s*,\s*";
            var matches = Regex.Matches(command, patternWithBoundaries).AsEnumerable();
            foreach (var match in matches.Reverse())
            {
                var index = match.Index;
                var length = match.Length;
                var value = Regex.Replace(match.Value, pattern, ",");
                command = command.Remove(index, length).Insert(index, value);
            }

            return command;
        }
    }
}
