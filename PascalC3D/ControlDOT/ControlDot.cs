using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.ControlDOT
{
    class ControlDot
    {
        /* Estructura del archivo:
         * digraph ArbolAST{
         * node[shape=box, color=turquoise4];
         * edge[color=blue4];
         * nodo0[label = "Mi kasa"];
         * nodo1[label = "Mi kasa"];
         * nodo0 -> nodo1;
         * nodo2[label = "Mi kasa"];
         * nodo0 -> nodo2;
         * nodo3[label = "Mi kasa"];
         * nodo0 -> nodo3;
         * }
        */

        private static int contador;
        private static String grafo;

        public static String getDOT(ParseTreeNode raiz)
        {
            grafo = "digraph ArbolAST{\n ";
            grafo += "node[shape=box, color=" + "\"#A200FF\"" + "];\n";
            grafo += "edge[color=" + "\"#008FFF\"" + "];\n ";
            grafo += "nodo0[label=\"" + escapar(raiz.ToString()) + "\"];\n ";
            contador = 1;
            recorrerAST("nodo0", raiz);
            grafo += "}";
            return grafo;
        }

        private static void recorrerAST(String padre, ParseTreeNode raiz)
        {
            foreach (ParseTreeNode hijo in raiz.ChildNodes)
            {
                String nombreHijo = "nodo" + contador.ToString();
                grafo += nombreHijo + "[label=\"" + escapar(hijo.ToString()) + "\"];\n ";
                grafo += padre + "->" + nombreHijo + ";\n ";
                contador++;
                recorrerAST(nombreHijo, hijo);
            }
        }
        
        private static String escapar(String cadena)
        {
            cadena = cadena.Replace("\\", "\\\\");
            cadena = cadena.Replace("\"", "\\\"");
            return cadena;
        }

    }
}
