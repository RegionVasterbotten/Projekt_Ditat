using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Ditat.Logic.Managers;
using NLog;

namespace Ditat.Api.Controllers
{
    public class LanguageController : ApiController
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        //GET: api/Language/1
        [Authorize]
        public IHttpActionResult GetLanguage(int id)
        {
            var languageTexts = LangaugeManager.GetLanguage(id);

            return Ok(languageTexts);
        }

        // GET: api/ProductImages
        [Authorize]
        public IHttpActionResult GetLanguages()
        {
            log.Info("GetLanguages called");

            var languageTexts = LangaugeManager.GetLanguages();

            log.Info("GetLanguages return");
            return Ok(languageTexts);
        }
    }
}