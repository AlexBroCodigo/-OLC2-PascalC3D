using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Instrucciones.Variables
{
    class Asignacion : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion target;
        private Expresion value;

        public Asignacion(Expresion target, Expresion value, int linea, int columna)
        {
            this.target = target;
            this.value = value;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                Retorno target = this.target.compilar(ent);
                Retorno value = this.value.compilar(ent);

                Generator generator = Generator.getInstance();
                Simbolo symbol = target.symbol;
                if (symbol.isConst) throw new Error("Semántico", "No se puede cambiar el valor de una constante", ent.obtenerAmbito(), linea, columna);
                if (!sameType(target.type, value.type)) throw new Error("Semántico", "No coincide el tipo de dato de la variable con el tipo de valor a asignar", ent.obtenerAmbito(), linea, columna);
                if (symbol.isGlobal)
                {
                    if (target.type.tipo == Tipos.BOOLEAN)
                    {
                        string templabel = generator.newLabel();
                        generator.addLabel(value.trueLabel);
                        generator.addSetStack("" + symbol.position, "1");
                        generator.addGoto(templabel);
                        generator.addLabel(value.falseLabel);
                        generator.addSetStack("" + symbol.position, "0");
                        generator.addLabel(templabel);
                    }
                    else
                    {
                        generator.addSetStack("" + symbol.position, value.getValue());
                    }
                }
                else if (symbol.isHeap)
                {
                    if (target.type.tipo == Tipos.BOOLEAN)
                    {
                        string templabel = generator.newLabel();
                        generator.addLabel(value.trueLabel);
                        generator.addSetHeap("" + symbol.position, "1");
                        generator.addGoto(templabel);
                        generator.addLabel(value.falseLabel);
                        generator.addSetHeap("" + symbol.position, "0");
                        generator.addLabel(templabel);
                    }
                    else
                    {
                        generator.addSetHeap(target.getValue(), value.getValue());
                    }
                }
                else
                {
                    if (target.type.tipo == Tipos.BOOLEAN)
                    {
                        string templabel = generator.newLabel();
                        generator.addLabel(value.trueLabel);
                        generator.addSetStack(target.getValue(), "1");
                        generator.addGoto(templabel);
                        generator.addLabel(value.falseLabel);
                        generator.addSetStack(target.getValue(), "0");
                        generator.addLabel(templabel);
                    }
                    else
                    {
                        generator.addSetStack(target.getValue(), value.getValue());
                    }
                }
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }


        private bool sameType(Tipo type1,Tipo type2)
        {
            if(type1.tipo == type2.tipo)
            {
                if (type1.tipo == Tipos.OBJECT) return type1.tipoId.ToLower().Equals(type2.tipoId.ToLower());
                return true;
            }
            else if(type1.tipo == Tipos.OBJECT || type2.tipo == Tipos.OBJECT)
            {
                if (type1.tipo == Tipos.VOID || type2.tipo == Tipos.VOID) return true;
            }
            return false;
        }


    }
}
