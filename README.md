# Diablo 2 Resurrected – Automatic Online Run Counter

This is a lightweight, automatic item-find run counter designed specifically for **ONLINE** characters in Diablo 2 Resurrected. 

<img width="1194" height="360" alt="image" src="https://github.com/user-attachments/assets/d26f3afd-e467-4f51-afe2-e5fdf46d7594" />

### ⏱️ Fully Automated Session & Run Tracking
* **Zero Manual Effort:** The tool automatically detects your game's start and end triggers (Save & Exit) without you having to press a single button.
* **Smart Session Analytics:** It tracks your total session time, measures individual run durations, calculates your overall average speed, and maintains a detailed history of all your runs.
* **Anti-Spam Protection:** By design, the counter ignores runs shorter than 15 seconds. This built-in safeguard prevents false counts when you are rapidly opening and closing games to fish for specific Terror Zone (TZ) prefixes.
* **Manual Run Adjustments:** Need to tweak your stats? While the timer operates automatically, users have full control to manually adjust or modify the total run count at any time. *(Note: Run durations have a 1-second resolution and cannot be manually edited).*
* **Loot Logging:** You can flag any successful run as "Found" to keep precise statistical track of your Holy Grail or specific item hunts.

### 🔒 100% Passive & Safe (Zero File Corruption Risk)
We understand how important your game data is. This tool is engineered to be entirely passive and non-intrusive:
* **No File Access or Modification:** The tool **never opens, reads, or writes to** your Diablo II files. There is absolutely **zero risk of file corruption**.
* **How it works:** It simply scans the directory every few seconds to check the *last modification date* of `settings.json` and `bzx-log.txt`. It also peeks at the timestamp of the `.ctlo` file (the keyboard control definitions file of your last-played character) solely to grab your character's name. 
* **Performance-Friendly:** Because it only checks metadata (timestamps) rather than opening the files themselves, it consumes virtually zero system resources. You won't even notice it's running in the background.
