using PascalC3D.Compilacion.Instrucciones.Functions;
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
        public int size;
        public LinkedList<string> ybreak;
        public LinkedList<string> ycontinue;
        public string yreturn;
        public string prop;
        public SimboloFunction actualFunc; 
        public string ambito;
        public string nombre;

        //PARA LAS INSTRUCCIONES DE TRANSFERENCIA (BREAK,CONTINUE,RETURN)
        public LinkedList<IteFor> fors; //PARA ALMACENAR LAS ACTUALIZACIONES DE VARIABLES DE LOS CICLOS FOR QUE NOS SERVIRA EN LOS CONTINUES

        public Entorno(Entorno anterior, string ambito, string nombre)
        {
            functions = new Hashtable();
            structs = new Hashtable();
            vars = new Hashtable();
            this.anterior = anterior;
            size = anterior!=null ? anterior.size : 0;
            ybreak = new LinkedList<string>();
            ycontinue = new LinkedList<string>();
            yreturn = anterior != null ? anterior.yreturn : null;
            this.prop = "main";
            actualFunc = anterior != null ? anterior.actualFunc : null;
            this.ambito = ambito;
            this.nombre = nombre;
            fors = new LinkedList<IteFor>();
        }

        public Simbolo addVar(string id, Tipo type, bool isConst, bool isHeap,int linea, int columna)
        {
            id = id.ToLower();
            if (!vars.ContainsKey(id))
            {
                Simbolo newVar = new Simbolo(type, id,size++, isConst,anterior == null, isHeap, linea, columna);
                vars.Add(id, newVar);
                return newVar;
            }
            else throw new Error("Semántico", "Ya existe una variable con el nombre: " + id + " en el mismo entorno",obtenerAmbito(),linea,columna);
        }

        public Simbolo addVarRef(string id, Tipo type, bool isConst, bool isHeap, int linea, int columna,bool isRef)
        {
            id = id.ToLower();
            if (!vars.ContainsKey(id))
            {
                Simbolo newVar = new Simbolo(type, id, size++, isConst, anterior == null, isHeap, linea, columna,isRef);
                vars.Add(id, newVar);
                this.size++;
                return newVar;
            }
            else throw new Error("Semántico", "Ya existe una variable con el nombre: " + id + " en el mismo entorno", obtenerAmbito(), linea, columna);
        }


        public Simbolo getVar(string id,int linea, int columna)
        {
            id = id.ToLower();
            for(Entorno e = this; e != null;e = e.anterior)
            {
                if (e.vars.ContainsKey(id)) return (Simbolo)e.vars[id];
            }
            throw new Error("Semántico","No existe la variable: " + id, obtenerAmbito(), linea, columna);
        }

        public bool addStruct(string id, int size, LinkedList<Param> prams)
        {
            if (this.structs.ContainsKey(id.ToLower())) return false;
            this.structs.Add(id.ToLower(), new SimboloStruct(id.ToLower(),size, prams));
            return true;
        }

        public SimboloStruct structExists(string id)
        {
            if (this.structs.ContainsKey(id.ToLower())) return (SimboloStruct)this.structs[id.ToLower()];
            return null;
        }

        public SimboloStruct searchStruct(string id)
        {
            id = id.ToLower();
            for(Entorno e = this;e != null; e = e.anterior)
            {
                if(e.structs.ContainsKey(id)) return (SimboloStruct)e.structs[id]; 
            }
            return null;
        }

        public SimboloStruct getStruct(string id)
        {
            Entorno ent = getGlobal();
            if (ent.structs.ContainsKey(id.ToLower())) return (SimboloStruct)ent.structs[id.ToLower()];
            return null;
        }

        public bool addFunc(FunctionSt func,string uniqueId)
        {
            if (this.functions.ContainsKey(func.id.ToLower())) return false;
            this.functions.Add(func.id.ToLower(), new SimboloFunction(func, uniqueId));
            return true;
        }

        public SimboloFunction getFunc(string id)
        {
            if (this.functions.ContainsKey(id.ToLower())) return (SimboloFunction)this.functions[id.ToLower()];
            return null;
        }

        public SimboloFunction searchFunc(string id)
        {
            id = id.ToLower();
            for (Entorno e = this; e != null; e = e.anterior)
            {
                if(e.functions.ContainsKey(id)) return (SimboloFunction)e.functions[id];
            }
            return null;
        }


        public void setEntornoFunc(string prop,SimboloFunction actualFunc,string ret)
        {
            this.size = 1; //1 porque la posicion 0 es para el return
            this.prop = prop;
            this.yreturn = ret;
            this.actualFunc = actualFunc;
        }



        /* UTILIDADES */
        public Entorno getGlobal()
        {
            Entorno ent = this;
            while (ent.anterior != null) ent = ent.anterior;
            return ent;
        }

        public int getSize()
        {
            return this.size;
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
