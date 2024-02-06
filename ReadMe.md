# SyncMeApp
SyncMeApp is an application for one-way synchronization of two folders. The application will turn the target folder into an exact copy of the source folder (recursively). Synchronization can be done repeatedly using the application timer.

## Usage
Enter parameters in the format parameter1=value parameter2=value ... (parameters are split by spaces, no spaces around '=' sign or '-' sign ahead of the parameter).
* Required parameters:
- **sourcedirectory** --> Path to the source folder ( Wrap the path in doublequotes "". If path ends with backslash, it needs to be escaped -> "\\")
- **replicadirectory** --> Path to the replica folder ( Wrap the path in doublequotes "". If path ends with backslash, it needs to be escaped -> "\\". Will be created if it doesn't exist)
- **logfile** --> Path to the log file (will be created if it doesn't exist)
* Optional parameters:
- *interval* --> Timer repeat period (0 (default) will turn off the timer)
- *timeunit* --> Unit of the interval (supported values: ms (default), s, min, h, d)
- *fileloglevel* --> Minimal level of file log level (Logs this level and higher. Supported values: debug, info, warn (default), error, fatal, off (turns off logging to a file)) 
- *consoleloglevel* --> Minimal level of console log level (Logs this level and higher. Supported values: debug, info (default), warn, error, fatal, off (turns off logging to a file)) 

**Example:**
*Minimal configuration:* SyncMeApp.exe sourcedirectory="d:\TestSource\\" replicadirectory="d:\TestReplica\\" logfile="d:\log\log.txt" 

*Full configuration:* SyncMeApp.exe sourcedirectory="d:\TestSource\\" replicadirectory="d:\TestReplica\\" logfile="d:\log\log.txt" interval=10 timeunit=min fileloglevel=info consoleloglevel=warn

 ## Warning
 - Any files in the replica folder may be deleted or overwritten!
 