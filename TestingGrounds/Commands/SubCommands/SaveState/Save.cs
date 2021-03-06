namespace TestingGrounds.Commands.SubCommands.SaveState
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using System;
    using System.IO;
    using System.Linq;

    public class Save : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("tg.save"))
            {
                response = "Permission denied. Required: tg.save";
                return false;
            }

            if (arguments.Count > 2)
            {
                response =
                    "Usage: save (name) (delete / (force -> If a file with the same name should be overwritten))";
                return false;
            }

            string fileName = arguments.Count > 0 ? arguments.At(0) : "Save";
            string fullDirectory = Path.Combine(TestingGrounds.SaveStateDirectory, fileName);

            if (Directory.GetFiles(TestingGrounds.SaveStateDirectory).Any(file => file == fileName))
            {
                if (arguments.Count != 2)
                {
                    response = "A save already exists with that name!" +
                               "\nSpecify the \"force\" parameter to overwrite or \"delete\" parameter to remove.";
                    return false;
                }

                switch (arguments.At(1))
                {
                    case "d":
                    case "delete":
                        File.Delete(fullDirectory);
                        response = $"Save \"{fileName}\" deleted.";
                        return false;
                    case "f":
                    case "force":
                        sender.Respond("SAVE#Overwriting..");
                        break;
                    default:
                        response = "A save already exists with that name and an incorrect parameter was specified." +
                                   "\nSpecify the \"force\" parameter to overwrite or \"delete\" parameter to remove.";
                        return false;
                }
            }

            Methods.SaveState(fullDirectory);
            response = $"Game saved with the name \"{fileName}\"";
            return true;
        }

        public string Command => "save";
        public string[] Aliases => new string[0];
        public string Description => "Saves the state of the current round.";
    }
}