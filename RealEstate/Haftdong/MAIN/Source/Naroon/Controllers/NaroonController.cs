using System;
using System.Globalization;
using System.Web.Http;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Naroon.Server.Controller;
using JahanJooy.RealEstateAgency.Naroon.Util;

namespace JahanJooy.RealEstateAgency.Naroon.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("files")]
    public class NaroonController : ExtendedApiController
    {
        [ComponentPlug]
        public NaroonUtil NaroonUtil { get; set; }

        [HttpGet, Route("dailyfetch")]
        public void StartFetchingData()
        {
            var currentDate = DateTime.Now.AddDays(-1);
            var calendar = new GregorianCalendar();
            var date = calendar.GetYear(currentDate) +
                       (calendar.GetMonth(currentDate).ToString().Length <= 1
                           ? "-0" + calendar.GetMonth(currentDate)
                           : "-" + calendar.GetMonth(currentDate)) +
                       (calendar.GetDayOfMonth(currentDate).ToString().Length <= 1
                           ? "-0" + calendar.GetDayOfMonth(currentDate)
                           : "-" + calendar.GetDayOfMonth(currentDate));
            NaroonUtil.RetrieveFromNaroon(date, date);
        }

        [HttpGet, Route("startfetching")]
        public void StartFetchingData([FromUri] string fromDate, [FromUri] string toDate)
        {
            NaroonUtil.RetrieveFromNaroon(fromDate, toDate);
        }
    }
}