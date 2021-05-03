using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorAST
{
    class AST
    {
        public LinkedList<Etiqueta> instrucciones;
        public LinkedList<Etiqueta> etiquetas;
        public LinkedList<string> etiquetasBetadas;

        public AST(LinkedList<Etiqueta> instrucciones)
        {
            this.instrucciones = instrucciones;
            etiquetas = new LinkedList<Etiqueta>();
            etiquetasBetadas = new LinkedList<string>();
        }

        public bool existeEtiqueta(string id)
        {
            foreach(Etiqueta etiqueta in this.etiquetas)
            {
                bool comparacion = etiqueta.id.Equals(id);
                if (comparacion) return true;
            }
            return false;
        }

        public void agregarEtiqueta(Etiqueta etiqueta)
        {
            etiquetas.AddLast(etiqueta);
        }

        public Etiqueta obtenerEtiqueta(string texto)
        {
            foreach(Etiqueta etiqueta in etiquetas)
            {
                if (etiqueta.id.Equals(texto)) return etiqueta;
            }
            return null;
        }

        public Etiqueta obtenerSiguienteEtiqueta(string texto)
        {
            int contador = 0;
            foreach(Etiqueta etiqueta in etiquetas)
            {
                if (etiqueta.id.Equals(texto))
                {
                    if (etiquetas.Count > contador + 1) return etiquetas.ElementAt(contador + 1);
                }
                contador++;
            }
            return null;
        }

    }
}
