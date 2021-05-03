using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PascalC3D.Optimizacion.Reporte
{
    class ReporteOptimizacion
    {
        private LinkedList<OPtimizacion> reporte;

        public ReporteOptimizacion()
        {
            this.reporte = new LinkedList<OPtimizacion>();
        }

        public void agregarOpt(OPtimizacion opt)
        {
            this.reporte.AddLast(opt);
        }

        public bool esVacio()
        {
            return reporte.Count == 0;
        }

        //CREAR METODO PARA GENERAR EL REPORTE DE OPTIMIZACION
        public void generarReporteOptimizacion()
        {
            string css = estiloTabla();
            generarArchivoEstiloTabla(css);
            string html = escribirTablaOptimizacion();
            generarArchivoOptimizacion(html);
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

        private string escribirTablaOptimizacion()
        {
            string html = "<!Doctype html>\n<html lang=\"es-Es\">\n<head>\n";
            html += "<link rel=\"stylesheet\" href=\"estiloTabla.css\">\n";
            html += "<title>Reporte Optimizacion</title>\n</head>\n<body>\n<h1><center>Reporte de Optimización</center></h1>\n<table style=\"margin: 0 auto;\">\n";
            html += "<thead>\n<tr>\n<th>Tipo</th>\n<th>Regla</th>\n<th>Código eliminado</th>\n<th>Código agregado</th>\n<th>Fila</th>\n</tr>\n</thead>\n<tbody>\n";
            //RECORRERMOS EL REPORTE
            foreach(OPtimizacion opti in reporte)
            {
                html += "<tr>\n";
                html += "<td>" + opti.tipo + "</td>\n";
                html += "<td>" + opti.regla + "</td>\n";
                html += "<td>" + opti.antes + "</td>\n";
                html += "<td>" + opti.despues + "</td>\n";
                html += "<td>" + opti.linea + "</td>\n";
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

        private void generarArchivoOptimizacion(string html)
        {
            TextWriter archivo;
            archivo = new StreamWriter("C:\\compiladores2\\ReporteOpti.html");
            archivo.WriteLine(html);
            archivo.Close();
        }

    }
}
