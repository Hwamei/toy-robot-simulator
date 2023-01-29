using ToyRobotSimulator.Core.Interfaces;

namespace ToyRobotSimulator.Core.UnitTests
{
    public class RobotProcessorTests
    {
        private const string InvalidCommandText = "INVALID COMMAND";

        private readonly ICommandParser _commandParser;
        private readonly ICommandExecutor _commandExecutor;

        public RobotProcessorTests()
        {
            _commandParser = new CommandParser();
            _commandExecutor = new CommandExecutor();
        }

        /// <summary>
        /// Test invalid commands
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("ABC 123")]
        [InlineData("PLACER 1,2,EAST MOVE LEFT MOVE P LACE 3,1 MOVE REPORT")]
        [InlineData("PLACE 1,2 MOVE LEFT MOVE PLACE 3,1 MOVE REPORT")]
        [InlineData(" PLACE 1,2 MOVE LEFT REPORT MOVE PLACE 3,1 MOVE REPORT ")]
        [InlineData("  PPLACE R 1,2,EAST MOVE LEFT MOVE P LACE 3,1 MOVE REPORT   ")]
        [InlineData("PLACE PPLACE R 1,2,EAST MOVE LEFT MOVE P LACE 3,1 MOVE REPORT")]
        [InlineData("PLACE 1,2 MOVE LEFT MOVE P LACE 3,1 MOVE REPORT")]
        public void RobotProcessor_InvalidCommand_ReturnInvalidCommandResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Empty(robotCommands);

            Assert.True(messages.All(m => m == "INVALID COMMAND"));
        }

        /// <summary>
        /// Test commands without any REPORT command
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 0,0,EAST")]
        [InlineData("PLACE 0,0,WEST left")]
        [InlineData("REPORT PLACE 0,0,EAST")]
        public void RobotProcessor_InvalidCommand_NoReport_ReturnInvalidCommandResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.True(messages.All(m => m == InvalidCommandText));
        }

        /// <summary>
        /// Test placement outside of the table
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 7,1,NORTH\r\nLEFT\r\nREPORT")]
        [InlineData("PLACE 0,6,NORTH LEFT REPORT")]
        [InlineData("  PLACE -1,0,NORTH  left  RePORT     ")]
        public void RobotProcessor_InvalidParam_PlaceOutside_ReturnInvalidCommandResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Empty(robotCommands);

            Assert.True(messages.All(m => m == InvalidCommandText));
        }



        /// <summary>
        /// Test robot movement - Example a
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 0,0,NORTH\r\nMOVE\r\nREPORT")]
        [InlineData("PLACE 0,0,NORTH MOVE REPORT")]
        [InlineData("  PLACE 0,0,NORTH  MOVE  REPORT     ")]
        public void RobotProcessor_ValidCommand_Move_ReturnCorrectResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Equal(3, robotCommands.Count);
            Assert.Equal(RobotActionEnum.Place, robotCommands.Select(c => c.Action).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Report, robotCommands.Select(c => c.Action).LastOrDefault());

            Assert.Equal("0,1,NORTH", messages.SingleOrDefault());
        }

        /// <summary>
        /// Test robot movement - Example b
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 0,0,NORTH\r\nLEFT\r\nREPORT")]
        [InlineData("PLACE 0,0,NORTH LEFT REPORT")]
        [InlineData("  PLACE 0,0,NORTH  left  RePORT     ")]
        public void RobotProcessor_ValidCommand_Left_ReturnCorrectResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Equal(3, robotCommands.Count);
            Assert.Equal(0, robotCommands.Select(c => c.X).FirstOrDefault());
            Assert.Equal(0, robotCommands.Select(c => c.Y).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Place, robotCommands.Select(c => c.Action).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Report, robotCommands.Select(c => c.Action).LastOrDefault());

            Assert.Equal("0,0,WEST", messages.SingleOrDefault());
        }

        /// <summary>
        /// Test robot movement - Example c
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 1,2,EAST\r\nMOVE\r\nMOVE\r\nLEFT\r\nMOVE\r\nREPORT")]
        [InlineData("PLACE 1,2,EAST MOVE MOVE LEFT MOVE REPORT")]
        [InlineData("  PLACE  1,2,EAST  MOVE MOVE    LEFT MOVE    REPORT     ")]
        public void RobotProcessor_ValidCommand_Move_Left_ReturnCorrectResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Equal(6, robotCommands.Count);
            Assert.Equal(1, robotCommands.Select(c => c.X).FirstOrDefault());
            Assert.Equal(2, robotCommands.Select(c => c.Y).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Place, robotCommands.Select(c => c.Action).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Left, robotCommands[3].Action);

            Assert.Equal("3,3,NORTH", messages.SingleOrDefault());
        }

        /// <summary>
        /// Test re-placement without direction - Example d
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 1,2,EAST\r\nMOVE\r\nLEFT\r\nMOVE\r\nPLACE 3,1\r\nMOVE\r\nREPORT")]
        [InlineData("PLACE 1,2,EAST MOVE LEFT MOVE PLACE 3,1 MOVE REPORT")]
        [InlineData("  PLACE 1,2,EAST   MOVE  LEFT    MOVE  PLACE 3,1 MOVE REPORT")]
        public void RobotProcessor_ValidCommand_MultiplePlace_ReturnCorrectResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Equal(7, robotCommands.Count);
            Assert.Equal(1, robotCommands.Select(c => c.X).FirstOrDefault());
            Assert.Equal(2, robotCommands.Select(c => c.Y).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Place, robotCommands.Select(c => c.Action).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Left, robotCommands[2].Action);
            Assert.Equal(RobotActionEnum.Report, robotCommands.Select(c => c.Action).LastOrDefault());
            Assert.Equal(3, robotCommands.Count(c => c.Action == RobotActionEnum.Move));

            Assert.Equal("3,2,NORTH", messages.SingleOrDefault());
        }

        /// <summary>
        /// Test robot movement
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 1,2,EAST\r\nMOVE\r\nMOVE\r\nLEFT\r\nMOVE\r\nRIGHT\r\nRIGHT\r\nRIGHT\r\nRIGHT\r\nRIGHT\r\nREPORT")]
        [InlineData("PLACE 1,2,EAST MOVE MOVE LEFT MOVE right right right right right REPORT")]
        [InlineData("  PLACE  1,2,EAST  MOVE MOVE    LEFT MOVE   Right Right Right Right Right REPORT     ")]
        public void RobotProcessor_ValidCommand_Move_Left_Right_ReturnCorrectResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Equal(11, robotCommands.Count);
            Assert.Equal(1, robotCommands.Select(c => c.X).FirstOrDefault());
            Assert.Equal(2, robotCommands.Select(c => c.Y).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Place, robotCommands.Select(c => c.Action).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Left, robotCommands[3].Action);
            Assert.Equal(5, robotCommands.Count(c => c.Action == RobotActionEnum.Right));

            Assert.Equal("3,3,EAST", messages.SingleOrDefault());
        }

        /// <summary>
        /// Test re-placement with direction
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 1,2,EAST\r\nMOVE\r\nLEFT\r\nMOVE\r\nPLACE 3,1,WEST\r\nMOVE\r\nREPORT")]
        [InlineData("PLACE 1,2,EAST MOVE LEFT MOVE PLACE 3,1,WEST MOVE REPORT")]
        [InlineData("  PLACE 1,2,EAST   MOVE  LEFT    MOVE  PLACE 3,1,west MOVE REPORT")]
        public void RobotProcessor_ValidCommand_MultiplePlace_MultipleDirection_ReturnCorrectResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Equal(7, robotCommands.Count);
            Assert.Equal(1, robotCommands.Select(c => c.X).FirstOrDefault());
            Assert.Equal(2, robotCommands.Select(c => c.Y).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Place, robotCommands.Select(c => c.Action).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Left, robotCommands[2].Action);
            Assert.Equal(RobotActionEnum.Report, robotCommands.Select(c => c.Action).LastOrDefault());
            Assert.Equal(3, robotCommands.Count(c => c.Action == RobotActionEnum.Move));

            Assert.Equal("2,1,WEST", messages.SingleOrDefault());
        }

        /// <summary>
        /// Test multiple report commands
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 2,2,EAST\r\nMOVE\r\nLEFT\r\nMOVE\r\nREPORT\r\nPLACE 3,3\r\nMOVE\r\nREPORT")]
        [InlineData("PLACE 2,2,EAST MOVE LEFT MOVE report PLACE 3,3 MOVE REPORT")]
        [InlineData("  PLACE 2,2,EAST   MOVE  LEFT    MOVE  report PLACE 3,3 MOVE REPORT")]
        public void RobotProcessor_ValidCommand_MultiplePlace_MultipleReport_ReturnCorrectResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Equal(8, robotCommands.Count);
            Assert.Equal(2, robotCommands.Select(c => c.X).FirstOrDefault());
            Assert.Equal(2, robotCommands.Select(c => c.Y).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Place, robotCommands.Select(c => c.Action).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Left, robotCommands[2].Action);
            Assert.Equal(RobotActionEnum.Report, robotCommands.Select(c => c.Action).LastOrDefault());
            Assert.Equal(3, robotCommands.Count(c => c.Action == RobotActionEnum.Move));
            Assert.Equal(2, robotCommands.Count(c => c.Action == RobotActionEnum.Report));

            Assert.Equal("3,3,NORTH", messages.FirstOrDefault());
            Assert.Equal("3,4,NORTH", messages.LastOrDefault());
        }

        /// <summary>
        /// Test prevent destruction, further commands are allowed
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE 5,5,EAST\r\nMOVE\r\nright\r\nRighT\r\nMOVE\r\nREPORT ")]
        [InlineData("PLACE 5,5,EAST MOVE RIGHT RIGHT MOVE REPORT")]
        [InlineData("  PLACE 5,5,EAST   MOVE  right Right    moVe   REPORT e..")]
        public void RobotProcessor_ValidCommand_PreventDestruction_ReturnCorrectResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Equal(6, robotCommands.Count);
            Assert.Equal(5, robotCommands.Select(c => c.X).FirstOrDefault());
            Assert.Equal(5, robotCommands.Select(c => c.Y).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Place, robotCommands.Select(c => c.Action).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Right, robotCommands[2].Action);
            Assert.Equal(RobotActionEnum.Report, robotCommands.Select(c => c.Action).LastOrDefault());
            Assert.Equal(2, robotCommands.Count(c => c.Action == RobotActionEnum.Move));

            Assert.Equal("4,5,WEST", messages.SingleOrDefault());
        }

        /// <summary>
        /// Test place input param with spaces
        /// </summary>
        /// <param name="command"></param>
        [Theory]
        [InlineData("PLACE  0 ,  0 ,EAST \r\nMOVE\r\nREPORT ,")]
        [InlineData("PLACE 0 ,0, EAST MOVE REPORT ,")]
        [InlineData("  PLACE  0  ,  0  ,   EAST   MOVE  REPORT  , ")]
        public void RobotProcessor_ValidCommand_PlaceParamWithSpaces_ReturnCorrectResult(string command)
        {
            var robotCommands = _commandParser.Parse(command);
            var messages = _commandExecutor.Execute(robotCommands);

            Assert.Equal(3, robotCommands.Count);
            Assert.Equal(0, robotCommands.Select(c => c.X).FirstOrDefault());
            Assert.Equal(0, robotCommands.Select(c => c.Y).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Place, robotCommands.Select(c => c.Action).FirstOrDefault());
            Assert.Equal(RobotActionEnum.Move, robotCommands[1].Action);
            Assert.Equal(RobotActionEnum.Report, robotCommands.Select(c => c.Action).LastOrDefault());
            Assert.Equal(1, robotCommands.Count(c => c.Action == RobotActionEnum.Move));

            Assert.Equal("1,0,EAST", messages.SingleOrDefault());
        }
    }
}