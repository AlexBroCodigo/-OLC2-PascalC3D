using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Instrucciones.Transfer
{
    class Return : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion value;

        public Return(Expresion value, int linea, int columna)
        {
            this.value = value;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            Retorno value;
            if (this.value == null) value = new Retorno("0", false, new Tipo(Tipos.VOID));
            else value = this.value.compilar(ent);
            SimboloFunction symFunc = ent.actualFunc;
            Generator generator = Generator.getInstance();

            if (symFunc == null) throw new Error("Semántico","Exit no esta dentro de una función",ent.obtenerAmbito(),linea,columna);
            if (!this.sameType(symFunc.type, value.type)) throw new Error("Semántico", "Se esperaba un " + symFunc.type.tipoToString() + " y se obtuvo un " + value.type.tipoToString(),ent.obtenerAmbito(),linea,columna);
            if (symFunc.type.tipo == Tipos.BOOLEAN)
            {
                string templabel = generator.newLabel();
                generator.addLabel(value.trueLabel);
                generator.addSetStack("SP", "1");
                generator.addGoto(templabel);
                generator.addLabel(value.falseLabel);
                generator.addSetStack("SP","0");
                generator.addLabel(templabel);
            }
            else if (symFunc.type.tipo != Tipos.VOID) generator.addSetStack("SP", value.getValue());

            generator.addGoto(ent.yreturn);
            return null;
        }

        public bool sameType(Tipo type1, Tipo type2)
        {
            if (type1.tipo == type2.tipo)
            {
                if (type1.tipo == Tipos.STRUCT) return type1.tipoId.ToLower().Equals(type2.tipoId.ToLower());
                return true;
            }
            else if (type1.tipo == Tipos.STRUCT || type2.tipo == Tipos.STRUCT)
            {
                if (type1.tipo == Tipos.VOID || type2.tipo == Tipos.VOID) return true;
            }
            return false;
        }

    }
}
