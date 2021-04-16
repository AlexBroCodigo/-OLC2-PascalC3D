using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.TablaSimbolos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Utils
{
    class Retorno
    {
        private string value;
        public bool isTemp;
        public Tipo type;
        public string trueLabel;
        public string falseLabel;
        public Simbolo symbol;
        public bool vieneDesdeObjeto;

        public Retorno(string value,bool isTemp, Tipo type, Simbolo symbol = null,bool vieneDesdeObjeto = false)
        {
            this.value = value;
            this.isTemp = isTemp;
            this.type = type;
            this.symbol = symbol;
            this.vieneDesdeObjeto = vieneDesdeObjeto; 
            trueLabel = falseLabel = "";
        }

        public string getValue()
        {
            Generator.getInstance().freeTemp(value);
            return value;
        }

        public string valorToString()
        {
            return value;
        }

    }
}
