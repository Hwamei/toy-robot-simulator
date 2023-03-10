# Overview
Toy Robot Simulator library implementation with C# based on .Net Standard 2.1

- **ToyRobotSimulator.Core**
The poject contains the code for the library

- **ToyRobotSimulator.UnitTests**
The project contains the tests for the library

- **ToyRobotSimulator.ConsoleApp**
The project is a demo UI

# Getting Started
1. Set ToyRobotSimulator.ConsoleApp as startup project
2. Build the solution
3. Run the tests
4. Run the console app

To send a command to the toy robot simulator
1. Instanciate the RobotProcessor class with new CommandParser and CommandExecutor instances as the input parameters
2. Run the Run method with a string type command as input parameter

    e.g.

    `var processor = new RobotProcessor(new CommandParser(), new CommandExecutor());`

    `var messages = processor.Run(command);`

# Requirements & Functionalities

Toy Robot Simulator

Create a library that can read in commands of the following form:

PLACE X,Y,DIRECTION
MOVE
LEFT
RIGHT
REPORT

- The library allows for a simulation of a toy robot moving on a 6 x 6 square tabletop.
- There are no obstructions on the table surface.
- The robot is free to roam around the surface of the table, but must be prevented from falling to destruction. Any movement that would result in this must be prevented, however further valid movement commands must still be allowed.
- PLACE will put the toy robot on the table in position X,Y and facing NORTH, SOUTH, EAST or WEST.
- (0,0) can be considered as the SOUTH WEST corner and (5,5) as the NORTH EAST corner.
- The first valid command to the robot is a PLACE command. After that, any sequence of commands may be issued, in any order, including another PLACE command. The library should discard all commands in the sequence until a valid PLACE command has been executed.
- The PLACE command should be discarded if it places the robot outside of the table surface.
- Once the robot is on the table, subsequent PLACE commands could leave out the direction and only provide the coordinates. When this happens, the robot moves to the new coordinates without changing the direction.
- MOVE will move the toy robot one unit forward in the direction it is currently facing.
- LEFT and RIGHT will rotate the robot 90 degrees in the specified direction without changing the position of the robot.
- REPORT will announce the X,Y and orientation of the robot.
- A robot that is not on the table can choose to ignore the MOVE, LEFT, RIGHT and REPORT commands.
- The library should discard all invalid commands and parameters.

Example Input and Output:

a)
PLACE 0,0,NORTH
MOVE
REPORT
Output: 0,1,NORTH

b)
PLACE 0,0,NORTH
LEFT
REPORT
Output: 0,0,WEST

c)
PLACE 1,2,EAST
MOVE
MOVE
LEFT
MOVE
REPORT
Output: 3,3,NORTH

d)
PLACE 1,2,EAST
MOVE
LEFT
MOVE
PLACE 3,1
MOVE
REPORT
Output: 3,2,NORTH


- Use your preferred language, platform and IDE to implement this solution.
- Your solution should be clean and easy to read, maintain and execute.
- You should provide build scripts or instructions to build and run the solution.
- There should be a user interface to run the solution and assess that it works correctly. This could be a command prompt interface that takes one string command in at a time.
- The code should be original and you may not use any external libraries or open source code to solve this problem, but you may use external libraries or tools for building or testing purposes.

# Common Mistakes
- The direction paramter of the PLACE command should be optional after the first use.
- Failed to ignore the direction for subsequent PLACE commands - PLACE X,Y is not accepted as a valid command when the robot is already on the table
- Application does not handle spaces in between the PLACE command input parameters
