using System;
using System.Collections.Generic;
using System.Linq;
using Gnoss.ApiWrapper;
using OmdbToGnoss.CORE;

namespace OmdToGnoss.CORE
{
    class Program
    {
        private const int NUMERO_PELICULAS_POR_DEFECTO = 1;
        private const int MAXIMO_PELICULAS = 600;
        private const int MAXIMO_INTENTOS_PATH = 3;

        private static readonly ResourceApi mResourceApi = ApiManager.Instance.ResourceApi;
        private static readonly CommunityApi mCommunityApi = ApiManager.Instance.CommunityApi;

        static void Main(string[] args)
        {
            try
            {
                InicializarPrograma();

                var ontologias = ObtenerOntologias();
                if (ontologias == null) return;

                int numPeliculas = ObtenerNumeroPeliculas();

                if (!ConfirmarCarga(ontologias, numPeliculas)) return;

                EjecutarCarga(ontologias, numPeliculas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError durante la ejecución: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPresione cualquier tecla para salir...");
                Console.ReadKey();
            }
        }

        private static void InicializarPrograma()
        {
            Utils.MostrarCabeceraPrograma();
            Utils.LeerOAuth();
            Utils.LimpiarCsvCacheInterna();
        }

        private static OntologiasConfig ObtenerOntologias()
        {
            var opciones = PrepararOpcionesMenu();
            MostrarMenu(opciones);

            string opcion = Console.ReadLine();

            return opcion switch
            {
                "1" => ProcesarOpcionComunidad(opciones.Comunidad),
                "2" => ProcesarOpcionLocal(opciones.Local),
                "3" => ProcesarOpcionManual(),
                _ => null
            };
        }

        private static OpcionesMenu PrepararOpcionesMenu()
        {
            return new OpcionesMenu
            {
                Comunidad = ObtenerOntologiasComunidad(),
                Local = ObtenerOntologiasLocal()
            };
        }

        private static OntologiasConfig ObtenerOntologiasComunidad()
        {
            var ontologiasComunidad = Utils.ObtenerOntologiasComunidad();

            var config = new OntologiasConfig
            {
                Pelicula = ontologiasComunidad.FirstOrDefault(s => s.ToLower().Contains("pelicula")),
                Persona = ontologiasComunidad.FirstOrDefault(s =>
                    s.ToLower().Contains("persona") && s.ToLower() != "persona")
            };

            if (!string.IsNullOrEmpty(config.Pelicula))
            {
                config.Genero = config.Pelicula.Replace("pelicula", "") + "genero";
            }

            config.EsValida = !string.IsNullOrEmpty(config.Pelicula) &&
                             !string.IsNullOrEmpty(config.Persona) &&
                             !string.IsNullOrEmpty(config.Genero);

            return config;
        }

        private static OntologiasConfig ObtenerOntologiasLocal(string path = null)
        {
            var ontologiasLocal = Utils.ObtenerOntologiasEquipo(path);

            if (ontologiasLocal == null || ontologiasLocal.Count != 3)
                return new OntologiasConfig { EsValida = false };

            var config = new OntologiasConfig
            {
                Pelicula = ontologiasLocal.FirstOrDefault(s => s.ToLower().Contains("pelicula")),
                Persona = ontologiasLocal.FirstOrDefault(s =>
                    s.ToLower().Contains("persona") && s.ToLower() != "persona")
            };

            if (!string.IsNullOrEmpty(config.Pelicula))
            {
                config.Genero = config.Pelicula.Replace("pelicula", "") + "genero";
            }

            config.EsValida = !string.IsNullOrEmpty(config.Pelicula) &&
                             !string.IsNullOrEmpty(config.Persona) &&
                             !string.IsNullOrEmpty(config.Genero);

            return config;
        }

        private static void MostrarMenu(OpcionesMenu opciones)
        {
            Console.WriteLine($"\n-------------------------------------------");
            Console.WriteLine($"Nombres de los OWLs / Ontologías / OCs \n");

            MostrarOpcionComunidad(opciones.Comunidad);
            MostrarOpcionLocal(opciones.Local);
            Console.WriteLine($"3. Introducir los nombres de las ontologías de manera manual");

            MostrarNotasAdicionales(opciones);

            Console.WriteLine($"Por favor, selecciona alguna de las opciones propuestas");
        }

        private static void MostrarOpcionComunidad(OntologiasConfig config)
        {
            if (config.EsValida)
            {
                Console.WriteLine($"1. Obtenerlos de la comunidad: {config.Pelicula}, {config.Persona}, {config.Genero}");
            }
            else
            {
                Console.WriteLine($"1. Obtenerlos de la comunidad [NO DISPONIBLE *]");
            }
        }

        private static void MostrarOpcionLocal(OntologiasConfig config)
        {
            if (config.EsValida)
            {
                Console.WriteLine($"2. Obtenerlos desde archivos locales (Directorio: OWLs): {config.Pelicula}, {config.Persona}, {config.Genero}");
            }
            else
            {
                Console.WriteLine($"2. Obtenerlos desde archivos locales (será necesario especificar la ruta) [**]");
            }
        }

        private static void MostrarNotasAdicionales(OpcionesMenu opciones)
        {
            Console.WriteLine();

            if (!opciones.Comunidad.EsValida)
            {
                Console.WriteLine($"[*] Para poder utilizar esta opción necesitas haber creado un recurso de cada tipo; es decir, un género, una persona y una película");
            }

            if (!opciones.Local.EsValida)
            {
                Console.WriteLine($"[**] Para utilizar la lectura en la ruta por defecto (./OWLs), tus ontologías deben estar en esta carpeta");
            }

            if (opciones.Comunidad.EsValida && opciones.Local.EsValida)
            {
                Console.WriteLine();
            }
        }

        private static OntologiasConfig ProcesarOpcionComunidad(OntologiasConfig config)
        {
            if (!config.EsValida)
            {
                Console.WriteLine($"\nExiste un problema con la opción de utilizar los nombres de las ontologías leídos desde la comunidad. " +
                                $"Para poder utilizar esta opción necesitas haber creado un recurso de cada tipo; es decir, un género, una persona y una película");
                return null;
            }

            Console.WriteLine($"\nSe cargarán las ontologías de la comunidad: {config.Pelicula}, {config.Persona}, {config.Genero}");
            return config;
        }

        private static OntologiasConfig ProcesarOpcionLocal(OntologiasConfig configDefecto)
        {
            if (configDefecto.EsValida)
            {
                Console.WriteLine($"\nSe cargarán las ontologías desde archivos locales: {configDefecto.Pelicula}, {configDefecto.Persona}, {configDefecto.Genero}");
                return configDefecto;
            }

            string path = ObtenerPathUsuario();
            if (string.IsNullOrEmpty(path)) return null;

            var config = ObtenerOntologiasLocal(path);
            if (!config.EsValida)
            {
                Console.WriteLine("No se encontraron ontologías válidas en la ruta especificada");
                return null;
            }

            return config;
        }

        private static string ObtenerPathUsuario()
        {
            Console.WriteLine($"\nIntroduce el path de la carpeta donde se encuentran las ontologías (OWLs)");

            for (int intento = 0; intento < MAXIMO_INTENTOS_PATH; intento++)
            {
                string path = Console.ReadLine();
                if (Directory.Exists(path))
                {
                    return path;
                }

                Console.WriteLine($"La ruta (path) especificada no es válida");
                if (intento == MAXIMO_INTENTOS_PATH - 1)
                {
                    Console.WriteLine("Se alcanzó el número máximo de intentos");
                }
            }

            return null;
        }

        private static OntologiasConfig ProcesarOpcionManual()
        {
            Console.WriteLine($"\nSe cargarán las ontologías de manera manual");

            return new OntologiasConfig
            {
                Pelicula = LeerOntologiaObligatoria("películas"),
                Persona = LeerOntologiaObligatoria("personas"),
                Genero = LeerOntologiaObligatoria("géneros"),
                EsValida = true
            };
        }

        private static string LeerOntologiaObligatoria(string tipo)
        {
            string ontologia;
            do
            {
                Console.WriteLine($"Introduce el nombre de la ontología de {tipo}");
                ontologia = Console.ReadLine();
            } while (string.IsNullOrEmpty(ontologia));

            return ontologia;
        }

        private static int ObtenerNumeroPeliculas()
        {
            Console.WriteLine($"\nIntroduce el número de películas que quieres cargar (Por defecto: {NUMERO_PELICULAS_POR_DEFECTO} / Máximo: {MAXIMO_PELICULAS})");
            string input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                return NUMERO_PELICULAS_POR_DEFECTO;
            }

            if (int.TryParse(input, out int num))
            {
                return Math.Min(num, MAXIMO_PELICULAS);
            }

            Console.WriteLine($"Valor no válido. Se usará el valor por defecto: {NUMERO_PELICULAS_POR_DEFECTO}");
            return NUMERO_PELICULAS_POR_DEFECTO;
        }

        private static bool ConfirmarCarga(OntologiasConfig ontologias, int numPeliculas)
        {
            Console.WriteLine($"\nSe cargarán: {numPeliculas} películas");
            Console.WriteLine($"Ontología de películas: {ontologias.Pelicula}");
            Console.WriteLine($"Ontología de personas: {ontologias.Persona}");
            Console.WriteLine($"Ontología de géneros: {ontologias.Genero}");
            Console.WriteLine($"\n¿De acuerdo? (s/N)");

            string respuesta = Console.ReadLine();
            bool confirmado = respuesta?.ToLower() == "s";

            if (!confirmado)
            {
                Console.WriteLine("Carga cancelada");
            }

            return confirmado;
        }

        private static void EjecutarCarga(OntologiasConfig ontologias, int numPeliculas)
        {
            Console.WriteLine("\nIniciando carga de datos...");

            var extractor = new Extractor(numPeliculas);
            var cargador = new Cargador(ontologias.Pelicula, ontologias.Persona, ontologias.Genero);

            cargador.CargarGeneros(extractor.Genders, extractor.GenerosGuid);
            cargador.CargarPersonas(extractor.Persons, extractor.PersonasGuid);
            cargador.CargarPeliculas(extractor.Movies, extractor.PeliculasGuid);

            Console.WriteLine($"\nCarga de películas, personas y géneros finalizada");
        }

        // Clases auxiliares
        private class OntologiasConfig
        {
            public string Pelicula { get; set; }
            public string Persona { get; set; }
            public string Genero { get; set; }
            public bool EsValida { get; set; }
        }

        private class OpcionesMenu
        {
            public OntologiasConfig Comunidad { get; set; }
            public OntologiasConfig Local { get; set; }
        }
    }
}