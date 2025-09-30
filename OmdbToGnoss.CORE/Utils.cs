using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper;
using System.Xml;
using System.Runtime.CompilerServices;
using OmdbToGnoss.CORE;

namespace OmdToGnoss.CORE
{
    public class Utils
    {
        const string MESSAGE_CONNFIG_OAuthKO = "No encontrado";

        public Utils() { }
        internal static List<string> ObtenerOntologiasComunidad()
        {
            var mResourceApi = ApiManager.Instance.ResourceApi;
            var mCommunityApi = ApiManager.Instance.CommunityApi;
            List<string> ontologias = new List<string>();
            mResourceApi.ChangeOntology(mCommunityApi.GetCommunityId().ToString());           
            try
            {
                SparqlObject resultados = mResourceApi.VirtuosoQuery("Select distinct ?ontologia", "Where { ?s a ?ontologia }", mCommunityApi.GetCommunityId());
                foreach (var resultado in resultados.results.bindings)
                {
                    ontologias.Add(resultado["ontologia"].value);
                }
            }
            catch (Exception e)
            {
                mResourceApi.Log.Error($"Error al hacer la consulta a Virtuoso: {e.Message} -> {e.StackTrace}");
            }

            return ontologias;
        }

        internal static List<string>? ObtenerOntologiasEquipo(string? pathLocalOntologias)
        {
            if (pathLocalOntologias is null)
            {
                pathLocalOntologias = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"OWLs"));
            }

            string[] archivosOWLs = Directory.GetFiles(pathLocalOntologias, "*.owl");

            // Extraer solo los nombres de archivo SIN extensión utilizando Path.GetFileNameWithoutExtension
            List<string> listaOWLs = new List<string>();
            foreach (string rutaArchivo in archivosOWLs)
            {
                listaOWLs.Add(Path.GetFileNameWithoutExtension(rutaArchivo));
            }

            // Alternativa con LINQ (más concisa):
            // List<string> listaOWLs = archivosOWLs.Select(Path.GetFileNameWithoutExtension).ToList();

            if (listaOWLs.Count == 0)
            {
                return null;
            }

            return listaOWLs;
        }

        /// <summary>
        /// Limpia el contenido de los archivos CSV de caché según las preferencias del usuario,
        /// manteniendo únicamente la línea de encabezados en cada archivo seleccionado.
        /// </summary>
        /// <remarks>
        /// Este método solicita al usuario confirmación para limpiar tres tipos de recursos:
        /// películas, personas y géneros. Para cada tipo confirmado, elimina todos los datos
        /// del archivo CSV correspondiente, preservando la primera línea (encabezados).
        /// </remarks>
        internal static void LimpiarCsvCacheInterna()
        {
            Console.WriteLine($"\n-------------------------------------------");

            // Solicitar confirmaciones
            var confirmaciones = new Dictionary<string, bool>
            {
                ["Peliculas"] = RealizarNuevaCargaDesdeCero("películas"),
                ["Personas"] = RealizarNuevaCargaDesdeCero("personas"),
                ["Generos"] = RealizarNuevaCargaDesdeCero("generos")
            };

            // Si no hay nada que procesar, salir
            if (!confirmaciones.Any(c => c.Value))
                return;

            // Obtener archivos CSV
            string dataPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));
            string[] archivos = Directory.GetFiles(dataPath, "*.csv");
            int archivosLimpiados = 0;

            // Procesar archivos secuencialmente para evitar problemas de concurrencia
            foreach (string archivo in archivos)
            {
                string nombreArchivo = Path.GetFileName(archivo);

                // Verificar si el archivo debe ser procesado
                var tipoArchivo = confirmaciones.Keys.FirstOrDefault(tipo => archivo.Contains(tipo));
                if (tipoArchivo == null || !confirmaciones[tipoArchivo])
                    continue;

                // Reintentar si el archivo está en uso
                int intentos = 0;
                const int maxIntentos = 3;

                while (intentos < maxIntentos)
                {
                    try
                    {
                        // Leer la primera línea
                        string primeraLinea;
                        using (var reader = new StreamReader(archivo))
                        {
                            primeraLinea = reader.ReadLine();
                        }

                        if (primeraLinea != null)
                        {
                            // Pequeña pausa para asegurar que el archivo se liberó
                            Thread.Sleep(100);

                            // Escribir solo la primera línea
                            using (var writer = new StreamWriter(archivo, false))
                            {
                                writer.WriteLine(primeraLinea);
                            }

                            Console.WriteLine($"Contenido limpiado en: {nombreArchivo}");
                            archivosLimpiados++;
                        }
                        break; // Salir del bucle si fue exitoso
                    }
                    catch (IOException ex) when (ex.Message.Contains("being used by another process"))
                    {
                        intentos++;
                        if (intentos < maxIntentos)
                        {
                            Console.WriteLine($"Archivo {nombreArchivo} en uso. Reintentando en 1 segundo... (intento {intentos}/{maxIntentos})");
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Console.WriteLine($"Error: No se pudo acceder a {nombreArchivo} después de {maxIntentos} intentos. {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar {nombreArchivo}: {ex.Message}");
                        break;
                    }
                }
            }

            Console.WriteLine($"Se limpiaron {archivosLimpiados} archivos CSV de la ruta {dataPath}");
        }

        /// <summary>
        /// Solicita al usuario confirmación para realizar una nueva carga desde cero de un tipo de recurso específico.
        /// </summary>
        /// <param name="tipoRecurso">El tipo de recurso a recargar desde cero (ej: "películas", "personas", "géneros")</param>
        /// <returns>
        /// true: Se realizará una nueva carga desde cero (el usuario respondió Y, sí, Enter, o cualquier respuesta que no sea negativa)
        /// false: NO se realizará una nueva carga desde cero (el usuario respondió específicamente N, n, No, o no)
        /// Implicación: Retornar true significa que todos los datos existentes del CSV serán eliminados para permitir
        /// una carga completa de datos nuevos, conservando únicamente los encabezados del archivo.
        /// </returns>
        private static bool RealizarNuevaCargaDesdeCero(string tipoRecurso)
        {
            Console.Write($"¿Quieres realizar una carga desde cero de {tipoRecurso}? (Y/n) -> ");
            string respuesta = Console.ReadLine()?.Trim().ToLower() ?? "";
            return !new[] { "n", "no" }.Contains(respuesta);
        }

        public static void LeerOAuth()
        {
            Console.WriteLine($"\n-------------------------------------------");
            Console.WriteLine($"Leyendo información del archivo OAuth en ./Config/ConfigOAuth/OAuth_V3.config");

            try
            {
                // Ruta del archivo de configuración
                string configPath = Path.Combine("Config", "ConfigOAuth", "OAuth.config");

                // Verificar si el archivo existe
                if (!File.Exists(configPath))
                {
                    Console.WriteLine($"Error: El archivo {configPath} no existe.");
                    return;
                }

                // Cargar el documento XML
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configPath);

                // Buscar los nodos requeridos
                string communityShortName = GetNodeValue(xmlDoc, "communityShortName");
                string developerEmail = GetNodeValue(xmlDoc, "developerEmail");

                bool configOAuthKO = communityShortName.Contains(MESSAGE_CONNFIG_OAuthKO) || developerEmail.Contains(MESSAGE_CONNFIG_OAuthKO);

                if (configOAuthKO) {
                    Console.WriteLine($"Error en el contenido del archivo de configuración");
                    Console.ReadKey();
                    return;
                }

                // Mostrar los valores por consola
                Console.WriteLine($"   - Comunidad en la que se va a realizar la carga (communityShortName): {communityShortName}");
                Console.WriteLine($"   - developerEmail: {developerEmail}");                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer el archivo de configuración: {ex.Message}");
                Console.WriteLine($"-------------------------------------------");
                Console.ReadLine();
            }
        }

        private static string GetNodeValue(XmlDocument xmlDoc, string nodeName)
        {
            XmlNode node = xmlDoc.SelectSingleNode($"//*[local-name()='{nodeName}']");
            return node != null ? node.InnerText : MESSAGE_CONNFIG_OAuthKO;
        }

        internal static void MostrarCabeceraPrograma()
        {
            Console.WriteLine("¡Bienvenido!\n");
            Console.WriteLine("Este programa ha sido diseñado por GNOSS en el marco de GNOSS Akademia para una aplicación web de Cine basada en Grafos de Conocimiento (Grafos administrados por Ontologías)\n");
            Console.WriteLine("Antes de comenzar con la carga, recuerda que debes haber incluido el contenido de tu archivo OAuth y que los JSON que contienen la información de las películas estén dentro de la carpeta Data. También es recomendable que hayas incluido tus archivos OWL " +
                "dentro de la carpeta OWL.");
            Console.WriteLine("\nPor favor, si has ejecutado de manera autónoma este programa y has detectado algún problema, te rogamos que te pongas en contacto con nosotros (Email: sergiodedios@gnoss.com)");
        }
    }
}
