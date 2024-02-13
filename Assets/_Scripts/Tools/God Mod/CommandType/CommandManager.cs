public class CommandManager {
    protected CommandConsoleRuntime commandConsole;

    public CommandManager(CommandConsoleRuntime commandConsole) {
        this.commandConsole = commandConsole;
    }

    protected void AddCommand(CommandConsole command) {
        commandConsole.AddCommand(command);
    }
}