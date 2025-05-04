using System;
using System.Drawing;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;


namespace AiVoice
{
    public partial class Form1 : Form
    {
        TextBox txtInput;
        RichTextBox txtOutput;
        Button btnMic, btnAsk;
        CheckBox chkReadAloud;
        ComboBox cmbLanguages;
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

        private Dictionary<string, List<string>> languageKeywords = new Dictionary<string, List<string>>()
        {
            ["C#"] = new List<string> { "public", "class", "void", "int", "string", "return", "new", "if", "else", "using" },
            ["Java"] = new List<string> { "public", "class", "void", "int", "String", "return", "new", "if", "else", "import" },
            ["Python"] = new List<string> { "def", "class", "return", "import", "as", "if", "else", "elif", "for", "while" },
            ["JavaScript"] = new List<string> { "function", "let", "const", "var", "return", "if", "else", "class", "import", "export" },
            ["HTML"] = new List<string> { "<html>", "<head>", "<body>", "<div>", "<span>", "<script>", "<style>", "<p>", "<a>", "<h1>" }
            // You can add more languages here
        };
        public Form1()
        {
            InitializeComponent();
            InitializeUI();
            InitializeSpeechRecognition();
        }

        private void InitializeUI()
        {
            this.Text = "TCU AI Assistant";
            this.BackColor = Color.FromArgb(0, 51, 102);
            this.Size = new Size(920, 720);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            Label lblTitle = new Label()
            {
                Text = "CodexTension",
                Font = new Font("Segoe UI", 25, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            Label lblTitle2 = new Label()
            {
                Text = "Tension",
                Font = new Font("Segoe UI", 25, FontStyle.Bold),
                ForeColor = Color.Red,
                Location = new Point(100, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle2);

            chkReadAloud = new CheckBox()
            {
                Text = "Read Aloud",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                Location = new Point(750, 40),
                AutoSize = true
            };
            this.Controls.Add(chkReadAloud);
            // Dropdown label
            Label lblLang = new Label()
            {
                Text = "Select Programming Language:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                Location = new Point(20, 500),
                AutoSize = true
            };
            this.Controls.Add(lblLang);

            // ComboBox
            cmbLanguages = new ComboBox()
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(250, 500),
                Size = new Size(200, 50),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            string[] languages = new string[]
            {
                "Ada", "Assembly", "Bash", "C", "C#", "C++", "COBOL", "D", "Dart", "Delphi", "Elixir", "Erlang", "F#", "Fortran",
                "Go", "Groovy", "Haskell", "HTML", "Java", "JavaScript", "Julia", "Kotlin", "Lisp", "Lua", "MATLAB",
                "Objective-C", "Pascal", "Perl", "PHP", "PowerShell", "Prolog", "Python", "R", "Ruby", "Rust",
                "Scala", "Shell", "SQL", "Swift", "TypeScript", "VB.NET", "Visual Basic"
            };
            cmbLanguages.Items.AddRange(languages.OrderBy(l => l).ToArray());
            cmbLanguages.SelectedIndex = 0;
            this.Controls.Add(cmbLanguages);


            txtOutput = new RichTextBox()
            {
                ReadOnly = true,
                Font = new Font("Consolas", 10), // Monospaced font
                Size = new Size(860, 400),
                Location = new Point(20, 80),
                BackColor = Color.Black,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true

            };
            this.Controls.Add(txtOutput);


            // Input Label
            Label inputLbl = new Label()
            {
                Text = "User Input:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                Location = new Point(20, 550),
                AutoSize = true
            };
            this.Controls.Add(inputLbl);

            // Input TextBox
            txtInput = new TextBox()
            {
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                Size = new Size(670, 30),
                Location = new Point(20, 580),
                BackColor = Color.WhiteSmoke,
                ForeColor = Color.Black
            };
            this.Controls.Add(txtInput);

            // Mic Button
            btnMic = new Button()
            {
                Size = new Size(50, 30),
                Location = new Point(700, 580),
                BackColor = Color.FromArgb(198, 40, 40),
                FlatStyle = FlatStyle.Flat,
                Text = "🎤" // Use emoji if you don't have image
                // Image = Properties.Resources.mic; // Or set your own image if available
            };
            btnMic.Click += BtnMic_Click;
            this.Controls.Add(btnMic);

            // Ask Button
            btnAsk = new Button()
            {
                Text = "Ask",
                Font = new Font("Segoe UI", 10),
                Size = new Size(60, 30),
                Location = new Point(760, 580),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAsk.Click += BtnAsk_Click;
            this.Controls.Add(btnAsk);
        }

        private void InitializeSpeechRecognition()
        {
            try
            {
                synthesizer.Rate = 0;
                synthesizer.Volume = 100;
                synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);

                recognizer.SetInputToDefaultAudioDevice();
                recognizer.LoadGrammar(new DictationGrammar());
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Recognizer_SpeechRecognized);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing speech recognition: " + ex.Message);
            }
        }

        private void BtnMic_Click(object sender, EventArgs e)
        {
            txtInput.Text = "Listening...";
            btnMic.BackColor = Color.Green;
            recognizer.RecognizeAsync(RecognizeMode.Single);
        }

        private async void BtnAsk_Click(object sender, EventArgs e)
        {
            string userInput = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userInput)) return;

            string selectedLanguage = cmbLanguages.SelectedItem.ToString();

            string prompt = $"Using {selectedLanguage}, answer this: {userInput}";
            string aiResponse = await GeminiClass.GetGeminiResponse(prompt, selectedLanguage);

            txtOutput.AppendText($"\n\nYour question is: {userInput}");
            txtOutput.AppendText($"\nSelected Language: {selectedLanguage}\n");
            txtOutput.AppendText($"\nAI Response:\n");

            int startIndex = txtOutput.TextLength;

            // Format response like a code block
            txtOutput.AppendText($"```{selectedLanguage}\n{aiResponse}\n```\n");

            // Optional: Highlight keywords for C# (you can expand per language)
            if (languageKeywords.ContainsKey(selectedLanguage))
            {
                HighlightKeywords(languageKeywords[selectedLanguage], Color.DeepSkyBlue, startIndex);
            }

            if (chkReadAloud.Checked)
            {
                synthesizer.SelectVoiceByHints(VoiceGender.Male);
                synthesizer.SpeakAsync(aiResponse);
            }

            txtInput.Clear();
        }

        private void HighlightKeywords(List<string> keywords, Color color, int searchStart)
        {
            foreach (var keyword in keywords)
            {
                int startIndex = searchStart;
                while (startIndex < txtOutput.TextLength)
                {
                    int wordStartIndex = txtOutput.Find(keyword, startIndex, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
                    if (wordStartIndex != -1)
                    {
                        txtOutput.Select(wordStartIndex, keyword.Length);
                        txtOutput.SelectionColor = color;
                        startIndex = wordStartIndex + keyword.Length;
                    }
                    else break;
                }
            }
            txtOutput.SelectionLength = 0;
            txtOutput.SelectionColor = Color.White;
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            txtInput.Text = e.Result.Text;
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }
}
