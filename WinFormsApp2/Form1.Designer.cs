using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
        }

        #endregion
    }
    public class MirrorImageApp : Form1
    {
        private Button processButton;
        private Button selectFolderButton;
        private Label statusLabel;
        private string selectedFolder;

        public MirrorImageApp()
        {
            Text = "Mirror Images Processor";
            Size = new Size(500, 250);

            selectFolderButton = new Button
            {
                Text = "Select Folder",
                Location = new Point(50, 50),
                Size = new Size(120, 40)
            };
            selectFolderButton.Click += SelectFolderButton_Click;

            processButton = new Button
            {
                Text = "Process Images",
                Location = new Point(200, 50),
                Size = new Size(120, 40),
                Enabled = false
            };
            processButton.Click += ProcessButton_Click;

            // Метка статусу
            statusLabel = new Label
            {
                Text = "Please select a folder to start processing.",
                Location = new Point(20, 150),
                Size = new Size(450, 20)
            };

            Controls.Add(selectFolderButton);
            Controls.Add(processButton);
            Controls.Add(statusLabel);
        }

        private void SelectFolderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFolder = folderDialog.SelectedPath;
                    statusLabel.Text = $"Selected folder: {selectedFolder}";
                    processButton.Enabled = true;
                }
            }
        }

        private void ProcessButton_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(selectedFolder);

            Regex regexExtForImage = new Regex(@"^((bmp)|(gif)|(tiff?)|(jpe?g)|(png))$", RegexOptions.IgnoreCase);

            int processedCount = 0;

            foreach (var file in files)
            {
                try
                {
                    string extension = Path.GetExtension(file).TrimStart('.').ToLower();
                    if (!regexExtForImage.IsMatch(extension))
                    {
                        // пропустити файл, якщо розширення не таке
                        continue;
                    }

                    // спроба завантажити файл
                    using (Bitmap bitmap = new Bitmap(file))
                    {
                        bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                        string newFileName = Path.Combine(
                            selectedFolder,
                            Path.GetFileNameWithoutExtension(file) + "-mirrored.gif"
                        );

                        bitmap.Save(newFileName, System.Drawing.Imaging.ImageFormat.Gif);
                        processedCount++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"The file \"{Path.GetFileName(file)}\" is not a valid image or could not be processed.\n\nError: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }

            statusLabel.Text = $"Processed {processedCount} images.";
            MessageBox.Show("Processing complete!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}