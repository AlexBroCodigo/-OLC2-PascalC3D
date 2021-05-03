using Irony;
using Irony.Parsing;
using PascalC3D.Compilacion.Arbol;
using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Instrucciones.Array;
using PascalC3D.Compilacion.Instrucciones.Functions;
using PascalC3D.Compilacion.Instrucciones.Object;
using PascalC3D.Compilacion.Instrucciones.Variables;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.ControlDOT;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PascalC3D.Compilacion.Analizador
{
    class Sintactico : Grammar
    {
        public static void analizar(String cadena)
        {
            Gramatica gramatica = new Gramatica();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(cadena);
            ParseTreeNode raiz = arbol.Root;
            Errores errores = new Errores();
            switch (verificarErroresLexSin(arbol, raiz, errores))
            {
                case 1:
                    errores.generarReporteErrores(1);
                    MessageBox.Show("Se encontraron errores lexicos o sintacticos y no se pudo recuperar. No generamos C3D", "Errores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case 2:
                    generarImagen(raiz);    //graficamos AST
                    errores.generarReporteErrores(1);
                    MessageBox.Show("Se encontraron errores lexicos o sintacticos pero nos recuperamos de ellos. No generamos C3D", "Errores", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 3:
                    generarImagen(raiz);    //graficamos AST
                    MessageBox.Show("Cadena analizada correctamente. A generar C3D!", "PascalC3D", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    generarC3D(arbol, errores);
                    errores.generarReporteErrores(1);
                    if (!errores.esVacio()) MessageBox.Show("Se encontraron errores semanticos durante la generacion del C3D", "PascalC3D", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }

        }

        public static void generarC3D(ParseTree arbol, Errores errores)
        {
            GeneradorAST migenerador = new GeneradorAST(arbol);
            AST ast = migenerador.miarbol;
            Entorno ent = new Entorno(null, "GLOBAL", "GLOBAL");
            if (ast != null)
            {
                //Primera pasada: solo funciones, structs y Arrays
                foreach (Instruccion element in ast.instrucciones)
                {
                    if(element is FunctionSt || element is StructSt || element is ArraySt) element.compilar(ent, errores);
                }

                //Segunda pasada: Solo declaraciones
                foreach(Instruccion element in ast.instrucciones)
                {
                    if (element is Declaracion || element is DeclaConstante) element.compilar(ent, errores);
                }
                string declaraciones = Generator.getInstance().getCode() + "\n"; //obtengo las declaraciones antes para guardarlos en los entornos antes de todo

                //Tercera pasada: Solo funciones (genera codigo);
                foreach(Instruccion element in ast.instrucciones)
                {
                    if (element is FunctionSt) element.compilar(ent, errores);
                }
                string funciones = Generator.getInstance().getCode(); //obtengo las funciones no nativas

                //Cuarta pasada: Las instrucciones que van dentro del main
                foreach (Instruccion element in ast.instrucciones)
                {
                    if (!(element is FunctionSt || element is StructSt || element is ArraySt || element is Declaracion || element is DeclaConstante)) element.compilar(ent, errores);
                }
                //GENERAMOS C3D
                string codigo = Generator.getInstance().getEncabezado();
                codigo += Generator.getInstance().getFuncionesNativas();
                codigo += funciones;
                codigo += Generator.getInstance().getOpenMain();
                codigo += declaraciones;
                codigo += Generator.getInstance().getCode();
                codigo += Generator.getInstance().getCloseMain();
                Form1.consola.Text = codigo;
                Generator.getInstance().clearCode();
                TableSymbol tabla = new TableSymbol();
                tabla.generarTablaSimbolos(ent);
            }
            else MessageBox.Show("Error generando mi AST", "Errores", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                var comand = String.Format("dot -Tjpg {0} -o {1}", "C:\\compiladores2\\CompiAST.dot", "C:\\compiladores2\\CompiAST.jpg");
                var prostart = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + comand);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = prostart;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo generar la imagen CompiAST.jpg");
            }
        }

        public static void generarArchivoDot(String grafo)
        {
            TextWriter archivo;
            archivo = new StreamWriter("C:\\compiladores2\\CompiAST.dot");
            archivo.WriteLine(grafo);
            archivo.Close();
        }
    }
}
