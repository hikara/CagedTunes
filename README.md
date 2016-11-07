# CagedTunes
htunes but with more Nick Cage

John Thompson and Jared Nesbit
		50%				50%

Both members contributed an equal amount of labor. We pair 
programmed for a good amount of this project, although John
implemented the playlistListView delete/rename functions, and Jared
implemented the details menu HTTP/XAML functionality.

https://github.com/hikara/CagedTunes

The only known bug in this project is that whenever the user
tries to edit a row of the datagrid, the drag/drop functions
would kick in and automatically display the song ID in the text
field. Furthermore, if the user tried to highlight the text in
the text field, an exception would be thrown. We tried for an
hour or so to fix this bug, but we couldn't figure out how. We
ultimately decided to disable the ability to edit datagrid rows,
even though the functionality for that could be done by simply
calling our musicLib.save() function after a row was edited.
