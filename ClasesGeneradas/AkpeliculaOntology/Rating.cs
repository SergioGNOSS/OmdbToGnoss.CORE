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

namespace AkpeliculaOntology
{
	[ExcludeFromCodeCoverage]
	public class Rating : GnossOCBase
	{
		public Rating() : base() { } 

		public Rating(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			mGNOSSID = pSemCmsModel.Entity.Uri;
			mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Schema_ratingSource = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/ratingSource"));
			this.Schema_ratingValue = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://schema.org/ratingValue")).Value;
		}

		public virtual string RdfType { get { return "http://schema.org/Rating"; } }
		public virtual string RdfsLabel { get { return "http://schema.org/Rating"; } }
		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Fuente de la calificación")]
		[RDFProperty("http://schema.org/ratingSource")]
		public  string Schema_ratingSource { get; set;}

		[LABEL(LanguageEnum.es,"Puntuación")]
		[RDFProperty("http://schema.org/ratingValue")]
		public  int Schema_ratingValue { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("schema:ratingSource", this.Schema_ratingSource));
			propList.Add(new StringOntologyProperty("schema:ratingValue", this.Schema_ratingValue.ToString()));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
		} 











	}
}
