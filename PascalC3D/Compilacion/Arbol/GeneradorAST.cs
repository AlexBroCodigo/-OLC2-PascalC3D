using Irony.Parsing;
using PascalC3D.Compilacion.Expresiones.Access;
using PascalC3D.Compilacion.Expresiones.Aritmetica;
using PascalC3D.Compilacion.Expresiones.Asignacion;
using PascalC3D.Compilacion.Expresiones.Literal;
using PascalC3D.Compilacion.Expresiones.Logica;
using PascalC3D.Compilacion.Expresiones.Relacional;
using PascalC3D.Compilacion.Instrucciones;
using PascalC3D.Compilacion.Instrucciones.Array;
using PascalC3D.Compilacion.Instrucciones.Control;
using PascalC3D.Compilacion.Instrucciones.Functions;
using PascalC3D.Compilacion.Instrucciones.Object;
using PascalC3D.Compilacion.Instrucciones.Transfer;
using PascalC3D.Compilacion.Instrucciones.Variables;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Arbol
{
    class GeneradorAST
    {
        private ParseTree arbolirony;

        public AST miarbol;

        public GeneradorAST(ParseTree arbol)
        {
            arbolirony = arbol;
            generar(arbolirony.Root);
        }

        private void generar(ParseTreeNode raiz)
        {
            miarbol = (AST)analizarNodo(raiz);
        }

        private void recorrerBloque(LinkedList<Instruccion> pila, Bloque bloque)
        {
            LinkedList<Instruccion> instrucciones = bloque.instrucciones;
            foreach (Instruccion ins in instrucciones)
            {
                if (ins is Bloque) recorrerBloque(pila, (Bloque)ins);
                else pila.AddLast(ins);
            }
        }

        private object analizarNodo(ParseTreeNode actual)
        {
            //INICIO DE LA GRAMATICA
            if (compararNodo(actual, "S"))
            {
                LinkedList<Instruccion> instrucciones = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[0]);
                return new AST(instrucciones);
            }
            else if (compararNodo(actual, "P"))
            {
                LinkedList<Instruccion> pilatemp = new LinkedList<Instruccion>();
                if (actual.ChildNodes.Count == 4)
                {
                    //MAIN
                    Bloque bloque = (Bloque)analizarNodo(actual.ChildNodes[3]);
                    recorrerBloque(pilatemp, bloque);
                }
                else if (actual.ChildNodes.Count == 5)
                {
                    //L_AC; MAIN
                    LinkedList<Bloque> bloques = ((LinkedList<Bloque>)analizarNodo(actual.ChildNodes[3])); //L_AC
                    foreach (Bloque bloque in bloques) recorrerBloque(pilatemp, bloque); //Agrego las instrucciones que viene en pilatemp
                    recorrerBloque(pilatemp, (Bloque)analizarNodo(actual.ChildNodes[4])); //MAIN
                }
                return pilatemp;
            }
            else if (compararNodo(actual, "L_AC"))
            {
                LinkedList<Bloque> acciones = new LinkedList<Bloque>();
                foreach (ParseTreeNode hijo in actual.ChildNodes) acciones.AddLast((Bloque)analizarNodo(hijo)); //AC
                return acciones;
            }
            else if (compararNodo(actual, "AC"))
            {
                return analizarNodo(actual.ChildNodes[0]); //retornando Bloque
            }
            else if (compararNodo(actual, "G_CNT"))
            {
                return new Bloque((LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[1]));
            }
            else if (compararNodo(actual, "L_CNT"))
            {
                LinkedList<Instruccion> constantes = new LinkedList<Instruccion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    constantes.AddLast((Instruccion)analizarNodo(hijo));
                }
                return constantes;
            }
            else if (compararNodo(actual, "CNT"))
            {
                string id = getLexema(actual.ChildNodes[0]);
                Expresion value = (Expresion)analizarNodo(actual.ChildNodes[2]);
                return new DeclaConstante(id,value,actual.ChildNodes[0].Token.Location.Line,actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "G_TY"))
            {
                return new Bloque((LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[1]));
            }
            else if (compararNodo(actual, "L_TY"))
            {
                LinkedList<Instruccion> instrucciones = new LinkedList<Instruccion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    instrucciones.AddLast((Instruccion)analizarNodo(hijo));
                }
                return instrucciones;
            }
            else if (compararNodo(actual, "TY"))
            {
                string id = getLexema(actual.ChildNodes[0]);
                if (compararNodo(actual.ChildNodes[2].ChildNodes[0], "OBJ"))
                {
                    LinkedList<Param> paramList = (LinkedList<Param>)analizarNodo(actual.ChildNodes[2].ChildNodes[0]); //OBJ
                    return new StructSt(id, paramList, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                }
                else //ARRAY
                {
                    LinkedList<Dimension> dimensiones = (LinkedList<Dimension>)analizarNodo(actual.ChildNodes[2].ChildNodes[0].ChildNodes[2]); //L_DIM
                    Tipo tipoArreglo = (Tipo)analizarNodo(actual.ChildNodes[2].ChildNodes[0].ChildNodes[5]); //ZTIPO
                    return new ArraySt(id,dimensiones,tipoArreglo,actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                }
            }
            else if (compararNodo(actual, "L_DIM"))
            {
                LinkedList<Dimension> dimensiones = new LinkedList<Dimension>();
                int numero = 0;
                foreach(ParseTreeNode hijo in actual.ChildNodes)
                {
                    Dimension dimension = (Dimension)analizarNodo(hijo);
                    numero++;
                    dimension.numero = numero;
                    dimensiones.AddLast(dimension);
                }
                return dimensiones;
            } else if (compararNodo(actual, "DIM"))
            {
                Primitivo inferior = (Primitivo)obtenerLiteral(actual.ChildNodes[0]);
                Primitivo superior = (Primitivo)obtenerLiteral(actual.ChildNodes[3]);
                return new Dimension(inferior, superior);
            }
            else if (compararNodo(actual, "OBJ"))
            {
                return analizarNodo(actual.ChildNodes[2]);
            }
            else if (compararNodo(actual, "L_AT"))
            {
                LinkedList<Param> paramList = new LinkedList<Param>();
                foreach(ParseTreeNode hijo in actual.ChildNodes)
                {
                    LinkedList<Param> paramhijos = (LinkedList<Param>)analizarNodo(hijo);
                    foreach (Param paramhijo in paramhijos) paramList.AddLast(paramhijo);
                }
                return paramList;
            }
            else if (compararNodo(actual, "AT"))
            {
                LinkedList<string> idList = (LinkedList<string>)analizarNodo(actual.ChildNodes[0]); //L_ID
                Tipo tipo = (Tipo)analizarNodo(actual.ChildNodes[2]); //ZTIPO
                LinkedList<Param> atributos = new LinkedList<Param>();
                foreach(string id in idList)
                {
                    atributos.AddLast(new Param(id, tipo));
                }
                return atributos;
            }
            else if (compararNodo(actual, "DECLAS"))
            {
                return new Bloque((LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[1]));
            }
            else if (compararNodo(actual, "L_VR"))
            {
                LinkedList<Instruccion> declaraciones = new LinkedList<Instruccion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes) declaraciones.AddLast((Instruccion)analizarNodo(hijo));
                return declaraciones;
            }
            else if (compararNodo(actual, "VR"))
            {
                LinkedList<string> idList;
                Tipo tipo;
                if (actual.ChildNodes.Count == 6)
                {
                    idList = (LinkedList<string>)analizarNodo(actual.ChildNodes[0]);
                    tipo = (Tipo)analizarNodo(actual.ChildNodes[2]);
                    Expresion value = (Expresion)analizarNodo(actual.ChildNodes[4]);
                    return new Declaracion(tipo, idList, value, actual.ChildNodes[1].Token.Location.Line, actual.ChildNodes[1].Token.Location.Column);
                }
                else //4 HIJOS 
                {
                    idList = (LinkedList<string>)analizarNodo(actual.ChildNodes[0]);
                    tipo = (Tipo)analizarNodo(actual.ChildNodes[2]);
                    Expresion value = null;
                    if(tipo.tipo == Tipos.STRUCT)
                    {
                        value = new NewStruct(tipo.tipoId, actual.ChildNodes[1].Token.Location.Line,actual.ChildNodes[1].Token.Location.Column);
                    }
                    return new Declaracion(tipo, idList,value, actual.ChildNodes[1].Token.Location.Line, actual.ChildNodes[1].Token.Location.Column);
                }
            }
            else if (compararNodo(actual, "L_ID"))
            {
                LinkedList<string> idList = new LinkedList<string>();

                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    idList.AddLast(getLexema(hijo));
                }
                return idList;
            }
            else if (compararNodo(actual, "ZTIPO"))
            {
                Tipos tipoenum;
                if (compararID(actual.ChildNodes[0]))
                {
                    tipoenum = Tipo.Tipos.STRUCT;
                    return new Tipo(tipoenum,getLexema(actual.ChildNodes[0]));
                }
                else //TIPO
                {
                    tipoenum = (Tipos)analizarNodo(actual.ChildNodes[0]);
                    return new Tipo(tipoenum);
                }
            }
            else if (compararNodo(actual, "TIPO"))
            {
                string tipocadena = getLexema(actual.ChildNodes[0]).ToLower();
                switch (tipocadena)
                {
                    case "integer": return Tipos.INTEGER;
                    case "real": return Tipos.REAL;
                    case "string": return Tipos.STRING;
                    default: return Tipos.BOOLEAN; //boolean
                }
            }
            else if (compararNodo(actual, "L_PROF"))
            {
                LinkedList<Instruccion> funciones = new LinkedList<Instruccion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    funciones.AddLast((Instruccion)analizarNodo(hijo));
                }
                return new Bloque(funciones);
            }
            else if (compararNodo(actual, "PROF"))
            {
                return analizarNodo(actual.ChildNodes[0]);
            }
            else if (compararNodo(actual, "PRO"))
            {
                Tipo tipo = new Tipo(Tipo.Tipos.VOID);
                string id = getLexema(actual.ChildNodes[1]);
                LinkedList<Param> parametros = new LinkedList<Param>();
                LinkedList<Instruccion> sentencias;
                if (actual.ChildNodes.Count == 7)
                {
                    parametros = (LinkedList<Param>)analizarNodo(actual.ChildNodes[3]);
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[6]);
                }
                else if (actual.ChildNodes.Count == 6)
                {
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[5]);
                }
                else //4 hijos
                {
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[3]);
                }
                return new FunctionSt(tipo, id, parametros, sentencias, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "FUN"))
            {
                string id = getLexema(actual.ChildNodes[1]);
                Tipo tipo;
                LinkedList<Param> parametros = new LinkedList<Param>();
                LinkedList<Instruccion> sentencias;
                if (actual.ChildNodes.Count == 9)
                {
                    parametros = (LinkedList<Param>)analizarNodo(actual.ChildNodes[3]);
                    tipo = (Tipo)analizarNodo(actual.ChildNodes[6]);
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[8]);
                }
                else if (actual.ChildNodes.Count == 8)
                {
                    tipo = (Tipo)analizarNodo(actual.ChildNodes[5]);
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[7]);
                }
                else //6 hijos
                {
                    tipo = (Tipo)analizarNodo(actual.ChildNodes[3]);
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[5]);
                }
                return new FunctionSt(tipo,id,parametros,sentencias,actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "L_PARAM"))
            {
                LinkedList<Param> parametros = new LinkedList<Param>();
                LinkedList<Param> paramhijos;
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    paramhijos = (LinkedList<Param>)analizarNodo(hijo);
                    foreach (Param param in paramhijos)
                    {
                        parametros.AddLast(param);
                    }
                }
                return parametros;
            }
            else if (compararNodo(actual, "PARAM"))
            {
                LinkedList<string> idList;
                Tipo tipo;
                bool isRef;
                if (actual.ChildNodes.Count == 3)
                {
                    idList = (LinkedList<string>)analizarNodo(actual.ChildNodes[0]);
                    tipo = (Tipo)analizarNodo(actual.ChildNodes[2]); //ZTIPO
                    isRef = false;
                } else //4 HIJOS
                {
                    idList = (LinkedList<string>)analizarNodo(actual.ChildNodes[1]);
                    tipo = (Tipo)analizarNodo(actual.ChildNodes[3]); //ZTIPO
                    isRef = true;
                }
                LinkedList<Param> atributos = new LinkedList<Param>();
                foreach (string id in idList)
                {
                    atributos.AddLast(new Param(id, tipo,isRef));
                }
                return atributos;
            }
            else if (compararNodo(actual, "SPACE"))
            {
                LinkedList<Instruccion> sentencias;
                if (actual.ChildNodes.Count == 1) //BEG
                {
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[0]);
                }
                else //L_DEF BEG
                {
                    sentencias = new LinkedList<Instruccion>();
                    LinkedList<Instruccion> definiciones = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[0]);
                    LinkedList<Instruccion> instrucciones = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[1]);
                    //RECORREMOS AMBAS LISTAS PARA CREAR SOLO UNA
                    foreach (Instruccion definicion in definiciones)
                    {
                        sentencias.AddLast(definicion);
                    }
                    foreach (Instruccion ins in instrucciones)
                    {
                        sentencias.AddLast(ins);
                    }
                }
                return sentencias;
            }
            else if (compararNodo(actual, "L_DEF"))
            {
                //Guardaremos todas las listas de declas o constantes en una sola
                LinkedList<Instruccion> declaraciones = new LinkedList<Instruccion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    LinkedList<Instruccion> declashijo = (LinkedList<Instruccion>)analizarNodo(hijo);
                    foreach (Instruccion decla in declashijo)
                    {
                        declaraciones.AddLast(decla);
                    }
                }
                return declaraciones;
            }
            else if (compararNodo(actual, "DEF"))
            {
                //Recordar que solo viene DECLAS. DEF -> DECLAS -> L_VR: lista
                return (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[0].ChildNodes[1]);
            }
            else if (compararNodo(actual, "MAIN"))
            {
                //L_SEN
                return new Bloque((LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[1]));
            }
            else if (compararNodo(actual, "L_SEN"))
            {
                LinkedList<Instruccion> sentencias = new LinkedList<Instruccion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    sentencias.AddLast((Instruccion)analizarNodo(hijo)); //SEN
                }
                return sentencias;
            }
            else if (compararNodo(actual, "SEN"))
            {
                return analizarNodo(actual.ChildNodes[0]);
            }
            else if (compararNodo(actual, "BEG"))
            {
                return analizarNodo(actual.ChildNodes[1]); //retorna lista de sentencias
            }
            else if (compararNodo(actual, "L_SENCU"))
            {
                LinkedList<Instruccion> sentencias = new LinkedList<Instruccion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    LinkedList<Instruccion> sentenciashijo = (LinkedList<Instruccion>)analizarNodo(hijo);
                    foreach (Instruccion senhijo in sentenciashijo)
                    {
                        sentencias.AddLast(senhijo);
                    }
                }
                return sentencias;
            }
            else if (compararNodo(actual, "SENCU"))
            {
                LinkedList<Instruccion> sentencias;
                if (compararNodo(actual.ChildNodes[0], "BEG"))
                {
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[0]);
                }
                else //SEN
                {
                    sentencias = new LinkedList<Instruccion>();
                    sentencias.AddLast((Instruccion)analizarNodo(actual.ChildNodes[0]));
                }
                return sentencias;
            }
            else if (compararNodo(actual, "ASIG"))
            {
                Expresion target = (Expresion)analizarNodo(actual.ChildNodes[0]);
                Expresion value = (Expresion)analizarNodo(actual.ChildNodes[2]);
                return new Asignacion(target, value, actual.ChildNodes[1].Token.Location.Line, actual.ChildNodes[1].Token.Location.Column);
            }
            else if (compararNodo(actual, "ASID"))
            {
                if (actual.ChildNodes.Count == 3)
                {
                    Expresion anterior = (Expresion)analizarNodo(actual.ChildNodes[0]);
                    string id = getLexema(actual.ChildNodes[2]);
                    return new AsignacionId(id, anterior, actual.ChildNodes[1].Token.Location.Line, actual.ChildNodes[1].Token.Location.Column);
                }
                else //UN HIJO
                {
                    string id = getLexema(actual.ChildNodes[0]);
                    return new AsignacionId(id, null, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                }
            }
            else if (compararNodo(actual, "IF"))
            {
                Expresion condicion = (Expresion)analizarNodo(actual.ChildNodes[1]);
                LinkedList<Instruccion> sentencias = new LinkedList<Instruccion>();
                LinkedList<Instruccion> sentenciasElse;
                if (actual.ChildNodes.Count <= 5)
                {
                    if (compararNodo(actual.ChildNodes[3], "BEG"))
                    {
                        sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[3]);
                    }
                    else //SEN
                    {
                        sentencias.AddLast((Instruccion)analizarNodo(actual.ChildNodes[3]));
                    }
                    sentenciasElse = null;
                    if (actual.ChildNodes.Count == 5)
                    {
                        sentenciasElse = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[4]); //ELSE
                    }
                }
                else //7 HIJOS
                {
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[4]);
                    sentenciasElse = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[6]);
                }
                return new If(condicion, sentencias, sentenciasElse, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "ELSE"))
            {
                LinkedList<Instruccion> sentencias;
                if (compararNodo(actual.ChildNodes[1], "BEG"))
                {
                    sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[1]);
                }
                else //SEN
                {
                    sentencias = new LinkedList<Instruccion>();
                    sentencias.AddLast((Instruccion)analizarNodo(actual.ChildNodes[1]));
                }
                return sentencias;
            }
            else if (compararNodo(actual, "CASE"))
            {
                string id;
                AccessId variable;
                LinkedList<Opcion> opciones;
                LinkedList<Instruccion> sentenciasElse = null;
                if (actual.ChildNodes.Count == 6)
                {
                    id = getLexema(actual.ChildNodes[1]);
                    variable = new AccessId(id, null, actual.ChildNodes[1].Token.Location.Line, actual.ChildNodes[1].Token.Location.Column);
                    opciones = (LinkedList<Opcion>)analizarNodo(actual.ChildNodes[3]);
                } else if(actual.ChildNodes.Count == 7)
                {
                    id = getLexema(actual.ChildNodes[1]);
                    variable = new AccessId(id, null, actual.ChildNodes[1].Token.Location.Line, actual.ChildNodes[1].Token.Location.Column);
                    opciones = (LinkedList<Opcion>)analizarNodo(actual.ChildNodes[3]);
                    sentenciasElse = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[4]);
                } else if(actual.ChildNodes.Count == 8)
                {
                    id = getLexema(actual.ChildNodes[2]);
                    variable = new AccessId(id, null, actual.ChildNodes[2].Token.Location.Line, actual.ChildNodes[2].Token.Location.Column);
                    opciones = (LinkedList<Opcion>)analizarNodo(actual.ChildNodes[5]);
                } else // 9 HIJOS
                {
                    id = getLexema(actual.ChildNodes[2]);
                    variable = new AccessId(id, null, actual.ChildNodes[2].Token.Location.Line, actual.ChildNodes[2].Token.Location.Column);
                    opciones = (LinkedList<Opcion>)analizarNodo(actual.ChildNodes[5]);
                    sentenciasElse = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[6]);
                }
                return new Case(variable, opciones, sentenciasElse, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "L_OPC"))
            {
                LinkedList<Opcion> opciones = new LinkedList<Opcion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    opciones.AddLast((Opcion)analizarNodo(hijo));
                }
                return opciones;
            }
            else if (compararNodo(actual, "OPC"))
            {
                LinkedList<Expresion> etiquetas = (LinkedList<Expresion>)analizarNodo(actual.ChildNodes[0]);
                LinkedList<Instruccion> sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[2]);
                return new Opcion(etiquetas, sentencias, actual.ChildNodes[1].Token.Location.Line, actual.ChildNodes[1].Token.Location.Column);
            }
            else if (compararNodo(actual, "LETC"))
            {
                LinkedList<Expresion> etiquetas = new LinkedList<Expresion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    etiquetas.AddLast((Expresion)analizarNodo(hijo));
                }
                return etiquetas;
            }
            else if (compararNodo(actual, "ETC"))
            {
                if (actual.ChildNodes.Count == 2)
                {

                    Primitivo numero = (Primitivo)obtenerLiteral(actual.ChildNodes[1]);
                    numero.value = "-" + numero.value.ToString();
                    return numero;
                }
                else
                {
                    return obtenerLiteral(actual.ChildNodes[0]);
                }
            }
            else if (compararNodo(actual, "REP"))
            {
                LinkedList<Instruccion> sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[1]);
                Expresion condicion = (Expresion)analizarNodo(actual.ChildNodes[3]);
                return new Repeat(condicion,sentencias, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "WH"))
            {
                Expresion condicion = (Expresion)analizarNodo(actual.ChildNodes[1]);
                LinkedList<Instruccion> sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[3]);
                return new While(condicion, sentencias, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "FOR"))
            {
                string id = getLexema(actual.ChildNodes[1]);
                Expresion primero = (Expresion)analizarNodo(actual.ChildNodes[3]);
                string fad = getLexema(actual.ChildNodes[4].ChildNodes[0]);
                Expresion segundo = (Expresion)analizarNodo(actual.ChildNodes[5]);
                LinkedList<Instruccion> sentencias = (LinkedList<Instruccion>)analizarNodo(actual.ChildNodes[7]);
                return new For(id, primero, fad, segundo, sentencias, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "BRK"))
            {
                return new Break(actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "COT"))
            {
                return new Continue(actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "WRT"))
            {
                bool esSalto = false;
                if (getLexema(actual.ChildNodes[0]).ToLower() == "writeln") esSalto = true; //sino es write
                return new Writeln((LinkedList<Expresion>)analizarNodo(actual.ChildNodes[2]), esSalto, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararNodo(actual, "EXT"))
            {
                if(actual.ChildNodes.Count == 5)
                {
                    return new Return((Expresion)analizarNodo(actual.ChildNodes[2]), actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                } else // 4 HIJOS
                {
                    return new Return(null, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                }
            }
            else if (compararNodo(actual, "L_EXP"))
            {
                LinkedList<Expresion> expresiones = new LinkedList<Expresion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    expresiones.AddLast((Expresion)analizarNodo(hijo));
                }
                return expresiones;
            }
            else if (compararNodo(actual, "EXPLOG"))
            {
                if (actual.ChildNodes.Count == 3)
                {
                    if (compararNodo(actual.ChildNodes[0], "(")) // ( EXPLOG )
                    {
                        return analizarNodo(actual.ChildNodes[1]);
                    }
                    else
                    {
                        //EXPLOG SIGNO EXPLOG
                        Expresion izquierda = (Expresion)analizarNodo(actual.ChildNodes[0]);
                        Expresion derecha = (Expresion)analizarNodo(actual.ChildNodes[2]);
                        return getOperacion(actual.ChildNodes[1], izquierda, derecha);
                    }
                }
                else if (actual.ChildNodes.Count == 2)
                {
                    //not EXPLOG
                    return new Not((Expresion)analizarNodo(actual.ChildNodes[1]), actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                }
                if (actual.ChildNodes.Count == 1)
                {
                    //UN HIJO
                    return analizarNodo(actual.ChildNodes[0]);
                }
            }
            else if (compararNodo(actual, "EXPRELA"))
            {
                if (actual.ChildNodes.Count == 3)
                {
                    if (compararNodo(actual.ChildNodes[0], "(")) // ( EXPRELA )
                    {
                        return analizarNodo(actual.ChildNodes[1]);
                    }
                    else
                    {
                        //EXPRELA SIGNO EXPRELA
                        Expresion izquierda = (Expresion)analizarNodo(actual.ChildNodes[0]);
                        Expresion derecha = (Expresion)analizarNodo(actual.ChildNodes[2]);
                        return getOperacion(actual.ChildNodes[1], izquierda, derecha);
                    }
                }
                else //UN HIJO
                {
                    return analizarNodo(actual.ChildNodes[0]);
                }
            }
            else if (compararNodo(actual, "EXPNUMERICA"))
            {
                if (actual.ChildNodes.Count == 3)
                {
                    if (compararNodo(actual.ChildNodes[0], "("))    // ( EXPNUMERICA )
                    {
                        return analizarNodo(actual.ChildNodes[1]);
                    } else
                    {
                        //EXPNUMERICA operador EXPNUMERICA
                        Expresion izquierda = (Expresion)analizarNodo(actual.ChildNodes[0]);
                        Expresion derecha = (Expresion)analizarNodo(actual.ChildNodes[2]);
                        return getOperacion(actual.ChildNodes[1], izquierda, derecha);
                    }
                }
                else if (actual.ChildNodes.Count == 2)
                {
                    Expresion unario = (Expresion)analizarNodo(actual.ChildNodes[1]);
                    return new RestaUni(unario, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                }
                else //UN HIJO
                {
                    if (compararID(actual.ChildNodes[0]))
                    {

                    }
                    else return analizarNodo(actual.ChildNodes[0]); //LITERAL o ACCESS
                }
            } else if (compararNodo(actual, "ACCESS"))
            {
                return analizarNodo(actual.ChildNodes[0]);
            }
            else if (compararNodo(actual, "ACCID"))
            {
                if(actual.ChildNodes.Count == 3)
                {
                    Expresion anterior = (Expresion)analizarNodo(actual.ChildNodes[0]);
                    string id = getLexema(actual.ChildNodes[2]);
                    return new AccessId(id, anterior, actual.ChildNodes[1].Token.Location.Line, actual.ChildNodes[1].Token.Location.Column);
                }
                else //UN HIJO
                {
                    string id = getLexema(actual.ChildNodes[0]);
                    return new AccessId(id, null, actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
                }
            }
            else if (compararNodo(actual, "CALL"))
            {
                string id = getLexema(actual.ChildNodes[0]);
                LinkedList<Expresion> parametros = new LinkedList<Expresion>();
                if (actual.ChildNodes.Count == 5)
                {
                    parametros = (LinkedList<Expresion>)analizarNodo(actual.ChildNodes[2]);
                }
                return new AsignacionFunc(id,parametros,null,actual.ChildNodes[0].Token.Location.Line, actual.ChildNodes[0].Token.Location.Column);
            }
            else if (compararLiteral(actual)) //LITERAL
            {
                return obtenerLiteral(actual);
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

        private bool compararLiteral(ParseTreeNode nodo)
        {
            string nombre = nodo.ToString();
            if (nombre.EndsWith("(cadena)") || nombre.EndsWith("(numero)") || nombre.EndsWith("(booleano)")) return true;
            return false;
        }

        private object obtenerLiteral(ParseTreeNode nodo)
        {
            string nombre = nodo.ToString();
            if (nombre.EndsWith("(cadena)"))
            {
                string value = nodo.Token.Text.Replace("'", "");
                return new PrimitivoString(Tipos.STRING,value, nodo.Token.Location.Line, nodo.Token.Location.Column);
            } 
            else if (nombre.EndsWith("(numero)"))
            {
                string value = nodo.Token.Text;
                if (value.Contains('.')) //REAL
                {
                    return new Primitivo(Tipos.REAL, value, nodo.Token.Location.Line, nodo.Token.Location.Column);
                }
                else //INTEGER
                {
                    return new Primitivo(Tipos.INTEGER, value, nodo.Token.Location.Line, nodo.Token.Location.Column);
                }
            } else
            {
                bool value;
                if (nodo.Token.Text.ToLower().Equals("true")) value = true;
                else value = false;
                return new Primitivo(Tipos.BOOLEAN, value, nodo.Token.Location.Line, nodo.Token.Location.Column);
            }
        }

        private bool compararID(ParseTreeNode nodo)
        {
            return nodo.ToString().EndsWith("(ID)");
        }

        private Expresion getOperacion(ParseTreeNode nodo, Expresion left, Expresion right)
        {
            string nombre = nodo.ToString().ToLower();
            if (nombre.Contains("and")) return new And(left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains("mod")) return new Modulo(left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains("or")) return new Or(left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains(">=")) return new Greater(true, left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains("<=")) return new Less(true, left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains("<>")) return new NotEquals(left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains(">")) return new Greater(false, left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains("<")) return new Less(false, left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains("=")) return new Equals(left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains("+")) return new Suma(left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains("-")) return new Resta(left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else if (nombre.Contains("*")) return new Multi(left, right, nodo.Token.Location.Line, nodo.Token.Location.Column);
            else return new Div(left, right, nodo.Token.Location.Line, nodo.Token.Location.Column); //DIVISION
        }
    }
}
