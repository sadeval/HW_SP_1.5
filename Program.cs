using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

public class ProcessViewer : Form
{
    private ListBox processListBox;
    private Button refreshButton;
    private Timer refreshTimer;
    private Label detailsLabel;

    public ProcessViewer()
    {
        Text = "Process Info Viewer";
        Size = new System.Drawing.Size(800, 600);

        processListBox = new ListBox { Dock = DockStyle.Left, Width = 400 };
        refreshButton = new Button { Text = "Обновить", Dock = DockStyle.Top };
        detailsLabel = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            Text = "Выберите процесс для отображения деталей."
        };

        Controls.Add(detailsLabel);
        Controls.Add(refreshButton);
        Controls.Add(processListBox);

        refreshButton.Click += RefreshButton_Click;
        processListBox.SelectedIndexChanged += ProcessListBox_SelectedIndexChanged;

        refreshTimer = new Timer { Interval = 5000 };
        refreshTimer.Tick += RefreshTimer_Tick;
        refreshTimer.Start();

        RefreshProcessList();
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
        RefreshProcessList();
    }

    private void RefreshTimer_Tick(object sender, EventArgs e)
    {
        RefreshProcessList();
    }

    private void RefreshProcessList()
    {
        processListBox.Items.Clear();
        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            processListBox.Items.Add($"{process.ProcessName} (ID: {process.Id})");
        }
    }

    private void ProcessListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (processListBox.SelectedItem != null)
        {
            string selectedProcessInfo = processListBox.SelectedItem.ToString();
            int processId = int.Parse(selectedProcessInfo.Split(new[] { " (ID: " }, StringSplitOptions.None)[1].TrimEnd(')'));

            try
            {
                var process = Process.GetProcessById(processId);
                var processStartTime = process.StartTime.ToString();
                var totalProcessorTime = process.TotalProcessorTime.ToString();
                var threadCount = process.Threads.Count;
                var sameProcessesCount = Process.GetProcessesByName(process.ProcessName).Length;

                detailsLabel.Text = $"Идентификатор процесса: {processId}\n" +
                                    $"Время старта: {processStartTime}\n" +
                                    $"Общее количество процессорного времени: {totalProcessorTime}\n" +
                                    $"Количество потоков: {threadCount}\n" +
                                    $"Количество копий процесса: {sameProcessesCount}";
            }
            catch (Exception ex)
            {
                detailsLabel.Text = $"Ошибка: {ex.Message}";
            }
        }
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new ProcessViewer());
    }
}
