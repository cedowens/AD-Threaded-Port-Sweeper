# AD-Threaded-Port-Sweeper
C# Code to dump all AD computers and then quickly sweep for a given port.

C# code that dumps all AD computers and then uses threading to quickly check each computer tro see whether a specified port is open or not. Output is written to stdout. The compbiled binary takes a single command argument, which is the port you want to sweep for.

Example Usage:
**AD-PortSweeper.exe [port_num]**

This binary can also be executed remotely in memory using Cobalt Strike's execute assembly function.

Steps:
1. Build the solution file in Visual Studio (or build via command line)
2. Run the compiled binary from a domain joined windows machine as a domain user
3. The script will show a list of machines with that particular port open

This method (port sweeping) also is a method that often goes undetected since the traffic looks different than port scanning traffic (since you are just checking for one port across many hosts).


