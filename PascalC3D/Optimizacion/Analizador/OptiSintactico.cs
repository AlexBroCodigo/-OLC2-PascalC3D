using Irony;
using Irony.Parsing;
using PascalC3D.ControlDOT;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PascalC3D.Optimizacion.Analizador
{
    class OptiSintactico : Grammar
    {
        public static void analizar(string cadena)
        {
            OptiGramatica gramatica = new OptiGramatica();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(cadena);
            ParseTreeNode raiz = arbol.Root;
            Errores errores = new Errores();
            switch (verificarErroresLexSin(arbol, raiz, errores))
            {
                case 1:
                    errores.generarReporteErrores(2);
                    MessageBox.Show("Se encontraron errores lexicos o sintacticos y no se pudo recuperar. No optimizamos el C3D", "Errores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case 2:
                    generarImagen(raiz);    //graficamos AST
                    errores.generarReporteErrores(2);
                    MessageBox.Show("Se encontraron errores lexicos o sintacticos pero nos recuperamos de ellos. No optimizamos el C3D", "Errores", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 3:
                    generarImagen(raiz);    //graficamos AST
                    MessageBox.Show("Cadena analizada correctamente. A optimizar el C3D!", "PascalC3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    optimizarC3D(cadena,arbol);
                    errores.generarReporteErrores(2);
                    if (!errores.esVacio()) MessageBox.Show("Se encontraron errores semanticos durante la optimizacion del C3D", "PascalC3D", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }


        private static void optimizarC3D(string texto,ParseTree arbol)
        {
            Optimizador optimizador = new Optimizador();
            optimizador.inicializar();
            Form1.consola.Text = optimizador.optimizar(texto, arbol);
            optimizador.reportar();
            
        }


        private static int verificarErroresLexSin(ParseTree arbol, ParseTreeNode raiz, Errores errores)
        {
            int retorno;    //1: Hay errores y no se recupero, 2: Hay errores y si se recupero, 3: No hay errores
            if (arbol.ParserMessages.Count > 0)
            {
                if (raiz == null)
                {
                    errores.agregarError(new Error("Sintáctico", "Error fatal, no se recupero el analizador", "-", 0, 0));
                    retorno = 1;
                }
                else retorno = 2;
                //guardo mensajes
                List<LogMessage> msjerrores = arbol.ParserMessages;
                foreach (LogMessage error in msjerrores)
                {
                    if (error.Message.Contains("Syntax"))
                    {
                        errores.agregarError(new Error("Sintáctico", error.Message, "-", error.Location.Line+1, error.Location.Column+1));
                    }
                    else
                    {
                        errores.agregarError(new Error("Léxico", error.Message, "-", error.Location.Line+1, error.Location.Column+1));
                    }
                }
            }
            else retorno = 3;
            return retorno;
        }

        private static void generarImagen(ParseTreeNode raiz)
        {
            String grafoDOT = ControlDot.getDOT(raiz);
            generarArchivoDot(grafoDOT);

            try
            {
                var comand = String.Format("dot -Tjpg {0} -o {1}", "C:\\compiladores2\\OptiAST.dot", "C:\\compiladores2\\OptiAST.jpg");
                var prostart = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + comand);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = prostart;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo generar la imagen OptiAST.jpg");
            }
        }

        public static void generarArchivoDot(String grafo)
        {
            TextWriter archivo;
            archivo = new StreamWriter("C:\\compiladores2\\OptiAST.dot");
            archivo.WriteLine(grafo);
            archivo.Close();
        }





    }
}
