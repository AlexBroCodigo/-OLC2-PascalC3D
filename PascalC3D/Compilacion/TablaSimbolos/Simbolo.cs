using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.TablaSimbolos
{
    class Simbolo
    {
        public Tipo type;
        public string identificador;
        public int position;
        public bool isConst;
        public bool isGlobal;
        public bool isHeap;
        public bool isRef;
        public int linea;
        public int columna;

        public Simbolo(Tipo type, string identificador, int position, bool isConst,bool isGlobal, bool isHeap,int linea, int columna,bool isRef = false)
        {
            this.type = type;
            this.identificador = identificador;
            this.position = position;
            this.isConst = isConst;
            this.isGlobal = isGlobal;
            this.isHeap = isHeap;
            this.isRef = isRef;
            this.linea = linea;
            this.columna = columna;
        }

        

    }
}
