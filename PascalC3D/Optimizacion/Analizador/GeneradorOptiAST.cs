using Irony.Parsing;
using PascalC3D.Optimizacion.OptimizadorAST;
using PascalC3D.Optimizacion.OptimizadorCondicionales;
using PascalC3D.Optimizacion.OptimizadorPrimitivas;
using PascalC3D.Optimizacion.OptimizadorValorImplicito;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PascalC3D.Optimizacion.OptimizadorValorImplicito.Operacion;

namespace PascalC3D.Optimizacion.Analizador
{
    class GeneradorOptiAST
    {
        public LinkedList<Funcion> funciones;
        public string head; //codigo donde guarda todo lo del encabezado


        public GeneradorOptiAST(ParseTree arbol)
        {
            generar(arbol.Root);
        }

        private void generar(ParseTreeNode raiz)
        {
            funciones = (LinkedList<Funcion>)analizarNodo(raiz);
        }

        private object analizarNodo(ParseTreeNode actual)
        {
            if (compararNodo(actual, "S"))
            {
                LinkedList<Funcion> funciones = (LinkedList<Funcion>)analizarNodo(actual.ChildNodes[0]);
                return funciones;
            }
            else if (compararNodo(actual, "COD"))
            {
                head = "";
                analizarNodo(actual.ChildNodes[0]);
                return analizarNodo(actual.ChildNodes[1]);
            }
            else if (compararNodo(actual, "HEAD"))
            {
                head += "#include <stdio.h>\n";
                analizarNodo(actual.ChildNodes[7]); //L_VR
                analizarNodo(actual.ChildNodes[8]); //G_TMP
            }
            else if (compararNodo(actual, "L_VR"))
            {
                foreach(ParseTreeNode hijo in actual.ChildNodes)
                {
                    analizarNodo(hijo);
                }
            }
            else if (compararNodo(actual, "VR"))
            {
                if(actual.ChildNodes.Count == 6)
                {
                    head += "float " + getLexema(actual.ChildNodes[1]) + "["+ getLexema(actual.ChildNodes[3]) + "];\n";
                } else //3 HIJOS
                {
                    head += "int " + getLexema(actual.ChildNodes[1]) + ";\n";
                }
            }
            else if (compararNodo(actual, "G_TMP"))
            {
                head += "float ";
                analizarNodo(actual.ChildNodes[1]);
            }
            else if (compararNodo(actual, "L_TMP"))
            {
                for(int i = 0; i < actual.ChildNodes.Count; i++)
                {
                    ParseTreeNode temporal = actual.ChildNodes.ElementAt(i);
                    string cadtemporal = getLexema(temporal);
                    if(i+1 == actual.ChildNodes.Count) //si es el ultimo
                    {
                        head += cadtemporal + ";\n\n";
                    } else
                    {
                        if (cadtemporal.EndsWith("0")) head += cadtemporal + ",\n";
                        else head += cadtemporal + ",";
                    }
                }
            }
            else if (compararNodo(actual, "L_FUN"))
            {
                LinkedList<Funcion> funciones = new LinkedList<Funcion>();
                foreach(ParseTreeNode hijo in actual.ChildNodes)
                {
                    Funcion funcion = (Funcion)analizarNodo(hijo);
                    funciones.AddLast(funcion);
                }
                return funciones;
            }
            else if (compararNodo(actual, "FUN"))
            {
                string id = getLexema(actual.ChildNodes[1]);
                LinkedList<Etiqueta> etiquetas;
                if (actual.ChildNodes.Count == 8)
                {
                    LinkedList<Instruccion> sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[5]);
                    LinkedList<Etiqueta> subetiquetas = (LinkedList<Etiqueta>)analizarNodo(actual.ChildNodes[6]);
                    
                    //Simulo la primera etiqueta
                    Etiqueta primerEtiqueta = new Etiqueta("//PET", sentencias, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                    etiquetas = new LinkedList<Etiqueta>();
                    etiquetas.AddLast(primerEtiqueta);
                    foreach(Etiqueta eti in subetiquetas)
                    {
                        etiquetas.AddLast(eti);
                    }
                } else
                {
                    etiquetas = (LinkedList<Etiqueta>)analizarNodo(actual.ChildNodes[5]);
                }
                return new Funcion(id, etiquetas);
            }
            else if (compararNodo(actual, "L_ET"))
            {
                LinkedList<Etiqueta> etiquetas = new LinkedList<Etiqueta>();
                foreach(ParseTreeNode hijo in actual.ChildNodes)
                {
                    Etiqueta etiqueta = (Etiqueta)analizarNodo(hijo);
                    etiquetas.AddLast(etiqueta);
                }
                return etiquetas;
            }
            else if (compararNodo(actual, "ET"))
            {
                string id = getLexema(actual.ChildNodes[0]);
                LinkedList<Instruccion> sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[2]);
                return new Etiqueta(id,sentencias,actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "L_SEN"))
            {
                LinkedList<Instruccion> sentencias = new LinkedList<Instruccion>();
                foreach(ParseTreeNode hijo in actual.ChildNodes)
                {
                    Instruccion sentencia = (Instruccion)analizarNodo(hijo);
                    sentencias.AddLast(sentencia);
                }
                return sentencias;
            }
            else if (compararNodo(actual, "SEN"))
            {
                return analizarNodo(actual.ChildNodes[0]);
            }
            else if (compararNodo(actual, "ASIG"))
            {
                string target = (string)analizarNodo(actual.ChildNodes[0]);
                Operacion expresion = (Operacion)analizarNodo(actual.ChildNodes[2]);
                return new Asignacion(target, expresion, actual.ChildNodes[1].Token.Location.Line, actual.ChildNodes[1].Token.Location.Column);
            }
            else if (compararNodo(actual, "TG"))
            {
                string target;
                if(actual.ChildNodes.Count == 1)
                {
                    target = getLexema(actual.ChildNodes[0]);
                } 
                else
                {
                    target = getLexema(actual.ChildNodes[0]);
                    target += "[" + (string)analizarNodo(actual.ChildNodes[2]) + "]";
                }
                return target;
            }
            else if (compararNodo(actual, "INDEX"))
            {
                string index;
                if(actual.ChildNodes.Count == 1)
                {
                    index = getLexema(actual.ChildNodes[0]);
                }
                else
                {
                    index = "(int)" + getLexema(actual.ChildNodes[3]); 
                }
                return index;
            }
            else if (compararNodo(actual, "EXP"))
            {
                return analizarNodo(actual.ChildNodes[0]);
            }
            else if (compararNodo(actual, "EXPNUM"))
            {
                Operacion opIzq = (Operacion)analizarNodo(actual.ChildNodes[0]);
                TIPO_OPERACION operacion = (TIPO_OPERACION)analizarNodo(actual.ChildNodes[1]);
                Operacion opDer = (Operacion)analizarNodo(actual.ChildNodes[2]);
                Operacion op = new Operacion();
                op.Operation(opIzq, opDer, operacion, 1, 1);
                return op;
            }
            else if (compararNodo(actual, "VALO"))
            {
                return analizarNodo(actual.ChildNodes[0]);
            }
            else if (compararNodo(actual, "PUN"))
            {
                Operacion op = new Operacion();
                op.Identificador(getLexema(actual.ChildNodes[0]),actual.ChildNodes[0].Token.Location.Line,actual.ChildNodes[0].Token.Location.Column);
                return op;
            } else if (compararNodo(actual, "PRIMI"))
            {
                Operacion op = new Operacion();
                string valor;
                if(actual.ChildNodes.Count == 1)
                {
                    valor = getLexema(actual.ChildNodes[0]);
                    if (valor.Contains(".")) op.Primitivo(new Primitivo(float.Parse(valor)));
                    else op.Primitivo(new Primitivo(int.Parse(valor)));
                }
                else
                {
                    valor = "-" + getLexema(actual.ChildNodes[1]);
                    if (valor.Contains(".")) op.Primitivo(new Primitivo(float.Parse(valor)));
                    else op.Primitivo(new Primitivo(int.Parse(valor)));
                }
                return op;
            } else if (compararNodo(actual, "TEMP"))
            {
                Operacion op = new Operacion();
                op.Identificador(getLexema(actual.ChildNodes[0]), actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                return op;
            } else if (compararNodo(actual, "STR"))
            {
                string estructura = getLexema(actual.ChildNodes[0]);
                estructura += "[" + analizarNodo(actual.ChildNodes[2]) + "]";
                Operacion op = new Operacion();
                op.Identificador(estructura, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                return op;
            }
            else if (compararNodo(actual, "ARI"))
            {
                return getOperacion(actual.ChildNodes[0]);
            }
            else if (compararNodo(actual, "IF"))
            {
                Operacion condicion = (Operacion)analizarNodo(actual.ChildNodes[2]);
                string etiqueta = getLexema(actual.ChildNodes[5]);
                return new If(condicion, etiqueta, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "COND"))
            {
                Operacion izq = (Operacion)analizarNodo(actual.ChildNodes[0]);
                TIPO_OPERACION operacion = (TIPO_OPERACION)analizarNodo(actual.ChildNodes[1]);
                Operacion der = (Operacion)analizarNodo(actual.ChildNodes[2]);
                Operacion op = new Operacion();
                op.Operation(izq, der, operacion, actual.ChildNodes[1].ChildNodes[0].Token.Location.Line, actual.ChildNodes[1].ChildNodes[0].Token.Location.Column);
                return op;
            }
            else if (compararNodo(actual, "VALI"))
            {
                return analizarNodo(actual.ChildNodes[0]);
            }
            else if (compararNodo(actual, "RELA"))
            {
                return getOperacion(actual.ChildNodes[0]);
            }
            else if (compararNodo(actual, "GO"))
            {
                string id = getLexema(actual.ChildNodes[1]);
                return new GOTO(id, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "PRT"))
            {
                string cadena = getLexema(actual.ChildNodes[2]);
                string value = (string)analizarNodo(actual.ChildNodes[4]);
                Operacion op = new Operacion();
                op.Identificador(value, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                return new Imprimir(op, cadena, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "VALP"))
            {
                string valp;
                if(actual.ChildNodes.Count == 1)
                {
                    valp = getLexema(actual.ChildNodes[0]);
                } else if(actual.ChildNodes.Count == 2)
                {
                    valp = "-" + getLexema(actual.ChildNodes[1]);
                } else
                {
                    valp = "(int)" + getLexema(actual.ChildNodes[3]);
                }
                return valp;
            }
            else if (compararNodo(actual, "RET"))
            {
                return new Exit();
            }
            else if (compararNodo(actual, "CALL"))
            {
                string id = getLexema(actual.ChildNodes[0]);
                return new Call(id);
            }
            return null;
        }

        private bool compararNodo(ParseTreeNode nodo, string nombre)
        {
            return nodo.Term.Name.Equals(nombre, System.StringComparison.InvariantCultureIgnoreCase);
        }

        private string getLexema(ParseTreeNode nodo)
        {
            return nodo.Token.Text;
        }

        private TIPO_OPERACION getOperacion(ParseTreeNode nodo)
        {
            string nombre = nodo.ToString().ToLower();
            if (nombre.Contains(">=")) return TIPO_OPERACION.MAYOR_IGUA_QUE;
            else if (nombre.Contains("<=")) return TIPO_OPERACION.MENOR_IGUA_QUE;
            else if (nombre.Contains("!=")) return TIPO_OPERACION.DIFERENTE_QUE;
            else if (nombre.Contains(">")) return TIPO_OPERACION.MAYOR_QUE;
            else if (nombre.Contains("<")) return TIPO_OPERACION.MENOR_QUE;
            else if (nombre.Contains("==")) return TIPO_OPERACION.IGUAL_IGUAL;
            else if (nombre.Contains("+")) return TIPO_OPERACION.SUMA;
            else if (nombre.Contains("-")) return TIPO_OPERACION.RESTA;
            else if (nombre.Contains("*")) return TIPO_OPERACION.MULTIPLICACION;
            else if (nombre.Contains("/")) return TIPO_OPERACION.DIVISION;
            else return TIPO_OPERACION.MODULO; //MODULO
        }

    }
}