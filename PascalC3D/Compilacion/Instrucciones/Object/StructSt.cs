using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Instrucciones.Object
{
    class StructSt : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private string id;
        private LinkedList<Param> attributes;

        public StructSt(string id, LinkedList<Param> attributes, int linea, int columna)
        {
            this.id = id;
            this.attributes = attributes;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                if (!ent.addStruct(this.id, this.attributes.Count, this.attributes)) throw new Error("Semántico", "Ya existe un object con el id: " + this.id, ent.obtenerAmbito(), linea, columna);
                this.validateParams(ent);
            }
            catch (Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }

        private void validateParams(Entorno ent)
        {
            LinkedList<string> set = new LinkedList<string>();
            foreach(Param param in attributes)
            {
                if (set.Contains(param.id.ToLower())) throw new Error("Semántico","Ya existe un atributo con el nombre: " + param.id + " en el object: " + this.id, ent.obtenerAmbito(), linea, columna);
                if(param.type.tipo == Tipos.STRUCT)
                {
                    SimboloStruct @struct = ent.structExists(param.type.tipoId);
                    if (@struct == null) throw new Error("Semántico","No existe el object: " + param.type.tipoId + "para el atributo: " + param.id, ent.obtenerAmbito(), linea, columna);
                    param.type.symStruct = @struct;
                }
                set.AddLast(param.id.ToLower());
            }
        }


    }
}
