using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PascalC3D.Compilacion.Analizador;
using ScintillaNET;

namespace PascalC3D
{
    public partial class Form1 : Form
    {
        public Scintilla TextArea;
        public Scintilla TextConsola;

        public static Scintilla consola;  //PARA ACCEDER DESDE AFUERA
        public static Scintilla txarea;   //PARA ACCEDER DESDE AFUERA
        
        public Form1()
        {
            InitializeComponent();
        }
        
        private void btncompilar_Click(object sender, EventArgs e)
        {
            consola.Clear();
            Sintactico.analizar(TextArea.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // CREATE CONTROL
            TextArea = new Scintilla();
            TextConsola = new Scintilla();
            txarea = TextArea;
            consola = TextConsola;
            TextPanel.Controls.Add(TextArea);
            TextPanel2.Controls.Add(TextConsola);

            // BASIC CONFIG
            TextArea.Dock = System.Windows.Forms.DockStyle.Fill;
            TextArea.TextChanged += (this.OnTextChanged);
            TextConsola.Dock = System.Windows.Forms.DockStyle.Fill;
            TextConsola.TextChanged += (this.OnTextChanged);


            // INITIAL VIEW CONFIG
            TextArea.WrapMode = ScintillaNET.WrapMode.None;
            TextArea.IndentationGuides = IndentView.LookBoth;
            TextConsola.WrapMode = ScintillaNET.WrapMode.None;
            TextConsola.IndentationGuides = IndentView.LookBoth;

            // STYLING
            InitColors();
            InitSyntaxColoring();
            
            // NUMBER MARGIN
            InitNumberMargin();
            
        }

        /**************************************************************Aqui empieza el Scintilla**************************************************************/
        
        private void InitColors()
        {
            TextArea.SetSelectionBackColor(true, IntToColor(0x114D9C));
            TextConsola.SetSelectionBackColor(true, IntToColor(0x114D9C));
        }

        private void InitSyntaxColoring()
        {

            // Configure the default style
            TextArea.StyleResetDefault();
            TextArea.Styles[Style.Default].Font = "Consolas";
            TextArea.Styles[Style.Default].Size = 14;
            TextArea.Styles[Style.Default].BackColor = Color.White; //IntToColor(0x212121);
            TextArea.Styles[Style.Default].ForeColor = Color.Black; //IntToColor(0xFFFFFF);
            TextArea.StyleClearAll();

            // Configure the Pascal (Pascal) lexer styles
            TextArea.Styles[Style.Pascal.Identifier].ForeColor = Color.Black;
            TextArea.Styles[Style.Pascal.Comment].ForeColor = Color.Brown;
            TextArea.Styles[Style.Pascal.Comment2].ForeColor = Color.RosyBrown;
            TextArea.Styles[Style.Pascal.CommentLine].ForeColor = Color.Green;
            TextArea.Styles[Style.Pascal.Number].ForeColor = Color.DarkOrange;
            TextArea.Styles[Style.Pascal.String].ForeColor = Color.MediumVioletRed;
            TextArea.Styles[Style.Pascal.Character].ForeColor = IntToColor(0xE95454);
            TextArea.Styles[Style.Pascal.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            TextArea.Styles[Style.Pascal.Operator].ForeColor = Color.DarkMagenta;
            TextArea.Styles[Style.Pascal.Word].ForeColor = Color.Blue;

            TextArea.Lexer = Lexer.Pascal;

            TextArea.SetKeywords(0, "graficar_ts exit object array integer break continue real string boolean writeln write and or not type var begin end if then else case of while repeat until for do to downto program const procedure function true false");

            // Configure the default style
            TextConsola.StyleResetDefault();
            TextConsola.Styles[Style.Default].Font = "Consolas";
            TextConsola.Styles[Style.Default].Size = 14;
            TextConsola.Styles[Style.Default].BackColor = Color.White; //IntToColor(0x212121);
            TextConsola.Styles[Style.Default].ForeColor = Color.Black; //IntToColor(0xFFFFFF);
            TextConsola.StyleClearAll();

            // Configure the C (C) lexer styless
            TextConsola.Styles[Style.Cpp.Identifier].ForeColor = Color.Black;
            TextConsola.Styles[Style.Cpp.Comment].ForeColor = Color.Green;
            TextConsola.Styles[Style.Cpp.CommentLine].ForeColor = Color.Green;
            TextConsola.Styles[Style.Cpp.CommentDoc].ForeColor = Color.Green;
            TextConsola.Styles[Style.Cpp.Number].ForeColor = Color.DarkCyan;
            TextConsola.Styles[Style.Cpp.String].ForeColor = Color.Orange;
            TextConsola.Styles[Style.Cpp.Character].ForeColor = Color.MediumVioletRed;
            TextConsola.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            TextConsola.Styles[Style.Cpp.Operator].ForeColor = Color.MediumVioletRed;
            TextConsola.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            TextConsola.Styles[Style.Cpp.Word2].ForeColor = Color.Red;

            TextConsola.Lexer = Lexer.Cpp;

            TextConsola.SetKeywords(0, "void int float goto if");
            TextConsola.SetKeywords(1, "return printf Stack Heap");

        }

        #region Numbers, Bookmarks, Code Folding

        /// <summary>
        /// the background color of the text area
        /// </summary>
        private const int BACK_COLOR = 0x2A211C;

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private const int FORE_COLOR = 0xB7B7B7;

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;
        private const int BOOKMARK_MARKER = 2;
        #endregion

        private void InitNumberMargin()
        {
            //PARA EL TEXTAREA
            TextArea.Styles[Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            TextArea.Styles[Style.LineNumber].ForeColor = Color.Aqua;//IntToColor(FORE_COLOR);
            TextArea.Styles[Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            TextArea.Styles[Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

            var nums = TextArea.Margins[NUMBER_MARGIN];
            nums.Width = 60;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            TextArea.MarginClick += TextArea_MarginClick;

            //PARA EL TEXTCONSOLA
            TextConsola.Styles[Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            TextConsola.Styles[Style.LineNumber].ForeColor = Color.Aqua;//IntToColor(FORE_COLOR);
            TextConsola.Styles[Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            TextConsola.Styles[Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

            var nums2 = TextConsola.Margins[NUMBER_MARGIN];
            nums2.Width = 60;
            nums2.Type = MarginType.Number;
            nums2.Sensitive = true;
            nums2.Mask = 0;

            TextConsola.MarginClick += TextConsola_MarginClick;
        }

        private void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == BOOKMARK_MARGIN)
            {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                var line = TextArea.Lines[TextArea.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                }
                else
                {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        private void TextConsola_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == BOOKMARK_MARGIN)
            {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                var line = TextConsola.Lines[TextConsola.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                }
                else
                {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {

        }

        /*************************************************************Aqui termina el Scitilla*****************************************************************/

        private void btnast1_Click(object sender, EventArgs e)
        {
            try
            {
                var prostart = new System.Diagnostics.ProcessStartInfo("cmd", "/C C:\\compiladores2\\CompiAST.jpg");
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = prostart;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception)
            {
                MessageBox.Show("Imagen no encontrada, ruta: C:\\compiladores2\\CompiAST.jpg");
            }
        }

        



    }
}
