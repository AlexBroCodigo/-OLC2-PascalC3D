using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PascalC3D.Utils
{
    class Errores
    {
        private LinkedList<Error> errores;
        public Errores()
        {
            errores = new LinkedList<Error>();
        }

        public void agregarError(Error error)
        {
            error.linea++;
            error.columna++;
            errores.AddLast(error);
        }

        public bool esVacio()
        {
            return errores.Count == 0;
        }

        public void generarReporteErrores(int boton)
        {
            //1: Compilar; 2:Optimizar;
            string css = estiloErrores();
            generarArchivoEstiloErrores(css);
            if (boton == 1)
            {
                string html = escribirReporteErrores1();
                generarArchivoErrores1(html);
            }
            else
            {
                string html = escribirReporteErrores2();
                generarArchivoErrores2(html);
            }
        }

        private string estiloErrores()
        {
            string css = "body {background-color: #d0efb141;font-family: calibri, Helvetica, Arial;}\n";
            css += "h1 {text-align: center;font-size: 100px;}\n";
            css += "table {width: 100%;border-collapse: collapse;font-size: 25px;font-weight: bold;}\n";
            css += "table td, table th {border: 0px dashed #77A6B6;padding: 10px;}\n";
            css += "table tr:nth-child(even){ background-color: #9DC3C2; }\n";
            css += "table tr:nth-child(odd){ background-color: #FDE05B; }\n";
            css += "table tr:hover {background-color: #77A6B6;color: #feffff;}\n";
            css += "table th {color: white;background-color: #E32305;text-align: left;padding-top: 12px;padding-bottom: 12px;}\n";
            css += ".content {width: 90%;margin: 0 auto;}";
            return css;
        }

        private void generarArchivoEstiloErrores(String css)
        {
            TextWriter archivo;
            archivo = new StreamWriter("C:\\compiladores2\\estiloErrores.css");
            archivo.WriteLine(css);
            archivo.Close();
        }

        private string escribirReporteErrores1()
        {
            string html = "<!Doctype html>\n<html lang=\"es-Es\">\n<head>\n";
            html += "<link rel=\"stylesheet\" href=\"estiloErrores.css\">\n";
            html += "<title>Reporte de Errores</title>\n</head>\n<body>\n<h1><center>Reporte de Errores en Compilación</center></h1>\n<table style=\"margin: 0 auto;\">\n";
            html += "<thead>\n<tr>\n<th>Tipo</th>\n<th>Descripción</th>\n<th>Ambito</th>\n<th>Linea</th>\n<th>Columna</th>\n</tr>\n</thead>\n<tbody>\n";
            foreach (Error error in errores)
            {
                html += "<tr>\n";
                html += "<td>" + error.tipo + "</td>\n";
                html += "<td>" + error.descripcion + "</td>\n";
                html += "<td>" + error.ambito + "</td>\n";
                html += "<td>" + error.linea + "</td>\n";
                html += "<td>" + error.columna + "</td>\n";
                html += "</tr>\n";
            }
            html += "</tbody>\n</table>\n</body>\n</html>";
            return html;
        }

        private void generarArchivoErrores1(String html)
        {
            TextWriter archivo;
            archivo = new StreamWriter("C:\\compiladores2\\CompiErrores.html");
            archivo.WriteLine(html);
            archivo.Close();
        }

        private string escribirReporteErrores2()
        {
            string html = "<!Doctype html>\n<html lang=\"es-Es\">\n<head>\n";
            html += "<link rel=\"stylesheet\" href=\"estiloErrores.css\">\n";
            html += "<title>Reporte de Errores</title>\n</head>\n<body>\n<h1><center>Reporte de Errores en Optimización</center></h1>\n<table style=\"margin: 0 auto;\">\n";
            html += "<thead>\n<tr>\n<th>Tipo</th>\n<th>Descripción</th>\n<th>Ambito</th>\n<th>Linea</th>\n<th>Columna</th>\n</tr>\n</thead>\n<tbody>\n";
            foreach (Error error in errores)
            {
                html += "<tr>\n";
                html += "<td>" + error.tipo + "</td>\n";
                html += "<td>" + error.descripcion + "</td>\n";
                html += "<td>" + error.ambito + "</td>\n";
                html += "<td>" + error.linea + "</td>\n";
                html += "<td>" + error.columna + "</td>\n";
                html += "</tr>\n";
            }
            html += "</tbody>\n</table>\n</body>\n</html>";
            return html;
        }

        private void generarArchivoErrores2(String html)
        {
            TextWriter archivo;
            archivo = new StreamWriter("C:\\compiladores2\\OptiErrores.html");
            archivo.WriteLine(html);
            archivo.Close();
        }

    }
}
