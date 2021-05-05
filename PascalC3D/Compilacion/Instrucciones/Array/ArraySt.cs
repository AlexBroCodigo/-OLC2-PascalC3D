using PascalC3D.Compilacion.Expresiones.Literal;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Instrucciones.Array
{
    class ArraySt : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private string id;
        private LinkedList<Dimension> dimensiones;
        private Tipo tipoArreglo;

        public ArraySt(string id, LinkedList<Dimension> dimensiones, Tipo tipoArreglo, int linea, int columna)
        {
            this.id = id;
            this.dimensiones = dimensiones;
            this.tipoArreglo = tipoArreglo;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                this.validarLimites(ent);
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }

        public void validarLimites(Entorno ent)
        {
            foreach(Dimension dimension in dimensiones)
            {
                Primitivo inferior = dimension.inferior;
                if (inferior.value.ToString().Contains(".")) throw new Error("Semántico","El limite inferior tiene que ser de tipo integer",ent.obtenerAmbito(),linea,columna);
                Primitivo superior = dimension.superior;
                if (superior.value.ToString().Contains(".")) throw new Error("Semántico","El limite superior tiene que ser de tipo integer", ent.obtenerAmbito(), linea, columna);
            }
        }
    }
}
