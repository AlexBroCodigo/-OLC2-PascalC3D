using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.TablaSimbolos
{
    class TableSymbol
    {

        public TableSymbol()
        {

        }

        public void generarTablaSimbolos(Entorno ent)
        {
            string css = estiloTabla();
            generarArchivoEstiloTabla(css);
            string html = escribirTablaSimbolos(ent);
            generarArchivoTabla(html);
        }

        private string estiloTabla()
        {
            string css = "body {background-color: #d0efb141;font-family: calibri, Helvetica, Arial;}\n";
            css += "h1 {text-align: center;font-size: 100px;}\n";
            css += "table {width: 100%;border-collapse: collapse;font-size: 25px;font-weight: bold;}\n";
            css += "table td, table th, table caption {border: 0px dashed #77A6B6;padding: 10px;}\n";
            css += "table tr:nth-child(even){ background-color: #9DC3C2; }\n";
            css += "table tr:nth-child(odd){ background-color: #B3D89C; }\n";
            css += "table tr:hover {background-color: #77A6B6;color: #feffff;}\n";
            css += "table th, table caption {color: white;background-color: #4d7298;text-align: left;padding-top: 12px;padding-bottom: 12px;}\n";
            css += ".content {width: 90%;margin: 0 auto;}";
            return css;
        }

        private string escribirTablaSimbolos(Entorno ent)
        {
            string html = "<!Doctype html>\n<html lang=\"es-Es\">\n<head>\n";
            html += "<link rel=\"stylesheet\" href=\"estiloTabla.css\">\n";
            html += "<title>Tabla de Simbolos</title>\n</head>\n<body>\n<h1><center>Tabla de Simbolos en Compilación</center></h1>\n<table style=\"margin: 0 auto;\">\n";
            html += "<caption>Variables globales declaradas</caption>\n";
            html += "<thead>\n<tr>\n<th>Nombre</th>\n<th>Tipo</th>\n<th>Posicion</th>\n<th>Es Constante</th>\n<th>Linea</th>\n<th>Columna</th>\n</tr>\n</thead>\n<tbody>\n";
            //RECORRERMOS LAS VARIABLES DEL ENTORNO GLOBAL
            foreach(DictionaryEntry variable in ent.vars)
            {
                Simbolo simbolo = (Simbolo)variable.Value;
                html += "<tr>\n";
                html += "<td>" + simbolo.identificador + "</td>\n";
                if (simbolo.type.tipo == Tipos.STRUCT) html += "<td>" + simbolo.type.tipoToString() + ":" + simbolo.type.tipoId + "</td>\n";
                else html += "<td>" + simbolo.type.tipoToString() + "</td>\n";
                html += "<td>" + simbolo.position + "</td>\n";
                if (simbolo.isConst) html += "<td>Si</td>\n";
                else html += "<td>No</td>\n";
                html += "<td>" + (simbolo.linea+1) + "</td>\n";
                html += "<td>" + (simbolo.columna+1) + "</td>\n";
                html += "</tr>\n";
            }
            html += "</tbody>\n</table>\n";
            html += "</br></br>\n";

            html += "<table style=\"margin: 0 auto;\">\n<caption>Procedimientos/Funciones declaradas</caption>\n";
            html += "<thead>\n<tr>\n<th>Nombre</th>\n<th>Tipo de Retorno</th>\n<th>PRO o FUN Padre</th>\n<th># Parametros</th>\n<th>Linea</th>\n<th>Columna</th>\n</tr>\n</thead>\n<tbody>\n";
            //RECORREMOS LAS FUNCIONES
            foreach (DictionaryEntry funcion in ent.functions)
            {
                SimboloFunction simbolo = (SimboloFunction)funcion.Value;
                html += "<tr>\n";
                html += "<td>" + simbolo.id + "</td>\n";
                if (simbolo.type.tipo == Tipos.STRUCT) html += "<td>" + simbolo.type.tipoToString() + ":" + simbolo.type.tipoId + "</td>\n";
                else  html += "<td>" + simbolo.type.tipoToString() + "</td>\n";
                html += "<td>--</td>\n";
                html += "<td>" + simbolo.size + "</td>\n";
                html += "<td>" + (simbolo.linea + 1) + "</td>\n";
                html += "<td>" + (simbolo.columna + 1) + "</td>\n";
                html += "</tr>\n";
            }
            html += "</tbody>\n</table>\n</body>\n</html>";
            return html;
        }

        private void generarArchivoEstiloTabla(string css)
        {
            TextWriter archivo;
            archivo = new StreamWriter("C:\\compiladores2\\estiloTabla.css");
            archivo.WriteLine(css);
            archivo.Close();
        }

        private void generarArchivoTabla(string html)
        {
            TextWriter archivo;
            archivo = new StreamWriter("C:\\compiladores2\\CompiTabla.html");
            archivo.WriteLine(html);
            archivo.Close();
        }



    }
}
