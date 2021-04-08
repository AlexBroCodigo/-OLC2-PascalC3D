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
    class DeclaConstante : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private string id;
        private Expresion value;

        public DeclaConstante(string id, Expresion value, int linea, int columna)
        {
            this.id = id;
            this.value = value;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                Generator generator = Generator.getInstance();
                Retorno value = this.value.compilar(ent);
                Simbolo newVar = ent.addVar(id, value.type, true, false, linea, columna);
                if (newVar.isGlobal)
                {
                    if(newVar.type.tipo == Tipos.BOOLEAN)
                    {
                        string templabel = generator.newLabel();
                        generator.addLabel(value.trueLabel);
                        generator.addSetStack(""+newVar.position,"1");
                        generator.addGoto(templabel);
                        generator.addLabel(value.falseLabel);
                        generator.addSetStack(""+newVar.position,"0");
                        generator.addLabel(templabel);
                    } else
                    {
                        generator.addSetStack(""+newVar.position, value.getValue());
                    }
                }
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
