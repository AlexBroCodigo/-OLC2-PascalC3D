using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Instrucciones.Transfer
{
    class Break : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        public Break(int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                if (ent.ybreak == null) throw new Error("Semántico","break no viene dentro de un ciclo",ent.obtenerAmbito(),linea,columna);
                Generator.getInstance().addGoto(ent.ybreak);
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
