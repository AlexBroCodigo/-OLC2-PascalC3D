using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Instrucciones.Variables;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Instrucciones.Transfer
{
    class Continue : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        public Continue(int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                if (ent.ycontinue.Count == 0) throw new Error("Semántico", "continue no viene dentro de un ciclo", ent.obtenerAmbito(), linea, columna);
                IteFor @for = ent.fors.Last.Value;
                if (@for.isFor)
                {
                    Asignacion actualizarVariable = @for.actualizarVariable;
                    actualizarVariable.compilar(ent, errores);
                }
                Generator.getInstance().addGoto(ent.ycontinue.Last.Value);
            }
            catch (Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
