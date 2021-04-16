using PascalC3D.Compilacion.Expresiones.Asignacion;
using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Instrucciones.Transfer;
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
                Generator generator = Generator.getInstance();
                generator.addComment("Inicia Asignacion");
                
                if (ent.ambito.Equals("FUNCTION"))
                {
                    SimboloFunction symFunc = ent.actualFunc;
                    if (symFunc != null)
                    {
                        AsignacionId mytarget = (AsignacionId)this.target;
                        if (mytarget.id.ToLower().Equals(symFunc.id.ToLower())) //Es un Return
                        {
                            Return @return = new Return(this.value, linea, columna);
                            @return.isAsignacion = true;
                            return @return.compilar(ent, errores);
                        }
                    }
                }

                Retorno target = this.target.compilar(ent);
                Retorno value = this.value.compilar(ent);
                
                Simbolo symbol = target.symbol;
                if (symbol.isConst) throw new Error("Semántico", "No se puede cambiar el valor de una constante", ent.obtenerAmbito(), linea, columna);
                if (!sameType(target.type, value.type)) throw new Error("Semántico", "No coincide el tipo de dato de la variable con el tipo de valor a asignar", ent.obtenerAmbito(), linea, columna);
                if (symbol.isHeap == false) //ES GLOBAL
                {
                    if (target.type.tipo == Tipos.BOOLEAN)
                    {
                        //Mi modificacion
                        if (value.getValue().Equals("1")) generator.addGoto(value.trueLabel);
                        else generator.addGoto(value.falseLabel);

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
                else if (symbol.isHeap)
                {
                    if (target.type.tipo == Tipos.BOOLEAN)
                    {
                        //Mi modificacion
                        if (value.getValue().Equals("1")) generator.addGoto(value.trueLabel);
                        else generator.addGoto(value.falseLabel);

                        string templabel = generator.newLabel();
                        generator.addLabel(value.trueLabel);
                        generator.addSetHeap(target.getValue(), "1");
                        generator.addGoto(templabel);
                        generator.addLabel(value.falseLabel);
                        generator.addSetHeap(target.getValue(), "0");
                        generator.addLabel(templabel);
                    }
                    else
                    {
                        generator.addSetHeap(target.getValue(), value.getValue());
                    }
                }
                generator.addComment("Finaliza Asignacion");
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
                if (type1.tipo == Tipos.STRUCT) return type1.tipoId.ToLower().Equals(type2.tipoId.ToLower());
                return true;
            }
            return false;
        }


    }
}
