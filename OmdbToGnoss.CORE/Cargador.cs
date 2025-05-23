using System.Runtime.CompilerServices;
using AkgeneroOntology;
using AkpeliculaOntology;
using AkpersonaOntology;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using OmdbToGnoss.CORE;

namespace OmdToGnoss.CORE
{
    class Cargador
    {
        private ResourceApi mResourceApi;
        private string ontoPelicula;
        private string ontoPersona;
        private string ontoGenero;

        public Cargador(string ontoPelicula, string ontoPersona, string ontoGenero)
        {
            this.ontoPelicula = ontoPelicula;  
            this.ontoPersona = ontoPersona;
            this.ontoGenero = ontoGenero;
            this.mResourceApi = ApiManager.Instance.ResourceApi;
        }

        public void CargarGeneros(List<Genre> generos, Dictionary<string, string> guidDictionary)
        {
            mResourceApi.ChangeOntology(this.ontoGenero);
            List<string> guidsExistentes = new List<string>();
            try
            {
                SparqlObject resultados = mResourceApi.VirtuosoQuery("Select distinct ?o ?id  ", "Where { ?o <http://schema.org/name> ?id }", this.ontoGenero);
                foreach (var resultado in resultados.results.bindings)
                {
                    guidsExistentes.Add(resultado["o"].value);
                }
            }
            catch (Exception e)
            {
                mResourceApi.Log.Error($"Error al hacer la consulta a Virtuoso: {e.Message} -> {e.StackTrace}");
            }

            foreach (Genre genero in generos)
            {
                SecondaryResource secondaryResource = genero.ToGnossApiResource(mResourceApi, guidDictionary[genero.Schema_name]);

                if (!guidsExistentes.Contains("http://gnoss.com/items/" + guidDictionary[genero.Schema_name]))
                {
                    try
                    {
                        mResourceApi.LoadSecondaryResource(secondaryResource);
                    }
                    catch (Exception e)
                    {
                        mResourceApi.Log.Error(genero.Schema_name);
                    }
                }
                else
                {
                    try
                    {
                        mResourceApi.ModifySecondaryResource(secondaryResource);
                    }
                    catch (Exception e)
                    {
                        mResourceApi.Log.Error(genero.Schema_name);
                    }
                }
            }

        }

        public void CargarPersonas(List<Person> personas, Dictionary<string, string> guidDictionary)
        {
            mResourceApi.ChangeOntology(this.ontoPersona);
            List<string> guidsExistentes = new List<string>();
            try
            {
                SparqlObject resultados = mResourceApi.VirtuosoQuery("Select distinct ?o ?id  ", "Where { ?o <http://schema.org/name> ?id }", this.ontoPersona);
                foreach (var resultado in resultados.results.bindings)
                {
                    guidsExistentes.Add(resultado["o"].value);
                }
            }
            catch (Exception e)
            {
                mResourceApi.Log.Error($"Error al hacer la consulta a Virtuoso: {e.Message} -> {e.StackTrace}");
            }



            foreach (Person persona in personas)
            {
                ComplexOntologyResource complexResource = persona.ToGnossApiResource(mResourceApi, null , new Guid(guidDictionary[persona.Schema_name].Split('_')[1]), new Guid(guidDictionary[persona.Schema_name].Split('_')[2]));

                if (!guidsExistentes.Contains("http://gnoss.com/items/" + guidDictionary[persona.Schema_name]))
                {
                    try
                    {
                        mResourceApi.LoadComplexSemanticResource(complexResource);
                    }
                    catch (Exception e)
                    {
                        mResourceApi.Log.Error(persona.Schema_name);
                    }
                }
                else
                {
                    try
                    {
                        mResourceApi.ModifyComplexSemanticResourceList(new List<ComplexOntologyResource>() { complexResource }, false);
                    }
                    catch (Exception e)
                    {
                        mResourceApi.Log.Error(persona.Schema_name);
                    }
                }
            }

        }

        public void CargarPeliculas(List<Movie> peliculas, Dictionary<string, string> guidDictionary)
        {
            mResourceApi.ChangeOntology(this.ontoPelicula);   
            List<string> guidsExistentes = new List<string>();
            try
            {
                SparqlObject resultados = mResourceApi.VirtuosoQuery("Select distinct ?o ?id  ", "Where { ?o <http://schema.org/name> ?id  }", this.ontoPelicula);
                foreach (var resultado in resultados.results.bindings)
                {
                    guidsExistentes.Add(resultado["o"].value);

                }
            }
            catch (Exception e)
            {
                mResourceApi.Log.Error($"Error al hacer la consulta a Virtuoso: {e.Message} -> {e.StackTrace}");
            }



            foreach (Movie pelicula in peliculas)
            {
                ComplexOntologyResource complexResource = pelicula.ToGnossApiResource(mResourceApi, null, new Guid(guidDictionary[pelicula.Schema_name].Split('_')[1]), new Guid(guidDictionary[pelicula.Schema_name].Split('_')[2]));

                if (!guidsExistentes.Contains("http://gnoss.com/items/" + guidDictionary[pelicula.Schema_name]))
                {
                    try
                    {
                        mResourceApi.LoadComplexSemanticResource(complexResource);
                    }
                    catch (Exception e)
                    {
                        mResourceApi.Log.Error(pelicula.Schema_name);
                    }
                }
                else
                {
                    try
                    {
                        mResourceApi.ModifyComplexSemanticResourceList(new List<ComplexOntologyResource>() { complexResource }, false);
                    }
                    catch (Exception e)
                    {
                        mResourceApi.Log.Error(pelicula.Schema_name);
                    }
                }
            }

        }
    }
}
