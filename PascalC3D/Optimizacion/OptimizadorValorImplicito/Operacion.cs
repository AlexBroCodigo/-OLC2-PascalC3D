using PascalC3D.Optimizacion.OptimizadorAST;
using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorValorImplicito
{

    class Operacion : Expresion
    {
        public enum TIPO_OPERACION
        {
            SUMA = 1,
            RESTA = 2,
            MULTIPLICACION = 3,
            DIVISION = 4,
            MODULO = 5,
            MAYOR_QUE = 6,
            MENOR_QUE = 7,
            MAYOR_IGUA_QUE = 8,
            MENOR_IGUA_QUE = 9,
            IGUAL_IGUAL = 10,
            DIFERENTE_QUE = 11,
            PRIMITIVO = 12,
            ID = 13
        }

        public TIPO_OPERACION tipo;
        public Operacion operadorIzq;
        public Operacion operadorDer;
        public object valor;
        public int linea;
        public int columna;

        public Operacion()
        {
            tipo = 0;
            operadorIzq = null;
            operadorDer = null;
            valor = null;
            linea = 0;
            columna = 0;
        }

        public void Primitivo(object valor)
        {
            this.tipo = TIPO_OPERACION.PRIMITIVO;
            this.valor = valor;
        }

        public void Identificador(object valor,int linea, int columna)
        {
            this.tipo = TIPO_OPERACION.ID;
            this.valor = valor;
            this.linea = linea;
            this.columna = columna;
        }

        public void Operation(Operacion izq,Operacion der,TIPO_OPERACION operacion,int linea,int columna)
        {
            this.tipo = operacion;
            this.operadorIzq = izq;
            this.operadorDer = der;
            this.linea = linea;
            this.columna = columna;
        }

        public OptimizacionResultado optimizarCodigo()
        {
            string antes = generarAugus();
            OptimizacionResultado resultado = new OptimizacionResultado();
            resultado.codigo = antes;
            return resultado;
        }

        public string generarAugus()
        {
            //PRIMITIVOS
            if (this.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                Primitivo primvalor = (Primitivo)this.valor;
                return primvalor.generarAugus();
            }
            //IDENTIFICADORES
            else if (this.tipo == TIPO_OPERACION.ID)
            {
                Simbolo simbolo = new Simbolo(this.valor.ToString(), this.linea, this.columna);
                return simbolo.generarAugus();
            }
            //SUMA
            else if (this.tipo == TIPO_OPERACION.SUMA) return this.operadorIzq.generarAugus() + "+" + this.operadorDer.generarAugus();

            //RESTA
            else if (this.tipo == TIPO_OPERACION.RESTA) return this.operadorIzq.generarAugus() + "-" + this.operadorDer.generarAugus();

            //MULTIPLICACION
            else if (this.tipo == TIPO_OPERACION.MULTIPLICACION) return this.operadorIzq.generarAugus() + "*" + this.operadorDer.generarAugus();

            //DIVISION
            else if (this.tipo == TIPO_OPERACION.DIVISION) return this.operadorIzq.generarAugus() + "/" + this.operadorDer.generarAugus();

            //MODULO
            else if (this.tipo == TIPO_OPERACION.MODULO) return this.operadorIzq.generarAugus() + "%" + this.operadorDer.generarAugus();

            //MAYOR QUE
            else if (this.tipo == TIPO_OPERACION.MAYOR_QUE) return this.operadorIzq.generarAugus() + ">" + this.operadorDer.generarAugus();

            //MAYOR IGUAL
            else if (this.tipo == TIPO_OPERACION.MAYOR_IGUA_QUE) return this.operadorIzq.generarAugus() + ">=" + this.operadorDer.generarAugus();

            //MENOR
            else if (this.tipo == TIPO_OPERACION.MENOR_QUE) return this.operadorIzq.generarAugus() + "<" + this.operadorDer.generarAugus();

            //MENOR IGUAL
            else if (this.tipo == TIPO_OPERACION.MENOR_IGUA_QUE) return this.operadorIzq.generarAugus() + "<=" + this.operadorDer.generarAugus();

            //IGUAL
            else if (this.tipo == TIPO_OPERACION.IGUAL_IGUAL) return this.operadorIzq.generarAugus() + "==" + this.operadorDer.generarAugus();

            //DIFERENTE
            else if (this.tipo == TIPO_OPERACION.DIFERENTE_QUE) return this.operadorIzq.generarAugus() + "!=" + this.operadorDer.generarAugus();

            else return "";
        }


        public string invertirCondicion()
        {
            //IGUAL
            if (this.tipo == TIPO_OPERACION.IGUAL_IGUAL) return this.operadorIzq.generarAugus() + "!=" + this.operadorDer.generarAugus();

            //DIFERENTE
            else if (this.tipo == TIPO_OPERACION.DIFERENTE_QUE) return this.operadorIzq.generarAugus() + "==" + this.operadorDer.generarAugus();

            else return generarAugus();

        }

        //MI REGLA 5
        public bool validarRegla1(object varActual, object varAsigna,object varPrevia, object varAsignaPrevia) 
        {
            if (varAsignaPrevia == varActual && varPrevia == varAsigna) return true;
            return false;
        }

        //MI REGLA 3
        public bool validarRegla4()
        {
            if(this.operadorIzq.tipo == TIPO_OPERACION.PRIMITIVO && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorIzq.generarAugus();
                string value2 = this.operadorDer.generarAugus();
                if (value.Equals(value2)) return true;
            } 
            else if(this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.ID)
            {
                string value = this.operadorIzq.generarAugus();
                string value2 = this.operadorDer.generarAugus();
                if(value.Equals(value2)) return true;
            }
            return false;
        }

        //MI REGLA 4
        public bool validarRegla5()
        {
            if(this.operadorIzq.tipo == TIPO_OPERACION.PRIMITIVO && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorIzq.generarAugus();
                string value2 = this.operadorDer.generarAugus();
                if (!value.Equals(value2)) return true;
            }
            return false;
        }

        //MI REGLA 6
        public bool validarRegla8(string id)
        {
            if(this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                if (this.operadorIzq.valor.Equals(id))
                {
                    string value = this.operadorDer.generarAugus();
                    if (value.Equals("0")) return true;
                }
            }
            else if(this.operadorDer.tipo == TIPO_OPERACION.ID && this.operadorIzq.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                if(this.operadorDer.valor.Equals(id))
                {
                    string value = this.operadorIzq.generarAugus();
                    if (value.Equals("0"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //MI REGLA 7
        public bool validarRegla9(string id)
        {
            if(this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                if (this.operadorIzq.valor.Equals(id))
                {
                    string value = this.operadorDer.generarAugus();
                    if (value.Equals("0")) return true;
                }
            }
            return false;
        }

        //MI REGLA 8
        public bool validarRegla10(string id)
        {
            if (this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                if (this.operadorIzq.valor.Equals(id))
                {
                    string value = this.operadorDer.generarAugus();
                    if (value.Equals("1")) return true;
                }
            }
            else if (this.operadorDer.tipo == TIPO_OPERACION.ID && this.operadorIzq.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                if (this.operadorDer.valor.Equals(id))
                {
                    string value = this.operadorIzq.generarAugus();
                    if (value.Equals("1"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //MI REGLA 9
        public bool validarRegla11(string id)
        {
            if (this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                if (this.operadorIzq.valor.Equals(id))
                {
                    string value = this.operadorDer.generarAugus();
                    if (value.Equals("1")) return true;
                }
            }
            return false;
        }

        //MI REGLA 10
        public object validarRegla12()
        {
            if (this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorDer.generarAugus();
                if (value.Equals("0")) return this.operadorIzq.valor;
                
            }
            else if (this.operadorDer.tipo == TIPO_OPERACION.ID && this.operadorIzq.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorIzq.generarAugus();
                if (value.Equals("0")) return this.operadorDer.valor;
            }
            return "";
        }

        //MI REGLA 11
        public object validarRegla13()
        {
            if(this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorDer.generarAugus();
                if (value.Equals("0")) return this.operadorIzq.valor;
            }
            return "";
        }

        //MI REGLA 12
        public object validarRegla14()
        {
            if (this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorDer.generarAugus();
                if (value.Equals("1")) return this.operadorIzq.valor;
            }
            else if (this.operadorDer.tipo == TIPO_OPERACION.ID && this.operadorIzq.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorIzq.generarAugus();
                if (value.Equals("1")) return this.operadorDer.valor;
            }
            return "";
        }

        //MI REGLA 13
        public object validarRegla15()
        {
            if (this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorDer.generarAugus();
                if (value.Equals("1")) return this.operadorIzq.valor;
            }
            return "";
        }

        //MI REGLA 14
        public object validarRegla16()
        {
            if (this.operadorIzq.tipo == TIPO_OPERACION.ID && this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorDer.generarAugus();
                if (value.Equals("2")) return this.operadorIzq.valor + "+" + this.operadorIzq.valor;
            }
            return "";
        }

        //MI REGLA 15
        public string validarRegla17()
        {
            if(this.operadorDer.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorDer.generarAugus();
                if (value.Equals("0")) return "0";
            }
            else if(this.operadorIzq.tipo == TIPO_OPERACION.PRIMITIVO)
            {
                string value = this.operadorIzq.generarAugus();
                if (value.Equals("0")) return "0";
            }
            return "";
        }

        //MI REGLA 16
        public string validarRegla18()
        {
            if (this.operadorIzq.tipo == TIPO_OPERACION.PRIMITIVO && this.operadorDer.tipo == TIPO_OPERACION.ID)
            {
                string value = this.operadorIzq.generarAugus();
                if (value.Equals("0")) return "0";
            }
            return "";
        }

    }
}
