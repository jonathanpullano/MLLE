using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MLLE
{
    public partial class HotkeysDialog : Form
    {
        private Dictionary<string, HotkeyInfo> hotkeys = new Dictionary<string, HotkeyInfo>();
        private Dictionary<string, HotkeyInfo> defaultHotkeys = new Dictionary<string, HotkeyInfo>();
        private DataGridView hotkeyGrid;
        private Button resetButton;
        private Button okButton;
        private Button cancelButton;

        public class HotkeyInfo
        {
            public string Action { get; set; }
            public Keys Key { get; set; }
            public string KeyDisplay { get { return GetKeyDisplay(); } }

            private string GetKeyDisplay()
            {
                if (Key == Keys.None) return "";

                string result = "";
                if ((Key & Keys.Control) == Keys.Control) result += "Ctrl+";
                if ((Key & Keys.Alt) == Keys.Alt) result += "Alt+";
                if ((Key & Keys.Shift) == Keys.Shift) result += "Shift+";

                Keys keyCode = Key & Keys.KeyCode;
                if (keyCode != Keys.None)
                {
                    result += keyCode.ToString();
                }

                return result;
            }
        }

        public HotkeysDialog()
        {
            InitializeComponent();
            LoadDefaultHotkeys();
            LoadCurrentHotkeys();
            PopulateGrid();
        }

        private void InitializeComponent()
        {
            this.Text = "Keyboard Shortcuts";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 3;
            mainLayout.ColumnCount = 1;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            // Header label
            Label headerLabel = new Label();
            headerLabel.Text = "MLLE Keyboard Shortcuts - Click on a shortcut to rebind it";
            headerLabel.Dock = DockStyle.Fill;
            headerLabel.TextAlign = ContentAlignment.MiddleLeft;
            headerLabel.Font = new Font(headerLabel.Font, FontStyle.Bold);
            headerLabel.Padding = new Padding(10, 0, 0, 0);
            mainLayout.Controls.Add(headerLabel, 0, 0);

            // DataGridView for hotkeys
            hotkeyGrid = new DataGridView();
            hotkeyGrid.Dock = DockStyle.Fill;
            hotkeyGrid.AllowUserToAddRows = false;
            hotkeyGrid.AllowUserToDeleteRows = false;
            hotkeyGrid.AllowUserToResizeRows = false;
            hotkeyGrid.RowHeadersVisible = false;
            hotkeyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            hotkeyGrid.MultiSelect = false;
            hotkeyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            hotkeyGrid.BackgroundColor = SystemColors.Window;
            hotkeyGrid.BorderStyle = BorderStyle.Fixed3D;
            hotkeyGrid.CellDoubleClick += HotkeyGrid_CellDoubleClick;

            // Add columns
            DataGridViewTextBoxColumn actionColumn = new DataGridViewTextBoxColumn();
            actionColumn.HeaderText = "Action";
            actionColumn.Name = "Action";
            actionColumn.ReadOnly = true;
            actionColumn.FillWeight = 70;

            DataGridViewTextBoxColumn shortcutColumn = new DataGridViewTextBoxColumn();
            shortcutColumn.HeaderText = "Shortcut";
            shortcutColumn.Name = "Shortcut";
            shortcutColumn.ReadOnly = true;
            shortcutColumn.FillWeight = 30;

            hotkeyGrid.Columns.Add(actionColumn);
            hotkeyGrid.Columns.Add(shortcutColumn);

            mainLayout.Controls.Add(hotkeyGrid, 0, 1);

            // Button panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 10, 10, 10);

            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Size = new Size(75, 25);
            cancelButton.Click += CancelButton_Click;
            buttonPanel.Controls.Add(cancelButton);

            okButton = new Button();
            okButton.Text = "OK";
            okButton.Size = new Size(75, 25);
            okButton.Click += OkButton_Click;
            buttonPanel.Controls.Add(okButton);

            resetButton = new Button();
            resetButton.Text = "Reset to Defaults";
            resetButton.Size = new Size(120, 25);
            resetButton.Click += ResetButton_Click;
            buttonPanel.Controls.Add(resetButton);

            mainLayout.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Add(mainLayout);
        }

        private void LoadDefaultHotkeys()
        {
            // File menu hotkeys
            defaultHotkeys["New Level"] = new HotkeyInfo { Action = "New Level", Key = Keys.Control | Keys.N };
            defaultHotkeys["Open Level"] = new HotkeyInfo { Action = "Open Level", Key = Keys.Control | Keys.O };
            defaultHotkeys["Save Level"] = new HotkeyInfo { Action = "Save Level", Key = Keys.Control | Keys.S };
            defaultHotkeys["Save and Run"] = new HotkeyInfo { Action = "Save and Run", Key = Keys.Control | Keys.R };

            // Edit menu hotkeys
            defaultHotkeys["Undo"] = new HotkeyInfo { Action = "Undo", Key = Keys.Control | Keys.Z };
            defaultHotkeys["Redo"] = new HotkeyInfo { Action = "Redo", Key = Keys.Control | Keys.Y };
            defaultHotkeys["Select All"] = new HotkeyInfo { Action = "Select All", Key = Keys.Control | Keys.A };
            defaultHotkeys["Clear Selection"] = new HotkeyInfo { Action = "Clear Selection", Key = Keys.Escape };
            defaultHotkeys["Cut"] = new HotkeyInfo { Action = "Cut", Key = Keys.Control | Keys.X };
            defaultHotkeys["Copy"] = new HotkeyInfo { Action = "Copy", Key = Keys.Control | Keys.C };
            defaultHotkeys["Paste"] = new HotkeyInfo { Action = "Paste", Key = Keys.Control | Keys.V };
            defaultHotkeys["Paste Once"] = new HotkeyInfo { Action = "Paste Once", Key = Keys.Control | Keys.Shift | Keys.V };

            // View menu hotkeys
            defaultHotkeys["Zoom In"] = new HotkeyInfo { Action = "Zoom In", Key = Keys.Add };
            defaultHotkeys["Zoom Out"] = new HotkeyInfo { Action = "Zoom Out", Key = Keys.Subtract };
            defaultHotkeys["100% Zoom"] = new HotkeyInfo { Action = "100% Zoom", Key = Keys.NumPad0 };

            // Layer hotkeys
            defaultHotkeys["Layer 1"] = new HotkeyInfo { Action = "Layer 1", Key = Keys.D1 };
            defaultHotkeys["Layer 2"] = new HotkeyInfo { Action = "Layer 2", Key = Keys.D2 };
            defaultHotkeys["Layer 3"] = new HotkeyInfo { Action = "Layer 3", Key = Keys.D3 };
            defaultHotkeys["Layer 4"] = new HotkeyInfo { Action = "Layer 4", Key = Keys.D4 };
            defaultHotkeys["Layer 5"] = new HotkeyInfo { Action = "Layer 5", Key = Keys.D5 };
            defaultHotkeys["Layer 6"] = new HotkeyInfo { Action = "Layer 6", Key = Keys.D6 };
            defaultHotkeys["Layer 7"] = new HotkeyInfo { Action = "Layer 7", Key = Keys.D7 };
            defaultHotkeys["Layer 8"] = new HotkeyInfo { Action = "Layer 8", Key = Keys.D8 };
            defaultHotkeys["All Layers"] = new HotkeyInfo { Action = "All Layers", Key = Keys.D0 };

            // Tool hotkeys
            defaultHotkeys["Select Tool"] = new HotkeyInfo { Action = "Select Tool", Key = Keys.S };
            defaultHotkeys["Draw Tool"] = new HotkeyInfo { Action = "Draw Tool", Key = Keys.D };
            defaultHotkeys["Eyedropper Tool"] = new HotkeyInfo { Action = "Eyedropper Tool", Key = Keys.E };
            defaultHotkeys["Fill Tool"] = new HotkeyInfo { Action = "Fill Tool", Key = Keys.F };
            defaultHotkeys["Rectangle Tool"] = new HotkeyInfo { Action = "Rectangle Tool", Key = Keys.R };

            // Other useful hotkeys
            defaultHotkeys["Toggle Grid"] = new HotkeyInfo { Action = "Toggle Grid", Key = Keys.G };
            defaultHotkeys["Toggle Mask Mode"] = new HotkeyInfo { Action = "Toggle Mask Mode", Key = Keys.M };
            defaultHotkeys["Toggle Event Mode"] = new HotkeyInfo { Action = "Toggle Event Mode", Key = Keys.V };
            defaultHotkeys["Toggle Tile Type Mode"] = new HotkeyInfo { Action = "Toggle Tile Type Mode", Key = Keys.T };
            defaultHotkeys["Smart Tiles"] = new HotkeyInfo { Action = "Smart Tiles", Key = Keys.B };
            defaultHotkeys["Test Level from Start"] = new HotkeyInfo { Action = "Test Level from Start", Key = Keys.F5 };
            defaultHotkeys["Add Selection"] = new HotkeyInfo { Action = "Add Selection", Key = Keys.Shift };
            defaultHotkeys["Subtract Selection"] = new HotkeyInfo { Action = "Subtract Selection", Key = Keys.Alt };
        }

        private void LoadCurrentHotkeys()
        {
            // Initially set to defaults, but this would load from settings in a real implementation
            foreach (var kvp in defaultHotkeys)
            {
                hotkeys[kvp.Key] = new HotkeyInfo { Action = kvp.Value.Action, Key = kvp.Value.Key };
            }
        }

        private void PopulateGrid()
        {
            hotkeyGrid.Rows.Clear();

            var sortedHotkeys = hotkeys.OrderBy(h => h.Value.Action);
            foreach (var hotkey in sortedHotkeys)
            {
                hotkeyGrid.Rows.Add(hotkey.Value.Action, hotkey.Value.KeyDisplay);
            }
        }

        private void HotkeyGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string action = hotkeyGrid.Rows[e.RowIndex].Cells[0].Value.ToString();

            using (HotkeyInputDialog inputDialog = new HotkeyInputDialog(action))
            {
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    // Check for conflicts
                    var conflict = hotkeys.FirstOrDefault(h =>
                        h.Value.Key == inputDialog.NewKey && h.Key != action);

                    if (conflict.Value != null)
                    {
                        DialogResult result = MessageBox.Show(
                            $"The key combination '{new HotkeyInfo { Key = inputDialog.NewKey }.KeyDisplay}' is already assigned to '{conflict.Value.Action}'.\n\n" +
                            "Do you want to reassign it?",
                            "Hotkey Conflict",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            // Clear the conflicting hotkey
                            hotkeys[conflict.Key].Key = Keys.None;
                        }
                        else
                        {
                            return;
                        }
                    }

                    hotkeys[action].Key = inputDialog.NewKey;
                    PopulateGrid();
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to reset all keyboard shortcuts to their default values?",
                "Reset Shortcuts",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LoadCurrentHotkeys();
                PopulateGrid();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            // Save hotkeys to settings here
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public Dictionary<string, HotkeyInfo> GetHotkeys()
        {
            return hotkeys;
        }
    }

    // Helper dialog for capturing hotkey input
    public class HotkeyInputDialog : Form
    {
        public Keys NewKey { get; private set; }
        private Label instructionLabel;
        private Label currentKeyLabel;
        private Button okButton;
        private Button cancelButton;
        private Button clearButton;

        public HotkeyInputDialog(string action)
        {
            this.Text = $"Set Hotkey for: {action}";
            this.Size = new Size(350, 180);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.KeyPreview = true;

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.RowCount = 3;
            layout.ColumnCount = 1;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layout.Padding = new Padding(10);

            instructionLabel = new Label();
            instructionLabel.Text = "Press the key combination you want to use:";
            instructionLabel.Dock = DockStyle.Fill;
            instructionLabel.TextAlign = ContentAlignment.MiddleCenter;
            layout.Controls.Add(instructionLabel, 0, 0);

            currentKeyLabel = new Label();
            currentKeyLabel.Text = "None";
            currentKeyLabel.Dock = DockStyle.Fill;
            currentKeyLabel.TextAlign = ContentAlignment.MiddleCenter;
            currentKeyLabel.Font = new Font(currentKeyLabel.Font.FontFamily, 12, FontStyle.Bold);
            currentKeyLabel.BorderStyle = BorderStyle.Fixed3D;
            layout.Controls.Add(currentKeyLabel, 0, 1);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;

            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Size = new Size(75, 25);
            cancelButton.Click += (s, e) => {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            buttonPanel.Controls.Add(cancelButton);

            okButton = new Button();
            okButton.Text = "OK";
            okButton.Size = new Size(75, 25);
            okButton.Enabled = false;
            okButton.Click += (s, e) => {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            buttonPanel.Controls.Add(okButton);

            clearButton = new Button();
            clearButton.Text = "Clear";
            clearButton.Size = new Size(75, 25);
            clearButton.Click += (s, e) => {
                NewKey = Keys.None;
                currentKeyLabel.Text = "None";
                okButton.Enabled = true;
            };
            buttonPanel.Controls.Add(clearButton);

            layout.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Add(layout);

            this.KeyDown += HotkeyInputDialog_KeyDown;
        }

        private void HotkeyInputDialog_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;

            // Ignore modifier keys alone
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey ||
                e.KeyCode == Keys.Menu || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                return;
            }

            NewKey = e.KeyData;

            var info = new HotkeysDialog.HotkeyInfo { Key = NewKey };
            currentKeyLabel.Text = info.KeyDisplay;
            okButton.Enabled = true;
        }
    }
}