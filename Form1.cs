using System.Media;
using System.Text.Json;

namespace ArdasD2RunCounter
{
    public partial class Form1 : Form
    {
        private static readonly TimeSpan DuplicateChangeWindow = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan StartDetectionCooldown = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan StartDetectionTimeout = TimeSpan.FromSeconds(30);
        private static readonly Color NormalCounterColor = Color.Black;
        private static readonly Color PausedCounterColor = Color.DimGray;
        private static readonly Color FoundCounterColor = Color.Goldenrod;

        private readonly Dictionary<string, DateTime> lastWriteTimes = new(StringComparer.OrdinalIgnoreCase);
        private readonly string runsFilePath = Path.Combine(Application.StartupPath, "runs.json");
        private RunDatabase database = new();
        private string? watchFolder;
        private string? blzLogFilePath;
        private DateTime? lastBlzLogWriteTime;
        private RunSession? activeRun;
        private DateTime? currentTurnStartedAt;
        private DateTime? lastRunDetectionTime;
        private DateTime? lastCompletedRunTime;
        private StartDetectionState startDetectionState = StartDetectionState.Idle;
        private DateTime? startDetectionAvailableAt;
        private DateTime? startDetectionExpiresAt;
        private bool missingFolderWarningShown;
        private bool missingBlzLogWarningShown;
        private bool pendingFoundForCurrentRun;
        private string? currentCharacter;

        public Form1()
        {
            InitializeComponent();
            ConfigureUi();
            LoadRuns();
            ApplyOptionsToUi();
            ResolveWatchedPaths();
            LoadInitialSnapshot();
            ApplyTimerInterval();
            RefreshAll();
            timer1.Start();
        }

        private void ConfigureUi()
        {
            listBox2.ContextMenuStrip = contextMenuStrip1;
            listBox2.SelectedIndexChanged += listBox2_SelectedIndexChanged;
            listBox2.MouseDown += listBox2_MouseDown;

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("#", 45);
            listView1.Columns.Add("Started", 125);
            listView1.Columns.Add("Finished", 125);
            listView1.Columns.Add("Character", 90);
            listView1.Columns.Add("Found", 55);
            listView1.Columns.Add("Files", 260);

            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button4.Click += button4_Click;
            button5.Click += button5_Click;
            button6.Click += button6_Click;
            button7.Click += button7_Click;
            button8.Click += button8_Click;

            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            renameToolStripMenuItem.Click += renameToolStripMenuItem_Click;
            duplicateToolStripMenuItem.Click += duplicateToolStripMenuItem_Click;
            modifyCountToolStripMenuItem.Click += modifyCountToolStripMenuItem_Click;
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;

            beepOnEndToolStripMenuItem.CheckOnClick = true;
            beepOnStartToolStripMenuItem.CheckOnClick = true;
            beepOnEndToolStripMenuItem.Click += beepOnEndToolStripMenuItem_Click;
            beepOnStartToolStripMenuItem.Click += beepOnStartToolStripMenuItem_Click;
            setInterval1500ToolStripMenuItem.Click += setInterval1500ToolStripMenuItem_Click;
        }

        private void LoadRuns()
        {
            if (!File.Exists(runsFilePath))
            {
                return;
            }

            try
            {
                var json = File.ReadAllText(runsFilePath);
                database = JsonSerializer.Deserialize<RunDatabase>(json) ?? new RunDatabase();
            }
            catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
            {
                AddLog($"runs.json could not be read: {ex.Message}");
                database = new RunDatabase();
            }
        }

        private void ResolveWatchedPaths()
        {
            watchFolder = ResolveWatchFolder();
            blzLogFilePath = database.Settings.TryDetectStart ? ResolveBlzLogFile() : null;

            database.Settings.WatchFolder = watchFolder;
            if (!string.IsNullOrWhiteSpace(blzLogFilePath))
            {
                database.Settings.BlzLogFilePath = blzLogFilePath;
            }

            SaveRuns();
        }

        private void ApplyOptionsToUi()
        {
            beepOnEndToolStripMenuItem.Checked = database.Settings.BeepEnabled;
            beepOnStartToolStripMenuItem.Checked = database.Settings.TryDetectStart;
            setInterval1500ToolStripMenuItem.Text = $"Set Interval ({database.Settings.TimerInterval})";
            timer1.Interval = Math.Max(100, database.Settings.TimerInterval);
        }

        private string? ResolveWatchFolder()
        {
            if (HasSettingsJson(database.Settings.WatchFolder))
            {
                return database.Settings.WatchFolder;
            }

            var defaultFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Saved Games",
                "Diablo II Resurrected");

            if (HasSettingsJson(defaultFolder))
            {
                return defaultFolder;
            }

            using var dialog = new OpenFileDialog
            {
                Title = "Select settings.json",
                Filter = "settings.json|settings.json|JSON files (*.json)|*.json|All files (*.*)|*.*",
                CheckFileExists = true,
                InitialDirectory = Directory.Exists(defaultFolder)
                    ? defaultFolder
                    : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                return Path.GetDirectoryName(dialog.FileName);
            }

            AddLog("settings.json was not selected. Run completion tracking is disabled until a valid folder is configured.");
            return null;
        }

        private string? ResolveBlzLogFile()
        {
            if (File.Exists(database.Settings.BlzLogFilePath))
            {
                return database.Settings.BlzLogFilePath;
            }

            var defaultFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "battleNet",
                "Diablo II Resurrected",
                "blz-log.txt");

            if (File.Exists(defaultFile))
            {
                return defaultFile;
            }

            using var dialog = new OpenFileDialog
            {
                Title = "Select blz-log.txt",
                Filter = "blz-log.txt|blz-log.txt|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                CheckFileExists = true,
                InitialDirectory = Directory.Exists(Path.GetDirectoryName(defaultFile))
                    ? Path.GetDirectoryName(defaultFile)
                    : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                return dialog.FileName;
            }

            AddLog("blz-log.txt was not selected. Turn start detection is disabled.");
            return null;
        }

        private static bool HasSettingsJson(string? folder)
        {
            return !string.IsNullOrWhiteSpace(folder) &&
                   Directory.Exists(folder) &&
                   Directory.EnumerateFiles(folder, "settings.json", SearchOption.TopDirectoryOnly).Any();
        }

        private void SaveRuns()
        {
            try
            {
                var json = JsonSerializer.Serialize(database, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(runsFilePath, json);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                AddLog($"runs.json could not be written: {ex.Message}");
            }
        }

        private void LoadInitialSnapshot()
        {
            lastWriteTimes.Clear();

            if (string.IsNullOrWhiteSpace(watchFolder) || !Directory.Exists(watchFolder))
            {
                AddLog($"Watch folder was not found: {watchFolder ?? "(not set)"}");
                missingFolderWarningShown = true;
                return;
            }

            foreach (var file in EnumerateFilesSafe(watchFolder))
            {
                if (TryReadLastWriteTime(file, out var lastWriteTime))
                {
                    lastWriteTimes[file] = lastWriteTime;
                }
            }

            if (!string.IsNullOrWhiteSpace(blzLogFilePath) && TryReadLastWriteTime(blzLogFilePath, out var blzLastWriteTime))
            {
                lastBlzLogWriteTime = blzLastWriteTime;
            }

            AddLog($"Monitoring started: {watchFolder}");
        }

        private void timer1_Tick(object? sender, EventArgs e)
        {
            ApplyTimerInterval();
            RefreshCounter();

            if (ProcessStartDetectionWindow())
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(watchFolder) || !Directory.Exists(watchFolder))
            {
                if (!missingFolderWarningShown)
                {
                    AddLog($"Watch folder was not found: {watchFolder ?? "(not set)"}");
                    missingFolderWarningShown = true;
                }

                return;
            }

            missingFolderWarningShown = false;
            var changedRelevantFiles = new List<string>();

            foreach (var file in EnumerateFilesSafe(watchFolder))
            {
                if (!TryReadLastWriteTime(file, out var currentLastWriteTime))
                {
                    continue;
                }

                if (!lastWriteTimes.TryGetValue(file, out var previousLastWriteTime))
                {
                    lastWriteTimes[file] = currentLastWriteTime;
                    if (IsRunSignalFile(file))
                    {
                        changedRelevantFiles.Add(file);
                    }

                    continue;
                }

                if (currentLastWriteTime != previousLastWriteTime)
                {
                    lastWriteTimes[file] = currentLastWriteTime;
                    if (IsRunSignalFile(file))
                    {
                        changedRelevantFiles.Add(file);
                    }
                }
            }

            if (changedRelevantFiles.Count == 0)
            {
                return;
            }

            var now = DateTime.Now;
            if (lastRunDetectionTime.HasValue && now - lastRunDetectionTime.Value < DuplicateChangeWindow)
            {
                AddLog($"Ignored within {DuplicateChangeWindow.TotalSeconds:0}s window: {FormatChangedFiles(changedRelevantFiles)}");
                return;
            }

            lastRunDetectionTime = now;
            CompleteRunTurn(now, changedRelevantFiles);
        }

        private bool ProcessStartDetectionWindow()
        {
            if (startDetectionState == StartDetectionState.Idle)
            {
                return false;
            }

            var now = DateTime.Now;
            if (startDetectionExpiresAt.HasValue && now >= startDetectionExpiresAt.Value)
            {
                AcceptDefaultTurnStart();
                return false;
            }

            if (startDetectionState == StartDetectionState.Cooldown)
            {
                if (startDetectionAvailableAt.HasValue && now >= startDetectionAvailableAt.Value)
                {
                    startDetectionState = StartDetectionState.AwaitingBlzLog;
                    SnapshotBlzLog();
                    AddLog("Start detection is now watching blz-log.txt.");
                }

                return true;
            }

            CheckBlzLogChange();
            return true;
        }

        private void CheckBlzLogChange()
        {
            if (!database.Settings.TryDetectStart)
            {
                AcceptDefaultTurnStart();
                return;
            }

            if (string.IsNullOrWhiteSpace(blzLogFilePath) || !File.Exists(blzLogFilePath))
            {
                if (!missingBlzLogWarningShown)
                {
                    AddLog($"blz-log.txt was not found: {blzLogFilePath ?? "(not set)"}");
                    missingBlzLogWarningShown = true;
                }

                return;
            }

            missingBlzLogWarningShown = false;

            if (!TryReadLastWriteTime(blzLogFilePath, out var currentLastWriteTime))
            {
                return;
            }

            if (!lastBlzLogWriteTime.HasValue)
            {
                lastBlzLogWriteTime = currentLastWriteTime;
                return;
            }

            if (currentLastWriteTime == lastBlzLogWriteTime.Value)
            {
                return;
            }

            lastBlzLogWriteTime = currentLastWriteTime;

            if (startDetectionState != StartDetectionState.AwaitingBlzLog ||
                !lastCompletedRunTime.HasValue ||
                currentLastWriteTime < lastCompletedRunTime.Value)
            {
                return;
            }

            currentTurnStartedAt = currentLastWriteTime;
            startDetectionState = StartDetectionState.Idle;
            AddLog($"New turn started: {currentLastWriteTime:dd.MM.yyyy HH:mm:ss}");
            PlayTurnStartSound();
        }

        private void BeginStartDetection(DateTime completedAt)
        {
            if (!database.Settings.TryDetectStart)
            {
                currentTurnStartedAt = completedAt;
                startDetectionState = StartDetectionState.Idle;
                AddLog("Start detection is disabled. Next turn start defaults to previous finish time.");
                return;
            }

            startDetectionState = StartDetectionState.Cooldown;
            startDetectionAvailableAt = completedAt + StartDetectionCooldown;
            startDetectionExpiresAt = startDetectionAvailableAt.Value + StartDetectionTimeout;
            AddLog($"Waiting {StartDetectionCooldown.TotalSeconds:0}s before watching blz-log.txt for the next turn start.");
        }

        private void AcceptDefaultTurnStart()
        {
            if (lastCompletedRunTime.HasValue)
            {
                currentTurnStartedAt = lastCompletedRunTime.Value;
                AddLog("No reliable blz-log start signal was detected. Next turn start defaults to previous finish time.");
            }

            startDetectionState = StartDetectionState.Idle;
            startDetectionAvailableAt = null;
            startDetectionExpiresAt = null;
        }

        private void SnapshotBlzLog()
        {
            if (!string.IsNullOrWhiteSpace(blzLogFilePath) && TryReadLastWriteTime(blzLogFilePath, out var blzLastWriteTime))
            {
                lastBlzLogWriteTime = blzLastWriteTime;
            }
        }

        private void CompleteRunTurn(DateTime finishedAt, List<string> changedFiles)
        {
            if (activeRun is null)
            {
                AddLog($"Run signal received, but there is no active run: {FormatChangedFiles(changedFiles)}");
                return;
            }

            var detectedCharacter = DetectCharacter(changedFiles) ?? currentCharacter ?? activeRun.CurrentCharacter ?? "Unknown";
            detectedCharacter = NormalizeCharacterName(detectedCharacter);

            if (!string.IsNullOrWhiteSpace(currentCharacter) &&
                !string.Equals(currentCharacter, detectedCharacter, StringComparison.OrdinalIgnoreCase))
            {
                AddLog($"Character changed: {currentCharacter} -> {detectedCharacter}. The run continues.");
            }

            currentCharacter = detectedCharacter;
            activeRun.CurrentCharacter = detectedCharacter;

            var turn = new RunTurn
            {
                Number = activeRun.Turns.Count + 1,
                StartedAt = currentTurnStartedAt ?? activeRun.StartedAt,
                FinishedAt = finishedAt,
                Character = detectedCharacter,
                Found = pendingFoundForCurrentRun,
                ChangedFiles = changedFiles.Select(Path.GetFileName).Where(name => name is not null).Cast<string>().ToList()
            };

            activeRun.Turns.Add(turn);
            activeRun.EndedAt = finishedAt;
            lastCompletedRunTime = finishedAt;
            pendingFoundForCurrentRun = false;

            BeginStartDetection(finishedAt);

            if (database.Settings.BeepEnabled)
            {
                SystemSounds.Beep.Play();
            }

            AddLog($"Run completed: #{turn.Number} {detectedCharacter} {finishedAt:dd.MM.yyyy HH:mm:ss}");
            SaveRuns();
            RefreshAll();
        }

        private void PlayTurnStartSound()
        {
            if (!database.Settings.BeepEnabled)
            {
                return;
            }

            SystemSounds.Asterisk.Play();
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
                button1.Text = "Resume Run";
                SetCounterColor(PausedCounterColor);
                AddLog("Monitoring paused.");
                return;
            }

            ApplyTimerInterval();
            LoadInitialSnapshot();
            lastRunDetectionTime = null;
            timer1.Start();
            button1.Text = "Pause Run";
            SetCounterColor(pendingFoundForCurrentRun ? FoundCounterColor : NormalCounterColor);
            AddLog("Monitoring resumed.");
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            var reset = MessageBox.Show(
                "Reset the active run and start a new one?",
                "New Run",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (reset != DialogResult.Yes)
            {
                return;
            }

            var runName = Prompt.Show("Enter the run name:", "New Run");
            if (string.IsNullOrWhiteSpace(runName))
            {
                return;
            }

            StartNewRun(runName.Trim());
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            ModifyActiveRunCount(1);
        }

        private void button4_Click(object? sender, EventArgs e)
        {
            ModifyActiveRunCount(-1);
        }

        private void button5_Click(object? sender, EventArgs e)
        {
            if (GetSelectedRun() is not { } selectedRun)
            {
                AddLog("Select a run from the run list before using Continue Run.");
                return;
            }

            activeRun = selectedRun;
            currentCharacter = activeRun.CurrentCharacter;
            currentTurnStartedAt = activeRun.EndedAt ?? activeRun.StartedAt;
            pendingFoundForCurrentRun = false;
            lastRunDetectionTime = null;
            lastCompletedRunTime = activeRun.EndedAt;
            startDetectionState = StartDetectionState.Idle;
            LoadInitialSnapshot();
            timer1.Start();
            button1.Text = "Pause Run";
            RefreshAll();
            AddLog($"Continuing run: {activeRun.Name}");
        }

        private void button6_Click(object? sender, EventArgs e)
        {
            if (activeRun is null)
            {
                AddLog("There is no active run to mark this turn as found.");
                return;
            }

            pendingFoundForCurrentRun = true;
            SetCounterColor(FoundCounterColor);
            AddLog("This turn will be marked as found.");
        }

        private void button7_Click(object? sender, EventArgs e)
        {
            var run = activeRun ?? GetSelectedRun();
            var previousTurn = run?.Turns.LastOrDefault();
            if (previousTurn is null)
            {
                AddLog("There is no completed turn to mark as found.");
                return;
            }

            previousTurn.Found = true;
            SaveRuns();
            RefreshAll();
            SetCounterColor(FoundCounterColor);
            AddLog($"Previous turn was marked as found: #{previousTurn.Number}");
        }

        private void StartNewRun(string runName)
        {
            activeRun = new RunSession
            {
                Id = Guid.NewGuid(),
                Name = runName,
                StartedAt = DateTime.Now
            };

            database.Runs.Insert(0, activeRun);
            currentCharacter = null;
            currentTurnStartedAt = activeRun.StartedAt;
            pendingFoundForCurrentRun = false;
            lastRunDetectionTime = null;
            lastCompletedRunTime = null;
            startDetectionState = StartDetectionState.Idle;
            LoadInitialSnapshot();
            timer1.Start();
            button1.Text = "Pause Run";
            SaveRuns();
            RefreshAll();
            AddLog($"New run started: {runName}");
        }

        private void ModifyActiveRunCount(int delta)
        {
            if (activeRun is null)
            {
                AddLog("There is no active run for count modification.");
                return;
            }

            activeRun.ManualModifier += delta;
            SaveRuns();
            RefreshAll();
            AddLog($"Modifier {(delta > 0 ? "+" : "")}{delta}: total modifier {activeRun.ManualModifier:+#;-#;0}");
        }

        private void button8_Click(object? sender, EventArgs e)
        {
            contextMenuStrip3.Show(button8, new Point(0, button8.Height));
        }

        private void beepOnEndToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            database.Settings.BeepEnabled = beepOnEndToolStripMenuItem.Checked;
            SaveRuns();
        }

        private void beepOnStartToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            database.Settings.TryDetectStart = beepOnStartToolStripMenuItem.Checked;
            if (!database.Settings.TryDetectStart)
            {
                AcceptDefaultTurnStart();
            }
            else if (string.IsNullOrWhiteSpace(blzLogFilePath))
            {
                blzLogFilePath = ResolveBlzLogFile();
                database.Settings.BlzLogFilePath = blzLogFilePath;
                SnapshotBlzLog();
            }

            SaveRuns();
        }

        private void setInterval1500ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var input = Prompt.Show("Enter timer interval in milliseconds:", "Set Interval", database.Settings.TimerInterval.ToString());
            if (!int.TryParse(input, out var interval) || interval < 100)
            {
                AddLog("Interval was not changed. Enter a number >= 100.");
                return;
            }

            database.Settings.TimerInterval = interval;
            ApplyOptionsToUi();
            SaveRuns();
            AddLog($"Timer interval set to {interval} ms.");
        }

        private void ApplyTimerInterval()
        {
            timer1.Interval = Math.Max(100, database.Settings.TimerInterval);
        }

        private void RefreshAll()
        {
            RefreshCounter();
            RefreshRunList();
            RefreshDetails();
        }

        private void RefreshCounter()
        {
            label1.Text = GetDisplayedCounter(activeRun).ToString();
            labelRunDuration.Text = FormatDuration(GetRunDuration(activeRun, useCurrentTime: activeRun is not null));
            labelTurnAverage.Text = FormatDuration(GetAverageTurnDuration(activeRun, useCurrentTime: activeRun is not null));

            if (!timer1.Enabled)
            {
                SetCounterColor(PausedCounterColor);
            }
            else
            {
                SetCounterColor(pendingFoundForCurrentRun ? FoundCounterColor : NormalCounterColor);
            }
        }

        private void SetCounterColor(Color color)
        {
            label1.ForeColor = color;
        }

        private static int GetDisplayedCounter(RunSession? run)
        {
            if (run is null)
            {
                return 0;
            }

            return Math.Max(1, run.Turns.Count + run.ManualModifier + 1);
        }

        private static TimeSpan GetRunDuration(RunSession? run, bool useCurrentTime)
        {
            if (run is null)
            {
                return TimeSpan.Zero;
            }

            var end = useCurrentTime ? DateTime.Now : run.EndedAt ?? DateTime.Now;
            return end - run.StartedAt;
        }

        private static TimeSpan GetAverageTurnDuration(RunSession? run, bool useCurrentTime)
        {
            if (run is null || run.Turns.Count == 0)
            {
                return TimeSpan.Zero;
            }

            if (run.Turns.All(turn => turn.StartedAt != default && turn.FinishedAt >= turn.StartedAt))
            {
                return TimeSpan.FromTicks((long)run.Turns.Average(turn => (turn.FinishedAt - turn.StartedAt).Ticks));
            }

            var totalDuration = GetRunDuration(run, useCurrentTime);
            return TimeSpan.FromTicks(totalDuration.Ticks / run.Turns.Count);
        }

        private static string FormatDuration(TimeSpan duration)
        {
            return duration.TotalHours >= 1
                ? $"{(int)duration.TotalHours:00}:{duration.Minutes:00}:{duration.Seconds:00}"
                : $"{duration.Minutes:00}:{duration.Seconds:00}";
        }

        private void RefreshRunList()
        {
            var selectedId = GetSelectedRun()?.Id ?? activeRun?.Id;
            listBox2.BeginUpdate();
            listBox2.Items.Clear();

            foreach (var run in database.Runs.OrderByDescending(run => run.StartedAt))
            {
                listBox2.Items.Add(new RunListItem(run));
            }

            if (selectedId.HasValue)
            {
                for (var i = 0; i < listBox2.Items.Count; i++)
                {
                    if (listBox2.Items[i] is RunListItem item && item.Run.Id == selectedId.Value)
                    {
                        listBox2.SelectedIndex = i;
                        break;
                    }
                }
            }

            listBox2.EndUpdate();
        }

        private void RefreshDetails()
        {
            var run = GetSelectedRun() ?? activeRun;
            listView1.BeginUpdate();
            listView1.Items.Clear();

            if (run is not null)
            {
                foreach (var turn in run.Turns.OrderByDescending(turn => turn.Number))
                {
                    var item = new ListViewItem(turn.Number.ToString());
                    item.SubItems.Add(turn.StartedAt == default ? "" : turn.StartedAt.ToString("dd.MM.yyyy HH:mm:ss"));
                    item.SubItems.Add(turn.FinishedAt.ToString("dd.MM.yyyy HH:mm:ss"));
                    item.SubItems.Add(turn.Character);
                    item.SubItems.Add(turn.Found ? "Yes" : "");
                    item.SubItems.Add(string.Join(", ", turn.ChangedFiles));
                    if (turn.Found)
                    {
                        item.ForeColor = FoundCounterColor;
                    }

                    listView1.Items.Add(item);
                }
            }

            listView1.EndUpdate();
        }

        private void listBox2_SelectedIndexChanged(object? sender, EventArgs e)
        {
            RefreshDetails();
        }

        private void listBox2_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var index = listBox2.IndexFromPoint(e.Location);
            if (index >= 0)
            {
                listBox2.SelectedIndex = index;
            }
        }

        private RunSession? GetSelectedRun()
        {
            return (listBox2.SelectedItem as RunListItem)?.Run;
        }

        private void AddLog(string text)
        {
            listBox1.Items.Insert(0, $"{DateTime.Now:HH:mm:ss} {text}");
        }

        private static bool IsRunSignalFile(string file)
        {
            var fileName = Path.GetFileName(file);
            return fileName.Equals("settings.json", StringComparison.OrdinalIgnoreCase) ||
                   Path.GetExtension(fileName).Equals(".ctlo", StringComparison.OrdinalIgnoreCase);
        }

        private static string? DetectCharacter(IEnumerable<string> changedFiles)
        {
            var ctloFile = changedFiles.FirstOrDefault(file =>
                Path.GetExtension(file).Equals(".ctlo", StringComparison.OrdinalIgnoreCase));

            if (ctloFile is null)
            {
                return null;
            }

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(ctloFile);
            var characterName = new string(nameWithoutExtension.Where(char.IsLetter).ToArray());
            return string.IsNullOrWhiteSpace(characterName) ? null : characterName;
        }

        private static string NormalizeCharacterName(string characterName)
        {
            characterName = characterName.Trim();
            if (characterName.Length == 0)
            {
                return "Unknown";
            }

            return char.ToUpperInvariant(characterName[0]) + characterName[1..].ToLowerInvariant();
        }

        private static string FormatChangedFiles(IEnumerable<string> changedFiles)
        {
            return string.Join(", ", changedFiles.Select(Path.GetFileName));
        }

        private static bool TryReadLastWriteTime(string file, out DateTime lastWriteTime)
        {
            try
            {
                lastWriteTime = File.GetLastWriteTime(file);
                return true;
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }

            lastWriteTime = default;
            return false;
        }

        private static IEnumerable<string> EnumerateFilesSafe(string rootFolder)
        {
            var pendingFolders = new Stack<string>();
            pendingFolders.Push(rootFolder);

            while (pendingFolders.Count > 0)
            {
                var folder = pendingFolders.Pop();

                string[] files;
                try
                {
                    files = Directory.GetFiles(folder);
                }
                catch (IOException)
                {
                    continue;
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }

                foreach (var file in files)
                {
                    yield return file;
                }

                string[] subFolders;
                try
                {
                    subFolders = Directory.GetDirectories(folder);
                }
                catch (IOException)
                {
                    continue;
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }

                foreach (var subFolder in subFolders)
                {
                    pendingFolders.Push(subFolder);
                }
            }
        }

        private void contextMenuStrip1_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var hasSelection = GetSelectedRun() is not null;
            copyToolStripMenuItem.Enabled = hasSelection;
            renameToolStripMenuItem.Enabled = hasSelection;
            duplicateToolStripMenuItem.Enabled = hasSelection;
            modifyCountToolStripMenuItem.Enabled = hasSelection;
            deleteToolStripMenuItem.Enabled = hasSelection;
        }

        private void copyToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (GetSelectedRun() is { } run)
            {
                Clipboard.SetText(new RunListItem(run).ToString());
            }
        }

        private void renameToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (GetSelectedRun() is not { } run)
            {
                return;
            }

            var newName = Prompt.Show("Enter the new run name:", "Rename", run.Name);
            if (string.IsNullOrWhiteSpace(newName))
            {
                return;
            }

            run.Name = newName.Trim();
            SaveRuns();
            RefreshAll();
        }

        private void duplicateToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (GetSelectedRun() is not { } run)
            {
                return;
            }

            var copy = run.Clone();
            copy.Id = Guid.NewGuid();
            copy.Name = $"{run.Name} Copy";
            copy.StartedAt = DateTime.Now;
            database.Runs.Insert(0, copy);
            SaveRuns();
            RefreshAll();
        }

        private void modifyCountToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (GetSelectedRun() is not { } run)
            {
                return;
            }

            var input = Prompt.Show("Enter the modifier value:", "Modify Count", run.ManualModifier.ToString());
            if (!int.TryParse(input, out var modifier))
            {
                return;
            }

            run.ManualModifier = modifier;
            SaveRuns();
            RefreshAll();
        }

        private void deleteToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (GetSelectedRun() is not { } run)
            {
                return;
            }

            var result = MessageBox.Show($"Delete '{run.Name}'?", "Delete Run", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
            {
                return;
            }

            database.Runs.Remove(run);
            if (activeRun?.Id == run.Id)
            {
                activeRun = null;
                currentCharacter = null;
                currentTurnStartedAt = null;
                pendingFoundForCurrentRun = false;
                startDetectionState = StartDetectionState.Idle;
            }

            SaveRuns();
            RefreshAll();
        }
    }

    public sealed class RunDatabase
    {
        public AppSettings Settings { get; set; } = new();
        public List<RunSession> Runs { get; set; } = new();
    }

    public enum StartDetectionState
    {
        Idle,
        Cooldown,
        AwaitingBlzLog
    }

    public sealed class AppSettings
    {
        public string? WatchFolder { get; set; }
        public string? BlzLogFilePath { get; set; }
        public bool BeepEnabled { get; set; } = true;
        public bool TryDetectStart { get; set; } = true;
        public int TimerInterval { get; set; } = 1500;
    }

    public sealed class RunSession
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int ManualModifier { get; set; }
        public string? CurrentCharacter { get; set; }
        public List<RunTurn> Turns { get; set; } = new();
        public int TotalRuns => Turns.Count + ManualModifier;
        public int TotalFound => Turns.Count(turn => turn.Found);

        public RunSession Clone()
        {
            return new RunSession
            {
                Id = Id,
                Name = Name,
                StartedAt = StartedAt,
                EndedAt = EndedAt,
                ManualModifier = ManualModifier,
                CurrentCharacter = CurrentCharacter,
                Turns = Turns.Select(turn => turn.Clone()).ToList()
            };
        }
    }

    public sealed class RunTurn
    {
        public int Number { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public string Character { get; set; } = "Unknown";
        public bool Found { get; set; }
        public List<string> ChangedFiles { get; set; } = new();

        public RunTurn Clone()
        {
            return new RunTurn
            {
                Number = Number,
                StartedAt = StartedAt,
                FinishedAt = FinishedAt,
                Character = Character,
                Found = Found,
                ChangedFiles = ChangedFiles.ToList()
            };
        }
    }

    public sealed class RunListItem
    {
        public RunListItem(RunSession run)
        {
            Run = run;
        }

        public RunSession Run { get; }

        public override string ToString()
        {
            var character = string.IsNullOrWhiteSpace(Run.CurrentCharacter) ? "Unknown" : Run.CurrentCharacter;
            var lastDate = Run.EndedAt ?? Run.StartedAt;
            var durationEnd = Run.EndedAt ?? DateTime.Now;
            var duration = durationEnd - Run.StartedAt;
            var modifier = Run.ManualModifier == 0 ? "" : $" (Modifier {Run.ManualModifier:+#;-#;0})";

            return $"{character}: ({Run.Name}) {Run.TotalRuns} Runs, Duration: {duration:dd\\:hh\\:mm\\:ss}, " +
                   $"{Run.TotalFound} found, Last: {lastDate:dd.MM.yyyy HH:mm:ss:ff}{modifier}";
        }
    }

    public static class Prompt
    {
        public static string? Show(string text, string caption, string defaultValue = "")
        {
            using var prompt = new Form
            {
                Width = 420,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var label = new Label { Left = 12, Top = 15, Width = 380, Text = text };
            var textBox = new TextBox { Left = 12, Top = 40, Width = 380, Text = defaultValue };
            var confirmation = new Button { Text = "OK", Left = 236, Width = 75, Top = 75, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Cancel", Left = 317, Width = 75, Top = 75, DialogResult = DialogResult.Cancel };

            prompt.Controls.Add(label);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
    }
}
