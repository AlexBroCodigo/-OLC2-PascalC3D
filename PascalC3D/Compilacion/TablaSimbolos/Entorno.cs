using PascalC3D.Compilacion.Instrucciones.Variables;
using PascalC3D.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.TablaSimbolos
{
    class Entorno
    {
        public Hashtable functions;
        public Hashtable structs;
        public Hashtable vars;
        public Entorno anterior;
        private int size;
        public string ybreak;
        public string ycontinue;
        public string yreturn;
        public string prop;
        //public SimboloFuncion actualFunc; 
        public string ambito;
        public string nombre;

        //PARA LAS INSTRUCCIONES DE TRANSFERENCIA (BREAK,CONTINUE,RETURN)
        public bool isAuxiliar; //si es un entorno auxiliar
        public bool isFor; //si esta dentro de un for 
        public Asignacion asignacion; //actualizar la variable para ciclo FOR


        public Entorno(Entorno anterior, string ambito, string nombre)
        {
            functions = new Hashtable();
            structs = new Hashtable();
            vars = new Hashtable();
            this.anterior = anterior;
            size = anterior!=null ? anterior.size : 0;
            ybreak = anterior != null ? anterior.ybreak : null;
            ycontinue = anterior != null ? anterior.ycontinue : null;
            yreturn = anterior != null ? anterior.yreturn : null;
            prop = "main";
            //actualFunc = anterior != null ? anterior.actualFunc : null;
            this.ambito = ambito;
            this.nombre = nombre;
            isAuxiliar = false;
            isFor = false;
        }

        public Entorno(Entorno anterior,bool isAuxiliar)
        {
            this.anterior = anterior;
            this.isAuxiliar = isAuxiliar;
            isFor = false;
            ambito = null;
            nombre = null;
        }

        public Entorno(Entorno anterior, bool isFor,Asignacion asignacion)
        {
            this.anterior = anterior;
            isAuxiliar = true;
            this.isFor = isFor;
            this.asignacion = asignacion;
            ambito = null;
            nombre = null;
        }


        public Simbolo addVar(string id, Tipo type, bool isConst, bool isRef,int linea, int columna)
        {
            Entorno ent = this;
            while (ent.isAuxiliar) ent = ent.anterior;

            id = id.ToLower();
            if (!ent.vars.ContainsKey(id))
            {
                Simbolo newVar = new Simbolo(type, id, ent.size++, isConst,ent.anterior == null, isRef, linea, columna);
                ent.vars.Add(id, newVar);
                return newVar;
            }
            else throw new Error("Semántico", "Ya existe una variable con el nombre: " + id + " en el mismo entorno",ent.obtenerAmbito(),linea,columna);
        }

        public Simbolo getVar(string id,int linea, int columna)
        {
            Entorno ent = this;
            while (ent.isAuxiliar) ent = ent.anterior;

            id = id.ToLower();
            for(Entorno e = ent; e != null;e = e.anterior)
            {
                if (e.vars.ContainsKey(id)) return (Simbolo)e.vars[id];
            }
            throw new Error("Semántico","No existe la variable: " + id, ent.obtenerAmbito(), linea, columna);
        }

        public int getSize()
        {
            Entorno ent = this;
            while (ent.isAuxiliar) ent = ent.anterior;
            return ent.size;
        }


        public string obtenerAmbito()
        {
            //Me va a buscar el ambito mas cercano que tenga:global, funcion o procedimiento
            string retorno = "";
            for (Entorno e = this; e != null; e = e.anterior)
            {
                if (e.ambito != null)
                {
                    retorno = e.ambito + ": " + e.nombre;
                    return retorno;
                }
            }
            return retorno;
        }

    }
}
