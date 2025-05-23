using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.Helpers;
using GnossBase;
using Es.Riam.Gnoss.Web.MVC.Models;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using Gnoss.ApiWrapper.Exceptions;
using System.Diagnostics.CodeAnalysis;
using Genre = AkgeneroOntology.Genre;
using Person = AkpersonaOntology.Person;

namespace AkpeliculaOntology
{
	[ExcludeFromCodeCoverage]
	public class Movie : GnossOCBase
	{
		public Movie() : base() { } 

		public Movie(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			GNOSSID = pSemCmsModel.RootEntities[0].Entity.Uri;
			Schema_genre = new List<Genre>();
			SemanticPropertyModel propSchema_genre = pSemCmsModel.GetPropertyByPath("http://schema.org/genre");
			if(propSchema_genre != null && propSchema_genre.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_genre.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Genre schema_genre = new Genre(propValue.RelatedEntity,idiomaUsuario);
						Schema_genre.Add(schema_genre);
					}
				}
			}
			Schema_author = new List<Person>();
			SemanticPropertyModel propSchema_author = pSemCmsModel.GetPropertyByPath("http://schema.org/author");
			if(propSchema_author != null && propSchema_author.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_author.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Person schema_author = new Person(propValue.RelatedEntity,idiomaUsuario);
						Schema_author.Add(schema_author);
					}
				}
			}
			Schema_rating = new List<Rating>();
			SemanticPropertyModel propSchema_rating = pSemCmsModel.GetPropertyByPath("http://schema.org/rating");
			if(propSchema_rating != null && propSchema_rating.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_rating.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Rating schema_rating = new Rating(propValue.RelatedEntity,idiomaUsuario);
						Schema_rating.Add(schema_rating);
					}
				}
			}
			Schema_director = new List<Person>();
			SemanticPropertyModel propSchema_director = pSemCmsModel.GetPropertyByPath("http://schema.org/director");
			if(propSchema_director != null && propSchema_director.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_director.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Person schema_director = new Person(propValue.RelatedEntity,idiomaUsuario);
						Schema_director.Add(schema_director);
					}
				}
			}
			Schema_actor = new List<Person>();
			SemanticPropertyModel propSchema_actor = pSemCmsModel.GetPropertyByPath("http://schema.org/actor");
			if(propSchema_actor != null && propSchema_actor.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_actor.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Person schema_actor = new Person(propValue.RelatedEntity,idiomaUsuario);
						Schema_actor.Add(schema_actor);
					}
				}
			}
			SemanticPropertyModel propSchema_url = pSemCmsModel.GetPropertyByPath("http://schema.org/url");
			this.Schema_url = new List<string>();
			if (propSchema_url != null && propSchema_url.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_url.PropertyValues)
				{
					this.Schema_url.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_aggregateRating = pSemCmsModel.GetPropertyByPath("http://schema.org/aggregateRating");
			this.Schema_aggregateRating = new List<string>();
			if (propSchema_aggregateRating != null && propSchema_aggregateRating.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_aggregateRating.PropertyValues)
				{
					this.Schema_aggregateRating.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_productionCompany = pSemCmsModel.GetPropertyByPath("http://schema.org/productionCompany");
			this.Schema_productionCompany = new List<string>();
			if (propSchema_productionCompany != null && propSchema_productionCompany.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_productionCompany.PropertyValues)
				{
					this.Schema_productionCompany.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_recordedAt = pSemCmsModel.GetPropertyByPath("http://schema.org/recordedAt");
			this.Schema_recordedAt = new List<string>();
			if (propSchema_recordedAt != null && propSchema_recordedAt.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_recordedAt.PropertyValues)
				{
					this.Schema_recordedAt.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_countryOfOrigin = pSemCmsModel.GetPropertyByPath("http://schema.org/countryOfOrigin");
			this.Schema_countryOfOrigin = new List<string>();
			if (propSchema_countryOfOrigin != null && propSchema_countryOfOrigin.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_countryOfOrigin.PropertyValues)
				{
					this.Schema_countryOfOrigin.Add(propValue.Value);
				}
			}
			this.Schema_duration = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/duration"));
			SemanticPropertyModel propSchema_inLanguage = pSemCmsModel.GetPropertyByPath("http://schema.org/inLanguage");
			this.Schema_inLanguage = new List<string>();
			if (propSchema_inLanguage != null && propSchema_inLanguage.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_inLanguage.PropertyValues)
				{
					this.Schema_inLanguage.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_award = pSemCmsModel.GetPropertyByPath("http://schema.org/award");
			this.Schema_award = new List<string>();
			if (propSchema_award != null && propSchema_award.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_award.PropertyValues)
				{
					this.Schema_award.Add(propValue.Value);
				}
			}
			this.Schema_description = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/description"));
			this.Schema_image = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/image"));
			this.Schema_name = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/name"));
var item0 = GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/datePublished"));
if(item0.HasValue){
			this.Schema_datePublished = item0.Value;
}
			this.Schema_contentRating = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/contentRating"));
		}

		public Movie(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			mGNOSSID = pSemCmsModel.Entity.Uri;
			mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			Schema_genre = new List<Genre>();
			SemanticPropertyModel propSchema_genre = pSemCmsModel.GetPropertyByPath("http://schema.org/genre");
			if(propSchema_genre != null && propSchema_genre.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_genre.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Genre schema_genre = new Genre(propValue.RelatedEntity,idiomaUsuario);
						Schema_genre.Add(schema_genre);
					}
				}
			}
			Schema_author = new List<Person>();
			SemanticPropertyModel propSchema_author = pSemCmsModel.GetPropertyByPath("http://schema.org/author");
			if(propSchema_author != null && propSchema_author.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_author.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Person schema_author = new Person(propValue.RelatedEntity,idiomaUsuario);
						Schema_author.Add(schema_author);
					}
				}
			}
			Schema_rating = new List<Rating>();
			SemanticPropertyModel propSchema_rating = pSemCmsModel.GetPropertyByPath("http://schema.org/rating");
			if(propSchema_rating != null && propSchema_rating.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_rating.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Rating schema_rating = new Rating(propValue.RelatedEntity,idiomaUsuario);
						Schema_rating.Add(schema_rating);
					}
				}
			}
			Schema_director = new List<Person>();
			SemanticPropertyModel propSchema_director = pSemCmsModel.GetPropertyByPath("http://schema.org/director");
			if(propSchema_director != null && propSchema_director.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_director.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Person schema_director = new Person(propValue.RelatedEntity,idiomaUsuario);
						Schema_director.Add(schema_director);
					}
				}
			}
			Schema_actor = new List<Person>();
			SemanticPropertyModel propSchema_actor = pSemCmsModel.GetPropertyByPath("http://schema.org/actor");
			if(propSchema_actor != null && propSchema_actor.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_actor.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Person schema_actor = new Person(propValue.RelatedEntity,idiomaUsuario);
						Schema_actor.Add(schema_actor);
					}
				}
			}
			SemanticPropertyModel propSchema_url = pSemCmsModel.GetPropertyByPath("http://schema.org/url");
			this.Schema_url = new List<string>();
			if (propSchema_url != null && propSchema_url.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_url.PropertyValues)
				{
					this.Schema_url.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_aggregateRating = pSemCmsModel.GetPropertyByPath("http://schema.org/aggregateRating");
			this.Schema_aggregateRating = new List<string>();
			if (propSchema_aggregateRating != null && propSchema_aggregateRating.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_aggregateRating.PropertyValues)
				{
					this.Schema_aggregateRating.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_productionCompany = pSemCmsModel.GetPropertyByPath("http://schema.org/productionCompany");
			this.Schema_productionCompany = new List<string>();
			if (propSchema_productionCompany != null && propSchema_productionCompany.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_productionCompany.PropertyValues)
				{
					this.Schema_productionCompany.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_recordedAt = pSemCmsModel.GetPropertyByPath("http://schema.org/recordedAt");
			this.Schema_recordedAt = new List<string>();
			if (propSchema_recordedAt != null && propSchema_recordedAt.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_recordedAt.PropertyValues)
				{
					this.Schema_recordedAt.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_countryOfOrigin = pSemCmsModel.GetPropertyByPath("http://schema.org/countryOfOrigin");
			this.Schema_countryOfOrigin = new List<string>();
			if (propSchema_countryOfOrigin != null && propSchema_countryOfOrigin.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_countryOfOrigin.PropertyValues)
				{
					this.Schema_countryOfOrigin.Add(propValue.Value);
				}
			}
			this.Schema_duration = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/duration"));
			SemanticPropertyModel propSchema_inLanguage = pSemCmsModel.GetPropertyByPath("http://schema.org/inLanguage");
			this.Schema_inLanguage = new List<string>();
			if (propSchema_inLanguage != null && propSchema_inLanguage.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_inLanguage.PropertyValues)
				{
					this.Schema_inLanguage.Add(propValue.Value);
				}
			}
			SemanticPropertyModel propSchema_award = pSemCmsModel.GetPropertyByPath("http://schema.org/award");
			this.Schema_award = new List<string>();
			if (propSchema_award != null && propSchema_award.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSchema_award.PropertyValues)
				{
					this.Schema_award.Add(propValue.Value);
				}
			}
			this.Schema_description = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/description"));
			this.Schema_image = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/image"));
			this.Schema_name = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/name"));
var item1 = GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/datePublished"));
if(item1.HasValue){
			this.Schema_datePublished = item1.Value;
}
			this.Schema_contentRating = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/contentRating"));
		}

		public virtual string RdfType { get { return "http://schema.org/Movie"; } }
		public virtual string RdfsLabel { get { return "http://schema.org/Movie"; } }
		[LABEL(LanguageEnum.es,"Género")]
		[RDFProperty("http://schema.org/genre")]
		public  List<Genre> Schema_genre { get; set;}
		public List<string> IdsSchema_genre { get; set;}

		[LABEL(LanguageEnum.es,"Autor / Autora")]
		[RDFProperty("http://schema.org/author")]
		public  List<Person> Schema_author { get; set;}
		public List<string> IdsSchema_author { get; set;}

		[LABEL(LanguageEnum.es,"Calificación")]
		[RDFProperty("http://schema.org/rating")]
		public  List<Rating> Schema_rating { get; set;}

		[LABEL(LanguageEnum.es,"Director / Directora")]
		[RDFProperty("http://schema.org/director")]
		public  List<Person> Schema_director { get; set;}
		public List<string> IdsSchema_director { get; set;}

		[LABEL(LanguageEnum.es,"Actor / Actriz")]
		[RDFProperty("http://schema.org/actor")]
		public  List<Person> Schema_actor { get; set;}
		public List<string> IdsSchema_actor { get; set;}

		[LABEL(LanguageEnum.es,"Url")]
		[RDFProperty("http://schema.org/url")]
		public  List<string> Schema_url { get; set;}

		[LABEL(LanguageEnum.es,"Calificación agregada")]
		[RDFProperty("http://schema.org/aggregateRating")]
		public  List<string> Schema_aggregateRating { get; set;}

		[LABEL(LanguageEnum.es,"Productora")]
		[RDFProperty("http://schema.org/productionCompany")]
		public  List<string> Schema_productionCompany { get; set;}

		[LABEL(LanguageEnum.es,"Gabrado en")]
		[RDFProperty("http://schema.org/recordedAt")]
		public  List<string> Schema_recordedAt { get; set;}

		[LABEL(LanguageEnum.es,"País de origen")]
		[RDFProperty("http://schema.org/countryOfOrigin")]
		public  List<string> Schema_countryOfOrigin { get; set;}

		[LABEL(LanguageEnum.es,"Duración")]
		[RDFProperty("http://schema.org/duration")]
		public  int? Schema_duration { get; set;}

		[LABEL(LanguageEnum.es,"En idioma")]
		[RDFProperty("http://schema.org/inLanguage")]
		public  List<string> Schema_inLanguage { get; set;}

		[LABEL(LanguageEnum.es,"Premios")]
		[RDFProperty("http://schema.org/award")]
		public  List<string> Schema_award { get; set;}

		[LABEL(LanguageEnum.es,"Descripción")]
		[RDFProperty("http://schema.org/description")]
		public  string Schema_description { get; set;}

		[LABEL(LanguageEnum.es,"Imagen")]
		[RDFProperty("http://schema.org/image")]
		public  string Schema_image { get; set;}

		[LABEL(LanguageEnum.es,"Nombre")]
		[RDFProperty("http://schema.org/name")]
		public  string Schema_name { get; set;}

		[LABEL(LanguageEnum.es,"Fecha de publicación")]
		[RDFProperty("http://schema.org/datePublished")]
		public  DateTime Schema_datePublished { get; set;}

		[LABEL(LanguageEnum.es,"Clasificación del contenido")]
		[RDFProperty("http://schema.org/contentRating")]
		public  string Schema_contentRating { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new ListStringOntologyProperty("schema:genre", this.IdsSchema_genre));
			propList.Add(new ListStringOntologyProperty("schema:author", this.IdsSchema_author));
			propList.Add(new ListStringOntologyProperty("schema:director", this.IdsSchema_director));
			propList.Add(new ListStringOntologyProperty("schema:actor", this.IdsSchema_actor));
			propList.Add(new ListStringOntologyProperty("schema:url", this.Schema_url));
			propList.Add(new ListStringOntologyProperty("schema:aggregateRating", this.Schema_aggregateRating));
			propList.Add(new ListStringOntologyProperty("schema:productionCompany", this.Schema_productionCompany));
			propList.Add(new ListStringOntologyProperty("schema:recordedAt", this.Schema_recordedAt));
			propList.Add(new ListStringOntologyProperty("schema:countryOfOrigin", this.Schema_countryOfOrigin));
			propList.Add(new StringOntologyProperty("schema:duration", this.Schema_duration.ToString()));
			propList.Add(new ListStringOntologyProperty("schema:inLanguage", this.Schema_inLanguage));
			propList.Add(new ListStringOntologyProperty("schema:award", this.Schema_award));
			propList.Add(new StringOntologyProperty("schema:description", this.Schema_description));
			propList.Add(new StringOntologyProperty("schema:image", this.Schema_image));
			propList.Add(new StringOntologyProperty("schema:name", this.Schema_name));
			propList.Add(new DateOntologyProperty("schema:datePublished", this.Schema_datePublished));
			propList.Add(new StringOntologyProperty("schema:contentRating", this.Schema_contentRating));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			if(Schema_rating!=null){
				foreach(Rating prop in Schema_rating){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityRating = new OntologyEntity("http://schema.org/Rating", "http://schema.org/Rating", "schema:rating", prop.propList, prop.entList);
				entList.Add(entityRating);
				prop.Entity = entityRating;
				}
			}
		} 
		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI)
		{
			return ToGnossApiResource(resourceAPI, new List<string>());
		}

		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias)
		{
			return ToGnossApiResource(resourceAPI, listaDeCategorias, Guid.Empty, Guid.Empty);
		}

		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI, List<Guid> listaDeCategorias)
		{
			return ToGnossApiResource(resourceAPI, null, Guid.Empty, Guid.Empty, listaDeCategorias);
		}

		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias, Guid idrecurso, Guid idarticulo, List<Guid> listaIdDeCategorias = null)
		{
			ComplexOntologyResource resource = new ComplexOntologyResource();
			Ontology ontology = null;
			GetEntities();
			GetProperties();
			if(idrecurso.Equals(Guid.Empty) && idarticulo.Equals(Guid.Empty))
			{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, RdfType, RdfsLabel, prefList, propList, entList);
			}
			else{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, RdfType, RdfsLabel, prefList, propList, entList,idrecurso,idarticulo);
			}
			resource.Id = GNOSSID;
			resource.Ontology = ontology;
			resource.TextCategories = listaDeCategorias;
			resource.CategoriesIds = listaIdDeCategorias;
			AddResourceTitle(resource);
			AddResourceDescription(resource);
			AddImages(resource);
			AddFiles(resource);
			return resource;
		}

		public override List<string> ToOntologyGnossTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://schema.org/Movie>", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://schema.org/Movie\"", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}>", list, " . ");
			if(this.Schema_rating != null)
			{
			foreach(var item0 in this.Schema_rating)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://schema.org/Rating>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://schema.org/Rating\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/rating", $"<{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Schema_ratingSource != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}",  "http://schema.org/ratingSource", $"\"{GenerarTextoSinSaltoDeLinea(item0.Schema_ratingSource)}\"", list, " . ");
				}
				if(item0.Schema_ratingValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}",  "http://schema.org/ratingValue", $"{item0.Schema_ratingValue.ToString()}", list, " . ");
				}
			}
			}
				if(this.IdsSchema_genre != null)
				{
					foreach(var item2 in this.IdsSchema_genre)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/genre", $"<{item2}>", list, " . ");
					}
				}
				if(this.IdsSchema_author != null)
				{
					foreach(var item2 in this.IdsSchema_author)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/author", $"<{item2}>", list, " . ");
					}
				}
				if(this.IdsSchema_director != null)
				{
					foreach(var item2 in this.IdsSchema_director)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/director", $"<{item2}>", list, " . ");
					}
				}
				if(this.IdsSchema_actor != null)
				{
					foreach(var item2 in this.IdsSchema_actor)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/actor", $"<{item2}>", list, " . ");
					}
				}
				if(this.Schema_url != null)
				{
					foreach(var item2 in this.Schema_url)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/url", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_aggregateRating != null)
				{
					foreach(var item2 in this.Schema_aggregateRating)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/aggregateRating", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_productionCompany != null)
				{
					foreach(var item2 in this.Schema_productionCompany)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/productionCompany", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_recordedAt != null)
				{
					foreach(var item2 in this.Schema_recordedAt)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/recordedAt", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_countryOfOrigin != null)
				{
					foreach(var item2 in this.Schema_countryOfOrigin)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/countryOfOrigin", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_duration != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/duration", $"{this.Schema_duration.Value.ToString()}", list, " . ");
				}
				if(this.Schema_inLanguage != null)
				{
					foreach(var item2 in this.Schema_inLanguage)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/inLanguage", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_award != null)
				{
					foreach(var item2 in this.Schema_award)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/award", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_description != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/description", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_description)}\"", list, " . ");
				}
				if(this.Schema_image != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/image", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_image)}\"", list, " . ");
				}
				if(this.Schema_name != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/name", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_name)}\"", list, " . ");
				}
				if(this.Schema_datePublished != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/datePublished", $"\"{this.Schema_datePublished.ToString("yyyyMMddHHmmss")}\"", list, " . ");
				}
				if(this.Schema_contentRating != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/contentRating", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_contentRating)}\"", list, " . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			List<string> listaSearch = new List<string>();
			AgregarTags(list);
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"\"akpelicula\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/type", $"\"http://schema.org/Movie\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasfechapublicacion", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hastipodoc", "\"5\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasfechamodificacion", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasnumeroVisitas", "0", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasprivacidadCom", "\"publico\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://xmlns.com/foaf/0.1/firstName", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_name)}\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasnombrecompleto", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_name)}\"", list, " . ");
			string search = string.Empty;
			if(this.Schema_rating != null)
			{
			foreach(var item0 in this.Schema_rating)
			{
				AgregarTripleALista($"http://gnossAuxiliar/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasEntidadAuxiliar", $"<{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/rating", $"<{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Schema_ratingSource != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}",  "http://schema.org/ratingSource", $"\"{GenerarTextoSinSaltoDeLinea(item0.Schema_ratingSource)}\"", list, " . ");
				}
				if(item0.Schema_ratingValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}",  "http://schema.org/ratingValue", $"{item0.Schema_ratingValue.ToString()}", list, " . ");
				}
			}
			}
				if(this.IdsSchema_genre != null)
				{
					foreach(var item2 in this.IdsSchema_genre)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/genre", $"<{itemRegex}>", list, " . ");
					}
				}
				if(this.IdsSchema_author != null)
				{
					foreach(var item2 in this.IdsSchema_author)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/author", $"<{itemRegex}>", list, " . ");
					}
				}
				if(this.IdsSchema_director != null)
				{
					foreach(var item2 in this.IdsSchema_director)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/director", $"<{itemRegex}>", list, " . ");
					}
				}
				if(this.IdsSchema_actor != null)
				{
					foreach(var item2 in this.IdsSchema_actor)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/actor", $"<{itemRegex}>", list, " . ");
					}
				}
				if(this.Schema_url != null)
				{
					foreach(var item2 in this.Schema_url)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/url", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_aggregateRating != null)
				{
					foreach(var item2 in this.Schema_aggregateRating)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/aggregateRating", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_productionCompany != null)
				{
					foreach(var item2 in this.Schema_productionCompany)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/productionCompany", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_recordedAt != null)
				{
					foreach(var item2 in this.Schema_recordedAt)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/recordedAt", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_countryOfOrigin != null)
				{
					foreach(var item2 in this.Schema_countryOfOrigin)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/countryOfOrigin", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_duration != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/duration", $"{this.Schema_duration.Value.ToString()}", list, " . ");
				}
				if(this.Schema_inLanguage != null)
				{
					foreach(var item2 in this.Schema_inLanguage)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/inLanguage", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_award != null)
				{
					foreach(var item2 in this.Schema_award)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/award", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_description != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/description", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_description)}\"", list, " . ");
				}
				if(this.Schema_image != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/image", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_image)}\"", list, " . ");
				}
				if(this.Schema_name != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/name", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_name)}\"", list, " . ");
				}
				if(this.Schema_datePublished != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/datePublished", $"{this.Schema_datePublished.ToString("yyyyMMddHHmmss")}", list, " . ");
				}
				if(this.Schema_contentRating != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/contentRating", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_contentRating)}\"", list, " . ");
				}
			if (listaSearch != null && listaSearch.Count > 0)
			{
				foreach(string valorSearch in listaSearch)
				{
					search += $"{valorSearch} ";
				}
			}
			if(!string.IsNullOrEmpty(search))
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/search", $"\"{GenerarTextoSinSaltoDeLinea(search.ToLower())}\"", list, " . ");
			}
			return list;
		}

		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{

			//Insert en la tabla Documento
			string tags = "";
			foreach(string tag in tagList)
			{
				tags += $"{tag}, ";
			}
			if (!string.IsNullOrEmpty(tags))
			{
				tags = tags.Substring(0, tags.LastIndexOf(','));
			}
			string titulo = $"{this.Schema_name.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "#COMILLA#").Replace("|", "#PIPE#")}";
			string descripcion = $"{this.Schema_description.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "#COMILLA#").Replace("|", "#PIPE#")}";
			string tablaDoc = $"'{titulo}', '{descripcion}', '{resourceAPI.GraphsUrl}', '{tags}'";
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>(ResourceID, tablaDoc);

			return valor;
		}

		public override string GetURI(ResourceApi resourceAPI)
		{
			return $"{resourceAPI.GraphsUrl}items/AkpeliculaOntology_{ResourceID}_{ArticleID}";
		}


		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
			resource.Title = this.Schema_name;
		}

		internal void AddResourceDescription(ComplexOntologyResource resource)
		{
			resource.Description = this.Schema_description;
		}




	}
}
