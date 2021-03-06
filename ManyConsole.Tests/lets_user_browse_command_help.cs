﻿using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManyConsole.Tests
{

    public class lets_user_browse_command_help
    {
        [Test]
        public void TestCommandDescriptions()
        {
            var firstcommand = new TestCommand().IsCommand("command-a", "oneline description a");
            var secondCommand = new TestCommand().IsCommand("command-b", "oneline description b");

            var commands = new ConsoleCommand[]
            {
                firstcommand,
                secondCommand
            }.ToList();

            var writer = new StringWriter();

            WhenTheUserDoesNotSpecifyACommandThenShowAvailableCommands(commands, writer, firstcommand, secondCommand, new string[0]);
            WhenTheUserDoesNotSpecifyACommandThenShowAvailableCommands(commands, writer, firstcommand, secondCommand, new [] { "help"});

            ShouldShowHelpWhenRequested(commands, new string[] { "command-c", "/?" });
            ShouldShowHelpWhenRequested(commands, new string[] { "help", "command-c" });
        }

        private void WhenTheUserDoesNotSpecifyACommandThenShowAvailableCommands(List<ConsoleCommand> commands, StringWriter writer,
                                                                                ConsoleCommand firstcommand,
                                                                                ConsoleCommand secondCommand, string[] arguments)
        {
            // when the user does not specify a command
            ConsoleCommandDispatcher.DispatchCommand(commands, arguments, writer);

            // then the output contains a list of available commands
            var output = writer.ToString();

            MyStringAssert.ContainsInOrder(output, firstcommand.Command,
                    firstcommand.OneLineDescription,
                    secondCommand.Command,
                    secondCommand.OneLineDescription);
        }

        private void ShouldShowHelpWhenRequested(List<ConsoleCommand> commands, string[] consoleArguments)
        {
            var writer = new StringWriter();

            // when we call a command, asking for help
            var commandC = new TestCommand()
                .IsCommand("command-c", "one line description for C")
                .HasLongDescription(
@"Lorem ipsum dolor sit amet, consectetur adipiscing elit,
sed do eiusmod tempor incididunt ut labore et dolore magna
aliqua. Ut enim ad minim veniam, quis nostrud exercitation
ullamco laboris nisi ut aliquip ex ea commodo consequat.
Duis aute irure dolor in reprehenderit in voluptate velit
esse cillum dolore eu fugiat nulla pariatur. Excepteur sint
occaecat cupidatat non proident, sunt in culpa qui officia
deserunt mollit anim id est laborum.")
                .HasAdditionalArguments(0, "<remaining> <args>")
                .HasOption("o|option=", "option description", v => { });

            commands.Add(commandC);

            var exitCode = ConsoleCommandDispatcher.DispatchCommand(commands, consoleArguments, writer);

            // then the output contains a all help available for that command
            var output = writer.ToString();
            MyStringAssert.ContainsInOrder(output,
                commandC.Command,
                commandC.OneLineDescription,
                commandC.LongDescription,
                commandC.RemainingArgumentsHelpText,
                "-o",
                "--option",
                "option description");
                
            Assert.AreEqual(-1, exitCode);
        }
    }
}
