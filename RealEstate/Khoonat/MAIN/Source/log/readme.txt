
This project is only placed in the solution to facilitate easy access to log4net log files
from within the Visual Studio environment.

This project should not contain any code, should not reference any assemblies or projects,
and none of the projects should reference this project. The output should not be included
in the solution deployment image.

Do NOT include log files in the project. If done so, the log files are added and committed
to the source control repository. Instead, when you need to access the log files, make sure
"Show All Files" option is turned on for this project, and open files directly without
adding them to the project.
