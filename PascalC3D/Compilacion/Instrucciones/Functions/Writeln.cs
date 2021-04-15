using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Instrucciones.Functions
{
    class Writeln : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private LinkedList<Expresion> expresiones;
        private bool isLine;

        public Writeln(LinkedList<Expresion> expresiones, bool isLine, int linea, int columna)
        {
            this.expresiones = expresiones;
            this.isLine = isLine;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent,Errores errores)
        {
            Generator generator = Generator.getInstance();
            generator.addComment("Inicia Writeln");
            try
            {
                foreach (Expresion expresion in expresiones)
                {
                    Retorno value = expresion.compilar(ent);
                    switch (value.type.tipo)
                    {
                        case Tipos.INTEGER:
                            if (value.isTemp) generator.addPrint("d","(int)"+value.getValue());
                            else generator.addPrint("d", value.getValue());
                            break;
                        case Tipos.REAL:
                            generator.addPrint("f", value.getValue());
                            break;
                        case Tipos.BOOLEAN:
                            if(value.symbol == null) // ACCEDIDO DE OBJETO
                            {
                                generator.addIf(value.getValue(),"1","==",value.trueLabel);
                                generator.addGoto(value.falseLabel);
                            }
                            string templabel = generator.newLabel();
                            generator.addLabel(value.trueLabel);
                            generator.addPrintTrue();
                            generator.addGoto(templabel);
                            generator.addLabel(value.falseLabel);
                            generator.addPrintFalse();
                            generator.addLabel(templabel);
                            break;
                        case Tipos.STRING:
                            generator.addNextEnv(ent.getSize());
                            generator.addSetStack("SP", value.getValue());
                            generator.addCall("native_print_str");
                            generator.addAntEnv(ent.getSize());
                            break;
                        default:
                            throw new Error("Semántico", "Tipo de dato no soportado en un writeln", ent.obtenerAmbito(), linea, columna);
                    }
                }
                if (isLine)
                {
                    generator.addPrint("c", "10");
                }
            }
            catch (Error ex)
            {
                errores.agregarError(ex);
            }
            generator.addComment("Finaliza Writeln");
            return null;
        }
    }
}
