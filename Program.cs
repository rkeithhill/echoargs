Console.WriteLine("Command line:");
Console.WriteLine(Environment.CommandLine + Environment.NewLine);

if (args.Length == 0)
{
	Console.WriteLine("No arguments specified.");
	return;
}

for (int ndx = 0; ndx < args.Length; ndx++)
{
	Console.WriteLine($"Arg[{ndx}] is: {args[ndx]}");
}
