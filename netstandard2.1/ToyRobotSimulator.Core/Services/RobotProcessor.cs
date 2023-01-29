using System;
using System.Collections.Generic;
using ToyRobotSimulator.Core.Interfaces;

namespace ToyRobotSimulator.Core.Services
{
    public class RobotProcessor
    {
        private readonly ICommandParser _commandParser;
        private readonly ICommandExecutor _commandExecutor;

        public RobotProcessor(ICommandParser commandParser, ICommandExecutor commandExecutor)
        {
            _commandParser = commandParser;
            _commandExecutor = commandExecutor;
        }

        public List<string> Run(string command)
        {
            List<string> messages;

            try
            {
                var robotCommands = _commandParser.Parse(command);
                messages = _commandExecutor.Execute(robotCommands);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return messages;
        }
    }
}
