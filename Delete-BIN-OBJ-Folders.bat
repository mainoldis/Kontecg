@ECHO off
cls

ECHO Deleting all BIN, OBJ and OUTPUT folders...
ECHO.

FOR /d /r . %%d in (bin,obj,output) DO (
	IF EXIST "%%d" (		 	 
		ECHO %%d | FIND /I "\node_modules\" > Nul && ( 
			ECHO.Skipping: %%d
		) || (
			ECHO.Deleting: %%d
			rd /s/q "%%d"
		)
	)
)

ECHO.
ECHO.BIN, OBJ and OUTPUT folders have been successfully deleted. Press any key to exit.
pause > nul