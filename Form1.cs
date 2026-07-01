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
        private readonly List<PausePeriod> currentRunPausePeriods = new();
        private TimeSpan currentRunPausedTime = TimeSpan.Zero;
        private DateTime? currentPauseStartedAt;
        private DateTime? lastRunDetectionTime;
        private DateTime? lastCompletedRunTime;
        private StartDetectionState startDetectionState = StartDetectionState.Idle;
        private DateTime? startDetectionAvailableAt;
        private DateTime? startDetectionExpiresAt;
        private bool missingFolderWarningShown;
        private bool missingBlzLogWarningShown;
        private bool pendingFoundForCurrentRun;
        private FoundInfo pendingFoundInfo = new();
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
            listView2.View = View.Details;
            listView2.FullRowSelect = true;
            listView2.GridLines = true;
            listView2.ContextMenuStrip = contextMenuStrip1;
            listView2.SelectedIndexChanged += listView2_SelectedIndexChanged;
            listView2.MouseDown += listView2_MouseDown;
            listView2.Columns.Add("Character", 80);
            listView2.Columns.Add("Session", 95);
            listView2.Columns.Add("Runs", 50);
            listView2.Columns.Add("Duration", 80);
            listView2.Columns.Add("Real Time", 80);
            listView2.Columns.Add("Found", 55);
            listView2.Columns.Add("Last", 125);
            listView2.Columns.Add("Modifier", 65);

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.ContextMenuStrip = contextMenuStrip2;
            listView1.MouseDown += listView1_MouseDown;
            listView1.Columns.Add("#", 45);
            listView1.Columns.Add("Started", 125);
            listView1.Columns.Add("Finished", 125);
            listView1.Columns.Add("Duration", 75);
            listView1.Columns.Add("Paused", 75);
            listView1.Columns.Add("Character", 90);
            listView1.Columns.Add("Found", 55);
            listView1.Columns.Add("Hi", 45);
            listView1.Columns.Add("Mid", 45);
            listView1.Columns.Add("Low", 45);
            listView1.Columns.Add("Note", 140);

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
            viewEventLogToolStripMenuItem.CheckOnClick = true;
            viewEventLogToolStripMenuItem.Checked = listBox1.Visible;
            viewEventLogToolStripMenuItem.Click += viewEventLogToolStripMenuItem_Click;
            viewDetailedFoundButtonsToolStripMenuItem.CheckOnClick = true;
            viewDetailedFoundButtonsToolStripMenuItem.Checked = groupBoxFound.Visible;
            viewDetailedFoundButtonsToolStripMenuItem.Click += viewDetailedFoundButtonsToolStripMenuItem_Click;

            editPauseToolStripMenuItem.Click += editPauseToolStripMenuItem_Click;
            removeToolStripMenuItem.Click += removeToolStripMenuItem_Click;
            markFoundToolStripMenuItem.Click += markFoundToolStripMenuItem_Click;
            removeFoundToolStripMenuItem.Click += removeFoundToolStripMenuItem_Click;

            buttonhip.Click += (_, _) => AddDetailedFound(FoundTier.High, 1);
            buttonhim.Click += (_, _) => AddDetailedFound(FoundTier.High, -1);
            buttonmidp.Click += (_, _) => AddDetailedFound(FoundTier.Mid, 1);
            buttonmidm.Click += (_, _) => AddDetailedFound(FoundTier.Mid, -1);
            buttonlop.Click += (_, _) => AddDetailedFound(FoundTier.Low, 1);
            buttonlom.Click += (_, _) => AddDetailedFound(FoundTier.Low, -1);

            groupBoxFound.Visible = false;
            viewDetailedFoundButtonsToolStripMenuItem.Checked = false;
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

            AddLog("blz-log.txt was not selected. Run start detection is disabled.");
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
            ResetCurrentRunPauseTracking();
            startDetectionState = StartDetectionState.Idle;
            AddLog($"New run started: {currentLastWriteTime:dd.MM.yyyy HH:mm:ss}");
            PlayTurnStartSound();
        }

        private void BeginStartDetection(DateTime completedAt)
        {
            if (!database.Settings.TryDetectStart)
            {
                currentTurnStartedAt = completedAt;
                ResetCurrentRunPauseTracking();
                startDetectionState = StartDetectionState.Idle;
                AddLog("Start detection is disabled. Next run start defaults to previous finish time.");
                return;
            }

            startDetectionState = StartDetectionState.Cooldown;
            startDetectionAvailableAt = completedAt + StartDetectionCooldown;
            startDetectionExpiresAt = startDetectionAvailableAt.Value + StartDetectionTimeout;
            AddLog($"Waiting {StartDetectionCooldown.TotalSeconds:0}s before watching blz-log.txt for the next run start.");
        }

        private void AcceptDefaultTurnStart()
        {
            if (lastCompletedRunTime.HasValue)
            {
                currentTurnStartedAt = lastCompletedRunTime.Value;
                ResetCurrentRunPauseTracking();
                AddLog("No reliable blz-log start signal was detected. Next run start defaults to previous finish time.");
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
            CloseActivePause(finishedAt);

            var turn = new RunTurn
            {
                Number = activeRun.Turns.Count + 1,
                StartedAt = currentTurnStartedAt ?? activeRun.StartedAt,
                FinishedAt = finishedAt,
                PausedSeconds = (long)currentRunPausedTime.TotalSeconds,
                PausePeriods = currentRunPausePeriods.Select(period => period.Clone()).ToList(),
                Character = detectedCharacter,
                Found = pendingFoundForCurrentRun || pendingFoundInfo.HasAnyValue,
                FoundHi = pendingFoundInfo.Hi,
                FoundMid = pendingFoundInfo.Mid,
                FoundLow = pendingFoundInfo.Low,
                FoundNote = pendingFoundInfo.Note
            };

            activeRun.Turns.Add(turn);
            activeRun.EndedAt = finishedAt;
            lastCompletedRunTime = finishedAt;
            currentTurnStartedAt = null;
            ResetCurrentRunPauseTracking();
            pendingFoundForCurrentRun = false;
            pendingFoundInfo = new FoundInfo();

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
                StartPause();
                timer1.Stop();
                button1.Text = "Resume Run";
                SetCounterColor(PausedCounterColor);
                AddLog("Monitoring paused.");
                return;
            }

            ResumePause();
            ApplyTimerInterval();
            LoadInitialSnapshot();
            lastRunDetectionTime = null;
            timer1.Start();
            button1.Text = "Pause Run";
            SetCounterColor(pendingFoundForCurrentRun ? FoundCounterColor : NormalCounterColor);
            AddLog("Monitoring resumed.");
        }

        private void StartPause()
        {
            if (activeRun is null || currentPauseStartedAt.HasValue)
            {
                return;
            }

            currentPauseStartedAt = DateTime.Now;
            activeRun.CurrentPauseStartedAt = currentPauseStartedAt;
            SaveRuns();
        }

        private void ResumePause()
        {
            if (activeRun is null || !currentPauseStartedAt.HasValue)
            {
                return;
            }

            CloseActivePause(DateTime.Now);
            SaveRuns();
        }

        private void CloseActivePause(DateTime endedAt)
        {
            if (activeRun is null || !currentPauseStartedAt.HasValue)
            {
                return;
            }

            var pauseStartedAt = currentPauseStartedAt.Value;
            if (endedAt < pauseStartedAt)
            {
                endedAt = pauseStartedAt;
            }

            var pausePeriod = new PausePeriod
            {
                StartedAt = pauseStartedAt,
                EndedAt = endedAt
            };

            currentRunPausePeriods.Add(pausePeriod);
            currentRunPausedTime += pausePeriod.Duration;
            currentPauseStartedAt = null;

            activeRun.CurrentRunPausedSeconds = (long)currentRunPausedTime.TotalSeconds;
            activeRun.CurrentRunPausePeriods = currentRunPausePeriods.Select(period => period.Clone()).ToList();
            activeRun.CurrentPauseStartedAt = null;
        }

        private void ResetCurrentRunPauseTracking()
        {
            currentRunPausePeriods.Clear();
            currentRunPausedTime = TimeSpan.Zero;
            currentPauseStartedAt = null;

            if (activeRun is null)
            {
                return;
            }

            activeRun.CurrentRunPausedSeconds = 0;
            activeRun.CurrentRunPausePeriods.Clear();
            activeRun.CurrentPauseStartedAt = null;
            SaveRuns();
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            var reset = MessageBox.Show(
                "Reset the active run and start a new one?",
                "New Session",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (reset != DialogResult.Yes)
            {
                return;
            }

            var runName = Prompt.Show("Enter the session name:", "New Session");
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
                AddLog("Select a session from the session list before using Continue Session.");
                return;
            }

            activeRun = selectedRun;
            currentCharacter = activeRun.CurrentCharacter;
            currentTurnStartedAt = DateTime.Now;
            ResetCurrentRunPauseTracking();
            pendingFoundForCurrentRun = false;
            pendingFoundInfo = new FoundInfo();
            lastRunDetectionTime = null;
            lastCompletedRunTime = activeRun.EndedAt;
            startDetectionState = StartDetectionState.Idle;
            LoadInitialSnapshot();
            timer1.Start();
            button1.Text = "Pause Run";
            RefreshAll();
            AddLog($"Continuing session: {activeRun.Name}");
        }

        private void button6_Click(object? sender, EventArgs e)
        {
            if (activeRun is null)
            {
                AddLog("There is no active session to mark this run as found.");
                return;
            }

            pendingFoundForCurrentRun = !pendingFoundForCurrentRun;
            if (!pendingFoundForCurrentRun)
            {
                pendingFoundInfo = new FoundInfo();
            }

            SetCounterColor(pendingFoundForCurrentRun ? FoundCounterColor : NormalCounterColor);
            AddLog(pendingFoundForCurrentRun
                ? "This run will be marked as found."
                : "This run found mark was removed.");
        }

        private void button7_Click(object? sender, EventArgs e)
        {
            var run = activeRun ?? GetSelectedRun();
            var previousTurn = run?.Turns.LastOrDefault();
            if (previousTurn is null)
            {
                AddLog("There is no completed run to mark as found.");
                return;
            }

            previousTurn.Found = !previousTurn.Found;
            if (!previousTurn.Found)
            {
                previousTurn.FoundHi = 0;
                previousTurn.FoundMid = 0;
                previousTurn.FoundLow = 0;
                previousTurn.FoundNote = "";
            }

            SaveRuns();
            RefreshAll();
            SetCounterColor(previousTurn.Found ? FoundCounterColor : NormalCounterColor);
            AddLog(previousTurn.Found
                ? $"Previous run was marked as found: #{previousTurn.Number}"
                : $"Previous run found mark was removed: #{previousTurn.Number}");
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
            ResetCurrentRunPauseTracking();
            pendingFoundForCurrentRun = false;
            pendingFoundInfo = new FoundInfo();
            lastRunDetectionTime = null;
            lastCompletedRunTime = null;
            startDetectionState = StartDetectionState.Idle;
            LoadInitialSnapshot();
            timer1.Start();
            button1.Text = "Pause Run";
            SaveRuns();
            RefreshAll();
            AddLog($"New session started: {runName}");
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

        private void viewEventLogToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            listBox1.Visible = viewEventLogToolStripMenuItem.Checked;
            if (listBox1.Visible)
            {
                groupBoxFound.Visible = false;
                viewDetailedFoundButtonsToolStripMenuItem.Checked = false;
            }
        }

        private void viewDetailedFoundButtonsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            groupBoxFound.Visible = viewDetailedFoundButtonsToolStripMenuItem.Checked;
            if (groupBoxFound.Visible)
            {
                listBox1.Visible = false;
                viewEventLogToolStripMenuItem.Checked = false;
            }
        }

        private void AddDetailedFound(FoundTier tier, int delta)
        {
            var note = textBox1.Text.Trim();
            if (checkBoxinPrevRun.Checked)
            {
                var runEntry = (activeRun ?? GetSelectedRun())?.Turns.LastOrDefault();
                if (runEntry is null)
                {
                    AddLog("There is no previous completed run for detailed found entry.");
                    return;
                }

                ApplyFoundDelta(runEntry, tier, delta, note);
                SaveRuns();
                RefreshAll();
                return;
            }

            if (activeRun is null)
            {
                AddLog("There is no active session for detailed found entry.");
                return;
            }

            ApplyFoundDelta(pendingFoundInfo, tier, delta, note);
            pendingFoundForCurrentRun = pendingFoundInfo.HasAnyValue;
            SetCounterColor(pendingFoundForCurrentRun ? FoundCounterColor : NormalCounterColor);
            AddLog($"Detailed found updated for active run: hi {pendingFoundInfo.Hi}, mid {pendingFoundInfo.Mid}, low {pendingFoundInfo.Low}");
        }

        private static void ApplyFoundDelta(RunTurn runEntry, FoundTier tier, int delta, string note)
        {
            switch (tier)
            {
                case FoundTier.High:
                    runEntry.FoundHi += delta;
                    break;
                case FoundTier.Mid:
                    runEntry.FoundMid += delta;
                    break;
                case FoundTier.Low:
                    runEntry.FoundLow += delta;
                    break;
            }

            runEntry.Found = runEntry.HasDetailedFound || runEntry.Found;
            if (!string.IsNullOrWhiteSpace(note))
            {
                runEntry.FoundNote = note;
            }
        }

        private static void ApplyFoundDelta(FoundInfo foundInfo, FoundTier tier, int delta, string note)
        {
            switch (tier)
            {
                case FoundTier.High:
                    foundInfo.Hi += delta;
                    break;
                case FoundTier.Mid:
                    foundInfo.Mid += delta;
                    break;
                case FoundTier.Low:
                    foundInfo.Low += delta;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                foundInfo.Note = note;
            }
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
            var sessionDuration = GetSessionDuration(activeRun, includeCurrentRun: activeRun is not null);
            var averageRunDuration = GetAverageRunDuration(activeRun);
            var currentRunDuration = GetCurrentRunDuration();

            labelRunDuration.Text = FormatDuration(sessionDuration);
            labelTurnAverage.Text = FormatDuration(averageRunDuration);
            labelTurnDuration.Text = FormatDuration(currentRunDuration);
            labelTurnDuration.ForeColor = averageRunDuration == TimeSpan.Zero || currentRunDuration <= averageRunDuration
                ? Color.ForestGreen
                : Color.Firebrick;

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

        private TimeSpan GetCurrentRunDuration()
        {
            if (activeRun is null || !currentTurnStartedAt.HasValue)
            {
                return TimeSpan.Zero;
            }

            var now = currentPauseStartedAt ?? DateTime.Now;
            return ClampDuration(now - currentTurnStartedAt.Value - GetCurrentRunPausedTime());
        }

        private TimeSpan GetCurrentRunPausedTime()
        {
            var pausedTime = currentRunPausedTime;
            if (currentPauseStartedAt.HasValue)
            {
                pausedTime += DateTime.Now - currentPauseStartedAt.Value;
            }

            return ClampDuration(pausedTime);
        }

        private TimeSpan GetSessionDuration(RunSession? run, bool includeCurrentRun)
        {
            if (run is null)
            {
                return TimeSpan.Zero;
            }

            var duration = TimeSpan.FromTicks(run.Turns.Sum(turn => GetRunEffectiveDuration(turn).Ticks));
            if (includeCurrentRun)
            {
                duration += GetCurrentRunDuration();
            }

            return duration;
        }

        private static TimeSpan GetAverageRunDuration(RunSession? run)
        {
            if (run is null || run.Turns.Count == 0)
            {
                return TimeSpan.Zero;
            }

            return TimeSpan.FromTicks((long)run.Turns.Average(turn => GetRunEffectiveDuration(turn).Ticks));
        }

        private static TimeSpan GetSessionRealDuration(RunSession run)
        {
            var end = run.EndedAt ?? DateTime.Now;
            return ClampDuration(end - run.StartedAt);
        }

        private static TimeSpan GetRunEffectiveDuration(RunTurn turn)
        {
            return turn.EffectiveDuration;
        }

        private static TimeSpan ClampDuration(TimeSpan duration)
        {
            return duration < TimeSpan.Zero ? TimeSpan.Zero : duration;
        }

        private static string FormatDuration(TimeSpan duration)
        {
            return duration.TotalHours >= 1
                ? $"{(int)duration.TotalHours:00}:{duration.Minutes:00}:{duration.Seconds:00}"
                : $"{duration.Minutes:00}:{duration.Seconds:00}";
        }

        private static string FormatFoundCount(int value)
        {
            return value == 0 ? "" : value.ToString();
        }

        private void RefreshRunList()
        {
            var selectedId = GetSelectedRun()?.Id ?? activeRun?.Id;
            listView2.BeginUpdate();
            listView2.Items.Clear();

            foreach (var run in database.Runs.OrderByDescending(run => run.StartedAt))
            {
                var item = new ListViewItem(string.IsNullOrWhiteSpace(run.CurrentCharacter) ? "Unknown" : run.CurrentCharacter);
                item.SubItems.Add(run.Name);
                item.SubItems.Add(run.TotalRuns.ToString());
                item.SubItems.Add(FormatDuration(run.CompletedRunDuration));
                item.SubItems.Add(FormatDuration(GetSessionRealDuration(run)));
                item.SubItems.Add(run.TotalFound.ToString());
                item.SubItems.Add((run.EndedAt ?? run.StartedAt).ToString("dd.MM.yyyy HH:mm:ss"));
                item.SubItems.Add(run.ManualModifier == 0 ? "" : run.ManualModifier.ToString("+#;-#;0"));
                item.Tag = run;
                listView2.Items.Add(item);
            }

            if (selectedId.HasValue)
            {
                foreach (ListViewItem item in listView2.Items)
                {
                    if (item.Tag is RunSession run && run.Id == selectedId.Value)
                    {
                        item.Selected = true;
                        item.Focused = true;
                        item.EnsureVisible();
                        break;
                    }
                }
            }

            listView2.EndUpdate();
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
                    item.SubItems.Add(FormatDuration(GetRunEffectiveDuration(turn)));
                    item.SubItems.Add(FormatDuration(TimeSpan.FromSeconds(turn.PausedSeconds)));
                    item.SubItems.Add(turn.Character);
                    item.SubItems.Add(turn.Found ? "Yes" : "");
                    item.SubItems.Add(FormatFoundCount(turn.FoundHi));
                    item.SubItems.Add(FormatFoundCount(turn.FoundMid));
                    item.SubItems.Add(FormatFoundCount(turn.FoundLow));
                    item.SubItems.Add(turn.FoundNote);
                    item.Tag = turn.Number;
                    if (turn.Found)
                    {
                        item.ForeColor = FoundCounterColor;
                    }

                    listView1.Items.Add(item);
                }
            }

            listView1.EndUpdate();
        }

        private void listView2_SelectedIndexChanged(object? sender, EventArgs e)
        {
            RefreshDetails();
        }

        private void listView2_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var item = listView2.GetItemAt(e.X, e.Y);
            if (item is not null)
            {
                item.Selected = true;
                item.Focused = true;
            }
        }

        private void listView1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var item = listView1.GetItemAt(e.X, e.Y);
            if (item is not null)
            {
                item.Selected = true;
                item.Focused = true;
            }
        }

        private RunSession? GetSelectedRun()
        {
            return listView2.SelectedItems.Count == 0
                ? null
                : listView2.SelectedItems[0].Tag as RunSession;
        }

        private RunTurn? GetSelectedRunEntry()
        {
            var session = GetSelectedRun() ?? activeRun;
            if (session is null || listView1.SelectedItems.Count == 0)
            {
                return null;
            }

            var number = listView1.SelectedItems[0].Tag is int taggedNumber
                ? taggedNumber
                : int.TryParse(listView1.SelectedItems[0].Text, out var parsedNumber) ? parsedNumber : -1;

            return session.Turns.FirstOrDefault(turn => turn.Number == number);
        }

        private void AddLog(string text)
        {
            listBox1.Items.Insert(0, $"{DateTime.Now:HH:mm:ss} {text}");
        }

        private void editPauseToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (GetSelectedRunEntry() is not { } runEntry)
            {
                return;
            }

            var input = Prompt.Show("Enter pause duration in seconds:", "Edit Pause Duration", runEntry.PausedSeconds.ToString());
            if (!long.TryParse(input, out var pausedSeconds) || pausedSeconds < 0)
            {
                AddLog("Pause duration was not changed. Enter a non-negative number.");
                return;
            }

            runEntry.PausedSeconds = pausedSeconds;
            SaveRuns();
            RefreshAll();
        }

        private void removeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var session = GetSelectedRun() ?? activeRun;
            var runEntry = GetSelectedRunEntry();
            if (session is null || runEntry is null)
            {
                return;
            }

            var result = MessageBox.Show($"Remove run #{runEntry.Number}?", "Remove Run", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
            {
                return;
            }

            session.Turns.Remove(runEntry);
            RenumberRuns(session);
            session.EndedAt = session.Turns.LastOrDefault()?.FinishedAt;
            SaveRuns();
            RefreshAll();
        }

        private void markFoundToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            SetSelectedRunFound(true);
        }

        private void removeFoundToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            SetSelectedRunFound(false);
        }

        private void SetSelectedRunFound(bool found)
        {
            if (GetSelectedRunEntry() is not { } runEntry)
            {
                return;
            }

            runEntry.Found = found;
            if (!found)
            {
                runEntry.FoundHi = 0;
                runEntry.FoundMid = 0;
                runEntry.FoundLow = 0;
                runEntry.FoundNote = "";
            }

            SaveRuns();
            RefreshAll();
        }

        private static void RenumberRuns(RunSession session)
        {
            for (var i = 0; i < session.Turns.Count; i++)
            {
                session.Turns[i].Number = i + 1;
            }
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

    public enum FoundTier
    {
        High,
        Mid,
        Low
    }

    public sealed class FoundInfo
    {
        public int Hi { get; set; }
        public int Mid { get; set; }
        public int Low { get; set; }
        public string Note { get; set; } = "";
        public bool HasAnyValue => Hi != 0 || Mid != 0 || Low != 0 || !string.IsNullOrWhiteSpace(Note);
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
        public long CurrentRunPausedSeconds { get; set; }
        public DateTime? CurrentPauseStartedAt { get; set; }
        public List<PausePeriod> CurrentRunPausePeriods { get; set; } = new();
        public List<RunTurn> Turns { get; set; } = new();
        public int TotalRuns => Turns.Count + ManualModifier;
        public int TotalFound => Turns.Count(turn => turn.Found);
        public TimeSpan CompletedRunDuration => TimeSpan.FromTicks(Turns.Sum(turn => turn.EffectiveDuration.Ticks));

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
                CurrentRunPausedSeconds = CurrentRunPausedSeconds,
                CurrentPauseStartedAt = CurrentPauseStartedAt,
                CurrentRunPausePeriods = CurrentRunPausePeriods.Select(period => period.Clone()).ToList(),
                Turns = Turns.Select(turn => turn.Clone()).ToList()
            };
        }
    }

    public sealed class RunTurn
    {
        public int Number { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public long PausedSeconds { get; set; }
        public List<PausePeriod> PausePeriods { get; set; } = new();
        public string Character { get; set; } = "Unknown";
        public bool Found { get; set; }
        public int FoundHi { get; set; }
        public int FoundMid { get; set; }
        public int FoundLow { get; set; }
        public string FoundNote { get; set; } = "";
        public bool HasDetailedFound => FoundHi != 0 || FoundMid != 0 || FoundLow != 0 || !string.IsNullOrWhiteSpace(FoundNote);
        public TimeSpan EffectiveDuration
        {
            get
            {
                if (StartedAt == default || FinishedAt < StartedAt)
                {
                    return TimeSpan.Zero;
                }

                var duration = FinishedAt - StartedAt - TimeSpan.FromSeconds(PausedSeconds);
                return duration < TimeSpan.Zero ? TimeSpan.Zero : duration;
            }
        }

        public RunTurn Clone()
        {
            return new RunTurn
            {
                Number = Number,
                StartedAt = StartedAt,
                FinishedAt = FinishedAt,
                PausedSeconds = PausedSeconds,
                PausePeriods = PausePeriods.Select(period => period.Clone()).ToList(),
                Character = Character,
                Found = Found,
                FoundHi = FoundHi,
                FoundMid = FoundMid,
                FoundLow = FoundLow,
                FoundNote = FoundNote
            };
        }
    }

    public sealed class PausePeriod
    {
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public TimeSpan Duration => EndedAt >= StartedAt ? EndedAt - StartedAt : TimeSpan.Zero;

        public PausePeriod Clone()
        {
            return new PausePeriod
            {
                StartedAt = StartedAt,
                EndedAt = EndedAt
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
            var duration = Run.CompletedRunDuration;
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
