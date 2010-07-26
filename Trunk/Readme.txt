In order to build the solution, you must first run at least once the Build.bat (or Build.ps1 from PowerShell) script.
This will generate an SNK key if you don't already have one named Tasq.snk. 

You can safely use your own key just naming the file Tasq.snk and placing it alongside Tasq.sln. The script will recognize the key file and extract the public key and update the generated InternalsVisibleTo public key to match, so that you can build and run the tests too.


