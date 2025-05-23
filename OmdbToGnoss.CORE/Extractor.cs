using System.Text;
using CsvHelper;
using System.Globalization;
using Newtonsoft.Json.Linq;
using AkpeliculaOntology;
using AkpersonaOntology;
using AkgeneroOntology;
using CsvHelper.Configuration;

namespace OmdToGnoss.CORE
{
    /// <summary>
    /// Clase encargada de extraer los datos a migrar.
    /// Se obtienen y tratan los datos de las películas, personas y géneros
    /// a partir de archivos JSON que contiene la información.
    /// 
    /// Los datos obtenidos son almacenados en objetos
    /// proporcionados por el generador de clases.
    /// </summary>
    class Extractor
    {

        #region Miembros

        private static Encoding EncodingANSI = Encoding.GetEncoding("iso8859-1"); // Encoding para poder convertir a UTF8 algunos datos en código no legible

        private int NUM_MAX_PELICULAS; // Cantidad maxima de películas a cargar
        private List<Movie> movies; // Lista de los objetos movie
        private List<Person> persons; // Lista de los objetos person
        private List<Genre> genders; // Lista de los objetos genre
        private Dictionary<string, string> personasGuidDictionary; // Diccionario que contiene la relación entre el nombre de una persona y su GUID
        private Dictionary<string, string> peliculasGuidDictionary; // Diccionario que contiene la relación entre el nombre de una película y su GUID
        private Dictionary<string, string> generosGuidDictionary; // Diccionario que contiene la relación entre el nombre de un género y su GUID
        Dictionary<string, int> meses; // Diccionario con los meses en formato Int, para generar las fechas


        #endregion

        #region SubClases y propiedades

        // Declaración de meses
        private Dictionary<string, int> Meses
        {
            get
            {
                if (meses == null)
                {
                    meses = new Dictionary<string, int>();
                    meses.Add("Jan", 1);
                    meses.Add("Feb", 2);
                    meses.Add("Mar", 3);
                    meses.Add("Apr", 4);
                    meses.Add("May", 5);
                    meses.Add("Jun", 6);
                    meses.Add("Jul", 7);
                    meses.Add("Aug", 8);
                    meses.Add("Sep", 9);
                    meses.Add("Oct", 10);
                    meses.Add("Nov", 11);
                    meses.Add("Dec", 12);
                }
                return meses;
            }
        }


        // Retorno de propiedades
        public List<Movie> Movies { get { return movies; } }
        public List<Person> Persons { get { return persons; } }
        public List<Genre> Genders { get { return genders; } }
        public Dictionary<string, string> PersonasGuid { get { return personasGuidDictionary; } }
        public Dictionary<string, string> PeliculasGuid { get { return peliculasGuidDictionary; } }
        public Dictionary<string, string> GenerosGuid { get { return generosGuidDictionary; } }

        #endregion

        #region Constructor

        public Extractor(int numMaxPeliculas)
        {
            // Establece el número de ficheros a leer
            this.NUM_MAX_PELICULAS = numMaxPeliculas;

            this.movies = new List<Movie>();
            this.persons = new List<Person>();
            this.genders = new List<Genre>();

            this.personasGuidDictionary = new Dictionary<string, string>();
            this.peliculasGuidDictionary = new Dictionary<string, string>();
            this.generosGuidDictionary = new Dictionary<string, string>();

            CargarDatos();

        }

        #endregion

        #region Métodos

        /// <summary>
        /// Metodo que lee los ficheros json y los pasa a listas de objetos
        /// </summary>
        public void CargarDatos()
        {
            List<CSVEntry> listGenerosGuids = LeerCSVGeneros();
            List<CSVEntry> listPersonasGuids = LeerCSVPersonas();
            List<CSVEntry> listPeliculasGuids = LeerCSVPeliculas();

            //Debido a la gran cantidad de datos le cuesta menos tiempo comprobar si existen repeticiones buscando el nombre de una pelicula o persona en un HashSet
            HashSet<String> nombreGeneros = new HashSet<String>();
            HashSet<String> nombrePeliculas = new HashSet<String>();
            HashSet<String> nombrePersonas = new HashSet<String>();

            //Conseguimos los nombres de los ficheros json y los leemos uno a uno. 
            string[] ficheros = Directory.GetFiles(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")));

            foreach (string fichero in ficheros)
            {
                if (!fichero.Contains(".json")) //Si el fichero no es un json pasamod al siguiebte
                {
                    continue;
                }
                JObject objJSON = JObject.Parse(File.ReadAllText(fichero)); //Lo convertimos a un JObject con el que trabajaremos

                if (objJSON.Property("Type") != null && objJSON.Property("Type").Value.ToString().ToLower().Equals("movie")) //Solo nos interesa si es una pelicula
                {
                    Movie movie = LeerPelicula(objJSON, listPersonasGuids, listGenerosGuids, nombrePersonas, nombreGeneros);

                    if (!peliculasGuidDictionary.ContainsKey(movie.Schema_name))
                    {

                        CSVEntry gaux = new CSVEntry();
                        gaux.guid = "Movie_" + Guid.NewGuid() + "_" + Guid.NewGuid();
                        gaux.identificador = movie.Schema_name;
                        listPeliculasGuids.Add(gaux);
                        peliculasGuidDictionary[movie.Schema_name] = gaux.guid;

                    }

                    if (!nombrePeliculas.Contains(movie.Schema_name))
                    {
                        movies.Add(movie);
                        nombrePeliculas.Add(movie.Schema_name);
                    }
                }

                if (nombrePeliculas.Count() >= NUM_MAX_PELICULAS)
                {
                    break;
                }

            }

            EscribirCSV(listGenerosGuids, listPersonasGuids, listPeliculasGuids);
        }

        private Movie LeerPelicula(JObject objJSON, List<CSVEntry> listPersonasGuids, List<CSVEntry> listGenerosGuid, HashSet<String> nombrePersonas, HashSet<String> nombreGeneros)
        {
            Movie movie = new Movie();
            string[] partesString;

            //Las criticas adjuntas
            if (objJSON.Property("Ratings").HasValues && !objJSON.Property("Ratings").Value.ToString().Equals("N/A"))
            {
                movie.Schema_rating = new List<Rating>();
                foreach (JObject jrating in objJSON.Property("Ratings").Value)
                {
                    Rating elRating = new Rating();
                    elRating.Schema_ratingSource = jrating.Property("Source").Value.ToString();

                    //Como nuestra ontologia tiene un int para este campo debemos hacer la conversion dependiendo de en que formato este
                    if (jrating.Property("Value").Value.ToString().Contains("/100"))
                    {
                        partesString = jrating.Property("Value").Value.ToString().Split('/');
                        elRating.Schema_ratingValue = int.Parse(partesString[0]);

                    }
                    else if (jrating.Property("Value").Value.ToString().Contains("%"))
                    {
                        partesString = jrating.Property("Value").Value.ToString().Split('%');
                        elRating.Schema_ratingValue = int.Parse(partesString[0]);
                    }
                    else
                    {
                        partesString = jrating.Property("Value").Value.ToString().Split('/');
                        elRating.Schema_ratingValue = (int)(float.Parse(partesString[0]) * 10);
                    }
                    movie.Schema_rating.Add(elRating);
                }
            }


            //El titulo de la pelicula
            if (objJSON.Property("Title").HasValues && !objJSON.Property("Title").Value.ToString().Equals("N/A"))
            {
                movie.Schema_name = PasarAUtf8(objJSON.Property("Title").Value.ToString());
            }


            //El raitng de imdb
            if (objJSON.Property("imdbRating").HasValues && !objJSON.Property("imdbRating").Value.ToString().Equals("N/A"))
            {
                movie.Schema_aggregateRating = new List<string>();
                float nota = float.Parse(objJSON.Property("imdbRating").Value.ToString()) * 10;
                movie.Schema_aggregateRating.Add(nota + "");

            }


            //La url de la imagen
            if (objJSON.Property("Poster").HasValues && !objJSON.Property("Poster").Value.ToString().Equals("N/A"))
            {
                movie.Schema_image = objJSON.Property("Poster").Value.ToString();
            }

            //Los paises
            if (objJSON.Property("Country").HasValues && !objJSON.Property("Country").Value.ToString().Equals("N/A"))
            {
                movie.Schema_countryOfOrigin = new List<string>();
                partesString = objJSON.Property("Country").Value.ToString().Split(',');
                foreach (string parte in partesString)
                {
                    movie.Schema_countryOfOrigin.Add(PasarAUtf8(parte.Trim()));
                }

            }

            //La descripcion de la pelicula
            if (objJSON.Property("Plot").HasValues && !objJSON.Property("Plot").Value.ToString().Equals("N/A"))
            {
                movie.Schema_description = PasarAUtf8(objJSON.Property("Plot").Value.ToString());
            }

            //Los generos
            if (objJSON.Property("Genre").HasValues && !objJSON.Property("Genre").Value.ToString().Equals("N/A"))
            {

                movie.Schema_genre = new List<Genre>();
                movie.IdsSchema_genre = new List<string>();
                partesString = objJSON.Property("Genre").Value.ToString().Split(','); //Los diferentes géneros vienen separados por comas
                foreach (string parte in partesString)
                {
                    Genre genre = new Genre(Guid.NewGuid().ToString());
                    genre.Schema_name = PasarAUtf8(parte.Split('(')[0].Trim()); //Algun fichero contiene el rol entre parentesis despues del nombre. Solo nos interesa el nombre 
                    if (!generosGuidDictionary.ContainsKey(genre.Schema_name)) //Si el género no tiene GUID, se lo generamos
                    {
                        CSVEntry gaux = new CSVEntry();
                        gaux.guid = "Genre_" + Guid.NewGuid() + "_" + Guid.NewGuid();
                        gaux.identificador = genre.Schema_name;
                        listGenerosGuid.Add(gaux); //<-Lo añadimos a la lista para escribirlo posteriormente en el CSV
                        generosGuidDictionary[genre.Schema_name] = gaux.guid;
                    }

                    movie.Schema_genre.Add(genre);
                    movie.IdsSchema_genre.Add("http://gnoss.com/items/" + generosGuidDictionary[genre.Schema_name]);
                    if (!nombreGeneros.Contains(genre.Schema_name))
                    {
                        genders.Add(genre);
                    }
                    nombreGeneros.Add(genre.Schema_name);
                }

            }

            //Los premios obtenidos
            if (objJSON.Property("Awards").HasValues && !objJSON.Property("Awards").Value.ToString().Equals("N/A"))
            {
                movie.Schema_award = new List<string>();
                partesString = objJSON.Property("Awards").Value.ToString().Split(',');
                foreach (string parte in partesString)
                {
                    movie.Schema_award.Add(PasarAUtf8(parte.Trim()));
                }

            }

            //Clasificación por edades/publico
            if (objJSON.Property("Rated").HasValues && !objJSON.Property("Rated").Value.ToString().Equals("N/A"))
            {
                movie.Schema_contentRating = PasarAUtf8(objJSON.Property("Rated").Value.ToString());
            }

            //Los idiomas
            if (objJSON.Property("Language").HasValues && !objJSON.Property("Language").Value.ToString().Equals("N/A"))
            {
                movie.Schema_inLanguage = new List<string>();
                partesString = objJSON.Property("Language").Value.ToString().Split(',');
                foreach (string parte in partesString)
                {
                    movie.Schema_inLanguage.Add(PasarAUtf8(parte.Trim()));
                }

            }

            //Las productoras
            if (objJSON.Property("Production").HasValues && !objJSON.Property("Production").Value.ToString().Equals("N/A"))
            {
                movie.Schema_productionCompany = new List<string>();
                partesString = objJSON.Property("Production").Value.ToString().Split(',');
                foreach (string parte in partesString)
                {
                    movie.Schema_productionCompany.Add(PasarAUtf8(parte.Trim()));
                }

            }


            //La pagina web
            if (objJSON.Property("Website").HasValues && !objJSON.Property("Website").Value.ToString().Equals("N/A"))
            {
                movie.Schema_url = new List<string>();
                partesString = objJSON.Property("Website").Value.ToString().Split(',');
                foreach (string parte in partesString)
                {
                    movie.Schema_url.Add(parte.Trim());
                }

            }

            //Fecha de lanzamiento
            if (objJSON.Property("Released").HasValues && !objJSON.Property("Released").Value.ToString().Equals("N/A"))
            {
                partesString = objJSON.Property("Released").Value.ToString().Split(' ');
                movie.Schema_datePublished = new DateTime(int.Parse(partesString[2]), Meses[partesString[1]], int.Parse(partesString[0]));
            }

            //Año de grabacion
            if (objJSON.Property("Year").HasValues && !objJSON.Property("Year").Value.ToString().Equals("N/A"))
            {
                movie.Schema_recordedAt = new List<string>();
                partesString = objJSON.Property("Year").Value.ToString().Split(',');
                foreach (string parte in partesString)
                {
                    movie.Schema_recordedAt.Add(parte.Trim());
                }
            }

            //Duración
            if (objJSON.Property("Runtime").HasValues && !objJSON.Property("Runtime").Value.ToString().Equals("N/A"))
            {                
                partesString = objJSON.Property("Runtime").Value.ToString().Split(' ');
                movie.Schema_duration = int.Parse(partesString[0].Trim());
            }

            //Director/es de la pelicula
            if (objJSON.Property("Director").HasValues && !objJSON.Property("Director").Value.ToString().Equals("N/A"))
            {
                movie.Schema_director = new List<Person>();
                movie.IdsSchema_director = new List<string>();
                partesString = objJSON.Property("Director").Value.ToString().Split(','); //Los diferentes directores vienen separados por comas
                foreach (string parte in partesString)
                {
                    Person person = new Person();
                    person.Schema_name = PasarAUtf8(parte.Split('(')[0].Trim()); //Algun fichero contiene el rol entre parentesis despues del nombre. Solo nos interesa el nombre 
                    if (!personasGuidDictionary.ContainsKey(person.Schema_name)) //Si la persona no tiene GUID, se lo generamos
                    {
                        CSVEntry gaux = new CSVEntry();
                        gaux.guid = "Person_" + Guid.NewGuid() + "_" + Guid.NewGuid();
                        gaux.identificador = person.Schema_name;
                        listPersonasGuids.Add(gaux); //<-Lo añadimos a la lista para escribirlo posteriormente en el CSV
                        personasGuidDictionary[person.Schema_name] = gaux.guid;
                    }

                    movie.Schema_director.Add(person);
                    movie.IdsSchema_director.Add("http://gnoss.com/items/" + personasGuidDictionary[person.Schema_name]);
                    if (!nombrePersonas.Contains(person.Schema_name))
                    {
                        persons.Add(person);
                    }
                    nombrePersonas.Add(person.Schema_name);
                }

            }


            if (objJSON.Property("Writer").HasValues && !objJSON.Property("Writer").Value.ToString().Equals("N/A"))
            {
                movie.Schema_author = new List<Person>();
                movie.IdsSchema_author = new List<string>();
                partesString = objJSON.Property("Writer").Value.ToString().Split(',');
                foreach (string parte in partesString)
                {
                    Person person = new Person();
                    person.Schema_name = PasarAUtf8(parte.Split('(')[0].Trim());
                    if (!personasGuidDictionary.ContainsKey(person.Schema_name))
                    {
                        CSVEntry gaux = new CSVEntry();
                        gaux.guid = "Person_" + Guid.NewGuid() + "_" + Guid.NewGuid();
                        gaux.identificador = person.Schema_name;
                        listPersonasGuids.Add(gaux);
                        personasGuidDictionary[person.Schema_name] = gaux.guid;
                    }

                    movie.Schema_author.Add(person);
                    movie.IdsSchema_author.Add("http://gnoss.com/items/" + personasGuidDictionary[person.Schema_name]);
                    if (!nombrePersonas.Contains(person.Schema_name))
                    {
                        persons.Add(person);
                    }
                    nombrePersonas.Add(person.Schema_name);
                }

            }


            if (objJSON.Property("Actors").HasValues && !objJSON.Property("Actors").Value.ToString().Equals("N/A"))
            {
                movie.Schema_actor = new List<Person>();
                movie.IdsSchema_actor = new List<string>();
                partesString = objJSON.Property("Actors").Value.ToString().Split(',');
                foreach (string parte in partesString)
                {
                    Person person = new Person();
                    person.Schema_name = PasarAUtf8(parte.Split('(')[0].Trim());
                    if (!personasGuidDictionary.ContainsKey(person.Schema_name))
                    {
                        CSVEntry gaux = new CSVEntry();
                        gaux.guid = "Person_" + Guid.NewGuid() + "_" + Guid.NewGuid();
                        gaux.identificador = person.Schema_name;
                        listPersonasGuids.Add(gaux);
                        personasGuidDictionary[person.Schema_name] = gaux.guid;
                    }

                    movie.Schema_actor.Add(person);
                    movie.IdsSchema_actor.Add("http://gnoss.com/items/" + personasGuidDictionary[person.Schema_name]);
                    if (!nombrePersonas.Contains(person.Schema_name))
                    {
                        persons.Add(person);
                    }
                    nombrePersonas.Add(person.Schema_name);
                }

            }

            return movie;
        }

        private void EscribirCSV(List<CSVEntry> listGenerosGuids, List<CSVEntry> listPersonasGuids, List<CSVEntry> listPeliculasGuids)
        {
            if (!Directory.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"))))
            {
                Directory.CreateDirectory(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")));
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
            };
            using (var writer = new StreamWriter(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Generos.csv")), true))
            using (var csvWriter = new CsvWriter(writer, config))
            {                
                csvWriter.WriteRecords(listGenerosGuids);
            }
            if (!Directory.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"))))
            {
                Directory.CreateDirectory(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")));
            }
            using (var writer = new StreamWriter(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Personas.csv")), true))

            using (var csvWriter = new CsvWriter(writer, config))
            {                
                csvWriter.WriteRecords(listPersonasGuids);
            }

            if (!Directory.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"))))
            {
                Directory.CreateDirectory(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")));
            }
            using (var writer2 = new StreamWriter(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Peliculas.csv")), true))
            using (var csvWriter2 = new CsvWriter(writer2, config))
            {
                csvWriter2.WriteRecords(listPeliculasGuids);
            }
        }

        /// <summary>
        /// Función para cambiar la codificación de un string y que se lea correctamente
        /// </summary>
        /// <param name="pCadena"></param>
        /// <returns></returns>
        public static string PasarAUtf8(string pCadena)
        {
            return Encoding.UTF8.GetString(EncodingANSI.GetBytes(pCadena));
        }

        private List<CSVEntry> LeerCSVGeneros()
        {
            //Comprobamos si existe un csv con GUIDS de los géneros de una ejecución anterior. Si existe cargamos sus datos
            List<CSVEntry> listGenerosGuids = new List<CSVEntry>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
            };

            if (File.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Generos.csv")))) //Ya existe el fichero de Guids de personas
            {
                using (var reader = new StreamReader(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Generos.csv"))))
                using (var csv = new CsvHelper.CsvReader(reader, config))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        generosGuidDictionary[csv.GetField("identificador")] = csv.GetField("guid");
                    }
                }
            }

            return listGenerosGuids;
        }

        private List<CSVEntry> LeerCSVPersonas()
        {
            //Comprobamos si existe un csv con GUIDS de personas de una ejecución anterior. Si existe cargamos sus datos
            List<CSVEntry> listPersonasGuids = new List<CSVEntry>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
            };

            if (File.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Personas.csv")))) //Ya existe el fichero de Guids de personas
            {
                using (var reader = new StreamReader(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Personas.csv"))))
                using (var csv = new CsvHelper.CsvReader(reader, config))
                {                  
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        personasGuidDictionary[csv.GetField("identificador")] = csv.GetField("guid");
                    }
                }
            }

            return listPersonasGuids;
        }

        private List<CSVEntry> LeerCSVPeliculas()
        {
            //Comprobamos si existe un csv con GUIDS de peliculas de una ejecución anterior. Si existe cargamos sus datos
            List<CSVEntry> listPeliculasGuids = new List<CSVEntry>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
            };

            if (File.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Peliculas.csv")))) //Ya existe el fichero de Guids de peliculas
            {
                using (var reader = new StreamReader(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Peliculas.csv"))))
                using (var csv = new CsvHelper.CsvReader(reader, config))
                {                   
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        peliculasGuidDictionary[csv.GetField("identificador")] = csv.GetField("guid");
                    }
                }
            }

            return listPeliculasGuids;
        }

        #endregion
    }

    /// <summary>
    /// Clase auxiliar para facilitar la escritura de GUIDs en un CSV
    /// </summary>
    public class CSVEntry
    {
        public string guid { get; set; }
        public string identificador { get; set; }
    }
}
