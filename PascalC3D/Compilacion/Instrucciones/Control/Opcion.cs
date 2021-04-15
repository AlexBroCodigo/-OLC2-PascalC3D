using PascalC3D.Compilacion.Expresiones.Logica;
using PascalC3D.Compilacion.Expresiones.Relacional;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalC3D.Compilacion.Instrucciones.Control
{
    class Opcion : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private LinkedList<Expresion> etiquetas;
        private LinkedList<Instruccion> sentencias;
        public Expresion variable;

        public Opcion(LinkedList<Expresion> etiquetas, LinkedList<Instruccion> sentencias, int linea, int columna)
        {
            this.etiquetas = etiquetas;
            this.sentencias = sentencias;
            this.linea = linea;
            this.columna = columna;
            variable = null;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            //CREO MIS UTILIDADES (CREO UN IF CON SU RESPECTIVA CONDICION)
            //PARA LA CONDICION
            Expresion condicion;
            if(etiquetas.Count == 1)
            {
                Expresion right = etiquetas.ElementAt(0);
                Equals igual = new Equals(variable,right,linea,columna);
                condicion = (Expresion)igual;
            } else //N etiquetas creo N Or
            {
                Or oranterior = null;
                Or oractual = null;
                Equals primero = null;
                
                for(int i = 0; i < etiquetas.Count; i++)
                {
                    Expresion etiqueta = etiquetas.ElementAt(i);
                    if(i == 0)
                    {
                        primero = new Equals(variable, etiqueta, linea, columna);
                    } else if(i == 1)
                    {
                        Equals segundo = new Equals(variable, etiqueta, linea, columna);
                        oractual = new Or(primero, segundo, linea, columna);
                    } else
                    {
                        Equals nuevo = new Equals(variable, etiqueta, linea, columna);
                        oranterior = oractual;
                        oractual = new Or(oranterior,nuevo,linea,columna);
                    }
                }
                condicion = oractual;
            }
            //PARA EL IF
            If miif = new If(condicion, sentencias, null, linea, columna);
            //COMPILACION
            return miif;
        }
    }
}
