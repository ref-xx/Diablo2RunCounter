This is a diablo 2 Ressurrected item find automatic run counter for ONLINE characters.

it detects game start and game end (save&exit) triggers by checking the date of two files:
settings.json and bzx-log.txt
it does not access those files, it only checks modification date of those files per couple of seconds (which you can change), so you won't feel it running. 
it also checks keyboard control definitions file of last accessed character: .ctlo file to grab the name of your character. 

The tool never accesses any file in your device. Just reads the directory and checks the file date to compare last modification time.

<img width="1202" height="364" alt="Diablo2runs" src="https://github.com/user-attachments/assets/ac1fdd1c-eb89-4c71-9251-921c4503ca00" />

it can calculate your total session time, measure each run duration and display an average duration. It keeps a record of every run you made.

You can also mark one run as "Found". It's for statistical purposes, if you after some special item, you can mark it as "found" per run.

You can adjust/modify run count (for any reason). 

d2runcounter is not designed to calculate precise duration. The duration cannot be manually adjusted. The Minimum time resolution is 1 seconds.

note. D2runCounter does not counts runs less then 15 seconds duration. it's by design. it's a counter protection scheme for when you are trying to find a special tz prefix by closing-opening new games.
