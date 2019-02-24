using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CommonLib;
using Microsoft.ConfigReaders;

namespace BlueprintMgmt
{
    class Program
    {

        private const string Create = "create";
        private const string Get = "get";
        private const string GetArtifacts = "get-artifacts";
        private const string GetArtifact = "get-artifact";
        private const string AddArtifact = "add-artifact";
        private const string Publish = "publish";
        private const string Assign = "assign";
        private const string Unassign = "unassign";
        private const string Delete = "delete";
        private const string Help = "help";

        private static string[] validCommands = new string[] { Create, Get, GetArtifacts, GetArtifact, AddArtifact, Publish, Assign, Unassign, Delete, Help };

        static void Main(string[] args)
        {
            try
            {
                var arguments = ParseArguments(args);

                ExecuteCommand(arguments);
            }
            catch(Exception ex)
            {
                Console.WriteLine("an error occurred while processing your request. Please review the error message: {0}", ex.Message);
            }
        }

        private static void ExecuteCommand(Arguments arguments)
        {
            var config = new AppSettingsConfigReader();
            var command = arguments.Command.ToLower();
            if(!validCommands.Contains(command))
            {
                throw new Exception($"Invalid command. Please use one of the following commands: {validCommands.Aggregate((res, item) => res + ", " + item)}");
            }
            if (!Help.Equals(command) && !Unassign.Equals(command))
            {
                if (!arguments.Parameters.ContainsKey("--name"))
                {
                    throw new KeyNotFoundException($"The command {arguments.Command} requires the blueprint name.");
                }
            }
            string blueprintName, assignmentName, artifactName;
            switch (command)
            {
                case Create:
                    blueprintName = arguments.Parameters["--name"];
                    CreateBlueprint(blueprintName, config);
                    break;
                case Get:
                    blueprintName = arguments.Parameters["--name"];
                    GetBlueprint(blueprintName, config);
                    break;
                case GetArtifacts:
                    blueprintName = arguments.Parameters["--name"];
                    GetBlueprintArtifacts(blueprintName, config);
                    break;
                case GetArtifact:
                    blueprintName = arguments.Parameters["--name"];
                    if (!arguments.Parameters.ContainsKey("--artifact-name"))
                    {
                        throw new KeyNotFoundException($"The command {arguments.Command} requires the artifact name.");
                    }
                    artifactName = arguments.Parameters["--artifact-name"];
                    GetBlueprintArtifact(blueprintName, artifactName, config);
                    break;
                case AddArtifact:
                    blueprintName = arguments.Parameters["--name"];
                    if (!arguments.Parameters.ContainsKey("--artifact-name"))
                    {
                        throw new KeyNotFoundException($"The command {arguments.Command} requires the artifact name.");
                    }
                    artifactName = arguments.Parameters["--artifact-name"];
                    AddBlueprintArtifact(blueprintName, artifactName, config);
                    break;
                case Publish:
                    blueprintName = arguments.Parameters["--name"];
                    if (!arguments.Parameters.ContainsKey("--version"))
                    {
                        throw new KeyNotFoundException($"The command {arguments.Command} requires the version.");
                    }
                    var version = arguments.Parameters["--version"];
                    PublishBlueprint(blueprintName, version, config);
                    break;
                case Assign:
                    blueprintName = arguments.Parameters["--name"];
                    if (!arguments.Parameters.ContainsKey("--assignment-name"))
                    {
                        throw new KeyNotFoundException($"The command {arguments.Command} requires the assignment name.");
                    }
                    assignmentName = arguments.Parameters["--assignment-name"];
                    AssignBlueprint(blueprintName, assignmentName, config);
                    break;
                case Unassign:
                    if (!arguments.Parameters.ContainsKey("--assignment-name"))
                    {
                        throw new KeyNotFoundException($"The command {arguments.Command} requires the assignment name.");
                    }
                    assignmentName = arguments.Parameters["--assignment-name"];
                    UnassignBlueprint(assignmentName, config);
                    break;
                case Delete:
                    blueprintName = arguments.Parameters["--name"];
                    DeleteBlueprint(blueprintName, config);
                    break;
                case Help:
                default:
                    ShowHelp(config);
                    break;
            }
        }

        private static Arguments ParseArguments(string[] args)
        {

            var arguments = new Arguments();
            if (args.Length == 0)
                arguments.Command = Help;
            else
                arguments.Command = args[0].ToLower();
            int i = 1;
            while (i < args.Length)
            {
                if (args[i].StartsWith("--"))
                {
                    if (i <= args.Length - 2 && !args[i + 1].StartsWith("--"))
                    {
                        arguments.Parameters[args[i].ToLower()] = args[i + 1];
                        i += 2;
                    }
                    else
                    {
                        arguments.Parameters[args[i].ToLower()] = "1";
                        i++;
                    }
                }
                else
                {
                    i++;
                }
            }
            return arguments;
        }

        private static void CreateBlueprint(string blueprintName, IConfigReader config)
        {
            var mgr = new BlueprintMgr(config);
            var filePath = Path.Combine(config["BlueprintsDir"], blueprintName, $"{blueprintName}.json");
            var data = File.ReadAllText(filePath);
            var result = mgr.CreateBlueprint(blueprintName, data).Result;
            Console.WriteLine(result);
            var blueprintDir = Path.Combine(config["BlueprintsDir"], blueprintName);
            var files = Directory.GetFiles(blueprintDir, "*.json");
            foreach(var artifactFilePath in files)
            {
                var artifactName = Path.GetFileNameWithoutExtension(artifactFilePath);
                if (!artifactName.Equals(blueprintName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var artifactFileName = Path.GetFileName(artifactFilePath);
                    data = File.ReadAllText(artifactFilePath);
                    result = mgr.AddArtifact(blueprintName, artifactName, data).Result;
                    Console.WriteLine(result);
                }
            }
        }
        private static void AddBlueprintArtifact(string blueprintName, string artifactName, IConfigReader config)
        {
            var mgr = new BlueprintMgr(config);
            var filePath = Path.Combine(config["BlueprintsDir"], blueprintName, $"{artifactName}.json");
            var data = File.ReadAllText(filePath);
            var result = mgr.AddArtifact(blueprintName, artifactName, data).Result;

            Console.WriteLine(result);
        }

        private static void GetBlueprint(string blueprintName, IConfigReader config)
        {
            var mgr = new BlueprintMgr(config);
            var result = mgr.Get(blueprintName).Result;

            Console.WriteLine(result);
        }

        private static void GetBlueprintArtifacts(string blueprintName, IConfigReader config)
        {
            var mgr = new BlueprintMgr(config);
            var result = mgr.GetArtifacts(blueprintName).Result;

            Console.WriteLine(result);
        }

        private static void GetBlueprintArtifact(string blueprintName, string artifactName, IConfigReader config)
        {
            var mgr = new BlueprintMgr(config);
            var result = mgr.GetArtifact(blueprintName, artifactName).Result;

            Console.WriteLine(result);
        }

        private static void PublishBlueprint(string blueprintName, string version, IConfigReader config)
        {
            var mgr = new BlueprintMgr(config);
            var result = mgr.Publish(blueprintName, version).Result;

            Console.WriteLine(result);
        }

        private static void AssignBlueprint(string blueprintName, string assignmentName, IConfigReader config)
        {
            var mgr = new BlueprintMgr(config);
            var filePath = Path.Combine(config["AssignmentsDir"], blueprintName, $"{blueprintName}.json");
            var data = File.ReadAllText(filePath);
            var result = mgr.Assign(blueprintName, assignmentName, data).Result;

            Console.WriteLine(result);
        }

        private static void UnassignBlueprint(string assignmentName, IConfigReader config)
        {
            var mgr = new BlueprintMgr(config);
            var result = mgr.Unassign(assignmentName).Result;

            Console.WriteLine(result);
        }

        private static void DeleteBlueprint(string blueprintName, IConfigReader config)
        {
            var mgr = new BlueprintMgr(config);
            var result = mgr.Delete(blueprintName).Result;

            Console.WriteLine(result);
        }

        private static void ShowHelp(IConfigReader config)
        {
            var filePath = Path.Combine(config["HelpDir"], "help.md");
            var helpText = File.ReadAllText(filePath);
            Console.WriteLine(helpText);
        }

    }

}
